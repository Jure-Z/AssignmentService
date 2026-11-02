namespace AssignmentService.Domain.Entities;

public class Evaluation
{
    public Guid Id { get; private set; }
    public Guid SubmissionId { get; private set; }
    public Submission Submission { get; private set; } = null!;
    public decimal Score { get; private set; }
    public string? Feedback { get; private set; }
    public DateTime EvaluatedAt { get; private set; }


    protected Evaluation() { }

    public Evaluation(Guid submissionId, decimal score, string? feedback)
    {
        if (submissionId == Guid.Empty)
            throw new ArgumentException("Ocena mora biti povezana z veljavno oddajo.", nameof(submissionId));

        if (score < 0)
            throw new ArgumentException("Ocena (točke) ne more biti negativna.", nameof(score));

        Id = Guid.NewGuid();
        SubmissionId = submissionId;
        Score = score;
        Feedback = feedback;
        EvaluatedAt = DateTime.UtcNow;
    }

    public void Update(decimal score, string? feedback)
    {
        Score = score;
        Feedback = feedback;
        EvaluatedAt = DateTime.UtcNow;
    }
}
