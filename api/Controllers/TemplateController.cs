using Inspections.Data;
using Inspections.Models.DTOs.Template;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TemplateEntity = Inspections.Models.Entities.Templates.Template;
using TemplateVersionEntity = Inspections.Models.Entities.Templates.TemplateVersion;
using TemplateFieldEntity = Inspections.Models.Entities.Templates.TemplateField;

namespace Inspections.API.Controllers.Template;

[ApiController]
[Route("api/[controller]")]
public class TemplateController : ControllerBase
{
    private readonly InspectionDbContext _context;

    public TemplateController(InspectionDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<TemplateDto>>> GetTemplates(CancellationToken cancellationToken)
    {
        var templates = await _context.Templates
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

        return Ok(templates);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TemplateDetailDto>> GetTemplate(int id, CancellationToken cancellationToken)
    {
        var template = await _context.Templates
            .AsNoTracking()
            .Include(t => t.Versions)
                .ThenInclude(v => v.Fields)
            .SingleOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (template is null)
        {
            return NotFound();
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

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<TemplateDto>> CreateTemplate([FromBody] CreateTemplateDto request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Template name is required.");
        }

        var template = new TemplateEntity
        {
            Name = request.Name.Trim(),
            Description = request.Description,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _context.Templates.Add(template);
        await _context.SaveChangesAsync(cancellationToken);

        var version = new TemplateVersionEntity
        {
            TemplateId = template.Id,
            VersionNumber = 1,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = User.Identity?.Name ?? "system",
            Template = template
        };

        _context.TemplateVersions.Add(version);
        await _context.SaveChangesAsync(cancellationToken);

        foreach (var fieldRequest in request.Fields.OrderBy(f => f.DisplayOrder))
        {
            if (string.IsNullOrWhiteSpace(fieldRequest.Name))
            {
                continue;
            }

            var field = new TemplateFieldEntity
            {
                TemplateId = template.Id,
                TemplateVersionId = version.Id,
                Name = fieldRequest.Name.Trim(),
                Label = string.IsNullOrWhiteSpace(fieldRequest.Label) ? fieldRequest.Name.Trim() : fieldRequest.Label.Trim(),
                FieldType = fieldRequest.FieldType,
                IsRequired = fieldRequest.IsRequired,
                DisplayOrder = fieldRequest.DisplayOrder,
                Options = fieldRequest.Options,
                TemplateVersion = version
            };

            _context.TemplateFields.Add(field);
        }

        await _context.SaveChangesAsync(cancellationToken);

        var created = new TemplateDto
        {
            Id = template.Id,
            Name = template.Name,
            Description = template.Description,
            IsActive = template.IsActive,
            CreatedAt = template.CreatedAt,
            VersionCount = 1
        };

        return CreatedAtAction(nameof(GetTemplate), new { id = template.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTemplate(int id, [FromBody] UpdateTemplateDto request, CancellationToken cancellationToken)
    {
        var template = await _context.Templates
            .SingleOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (template is null)
        {
            return NotFound();
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

        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTemplate(int id, CancellationToken cancellationToken)
    {
        var template = await _context.Templates
            .SingleOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (template is null)
        {
            return NotFound();
        }

        if (await _context.Inspections.AnyAsync(i => i.TemplateId == id, cancellationToken) ||
            await _context.Checklists.AnyAsync(c => c.TemplateVersion.TemplateId == id, cancellationToken))
        {
            return Conflict("This template is already in use. Deactivate it instead.");
        }

        _context.Templates.Remove(template);
        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
