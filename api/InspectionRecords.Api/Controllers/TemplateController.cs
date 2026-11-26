using Inspections.Models.DTOs.Template;
using Inspections.Service;
using Microsoft.AspNetCore.Mvc;

namespace Inspections.API.Controllers.Template;

[ApiController]
[Route("api/[controller]")]
public class TemplateController(TemplateService templates) : ControllerBase
{
    [HttpGet]
    public Task<List<TemplateDto>> GetTemplates(CancellationToken token) => templates.GetTemplates(token);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TemplateDetailDto>> GetTemplate(int id, CancellationToken token) =>
        await templates.GetTemplate(id, token) is { } template ? Ok(template) : NotFound();

    [HttpPost]
    public async Task<ActionResult<TemplateDto>> CreateTemplate(CreateTemplateDto request, CancellationToken token)
    {
        var created = await templates.CreateTemplate(request, User.Identity?.Name ?? "system", token);
        return created is null
            ? BadRequest("Template name is required.")
            : CreatedAtAction(nameof(GetTemplate), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTemplate(int id, UpdateTemplateDto request, CancellationToken token) =>
        await templates.UpdateTemplate(id, request, token) ? NoContent() : NotFound();

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTemplate(int id, CancellationToken token) =>
        await templates.DeleteTemplate(id, token) switch
        {
            TemplateDeletion.Deleted => NoContent(),
            TemplateDeletion.InUse => Conflict("This template is already in use. Deactivate it instead."),
            _ => NotFound()
        };
}
