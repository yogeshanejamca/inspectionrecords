using Inspections.Data;
using Inspections.Models.DTOs;
using Inspections.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inspections.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InspectionsController(IInspectionService inspectionService, InspectionDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<InspectionDto>>> GetAll(CancellationToken cancellationToken)
    {
        var inspections = await inspectionService.GetAllAsync(cancellationToken);
        return Ok(inspections);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<InspectionDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var inspection = await inspectionService.GetByIdAsync(id, cancellationToken);
        if (inspection is null)
        {
            return NotFound();
        }

        return Ok(inspection);
    }
    [HttpGet("{id:int}/pauses")]
    public async Task<ActionResult<IReadOnlyList<object>>> GetPauses(int id, CancellationToken cancellationToken)
    {
        var pauses = await inspectionService.GetPausesAsync(id, cancellationToken);
        if (pauses is null || pauses.Count == 0)
            return Ok(Array.Empty<object>());

        var result = pauses.Select(p => new
        {
            p.Id,
            p.InspectionId,
            p.StartedAt,
            p.EndedAt,
            DurationSeconds = p.Duration?.TotalSeconds
        });

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<InspectionDto>> Create(
        [FromBody] CreateInspectionRequest request,
        CancellationToken cancellationToken)
    {
        if (request.ScheduledAt == default)
            return BadRequest("A scheduled date is required.");
        if (request.TemplateId is int templateId &&
            !await db.Templates.AnyAsync(template => template.Id == templateId && template.IsActive, cancellationToken))
            return BadRequest("Select an active template.");

        var created = await inspectionService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<InspectionDto>> Update(
        int id,
        [FromBody] UpdateInspectionRequest request,
        CancellationToken cancellationToken)
    {
        if (request.ScheduledAt == default)
            return BadRequest("A scheduled date is required.");

        var updated = await inspectionService.UpdateAsync(id, request, cancellationToken);
        if (updated is null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await inspectionService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

}
