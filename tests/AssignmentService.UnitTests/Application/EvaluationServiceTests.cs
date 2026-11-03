using AssignmentService.Application.DTOs;
using AssignmentService.Application.Interfaces;
using AssignmentService.Application.Services;
using AssignmentService.Domain.Entities;
using AssignmentService.UnitTests.Common;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace AssignmentService.UnitTests.Application;

public class EvaluationServiceTests
{
    private readonly IEvaluationRepository _evaluationRepo = Substitute.For<IEvaluationRepository>();
    private readonly ISubmissionRepository _submissionRepo = Substitute.For<ISubmissionRepository>();
    private readonly ILogger<EvaluationService> _logger = Substitute.For<ILogger<EvaluationService>>();
    private readonly EvaluationService _service;

    public EvaluationServiceTests()
    {
        _service = new EvaluationService(_evaluationRepo, _submissionRepo, _logger);
    }

    [Fact]
    public async Task UpsertEvaluationAsync_WhenSubmissionDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var submissionId = Guid.NewGuid();
        _submissionRepo.GetByIdWithAssignmentAsync(submissionId).Returns((Submission?)null);

        // Act
        var act = async () => await _service.UpsertEvaluationAsync(submissionId, new UpsertEvaluationDto(10m, "Feedback"));

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task UpsertEvaluationAsync_WhenScoreExceedsMaxScore_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var submissionId = Guid.NewGuid();
        var assignment = new Assignment("DN", "Opis", DateTime.UtcNow.AddDays(3), maxScore: 50m);
        var submission = new Submission(assignment.Id, "student-1", "Vsebina");
        TestHelpers.SetPrivateProperty(submission, nameof(Submission.Assignment), assignment);

        _submissionRepo.GetByIdWithAssignmentAsync(submissionId).Returns(submission);

        var dto = new UpsertEvaluationDto(Score: 55m, Feedback: "Preveč točk");

        // Act
        var act = async () => await _service.UpsertEvaluationAsync(submissionId, dto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*ne more presegati maksimalnega števila točk*");
    }

    [Fact]
    public async Task UpsertEvaluationAsync_WhenNoEvaluationExists_ShouldCreateNewEvaluation()
    {
        // Arrange
        var submissionId = Guid.NewGuid();
        var assignment = new Assignment("DN", "Opis", DateTime.UtcNow.AddDays(3), maxScore: 100m);
        var submission = new Submission(assignment.Id, "student-1", "Vsebina");
        TestHelpers.SetPrivateProperty(submission, nameof(Submission.Assignment), assignment);

        _submissionRepo.GetByIdWithAssignmentAsync(submissionId).Returns(submission);
        _evaluationRepo.GetBySubmissionIdAsync(submissionId).Returns((Evaluation?)null);

        var dto = new UpsertEvaluationDto(Score: 85m, Feedback: "Dobra rešitev");

        // Act
        var resultId = await _service.UpsertEvaluationAsync(submissionId, dto);

        // Assert
        resultId.Should().NotBeEmpty();
        await _evaluationRepo.Received(1).AddAsync(Arg.Is<Evaluation>(e => e.Score == 85m && e.Feedback == "Dobra rešitev"));
        await _evaluationRepo.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task UpsertEvaluationAsync_WhenEvaluationAlreadyExists_ShouldUpdateExistingEvaluation()
    {
        // Arrange
        var submissionId = Guid.NewGuid();
        var assignment = new Assignment("DN", "Opis", DateTime.UtcNow.AddDays(3), maxScore: 100m);
        var submission = new Submission(assignment.Id, "student-1", "Vsebina");
        TestHelpers.SetPrivateProperty(submission, nameof(Submission.Assignment), assignment);

        var existingEvaluation = new Evaluation(submissionId, 60m, "Stara ocena");

        _submissionRepo.GetByIdWithAssignmentAsync(submissionId).Returns(submission);
        _evaluationRepo.GetBySubmissionIdAsync(submissionId).Returns(existingEvaluation);

        var dto = new UpsertEvaluationDto(Score: 90m, Feedback: "Popravljeno na bolje");

        // Act
        var resultId = await _service.UpsertEvaluationAsync(submissionId, dto);

        // Assert
        resultId.Should().Be(existingEvaluation.Id);
        existingEvaluation.Score.Should().Be(90m);
        existingEvaluation.Feedback.Should().Be("Popravljeno na bolje");
        await _evaluationRepo.DidNotReceive().AddAsync(Arg.Any<Evaluation>());
        await _evaluationRepo.Received(1).SaveChangesAsync();
    }
}