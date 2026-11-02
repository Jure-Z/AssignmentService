using AssignmentService.Domain.Entities;

namespace AssignmentService.Application.Interfaces;

public interface ISubmissionRepository
{
    Task<Submission?> GetByIdAsync(Guid id);
    Task<Submission?> GetByIdWithAssignmentAsync(Guid id);
    Task<IEnumerable<Submission>> GetByAssignmentIdAsync(Guid assignmentId);
    Task<Submission?> GetByAssignmentAndStudentAsync(Guid assignmentId, string studentId);
    Task AddAsync(Submission submission);
    Task SaveChangesAsync();
    Task DeleteAsync(Submission submission);
}