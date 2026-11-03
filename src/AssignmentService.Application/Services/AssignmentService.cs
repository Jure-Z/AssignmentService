using AssignmentService.Application.DTOs;
using AssignmentService.Application.Interfaces;
using AssignmentService.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace AssignmentService.Application.Services;

public class AssignmentService
{
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly ILogger<AssignmentService> _logger;

    public AssignmentService(IAssignmentRepository assignmentRepository, ILogger<AssignmentService> logger)
    {
        _assignmentRepository = assignmentRepository;
        _logger = logger;
    }

    public async Task<Guid> CreateAssignmentAsync(CreateAssignmentDto dto)
    {
        _logger.LogInformation("Začetek postopka ustvarjanja nove naloge z naslovom '{Title}'", dto.Title);

        var assignment = new Assignment(dto.Title, dto.Description, dto.Deadline, dto.MaxScore);

        await _assignmentRepository.AddAsync(assignment);
        await _assignmentRepository.SaveChangesAsync();

        _logger.LogInformation("Uspešno ustvarjena naloga z ID-jem {AssignmentId}", assignment.Id);

        return assignment.Id;
    }

    public async Task<IEnumerable<AssignmentDto>> GetAllAssignmentsAsync()
    {
        _logger.LogInformation("Pridobivanje seznama vseh nalog");

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
        _logger.LogInformation("Pridobivanje podrobnosti za nalogo z ID-jem {AssignmentId}", id);

        var assignment = await _assignmentRepository.GetByIdAsync(id);
        if (assignment == null)
        {
            _logger.LogWarning("Naloga z ID-jem {AssignmentId} ni bila najdena", id);
            return null;
        }

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
        _logger.LogInformation("Začetek posodabljanja naloge z ID-jem {AssignmentId}", id);

        var assignment = await _assignmentRepository.GetByIdAsync(id);
        if (assignment == null)
        {
            _logger.LogWarning("Poskus posodobitve neobstoječe naloge z ID-jem {AssignmentId}", id);
            return false;
        }

        assignment.Update(dto.Title, dto.Description, dto.Deadline, dto.MaxScore);

        await _assignmentRepository.SaveChangesAsync();

        _logger.LogInformation("Uspešno posodobljena naloga z ID-jem {AssignmentId}", id);

        return true;
    }

    public async Task<bool> DeleteAssignmentAsync(Guid id)
    {
        _logger.LogInformation("Začetek postopka brisanja naloge z ID-jem {AssignmentId}", id);

        var assignment = await _assignmentRepository.GetByIdAsync(id);
        if (assignment == null)
        {
            _logger.LogWarning("Poskus brisanja neobstoječe naloge z ID-jem {AssignmentId}", id);
            return false;
        }

        await _assignmentRepository.DeleteAsync(assignment);

        _logger.LogInformation("Uspešno izbrisana naloga z ID-jem {AssignmentId}", id);

        return true;
    }
}