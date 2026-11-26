using Inspections.Models.DTOs.Template;
using Inspections.Models.Entities.Templates;

namespace Inspections.Service;

public sealed class ChecklistService(IChecklistRepository repository)
{
    public async Task<List<ChecklistDto>> GetChecklists(CancellationToken cancellationToken)
    {
        var checklists = await repository.ListAsync(null, cancellationToken);

        return checklists;
    }

    public async Task<ChecklistDto?> GetChecklistById(int id, CancellationToken cancellationToken)
    {
        var checklist = await repository.FindAsync(id, cancellationToken);

        if (checklist is null)
        {
            return null;
        }

        var dto = new ChecklistDto
        {
            Id = checklist.Id,
            InspectionId = checklist.InspectionId,
            TemplateId = checklist.TemplateVersion.TemplateId,
            TemplateName = checklist.TemplateVersion.Template.Name,
            Fields = checklist.FieldValues
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
        };

        return dto;
    }

    public async Task<List<ChecklistDto>> GetChecklistsByInspection(int inspectionId, CancellationToken cancellationToken)
    {
        var checklists = await repository.ListAsync(inspectionId, cancellationToken);

        return checklists;
    }

    public async Task<ChecklistDto?> CreateChecklist(CreateChecklistDto request, CancellationToken cancellationToken)
    {
        if (!await repository.InspectionExistsAsync(request.InspectionId, cancellationToken))
            return null;

        var templateVersion = await repository.GetTemplateVersionAsync(request.TemplateVersionId, cancellationToken);

        if (templateVersion is null)
        {
            return null;
        }

        var checklist = new Checklist
        {
            InspectionId = request.InspectionId,
            TemplateVersionId = templateVersion.Id,
            CreatedAt = DateTime.UtcNow,
            TemplateVersion = templateVersion
        };

        var fieldValues = templateVersion.Fields
            .OrderBy(f => f.DisplayOrder)
            .Select(field => new ChecklistFieldValue
            {
                TemplateFieldId = field.Id,
                Value = null,
                Checklist = checklist,
                TemplateField = field
            })
            .ToList();

        checklist.FieldValues = fieldValues;
        await repository.AddAsync(checklist, cancellationToken);

        var dto = new ChecklistDto
        {
            Id = checklist.Id,
            InspectionId = checklist.InspectionId,
            TemplateId = templateVersion.TemplateId,
            TemplateName = templateVersion.Template?.Name ?? string.Empty,
            Fields = fieldValues
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
        };

        return dto;
    }
}
