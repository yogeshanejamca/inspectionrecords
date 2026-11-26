using Inspections.Models.Entities;

namespace Inspections.Data.Repositories;

public interface IStateRecordRepository
{
    Task<List<StateMap>> GetMapsAsync(CancellationToken cancellationToken);
    Task<List<StateDefinition>> GetDefinitionsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<StateRecord>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<StateRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<StateRecord> AddAsync(StateRecord stateRecord, CancellationToken cancellationToken = default);
    Task<StateRecord?> UpdateAsync(StateRecord stateRecord, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
