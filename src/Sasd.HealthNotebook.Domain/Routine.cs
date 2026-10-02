namespace Sasd.HealthNotebook.Domain;

/// <summary>Explicit personal routine state; pausing never deletes history.</summary>
public enum RoutineStatus { Active, Paused }

/// <summary>User-configured regular execution of one action, without scheduling automation.</summary>
public sealed record Routine
{
    /// <summary>Stable identity.</summary>
    public required Guid Id { get; init; }
    /// <summary>Required action parent.</summary>
    public required Guid HealthActionId { get; init; }
    /// <summary>User-entered title.</summary>
    public required string Title { get; init; }
    /// <summary>Optional description.</summary>
    public string? Description { get; init; }
    /// <summary>Optional personal rhythm, e.g. selected days; never parsed as a reminder.</summary>
    public string? ScheduleText { get; init; }
    /// <summary>Explicit active/paused state.</summary>
    public required RoutineStatus Status { get; init; }
    /// <summary>Technical creation instant.</summary>
    public required DateTimeOffset CreatedAt { get; init; }
    /// <summary>Technical modification instant.</summary>
    public required DateTimeOffset ModifiedAt { get; init; }
    /// <summary>Creates a personal routine without interpreting its rhythm.</summary>
    public static Routine Create(Guid actionId, string title, string? description = null,
        string? scheduleText = null, RoutineStatus status = RoutineStatus.Active)
    {
        var now = DateTimeOffset.Now;
        var routine = new Routine { Id = Guid.NewGuid(), HealthActionId = actionId, Title = title?.Trim() ?? string.Empty,
            Description = description, ScheduleText = scheduleText, Status = status, CreatedAt = now, ModifiedAt = now };
        routine.Validate(); return routine;
    }
    /// <summary>Changes only status; preserves content and history.</summary>
    public Routine WithStatus(RoutineStatus status)
    {
        var changed = this with { Status = status, ModifiedAt = DateTimeOffset.Now > ModifiedAt ? DateTimeOffset.Now : ModifiedAt };
        changed.Validate(); return changed;
    }
    /// <summary>Validates structural rules, not medical suitability.</summary>
    public void Validate()
    {
        if (Id == Guid.Empty || HealthActionId == Guid.Empty || CreatedAt == default || ModifiedAt < CreatedAt
            || !Enum.IsDefined(Status) || string.IsNullOrWhiteSpace(Title) || Title.Length > HealthAction.MaximumTitleLength
            || Description?.Length > HealthAction.MaximumTextLength || ScheduleText?.Length > HealthAction.MaximumTitleLength)
            throw new ArgumentException("Invalid routine metadata or text length.");
    }
}
