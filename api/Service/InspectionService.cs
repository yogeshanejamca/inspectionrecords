using Inspections.Data.Repositories;
using Inspections.Models.DTOs;
using Inspections.Models.Entities;

namespace Inspections.Service;

public class InspectionService : IInspectionService
{
    private readonly IInspectionRepository _repository;
    public InspectionService(IInspectionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<InspectionDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var inspections = await _repository.GetAllAsync(cancellationToken);
        return inspections.Select(MapToDto).ToList();
    }

    public async Task<InspectionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var inspection = await _repository.GetByIdAsync(id, cancellationToken);
        return inspection is null ? null : MapToDto(inspection);
    }

    public async Task<InspectionDto> CreateAsync(CreateInspectionRequest request, CancellationToken cancellationToken = default)
    {
        var inspection = new Inspection
        {
            TemplateId = request.TemplateId,
            Title = request.Title.Trim(),
            Description = request.Description,
            ScheduledAt = request.ScheduledAt.ToUniversalTime(),
            Status = InspectionStatus.Scheduled
        };

        var created = await _repository.AddAsync(inspection, cancellationToken);
        return MapToDto(created);
    }

    public async Task<InspectionDto?> UpdateAsync(int id, UpdateInspectionRequest request, CancellationToken cancellationToken = default)
    {
        var inspection = new Inspection
        {
            Id = id,
            Title = request.Title.Trim(),
            Description = request.Description,
            Status = request.Status,
            ScheduledAt = request.ScheduledAt.ToUniversalTime()
        };

        var updated = await _repository.UpdateAsync(inspection, cancellationToken);

        return updated is null ? null : MapToDto(updated);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return _repository.DeleteAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyList<Inspections.Models.Entities.InspectionPause>> GetPausesAsync(int inspectionId, CancellationToken cancellationToken = default)
    {
        return await _repository.GetPausesAsync(inspectionId, cancellationToken);
    }

    private static InspectionDto MapToDto(Inspection inspection) =>
        new(
            inspection.Id,
            inspection.TemplateId,
            inspection.Title,
            inspection.Description,
            inspection.Status,
            DateTime.SpecifyKind(inspection.ScheduledAt, DateTimeKind.Utc),
            DateTime.SpecifyKind(inspection.CreatedAt, DateTimeKind.Utc),
            inspection.UpdatedAt is { } updatedAt ? DateTime.SpecifyKind(updatedAt, DateTimeKind.Utc) : null);
}
