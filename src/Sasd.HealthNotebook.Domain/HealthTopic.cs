namespace Sasd.HealthNotebook.Domain;

/// <summary>
/// Represents one documented health topic.
///
/// A health topic can be an illness, a suspected condition, an unresolved symptom cluster,
/// a long-term observation, or a question that should be discussed with a doctor.
///
/// The entity deliberately stores documentation data only. It does not contain logic for
/// diagnosis, treatment recommendation, medication dosing, or medical decision making.
/// </summary>
public sealed class HealthTopic
{
    /// <summary>
    /// Creates a new instance for serializers and repository code.
    /// Application code should prefer <see cref="Create" /> so that basic validation is applied.
    /// </summary>
    public HealthTopic()
    {
        Title = string.Empty;
        ShortDescription = string.Empty;
        Notes = string.Empty;
    }

    /// <summary>
    /// Gets or sets the stable technical identifier of the topic.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the user-visible title of the topic.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the documentation status selected by the user.
    /// </summary>
    public HealthTopicStatus Status { get; set; }

    /// <summary>Previous status for explicit reactivation; absent in legacy stores.</summary>
    public HealthTopicStatus? StatusBeforeArchive { get; set; }

    /// <summary>
    /// Gets or sets the personal organization priority selected by the user.
    /// </summary>
    public HealthTopicPriority Priority { get; set; }

    /// <summary>
    /// Gets or sets a short description suitable for a list or dashboard.
    /// Sensitive details should be kept brief because they may be visible on screen.
    /// </summary>
    public string ShortDescription { get; set; }

    /// <summary>
    /// Gets or sets free-form notes for the topic.
    /// The application must not write this content to logs or error reports.
    /// </summary>
    public string Notes { get; set; }

    /// <summary>
    /// Gets or sets the time at which this topic was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the time at which this topic was last modified.
    /// </summary>
    public DateTimeOffset ModifiedAt { get; set; }

    /// <summary>
    /// Creates a validated health topic with safe defaults.
    /// </summary>
    /// <param name="title">User-visible topic title.</param>
    /// <param name="status">Documentation status.</param>
    /// <param name="priority">Personal organization priority.</param>
    /// <param name="shortDescription">Optional short description.</param>
    /// <param name="notes">Optional notes.</param>
    /// <returns>A new health topic entity.</returns>
    /// <exception cref="ArgumentException">Thrown when the title is empty or too long.</exception>
    public static HealthTopic Create(
        string title,
        HealthTopicStatus status,
        HealthTopicPriority priority,
        string? shortDescription,
        string? notes)
    {
        string normalizedTitle = NormalizeRequiredTitle(title);
        DateTimeOffset now = DateTimeOffset.Now;

        return new HealthTopic
        {
            Id = Guid.NewGuid(),
            Title = normalizedTitle,
            Status = status,
            Priority = priority,
            ShortDescription = NormalizeOptionalText(shortDescription, 500),
            Notes = NormalizeOptionalText(notes, 4000),
            CreatedAt = now,
            ModifiedAt = now
        };
    }

    /// <summary>
    /// Updates the editable fields of the topic.
    /// Validates all inputs before changing fields; no partial correction or text truncation.
    /// </summary>
    public void UpdateDocumentation(
        string title,
        HealthTopicStatus status,
        HealthTopicPriority priority,
        string? shortDescription,
        string? notes)
    {
        string normalizedTitle = NormalizeRequiredTitle(title);
        if (!Enum.IsDefined(status) || !Enum.IsDefined(priority) || shortDescription?.Length > 500 || notes?.Length > 4000)
            throw new ArgumentException("Invalid topic documentation fields.");
        string description = NormalizeOptionalText(shortDescription, 500), normalizedNotes = NormalizeOptionalText(notes, 4000);
        if (Title == normalizedTitle && Status == status && Priority == priority && ShortDescription == description && Notes == normalizedNotes) return;
        Title = normalizedTitle; Status = status; Priority = priority;
        ShortDescription = description; Notes = normalizedNotes;
        var now = DateTimeOffset.Now; ModifiedAt = now > ModifiedAt ? now : ModifiedAt.AddTicks(1);
    }

    private static string NormalizeRequiredTitle(string title)
    {
        string normalized = (title ?? string.Empty).Trim();

        if (normalized.Length == 0)
        {
            throw new ArgumentException("A health topic requires a title.", nameof(title));
        }

        if (normalized.Length > 160)
        {
            throw new ArgumentException("The health topic title is too long. Please use 160 characters or fewer.", nameof(title));
        }

        return normalized;
    }

    private static string NormalizeOptionalText(string? value, int maxLength)
    {
        string normalized = (value ?? string.Empty).Trim();

        if (normalized.Length > maxLength)
        {
            return normalized[..maxLength];
        }

        return normalized;
    }
}
