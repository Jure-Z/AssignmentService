using AssignmentService.Domain.Entities;

namespace AssignmentService.Application.Interfaces;

public interface IEvaluationRepository
{
    Task<Evaluation?> GetByIdAsync(Guid id);
    Task<Evaluation?> GetBySubmissionIdAsync(Guid submissionId);
    Task AddAsync(Evaluation evaluation);
    Task SaveChangesAsync();
    Task DeleteAsync(Evaluation evaluation);
}
