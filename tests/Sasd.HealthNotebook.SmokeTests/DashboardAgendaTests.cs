using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.Infrastructure.Storage;

namespace Sasd.HealthNotebook.SmokeTests;

/// <summary>UT-DASH-001/002/003 and IT-DASH-001: synthetic fixed-time, no-write agenda.</summary>
internal static class DashboardAgendaTests
{
    internal static async Task RunAsync()
    {
        var now = new DateTimeOffset(2026, 10, 5, 0, 0, 0, TimeSpan.FromHours(2));
        var today = new DateOnly(2026, 10, 5);
        var parent = Session.Create(now, "CODEX TEST – Agenda", SessionType.DoctorVisit, SessionStatus.Planned)
            with { CreatedAt = now.AddDays(-1), ModifiedAt = now.AddDays(-1) };
        var completed = parent with { Id = Guid.NewGuid(), Status = SessionStatus.Completed };
        var cancelled = parent with { Id = Guid.NewGuid(), Status = SessionStatus.Cancelled };
        var archived = parent with { Id = Guid.NewGuid(), IsArchived = true };
        var past = parent with { Id = Guid.NewGuid(), ScheduledAt = now.AddTicks(-1).ToOffset(TimeSpan.FromHours(14)) };
        var future = parent with { Id = Guid.NewGuid(), ScheduledAt = now.AddHours(1).ToOffset(TimeSpan.FromHours(-5)) };
        var tieA = parent with { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), ScheduledAt = now.ToOffset(TimeSpan.FromHours(-5)), CreatedAt = now.AddDays(-2) };
        var tieB = tieA with { Id = Guid.Parse("00000000-0000-0000-0000-000000000002") };
        var sessions = new[] { future, archived, completed, cancelled, past, parent, tieB, tieA };
        var overdue = SessionFollowUp.Create(parent.Id, "CODEX TEST – Past", dueDate: today.AddDays(-1)) with { CreatedAt = now.AddDays(-1), ModifiedAt = now.AddDays(-1) };
        var dueToday = SessionFollowUp.Create(completed.Id, "CODEX TEST – Today", dueDate: today) with { CreatedAt = now.AddDays(-1), ModifiedAt = now.AddDays(-1) };
        var later = SessionFollowUp.Create(cancelled.Id, "CODEX TEST – Later", dueDate: today.AddDays(1)) with { CreatedAt = now.AddDays(-1), ModifiedAt = now.AddDays(-1) };
        var noDate = SessionFollowUp.Create(parent.Id, "CODEX TEST – No date") with { CreatedAt = now.AddDays(-1), ModifiedAt = now.AddDays(-1) };
        var done = overdue with { Id = Guid.NewGuid(), Status = SessionFollowUpStatus.Done };
        var hidden = overdue with { Id = Guid.NewGuid(), SessionId = archived.Id };
        var stepTieA = dueToday with { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), CreatedAt = now.AddDays(-2) };
        var stepTieB = stepTieA with { Id = Guid.Parse("00000000-0000-0000-0000-000000000004") };
        var steps = new[] { noDate, hidden, later, done, dueToday, overdue, stepTieB, stepTieA };
        var notebook = new SessionNotebook(sessions, Array.Empty<SessionQuestion>(), steps);
        var agenda = DashboardAgendaProjector.Project(notebook, now);
        Check(agenda.Sessions.Select(item => item.SessionId).SequenceEqual(new[] { tieA.Id, tieB.Id, parent.Id, future.Id }), "Upcoming status/instant/tie rules failed.");
        Check(agenda.FollowUps.Select(item => item.FollowUpId).SequenceEqual(new[] { overdue.Id, stepTieA.Id, stepTieB.Id, dueToday.Id, later.Id, noDate.Id }), "Open/archive/date/tie rules failed.");
        Check(agenda.FollowUps.Select(item => item.Group).SequenceEqual(new[] { FollowUpDueGroup.Overdue, FollowUpDueGroup.Today, FollowUpDueGroup.Today, FollowUpDueGroup.Today, FollowUpDueGroup.Later, FollowUpDueGroup.NoDate }), "Calendar groups failed.");
        var reversed = DashboardAgendaProjector.Project(new(sessions.AsEnumerable().Reverse().ToArray(), Array.Empty<SessionQuestion>(), steps.AsEnumerable().Reverse().ToArray()), now);
        Check(agenda.Sessions.SequenceEqual(reversed.Sessions) && agenda.FollowUps.SequenceEqual(reversed.FollowUps), "Input order affected projection.");
        var beforeMidnight = DashboardAgendaProjector.Project(notebook, now.AddTicks(-1));
        Check(beforeMidnight.FollowUps.Single(item => item.FollowUpId == overdue.Id).Group == FollowUpDueGroup.Today
            && beforeMidnight.FollowUps.Single(item => item.FollowUpId == dueToday.Id).Group == FollowUpDueGroup.Later, "Midnight boundary failed.");
        var nextDay = DashboardAgendaProjector.Project(notebook, now.AddDays(1));
        Check(nextDay.FollowUps.Single(item => item.FollowUpId == dueToday.Id).Group == FollowUpDueGroup.Overdue
            && nextDay.FollowUps.Single(item => item.FollowUpId == later.Id).Group == FollowUpDueGroup.Today, "Day change failed.");
        var sameInstantOtherDate = DashboardAgendaProjector.Project(notebook, now.ToOffset(TimeSpan.FromHours(-5)));
        Check(agenda.Sessions.SequenceEqual(sameInstantOtherDate.Sessions)
            && sameInstantOtherDate.FollowUps.Single(item => item.FollowUpId == overdue.Id).Group == FollowUpDueGroup.Today, "Offset instant/calendar distinction failed.");
        Check(DashboardAgendaProjector.Project(notebook, now).FollowUps.SequenceEqual(agenda.FollowUps), "Projection used system time.");

        // Existing persisted fixtures must remain byte-for-byte untouched by agenda reads.
        string root = LocalHealthNotebookPaths.DataDirectory;
        var files = Directory.GetFiles(root).ToDictionary(path => path, File.ReadAllBytes);
        var service = new SessionService(new JsonSessionRepository(), new JsonHealthTopicRepository());
        await service.GetDashboardAgendaAsync(now);
        await service.GetDashboardAgendaAsync(now.AddDays(1));
        Check(files.Keys.Order().SequenceEqual(Directory.GetFiles(root).Order()), "Agenda created a store/cache.");
        Check(files.All(pair => pair.Value.SequenceEqual(File.ReadAllBytes(pair.Key))), "Agenda rewrote primary/backup data.");

        string emptyRoot = Path.Combine(root, "agenda-read-guards");
        Directory.CreateDirectory(emptyRoot);
        Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, emptyRoot);
        try
        {
            service = new SessionService(new JsonSessionRepository(), new JsonHealthTopicRepository());
            var empty = await service.GetDashboardAgendaAsync(now);
            Check(empty.Sessions.Count == 0 && empty.FollowUps.Count == 0 && Directory.GetFiles(emptyRoot).Length == 0, "Missing store was written.");
            string storePath = Path.Combine(emptyRoot, "sessions.json");
            await File.WriteAllTextAsync(storePath, "{ invalid synthetic store");
            bool failed = false;
            try { await service.GetDashboardAgendaAsync(now); }
            catch { failed = true; }
            Check(failed && await File.ReadAllTextAsync(storePath) == "{ invalid synthetic store", "Corruption became empty data or was rewritten.");
        }
        finally { Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, root); }
        Console.WriteLine("Dashboard agenda fixed-time/projection/no-write checks passed (UT/IT-DASH-001/002/003).");
    }

    private static void Check(bool condition, string message)
    { if (!condition) throw new InvalidOperationException(message); }
}
