namespace AssignmentService.Domain.Entities;

public class Assignment
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime Deadline { get; private set; }
    public decimal MaxScore { get; private set; }


    //related submissions
    private readonly List<Submission> _submissions = new();
    public IReadOnlyCollection<Submission> Submissions => _submissions.AsReadOnly();

    //empty constructor
    protected Assignment() { }

    public Assignment(string title, string description, DateTime deadline, decimal maxScore)
    {
        Validate(title, description, deadline, maxScore);

        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        CreatedAt = DateTime.UtcNow;
        Deadline = deadline;
        MaxScore = maxScore;
    }

    public void Update(string title, string description, DateTime deadline, decimal maxScore)
    {
        Validate(title, description, deadline, maxScore);

        Title = title;
        Description = description;
        Deadline = deadline;
        MaxScore = maxScore;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void Validate(string title, string description, DateTime deadline, decimal maxScore)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Naslov naloge je obvezen.", nameof(title));

        if (title.Length > 200)
            throw new ArgumentException("Naslov ne sme biti daljši od 200 znakov.", nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Opis naloge je obvezen.", nameof(description));

        if (deadline <= DateTime.UtcNow)
            throw new ArgumentException("Rok oddaje mora biti v prihodnosti.", nameof(deadline));

        if (maxScore <= 0)
            throw new ArgumentException("Največje število točk mora biti večje od 0.", nameof(maxScore));
    }
}