using Inspections.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inspections.Data.Repositories;

public class StateRecordRepository : IStateRecordRepository
{
    private readonly RecordsDbContext _context;

    public StateRecordRepository(RecordsDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<StateRecord>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.StateRecords
            .AsNoTracking()
            .OrderBy(record => record.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<StateRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.StateRecords.FindAsync([id], cancellationToken);
    }

    public async Task<StateRecord> AddAsync(StateRecord stateRecord, CancellationToken cancellationToken = default)
    {
        _context.StateRecords.Add(stateRecord);
        await _context.SaveChangesAsync(cancellationToken);
        return stateRecord;
    }

    public async Task<StateRecord?> UpdateAsync(StateRecord stateRecord, CancellationToken cancellationToken = default)
    {
        var existing = await _context.StateRecords.FindAsync([stateRecord.Id], cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.StateId = stateRecord.StateId;
        existing.Name = stateRecord.Name;
        existing.Icon = stateRecord.Icon;
        existing.Description = stateRecord.Description;
        existing.Points = stateRecord.Points;
        existing.Visited = stateRecord.Visited;
        existing.Distance = stateRecord.Distance;

        // Update image fields if provided
        if (stateRecord.ImageUrl != null)
            existing.ImageUrl = stateRecord.ImageUrl;
        if (stateRecord.ImageFileName != null)
            existing.ImageFileName = stateRecord.ImageFileName;
        if (stateRecord.ImageUploadedAt.HasValue)
            existing.ImageUploadedAt = stateRecord.ImageUploadedAt;

        await _context.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existing = await _context.StateRecords.FindAsync([id], cancellationToken);
        if (existing is null)
        {
            return false;
        }

        _context.StateRecords.Remove(existing);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
