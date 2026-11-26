using Inspections.Data;
using Inspections.Data.Repositories;
using Inspections.Models.DTOs;
using Inspections.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Inspections.Service;

public class StateRecordService : IStateRecordService
{
    private readonly IStateRecordRepository _repository;
    private readonly RecordsDbContext _recordsDbContext;

    public StateRecordService(IStateRecordRepository repository, RecordsDbContext recordsDbContext)
    {
        _repository = repository;
        _recordsDbContext = recordsDbContext;
    }

    public async Task<IReadOnlyList<StateRecordDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var records = await _repository.GetAllAsync(cancellationToken);
        return records.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<StateMapDto>> GetMapLocationsAsync(CancellationToken cancellationToken = default)
    {
        var maps = await _recordsDbContext.StateMaps
            .AsNoTracking()
            .OrderBy(map => map.Name)
            .ToListAsync(cancellationToken);

        return maps.Select(map => new StateMapDto(
            map.Id,
            map.Name,
            map.Icon,
            map.X,
            map.Y,
            map.Description,
            map.Color)).ToList();
    }

    public async Task<IReadOnlyDictionary<string, StateDataDto>> GetStatesDataAsync(CancellationToken cancellationToken = default)
    {
        var records = await _repository.GetAllAsync(cancellationToken);
        var recordsByState = records.GroupBy(r => r.StateId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var stateDefinitions = await _recordsDbContext.StateDefinitions
            .AsNoTracking()
            .OrderBy(state => state.Name)
            .ToListAsync(cancellationToken);

        return stateDefinitions.ToDictionary(
            state => state.Id,
            state =>
            {
                var places = recordsByState.TryGetValue(state.Id, out var stateRecords)
                    ? stateRecords.Select(record => new StatePlaceDto(
                        record.Id.ToString(),
                        record.Name,
                        record.Icon,
                        record.Description,
                        record.Points,
                        record.Visited,
                        record.Distance)).ToList()
                    : new List<StatePlaceDto>();

                return new StateDataDto(
                    state.Id,
                    state.Name,
                    state.Emoji,
                    places.Where(place => place.Visited).Sum(place => place.Distance),
                    places.Where(place => place.Visited).Sum(place => place.Points),
                    places);
            });
    }

    public async Task<StateRecordDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var record = await _repository.GetByIdAsync(id, cancellationToken);
        return record is null ? null : MapToDto(record);
    }

    public async Task<StateRecordDto> CreateAsync(CreateStateRecordRequest request, CancellationToken cancellationToken = default)
    {
        var stateRecord = new StateRecord
        {
            Id = Guid.NewGuid(),
            StateId = request.StateId,
            Name = request.Name,
            Icon = request.Icon,
            Description = request.Description,
            Points = request.Points,
            Visited = request.Visited,
            Distance = request.Distance,
            ImageUrl = null,
            ImageFileName = null,
            ImageUploadedAt = null
        };

        var created = await _repository.AddAsync(stateRecord, cancellationToken);
        return MapToDto(created);
    }

    public async Task<StateRecordDto?> UpdateAsync(Guid id, UpdateStateRecordRequest request, CancellationToken cancellationToken = default)
    {
        var stateRecord = new StateRecord
        {
            Id = id,
            StateId = request.StateId,
            Name = request.Name,
            Icon = request.Icon,
            Description = request.Description,
            Points = request.Points,
            Visited = request.Visited,
            Distance = request.Distance
        };

        var updated = await _repository.UpdateAsync(stateRecord, cancellationToken);
        return updated is null ? null : MapToDto(updated);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _repository.DeleteAsync(id, cancellationToken);
    }

    private static StateRecordDto MapToDto(StateRecord stateRecord) =>
        new(
            stateRecord.Id,
            stateRecord.Name,
            stateRecord.Icon,
            stateRecord.Description,
            stateRecord.Points,
            stateRecord.Visited,
            stateRecord.Distance,
            stateRecord.ImageUrl,
            stateRecord.ImageFileName,
            stateRecord.ImageUploadedAt);


    public async Task<bool> UpdateImageInfoAsync(
        Guid id,
        string imageUrl,
        string imageFileName,
        CancellationToken cancellationToken = default)
    {
        var stateRecord = await _repository.GetByIdAsync(id, cancellationToken);
        if (stateRecord == null)
        {
            return false;
        }

        stateRecord.ImageUrl = imageUrl;
        stateRecord.ImageFileName = imageFileName;
        stateRecord.ImageUploadedAt = DateTime.UtcNow;

        var updated = await _repository.UpdateAsync(stateRecord, cancellationToken);
        return updated != null;
    }
}