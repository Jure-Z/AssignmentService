namespace AssignmentService.Application.DTOs;

public record UpsertEvaluationDto(
    decimal Score,
    string? Feedback
);

public record EvaluationDto(
    Guid Id,
    Guid SubmissionId,
    decimal Score,
    string? Feedback,
    DateTime EvaluatedAt
);