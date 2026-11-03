using AssignmentService.Domain.Entities;
using AssignmentService.UnitTests.Common;
using FluentAssertions;
using Xunit;

namespace AssignmentService.UnitTests.Domain;

public class SubmissionTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldInitializeInDraftStatus()
    {
        // Arrange & Act
        var assignmentId = Guid.NewGuid();
        var submission = new Submission(assignmentId, "student-123", "Koda naloge");

        // Assert
        submission.Id.Should().NotBeEmpty();
        submission.AssignmentId.Should().Be(assignmentId);
        submission.StudentId.Should().Be("student-123");
        submission.Content.Should().Be("Koda naloge");
        submission.Status.Should().Be(SubmissionStatus.Draft);
        submission.SubmittedAt.Should().BeNull();
    }

    [Fact]
    public void Update_WhenInDraftStatus_ShouldUpdateContentAndSetUpdatedAt()
    {
        // Arrange
        var submission = new Submission(Guid.NewGuid(), "student-123", "Stara vsebina");

        // Act
        submission.Update("Nova vsebina");

        // Assert
        submission.Content.Should().Be("Nova vsebina");
        submission.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Update_WhenAlreadySubmitted_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var assignment = new Assignment("Test", "Opis", DateTime.UtcNow.AddDays(2), 100m);
        var submission = new Submission(assignment.Id, "student-123", "Vsebina");
        TestHelpers.SetPrivateProperty(submission, nameof(Submission.Assignment), assignment);
        submission.Submit();

        // Act
        var act = () => submission.Update("Poskus posodobitve");

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*ker je bila že oddana*");
    }

    [Fact]
    public void Submit_BeforeDeadline_ShouldSetStatusToSubmitted()
    {
        // Arrange
        var assignment = new Assignment("Test", "Opis", DateTime.UtcNow.AddDays(1), 100m);
        var submission = new Submission(assignment.Id, "student-123", "Moja rešitev");
        TestHelpers.SetPrivateProperty(submission, nameof(Submission.Assignment), assignment);

        // Act
        submission.Submit();

        // Assert
        submission.Status.Should().Be(SubmissionStatus.Submitted);
        submission.SubmittedAt.Should().NotBeNull();
    }

    [Fact]
    public void Submit_AfterDeadline_ShouldSetStatusToLate()
    {
        // Arrange
        var assignment = new Assignment("Test", "Opis", DateTime.UtcNow.AddDays(1), 100m);
        // Simuliramo pretekel rok z nastavitvijo datuma v preteklost
        TestHelpers.SetPrivateProperty(assignment, nameof(Assignment.Deadline), DateTime.UtcNow.AddHours(-2));

        var submission = new Submission(assignment.Id, "student-123", "Pozna rešitev");
        TestHelpers.SetPrivateProperty(submission, nameof(Submission.Assignment), assignment);

        // Act
        submission.Submit();

        // Assert
        submission.Status.Should().Be(SubmissionStatus.Late);
        submission.SubmittedAt.Should().NotBeNull();
    }

    [Fact]
    public void Submit_WhenContentIsEmpty_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var assignment = new Assignment("Test", "Opis", DateTime.UtcNow.AddDays(1), 100m);
        var submission = new Submission(assignment.Id, "student-123", "   ");
        TestHelpers.SetPrivateProperty(submission, nameof(Submission.Assignment), assignment);

        // Act
        var act = () => submission.Submit();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Prazne oddaje ni mogoče oddati*");
    }

    [Fact]
    public void Submit_WhenAssignmentContextIsMissing_ShouldThrowInvalidOperationException()
    {
        // Arrange: Assignment navigacijska lastnost je null
        var submission = new Submission(Guid.NewGuid(), "student-123", "Vsebina");

        // Act
        var act = () => submission.Submit();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*manjka podatkovni kontekst*");
    }
}