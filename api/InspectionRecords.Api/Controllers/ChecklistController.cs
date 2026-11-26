using Inspections.Models.DTOs.Template;
using Inspections.Service;
using Microsoft.AspNetCore.Mvc;

namespace Inspections.API.Controllers.Template;

[ApiController]
[Route("api/[controller]")]
public class ChecklistController(ChecklistService checklists) : ControllerBase
{
    [HttpGet]
    public Task<List<ChecklistDto>> GetChecklists(CancellationToken token) => checklists.GetChecklists(token);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ChecklistDto>> GetChecklistById(int id, CancellationToken token) =>
        await checklists.GetChecklistById(id, token) is { } checklist ? Ok(checklist) : NotFound();

    [HttpGet("inspection/{inspectionId:int}")]
    public Task<List<ChecklistDto>> GetChecklistsByInspection(int inspectionId, CancellationToken token) =>
        checklists.GetChecklistsByInspection(inspectionId, token);

    [HttpPost]
    public async Task<ActionResult<ChecklistDto>> CreateChecklist(CreateChecklistDto request, CancellationToken token)
    {
        if (request.InspectionId <= 0) return BadRequest("InspectionId is required.");
        if (request.TemplateVersionId <= 0) return BadRequest("TemplateVersionId is required.");

        var created = await checklists.CreateChecklist(request, token);
        return created is null
            ? NotFound("Inspection or template version not found.")
            : CreatedAtAction(nameof(GetChecklistById), new { id = created.Id }, created);
    }
}
