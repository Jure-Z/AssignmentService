using System;
using System.Collections.Generic;
using System.Text;

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
    public DateTime? SubmittedAt { get; private set; }
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

    public void Submit(DateTime assignmentDeadline)
    {
        SubmittedAt = DateTime.UtcNow;

        if (SubmittedAt > assignmentDeadline)
        {
            Status = SubmissionStatus.Late;
        }
        else
        {
            Status = SubmissionStatus.Submitted;
        }
    }
}
