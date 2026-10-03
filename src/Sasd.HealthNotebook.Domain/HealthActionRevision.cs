namespace Sasd.HealthNotebook.Domain;

/// <summary>Previous professional instruction, retained locally with the action; never a technical log.</summary>
public sealed record HealthActionRevision
{
    /// <summary>Stable revision identity.</summary>
    public required Guid Id { get; init; }
    /// <summary>Existing action parent.</summary>
    public required Guid HealthActionId { get; init; }
    /// <summary>Instant of the correction.</summary>
    public required DateTimeOffset ChangedAt { get; init; }
    /// <summary>Complete previous documented content and provenance.</summary>
    public required HealthAction Previous { get; init; }
    /// <summary>Optional user-entered explanation, never logged.</summary>
    public string? ChangeReason { get; init; }
    /// <summary>Checks identity and chronology without interpreting the instruction.</summary>
    public void Validate()
    {
        if (Id == Guid.Empty || HealthActionId == Guid.Empty || Previous is null
            || Previous.Id != HealthActionId || ChangedAt <= Previous.ModifiedAt
            || ChangeReason?.Length > HealthAction.MaximumTextLength)
            throw new ArgumentException("Invalid action revision metadata.");
        Previous.Validate();
    }
}
