using System.Net;
using System.Net.Http.Json;
using AssignmentService.Application.DTOs;
using FluentAssertions;
using Xunit;

namespace AssignmentService.IntegrationTests;

public class WorkflowIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public WorkflowIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CompleteAssignmentLifecycle_ShouldSucceed()
    {
        // -------------------------------------------------------------
        // 1. Profesor ustvari novo nalogo (MaxScore = 100)
        // -------------------------------------------------------------
        var createAssignmentDto = new CreateAssignmentDto(
            Title: "E2E Test Naloga",
            Description: "Opis celotnega toka",
            Deadline: DateTime.UtcNow.AddDays(7),
            MaxScore: 100m
        );

        var assignmentResponse = await _client.PostAsJsonAsync("/api/assignments", createAssignmentDto);
        assignmentResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var assignmentId = (await assignmentResponse.Content.ReadFromJsonAsync<IdResponse>())!.Id;

        // -------------------------------------------------------------
        // 2. Študent ustvari osnutek oddaje (Draft)
        // -------------------------------------------------------------
        var studentId = "student-janez-novak";
        var createSubmissionDto = new CreateSubmissionDto(
            AssignmentId: assignmentId,
            StudentId: studentId,
            Content: "Moja izvorna koda rešitve"
        );

        var submissionResponse = await _client.PostAsJsonAsync("/api/submissions", createSubmissionDto);
        submissionResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var submissionId = (await submissionResponse.Content.ReadFromJsonAsync<IdResponse>())!.Id;

        // Preverimo, da je oddaja v statusu Draft
        var draftDto = await _client.GetFromJsonAsync<SubmissionDto>($"/api/submissions/{submissionId}");
        draftDto!.Status.Should().Be("Draft");
        draftDto.Evaluation.Should().BeNull();

        // -------------------------------------------------------------
        // 3. Študent odda nalogo v pregled
        // -------------------------------------------------------------
        var submitResponse = await _client.PostAsync($"/api/submissions/{submissionId}/submit", null);
        submitResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Preverimo, da se je status spremenil v Submitted
        var submittedDto = await _client.GetFromJsonAsync<SubmissionDto>($"/api/submissions/{submissionId}");
        submittedDto!.Status.Should().Be("Submitted");
        submittedDto.SubmittedAt.Should().NotBeNull();

        // -------------------------------------------------------------
        // 4. Profesor oceni oddano nalogo
        // -------------------------------------------------------------
        var evaluationDto = new UpsertEvaluationDto(
            Score: 92.5m,
            Feedback: "Odlična rešitev, dobro strukturirano!"
        );

        var evalResponse = await _client.PostAsJsonAsync($"/api/submissions/{submissionId}/evaluation", evaluationDto);
        evalResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // -------------------------------------------------------------
        // 5. Preverimo končno stanje: oddaja ima pripeto veljavno oceno
        // -------------------------------------------------------------
        var finalSubmission = await _client.GetFromJsonAsync<SubmissionDto>($"/api/submissions/{submissionId}");
        finalSubmission.Should().NotBeNull();
        finalSubmission!.Evaluation.Should().NotBeNull();
        finalSubmission.Evaluation!.Score.Should().Be(92.5m);
        finalSubmission.Evaluation.Feedback.Should().Be("Odlična rešitev, dobro strukturirano!");
    }

    private record IdResponse(Guid Id);
}