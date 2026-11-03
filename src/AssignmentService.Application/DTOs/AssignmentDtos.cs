namespace AssignmentService.Application.DTOs;

public record AssignmentDto(
    Guid Id,
    string Title,
    string Description,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime Deadline,
    decimal MaxScore
);

public record CreateAssignmentDto(
    string Title,
    string Description,
    DateTime Deadline,
    decimal MaxScore
);

public record UpdateAssignmentDto(
    string Title,
    string Description,
    DateTime Deadline,
    decimal MaxScore
);
