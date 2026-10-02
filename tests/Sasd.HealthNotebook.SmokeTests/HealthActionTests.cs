using System.Text.Json.Nodes;
using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Repositories;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.Infrastructure.Storage;

namespace Sasd.HealthNotebook.SmokeTests;

/// <summary>UT/IT/SEC-ACT-001: synthetic documentation, relationships and safe store failure paths.</summary>
internal static class HealthActionTests
{
    internal static async Task RunAsync()
    {
        string root = LocalHealthNotebookPaths.DataDirectory;
        Assert(root.StartsWith(Path.Combine(Program.FindRepositoryRoot(), ".codex") + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase), "Unsafe action test path.");
        var prior = new Dictionary<string, byte[]>();
        foreach (string name in new[] { "health-topics.json", "health-entries.json", "sources.json", "measurements.json", "sessions.json" })
            prior[name] = await File.ReadAllBytesAsync(Path.Combine(root, name));
        var service = Service();
        var time = new DateTimeOffset(2026, 10, 2, 12, 30, 0, TimeSpan.FromHours(2));
        Guid topicId = (await new JsonHealthTopicRepository().GetAllAsync())[0].Id;
        Assert((await service.GetActionsAsync()).Count == 0, "Missing store not empty.");
        var action = await service.CreateActionAsync(new("  CODEX TEST – Action  ", HealthActionType.Organization,
            HealthTopicId: topicId, Description: "Synthetic action description.", Origin: HealthActionOrigin.Coach, OriginNote: "Synthetic reported origin."));
        var independent = await service.CreateActionAsync(new("CODEX TEST – Independent", Status: HealthActionStatus.Inactive));
        Assert(independent.HealthTopicId is null && action.Title == "CODEX TEST – Action", "Optional topic/title failed.");
        foreach (var type in Enum.GetValues<HealthActionType>())
            await service.CreateActionAsync(new("CODEX TEST – Category", type, Origin: (HealthActionOrigin)(int)type));
        var archivedTopic = new HealthTopic { Id = topicId, Title = "CODEX TEST – Archived current title", Status = HealthTopicStatus.Archived };
        var archived = new HealthActionService(new JsonHealthActionRepository(), new TopicSnapshot(new[] { archivedTopic }));
        var archivedAction = await archived.CreateActionAsync(new("CODEX TEST – Archived reference", HealthTopicId: topicId));
        Assert((await archived.GetActionsAsync()).Single(item => item.Action.Id == action.Id).HealthTopicTitle == archivedTopic.Title, "Current archived topic title not resolved.");
        var missing = new HealthActionService(new JsonHealthActionRepository(), new TopicSnapshot(Array.Empty<HealthTopic>()));
        Assert((await missing.GetActionsAsync()).Single(item => item.Action.Id == action.Id).HealthTopicTitle is null, "Missing topic lost action.");
        var routine = await service.CreateRoutineAsync(new(action.Id, "CODEX TEST – Routine", "Synthetic routine description.", "Synthetic selected days"));
        var otherRoutine = await service.CreateRoutineAsync(new(independent.Id, "CODEX TEST – Other routine", Status: RoutineStatus.Paused));
        var entry = await service.CreateProgressEntryAsync(new(routine.Id, time, ProgressCompletion.Performed, "Synthetic exact note.\r\nSecond line."));
        await service.CreateProgressEntryAsync(new(routine.Id, time.AddDays(-1), ProgressCompletion.NotPerformed));
        await service.CreateProgressEntryAsync(new(routine.Id, time.AddHours(1), ProgressCompletion.Skipped, "Synthetic skip note."));
        // Offset ordering compares instants, not the wall-clock component.
        await service.CreateProgressEntryAsync(new(routine.Id, time.ToOffset(TimeSpan.FromHours(-4)).AddMinutes(30), ProgressCompletion.Performed));
        await service.SetRoutineStatusAsync(routine.Id, RoutineStatus.Paused);
        var fresh = Service();
        var details = await fresh.GetActionDetailsAsync(action.Id);
        Assert(details.Actions.Single() == action && details.Routines.Single().HealthActionId == action.Id
            && details.Routines.Single().Status == RoutineStatus.Paused && details.Routines.Single().ScheduleText == routine.ScheduleText, "Action/routine independent reload failed.");
        Assert(details.ProgressEntries.Single(item => item.Id == entry.Id) == entry, "Progress identity, offset, completion or exact note lost.");
        Assert(details.ProgressEntries.Count == 4 && details.ProgressEntries.Zip(details.ProgressEntries.Skip(1)).All(pair => pair.First.OccurredAt >= pair.Second.OccurredAt), "History chronological ordering failed.");
        await fresh.CreateProgressEntryAsync(new(routine.Id, time.AddDays(-2), ProgressCompletion.NotPerformed, "Synthetic historical entry while paused."));
        byte[] beforeStatus = await File.ReadAllBytesAsync(LocalHealthNotebookPaths.HealthActionsFilePath);
        await fresh.SetRoutineStatusAsync(routine.Id, RoutineStatus.Active);
        var reactivated = (await Service().GetActionDetailsAsync(action.Id)).Routines.Single();
        Assert(reactivated.Status == RoutineStatus.Active && reactivated.Title == routine.Title && reactivated.CreatedAt == routine.CreatedAt
            && (await fresh.GetActionDetailsAsync(action.Id)).ProgressEntries.Count == 5, "Reactivation changed unrelated data/history.");
        var actions = await fresh.GetActionsAsync();
        Assert(actions.Zip(actions.Skip(1)).All(pair => pair.First.Action.CreatedAt >= pair.Second.Action.CreatedAt), "Action sort failed.");
        Assert((await fresh.GetActionDetailsAsync(independent.Id)).ProgressEntries.Count == 0, "Foreign history leaked into action details.");
        var overview = await fresh.GetOverviewAsync(DateOnly.FromDateTime(time.LocalDateTime));
        Assert(overview.ActiveActions == 8 && overview.ActiveRoutines == 1 && overview.EntriesToday == 3, "Documentative overview counts wrong.");
        await Fails<ArgumentException>(() => service.CreateActionAsync(new(" ")));
        await Fails<ArgumentException>(() => service.CreateActionAsync(new(new string('x', 161))));
        await Fails<ArgumentException>(() => service.CreateActionAsync(new("CODEX TEST", Description: new string('x', 4001))));
        await Fails<ArgumentException>(() => service.CreateActionAsync(new("CODEX TEST", OriginNote: new string('x', 4001))));
        await Fails<ArgumentException>(() => service.CreateActionAsync(new("CODEX TEST", HealthTopicId: Guid.NewGuid())));
        await Fails<ArgumentException>(() => service.CreateActionAsync(new("CODEX TEST", ActionType: (HealthActionType)999)));
        await Fails<ArgumentException>(() => service.CreateActionAsync(new("CODEX TEST", Origin: (HealthActionOrigin)999)));
        await Fails<ArgumentException>(() => service.CreateRoutineAsync(new(Guid.NewGuid(), "Synthetic orphan")));
        await Fails<ArgumentException>(() => service.CreateRoutineAsync(new(action.Id, " ")));
        await Fails<ArgumentException>(() => service.CreateRoutineAsync(new(action.Id, "CODEX TEST", ScheduleText: new string('x', 161))));
        await Fails<ArgumentException>(() => service.CreateProgressEntryAsync(new(Guid.NewGuid(), time)));
        await Fails<ArgumentException>(() => service.CreateProgressEntryAsync(new(routine.Id, default)));
        await Fails<ArgumentException>(() => service.CreateProgressEntryAsync(new(routine.Id, time, (ProgressCompletion)999)));
        await Fails<ArgumentException>(() => service.CreateProgressEntryAsync(new(routine.Id, time, Note: new string('x', 4001))));
        await Fails<ArgumentException>(() => service.SetRoutineStatusAsync(routine.Id, (RoutineStatus)999));
        await Fails<ArgumentException>(() => service.SetRoutineStatusAsync(Guid.NewGuid(), RoutineStatus.Paused));
        var repository = new JsonHealthActionRepository();
        await Fails<InvalidDataException>(() => repository.AddRoutineAsync(Routine.Create(Guid.NewGuid(), "Synthetic orphan")));
        await Fails<InvalidDataException>(() => repository.AddProgressEntryAsync(ProgressEntry.Create(Guid.NewGuid(), time, ProgressCompletion.Performed)));
        await Fails<InvalidDataException>(() => repository.AddActionAsync(action));
        byte[] valid = await File.ReadAllBytesAsync(LocalHealthNotebookPaths.HealthActionsFilePath);
        byte[] backup = await File.ReadAllBytesAsync(Path.Combine(root, "health-actions.backup.json"));
        Assert(beforeStatus.SequenceEqual(backup), "Backup is not the exact last valid store.");
        var json = JsonNode.Parse(valid)!.AsObject();
        Assert(json["Store"]!.GetValue<string>() == "SASD.HealthNotebook.HealthActions" && json["Version"]!.GetValue<int>() == 1, "Store contract wrong.");
        Assert(json["Actions"]!.AsArray().All(item => item!["HealthTopicTitle"] is null), "Topic title persisted redundantly.");
        foreach (string bad in new[] { "{", "", "{}", json.ToJsonString().Replace("SASD.HealthNotebook.HealthActions", "Foreign.Store"), json.ToJsonString().Replace("\"Version\":1", "\"Version\":2") })
            await RejectStore(root, bad);
        foreach (string collection in new[] { "Actions", "Routines", "ProgressEntries" })
        {
            var duplicate = json.DeepClone(); duplicate[collection]!.AsArray().Add(duplicate[collection]![0]!.DeepClone());
            await RejectStore(root, duplicate.ToJsonString());
            var nullItem = json.DeepClone(); nullItem[collection]!.AsArray().Add(null); await RejectStore(root, nullItem.ToJsonString());
        }
        var orphan = json.DeepClone(); orphan["Routines"]![0]!["HealthActionId"] = Guid.NewGuid(); await RejectStore(root, orphan.ToJsonString());
        orphan = json.DeepClone(); orphan["ProgressEntries"]![0]!["RoutineId"] = Guid.NewGuid(); await RejectStore(root, orphan.ToJsonString());
        var unknown = json.DeepClone(); unknown["FutureField"] = true; await RejectStore(root, unknown.ToJsonString());
        var missingField = json.DeepClone(); missingField["ProgressEntries"]![0]!.AsObject().Remove("Completion"); await RejectStore(root, missingField.ToJsonString());
        foreach (string suffix in new[] { ".tmp", ".lock" })
        {
            string guard = LocalHealthNotebookPaths.HealthActionsFilePath + suffix;
            await File.WriteAllTextAsync(guard, "Synthetic existing guard.");
            await Fails<IOException>(() => service.CreateActionAsync(new("CODEX TEST – Blocked")));
            Assert(await File.ReadAllTextAsync(guard) == "Synthetic existing guard." && Enumerable.SequenceEqual(valid, await File.ReadAllBytesAsync(LocalHealthNotebookPaths.HealthActionsFilePath)), "Guard/store overwritten.");
            File.Delete(guard); // Exact synthetic file created by this test.
        }
        foreach (var file in prior) Assert(Enumerable.SequenceEqual(file.Value, await File.ReadAllBytesAsync(Path.Combine(root, file.Key))), "Action writes altered an existing store.");
        string failureRoot = Path.Combine(root, "action-backup-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(failureRoot);
        Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, failureRoot);
        try
        {
            string primary = LocalHealthNotebookPaths.HealthActionsFilePath, backupPath = Path.Combine(failureRoot, "health-actions.backup.json");
            await File.WriteAllBytesAsync(backupPath, valid);
            await Fails<InvalidDataException>(() => Service().GetActionsAsync());
            await Fails<InvalidDataException>(() => Service().CreateActionAsync(new("CODEX TEST – Missing primary")));
            Assert(!File.Exists(primary), "Primary silently recovered/recreated.");
            await File.WriteAllBytesAsync(primary, valid); await File.WriteAllTextAsync(backupPath, "Foreign synthetic backup");
            await Fails<InvalidDataException>(() => Service().CreateActionAsync(new("CODEX TEST – Invalid backup")));
            Assert(Enumerable.SequenceEqual(valid, await File.ReadAllBytesAsync(primary)) && await File.ReadAllTextAsync(backupPath) == "Foreign synthetic backup", "Invalid backup overwritten.");
        }
        finally { Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, root); }
        Console.WriteLine("HealthAction/Routine/Progress domain, application, persistence and overview checks passed.");
    }
    private static HealthActionService Service() => new(new JsonHealthActionRepository(), new JsonHealthTopicRepository());
    private static async Task RejectStore(string root, string contents)
    {
        string child = Path.Combine(root, "action-invalid-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(child);
        Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, child);
        try
        {
            string path = LocalHealthNotebookPaths.HealthActionsFilePath; await File.WriteAllTextAsync(path, contents);
            await Fails<InvalidDataException>(() => Service().GetActionsAsync());
            await Fails<InvalidDataException>(() => Service().CreateActionAsync(new("CODEX TEST – Rejected")));
            Assert(await File.ReadAllTextAsync(path) == contents, "Invalid store changed.");
        }
        finally { Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, root); }
    }
    private static void Assert(bool ok, string message) { if (!ok) throw new InvalidOperationException(message); }
    private static async Task Fails<T>(Func<Task> action) where T : Exception
    { try { await action(); } catch (T) { return; } throw new InvalidOperationException("Expected " + typeof(T).Name); }
    private sealed class TopicSnapshot(IReadOnlyList<HealthTopic> topics) : IHealthTopicRepository
    {
        public Task<IReadOnlyList<HealthTopic>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult(topics);
        public Task AddAsync(HealthTopic topic, CancellationToken cancellationToken = default) => throw new InvalidOperationException("Actions must not write topics.");
        public Task SaveAllAsync(IReadOnlyList<HealthTopic> items, CancellationToken cancellationToken = default) => throw new InvalidOperationException("Actions must not write topics.");
    }
}
