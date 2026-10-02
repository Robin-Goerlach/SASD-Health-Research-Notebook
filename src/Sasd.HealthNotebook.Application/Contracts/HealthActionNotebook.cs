using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.Application.Contracts;

/// <summary>Coherent snapshot of the action/routine/history store.</summary>
public sealed record HealthActionNotebook(IReadOnlyList<HealthAction> Actions, IReadOnlyList<Routine> Routines,
    IReadOnlyList<ProgressEntry> ProgressEntries);
/// <summary>Current topic title resolved for presentation only.</summary>
public sealed record HealthActionSummary(HealthAction Action, string? HealthTopicTitle);
/// <summary>User-entered action and reported provenance.</summary>
public sealed record CreateHealthActionRequest(string Title, HealthActionType ActionType = HealthActionType.Other,
    HealthActionStatus Status = HealthActionStatus.Active, Guid? HealthTopicId = null, string? Description = null,
    HealthActionOrigin Origin = HealthActionOrigin.SelfDefined, string? OriginNote = null);
/// <summary>User-configured routine belonging to one action.</summary>
public sealed record CreateRoutineRequest(Guid HealthActionId, string Title, string? Description = null,
    string? ScheduleText = null, RoutineStatus Status = RoutineStatus.Active);
/// <summary>Explicit personal execution record.</summary>
public sealed record CreateProgressEntryRequest(Guid RoutineId, DateTimeOffset OccurredAt,
    ProgressCompletion Completion = ProgressCompletion.Performed, string? Note = null);
/// <summary>Documentative counts; today uses the user's local calendar date.</summary>
public sealed record HealthActionOverview(int ActiveActions, int ActiveRoutines, int EntriesToday);
