using Inspections.Models.Entities;

namespace Inspections.Service;

public interface IInspectionImageStore
{
    Task<bool> InspectionExistsAsync(int inspectionId, CancellationToken token);
    Task AddAsync(InspectionImage image, CancellationToken token);
    Task<List<string>> ListNamesAsync(int inspectionId, CancellationToken token);
    Task<InspectionImage?> FindAsync(int inspectionId, string name, CancellationToken token);
}
