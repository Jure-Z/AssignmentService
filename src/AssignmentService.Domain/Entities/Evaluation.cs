using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentService.Domain.Entities;

public class Evaluation
{
    public Guid Id { get; private set; }
    public Guid SubmissionId { get; private set; }
    public Submission Submission { get; private set; } = null!;
    public string EvaluatorId { get; private set; } = string.Empty;
    public decimal Score { get; private set; }
    public string? Feedback { get; private set; }
    public DateTime EvaluatedAt { get; private set; }


    protected Evaluation() { }

    public Evaluation(Guid submissionId, string evaluatorId, decimal score, string? feedback)
    {
        // Domenska pravila
        if (submissionId == Guid.Empty)
            throw new ArgumentException("Ocena mora biti povezana z veljavno oddajo.", nameof(submissionId));

        if (string.IsNullOrWhiteSpace(evaluatorId))
            throw new ArgumentException("Identifikator ocenjevalca ne sme biti prazen.", nameof(evaluatorId));

        if (score < 0)
            throw new ArgumentException("Ocena (točke) ne more biti negativna.", nameof(score));

        Id = Guid.NewGuid();
        SubmissionId = submissionId;
        EvaluatorId = evaluatorId;
        Score = score;
        Feedback = feedback;
        EvaluatedAt = DateTime.UtcNow;
    }
}
