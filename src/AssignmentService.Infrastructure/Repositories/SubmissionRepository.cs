using AssignmentService.Application.Interfaces;
using AssignmentService.Domain.Entities;
using AssignmentService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AssignmentService.Infrastructure.Repositories;

public class SubmissionRepository : ISubmissionRepository
{
    private readonly AppDbContext _context;

    public SubmissionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Submission?> GetByIdAsync(Guid id)
    {
        return await _context.Submissions
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Submission?> GetByIdWithAssignmentAsync(Guid id)
    {
        return await _context.Submissions
            .Include(s => s.Assignment)
            .Include(s => s.Evaluation)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Submission>> GetByAssignmentIdAsync(Guid assignmentId)
    {
        return await _context.Submissions
            .Where(s => s.AssignmentId == assignmentId)
            .ToListAsync();
    }

    public async Task<Submission?> GetByAssignmentAndStudentAsync(Guid assignmentId, string studentId)
    {
        return await _context.Submissions
            .FirstOrDefaultAsync(s => s.AssignmentId == assignmentId && s.StudentId == studentId);
    }

    public async Task AddAsync(Submission submission)
    {
        await _context.Submissions.AddAsync(submission);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public Task DeleteAsync(Submission submission)
    {
        _context.Submissions.Remove(submission);
        return Task.CompletedTask;
    }
}
