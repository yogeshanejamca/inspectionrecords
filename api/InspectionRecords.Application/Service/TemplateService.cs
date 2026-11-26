using Inspections.Models.DTOs.Template;
using TemplateEntity = Inspections.Models.Entities.Templates.Template;
using TemplateVersionEntity = Inspections.Models.Entities.Templates.TemplateVersion;
using TemplateFieldEntity = Inspections.Models.Entities.Templates.TemplateField;

namespace Inspections.Service;

public sealed class TemplateService(ITemplateRepository repository)
{
    public async Task<List<TemplateDto>> GetTemplates(CancellationToken cancellationToken)
    {
        var templates = await repository.ListAsync(cancellationToken);

        return templates;
    }

    public async Task<TemplateDetailDto?> GetTemplate(int id, CancellationToken cancellationToken)
    {
        var template = await repository.GetDetailsAsync(id, cancellationToken);

        if (template is null)
        {
            return null;
        }

        var dto = new TemplateDetailDto
        {
            Id = template.Id,
            Name = template.Name,
            Description = template.Description,
            IsActive = template.IsActive,
            CreatedAt = template.CreatedAt,
            VersionCount = template.Versions.Count,
            Versions = template.Versions
                .OrderByDescending(v => v.VersionNumber)
                .Select(v => new TemplateVersionDto
                {
                    Id = v.Id,
                    TemplateId = v.TemplateId,
                    VersionNumber = v.VersionNumber,
                    CreatedAt = v.CreatedAt,
                    CreatedBy = v.CreatedBy,
                    Fields = v.Fields
                        .OrderBy(f => f.DisplayOrder)
                        .Select(f => new TemplateFieldDto
                        {
                            Id = f.Id,
                            TemplateId = f.TemplateId,
                            TemplateVersionId = f.TemplateVersionId,
                            Name = f.Name,
                            Label = f.Label,
                            FieldType = f.FieldType,
                            IsRequired = f.IsRequired,
                            DisplayOrder = f.DisplayOrder,
                            Options = f.Options
                        })
                        .ToList()
                })
                .ToList()
        };

        return dto;
    }

    public async Task<TemplateDto?> CreateTemplate(CreateTemplateDto request, string createdBy, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return null;
        }

        var template = new TemplateEntity
        {
            Name = request.Name.Trim(),
            Description = request.Description,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        var version = new TemplateVersionEntity
        {
            VersionNumber = 1,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy,
            Template = template
        };

        template.Versions.Add(version);

        foreach (var fieldRequest in request.Fields.OrderBy(f => f.DisplayOrder))
        {
            if (string.IsNullOrWhiteSpace(fieldRequest.Name))
            {
                continue;
            }

            var field = new TemplateFieldEntity
            {
                Name = fieldRequest.Name.Trim(),
                Label = string.IsNullOrWhiteSpace(fieldRequest.Label) ? fieldRequest.Name.Trim() : fieldRequest.Label.Trim(),
                FieldType = fieldRequest.FieldType,
                IsRequired = fieldRequest.IsRequired,
                DisplayOrder = fieldRequest.DisplayOrder,
                Options = fieldRequest.Options,
                TemplateVersion = version
            };

            version.Fields.Add(field);
        }

        await repository.AddAsync(template, version, cancellationToken);

        var created = new TemplateDto
        {
            Id = template.Id,
            Name = template.Name,
            Description = template.Description,
            IsActive = template.IsActive,
            CreatedAt = template.CreatedAt,
            VersionCount = 1
        };

        return created;
    }

    public async Task<bool> UpdateTemplate(int id, UpdateTemplateDto request, CancellationToken cancellationToken)
    {
        var template = await repository.FindAsync(id, cancellationToken);

        if (template is null)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            template.Name = request.Name.Trim();
        }

        if (request.Description is not null)
        {
            template.Description = request.Description;
        }

        if (request.IsActive.HasValue)
        {
            template.IsActive = request.IsActive.Value;
        }

        await repository.SaveAsync(cancellationToken);
        return true;
    }

    public async Task<TemplateDeletion> DeleteTemplate(int id, CancellationToken cancellationToken)
    {
        var template = await repository.FindAsync(id, cancellationToken);

        if (template is null)
        {
            return TemplateDeletion.Missing;
        }

        if (await repository.IsInUseAsync(id, cancellationToken))
        {
            return TemplateDeletion.InUse;
        }

        await repository.DeleteAsync(template, cancellationToken);
        return TemplateDeletion.Deleted;
    }
}
