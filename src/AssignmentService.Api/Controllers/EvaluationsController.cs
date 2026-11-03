using AssignmentService.Application.DTOs;
using AssignmentService.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentService.Api.Controllers;

[ApiController]
[Route("api/submissions/{submissionId:guid}/evaluation")]
public class EvaluationsController : ControllerBase
{
    private readonly EvaluationService _evaluationService;

    public EvaluationsController(EvaluationService evaluationService)
    {
        _evaluationService = evaluationService;
    }

    // GET: api/submissions/{submissionId}/evaluation
    [HttpGet]
    [EndpointSummary("Pridobi oceno oddaje")]
    [EndpointDescription("Vrne oceno in povratne informacije za določeno oddajo.")]
    [ProducesResponseType(typeof(EvaluationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EvaluationDto>> GetBySubmission(Guid submissionId)
    {
        var evaluation = await _evaluationService.GetEvaluationBySubmissionIdAsync(submissionId);
        if (evaluation == null)
        {
            return NotFound();
        }

        return Ok(evaluation);
    }

    // POST / PUT (Upsert): api/submissions/{submissionId}/evaluation
    [HttpPost]
    [EndpointSummary("Ustvari ali posodobi oceno (Upsert)")]
    [EndpointDescription("Vpiše novo oceno ali posodobi obstoječo oceno za izbrano oddajo ter preveri mejne točke naloge.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Upsert(Guid submissionId, [FromBody] UpsertEvaluationDto dto)
    {
        var evaluationId = await _evaluationService.UpsertEvaluationAsync(submissionId, dto);
        return Ok(new { Id = evaluationId });
    }

    // DELETE: api/submissions/{submissionId}/evaluation
    [HttpDelete]
    [EndpointSummary("Izbriši oceno oddaje")]
    [EndpointDescription("Izbriše obstoječo oceno, ki je bila pripeta določeni oddaji.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid submissionId)
    {
        var success = await _evaluationService.DeleteEvaluationAsync(submissionId);
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}