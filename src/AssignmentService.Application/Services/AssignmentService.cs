using AssignmentService.Application.DTOs;
using AssignmentService.Application.Interfaces;
using AssignmentService.Domain.Entities;

namespace AssignmentService.Application.Services;

public class AssignmentService
{
    private readonly IAssignmentRepository _assignmentRepository;

    public AssignmentService(IAssignmentRepository assignmentRepository)
    {
        _assignmentRepository = assignmentRepository;
    }

    public async Task<Guid> CreateAssignmentAsync(CreateAssignmentDto dto)
    {
        var assignment = new Assignment(dto.Title, dto.Description, dto.Deadline, dto.MaxScore);

        await _assignmentRepository.AddAsync(assignment);
        await _assignmentRepository.SaveChangesAsync();

        return assignment.Id;
    }

    public async Task<IEnumerable<AssignmentDto>> GetAllAssignmentsAsync()
    {
        var assignments = await _assignmentRepository.GetAllAsync();

        return assignments.Select(a => new AssignmentDto(
            a.Id,
            a.Title,
            a.Description,
            a.CreatedAt,
            a.UpdatedAt,
            a.Deadline,
            a.MaxScore
        ));
    }

    public async Task<AssignmentDto?> GetAssignmentByIdAsync(Guid id)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(id);
        if (assignment == null) return null;

        return new AssignmentDto(
            assignment.Id,
            assignment.Title,
            assignment.Description,
            assignment.CreatedAt,
            assignment.UpdatedAt,
            assignment.Deadline,
            assignment.MaxScore
        );
    }

    public async Task<bool> UpdateAssignmentAsync(Guid id, UpdateAssignmentDto dto)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(id);
        if (assignment == null) return false;

        assignment.Update(dto.Title, dto.Description, dto.Deadline, dto.MaxScore);

        await _assignmentRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAssignmentAsync(Guid id)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(id);
        if (assignment == null) return false;

        await _assignmentRepository.DeleteAsync(assignment);

        return true;
    }
}