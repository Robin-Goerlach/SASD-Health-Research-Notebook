namespace Sasd.HealthNotebook.Domain;

/// <summary>Small personal organization categories, without clinical classification.</summary>
public enum HealthActionType { Movement, Nutrition, Sleep, Relaxation, Organization, Other }
/// <summary>Reported origin, not verification of a professional instruction.</summary>
public enum HealthActionOrigin { SelfDefined, Doctor, Therapist, Coach, Source, Other }
/// <summary>User-selected state; no automatic transitions.</summary>
public enum HealthActionStatus { Active, Inactive }

/// <summary>A user-documented action with immutable content and provenance in Slice 1.</summary>
public sealed record HealthAction
{
    /// <summary>Maximum title or short schedule length.</summary>
    public const int MaximumTitleLength = 160;
    /// <summary>Maximum free-text length.</summary>
    public const int MaximumTextLength = 4000;
    /// <summary>Separate archive flag; absent in v1 stores means false. Business status is preserved.</summary>
    public bool IsArchived { get; init; }
    /// <summary>Stable identity.</summary>
    public required Guid Id { get; init; }
    /// <summary>User-entered title.</summary>
    public required string Title { get; init; }
    /// <summary>Optional topic reference; no copied title or cascade.</summary>
    public Guid? HealthTopicId { get; init; }
    /// <summary>Optional reported source reference; content is not copied.</summary>
    public Guid? SourceId { get; init; }
    /// <summary>Optional reported conversation reference; content is not copied.</summary>
    public Guid? SessionId { get; init; }
    /// <summary>Personal organization category.</summary>
    public required HealthActionType ActionType { get; init; }
    /// <summary>Origin as reported by the user.</summary>
    public required HealthActionOrigin Origin { get; init; }
    /// <summary>Optional exact user-entered origin detail.</summary>
    public string? OriginNote { get; init; }
    /// <summary>Optional user-entered description.</summary>
    public string? Description { get; init; }
    /// <summary>User-selected state.</summary>
    public required HealthActionStatus Status { get; init; }
    /// <summary>Technical creation instant.</summary>
    public required DateTimeOffset CreatedAt { get; init; }
    /// <summary>Technical modification instant.</summary>
    public required DateTimeOffset ModifiedAt { get; init; }
    /// <summary>Creates documentation without deriving any action from health data.</summary>
    public static HealthAction Create(string title, HealthActionType type = HealthActionType.Other,
        HealthActionStatus status = HealthActionStatus.Active, Guid? healthTopicId = null,
        string? description = null, HealthActionOrigin origin = HealthActionOrigin.SelfDefined, string? originNote = null, Guid? sourceId = null, Guid? sessionId = null)
    {
        var now = DateTimeOffset.Now;
        var action = new HealthAction { Id = Guid.NewGuid(), Title = title?.Trim() ?? string.Empty,
            ActionType = type, Status = status, HealthTopicId = healthTopicId, Description = description,
            Origin = origin, OriginNote = originNote, SourceId = sourceId, SessionId = sessionId, CreatedAt = now, ModifiedAt = now };
        action.Validate(); return action;
    }
    /// <summary>Checks structural rules with content-free errors.</summary>
    public void Validate()
    {
        if (Id == Guid.Empty || HealthTopicId == Guid.Empty || SourceId == Guid.Empty || SessionId == Guid.Empty || CreatedAt == default || ModifiedAt < CreatedAt
            || !Enum.IsDefined(ActionType) || !Enum.IsDefined(Status) || !Enum.IsDefined(Origin)
            || string.IsNullOrWhiteSpace(Title) || Title.Length > MaximumTitleLength
            || Description?.Length > MaximumTextLength || OriginNote?.Length > MaximumTextLength)
            throw new ArgumentException("Invalid action metadata or text length.");
    }
}
