using System.Text.Json.Nodes;
using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.Infrastructure.Storage;

namespace Sasd.HealthNotebook.SmokeTests;

/// <summary>UT/IT/SEC-SES-001: synthetic session lifecycle, integrity and owned-store guards.</summary>
internal static class SessionTests
{
    internal static async Task RunAsync()
    {
        string root = LocalHealthNotebookPaths.DataDirectory;
        Assert(root.StartsWith(Path.Combine(Program.FindRepositoryRoot(), ".codex") + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase), "Unsafe session test root.");
        var prior = new Dictionary<string, byte[]>();
        foreach (string file in new[] { "health-topics.json", "health-entries.json", "sources.json", "measurements.json" })
            prior[file] = await File.ReadAllBytesAsync(Path.Combine(root, file));
        Guid topicId = (await new JsonHealthTopicRepository().GetAllAsync())[0].Id;
        var time = new DateTimeOffset(2026, 10, 1, 12, 0, 0, TimeSpan.FromHours(2));
        var service = Service();
        Assert((await service.GetSessionsAsync()).Count == 0, "Missing store not empty.");
        var first = await service.CreateSessionAsync(new(time, "CODEX TEST – Session", HealthTopicId: topicId, ContactText: "Synthetic contact", Notes: "Synthetic conversation note."));
        foreach (SessionType type in Enum.GetValues<SessionType>())
            await service.CreateSessionAsync(new(time.AddDays((int)type + 1), "CODEX TEST – Conversation", type, SessionStatus.Completed));
        var question = await service.CreateQuestionAsync(new(first.Id, "Synthetic user question?", 2));
        var earlier = await service.CreateQuestionAsync(new(first.Id, "Synthetic earlier question?", 1, true, "Synthetic documented answer."));
        var followUp = await service.CreateFollowUpAsync(new(first.Id, "Synthetic next step."));
        await service.SetQuestionAnswerAsync(question.Id, true, "Synthetic answer note.");
        await service.SetFollowUpStatusAsync(followUp.Id, SessionFollowUpStatus.Done);
        var details = await Service().GetSessionDetailsAsync(first.Id);
        Assert(details.Sessions.Single().Id == first.Id && details.Questions.Count == 2 && details.Questions[0].Id == earlier.Id, "Independent child reload/order failed.");
        var actual = details.Questions.Single(item => item.Id == question.Id);
        Assert(actual.SessionId == first.Id && actual.IsAnswered && actual.AnswerNote == "Synthetic answer note." && actual.Text == question.Text
            && actual.CreatedAt == question.CreatedAt && actual.ModifiedAt >= question.ModifiedAt, "Answer fields or identity lost.");
        Assert(details.FollowUps.Single().SessionId == first.Id && details.FollowUps.Single().Status == SessionFollowUpStatus.Done, "Follow-up status lost.");
        await service.SetQuestionAnswerAsync(question.Id, false, actual.AnswerNote);
        await service.SetFollowUpStatusAsync(followUp.Id, SessionFollowUpStatus.Open);
        Assert(!(await Service().GetSessionDetailsAsync(first.Id)).Questions.Single(item => item.Id == question.Id).IsAnswered, "Cannot reopen question.");
        Assert((await Service().GetSessionDetailsAsync(first.Id)).FollowUps.Single().Status == SessionFollowUpStatus.Open, "Cannot reopen follow-up.");
        var sessions = await Service().GetSessionsAsync();
        Assert(sessions.Count == 7 && sessions.Zip(sessions.Skip(1)).All(pair => pair.First.Session.ScheduledAt >= pair.Second.Session.ScheduledAt), "Session chronological reload failed.");
        Assert((await service.GetSessionsAsync(topicId)).Single().HealthTopicTitle is not null, "Topic resolution failed.");
        await Fails<ArgumentException>(() => service.CreateSessionAsync(new(time, " ")));
        await Fails<ArgumentException>(() => service.CreateSessionAsync(new(time, new string('x', 161))));
        await Fails<ArgumentException>(() => service.CreateSessionAsync(new(time, "CODEX TEST", Notes: new string('x', 4001))));
        await Fails<ArgumentException>(() => service.CreateSessionAsync(new(default, "CODEX TEST")));
        await Fails<ArgumentException>(() => service.CreateSessionAsync(new(time, "CODEX TEST", HealthTopicId: Guid.NewGuid())));
        await Fails<ArgumentException>(() => service.CreateQuestionAsync(new(Guid.NewGuid(), "Synthetic orphan?")));
        await Fails<ArgumentException>(() => service.CreateFollowUpAsync(new(Guid.NewGuid(), "Synthetic orphan.")));
        await Fails<ArgumentException>(() => service.CreateQuestionAsync(new(first.Id, " ")));
        await Fails<ArgumentException>(() => service.CreateQuestionAsync(new(first.Id, "Synthetic?", -1)));
        await Fails<ArgumentException>(() => service.SetQuestionAnswerAsync(question.Id, true, new string('x', 4001)));
        await Fails<ArgumentException>(() => service.SetQuestionAnswerAsync(Guid.NewGuid(), true, null));
        await Fails<ArgumentException>(() => service.SetFollowUpStatusAsync(followUp.Id, (SessionFollowUpStatus)999));
        await Fails<ArgumentException>(() => service.SetFollowUpStatusAsync(Guid.NewGuid(), SessionFollowUpStatus.Done));
        var repository = new JsonSessionRepository();
        await Fails<InvalidDataException>(() => repository.AddQuestionAsync(SessionQuestion.Create(Guid.NewGuid(), "Synthetic orphan?")));
        await Fails<InvalidDataException>(() => repository.AddFollowUpAsync(SessionFollowUp.Create(Guid.NewGuid(), "Synthetic orphan.")));
        Assert(LocalHealthNotebookPaths.SessionsFilePath == Path.Combine(root, "sessions.json"), "Shared session path ignored.");
        byte[] valid = await File.ReadAllBytesAsync(LocalHealthNotebookPaths.SessionsFilePath);
        var json = JsonNode.Parse(valid)!.AsObject();
        Assert(json["Store"]!.GetValue<string>() == "SASD.HealthNotebook.Sessions" && json["Version"]!.GetValue<int>() == 1, "Store contract wrong.");
        Assert(json["Sessions"]!.AsArray().All(item => item!["HealthTopicTitle"] is null), "Redundant topic title.");
        var backup = await File.ReadAllBytesAsync(Path.Combine(root, "sessions.backup.json"));
        Assert(JsonNode.Parse(backup)!["Questions"]!.AsArray().Single(item => item!["Id"]!.GetValue<Guid>() == question.Id)!["IsAnswered"]!.GetValue<bool>() == false, "Last-good backup incorrect.");
        foreach (string bad in new[] { "{", "", "{}", json.ToJsonString().Replace("SASD.HealthNotebook.Sessions", "Foreign.Store"), json.ToJsonString().Replace("\"Version\":1", "\"Version\":2") })
            await RejectStore(root, bad, time);
        var orphan = json.DeepClone(); orphan["Questions"]![0]!["SessionId"] = Guid.NewGuid();
        await RejectStore(root, orphan.ToJsonString(), time);
        foreach (string suffix in new[] { ".tmp", ".lock" })
        {
            string guard = LocalHealthNotebookPaths.SessionsFilePath + suffix;
            await File.WriteAllTextAsync(guard, "Synthetic existing file.");
            await Fails<IOException>(() => service.CreateSessionAsync(new(time, "CODEX TEST – Blocked")));
            byte[] current = await File.ReadAllBytesAsync(LocalHealthNotebookPaths.SessionsFilePath);
            Assert(await File.ReadAllTextAsync(guard) == "Synthetic existing file." && valid.SequenceEqual(current), "Existing guard or store changed.");
            File.Delete(guard); // This test created this exact synthetic file.
        }
        foreach (var file in prior)
        {
            byte[] current = await File.ReadAllBytesAsync(Path.Combine(root, file.Key));
            Assert(file.Value.SequenceEqual(current), "Session writes altered an existing store.");
        }
        Console.WriteLine("Session domain/application/persistence checks passed (FR-SES-001/002/003/004/007/008, Slice 1).");
    }
    private static SessionService Service() => new(new JsonSessionRepository(), new JsonHealthTopicRepository());
    private static async Task RejectStore(string root, string contents, DateTimeOffset time)
    {
        string child = Path.Combine(root, "session-invalid-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(child);
        Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, child);
        try
        {
            string path = LocalHealthNotebookPaths.SessionsFilePath;
            await File.WriteAllTextAsync(path, contents);
            await Fails<InvalidDataException>(() => Service().GetSessionsAsync());
            await Fails<InvalidDataException>(() => Service().CreateSessionAsync(new(time, "CODEX TEST – Rejected")));
            Assert(await File.ReadAllTextAsync(path) == contents, "Invalid store overwritten.");
        }
        finally { Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, root); }
    }
    private static void Assert(bool ok, string message) { if (!ok) throw new InvalidOperationException(message); }
    private static async Task Fails<T>(Func<Task> action) where T : Exception
    { try { await action(); } catch (T) { return; } throw new InvalidOperationException("Expected " + typeof(T).Name); }
}
