namespace AssignmentService.Domain.Entities;

public enum SubmissionStatus
{
    Draft = 0,
    Submitted = 1,
    Late = 2,
    Evaluated = 3
}
public class Submission
{
    public Guid Id { get; private set; }
    public Guid AssignmentId { get; private set; }
    public Assignment Assignment { get; private set; } = null!;
    public string StudentId { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime? SubmittedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public SubmissionStatus Status { get; private set; }

    //related evaluation
    public Evaluation? Evaluation { get; private set; }

    protected Submission() { }

    public Submission(Guid assignmentId, string studentId, string content)
    {
        if (assignmentId == Guid.Empty)
            throw new ArgumentException("Oddaja mora biti povezana z veljavno nalogo.", nameof(assignmentId));

        if (string.IsNullOrWhiteSpace(studentId))
            throw new ArgumentException("Identifikator študenta ne sme biti prazen.", nameof(studentId));

        Id = Guid.NewGuid();
        AssignmentId = assignmentId;
        StudentId = studentId;
        Content = content ?? string.Empty;
        Status = SubmissionStatus.Draft;
    }

    public void Update(string content)
    {
        if (Status != SubmissionStatus.Draft)
            throw new InvalidOperationException("Oddaje ni mogoče posodobiti, ker je bila že oddana.");

        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Vsebina oddaje ne sme biti prazna.", nameof(content));

        Content = content;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Submit()
    {
        if (Evaluation != null || Status == SubmissionStatus.Evaluated)
            throw new InvalidOperationException("Ocenjene oddaje ni mogoče ponovno oddati.");

        if (Status == SubmissionStatus.Submitted || Status == SubmissionStatus.Late)
            throw new InvalidOperationException("Naloga je bila že oddana.");

        if (string.IsNullOrWhiteSpace(Content))
            throw new InvalidOperationException("Prazne oddaje ni mogoče oddati. Prosimo, vnesite vsebino.");

        if (Assignment == null)
            throw new InvalidOperationException("Oddaja ni povezana z nalogo (manjka podatkovni kontekst).");

        SubmittedAt = DateTime.UtcNow;

        if (SubmittedAt > Assignment.Deadline)
        {
            Status = SubmissionStatus.Late;
        }
        else
        {
            Status = SubmissionStatus.Submitted;
        }
    }
}
