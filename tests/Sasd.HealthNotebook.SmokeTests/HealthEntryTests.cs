using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Application.Repositories;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.Infrastructure.Storage;

namespace Sasd.HealthNotebook.SmokeTests;

/// <summary>FR-OBS-001: domain, shared service, isolated persistence and non-destructive failure checks.</summary>
internal static class HealthEntryTests
{
    internal static async Task RunAsync()
    {
        Assert(LocalHealthNotebookPaths.DataDirectory.StartsWith(
            Path.Combine(Program.FindRepositoryRoot(), ".codex") + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase),
            "Entry tests require repository .codex data.");
        // UT-ENTRY-001: domain validation never truncates user input.
        DateTimeOffset eventTime = new(2026, 9, 20, 14, 0, 0, TimeSpan.FromHours(2));
        var valid = HealthEntry.Create(HealthEntryType.Note, eventTime, "  CODEX TEST – Timeline Entry  ", "  synthetic text\r\n  ");
        Assert(valid.Title == "CODEX TEST – Timeline Entry" && valid.Content == "  synthetic text\r\n  ", "Entry normalization changed content.");
        Assert(valid.CreatedAt == valid.ModifiedAt && valid.OccurredAt == eventTime && valid.HealthTopicId is null, "Invalid entry defaults.");
        foreach (string title in new[] { "", " ", new string('x', 161) })
            ExpectArgument(() => HealthEntry.Create(HealthEntryType.Note, eventTime, title, ""));
        ExpectArgument(() => HealthEntry.Create(HealthEntryType.Note, eventTime, "Synthetic", new string('x', 4001)));
        ExpectArgument(() => HealthEntry.Create((HealthEntryType)999, eventTime, "Synthetic", ""));
        ExpectArgument(() => HealthEntry.Create(HealthEntryType.Note, default, "Synthetic", ""));
        ExpectArgument(() => HealthEntry.Create(HealthEntryType.Note, eventTime, "Synthetic", "", Guid.Empty));
        HealthEntry.Create(HealthEntryType.Research, eventTime, new string('x', 160), new string('x', 4000));

        // IT-ENTRY-001: override, chronological ordering by instant, optional linkage and topic integrity.
        byte[] topicsBefore = await File.ReadAllBytesAsync(LocalHealthNotebookPaths.HealthTopicsFilePath);
        Guid topicId = (await new JsonHealthTopicRepository().GetAllAsync())[0].Id;
        var repository = new JsonHealthEntryRepository();
        var service = new HealthEntryService(repository, new JsonHealthTopicRepository());
        await service.CreateEntryAsync(Request(eventTime, null, HealthEntryType.Note));
        await service.CreateEntryAsync(Request(eventTime.AddDays(-1), topicId, HealthEntryType.Observation));
        // Different displayed local time, but a later actual instant.
        await service.CreateEntryAsync(Request(eventTime.ToOffset(TimeSpan.FromHours(-4)).AddHours(1), topicId, HealthEntryType.Research));
        var reloaded = await new HealthEntryService(new JsonHealthEntryRepository(), new JsonHealthTopicRepository()).GetEntriesAsync();
        Assert(reloaded.Count == 3 && reloaded[0].EntryType == HealthEntryType.Research
            && reloaded[1].EntryType == HealthEntryType.Note && reloaded[2].EntryType == HealthEntryType.Observation,
            "Timeline is not ordered by documented instant.");
        Assert(reloaded[0].HealthTopicId == topicId && reloaded[0].HealthTopicTitle is not null
            && reloaded[1].HealthTopicId is null && reloaded[1].Content == "Synthetic content.", "Entry roundtrip/link projection failed.");
        Assert((await service.GetEntriesAsync(topicId)).Count == 2, "Topic filter failed.");
        await ExpectFailureAsync<ArgumentException>(() => service.CreateEntryAsync(Request(eventTime, Guid.NewGuid(), HealthEntryType.Note)));
        Assert((await service.GetEntriesAsync()).Count == 3, "Rejected reference changed stored entries.");
        byte[] topicsAfter = await File.ReadAllBytesAsync(LocalHealthNotebookPaths.HealthTopicsFilePath);
        Assert(topicsBefore.SequenceEqual(topicsAfter), "Topic JSON was modified.");
        Assert(LocalHealthNotebookPaths.HealthEntriesFilePath == Path.Combine(LocalHealthNotebookPaths.DataDirectory, "health-entries.json"), "Entry override ignored.");
        Assert(File.Exists(Path.Combine(LocalHealthNotebookPaths.DataDirectory, "health-entries.backup.json")), "Entry backup missing.");
        using (var backup = System.Text.Json.JsonDocument.Parse(await File.ReadAllTextAsync(
            Path.Combine(LocalHealthNotebookPaths.DataDirectory, "health-entries.backup.json"))))
            Assert(backup.RootElement.GetProperty("Entries").GetArrayLength() == 2, "Backup does not contain the previous committed collection.");
        // Current title/archive state is resolved, never persisted on the entry.
        var topicStub = new ReadOnlyTopicRepository
        {
            Topics = new[] { new HealthTopic { Id = topicId, Title = "Synthetic renamed topic", Status = HealthTopicStatus.Archived } }
        };
        var linkedService = new HealthEntryService(new JsonHealthEntryRepository(), topicStub);
        var archived = await linkedService.GetEntriesAsync(topicId);
        Assert(archived.Count == 2 && archived.All(entry => entry.HealthTopicTitle == "Synthetic renamed topic"),
            "Current archived topic title was not resolved.");
        topicStub.Topics = Array.Empty<HealthTopic>();
        var orphaned = await linkedService.GetEntriesAsync();
        Assert(orphaned.Count == 3 && orphaned.Where(entry => entry.HealthTopicId.HasValue).All(entry => entry.HealthTopicTitle is null),
            "Missing topic removed entries or broke timeline.");
        using (var document = System.Text.Json.JsonDocument.Parse(await File.ReadAllTextAsync(LocalHealthNotebookPaths.HealthEntriesFilePath)))
            Assert(document.RootElement.GetProperty("Entries").EnumerateArray().All(entry => !entry.TryGetProperty("HealthTopicTitle", out _)),
                "Topic title was redundantly persisted.");
        var duplicate = (await repository.GetAllAsync())[0];
        await ExpectFailureAsync<InvalidOperationException>(() => repository.AddAsync(duplicate));
        byte[] entryBytesBeforeCancel = await File.ReadAllBytesAsync(LocalHealthNotebookPaths.HealthEntriesFilePath);
        await ExpectFailureAsync<OperationCanceledException>(() => repository.AddAsync(valid, new CancellationToken(canceled: true)));
        byte[] entryBytesAfterCancel = await File.ReadAllBytesAsync(LocalHealthNotebookPaths.HealthEntriesFilePath);
        Assert(entryBytesBeforeCancel.SequenceEqual(entryBytesAfterCancel), "Canceled write changed entries.");

        // SEC-ENTRY-001: foreign/corrupt primary or backup stores cannot be overwritten.
        string originalRoot = LocalHealthNotebookPaths.DataDirectory;
        try
        {
            foreach (string foreign in new[] { "", "null", "[]", "{}", "{broken", "{\"Store\":\"Other\",\"Version\":1,\"Entries\":[]}",
                "{\"Store\":\"SASD.HealthNotebook.HealthEntries\",\"Version\":2,\"Entries\":[]}",
                "{\"Store\":\"SASD.HealthNotebook.HealthEntries\",\"Version\":1,\"Entries\":[],\"Future\":true}",
                System.Text.Encoding.UTF8.GetString(topicsBefore) })
            {
                string root = Path.Combine(originalRoot, "entry-failure-tests", Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(root);
                Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, root);
                string path = LocalHealthNotebookPaths.HealthEntriesFilePath;
                await File.WriteAllTextAsync(path, foreign);
                await ExpectFailureAsync<InvalidDataException>(() => new JsonHealthEntryRepository().AddAsync(valid));
                Assert(await File.ReadAllTextAsync(path) == foreign, "Rejected JSON was overwritten.");
            }
            string temporaryRoot = Path.Combine(originalRoot, "entry-failure-tests", Guid.NewGuid().ToString("N"));
            Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, temporaryRoot);
            var temporaryRepository = new JsonHealthEntryRepository();
            Assert((await temporaryRepository.GetAllAsync()).Count == 0, "Nonexistent store should be empty.");
            await temporaryRepository.AddAsync(valid);
            byte[] beforeTemporaryFailure = await File.ReadAllBytesAsync(LocalHealthNotebookPaths.HealthEntriesFilePath);
            string temporaryPath = LocalHealthNotebookPaths.HealthEntriesFilePath + ".tmp";
            await File.WriteAllTextAsync(temporaryPath, "Synthetic unfinished write");
            await ExpectFailureAsync<IOException>(() => temporaryRepository.AddAsync(HealthEntry.Create(HealthEntryType.Note, eventTime, "Synthetic", "")));
            byte[] afterTemporaryFailure = await File.ReadAllBytesAsync(LocalHealthNotebookPaths.HealthEntriesFilePath);
            Assert(beforeTemporaryFailure.SequenceEqual(afterTemporaryFailure)
                && await File.ReadAllTextAsync(temporaryPath) == "Synthetic unfinished write", "Existing temporary file was overwritten or removed.");
            string emptyRoot = Path.Combine(originalRoot, "entry-failure-tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(emptyRoot);
            Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, emptyRoot);
            await File.WriteAllTextAsync(LocalHealthNotebookPaths.HealthEntriesFilePath,
                "{\"Store\":\"SASD.HealthNotebook.HealthEntries\",\"Version\":1,\"Entries\":[]}");
            var emptyRepository = new JsonHealthEntryRepository();
            Assert((await emptyRepository.GetAllAsync()).Count == 0, "Valid empty store rejected.");
            await emptyRepository.AddAsync(valid);
            Assert((await emptyRepository.GetAllAsync()).Count == 1, "Cannot append to valid empty store.");
            string missingPrimaryRoot = Path.Combine(originalRoot, "entry-failure-tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(missingPrimaryRoot);
            Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, missingPrimaryRoot);
            string survivingBackup = Path.Combine(missingPrimaryRoot, "health-entries.backup.json");
            await File.WriteAllBytesAsync(survivingBackup, entryBytesBeforeCancel);
            await ExpectFailureAsync<InvalidDataException>(() => new JsonHealthEntryRepository().GetAllAsync());
            await ExpectFailureAsync<InvalidDataException>(() => new JsonHealthEntryRepository().AddAsync(valid));
            Assert(!File.Exists(LocalHealthNotebookPaths.HealthEntriesFilePath), "Missing primary was silently recreated.");
            string backupRoot = Path.Combine(originalRoot, "entry-failure-tests", Guid.NewGuid().ToString("N"));
            Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, backupRoot);
            var backupRepository = new JsonHealthEntryRepository();
            await backupRepository.AddAsync(valid);
            byte[] primaryBefore = await File.ReadAllBytesAsync(LocalHealthNotebookPaths.HealthEntriesFilePath);
            string backupPath = Path.Combine(backupRoot, "health-entries.backup.json");
            await File.WriteAllTextAsync(backupPath, "foreign synthetic backup");
            await ExpectFailureAsync<InvalidDataException>(() => backupRepository.AddAsync(HealthEntry.Create(HealthEntryType.Note, eventTime, "Synthetic", "")));
            byte[] primaryAfter = await File.ReadAllBytesAsync(LocalHealthNotebookPaths.HealthEntriesFilePath);
            Assert(primaryBefore.SequenceEqual(primaryAfter)
                && await File.ReadAllTextAsync(backupPath) == "foreign synthetic backup", "Foreign backup was overwritten.");
            // An existing writer lock must remain untouched, not be treated as disposable stale data.
            string lockPath = LocalHealthNotebookPaths.HealthEntriesFilePath + ".lock";
            await File.WriteAllTextAsync(lockPath, "synthetic writer lock");
            await ExpectFailureAsync<IOException>(() => backupRepository.AddAsync(valid));
            Assert(await File.ReadAllTextAsync(lockPath) == "synthetic writer lock", "Existing writer lock was changed.");
        }
        finally { Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, originalRoot); }
    }

    private static CreateHealthEntryRequest Request(DateTimeOffset time, Guid? topic, HealthEntryType type) =>
        new() { Title = "CODEX TEST – Timeline Entry", Content = "Synthetic content.", OccurredAt = time, HealthTopicId = topic, EntryType = type };
    private static void ExpectArgument(Action action)
    {
        try { action(); } catch (ArgumentException) { return; }
        throw new InvalidOperationException("Invalid entry input was accepted.");
    }
    private static async Task ExpectFailureAsync<T>(Func<Task> action) where T : Exception
    {
        try { await action(); } catch (T) { return; }
        throw new InvalidOperationException("Expected safe persistence failure did not occur.");
    }
    private static void Assert(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }

    private sealed class ReadOnlyTopicRepository : IHealthTopicRepository
    {
        public IReadOnlyList<HealthTopic> Topics { get; set; } = Array.Empty<HealthTopic>();
        public Task<IReadOnlyList<HealthTopic>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult(Topics);
        public Task AddAsync(HealthTopic topic, CancellationToken cancellationToken = default) => throw new InvalidOperationException("Entry service must not write topics.");
        public Task SaveAllAsync(IReadOnlyList<HealthTopic> topics, CancellationToken cancellationToken = default) => throw new InvalidOperationException("Entry service must not write topics.");
    }
}
