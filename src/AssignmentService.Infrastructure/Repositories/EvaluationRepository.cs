using AssignmentService.Application.Interfaces;
using AssignmentService.Domain.Entities;
using AssignmentService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AssignmentService.Infrastructure.Repositories;

public class EvaluationRepository : IEvaluationRepository
{
    private readonly AppDbContext _context;

    public EvaluationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Evaluation?> GetByIdAsync(Guid id)
    {
        return await _context.Evaluations
            .Include(e => e.Submission)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Evaluation?> GetBySubmissionIdAsync(Guid submissionId)
    {
        return await _context.Evaluations
            .FirstOrDefaultAsync(e => e.SubmissionId == submissionId);
    }

    public async Task AddAsync(Evaluation evaluation)
    {
        await _context.Evaluations.AddAsync(evaluation);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public Task DeleteAsync(Evaluation evaluation)
    {
        _context.Evaluations.Remove(evaluation);
        return Task.CompletedTask;
    }
}