using AssignmentService.Domain.Entities;

namespace AssignmentService.Application.Interfaces;

public interface IAssignmentRepository
{
    Task<Assignment?> GetByIdAsync(Guid id);
    Task<IEnumerable<Assignment>> GetAllAsync();
    Task AddAsync(Assignment assignment);
    Task SaveChangesAsync();
    Task DeleteAsync(Assignment assignment);
}