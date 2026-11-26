using Inspections.Models.DTOs;

namespace Inspections.Service;

public interface IInspectionService
{
    Task<IReadOnlyList<InspectionDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<InspectionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<InspectionDto> CreateAsync(CreateInspectionRequest request, CancellationToken cancellationToken = default);
    Task<InspectionDto?> UpdateAsync(int id, UpdateInspectionRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Inspections.Models.Entities.InspectionPause>> GetPausesAsync(int inspectionId, CancellationToken cancellationToken = default);
}
