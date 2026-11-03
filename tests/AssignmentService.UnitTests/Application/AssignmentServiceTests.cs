using AssignmentService.Application.DTOs;
using AssignmentService.Application.Interfaces;
using AssignmentService.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace AssignmentService.UnitTests.Application;

public class AssignmentServiceTests
{
    private readonly IAssignmentRepository _repository = Substitute.For<IAssignmentRepository>();
    private readonly ILogger<AssignmentService.Application.Services.AssignmentService> _logger =
        Substitute.For<ILogger<AssignmentService.Application.Services.AssignmentService>>();
    private readonly AssignmentService.Application.Services.AssignmentService _service;

    public AssignmentServiceTests()
    {
        _service = new AssignmentService.Application.Services.AssignmentService(_repository, _logger);
    }

    [Fact]
    public async Task CreateAssignmentAsync_ShouldPersistAssignmentAndReturnId()
    {
        // Arrange
        var dto = new CreateAssignmentDto("DN 1", "Opis", DateTime.UtcNow.AddDays(3), 100m);

        // Act
        var resultId = await _service.CreateAssignmentAsync(dto);

        // Assert
        resultId.Should().NotBeEmpty();
        await _repository.Received(1).AddAsync(Arg.Is<Assignment>(a => a.Title == dto.Title));
        await _repository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task GetAssignmentByIdAsync_WhenAssignmentNotFound_ShouldReturnNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repository.GetByIdAsync(id).Returns((Assignment?)null);

        // Act
        var result = await _service.GetAssignmentByIdAsync(id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAssignmentAsync_WhenAssignmentNotFound_ShouldReturnFalse()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repository.GetByIdAsync(id).Returns((Assignment?)null);

        // Act
        var result = await _service.DeleteAssignmentAsync(id);

        // Assert
        result.Should().BeFalse();
        await _repository.DidNotReceive().DeleteAsync(Arg.Any<Assignment>());
    }
}