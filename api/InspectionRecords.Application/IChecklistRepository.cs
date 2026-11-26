using Inspections.Models.DTOs.Template;
using Inspections.Models.Entities.Templates;

namespace Inspections.Service;

public interface IChecklistRepository
{
    Task<List<ChecklistDto>> ListAsync(int? inspectionId, CancellationToken token);
    Task<Checklist?> FindAsync(int id, CancellationToken token);
    Task<bool> InspectionExistsAsync(int id, CancellationToken token);
    Task<TemplateVersion?> GetTemplateVersionAsync(int id, CancellationToken token);
    Task AddAsync(Checklist checklist, CancellationToken token);
}
