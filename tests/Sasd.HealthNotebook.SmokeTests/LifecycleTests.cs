using System.Text.Json.Nodes;
using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.Infrastructure.Storage;

namespace Sasd.HealthNotebook.SmokeTests;

/// <summary>IT/SEC/MIG-LIF-001: corrections, archives, audit and safe deletion using synthetic local records.</summary>
internal static class LifecycleTests
{
    public static async Task RunAsync()
    {
        string incoming = LocalHealthNotebookPaths.DataDirectory;
        string root = Path.Combine(incoming, "lifecycle-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, root);
        try
        {
            var topics = new JsonHealthTopicRepository();
            var topic = HealthTopic.Create("CODEX TEST – lifecycle topic", HealthTopicStatus.Archived, HealthTopicPriority.Normal, null, null);
            await topics.AddAsync(topic);
            var source = Source.Create(SourceType.ProfessionalStatement, "CODEX TEST – lifecycle source", null, null, null, null, null, null);
            await new JsonSourceRepository().AddSourceAsync(source);
            var untouched = new[] { "health-topics.json", "sources.json" }.ToDictionary(name => name, name => File.ReadAllBytes(Path.Combine(root, name)));
            var measurements = new MeasurementService(new JsonMeasurementRepository(), topics);
            var entries = new HealthEntryService(new JsonHealthEntryRepository(), topics);
            var sessions = new SessionService(new JsonSessionRepository(), topics);
            var actions = Actions();
            var time = new DateTimeOffset(2026, 9, 1, 12, 0, 0, TimeSpan.FromHours(2));

            await measurements.CreateMeasurementAsync(new() { MeasurementType = MeasurementType.Weight, OccurredAt = time, Value = 12, HealthTopicId = topic.Id });
            var m = (await new JsonMeasurementRepository().GetAllAsync()).Single();
            await measurements.UpdateMeasurementAsync(m.Id, new() { MeasurementType = m.MeasurementType, OccurredAt = time.AddDays(1), Value = 13, HealthTopicId = topic.Id }, m.ModifiedAt);
            var updatedM = await measurements.GetByIdAsync(m.Id);
            Check(updatedM.Id == m.Id && updatedM.CreatedAt == m.CreatedAt && updatedM.ModifiedAt > m.ModifiedAt
                && updatedM.Value == 13 && updatedM.OccurredAt == time.AddDays(1) && updatedM.HealthTopicId == topic.Id, "Measurement correction metadata.");
            await measurements.UpdateMeasurementAsync(m.Id, new() { MeasurementType = m.MeasurementType, OccurredAt = updatedM.OccurredAt, Value = 13 }, updatedM.ModifiedAt);
            updatedM = await measurements.GetByIdAsync(m.Id);
            Check(updatedM.HealthTopicId is null, "Explicit topic change failed.");
            await Fails<LifecycleConflictException>(() => measurements.DeleteMeasurementAsync(m.Id, m.ModifiedAt));
            await NoRewrite("measurements.json", () => measurements.UpdateMeasurementAsync(m.Id, new() { MeasurementType = m.MeasurementType, OccurredAt = updatedM.OccurredAt, Value = 13 }, updatedM.ModifiedAt));
            await Fails<ArgumentException>(() => measurements.UpdateMeasurementAsync(m.Id, new() { MeasurementType = m.MeasurementType, OccurredAt = time, Value = double.NaN }, updatedM.ModifiedAt));
            await Guards("measurements.json", () => measurements.DeleteMeasurementAsync(m.Id, updatedM.ModifiedAt));
            byte[] beforeDelete = File.ReadAllBytes(LocalHealthNotebookPaths.MeasurementsFilePath);
            await measurements.DeleteMeasurementAsync(m.Id, updatedM.ModifiedAt);
            Check((await new JsonMeasurementRepository().GetAllAsync()).Count == 0, "Deleted measurement reloaded.");
            Check(beforeDelete.SequenceEqual(File.ReadAllBytes(Path.Combine(root, "measurements.backup.json"))), "Delete backup is not last-good bytes.");

            await entries.CreateEntryAsync(new() { Title = "CODEX TEST – entry", Content = "Synthetic original", EntryType = HealthEntryType.Note, OccurredAt = time, HealthTopicId = topic.Id });
            var e = (await new JsonHealthEntryRepository().GetAllAsync()).Single();
            await entries.UpdateHealthEntryAsync(e.Id, new() { Title = "CODEX TEST – corrected", Content = "Synthetic corrected", EntryType = e.EntryType, OccurredAt = time, HealthTopicId = e.HealthTopicId }, e.ModifiedAt);
            var updatedE = await entries.GetByIdAsync(e.Id);
            Check(updatedE.Id == e.Id && updatedE.CreatedAt == e.CreatedAt && updatedE.ModifiedAt > e.ModifiedAt
                && updatedE.Content == "Synthetic corrected" && updatedE.Title == "CODEX TEST – corrected" && updatedE.HealthTopicId == topic.Id, "Entry correction metadata.");
            await Guards("health-entries.json", () => entries.DeleteHealthEntryAsync(e.Id, updatedE.ModifiedAt));
            await entries.DeleteHealthEntryAsync(e.Id, updatedE.ModifiedAt);
            Check((await new JsonHealthEntryRepository().GetAllAsync()).Count == 0, "Deleted entry reloaded.");

            var s = await sessions.CreateSessionAsync(new(time, "CODEX TEST – session", Status: SessionStatus.Completed, HealthTopicId: topic.Id));
            var q = await sessions.CreateQuestionAsync(new(s.Id, "Synthetic question", AnswerNote: "Synthetic answer"));
            var f = await sessions.CreateFollowUpAsync(new(s.Id, "Synthetic next step"));
            await sessions.UpdateSessionAsync(s.Id, new(time.AddHours(1), "CODEX TEST – corrected session", Status: s.Status, HealthTopicId: topic.Id, Notes: "Synthetic notes"), s.ModifiedAt);
            var updatedS = (await sessions.GetSessionDetailsAsync(s.Id)).Sessions.Single();
            Check(updatedS.CreatedAt == s.CreatedAt && updatedS.ModifiedAt > s.ModifiedAt && updatedS.ScheduledAt == time.AddHours(1), "Session edit metadata.");
            await sessions.ArchiveSessionAsync(s.Id, updatedS.ModifiedAt);
            updatedS = (await new JsonSessionRepository().LoadAsync()).Sessions.Single();
            Check(updatedS.IsArchived && updatedS.Status == SessionStatus.Completed, "Archive changed session business status.");
            await sessions.ReactivateSessionAsync(s.Id, updatedS.ModifiedAt);
            var sessionDetails = await new SessionService(new JsonSessionRepository(), topics).GetSessionDetailsAsync(s.Id);
            Check(!sessionDetails.Sessions.Single().IsArchived && sessionDetails.Questions.Single() == q && sessionDetails.FollowUps.Single() == f, "Session children lost.");

            var a = await actions.CreateActionAsync(new("CODEX TEST – professional instruction", Origin: HealthActionOrigin.Doctor, Description: "Synthetic previous instruction", SourceId: source.Id, SessionId: s.Id));
            var r = await actions.CreateRoutineAsync(new(a.Id, "CODEX TEST – routine", ScheduleText: "Synthetic rhythm"));
            var p = await actions.CreateProgressEntryAsync(new(r.Id, time, Note: "Synthetic progress", Count: 1));
            await actions.UpdateHealthActionAsync(a.Id, new("CODEX TEST – corrected instruction", Origin: HealthActionOrigin.SelfDefined, Description: "Synthetic corrected instruction", SourceId: source.Id, SessionId: s.Id), a.ModifiedAt, "Synthetic typo correction");
            var details = await Actions().GetActionDetailsAsync(a.Id);
            var updatedA = details.Actions.Single();
            Check(updatedA.Id == a.Id && updatedA.CreatedAt == a.CreatedAt && updatedA.ModifiedAt > a.ModifiedAt && updatedA.SourceId == source.Id && updatedA.SessionId == s.Id, "Action correction metadata.");
            var revision = details.Revisions.Single();
            Check(revision.Previous == a && revision.HealthActionId == a.Id && revision.ChangedAt == updatedA.ModifiedAt && revision.ChangeReason == "Synthetic typo correction", "Professional previous content/provenance lost.");
            await NoRewrite("health-actions.json", () => actions.UpdateHealthActionAsync(a.Id, new(updatedA.Title, Origin: updatedA.Origin, Description: updatedA.Description, SourceId: source.Id, SessionId: s.Id), updatedA.ModifiedAt));
            await Fails<LifecycleConflictException>(() => actions.UpdateHealthActionAsync(a.Id, new("CODEX TEST – stale"), a.ModifiedAt));
            await actions.ArchiveHealthActionAsync(a.Id, updatedA.ModifiedAt);
            updatedA = (await Actions().GetActionDetailsAsync(a.Id)).Actions.Single();
            Check(updatedA.IsArchived && (await actions.GetOverviewAsync(DateOnly.FromDateTime(time.LocalDateTime))).ActiveActions == 0, "Archived action counted as active.");
            Check((await actions.GetOverviewAsync(DateOnly.FromDateTime(time.LocalDateTime))).ActiveRoutines == 1, "Archive silently paused routine.");
            await actions.ReactivateHealthActionAsync(a.Id, updatedA.ModifiedAt);
            details = await Actions().GetActionDetailsAsync(a.Id);
            Check(!details.Actions.Single().IsArchived && details.Revisions.Single() == revision && details.Routines.Single() == r && details.ProgressEntries.Single() == p, "Archive/reactivation changed children or audit.");

            await actions.UpdateRoutineAsync(r.Id, new(a.Id, "CODEX TEST – corrected routine", "Synthetic description", "Synthetic changed rhythm", RoutineStatus.Paused), r.ModifiedAt);
            var updatedR = (await Actions().GetActionDetailsAsync(a.Id)).Routines.Single();
            Check(updatedR.CreatedAt == r.CreatedAt && updatedR.ModifiedAt > r.ModifiedAt && updatedR.ScheduleText == "Synthetic changed rhythm", "Routine correction.");
            Check((await actions.GetOverviewAsync(DateOnly.FromDateTime(time.LocalDateTime))).ActiveRoutines == 0, "Paused routine counted active.");
            await actions.SetRoutineStatusAsync(r.Id, RoutineStatus.Active);
            await actions.SetRoutineStatusAsync(r.Id, RoutineStatus.Paused);
            Check((await Actions().GetActionDetailsAsync(a.Id)).ProgressEntries.Single() == p, "Pause lost history.");
            updatedR = (await Actions().GetActionDetailsAsync(a.Id)).Routines.Single();
            await Fails<ArgumentException>(() => actions.UpdateRoutineAsync(r.Id, new(Guid.NewGuid(), "CODEX TEST – moved"), updatedR.ModifiedAt));
            await actions.UpdateProgressEntryAsync(p.Id, new(r.Id, time.AddDays(1), ProgressCompletion.Skipped, "Synthetic corrected progress", 2), p.ModifiedAt);
            var updatedP = (await Actions().GetActionDetailsAsync(a.Id)).ProgressEntries.Single();
            Check(updatedP.Id == p.Id && updatedP.CreatedAt == p.CreatedAt && updatedP.ModifiedAt > p.ModifiedAt && updatedP.Count == 2 && updatedP.Completion == ProgressCompletion.Skipped, "Progress correction metadata.");
            await Fails<ArgumentException>(() => actions.UpdateProgressEntryAsync(p.Id, new(Guid.NewGuid(), time), updatedP.ModifiedAt));
            await Guards("health-actions.json", () => actions.DeleteProgressEntryAsync(p.Id, updatedP.ModifiedAt));
            await actions.DeleteProgressEntryAsync(p.Id, updatedP.ModifiedAt);
            Check((await Actions().GetActionDetailsAsync(a.Id)).ProgressEntries.Count == 0, "Deleted progress reloaded.");
            Check((await actions.GetOverviewAsync(DateOnly.FromDateTime(updatedP.OccurredAt.LocalDateTime))).EntriesToday == 0, "Deleted progress counted.");
            var normal = await actions.CreateActionAsync(new("CODEX TEST – normal"));
            await actions.UpdateHealthActionAsync(normal.Id, new("CODEX TEST – normal correction"), normal.ModifiedAt);
            Check((await Actions().GetActionDetailsAsync(normal.Id)).Revisions.Count == 0, "Global audit was introduced.");
            var sourced = await actions.CreateActionAsync(new("CODEX TEST – source instruction", Origin: HealthActionOrigin.Source, SourceId: source.Id, Description: "Synthetic sourced original"));
            await actions.UpdateHealthActionAsync(sourced.Id, new("CODEX TEST – corrected source instruction", Description: "Synthetic sourced correction"), sourced.ModifiedAt);
            Check((await Actions().GetActionDetailsAsync(sourced.Id)).Revisions.Single().Previous == sourced, "Professional source removal lost prior instruction.");
            foreach (var file in untouched) Check(file.Value.SequenceEqual(File.ReadAllBytes(Path.Combine(root, file.Key))), "Other store changed.");
            // Parent Hard Delete is deliberately unavailable, even for direct repository clients.
            foreach (Type type in new[] { typeof(SessionService), typeof(JsonSessionRepository), typeof(HealthActionService), typeof(JsonHealthActionRepository), typeof(SourceService), typeof(HealthTopicService) })
                Check(!type.GetMethods().Any(method => method.Name is "DeleteSessionAsync" or "DeleteHealthActionAsync" or "DeleteRoutineAsync" or "DeleteSourceAsync" or "DeleteHealthTopicAsync"), "Parent delete unexpectedly available.");
            await OffsetCorrections(time);
            await VersionCompatibility(root);
            var futureRoutine = r with { ModifiedAt = DateTimeOffset.Now.AddDays(1) };
            Check(futureRoutine.WithStatus(RoutineStatus.Paused).ModifiedAt > futureRoutine.ModifiedAt, "Clock rollback reused concurrency token.");
            Console.WriteLine("Lifecycle corrections/archive/delete/audit/compatibility checks passed (FR-LIF-001/002/003, FR-ACT-006).");
        }
        finally { Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, incoming); }
    }

