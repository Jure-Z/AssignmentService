using AssignmentService.Application.DTOs;
using AssignmentService.Application.Interfaces;
using AssignmentService.Application.Services;
using AssignmentService.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace AssignmentService.UnitTests.Application;

public class SubmissionServiceTests
{
    private readonly ISubmissionRepository _submissionRepo = Substitute.For<ISubmissionRepository>();
    private readonly IAssignmentRepository _assignmentRepo = Substitute.For<IAssignmentRepository>();
    private readonly ILogger<SubmissionService> _logger = Substitute.For<ILogger<SubmissionService>>();
    private readonly SubmissionService _service;

    public SubmissionServiceTests()
    {
        _service = new SubmissionService(_submissionRepo, _assignmentRepo, _logger);
    }

    [Fact]
    public async Task CreateSubmissionAsync_WhenAssignmentDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var dto = new CreateSubmissionDto(Guid.NewGuid(), "student-1", "Vsebina");
        _assignmentRepo.GetByIdAsync(dto.AssignmentId).Returns((Assignment?)null);

        // Act
        var act = async () => await _service.CreateSubmissionAsync(dto);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{dto.AssignmentId}*");
    }

    [Fact]
    public async Task CreateSubmissionAsync_WhenStudentAlreadySubmitted_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var dto = new CreateSubmissionDto(Guid.NewGuid(), "student-1", "Druga rešitev");
        var assignment = new Assignment("Test", "Opis", DateTime.UtcNow.AddDays(2), 100m);
        var existingSubmission = new Submission(dto.AssignmentId, dto.StudentId, "Prva rešitev");

        _assignmentRepo.GetByIdAsync(dto.AssignmentId).Returns(assignment);
        _submissionRepo.GetByAssignmentAndStudentAsync(dto.AssignmentId, dto.StudentId).Returns(existingSubmission);

        // Act
        var act = async () => await _service.CreateSubmissionAsync(dto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*že ustvaril oddajo*");
    }

    [Fact]
    public async Task SubmitAssignmentAsync_WhenSubmissionNotFound_ShouldReturnFalse()
    {
        // Arrange
        var submissionId = Guid.NewGuid();
        _submissionRepo.GetByIdWithAssignmentAsync(submissionId).Returns((Submission?)null);

        // Act
        var result = await _service.SubmitAssignmentAsync(submissionId);

        // Assert
        result.Should().BeFalse();
        await _submissionRepo.DidNotReceive().SaveChangesAsync();
    }
}