using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.Application.Services;

/// <summary>Projects documented sessions without writes, notifications or inferred tasks.</summary>
public static class DashboardAgendaProjector
{
    /// <summary>
    /// Uses the supplied instant for appointments and its local calendar date for
    /// follow-ups. The caller supplies local now; fixed offsets in tests make midnight
    /// behavior independent of the machine clock/time zone. DueDate is a calendar
    /// date, not a reminder instant. No timezone conversion is applied to DueDate.
    /// </summary>
    public static DashboardAgenda Project(SessionNotebook notebook, DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(notebook);
        var today = DateOnly.FromDateTime(now.DateTime);
        var visibleParents = notebook.Sessions.Where(session => !session.IsArchived).ToDictionary(session => session.Id);
        var sessions = visibleParents.Values
            .Where(session => session.Status == SessionStatus.Planned && session.ScheduledAt >= now)
            .OrderBy(session => session.ScheduledAt).ThenBy(session => session.CreatedAt).ThenBy(session => session.Id)
            .Select(session => new AgendaSession(session.Id, session.ScheduledAt, session.Title, session.ContactText)).ToArray();

        // A completed conversation can still require a callback or other user-entered
        // next step. Parent business status must not hide that unfinished documentation.
        var followUps = notebook.FollowUps
            .Where(item => item.Status == SessionFollowUpStatus.Open && visibleParents.ContainsKey(item.SessionId))
            .OrderBy(item => Group(item.DueDate, today)).ThenBy(item => item.DueDate)
            .ThenBy(item => item.CreatedAt).ThenBy(item => item.Id)
            .Select(item => new AgendaFollowUp(item.SessionId, item.Id, item.Text, item.DueDate, Group(item.DueDate, today))).ToArray();

        // Routine.ScheduleText intentionally has no role here: personal free text is
        // not a recurrence rule and must never silently create scheduled work.
        return new DashboardAgenda(sessions, followUps);
    }

    private static FollowUpDueGroup Group(DateOnly? dueDate, DateOnly today) => dueDate is null
        ? FollowUpDueGroup.NoDate : dueDate < today ? FollowUpDueGroup.Overdue
        : dueDate == today ? FollowUpDueGroup.Today : FollowUpDueGroup.Later;
}
