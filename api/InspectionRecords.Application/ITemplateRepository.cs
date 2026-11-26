using Inspections.Models.DTOs.Template;
using Inspections.Models.Entities.Templates;

namespace Inspections.Service;

public interface ITemplateRepository
{
    Task<List<TemplateDto>> ListAsync(CancellationToken token);
    Task<Template?> GetDetailsAsync(int id, CancellationToken token);
    Task<Template?> FindAsync(int id, CancellationToken token);
    Task<bool> IsActiveAsync(int id, CancellationToken token);
    Task<bool> IsInUseAsync(int id, CancellationToken token);
    Task SaveAsync(CancellationToken token);
    Task AddAsync(Template template, TemplateVersion version, CancellationToken token);
    Task DeleteAsync(Template template, CancellationToken token);
}

public enum TemplateDeletion { Missing, InUse, Deleted }
