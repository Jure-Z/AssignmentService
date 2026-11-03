using AssignmentService.Application.DTOs;
using AssignmentService.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubmissionsController : ControllerBase
{
    private readonly SubmissionService _submissionService;

    public SubmissionsController(SubmissionService submissionService)
    {
        _submissionService = submissionService;
    }

    // GET: api/Submissions/{id}
    [HttpGet("{id:guid}")]
    [EndpointSummary("Pridobi oddajo po ID-ju")]
    [EndpointDescription("Vrne podrobnosti posamezne oddaje študenta, vključno z vsebino, statusom in povezano oceno.")]
    [ProducesResponseType(typeof(SubmissionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SubmissionDto>> GetById(Guid id)
    {
        var submission = await _submissionService.GetSubmissionByIdAsync(id);
        if (submission == null)
        {
            return NotFound();
        }

        return Ok(submission);
    }

    // GET: api/Submissions/assignment/{assignmentId}
    [HttpGet("assignment/{assignmentId:guid}")]
    [EndpointSummary("Pridobi oddaje za določeno nalogo")]
    [EndpointDescription("Vrne seznam vseh oddaj, ki so bile oddane za specifično nalogo.")]
    [ProducesResponseType(typeof(IEnumerable<SubmissionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SubmissionDto>>> GetByAssignmentId(Guid assignmentId)
    {
        var submissions = await _submissionService.GetSubmissionsByAssignmentIdAsync(assignmentId);
        return Ok(submissions);
    }

    // POST: api/Submissions
    [HttpPost]
    [EndpointSummary("Ustvari nov osnutek oddaje")]
    [EndpointDescription("Ustvari nov osnutek oddaje (Draft) za določenega študenta in nalogo.")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateSubmissionDto dto)
    {
        var id = await _submissionService.CreateSubmissionAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id }, new { Id = id });
    }

    // PUT: api/Submissions/{id}
    [HttpPut("{id:guid}")]
    [EndpointSummary("Posodobi vsebino osnutka oddaje")]
    [EndpointDescription("Omogoča urejanje in posodabljanje vsebine oddaje, dokler je ta še v statusu osnutka (Draft).")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSubmissionDto dto)
    {
        var success = await _submissionService.UpdateSubmissionAsync(id, dto);
        if (!success)
        {
            return NotFound();
        }

        return NoContent(); // Status 204 ob uspešni posodobitvi vsebine
    }

    // POST: api/Submissions/{id}/submit
    [HttpPost("{id:guid}/submit")]
    [EndpointSummary("Oddaj nalogo v pregled")]
    [EndpointDescription("Zaklene osnutek oddaje in preveri rok oddaje. Status se spremeni v Submitted ali Late.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Submit(Guid id)
    {
        var success = await _submissionService.SubmitAssignmentAsync(id);
        if (!success)
        {
            return NotFound();
        }

        return NoContent(); // Status 204 ob uspešni oddaji (zaklepu naloge in preverjanju roka)
    }

    // DELETE: api/Submissions/{id}
    [HttpDelete("{id:guid}")]
    [EndpointSummary("Izbriši oddajo")]
    [EndpointDescription("Izbriše obstoječo oddajo iz sistema, če pogoji to dopuščajo.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _submissionService.DeleteSubmissionAsync(id);
        if (!success)
        {
            return NotFound();
        }

        return NoContent(); // Status 204 ob uspešnem brisanju
    }
}