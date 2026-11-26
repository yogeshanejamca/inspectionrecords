using Inspections.Models.DTOs.Template;
using Inspections.Models.Entities.Templates;
using Inspections.Service;
using Microsoft.EntityFrameworkCore;

namespace Inspections.Data.Repositories;

internal sealed class TemplateRepository(InspectionDbContext db) : ITemplateRepository
{
    public Task<List<TemplateDto>> ListAsync(CancellationToken cancellationToken) =>
        db.Templates
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .Select(t => new TemplateDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                IsActive = t.IsActive,
                CreatedAt = t.CreatedAt,
                VersionCount = t.Versions.Count
            })
            .ToListAsync(cancellationToken);

    public Task<Template?> GetDetailsAsync(int id, CancellationToken token) =>
        db.Templates.AsNoTracking().Include(template => template.Versions)
            .ThenInclude(version => version.Fields).SingleOrDefaultAsync(template => template.Id == id, token);

    public Task<Template?> FindAsync(int id, CancellationToken token) =>
        db.Templates.SingleOrDefaultAsync(template => template.Id == id, token);

    public Task<bool> IsActiveAsync(int id, CancellationToken token) =>
        db.Templates.AnyAsync(template => template.Id == id && template.IsActive, token);

    public async Task<bool> IsInUseAsync(int id, CancellationToken token) =>
        await db.Inspections.AnyAsync(item => item.TemplateId == id, token) ||
        await db.Checklists.AnyAsync(item => item.TemplateVersion.TemplateId == id, token);

    public Task SaveAsync(CancellationToken token) => db.SaveChangesAsync(token);

    public async Task AddAsync(Template template, TemplateVersion version, CancellationToken token)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(token);
        var fields = version.Fields.ToArray();
        version.Fields.Clear();
        db.Templates.Add(template);
        await db.SaveChangesAsync(token);
        foreach (var field in fields)
        {
            field.TemplateId = template.Id;
            version.Fields.Add(field);
        }
        await db.SaveChangesAsync(token);
        await transaction.CommitAsync(token);
    }

    public async Task DeleteAsync(Template template, CancellationToken token)
    {
        db.Templates.Remove(template);
        await db.SaveChangesAsync(token);
    }
}
