using AssignmentService.Application.DTOs;
using AssignmentService.Application.Interfaces;
using AssignmentService.Domain.Entities;

namespace AssignmentService.Application.Services;

public class EvaluationService
{
    private readonly IEvaluationRepository _evaluationRepository;
    private readonly ISubmissionRepository _submissionRepository;

    public EvaluationService(
        IEvaluationRepository evaluationRepository,
        ISubmissionRepository submissionRepository)
    {
        _evaluationRepository = evaluationRepository;
        _submissionRepository = submissionRepository;
    }

    public async Task<Guid> UpsertEvaluationAsync(Guid submissionId, UpsertEvaluationDto dto)
    {

        var submission = await _submissionRepository.GetByIdWithAssignmentAsync(submissionId);
        if (submission == null)
        {
            throw new KeyNotFoundException($"Oddaja z ID-jem '{submissionId}' ne obstaja.");
        }

        if (dto.Score > submission.Assignment.MaxScore)
        {
            throw new InvalidOperationException($"Ocena ({dto.Score}) ne more presegati maksimalnega števila točk naloge ({submission.Assignment.MaxScore}).");
        }

        var existingEvaluation = await _evaluationRepository.GetBySubmissionIdAsync(submissionId);

        if (existingEvaluation != null)
        {
            // Posodobimo obstoječo oceno
            existingEvaluation.Update(dto.Score, dto.Feedback);
            await _evaluationRepository.SaveChangesAsync();
            return existingEvaluation.Id;
        }
        else
        {
            // Ustvarimo novo oceno
            var evaluation = new Evaluation(submissionId, dto.Score, dto.Feedback);
            await _evaluationRepository.AddAsync(evaluation);
            await _evaluationRepository.SaveChangesAsync();
            return evaluation.Id;
        }
    }

    public async Task<EvaluationDto?> GetEvaluationBySubmissionIdAsync(Guid submissionId)
    {
        var evaluation = await _evaluationRepository.GetBySubmissionIdAsync(submissionId);
        if (evaluation == null) return null;

        return new EvaluationDto(
            evaluation.Id,
            evaluation.SubmissionId,
            evaluation.Score,
            evaluation.Feedback,
            evaluation.EvaluatedAt
        );
    }

    public async Task<bool> DeleteEvaluationAsync(Guid submissionId)
    {
        var evaluation = await _evaluationRepository.GetBySubmissionIdAsync(submissionId);
        if (evaluation == null) return false;

        await _evaluationRepository.DeleteAsync(evaluation);
        await _evaluationRepository.SaveChangesAsync();
        return true;
    }
}