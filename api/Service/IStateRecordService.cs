using Inspections.Models.DTOs;

namespace Inspections.Service;

public interface IStateRecordService
{
    Task<IReadOnlyList<StateRecordDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StateMapDto>> GetMapLocationsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyDictionary<string, StateDataDto>> GetStatesDataAsync(CancellationToken cancellationToken = default);
    Task<StateRecordDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<StateRecordDto> CreateAsync(CreateStateRecordRequest request, CancellationToken cancellationToken = default);
    Task<StateRecordDto?> UpdateAsync(Guid id, UpdateStateRecordRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> UpdateImageInfoAsync(Guid id, string imageUrl, string imageFileName, CancellationToken cancellationToken = default);
}
