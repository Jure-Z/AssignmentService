using AssignmentService.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace AssignmentService.UnitTests.Domain;

public class EvaluationTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateEvaluation()
    {
        // Arrange & Act
        var submissionId = Guid.NewGuid();
        var evaluation = new Evaluation(submissionId, 95.5m, "Odlično opravljeno!");

        // Assert
        evaluation.Id.Should().NotBeEmpty();
        evaluation.SubmissionId.Should().Be(submissionId);
        evaluation.Score.Should().Be(95.5m);
        evaluation.Feedback.Should().Be("Odlično opravljeno!");
        evaluation.EvaluatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void Constructor_WithNegativeScore_ShouldThrowArgumentException()
    {
        // Act
        var act = () => new Evaluation(Guid.NewGuid(), -5m, "Feedback");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("score");
    }

    [Fact]
    public void Update_ShouldModifyScoreAndFeedback()
    {
        // Arrange
        var evaluation = new Evaluation(Guid.NewGuid(), 50m, "Začetna ocena");

        // Act
        evaluation.Update(80m, "Popravljena ocena");

        // Assert
        evaluation.Score.Should().Be(80m);
        evaluation.Feedback.Should().Be("Popravljena ocena");
        evaluation.EvaluatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }
}