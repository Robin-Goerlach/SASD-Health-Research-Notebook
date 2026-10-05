using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.Application.Contracts;

/// <summary>Timeline projection; topic names are resolved from the shared topic repository.</summary>
public sealed record HealthEntrySummary(Guid Id, Guid? HealthTopicId, string? HealthTopicTitle,
    HealthEntryType EntryType, DateTimeOffset OccurredAt, string Title, string Content)
{
    /// <summary>Version of the displayed record; destructive commands retain this conflict token.</summary>
    public DateTimeOffset ModifiedAt { get; init; }
}
