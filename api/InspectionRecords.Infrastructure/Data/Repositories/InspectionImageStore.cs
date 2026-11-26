using Inspections.Models.Entities;
using Inspections.Service;
using Microsoft.EntityFrameworkCore;

namespace Inspections.Data.Repositories;

internal sealed class InspectionImageStore(InspectionDbContext db) : IInspectionImageStore
{
    public Task<bool> InspectionExistsAsync(int inspectionId, CancellationToken token) =>
        db.Inspections.AnyAsync(item => item.Id == inspectionId, token);

    public async Task AddAsync(InspectionImage image, CancellationToken token)
    {
        db.InspectionImages.Add(image);
        await db.SaveChangesAsync(token);
    }

    public Task<List<string>> ListNamesAsync(int inspectionId, CancellationToken token) =>
        db.InspectionImages.Where(image => image.InspectionId == inspectionId)
            .OrderBy(image => image.Id).Select(image => image.BlobName).ToListAsync(token);

    public Task<InspectionImage?> FindAsync(int inspectionId, string name, CancellationToken token) =>
        db.InspectionImages.AsNoTracking().FirstOrDefaultAsync(
            image => image.InspectionId == inspectionId && image.BlobName == name, token);
}
