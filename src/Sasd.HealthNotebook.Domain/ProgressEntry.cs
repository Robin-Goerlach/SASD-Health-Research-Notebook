namespace Sasd.HealthNotebook.Domain;

/// <summary>Recorded execution, never a health or effectiveness assessment.</summary>
public enum ProgressCompletion { Performed, NotPerformed, Skipped }

/// <summary>An append-only personal routine history entry.</summary>
public sealed record ProgressEntry
{
    /// <summary>Stable identity.</summary>
    public required Guid Id { get; init; }
    /// <summary>Exactly one required routine parent.</summary>
    public required Guid RoutineId { get; init; }
    /// <summary>Documented instant with offset.</summary>
    public required DateTimeOffset OccurredAt { get; init; }
    /// <summary>User-selected execution state.</summary>
    public required ProgressCompletion Completion { get; init; }
    /// <summary>Optional personal note, without interpretation.</summary>
    public string? Note { get; init; }
    /// <summary>Technical creation instant.</summary>
    public required DateTimeOffset CreatedAt { get; init; }
    /// <summary>Technical modification instant.</summary>
    public required DateTimeOffset ModifiedAt { get; init; }
    /// <summary>Appends a documented execution, without changing other entries.</summary>
    public static ProgressEntry Create(Guid routineId, DateTimeOffset occurredAt, ProgressCompletion completion, string? note = null)
    {
        var now = DateTimeOffset.Now;
        var entry = new ProgressEntry { Id = Guid.NewGuid(), RoutineId = routineId, OccurredAt = occurredAt,
            Completion = completion, Note = note, CreatedAt = now, ModifiedAt = now };
        entry.Validate(); return entry;
    }
    /// <summary>Validates structure and bounds only.</summary>
    public void Validate()
    {
        if (Id == Guid.Empty || RoutineId == Guid.Empty || OccurredAt == default || CreatedAt == default
            || ModifiedAt < CreatedAt || !Enum.IsDefined(Completion) || Note?.Length > HealthAction.MaximumTextLength)
            throw new ArgumentException("Invalid progress metadata or text length.");
    }
}
