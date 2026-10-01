using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.Application.Contracts;

/// <summary>User input for a new timeline entry.</summary>
public sealed class CreateHealthEntryRequest
{
    /// <summary>Optional topic identifier.</summary>
    public Guid? HealthTopicId { get; init; }
    /// <summary>Documentation category.</summary>
    public HealthEntryType EntryType { get; init; }
    /// <summary>Documented event time.</summary>
    public DateTimeOffset OccurredAt { get; init; }
    /// <summary>Required title.</summary>
    public string Title { get; init; } = string.Empty;
    /// <summary>Optional text.</summary>
    public string Content { get; init; } = string.Empty;
}
