namespace AssignmentService.Application.DTOs;

public record UpdateAssignmentDto(
    string Title,
    string Description,
    DateTime Deadline,
    decimal MaxScore
);
