using AssignmentService.Application.DTOs;
using AssignmentService.Application.Interfaces;
using AssignmentService.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace AssignmentService.Application.Services;

public class EvaluationService
{
    private readonly IEvaluationRepository _evaluationRepository;
    private readonly ISubmissionRepository _submissionRepository;
    private readonly ILogger<EvaluationService> _logger;

    public EvaluationService(
        IEvaluationRepository evaluationRepository,
        ISubmissionRepository submissionRepository,
        ILogger<EvaluationService> logger)
    {
        _evaluationRepository = evaluationRepository;
        _submissionRepository = submissionRepository;
        _logger = logger;
    }

    public async Task<Guid> UpsertEvaluationAsync(Guid submissionId, UpsertEvaluationDto dto)
    {
        _logger.LogInformation("Začetek ocene za oddajo z ID-jem {SubmissionId} s številom točk {Score}", submissionId, dto.Score);

        var submission = await _submissionRepository.GetByIdWithAssignmentAsync(submissionId);
        if (submission == null)
        {
            _logger.LogWarning("Poskus ocenjevanja neobstoječe oddaje z ID-jem {SubmissionId}", submissionId);
            throw new KeyNotFoundException($"Oddaja z ID-jem '{submissionId}' ne obstaja.");
        }

        if (dto.Score > submission.Assignment.MaxScore)
        {
            _logger.LogWarning("Poskus vnosa previsoke ocene {Score} za oddajo {SubmissionId}. Maksimalno število točk naloge je {MaxScore}",
                dto.Score, submissionId, submission.Assignment.MaxScore);
            throw new InvalidOperationException($"Ocena ({dto.Score}) ne more presegati maksimalnega števila točk naloge ({submission.Assignment.MaxScore}).");
        }

        var existingEvaluation = await _evaluationRepository.GetBySubmissionIdAsync(submissionId);

        if (existingEvaluation != null)
        {
            _logger.LogInformation("Posodabljanje obstoječe ocene z ID-jem {EvaluationId} za oddajo {SubmissionId}", existingEvaluation.Id, submissionId);

            // Posodobimo obstoječo oceno
            existingEvaluation.Update(dto.Score, dto.Feedback);
            await _evaluationRepository.SaveChangesAsync();

            _logger.LogInformation("Uspešno posodobljena ocena z ID-jem {EvaluationId}", existingEvaluation.Id);

            return existingEvaluation.Id;
        }
        else
        {
            _logger.LogInformation("Ustvarjanje nove ocene za oddajo z ID-jem {SubmissionId}", submissionId);

            // Ustvarimo novo oceno
            var evaluation = new Evaluation(submissionId, dto.Score, dto.Feedback);
            await _evaluationRepository.AddAsync(evaluation);
            await _evaluationRepository.SaveChangesAsync();

            _logger.LogInformation("Uspešno ustvarjena nova ocena z ID-jem {EvaluationId} za oddajo {SubmissionId}", evaluation.Id, submissionId);

            return evaluation.Id;
        }
    }

    public async Task<EvaluationDto?> GetEvaluationBySubmissionIdAsync(Guid submissionId)
    {
        _logger.LogInformation("Pridobivanje ocene za oddajo z ID-jem {SubmissionId}", submissionId);

        var evaluation = await _evaluationRepository.GetBySubmissionIdAsync(submissionId);
        if (evaluation == null)
        {
            _logger.LogWarning("Ocena za oddajo z ID-jem {SubmissionId} ni bila najdena", submissionId);
            return null;
        }

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
        _logger.LogInformation("Začetek brisanja ocene za oddajo z ID-jem {SubmissionId}", submissionId);

        var evaluation = await _evaluationRepository.GetBySubmissionIdAsync(submissionId);
        if (evaluation == null)
        {
            _logger.LogWarning("Poskus brisanja neobstoječe ocene za oddajo z ID-jem {SubmissionId}", submissionId);
            return false;
        }

        await _evaluationRepository.DeleteAsync(evaluation);
        await _evaluationRepository.SaveChangesAsync();

        _logger.LogInformation("Uspešno izbrisana ocena z ID-jem {EvaluationId} za oddajo {SubmissionId}", evaluation.Id, submissionId);

        return true;
    }
}