namespace AssignmentService.Application.DTOs;

public record CreateAssignmentDto(string Title, string Description, DateTime Deadline, decimal MaxScore);