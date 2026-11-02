using AssignmentService.Application.DTOs;
using AssignmentService.Application.Interfaces;
using AssignmentService.Domain.Entities;

namespace AssignmentService.Application.Services;

public class SubmissionService
{
    private readonly ISubmissionRepository _submissionRepository;
    private readonly IAssignmentRepository _assignmentRepository;

    public SubmissionService(
        ISubmissionRepository submissionRepository,
        IAssignmentRepository assignmentRepository)
    {
        _submissionRepository = submissionRepository;
        _assignmentRepository = assignmentRepository;
    }

    public async Task<Guid> CreateSubmissionAsync(CreateSubmissionDto dto)
    {
        // Preveri ali naloga obstaja
        var assignment = await _assignmentRepository.GetByIdAsync(dto.AssignmentId);
        if (assignment == null)
        {
            throw new KeyNotFoundException($"Naloga z ID-jem '{dto.AssignmentId}' ne obstaja.");
        }

        // Preverimo, če je študent že ustvaril oddajo za to nalogo
        var existingSubmission = await _submissionRepository.GetByAssignmentAndStudentAsync(dto.AssignmentId, dto.StudentId);
        if (existingSubmission != null)
        {
            throw new InvalidOperationException("Študent je za to nalogo že ustvaril oddajo.");
        }

        var submission = new Submission(dto.AssignmentId, dto.StudentId, dto.Content);

        await _submissionRepository.AddAsync(submission);
        await _submissionRepository.SaveChangesAsync();

        return submission.Id;
    }

    public async Task<SubmissionDto?> GetSubmissionByIdAsync(Guid id)
    {
        var submission = await _submissionRepository.GetByIdAsync(id);
        if (submission == null) return null;

        return MapToDto(submission);
    }

    public async Task<IEnumerable<SubmissionDto>> GetSubmissionsByAssignmentIdAsync(Guid assignmentId)
    {
        var submissions = await _submissionRepository.GetByAssignmentIdAsync(assignmentId);
        return submissions.Select(MapToDto);
    }

    public async Task<bool> UpdateSubmissionAsync(Guid id, UpdateSubmissionDto dto)
    {
        var submission = await _submissionRepository.GetByIdAsync(id);
        if (submission == null) return false;

        submission.Update(dto.Content);

        await _submissionRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SubmitAssignmentAsync(Guid id)
    {
        var submission = await _submissionRepository.GetByIdWithAssignmentAsync(id);
        if (submission == null) return false;

        submission.Submit();

        await _submissionRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteSubmissionAsync(Guid id)
    {
        var submission = await _submissionRepository.GetByIdAsync(id);
        if (submission == null) return false;

        await _submissionRepository.DeleteAsync(submission);
        await _submissionRepository.SaveChangesAsync();
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