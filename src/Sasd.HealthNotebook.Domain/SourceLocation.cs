namespace Sasd.HealthNotebook.Domain;

/// <summary>A concrete locator within exactly one source; no copied source metadata.</summary>
public sealed class SourceLocation
{
    /// <summary>Maximum locator length.</summary>
    public const int MaximumLocatorLength = 160;
    /// <summary>Maximum optional note length.</summary>
    public const int MaximumNoteLength = 4000;
    /// <summary>Stable identifier.</summary>
    public required Guid Id { get; init; }
    /// <summary>Owning source identifier.</summary>
    public required Guid SourceId { get; init; }
    /// <summary>User-selected locator category.</summary>
    public required SourceLocationType LocationType { get; init; }
    /// <summary>Required explicit locator.</summary>
    public required string Locator { get; init; }
    /// <summary>Optional user-entered note; must not be logged.</summary>
    public string? Note { get; init; }
    /// <summary>Technical creation time.</summary>
    public required DateTimeOffset CreatedAt { get; init; }
    /// <summary>Creates a validated locator without interpreting its contents.</summary>
    public static SourceLocation Create(Guid sourceId, SourceLocationType type, string locator, string? note = null)
    {
        var location = new SourceLocation { Id = Guid.NewGuid(), SourceId = sourceId, LocationType = type,
            Locator = locator?.Trim() ?? string.Empty, Note = note, CreatedAt = DateTimeOffset.Now };
        location.Validate();
        return location;
    }
    /// <summary>Checks entity invariants. Source existence is checked by Application/storage.</summary>
    public void Validate()
    {
        if (Id == Guid.Empty || SourceId == Guid.Empty || !Enum.IsDefined(LocationType) || CreatedAt == default
            || string.IsNullOrWhiteSpace(Locator) || Locator.Length > MaximumLocatorLength || Note?.Length > MaximumNoteLength)
            throw new ArgumentException("Invalid source location; check locator and text lengths.");
    }
}
