using AssignmentService.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssignmentsController : ControllerBase
{
    private readonly AssignmentService.Application.Services.AssignmentService _assignmentService;

    public AssignmentsController(AssignmentService.Application.Services.AssignmentService assignmentService)
    {
        _assignmentService = assignmentService;
    }

    // GET: api/Assignments
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AssignmentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AssignmentDto>>> GetAll()
    {
        var assignments = await _assignmentService.GetAllAssignmentsAsync();
        return Ok(assignments);
    }

    // GET: api/Assignments/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AssignmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AssignmentDto>> GetById(Guid id)
    {
        var assignment = await _assignmentService.GetAssignmentByIdAsync(id);
        if (assignment == null)
        {
            return NotFound();
        }

        return Ok(assignment);
    }

    // POST: api/Assignments
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateAssignmentDto dto)
    {
        var id = await _assignmentService.CreateAssignmentAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id }, new { Id = id });
    }

    // PUT: api/Assignments/{id}
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAssignmentDto dto)
    {
        var success = await _assignmentService.UpdateAssignmentAsync(id, dto);
        if (!success)
        {
            return NotFound();
        }

        return NoContent(); // Status 204 ob uspešni posodobitvi
    }

    // DELETE: api/Assignments/{id}
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _assignmentService.DeleteAssignmentAsync(id);
        if (!success)
        {
            return NotFound();
        }

        return NoContent(); // Status 204 ob uspešnem brisanju
    }


}