    private static async Task OffsetCorrections(DateTimeOffset time)
    {
        DateTimeOffset corrected = time.ToOffset(TimeSpan.FromHours(1));
        var measurements = new JsonMeasurementRepository();
        var m = Measurement.Create(MeasurementType.Weight, time, 12); await measurements.AddAsync(m);
        await measurements.UpdateAsync(m with { OccurredAt = corrected }, m.ModifiedAt);
        Check((await measurements.GetAllAsync()).Single(item => item.Id == m.Id).OccurredAt.EqualsExact(corrected), "Measurement offset correction lost.");
        var entries = new JsonHealthEntryRepository();
        var e = HealthEntry.Create(HealthEntryType.Note, time, "CODEX TEST – offset", null); await entries.AddAsync(e);
        await entries.UpdateAsync(e with { OccurredAt = corrected }, e.ModifiedAt);
        Check((await entries.GetAllAsync()).Single(item => item.Id == e.Id).OccurredAt.EqualsExact(corrected), "Entry offset correction lost.");
        var sessions = new JsonSessionRepository();
        var s = Session.Create(time, "CODEX TEST – offset", SessionType.Coaching, SessionStatus.Planned); await sessions.AddSessionAsync(s);
        await sessions.UpdateSessionAsync(s with { ScheduledAt = corrected }, s.ModifiedAt);
        Check((await sessions.LoadAsync()).Sessions.Single(item => item.Id == s.Id).ScheduledAt.EqualsExact(corrected), "Session offset correction lost.");
        var actions = new JsonHealthActionRepository();
        var r = (await actions.LoadAsync()).Routines.First();
        var p = ProgressEntry.Create(r.Id, time, ProgressCompletion.Performed); await actions.AddProgressEntryAsync(p);
        await actions.UpdateProgressEntryAsync(p with { OccurredAt = corrected }, p.ModifiedAt);
        Check((await actions.LoadAsync()).ProgressEntries.Single(item => item.Id == p.Id).OccurredAt.EqualsExact(corrected), "Progress offset correction lost.");
    }

