namespace Sasd.HealthNotebook.Domain;

/// <summary>User-documented next step; never generated as a medical instruction.</summary>
public sealed record SessionFollowUp
{
    /// <summary>Stable identifier.</summary>
    public required Guid Id { get; init; }
    /// <summary>Owning session.</summary>
    public required Guid SessionId { get; init; }
    /// <summary>Required next-step text, at most 4000 characters.</summary>
    public required string Text { get; init; }
    /// <summary>User-maintained state.</summary>
    public required SessionFollowUpStatus Status { get; init; }
    /// <summary>Optional user-selected calendar due date; no reminder is generated.</summary>
    public DateOnly? DueDate { get; init; }
    /// <summary>Technical creation time.</summary>
    public required DateTimeOffset CreatedAt { get; init; }
    /// <summary>Technical last update time.</summary>
    public required DateTimeOffset ModifiedAt { get; init; }
    /// <summary>Creates a next step without reminders or inferred actions.</summary>
    public static SessionFollowUp Create(Guid sessionId, string text, SessionFollowUpStatus status = SessionFollowUpStatus.Open, DateOnly? dueDate = null)
    {
        var now = DateTimeOffset.Now;
        var followUp = new SessionFollowUp { Id = Guid.NewGuid(), SessionId = sessionId, Text = text?.Trim() ?? string.Empty,
            Status = status, DueDate = dueDate, CreatedAt = now, ModifiedAt = now };
        followUp.Validate(); return followUp;
    }
    /// <summary>Updates only the explicitly selected state.</summary>
    public SessionFollowUp WithStatus(SessionFollowUpStatus status)
    {
        var updated = this with { Status = status, ModifiedAt = DateTimeOffset.Now };
        updated.Validate(); return updated;
    }
    /// <summary>Checks metadata and bounded text without interpreting the next step.</summary>
    public void Validate()
    {
        if (Id == Guid.Empty || SessionId == Guid.Empty || CreatedAt == default || ModifiedAt < CreatedAt || !Enum.IsDefined(Status)
            || string.IsNullOrWhiteSpace(Text) || Text.Length > Session.MaximumTextLength)
            throw new ArgumentException("Invalid session follow-up metadata or text length.");
    }
}
