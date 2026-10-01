using System.Text.Json;
using System.Text.Json.Nodes;
using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Repositories;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.Infrastructure.Storage;

namespace Sasd.HealthNotebook.SmokeTests;

/// <summary>FR-SRC-001/002/004/005/006: validation, references, isolated roundtrip and safe failures.</summary>
internal static class SourceTests
{
    internal static async Task RunAsync()
    {
        string root = LocalHealthNotebookPaths.DataDirectory;
        Assert(root.StartsWith(Path.Combine(Program.FindRepositoryRoot(), ".codex") + Path.DirectorySeparatorChar,
            StringComparison.OrdinalIgnoreCase), "Source tests require repository .codex data.");
        Assert(LocalHealthNotebookPaths.SourcesFilePath == Path.Combine(root, "sources.json"), "Source override ignored.");
        // UT-SRC-001: metadata categories are documentation only. No truncation of any field.
        var valid = Source.Create(SourceType.WebPage, "  CODEX TEST – Synthetic Research Source  ",
            "https://example.invalid/synthetic", "Synthetic institution", new DateOnly(2026, 9, 1), new DateOnly(2026, 10, 1), "Synthetic ID");
        Assert(valid.Title == "CODEX TEST – Synthetic Research Source" && valid.CreatedAt == valid.ModifiedAt, "Source defaults failed.");
        Source.Create(SourceType.WebPage, "", "https://example.invalid/");
        Source.Create(SourceType.Conversation, null, authorOrInstitution: "Synthetic provider");
        Source.Create(SourceType.Book, new string('x', 160), authorOrInstitution: new string('x', 160), externalIdentifier: new string('x', 160));
        ExpectArgument(() => Source.Create(SourceType.Book, " "));
        ExpectArgument(() => Source.Create(SourceType.Book, new string('x', 161)));
        ExpectArgument(() => Source.Create(SourceType.Book, "Synthetic", authorOrInstitution: new string('x', 161)));
        ExpectArgument(() => Source.Create(SourceType.Book, "Synthetic", externalIdentifier: new string('x', 161)));
        ExpectArgument(() => Source.Create((SourceType)999, "Synthetic"));
        ExpectArgument(() => Source.Create(SourceType.WebPage, "Synthetic", "relative"));
        ExpectArgument(() => Source.Create(SourceType.WebPage, "Synthetic", "file:///C:/synthetic"));
        ExpectArgument(() => Source.Create(SourceType.WebPage, "Synthetic", "https://example.invalid/" + new string('x', 2048)));
        ExpectArgument(() => SourceLocation.Create(Guid.Empty, SourceLocationType.Page, "Page 17"));
        ExpectArgument(() => SourceLocation.Create(valid.Id, SourceLocationType.Page, " "));
        ExpectArgument(() => SourceLocation.Create(valid.Id, SourceLocationType.Page, new string('x', 161)));
        ExpectArgument(() => SourceLocation.Create(valid.Id, SourceLocationType.Page, "Page 17", new string('x', 4001)));
        ExpectArgument(() => SourceLocation.Create(valid.Id, (SourceLocationType)999, "Synthetic"));
        ExpectArgument(() => EvidenceNote.Create(valid.Id, null, " ", "", "Synthetic summary"));
        ExpectArgument(() => EvidenceNote.Create(valid.Id, null, "Synthetic claim", "", " "));
        ExpectArgument(() => EvidenceNote.Create(valid.Id, null, new string('x', 4001), "", "Synthetic summary"));
        ExpectArgument(() => EvidenceNote.Create(valid.Id, null, "Synthetic claim", new string('x', 1001), "Synthetic summary"));
        ExpectArgument(() => EvidenceNote.Create(valid.Id, null, "Synthetic claim", "", new string('x', 4001)));
        EvidenceNote.Create(valid.Id, null, new string('x', 4000), new string('x', 1000), new string('x', 4000));

        byte[] topicsBefore = await File.ReadAllBytesAsync(LocalHealthNotebookPaths.HealthTopicsFilePath);
        byte[] entriesBefore = await File.ReadAllBytesAsync(LocalHealthNotebookPaths.HealthEntriesFilePath);
        var topics = new JsonHealthTopicRepository();
        var topic = (await topics.GetAllAsync())[0];
        var repository = new JsonSourceRepository();
        var service = new SourceService(repository, topics);
        Assert((await repository.LoadAsync()).Sources.Count == 0, "Missing source store must be empty.");
        // IT-SRC-001: all types and optional metadata survive independent repository/service recreation.
        var first = await service.CreateSourceAsync(new CreateSourceRequest { SourceType = valid.SourceType, Title = valid.Title,
            Url = valid.Url, AuthorOrInstitution = valid.AuthorOrInstitution, PublicationDate = valid.PublicationDate,
            AccessedAt = valid.AccessedAt, ExternalIdentifier = valid.ExternalIdentifier, HealthTopicId = topic.Id });
        foreach (var type in Enum.GetValues<SourceType>().Where(type => type != SourceType.WebPage))
            await service.CreateSourceAsync(new CreateSourceRequest { SourceType = type, Title = "CODEX TEST – Synthetic Research Source " + type });
        var second = (await service.GetSourcesAsync()).First(item => item.Source.Id != first.Id).Source;
        await ExpectFailureAsync<ArgumentException>(() => service.CreateSourceAsync(new CreateSourceRequest { Title = "Synthetic", HealthTopicId = Guid.NewGuid() }));
        var location = await service.CreateLocationAsync(new CreateSourceLocationRequest { SourceId = first.Id,
            LocationType = SourceLocationType.Page, Locator = "Page 17", Note = "Synthetic locator note." });
        foreach (var type in Enum.GetValues<SourceLocationType>().Where(type => type != SourceLocationType.Page))
            await service.CreateLocationAsync(new CreateSourceLocationRequest { SourceId = first.Id, LocationType = type, Locator = "Synthetic locator " + type });
        await ExpectFailureAsync<ArgumentException>(() => service.CreateLocationAsync(new CreateSourceLocationRequest { SourceId = Guid.NewGuid(), Locator = "Page 17" }));
        await service.CreateNoteAsync(NoteRequest(first.Id, null));
        var note = await service.CreateNoteAsync(NoteRequest(first.Id, location.Id));
        await ExpectFailureAsync<ArgumentException>(() => service.CreateNoteAsync(NoteRequest(second.Id, location.Id)));
        await ExpectFailureAsync<ArgumentException>(() => service.CreateNoteAsync(NoteRequest(first.Id, Guid.NewGuid())));
        await ExpectFailureAsync<ArgumentException>(() => service.CreateNoteAsync(NoteRequest(Guid.NewGuid(), null)));
        var reloaded = new SourceService(new JsonSourceRepository(), new JsonHealthTopicRepository());
        var sources = await reloaded.GetSourcesAsync();
        var loaded = await reloaded.GetSourceAsync(first.Id);
        Assert(sources.Count == 7 && sources.Select(item => item.Source.SourceType).Distinct().Count() == 7, "Source type roundtrip failed.");
        Assert(loaded is not null && loaded.Url == valid.Url && loaded.AuthorOrInstitution == valid.AuthorOrInstitution
            && loaded.PublicationDate == valid.PublicationDate && loaded.AccessedAt == valid.AccessedAt && loaded.ExternalIdentifier == valid.ExternalIdentifier,
            "Metadata roundtrip failed.");
        Assert(sources.Single(item => item.Source.Id == first.Id).HealthTopicTitle == topic.Title, "Source topic title resolution failed.");
        var locations = await reloaded.GetLocationsAsync(first.Id);
        var notes = await reloaded.GetNotesAsync(first.Id);
        Assert(locations.Count == 6 && locations.Any(item => item.Id == location.Id && item.Note == "Synthetic locator note."), "Location reload failed.");
        Assert(notes.Count == 2 && notes[0].Id == note.Id && notes[0].SourceId == first.Id && notes[0].SourceLocationId == location.Id
            && notes[1].SourceLocationId is null, "Note reference reload failed.");
        Assert(notes[0].Statement == "Synthetic statement for automated verification."
            && notes[0].Excerpt == "  Synthetic excerpt.\r\n"
            && notes[0].OwnParaphraseOrAssessment == "Synthetic personal summary for test purposes only.", "Original and personal text were conflated or trimmed.");
        var details = await reloaded.GetSourceDetailsAsync(first.Id);
        Assert(details.Sources.Count == 1 && details.Locations.Count == 6 && details.Notes.Count == 2, "Selected-source snapshot failed.");
        var missingTopics = new SourceService(new JsonSourceRepository(), new EmptyTopicRepository());
        Assert((await missingTopics.GetSourcesAsync()).Single(item => item.Source.Id == first.Id).HealthTopicTitle is null,
            "Missing topic must not remove source records.");
        using (var document = JsonDocument.Parse(await File.ReadAllTextAsync(LocalHealthNotebookPaths.SourcesFilePath)))
        {
            Assert(document.RootElement.GetProperty("Store").GetString() == "SASD.HealthNotebook.Sources"
                && document.RootElement.GetProperty("Version").GetInt32() == 1, "Store ownership missing.");
            foreach (string collection in new[] { "Locations", "Notes" })
                Assert(document.RootElement.GetProperty(collection).EnumerateArray().All(item => !item.TryGetProperty("SourceTitle", out _)), "Source title copied redundantly.");
        }
        using (var backup = JsonDocument.Parse(await File.ReadAllTextAsync(Path.Combine(root, "sources.backup.json")) ))
            Assert(backup.RootElement.GetProperty("Notes").GetArrayLength() == 1, "Last-good backup failed.");
        byte[] primary = await File.ReadAllBytesAsync(LocalHealthNotebookPaths.SourcesFilePath);
        await ExpectFailureAsync<InvalidDataException>(() => repository.AddSourceAsync(first));
        await ExpectFailureAsync<InvalidDataException>(() => repository.AddLocationAsync(SourceLocation.Create(Guid.NewGuid(), SourceLocationType.Page, "Page 17")));
        await ExpectFailureAsync<InvalidDataException>(() => repository.AddNoteAsync(EvidenceNote.Create(second.Id, location.Id, "Synthetic", "", "Synthetic summary")));
        await ExpectFailureAsync<OperationCanceledException>(() => repository.AddSourceAsync(valid, new CancellationToken(canceled: true)));
        byte[] primaryAfter = await File.ReadAllBytesAsync(LocalHealthNotebookPaths.SourcesFilePath);
        byte[] topicsAfter = await File.ReadAllBytesAsync(LocalHealthNotebookPaths.HealthTopicsFilePath);
        byte[] entriesAfter = await File.ReadAllBytesAsync(LocalHealthNotebookPaths.HealthEntriesFilePath);
        Assert(primary.SequenceEqual(primaryAfter), "Rejected write changed store.");
        Assert(topicsBefore.SequenceEqual(topicsAfter), "Sources modified topics.");
        Assert(entriesBefore.SequenceEqual(entriesAfter), "Sources modified entries.");

        // SEC-SRC-001: reject corrupted, foreign, future and inconsistent stores without overwriting any bytes.
        try
        {
            string emptyStore = "{\"Store\":\"SASD.HealthNotebook.Sources\",\"Version\":1,\"Sources\":[],\"Locations\":[],\"Notes\":[]}";
            var corruptRelationship = JsonNode.Parse(primary)!.AsObject();
            corruptRelationship["Notes"]![0]!["SourceLocationId"] = Guid.NewGuid().ToString();
            var foreignLocation = JsonNode.Parse(primary)!.AsObject();
            foreignLocation["Notes"]![0]!["SourceId"] = second.Id.ToString();
            foreignLocation["Notes"]![0]!["SourceLocationId"] = location.Id.ToString();
            var orphanLocation = JsonNode.Parse(primary)!.AsObject();
            orphanLocation["Locations"]![0]!["SourceId"] = Guid.NewGuid().ToString();
            var duplicateSource = JsonNode.Parse(primary)!.AsObject();
            duplicateSource["Sources"]!.AsArray().Add(duplicateSource["Sources"]![0]!.DeepClone());
            var invalidType = JsonNode.Parse(primary)!.AsObject();
            invalidType["Sources"]![0]!["SourceType"] = "Synthetic private text must not appear in errors";
            foreach (string bad in new[] { "", "null", "[]", "{}", "{broken", emptyStore.Replace("SASD.HealthNotebook.Sources", "Other"),
                emptyStore.Replace("\"Version\":1", "\"Version\":2"), emptyStore.Replace("\"Notes\":[]", "\"Notes\":null"),
                emptyStore.Replace("\"Version\":1", "\"Version\":1,\"Future\":true"), corruptRelationship.ToJsonString(), foreignLocation.ToJsonString(),
                orphanLocation.ToJsonString(), duplicateSource.ToJsonString(), invalidType.ToJsonString(),
                System.Text.Encoding.UTF8.GetString(topicsBefore), System.Text.Encoding.UTF8.GetString(entriesBefore) })
            {
                UseNewRoot(root);
                string path = LocalHealthNotebookPaths.SourcesFilePath;
                await File.WriteAllTextAsync(path, bad);
                await ExpectFailureAsync<InvalidDataException>(() => new JsonSourceRepository().LoadAsync());
                await ExpectFailureAsync<InvalidDataException>(() => new JsonSourceRepository().AddSourceAsync(valid));
                Assert(await File.ReadAllTextAsync(path) == bad, "Invalid source store overwritten.");
            }
            UseNewRoot(root);
            await File.WriteAllTextAsync(LocalHealthNotebookPaths.SourcesFilePath, emptyStore);
            var emptyRepository = new JsonSourceRepository();
            Assert((await emptyRepository.LoadAsync()).Sources.Count == 0, "Valid empty source store rejected.");
            await emptyRepository.AddSourceAsync(valid);
            Assert((await emptyRepository.LoadAsync()).Sources.Count == 1, "Append to empty source store failed.");
            foreach (string suffix in new[] { ".tmp", ".lock" })
            {
                UseNewRoot(root);
                var guarded = new JsonSourceRepository(); await guarded.AddSourceAsync(valid);
                byte[] before = await File.ReadAllBytesAsync(LocalHealthNotebookPaths.SourcesFilePath);
                string guard = LocalHealthNotebookPaths.SourcesFilePath + suffix;
                await File.WriteAllTextAsync(guard, "Synthetic unfinished write");
                await ExpectFailureAsync<IOException>(() => guarded.AddSourceAsync(Source.Create(SourceType.Book, "Synthetic second")));
                byte[] after = await File.ReadAllBytesAsync(LocalHealthNotebookPaths.SourcesFilePath);
                Assert(before.SequenceEqual(after)
                    && await File.ReadAllTextAsync(guard) == "Synthetic unfinished write", "Temp/lock file changed.");
            }
            UseNewRoot(root);
            var backupRepository = new JsonSourceRepository(); await backupRepository.AddSourceAsync(valid);
            string backupPath = Path.Combine(LocalHealthNotebookPaths.DataDirectory, "sources.backup.json");
            await File.WriteAllTextAsync(backupPath, "Synthetic foreign backup");
            byte[] beforeBackup = await File.ReadAllBytesAsync(LocalHealthNotebookPaths.SourcesFilePath);
            await ExpectFailureAsync<InvalidDataException>(() => backupRepository.AddSourceAsync(Source.Create(SourceType.Book, "Synthetic second")));
            byte[] afterBackup = await File.ReadAllBytesAsync(LocalHealthNotebookPaths.SourcesFilePath);
            Assert(beforeBackup.SequenceEqual(afterBackup)
                && await File.ReadAllTextAsync(backupPath) == "Synthetic foreign backup", "Foreign backup overwritten.");
            UseNewRoot(root);
            await File.WriteAllBytesAsync(Path.Combine(LocalHealthNotebookPaths.DataDirectory, "sources.backup.json"), primary);
            await ExpectFailureAsync<InvalidDataException>(() => new JsonSourceRepository().LoadAsync());
            await ExpectFailureAsync<InvalidDataException>(() => new JsonSourceRepository().AddSourceAsync(valid));
            Assert(!File.Exists(LocalHealthNotebookPaths.SourcesFilePath), "Missing primary silently recreated.");
        }
        finally { Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, root); }
    }
    private static CreateEvidenceNoteRequest NoteRequest(Guid source, Guid? location) => new() { SourceId = source, SourceLocationId = location,
        Statement = "Synthetic statement for automated verification.", Excerpt = "  Synthetic excerpt.\r\n",
        OwnParaphraseOrAssessment = "Synthetic personal summary for test purposes only." };
    private static void UseNewRoot(string parent)
    {
        string path = Path.Combine(parent, "source-failures", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path); Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, path);
    }
    private static void ExpectArgument(Action action)
    {
        try { action(); } catch (ArgumentException) { return; }
        throw new InvalidOperationException("Invalid source input accepted.");
    }
    private static async Task ExpectFailureAsync<T>(Func<Task> action) where T : Exception
    {
        try { await action(); } catch (T exception)
        {
            Assert(!exception.ToString().Contains("Synthetic private text must not appear in errors", StringComparison.Ordinal),
                "Source content leaked into an exception.");
            return;
        }
        throw new InvalidOperationException("Expected safe source failure did not occur.");
    }
    private static void Assert(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
    private sealed class EmptyTopicRepository : IHealthTopicRepository
    {
        public Task<IReadOnlyList<HealthTopic>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<HealthTopic>>(Array.Empty<HealthTopic>());
        public Task AddAsync(HealthTopic topic, CancellationToken cancellationToken = default) => throw new InvalidOperationException("Must not write topics.");
        public Task SaveAllAsync(IReadOnlyList<HealthTopic> topics, CancellationToken cancellationToken = default) => throw new InvalidOperationException("Must not write topics.");
    }
}
