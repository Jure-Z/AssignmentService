using System.Net;
using System.Net.Http.Json;
using AssignmentService.Application.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AssignmentService.IntegrationTests;

public class BusinessRuleValidationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BusinessRuleValidationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateSubmission_WhenStudentAlreadySubmitted_ShouldReturn400BadRequest()
    {
        // Arrange: Ustvarimo nalogo
        var assignmentResponse = await _client.PostAsJsonAsync("/api/assignments", new CreateAssignmentDto(
            Title: "Dvojna oddaja test",
            Description: "Opis",
            Deadline: DateTime.UtcNow.AddDays(5),
            MaxScore: 100m
        ));
        var assignmentId = (await assignmentResponse.Content.ReadFromJsonAsync<IdResponse>())!.Id;

        var submissionDto = new CreateSubmissionDto(assignmentId, "student-podvojen", "Prva vsebina");

        // Act: Prva oddaja uspe
        var firstResponse = await _client.PostAsJsonAsync("/api/submissions", submissionDto);
        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Druga oddaja istega študenta za isto nalogo
        var secondResponse = await _client.PostAsJsonAsync("/api/submissions", submissionDto);

        // Assert: GlobalExceptionHandler mora vrniti 400 Bad Request
        secondResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = await secondResponse.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails!.Detail.Should().Contain("že ustvaril oddajo");
    }

    [Fact]
    public async Task UpsertEvaluation_WhenScoreExceedsMaxScore_ShouldReturn400BadRequest()
    {
        // Arrange: Naloga z max 50 točkami
        var assignmentResponse = await _client.PostAsJsonAsync("/api/assignments", new CreateAssignmentDto(
            Title: "Naloga 50 točk",
            Description: "Opis",
            Deadline: DateTime.UtcNow.AddDays(5),
            MaxScore: 50m
        ));
        var assignmentId = (await assignmentResponse.Content.ReadFromJsonAsync<IdResponse>())!.Id;

        // Študent ustvari oddajo
        var submissionResponse = await _client.PostAsJsonAsync("/api/submissions",
            new CreateSubmissionDto(assignmentId, "student-ocena", "Vsebina"));
        var submissionId = (await submissionResponse.Content.ReadFromJsonAsync<IdResponse>())!.Id;

        // Act: Poskus vnosa 60 točk na nalogi s 50 točkami
        var invalidEvaluation = new UpsertEvaluationDto(Score: 60m, Feedback: "Preveč točk");
        var evalResponse = await _client.PostAsJsonAsync($"/api/submissions/{submissionId}/evaluation", invalidEvaluation);

        // Assert: Poslovna napaka mora vrniti 400 Bad Request
        evalResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = await evalResponse.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails!.Detail.Should().Contain("ne more presegati maksimalnega števila točk naloge");
    }

    private record IdResponse(Guid Id);
}