    private static async Task VersionCompatibility(string root)
    {
        foreach (string name in new[] { "sessions.json", "health-actions.json" })
        {
            string dir = Path.Combine(root, "v1-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(dir);
            var old = JsonNode.Parse(File.ReadAllText(Path.Combine(root, name)))!;
            old["Version"] = 1; old.AsObject().Remove("Revisions");
            foreach (var item in old[name == "sessions.json" ? "Sessions" : "Actions"]!.AsArray()) item!.AsObject().Remove("IsArchived");
            string path = Path.Combine(dir, name); await File.WriteAllTextAsync(path, old.ToJsonString());
            byte[] before = File.ReadAllBytes(path);
            Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, dir);
            try
            {
                if (name == "sessions.json")
                {
                    var repo = new JsonSessionRepository(); var store = await repo.LoadAsync();
                    Check(store.Sessions.All(item => !item.IsArchived), "v1 archive default wrong.");
                    Check(before.SequenceEqual(File.ReadAllBytes(path)), "Read migrated v1.");
                    var item = store.Sessions.First(); await repo.SetSessionArchivedAsync(item.Id, true, item.ModifiedAt);
                }
                else
                {
                    var repo = new JsonHealthActionRepository(); var store = await repo.LoadAsync();
                    Check(store.Actions.All(item => !item.IsArchived) && store.Revisions.Count == 0, "v1 defaults wrong.");
                    Check(before.SequenceEqual(File.ReadAllBytes(path)), "Read migrated v1.");
                    var item = store.Actions.First(); await repo.SetActionArchivedAsync(item.Id, true, item.ModifiedAt);
                }
                Check(JsonNode.Parse(File.ReadAllText(path))!["Version"]!.GetValue<int>() == 2, "Write did not explicitly upgrade version.");
                Check(before.SequenceEqual(File.ReadAllBytes(Path.Combine(dir, name.Replace(".json", ".backup.json")))), "v1 backup changed.");
            }
            finally { Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, root); }
        }
    }
    private static HealthActionService Actions() => new(new JsonHealthActionRepository(), new JsonHealthTopicRepository(), new JsonSourceRepository(), new JsonSessionRepository());
    private static async Task NoRewrite(string name, Func<Task> action)
    {
        string path = Path.Combine(LocalHealthNotebookPaths.DataDirectory, name);
        byte[] primary = File.ReadAllBytes(path), backup = File.ReadAllBytes(path.Replace(".json", ".backup.json"));
        DateTime modified = File.GetLastWriteTimeUtc(path);
        await action();
        Check(primary.SequenceEqual(File.ReadAllBytes(path)) && backup.SequenceEqual(File.ReadAllBytes(path.Replace(".json", ".backup.json"))) && File.GetLastWriteTimeUtc(path) == modified, "No-op rewrote store/backup.");
    }
    private static async Task Guards(string name, Func<Task> action)
    {
        string path = Path.Combine(LocalHealthNotebookPaths.DataDirectory, name);
        byte[] before = File.ReadAllBytes(path);
        foreach (string suffix in new[] { ".tmp", ".lock" })
        {
            string guard = path + suffix; await File.WriteAllTextAsync(guard, "Synthetic owned guard");
            await Fails<IOException>(action);
            Check(File.ReadAllText(guard) == "Synthetic owned guard" && before.SequenceEqual(File.ReadAllBytes(path)), "Guard or primary lost.");
            File.Delete(guard); // Exact synthetic fixture owned by this test.
        }
        await File.WriteAllTextAsync(path, "{");
        await Fails<System.IO.InvalidDataException>(action);
        Check(File.ReadAllText(path) == "{", "Corrupt primary overwritten.");
        await File.WriteAllBytesAsync(path, before);
    }
    private static void Check(bool value, string message) { if (!value) throw new InvalidOperationException(message); }
    private static async Task Fails<T>(Func<Task> action) where T : Exception
    { try { await action(); } catch (T) { return; } throw new InvalidOperationException("Expected " + typeof(T).Name); }
}
