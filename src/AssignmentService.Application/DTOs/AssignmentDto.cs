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
