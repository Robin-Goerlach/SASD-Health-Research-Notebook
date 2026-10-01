using System.Text.Json;
using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.Infrastructure.Storage;

namespace Sasd.HealthNotebook.SmokeTests;

/// <summary>IT-SRC-RELOAD-001: fresh service/repository detail loading from an isolated persisted location.</summary>
internal static class SourceLocationReloadTests
{
    internal static async Task RunAsync()
    {
        string original = LocalHealthNotebookPaths.DataDirectory;
        string root = Path.Combine(original, "source-location-reload", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, root);
        try
        {
            // Synthetic sentinels must remain byte-for-byte untouched by source operations.
            string[] otherFiles = { "health-topics.json", "health-entries.json", "measurements.json" };
            await File.WriteAllTextAsync(Path.Combine(root, otherFiles[0]), "[]");
            foreach (string file in otherFiles.Skip(1)) await File.WriteAllTextAsync(Path.Combine(root, file), "Synthetic unrelated store sentinel.");
            var before = new Dictionary<string, byte[]>();
            foreach (string file in otherFiles) before[file] = await File.ReadAllBytesAsync(Path.Combine(root, file));
            var expected = await CreateAsync();
            string path = LocalHealthNotebookPaths.SourcesFilePath;
            byte[] stored = await File.ReadAllBytesAsync(path);
            using (var json = JsonDocument.Parse(stored))
            {
                var location = json.RootElement.GetProperty("Locations").EnumerateArray().Single();
                Assert(location.GetProperty("Id").GetGuid() == expected.Id
                    && location.GetProperty("SourceId").GetGuid() == expected.SourceId
                    && location.GetProperty("LocationType").GetString() == "Page"
                    && location.GetProperty("Locator").GetString() == expected.Locator
                    && location.GetProperty("Note").GetString() == expected.Note
                    && location.GetProperty("CreatedAt").GetDateTimeOffset() == expected.CreatedAt,
                    "Location was not persisted with its original fields.");
            }
            // CreateAsync has returned: no original service/repository is reused.
            var repository = new JsonSourceRepository();
            var service = new SourceService(repository, new JsonHealthTopicRepository());
            Assert(LocalHealthNotebookPaths.SourcesFilePath == path, "Restart changed the shared source data path.");
            Assert((await repository.LoadAsync()).Locations.Single().Id == expected.Id, "Location missing after deserialization.");
            Assert((await service.GetSourceAsync(expected.SourceId))?.Id == expected.SourceId, "Source missing after restart.");
            var actual = (await service.GetSourceDetailsAsync(expected.SourceId)).Locations.Single();
            Assert(actual.Id == expected.Id && actual.SourceId == expected.SourceId && actual.LocationType == expected.LocationType
                && actual.Locator == expected.Locator && actual.Note == expected.Note && actual.CreatedAt == expected.CreatedAt,
                "Fresh service detail path lost the original location.");
            byte[] after = await File.ReadAllBytesAsync(path);
            Assert(stored.SequenceEqual(after), "Source detail loading modified the store.");
            foreach (string file in otherFiles)
            {
                byte[] otherAfter = await File.ReadAllBytesAsync(Path.Combine(root, file));
                Assert(before[file].SequenceEqual(otherAfter), "Source workflow modified another notebook store.");
            }
            Console.WriteLine("SourceLocation persisted JSON and fresh service detail reload passed.");
        }
        finally { Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, original); }
    }
    private static async Task<SourceLocation> CreateAsync()
    {
        var service = new SourceService(new JsonSourceRepository(), new JsonHealthTopicRepository());
        var source = await service.CreateSourceAsync(new CreateSourceRequest { Title = "CODEX TEST – Location reload", SourceType = SourceType.Book });
        return await service.CreateLocationAsync(new CreateSourceLocationRequest { SourceId = source.Id,
            LocationType = SourceLocationType.Page, Locator = "Page 17", Note = "Synthetic persisted location note." });
    }
    private static void Assert(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
}
