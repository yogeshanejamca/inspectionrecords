using Inspections.Models.DTOs.Template;
using Inspections.Models.Entities.Templates;
using Inspections.Service;
using Microsoft.EntityFrameworkCore;

namespace Inspections.Data.Repositories;

internal sealed class ChecklistRepository(InspectionDbContext db) : IChecklistRepository
{
    public Task<List<ChecklistDto>> ListAsync(int? inspectionId, CancellationToken cancellationToken) =>
        db.Checklists
            .AsNoTracking()
            .Where(item => inspectionId == null || item.InspectionId == inspectionId)
            .Include(c => c.TemplateVersion)
                .ThenInclude(v => v.Template)
            .Include(c => c.FieldValues)
                .ThenInclude(fv => fv.TemplateField)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new ChecklistDto
            {
                Id = c.Id,
                InspectionId = c.InspectionId,
                TemplateId = c.TemplateVersion.TemplateId,
                TemplateName = c.TemplateVersion.Template.Name,
                Fields = c.FieldValues
                    .OrderBy(fv => fv.TemplateField.DisplayOrder)
                    .Select(fv => new ChecklistFieldValueDto
                    {
                        Id = fv.Id,
                        TemplateFieldId = fv.TemplateFieldId,
                        Name = fv.TemplateField.Name,
                        Label = fv.TemplateField.Label,
                        FieldType = fv.TemplateField.FieldType,
                        IsRequired = fv.TemplateField.IsRequired,
                        DisplayOrder = fv.TemplateField.DisplayOrder,
                        Value = fv.Value
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);

    public Task<Checklist?> FindAsync(int id, CancellationToken token) =>
        db.Checklists.AsNoTracking().Include(item => item.TemplateVersion).ThenInclude(version => version.Template)
            .Include(item => item.FieldValues).ThenInclude(value => value.TemplateField)
            .SingleOrDefaultAsync(item => item.Id == id, token);

    public Task<bool> InspectionExistsAsync(int id, CancellationToken token) =>
        db.Inspections.AnyAsync(item => item.Id == id, token);

    public Task<TemplateVersion?> GetTemplateVersionAsync(int id, CancellationToken token) =>
        db.TemplateVersions.Include(version => version.Fields).Include(version => version.Template)
            .SingleOrDefaultAsync(version => version.Id == id, token);

    public async Task AddAsync(Checklist checklist, CancellationToken token)
    {
        db.Checklists.Add(checklist);
        await db.SaveChangesAsync(token);
    }
}
