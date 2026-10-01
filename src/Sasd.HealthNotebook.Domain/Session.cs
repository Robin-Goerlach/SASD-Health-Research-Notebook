namespace Sasd.HealthNotebook.Domain;

/// <summary>User-selected conversation category, without clinical interpretation.</summary>
public enum SessionType { DoctorVisit, Checkup, Coaching, Physiotherapy, NutritionConsultation, OtherConsultation }
/// <summary>User-maintained appointment state; no automatic transitions.</summary>
public enum SessionStatus { Planned, Completed, Cancelled }
/// <summary>User-maintained state of a next step.</summary>
public enum SessionFollowUpStatus { Open, Done }

/// <summary>A documented conversation or appointment, independent of UI and storage.</summary>
public sealed record Session
{
    /// <summary>Maximum identifying title length.</summary>
    public const int MaximumTitleLength = 160;
    /// <summary>Maximum free-text length.</summary>
    public const int MaximumTextLength = 4000;
    /// <summary>Stable identifier.</summary>
    public required Guid Id { get; init; }
    /// <summary>Scheduled/documented time including offset.</summary>
    public required DateTimeOffset ScheduledAt { get; init; }
    /// <summary>User-selected conversation category.</summary>
    public required SessionType SessionType { get; init; }
    /// <summary>User-maintained status.</summary>
    public required SessionStatus Status { get; init; }
    /// <summary>Required title/reason.</summary>
    public required string Title { get; init; }
    /// <summary>Optional topic reference; no title copy.</summary>
    public Guid? HealthTopicId { get; init; }
    /// <summary>Optional contact/institution text, limited to 160 characters.</summary>
    public string? ContactText { get; init; }
    /// <summary>Personal conversation notes; never verified as medical facts.</summary>
    public string? Notes { get; init; }
    /// <summary>Technical creation time.</summary>
    public required DateTimeOffset CreatedAt { get; init; }
    /// <summary>Technical last update time.</summary>
    public required DateTimeOffset ModifiedAt { get; init; }
    /// <summary>Creates a validated appointment without interpreting its text.</summary>
    public static Session Create(DateTimeOffset scheduledAt, string title, SessionType type, SessionStatus status,
        Guid? healthTopicId = null, string? contactText = null, string? notes = null)
    {
        var now = DateTimeOffset.Now;
        var session = new Session { Id = Guid.NewGuid(), ScheduledAt = scheduledAt, Title = title?.Trim() ?? string.Empty,
            SessionType = type, Status = status, HealthTopicId = healthTopicId, ContactText = contactText, Notes = notes,
            CreatedAt = now, ModifiedAt = now };
        session.Validate(); return session;
    }
    /// <summary>Validates structure without including user content in errors.</summary>
    public void Validate()
    {
        if (Id == Guid.Empty || HealthTopicId == Guid.Empty || ScheduledAt == default || CreatedAt == default
            || ModifiedAt < CreatedAt || !Enum.IsDefined(SessionType) || !Enum.IsDefined(Status)
            || string.IsNullOrWhiteSpace(Title) || Title.Length > MaximumTitleLength
            || ContactText?.Length > MaximumTitleLength || Notes?.Length > MaximumTextLength)
            throw new ArgumentException("Invalid session metadata or text length.");
    }
}
