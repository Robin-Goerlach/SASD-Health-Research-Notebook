namespace Sasd.HealthNotebook.Domain;

/// <summary>A time-related notebook entry. Content is stored without medical evaluation.</summary>
public sealed class HealthEntry
{
    /// <summary>Maximum title length, matching HealthTopic.</summary>
    public const int MaximumTitleLength = 160;
    /// <summary>Maximum content length, matching HealthTopic notes.</summary>
    public const int MaximumContentLength = 4000;
    /// <summary>Stable identifier.</summary>
    public required Guid Id { get; init; }
    /// <summary>Optional single topic reference.</summary>
    public Guid? HealthTopicId { get; init; }
    /// <summary>User-selected category.</summary>
    public required HealthEntryType EntryType { get; init; }
    /// <summary>When the documented event occurred, including its UTC offset.</summary>
    public required DateTimeOffset OccurredAt { get; init; }
    /// <summary>Required user-entered title.</summary>
    public required string Title { get; init; }
    /// <summary>User-entered content; must never be logged.</summary>
    public required string Content { get; init; }
    /// <summary>Technical creation time.</summary>
    public required DateTimeOffset CreatedAt { get; init; }
    /// <summary>Technical modification time (equal to creation time in this create-only slice).</summary>
    public required DateTimeOffset ModifiedAt { get; init; }

    /// <summary>Creates a validated entry without truncating user content.</summary>
    public static HealthEntry Create(HealthEntryType entryType, DateTimeOffset occurredAt,
        string title, string? content, Guid? healthTopicId = null)
    {
        var now = DateTimeOffset.Now;
        var entry = new HealthEntry { Id = Guid.NewGuid(), HealthTopicId = healthTopicId,
            EntryType = entryType, OccurredAt = occurredAt, Title = (title ?? string.Empty).Trim(),
            Content = content ?? string.Empty, CreatedAt = now, ModifiedAt = now };
        entry.Validate();
        return entry;
    }

    /// <summary>Checks domain invariants, including entities read from persistence.</summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Title) || Title.Length > MaximumTitleLength)
            throw new ArgumentException("An entry requires a title of at most 160 characters.", nameof(Title));
        if (Content is null || Content.Length > MaximumContentLength)
            throw new ArgumentException("Entry content must not exceed 4000 characters.", nameof(Content));
        if (!Enum.IsDefined(EntryType) || Id == Guid.Empty || HealthTopicId == Guid.Empty
            || OccurredAt == default || CreatedAt == default || ModifiedAt < CreatedAt)
            throw new ArgumentException("Invalid entry metadata.");
    }
}
