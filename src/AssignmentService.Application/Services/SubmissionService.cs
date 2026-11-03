using AssignmentService.Application.DTOs;
using AssignmentService.Application.Interfaces;
using AssignmentService.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace AssignmentService.Application.Services;

public class SubmissionService
{
    private readonly ISubmissionRepository _submissionRepository;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly ILogger<SubmissionService> _logger;

    public SubmissionService(
        ISubmissionRepository submissionRepository,
        IAssignmentRepository assignmentRepository,
        ILogger<SubmissionService> logger)
    {
        _submissionRepository = submissionRepository;
        _assignmentRepository = assignmentRepository;
        _logger = logger;
    }

    public async Task<Guid> CreateSubmissionAsync(CreateSubmissionDto dto)
    {
        _logger.LogInformation("Začetek postopka ustvarjanja oddaje za študenta {StudentId} in nalogo {AssignmentId}", dto.StudentId, dto.AssignmentId);

        // Preveri ali naloga obstaja
        var assignment = await _assignmentRepository.GetByIdAsync(dto.AssignmentId);
        if (assignment == null)
        {
            _logger.LogWarning("Poskus ustvarjanja oddaje za neobstoječo nalogo z ID-jem {AssignmentId}", dto.AssignmentId);
            throw new KeyNotFoundException($"Naloga z ID-jem '{dto.AssignmentId}' ne obstaja.");
        }

        // Preverimo, če je študent že ustvaril oddajo za to nalogo
        var existingSubmission = await _submissionRepository.GetByAssignmentAndStudentAsync(dto.AssignmentId, dto.StudentId);
        if (existingSubmission != null)
        {
            _logger.LogWarning("Študent {StudentId} je že poskušal ponovno ustvariti oddajo za nalogo {AssignmentId}", dto.StudentId, dto.AssignmentId);
            throw new InvalidOperationException("Študent je za to nalogo že ustvaril oddajo.");
        }

        var submission = new Submission(dto.AssignmentId, dto.StudentId, dto.Content);

        await _submissionRepository.AddAsync(submission);
        await _submissionRepository.SaveChangesAsync();

        _logger.LogInformation("Uspešno ustvarjena oddaja z ID-jem {SubmissionId} za študenta {StudentId}", submission.Id, dto.StudentId);

        return submission.Id;
    }

    public async Task<SubmissionDto?> GetSubmissionByIdAsync(Guid id)
    {
        _logger.LogInformation("Pridobivanje oddaje z ID-jem {SubmissionId}", id);

        var submission = await _submissionRepository.GetByIdAsync(id);
        if (submission == null)
        {
            _logger.LogWarning("Oddaja z ID-jem {SubmissionId} ni bila najdena", id);
            return null;
        }

        return MapToDto(submission);
    }

    public async Task<IEnumerable<SubmissionDto>> GetSubmissionsByAssignmentIdAsync(Guid assignmentId)
    {
        _logger.LogInformation("Pridobivanje seznama vseh oddaj za nalogo z ID-jem {AssignmentId}", assignmentId);

        var submissions = await _submissionRepository.GetByAssignmentIdAsync(assignmentId);
        return submissions.Select(MapToDto);
    }

    public async Task<bool> UpdateSubmissionAsync(Guid id, UpdateSubmissionDto dto)
    {
        _logger.LogInformation("Začetek posodabljanja oddaje z ID-jem {SubmissionId}", id);

        var submission = await _submissionRepository.GetByIdAsync(id);
        if (submission == null)
        {
            _logger.LogWarning("Poskus posodobitve neobstoječe oddaje z ID-jem {SubmissionId}", id);
            return false;
        }

        submission.Update(dto.Content);

        await _submissionRepository.SaveChangesAsync();

        _logger.LogInformation("Uspešno posodobljena vsebina oddaje z ID-jem {SubmissionId}", id);

        return true;
    }

    public async Task<bool> SubmitAssignmentAsync(Guid id)
    {
        _logger.LogInformation("Začetek oddaje (submit) naloge za oddajo z ID-jem {SubmissionId}", id);

        var submission = await _submissionRepository.GetByIdWithAssignmentAsync(id);
        if (submission == null)
        {
            _logger.LogWarning("Poskus oddaje neobstoječe oddaje z ID-jem {SubmissionId}", id);
            return false;
        }

        submission.Submit();

        await _submissionRepository.SaveChangesAsync();

        _logger.LogInformation("Oddaja z ID-jem {SubmissionId} je bila uspešno oddana", id);

        return true;
    }

    public async Task<bool> DeleteSubmissionAsync(Guid id)
    {
        _logger.LogInformation("Začetek brisanja oddaje z ID-jem {SubmissionId}", id);

        var submission = await _submissionRepository.GetByIdAsync(id);
        if (submission == null)
        {
            _logger.LogWarning("Poskus brisanja neobstoječe oddaje z ID-jem {SubmissionId}", id);
            return false;
        }

        await _submissionRepository.DeleteAsync(submission);
        await _submissionRepository.SaveChangesAsync();

        _logger.LogInformation("Uspešno izbrisana oddaja z ID-jem {SubmissionId}", id);

        return true;
    }

    // Pomožna metoda za mapiranje entitete v DTO
    private static SubmissionDto MapToDto(Submission s)
    {
        return new SubmissionDto(
            s.Id,
            s.AssignmentId,
            s.StudentId,
            s.Content,
            s.CreatedAt,
            s.SubmittedAt,
            s.UpdatedAt,
            s.Status.ToString(),
            s.Evaluation != null ? new EvaluationDto(s.Evaluation.Id, s.Id, s.Evaluation.Score, s.Evaluation.Feedback, s.Evaluation.EvaluatedAt) : null
        );
    }
}