using Inspections.Models.DTOs;
using Inspections.Service;
using Microsoft.AspNetCore.Mvc;


namespace Inspections.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StateRecordController : ControllerBase
{
    private readonly IStateRecordService _stateRecordService;

    public StateRecordController(IStateRecordService stateRecordService)
    {
        _stateRecordService = stateRecordService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StateRecordDto>>> GetAll(CancellationToken cancellationToken)
    {
        var records = await _stateRecordService.GetAllAsync(cancellationToken);
        return Ok(records);
    }

    [HttpGet("map-locations")]
    public async Task<ActionResult<IReadOnlyList<StateMapDto>>> GetMapLocations(CancellationToken cancellationToken)
    {
        var maps = await _stateRecordService.GetMapLocationsAsync(cancellationToken);
        return Ok(maps);
    }

    [HttpGet("states")]
    public async Task<ActionResult<IReadOnlyDictionary<string, StateDataDto>>> GetStates(CancellationToken cancellationToken)
    {
        var states = await _stateRecordService.GetStatesDataAsync(cancellationToken);
        return Ok(states);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<StateRecordDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var record = await _stateRecordService.GetByIdAsync(id, cancellationToken);
        if (record is null)
        {
            return NotFound();
        }

        return Ok(record);
    }

    [HttpPost]
    public async Task<ActionResult<StateRecordDto>> Create(
        [FromBody] CreateStateRecordRequest request,
        CancellationToken cancellationToken)
    {
        var created = await _stateRecordService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<StateRecordDto>> Update(
        Guid id,
        [FromBody] UpdateStateRecordRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _stateRecordService.UpdateAsync(id, request, cancellationToken);
        if (updated is null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _stateRecordService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
