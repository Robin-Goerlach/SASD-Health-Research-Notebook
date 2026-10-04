using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.Application.Contracts;

/// <summary>
/// Read model used by the UI to display health topics without exposing unnecessary details.
/// </summary>
public sealed class HealthTopicSummary
{
    /// <summary>
    /// Gets or sets the stable identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the topic title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the documentation status.
    /// </summary>
    public HealthTopicStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the personal organization priority.
    /// </summary>
    public HealthTopicPriority Priority { get; set; }

    /// <summary>
    /// Gets or sets a short description.
    /// </summary>
    public string ShortDescription { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the creation date.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>Displayed optimistic concurrency token.</summary>
    public DateTimeOffset ModifiedAt { get; set; }

    /// <summary>
    /// Creates a list summary from a domain entity.
    /// </summary>
    public static HealthTopicSummary FromTopic(HealthTopic topic)
    {
        ArgumentNullException.ThrowIfNull(topic);

        return new HealthTopicSummary
        {
            Id = topic.Id,
            Title = topic.Title,
            Status = topic.Status,
            Priority = topic.Priority,
            ShortDescription = topic.ShortDescription,
            CreatedAt = topic.CreatedAt, ModifiedAt = topic.ModifiedAt
        };
    }
}
