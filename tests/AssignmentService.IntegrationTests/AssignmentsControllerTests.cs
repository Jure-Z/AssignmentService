using System.Net;
using System.Net.Http.Json;
using AssignmentService.Application.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AssignmentService.IntegrationTests;

public class AssignmentsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AssignmentsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateAssignment_WithValidData_ShouldReturn201CreatedAndPersist()
    {
        // Arrange
        var request = new CreateAssignmentDto(
            Title: "Projektna naloga - Integracijski test",
            Description: "Opis projektne naloge za študente.",
            Deadline: DateTime.UtcNow.AddDays(10),
            MaxScore: 100m
        );

        // Act
        var postResponse = await _client.PostAsJsonAsync("/api/assignments", request);

        // Assert
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        postResponse.Headers.Location.Should().NotBeNull();

        // Preverimo še, ali jo lahko takoj preberemo prek GET
        var created = await postResponse.Content.ReadFromJsonAsync<CreatedIdResponse>();
        created.Should().NotBeNull();

        var getResponse = await _client.GetAsync($"/api/assignments/{created!.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var assignmentDto = await getResponse.Content.ReadFromJsonAsync<AssignmentDto>();
        assignmentDto.Should().NotBeNull();
        assignmentDto!.Title.Should().Be(request.Title);
        assignmentDto.MaxScore.Should().Be(100m);
    }

    [Fact]
    public async Task CreateAssignment_WithPastDeadline_ShouldReturn400BadRequest()
    {
        // Arrange: rok v preteklosti mora sprožiti FluentValidation pravilo
        var invalidRequest = new CreateAssignmentDto(
            Title: "Neveljavna naloga",
            Description: "Opis",
            Deadline: DateTime.UtcNow.AddDays(-1),
            MaxScore: 50m
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/assignments", invalidRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().ContainKey("Deadline");
    }

    [Fact]
    public async Task GetById_WhenNotFound_ShouldReturn404NotFound()
    {
        // Act
        var response = await _client.GetAsync($"/api/assignments/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // Pomožni zapis za branje ID-ja iz Created odgovora
    private record CreatedIdResponse(Guid Id);
}