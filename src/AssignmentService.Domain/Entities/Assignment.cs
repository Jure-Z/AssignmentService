using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentService.Domain.Entities;

public class Assignment
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime Deadline { get; private set; }
    public decimal MaxScore { get; private set; }


    //related submissions
    private readonly List<Submission> _submissions = new();
    public IReadOnlyCollection<Submission> Submissions => _submissions.AsReadOnly();

    //empty constructor
    protected Assignment() { }

    public Assignment(string title, string description, DateTime deadline, decimal maxScore)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Naslov naloge ne sme biti prazen.", nameof(title));

        if (maxScore <= 0)
            throw new ArgumentException("Maksimalno število točk mora biti večje od 0.", nameof(maxScore));

        if (deadline <= DateTime.UtcNow)
            throw new ArgumentException("Rok oddaje (deadline) mora biti v prihodnosti.", nameof(deadline));

        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        CreatedAt = DateTime.UtcNow;
        Deadline = deadline;
        MaxScore = maxScore;
    }
}