using AssignmentService.Application.Interfaces;
using AssignmentService.Domain.Entities;
using AssignmentService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AssignmentService.Infrastructure.Repositories;

public class AssignmentRepository : IAssignmentRepository
{
    private readonly AppDbContext _context;

    public AssignmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Assignment?> GetByIdAsync(Guid id)
    {
        return await _context.Assignments
            .Include(a => a.Submissions)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Assignment>> GetAllAsync()
    {
        return await _context.Assignments.ToListAsync();
    }

    public async Task AddAsync(Assignment assignment)
    {
        await _context.Assignments.AddAsync(assignment);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Assignment assignment)
    {
        _context.Assignments.Remove(assignment);
        await _context.SaveChangesAsync();
    }
}