using Inspections.Models.Entities;

namespace Inspections.Data.Repositories;

public interface IInspectionRepository
{
    Task<IReadOnlyList<Inspection>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Inspection?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Inspection> AddAsync(Inspection inspection, CancellationToken cancellationToken = default);
    Task<Inspection?> UpdateAsync(Inspection inspection, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    // Pause tracking helpers
    Task<InspectionPause> AddPauseAsync(InspectionPause pause, CancellationToken cancellationToken = default);
    Task<InspectionPause?> EndPauseAsync(int inspectionId, DateTime endedAt, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InspectionPause>> GetPausesAsync(int inspectionId, CancellationToken cancellationToken = default);
}
