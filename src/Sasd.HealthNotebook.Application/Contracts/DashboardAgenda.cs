namespace Sasd.HealthNotebook.Application.Contracts;

/// <summary>Calendar grouping only; never a medical urgency or notification state.</summary>
public enum FollowUpDueGroup { Overdue, Today, Later, NoDate }

/// <summary>Read-only upcoming appointment with stable navigation identity.</summary>
public sealed record AgendaSession(Guid SessionId, DateTimeOffset ScheduledAt, string Title, string? ContactText);

/// <summary>Read-only open next step; parent and child IDs support exact navigation.</summary>
public sealed record AgendaFollowUp(Guid SessionId, Guid FollowUpId, string Text, DateOnly? DueDate, FollowUpDueGroup Group);

/// <summary>
/// Ephemeral projection of one coherent session snapshot. This is not a persisted
/// cache, reminder schedule or duplicate source of session truth.
/// </summary>
public sealed record DashboardAgenda(IReadOnlyList<AgendaSession> Sessions, IReadOnlyList<AgendaFollowUp> FollowUps);
