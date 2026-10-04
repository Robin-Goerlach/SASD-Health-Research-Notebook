using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Repositories;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.Infrastructure.Storage;

namespace Sasd.HealthNotebook.SmokeTests;

/// <summary>IT-LIF-002: topic references, writer ordering and answer-preserving child lifecycle.</summary>
internal static class ParentLifecycleTests
{
    internal static async Task RunAsync()
    {
        string previous = LocalHealthNotebookPaths.DataDirectory;
        string root = Path.Combine(previous, "parent-lifecycle"); Directory.CreateDirectory(root);
        Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, root);
        try
        {
            var topics = new JsonHealthTopicRepository(); var service = new HealthTopicService(topics);
            var topic = HealthTopic.Create("CODEX TEST – topic", HealthTopicStatus.Suspected, HealthTopicPriority.Normal, "Synthetic description", "Synthetic note");
            await topics.AddAsync(topic);
            byte[] initial = File.ReadAllBytes(LocalHealthNotebookPaths.HealthTopicsFilePath);
            await service.UpdateHealthTopicAsync(topic.Id, Request(topic), topic.ModifiedAt);
            Check(initial.SequenceEqual(File.ReadAllBytes(LocalHealthNotebookPaths.HealthTopicsFilePath)), "No-op topic rewrote store.");
            var request = Request(topic); request.Title = "CODEX TEST – corrected topic"; await service.UpdateHealthTopicAsync(topic.Id, request, topic.ModifiedAt);
            var corrected = await service.GetByIdAsync(topic.Id); Check(corrected.CreatedAt == topic.CreatedAt && corrected.ModifiedAt > topic.ModifiedAt, "Topic timestamps lost.");
            await Fails<LifecycleConflictException>(() => service.DeleteHealthTopicAsync(topic.Id, topic.ModifiedAt));
            await service.ArchiveHealthTopicAsync(topic.Id, corrected.ModifiedAt); var archived = await service.GetByIdAsync(topic.Id);
            Check(archived.Status == HealthTopicStatus.Archived && archived.StatusBeforeArchive == HealthTopicStatus.Suspected, "Topic archive discarded status.");
            await service.ReactivateHealthTopicAsync(topic.Id, archived.ModifiedAt); corrected = await service.GetByIdAsync(topic.Id);
            Check(corrected.Status == HealthTopicStatus.Suspected, "Topic reactivate changed original status.");
            request = Request(corrected); request.Notes = new string('x', 4001);
            await Fails<ArgumentException>(() => service.UpdateHealthTopicAsync(topic.Id, request, corrected.ModifiedAt));
            // Unknown future fields must not silently disappear during any topic write.
            string topicPath = LocalHealthNotebookPaths.HealthTopicsFilePath;
            byte[] supported = File.ReadAllBytes(topicPath);
            var future = System.Text.Json.Nodes.JsonNode.Parse(supported)!.AsArray(); future[0]!["FutureField"] = "Synthetic future documentation";
            File.WriteAllText(topicPath, future.ToJsonString()); byte[] futureBytes = File.ReadAllBytes(topicPath);
            string topicBackup = Path.Combine(root, "health-topics.backup.json"); byte[] backupBytes = File.ReadAllBytes(topicBackup);
            await Fails<InvalidDataException>(() => service.ArchiveHealthTopicAsync(topic.Id, corrected.ModifiedAt));
            Check(futureBytes.SequenceEqual(File.ReadAllBytes(topicPath)) && backupBytes.SequenceEqual(File.ReadAllBytes(topicBackup)), "Unknown topic fields lost on write.");
            File.WriteAllBytes(topicPath, supported);
            foreach (string suffix in new[] { ".tmp", ".lock" })
            {
                string ownedGuard = topicPath + suffix; File.WriteAllText(ownedGuard, "synthetic guard");
                await Fails<IOException>(() => service.ArchiveHealthTopicAsync(topic.Id, corrected.ModifiedAt));
                Check(File.ReadAllText(ownedGuard) == "synthetic guard" && supported.SequenceEqual(File.ReadAllBytes(topicPath)), "Topic guard changed existing files."); File.Delete(ownedGuard);
            }
            // Optional archive metadata is genuinely backward compatible: read-only legacy
            // load preserves bytes; explicit reactivation uses the documented safe default.
            var legacy = System.Text.Json.Nodes.JsonNode.Parse(supported)!.AsArray(); legacy[0]!.AsObject().Remove("StatusBeforeArchive");
            File.WriteAllText(topicPath, legacy.ToJsonString()); byte[] legacyBytes = File.ReadAllBytes(topicPath);
            await topics.GetAllAsync(); Check(legacyBytes.SequenceEqual(File.ReadAllBytes(topicPath)), "Legacy topic read migrated data.");
            legacy[0]!["Status"] = "Archived"; File.WriteAllText(topicPath, legacy.ToJsonString());
            await service.ReactivateHealthTopicAsync(topic.Id, corrected.ModifiedAt);
            Check((await service.GetByIdAsync(topic.Id)).Status == HealthTopicStatus.Observation, "Legacy archive default not explicit.");
            File.WriteAllBytes(topicPath, supported); File.WriteAllBytes(topicBackup, backupBytes);
            File.Delete(topicPath);
            await Fails<InvalidDataException>(() => topics.AddAsync(topic));
            Check(!File.Exists(topicPath) && backupBytes.SequenceEqual(File.ReadAllBytes(topicBackup)), "Missing topic primary was silently recreated.");
            File.WriteAllBytes(topicPath, supported);
            // Each dependency independently blocks deletion. Unrelated stores remain byte-identical.
            foreach (string kind in new[] { "entry", "measurement", "source", "session", "action", "history" })
            {
                string childRoot = Path.Combine(root, kind); Directory.CreateDirectory(childRoot);
                Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, childRoot);
                var repo = new JsonHealthTopicRepository(); await repo.AddAsync(topic);
                var childService = new HealthTopicService(repo);
                if (kind == "entry") await new JsonHealthEntryRepository().AddAsync(HealthEntry.Create(HealthEntryType.Note, DateTimeOffset.Now, "CODEX TEST – linked entry", null, healthTopicId: topic.Id));
                if (kind == "measurement") await new JsonMeasurementRepository().AddAsync(Measurement.Create(MeasurementType.Weight, DateTimeOffset.Now, value: 13, healthTopicId: topic.Id));
                if (kind == "source") await new JsonSourceRepository().AddSourceAsync(Source.Create(SourceType.WebPage, title: "CODEX TEST – source", healthTopicId: topic.Id));
                if (kind == "session") await new JsonSessionRepository().AddSessionAsync(Session.Create(DateTimeOffset.Now, "CODEX TEST – session", SessionType.DoctorVisit, SessionStatus.Planned, topic.Id, null, null) with { IsArchived = true });
                if (kind is "action" or "history")
                {
                    var actionRepo = new JsonHealthActionRepository(); var action = HealthAction.Create("CODEX TEST – action", healthTopicId: topic.Id, origin: HealthActionOrigin.Doctor) with { IsArchived = true };
                    await actionRepo.AddActionAsync(action);
                    if (kind == "history") await actionRepo.UpdateActionAsync(action with { HealthTopicId = null }, action.ModifiedAt, "Synthetic correction");
                }
                var bytes = Directory.GetFiles(childRoot).ToDictionary(path => path, File.ReadAllBytes);
                await Fails<LifecycleDeleteBlockedException>(() => childService.DeleteHealthTopicAsync(topic.Id, topic.ModifiedAt));
                Check(bytes.All(pair => pair.Value.SequenceEqual(File.ReadAllBytes(pair.Key))), "Blocked topic delete changed related stores.");
            }
            Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, root);
            // Partial lock acquisition must release only locks owned by this operation.
            string locked = Path.Combine(root, "sessions.json.lock"); File.WriteAllText(locked, "synthetic held lock");
            await Fails<IOException>(() => service.DeleteHealthTopicAsync(topic.Id, corrected.ModifiedAt));
            Check(!File.Exists(LocalHealthNotebookPaths.HealthTopicsFilePath + ".lock") && File.ReadAllText(locked) == "synthetic held lock", "Partial lock failure lost exclusion."); File.Delete(locked);
            string corrupt = LocalHealthNotebookPaths.MeasurementsFilePath; File.WriteAllText(corrupt, "{}");
            await Fails<InvalidDataException>(() => service.DeleteHealthTopicAsync(topic.Id, corrected.ModifiedAt)); File.Delete(corrupt);
            // Pause the application-level topic lookup: Delete finishes first, but its
            // stale successful lookup cannot authorize a later repository reference write.
            var paused = new PausedTopics(corrected); var measurements = new MeasurementService(new JsonMeasurementRepository(), paused);
            var pending = measurements.CreateMeasurementAsync(new() { MeasurementType = MeasurementType.Weight, Value = 13, OccurredAt = DateTimeOffset.Now, HealthTopicId = topic.Id });
            await paused.Entered.Task;
            await service.DeleteHealthTopicAsync(topic.Id, corrected.ModifiedAt); paused.Resume.SetResult();
            await Fails<ArgumentException>(() => pending);
            Check((await new JsonHealthTopicRepository().GetAllAsync()).Count == 0 && (await new JsonMeasurementRepository().GetAllAsync()).Count == 0, "Late writer created orphan after delete.");
            await Fails<ArgumentException>(() => new JsonSessionRepository().AddSessionAsync(Session.Create(DateTimeOffset.Now, "CODEX TEST", SessionType.DoctorVisit, SessionStatus.Planned, topic.Id, null, null)));
            await Fails<ArgumentException>(() => new JsonHealthActionRepository().AddActionAsync(HealthAction.Create("CODEX TEST", healthTopicId: topic.Id)));
            await Fails<ArgumentException>(() => new JsonSourceRepository().AddSourceAsync(Source.Create(SourceType.WebPage, title: "CODEX TEST", healthTopicId: topic.Id)));
            await Fails<ArgumentException>(() => new JsonHealthEntryRepository().AddAsync(HealthEntry.Create(HealthEntryType.Note, DateTimeOffset.Now, "CODEX TEST", null, healthTopicId: topic.Id)));
            await topics.AddAsync(topic); await Fails<InvalidOperationException>(() => topics.SaveAllAsync(Array.Empty<HealthTopic>()));
            var sessions = new JsonSessionRepository(); var session = Session.Create(DateTimeOffset.Now, "CODEX TEST – child lifecycle", SessionType.DoctorVisit, SessionStatus.Planned, null, null, null); await sessions.AddSessionAsync(session);
            var question = SessionQuestion.Create(session.Id, "Synthetic question"); await sessions.AddQuestionAsync(question);
            await sessions.UpdateQuestionAsync(question with { Text = "Synthetic correction", SortOrder = 2 }, question.ModifiedAt);
            var q = (await sessions.LoadAsync()).Questions.Single(); Check(q.Id == question.Id && q.CreatedAt == question.CreatedAt && q.ModifiedAt > question.ModifiedAt, "Question metadata lost.");
            byte[] childBytes = File.ReadAllBytes(LocalHealthNotebookPaths.SessionsFilePath);
            await sessions.UpdateQuestionAsync(q, q.ModifiedAt); Check(childBytes.SequenceEqual(File.ReadAllBytes(LocalHealthNotebookPaths.SessionsFilePath)), "No-op question edit rewrote store.");
            var futureQuestion = q with { ModifiedAt = DateTimeOffset.Now.AddDays(1) };
            Check(futureQuestion.WithAnswer(true, "Synthetic answer").ModifiedAt > futureQuestion.ModifiedAt, "Question clock rollback weakened conflict token.");
            await Fails<LifecycleConflictException>(() => sessions.DeleteQuestionAsync(question.Id, question.ModifiedAt));
            await sessions.SetQuestionAnswerAsync(q.Id, true, "Synthetic documented answer"); q = (await sessions.LoadAsync()).Questions.Single();
            await Fails<LifecycleDeleteBlockedException>(() => sessions.DeleteQuestionAsync(q.Id, q.ModifiedAt));
            await sessions.SetQuestionAnswerAsync(q.Id, false, q.AnswerNote); q = (await sessions.LoadAsync()).Questions.Single();
            await Fails<LifecycleDeleteBlockedException>(() => sessions.DeleteQuestionAsync(q.Id, q.ModifiedAt));
            var unanswered = SessionQuestion.Create(session.Id, "Synthetic unanswered"); await sessions.AddQuestionAsync(unanswered); await sessions.DeleteQuestionAsync(unanswered.Id, unanswered.ModifiedAt);
            var followUp = SessionFollowUp.Create(session.Id, "Synthetic next step"); await sessions.AddFollowUpAsync(followUp);
            await sessions.UpdateFollowUpAsync(followUp with { Text = "Synthetic corrected next step", DueDate = new DateOnly(2026, 10, 20), Status = SessionFollowUpStatus.Done }, followUp.ModifiedAt);
            var f = (await sessions.LoadAsync()).FollowUps.Single(); Check(f.CreatedAt == followUp.CreatedAt && f.ModifiedAt > followUp.ModifiedAt, "Follow-up metadata lost.");
            childBytes = File.ReadAllBytes(LocalHealthNotebookPaths.SessionsFilePath);
            await sessions.UpdateFollowUpAsync(f, f.ModifiedAt); Check(childBytes.SequenceEqual(File.ReadAllBytes(LocalHealthNotebookPaths.SessionsFilePath)), "No-op follow-up edit rewrote store.");
            var futureFollowUp = f with { ModifiedAt = DateTimeOffset.Now.AddDays(1) };
            Check(futureFollowUp.WithStatus(SessionFollowUpStatus.Open).ModifiedAt > futureFollowUp.ModifiedAt, "Follow-up clock rollback weakened conflict token.");
            await Fails<LifecycleConflictException>(() => sessions.DeleteFollowUpAsync(f.Id, followUp.ModifiedAt)); await sessions.DeleteFollowUpAsync(f.Id, f.ModifiedAt);
            var reloaded = await new JsonSessionRepository().LoadAsync(); Check(reloaded.Questions.Count == 1 && reloaded.Questions[0].AnswerNote == "Synthetic documented answer" && reloaded.FollowUps.Count == 0 && reloaded.Sessions.Count == 1, "Child delete cascaded or lost answer.");
            Console.WriteLine("Topic/reference-lock and session-child lifecycle checks passed.");
        }
        finally { Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, previous); }
    }
    private static CreateHealthTopicRequest Request(HealthTopic topic) => new() { Title = topic.Title, Status = topic.Status, Priority = topic.Priority, ShortDescription = topic.ShortDescription, Notes = topic.Notes };
    private static void Check(bool value, string message) { if (!value) throw new InvalidOperationException(message); }
    private static async Task Fails<T>(Func<Task> action) where T : Exception { try { await action(); } catch (T) { return; } throw new InvalidOperationException("Expected " + typeof(T).Name); }
    private sealed class PausedTopics(HealthTopic topic) : IHealthTopicRepository
    {
        internal TaskCompletionSource Entered { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        internal TaskCompletionSource Resume { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public async Task<IReadOnlyList<HealthTopic>> GetAllAsync(CancellationToken cancellationToken = default) { Entered.SetResult(); await Resume.Task; return new[] { topic }; }
        public Task AddAsync(HealthTopic value, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task SaveAllAsync(IReadOnlyList<HealthTopic> value, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
}
