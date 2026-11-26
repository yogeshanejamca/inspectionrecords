using Inspections.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inspections.Data.Repositories;

public class InspectionRepository : IInspectionRepository
{
    private readonly InspectionDbContext _context;

    public InspectionRepository(InspectionDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Inspection>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Inspections
            .AsNoTracking()
            .OrderByDescending(i => i.ScheduledAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Inspection?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Inspections.FindAsync([id], cancellationToken);
    }

    public async Task<Inspection> AddAsync(Inspection inspection, CancellationToken cancellationToken = default)
    {
        _context.Inspections.Add(inspection);
        await _context.SaveChangesAsync(cancellationToken);
        return inspection;
    }

    public async Task<Inspection?> UpdateAsync(Inspection inspection, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Inspections.FindAsync([inspection.Id], cancellationToken);
        if (existing is null)
        {
            return null;
        }

        var previousStatus = existing.Status;

        existing.Title = inspection.Title;
        existing.Description = inspection.Description;
        existing.Status = inspection.Status;
        existing.ScheduledAt = inspection.ScheduledAt;
        existing.UpdatedAt = DateTime.UtcNow;

        // If status changed to Paused, create a new InspectionPause
        if (!string.Equals(previousStatus, inspection.Status, StringComparison.OrdinalIgnoreCase)
            && string.Equals(inspection.Status, InspectionStatus.Paused, StringComparison.OrdinalIgnoreCase))
        {
            var pause = new InspectionPause
            {
                InspectionId = existing.Id,
                StartedAt = DateTime.UtcNow
            };

            _context.Add(pause);
        }

        // If status changed from Paused to something else, end the latest open pause
        if (!string.Equals(previousStatus, inspection.Status, StringComparison.OrdinalIgnoreCase)
            && string.Equals(previousStatus, InspectionStatus.Paused, StringComparison.OrdinalIgnoreCase)
            && !string.Equals(inspection.Status, InspectionStatus.Paused, StringComparison.OrdinalIgnoreCase))
        {
            var openPause = await _context.InspectionPauses
                .Where(p => p.InspectionId == existing.Id && p.EndedAt == null)
                .OrderByDescending(p => p.StartedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (openPause is not null)
            {
                openPause.EndedAt = DateTime.UtcNow;
                openPause.Duration = openPause.EndedAt.Value - openPause.StartedAt;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Inspections.FindAsync([id], cancellationToken);
        if (existing is null)
        {
            return false;
        }

        _context.Inspections.Remove(existing);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<InspectionPause> AddPauseAsync(InspectionPause pause, CancellationToken cancellationToken = default)
    {
        _context.InspectionPauses.Add(pause);
        await _context.SaveChangesAsync(cancellationToken);
        return pause;
    }

    public async Task<InspectionPause?> EndPauseAsync(int inspectionId, DateTime endedAt, CancellationToken cancellationToken = default)
    {
        var openPause = await _context.InspectionPauses
            .Where(p => p.InspectionId == inspectionId && p.EndedAt == null)
            .OrderByDescending(p => p.StartedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (openPause is null)
            return null;

        openPause.EndedAt = endedAt;
        openPause.Duration = openPause.EndedAt.Value - openPause.StartedAt;
        await _context.SaveChangesAsync(cancellationToken);
        return openPause;
    }

    public async Task<IReadOnlyList<InspectionPause>> GetPausesAsync(int inspectionId, CancellationToken cancellationToken = default)
    {
        return await _context.InspectionPauses
            .AsNoTracking()
            .Where(p => p.InspectionId == inspectionId)
            .OrderByDescending(p => p.StartedAt)
            .ToListAsync(cancellationToken);
    }
}
