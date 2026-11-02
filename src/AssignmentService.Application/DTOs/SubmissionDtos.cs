namespace AssignmentService.Application.DTOs;

// DTO za prikaz podatkov o oddaji
public record SubmissionDto(
    Guid Id,
    Guid AssignmentId,
    string StudentId,
    string Content,
    DateTime CreatedAt,
    DateTime? SubmittedAt,
    DateTime? UpdatedAt,
    string Status, // Lahko vrnemo kot niz (npr. "Draft", "Submitted", "Late", "Evaluated")
    EvaluationDto? Evaluation // Povezana ocena, če obstaja
);

// DTO za ustvarjanje novega osnutka oddaje (POST)
public record CreateSubmissionDto(
    Guid AssignmentId,
    string StudentId,
    string Content
);

// DTO za posodabljanje vsebine oddaje (PUT)
public record UpdateSubmissionDto(
    string Content
);