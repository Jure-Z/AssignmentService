using AssignmentService.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace AssignmentService.UnitTests.Domain;

public class AssignmentTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateAssignment()
    {
        // Arrange & Act
        var deadline = DateTime.UtcNow.AddDays(7);
        var assignment = new Assignment("DN 1", "Opis naloge", deadline, 100m);

        // Assert
        assignment.Id.Should().NotBeEmpty();
        assignment.Title.Should().Be("DN 1");
        assignment.Description.Should().Be("Opis naloge");
        assignment.Deadline.Should().Be(deadline);
        assignment.MaxScore.Should().Be(100m);
        assignment.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        assignment.UpdatedAt.Should().BeNull();
    }

    [Theory]
    [InlineData("", "Opis", 100)]
    [InlineData("   ", "Opis", 100)]
    [InlineData(null, "Opis", 100)]
    public void Constructor_WithInvalidTitle_ShouldThrowArgumentException(string? invalidTitle, string description, decimal maxScore)
    {
        // Act
        var act = () => new Assignment(invalidTitle!, description, DateTime.UtcNow.AddDays(1), maxScore);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("title");
    }

    [Fact]
    public void Constructor_WithPastDeadline_ShouldThrowArgumentException()
    {
        // Act
        var act = () => new Assignment("Naslov", "Opis", DateTime.UtcNow.AddMinutes(-5), 100m);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("deadline");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Constructor_WithInvalidMaxScore_ShouldThrowArgumentException(decimal invalidScore)
    {
        // Act
        var act = () => new Assignment("Naslov", "Opis", DateTime.UtcNow.AddDays(1), invalidScore);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("maxScore");
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateFieldsAndSetUpdatedAt()
    {
        // Arrange
        var assignment = new Assignment("Original", "Opis", DateTime.UtcNow.AddDays(2), 50m);
        var newDeadline = DateTime.UtcNow.AddDays(5);

        // Act
        assignment.Update("Posodobljeno", "Nov opis", newDeadline, 80m);

        // Assert
        assignment.Title.Should().Be("Posodobljeno");
        assignment.Description.Should().Be("Nov opis");
        assignment.Deadline.Should().Be(newDeadline);
        assignment.MaxScore.Should().Be(80m);
        assignment.UpdatedAt.Should().NotBeNull();
        assignment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }
}