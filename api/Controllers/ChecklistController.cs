using Inspections.Data;
using Inspections.Models.DTOs.Template;
using Inspections.Models.Entities.Templates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inspections.API.Controllers.Template;

[ApiController]
[Route("api/[controller]")]
public class ChecklistController : ControllerBase
{
    private readonly InspectionDbContext _context;

    public ChecklistController(InspectionDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<ChecklistDto>>> GetChecklists(CancellationToken cancellationToken)
    {
        var checklists = await _context.Checklists
            .AsNoTracking()
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

        return Ok(checklists);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ChecklistDto>> GetChecklistById(int id, CancellationToken cancellationToken)
    {
        var checklist = await _context.Checklists
            .AsNoTracking()
            .Include(c => c.TemplateVersion)
                .ThenInclude(v => v.Template)
            .Include(c => c.FieldValues)
                .ThenInclude(fv => fv.TemplateField)
            .SingleOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (checklist is null)
        {
            return NotFound();
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

        return Ok(dto);
    }

    [HttpGet("inspection/{inspectionId:int}")]
    public async Task<ActionResult<List<ChecklistDto>>> GetChecklistsByInspection(int inspectionId, CancellationToken cancellationToken)
    {
        var checklists = await _context.Checklists
            .AsNoTracking()
            .Where(c => c.InspectionId == inspectionId)
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

        return Ok(checklists);
    }

    [HttpPost]
    public async Task<ActionResult<ChecklistDto>> CreateChecklist([FromBody] CreateChecklistDto request, CancellationToken cancellationToken)
    {
        if (request.InspectionId <= 0)
        {
            return BadRequest("InspectionId is required.");
        }

        if (request.TemplateVersionId <= 0)
        {
            return BadRequest("TemplateVersionId is required.");
        }

        var templateVersion = await _context.TemplateVersions
            .Include(v => v.Fields)
            .SingleOrDefaultAsync(v => v.Id == request.TemplateVersionId, cancellationToken);

        if (templateVersion is null)
        {
            return NotFound("Template version not found.");
        }

        var checklist = new Checklist
        {
            InspectionId = request.InspectionId,
            TemplateVersionId = templateVersion.Id,
            CreatedAt = DateTime.UtcNow,
            TemplateVersion = templateVersion
        };

        _context.Checklists.Add(checklist);
        await _context.SaveChangesAsync(cancellationToken);

        var fieldValues = templateVersion.Fields
            .OrderBy(f => f.DisplayOrder)
            .Select(field => new ChecklistFieldValue
            {
                ChecklistId = checklist.Id,
                TemplateFieldId = field.Id,
                Value = null,
                Checklist = checklist,
                TemplateField = field
            })
            .ToList();

        _context.ChecklistFieldValues.AddRange(fieldValues);
        await _context.SaveChangesAsync(cancellationToken);

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

        return CreatedAtAction(nameof(GetChecklistById), new { id = checklist.Id }, dto);
    }
}
