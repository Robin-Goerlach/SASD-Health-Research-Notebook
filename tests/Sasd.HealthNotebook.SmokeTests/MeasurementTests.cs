using System.Text.Json;
using System.Text.Json.Nodes;
using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Repositories;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.Infrastructure.Storage;

namespace Sasd.HealthNotebook.SmokeTests;

/// <summary>UT/IT/SEC-MEA-001: numeric invariants, isolated persistence and preservation of existing stores.</summary>
internal static class MeasurementTests
{
    private static readonly DateTimeOffset Time = new(2026, 9, 20, 14, 0, 0, TimeSpan.FromHours(2));
    internal static async Task RunAsync()
    {
        string original = LocalHealthNotebookPaths.DataDirectory;
        Assert(original.StartsWith(Path.Combine(Program.FindRepositoryRoot(), ".codex") + Path.DirectorySeparatorChar,
            StringComparison.OrdinalIgnoreCase), "Measurement tests must use repository .codex.");
        CheckDomain();
        string[] priorFiles = { "health-topics.json", "health-entries.json", "sources.json" };
        var priorBytes = new Dictionary<string, byte[]>();
        foreach (string file in priorFiles) priorBytes[file] = await File.ReadAllBytesAsync(Path.Combine(original, file));
        Guid topicId = (await new JsonHealthTopicRepository().GetAllAsync())[0].Id;
        var repository = new JsonMeasurementRepository();
        var service = new MeasurementService(repository, new JsonHealthTopicRepository());
        Assert((await service.GetMeasurementsAsync()).Count == 0, "Missing measurement store is not empty.");
        foreach (MeasurementType type in Enum.GetValues<MeasurementType>())
            await service.CreateMeasurementAsync(Request(type, Time.AddDays((int)type), (int)type % 2 == 0 ? topicId : null));
        // Sorting is by actual instant, including offsets, not by displayed local time.
        await service.CreateMeasurementAsync(Request(MeasurementType.Pulse, Time.AddDays(10).ToOffset(TimeSpan.FromHours(-4)), null));
        var reloaded = await new MeasurementService(new JsonMeasurementRepository(), new JsonHealthTopicRepository()).GetMeasurementsAsync();
        Assert(reloaded.Count == 6 && reloaded[0].Measurement.OccurredAt == Time.AddDays(10), "Measurement chronological reload failed.");
        Assert(reloaded.Zip(reloaded.Skip(1)).All(pair => pair.First.Measurement.OccurredAt >= pair.Second.Measurement.OccurredAt), "Measurement order is incorrect.");
        Assert((await service.GetMeasurementsAsync(topicId)).Count == 3, "Measurement topic filter failed.");
        var bp = reloaded.Single(item => item.Measurement.MeasurementType == MeasurementType.BloodPressure).Measurement;
        Assert(bp.Systolic == 120 && bp.Diastolic == 80 && bp.Pulse == 70 && bp.Value is null, "Pressure components did not roundtrip separately.");
        Assert(reloaded.All(item => item.Measurement.Note == "CODEX TEST – Synthetic measurement only."
            && item.Measurement.Context == "Synthetic measurement situation." && item.Measurement.AcquisitionMethod == "Manual"), "Personal text or acquisition method lost.");
        foreach (MeasurementType type in Enum.GetValues<MeasurementType>())
        {
            var expected = Request(type, Time.AddDays((int)type), (int)type % 2 == 0 ? topicId : null);
            var actual = reloaded.Single(item => item.Measurement.OccurredAt == expected.OccurredAt).Measurement;
            MeasurementUnit unit = type switch { MeasurementType.BloodPressure => MeasurementUnit.MmHg,
                MeasurementType.Pulse => MeasurementUnit.PerMinute, MeasurementType.Temperature => MeasurementUnit.Celsius,
                MeasurementType.BloodGlucose => MeasurementUnit.MgPerDeciliter, _ => MeasurementUnit.Kilogram };
            Assert(actual.MeasurementType == type && actual.Unit == unit && actual.Value == expected.Value
                && actual.Systolic == expected.Systolic && actual.Diastolic == expected.Diastolic && actual.Pulse == expected.Pulse,
                "Numeric value/type/unit roundtrip failed.");
        }
        await ExpectFailure<ArgumentException>(() => service.CreateMeasurementAsync(Request(MeasurementType.Weight, Time, Guid.NewGuid())));
        Assert((await repository.GetAllAsync()).Count == 6, "Invalid topic changed measurements.");
        Assert(LocalHealthNotebookPaths.MeasurementsFilePath == Path.Combine(original, "measurements.json"), "Shared measurement path override ignored.");
        byte[] validStore = await File.ReadAllBytesAsync(LocalHealthNotebookPaths.MeasurementsFilePath);
        using (var json = JsonDocument.Parse(validStore))
        {
            Assert(json.RootElement.GetProperty("Store").GetString() == "SASD.HealthNotebook.Measurements"
                && json.RootElement.GetProperty("Version").GetInt32() == 1, "Measurement store contract changed.");
            foreach (var item in json.RootElement.GetProperty("Measurements").EnumerateArray())
            {
                Assert(!item.TryGetProperty("HealthTopicTitle", out _), "Measurement copied a topic title.");
                Assert(item.GetProperty("MeasurementType").ValueKind == JsonValueKind.String && item.GetProperty("Unit").ValueKind == JsonValueKind.String, "Type/unit enum contract failed.");
                string field = item.GetProperty("MeasurementType").GetString() == "BloodPressure" ? "Systolic" : "Value";
                Assert(item.GetProperty(field).ValueKind == JsonValueKind.Number, "Measurement persisted a formatted string.");
            }
        }
        using (var backup = JsonDocument.Parse(await File.ReadAllBytesAsync(Path.Combine(original, "measurements.backup.json"))))
            Assert(backup.RootElement.GetProperty("Measurements").GetArrayLength() == 5, "Backup is not the previous committed store.");
        var topics = new TopicStub { Topics = new[] { new HealthTopic { Id = topicId, Title = "CODEX TEST – Renamed topic", Status = HealthTopicStatus.Archived } } };
        var projected = new MeasurementService(repository, topics);
        Assert((await projected.GetMeasurementsAsync(topicId)).All(item => item.HealthTopicTitle == "CODEX TEST – Renamed topic"), "Current archived topic not resolved.");
        topics.Topics = Array.Empty<HealthTopic>();
        var missing = await projected.GetMeasurementsAsync();
        Assert(missing.Count == 6 && missing.All(item => item.HealthTopicTitle is null), "Missing topics lost measurements.");
        await ExpectFailure<InvalidOperationException>(() => repository.AddAsync(bp));
        await ExpectFailure<OperationCanceledException>(() => repository.AddAsync(Single(), new CancellationToken(true)));
        Assert(await BytesUnchanged(validStore, LocalHealthNotebookPaths.MeasurementsFilePath), "Rejected/canceled write changed store.");
        try { await CheckFailures(original, validStore, priorBytes["health-topics.json"]); }
        finally { Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, original); }
        foreach (string file in priorFiles)
            Assert(await BytesUnchanged(priorBytes[file], Path.Combine(original, file)), "Measurement writes modified an existing notebook store.");
        Console.WriteLine("Measurement domain/application/persistence checks passed (FR-MEA-001/002/004/005/007).");
    }
    private static void CheckDomain()
    {
        foreach (MeasurementType type in Enum.GetValues<MeasurementType>())
        {
            var request = Request(type, Time, null);
            var valid = Measurement.Create(type, Time, request.Value, request.Systolic, request.Diastolic, request.Pulse);
            Assert(valid.Unit == Measurement.UnitFor(type) && valid.CreatedAt == valid.ModifiedAt && valid.HealthTopicId is null, "Measurement defaults failed.");
            ExpectArgument(() => Measurement.Create(type, Time));
            foreach (double invalid in new[] { -1d, double.NaN, double.PositiveInfinity, double.NegativeInfinity })
                ExpectArgument(() => type == MeasurementType.BloodPressure
                    ? Measurement.Create(type, Time, systolic: invalid, diastolic: 80)
                    : Measurement.Create(type, Time, value: invalid));
        }
        ExpectArgument(() => Measurement.Create(MeasurementType.BloodPressure, Time, systolic: 120));
        ExpectArgument(() => Measurement.Create(MeasurementType.BloodPressure, Time, diastolic: 80));
        ExpectArgument(() => Measurement.Create(MeasurementType.BloodPressure, Time, value: 1, systolic: 120, diastolic: 80));
        foreach (double invalid in new[] { -1d, double.NaN, double.PositiveInfinity })
        {
            ExpectArgument(() => Measurement.Create(MeasurementType.BloodPressure, Time, systolic: 120, diastolic: invalid));
            ExpectArgument(() => Measurement.Create(MeasurementType.BloodPressure, Time, systolic: 120, diastolic: 80, pulse: invalid));
        }
        ExpectArgument(() => Measurement.Create(MeasurementType.Pulse, Time, value: 70, pulse: 70));
        ExpectArgument(() => Measurement.Create(MeasurementType.Weight, Time, value: 80, systolic: 120));
        ExpectArgument(() => Measurement.Create((MeasurementType)999, Time, value: 1));
        ExpectArgument(() => Measurement.Create(MeasurementType.Pulse, default, value: 70));
        ExpectArgument(() => Measurement.Create(MeasurementType.Weight, Time, value: 80, healthTopicId: Guid.Empty));
        ExpectArgument(() => Measurement.Create(MeasurementType.Weight, Time, value: 80, note: new string('x', 4001)));
        ExpectArgument(() => Measurement.Create(MeasurementType.Weight, Time, value: 80, context: new string('x', 501)));
        Measurement.Create(MeasurementType.Weight, Time, value: double.MaxValue, note: new string('x', 4000), context: new string('x', 500));
        // No clinical lower/upper limits or component comparison: only structural validity.
        Measurement.Create(MeasurementType.BloodPressure, Time, systolic: 0, diastolic: 1000000);
        Measurement.Create(MeasurementType.Temperature, Time, value: 0);
    }
    private static async Task CheckFailures(string original, byte[] validStore, byte[] foreignStore)
    {
        foreach (string content in new[] { "", "null", "[]", "{}", "{broken",
            "{\"Store\":\"Other\",\"Version\":1,\"Measurements\":[]}",
            "{\"Store\":\"SASD.HealthNotebook.Measurements\",\"Version\":2,\"Measurements\":[]}",
            "{\"Store\":\"SASD.HealthNotebook.Measurements\",\"Version\":1,\"Measurements\":null}",
            "{\"Store\":\"SASD.HealthNotebook.Measurements\",\"Version\":1,\"Measurements\":[],\"Future\":true}",
            System.Text.Encoding.UTF8.GetString(foreignStore) })
        {
            string root = FailureRoot(original); string path = LocalHealthNotebookPaths.MeasurementsFilePath;
            await File.WriteAllTextAsync(path, content);
            var repo = new JsonMeasurementRepository();
            await ExpectFailure<InvalidDataException>(() => repo.GetAllAsync());
            await ExpectFailure<InvalidDataException>(() => repo.AddAsync(Single()));
            Assert(await File.ReadAllTextAsync(path) == content, "Rejected measurement JSON was overwritten.");
        }
        // Invalid numeric/domain metadata is also rejected during deserialization.
        foreach (string field in new[] { "Unit", "AcquisitionMethod", "Value", "HealthTopicId" })
        {
            FailureRoot(original);
            var json = JsonNode.Parse(validStore)!;
            var row = json["Measurements"]![1]!; // standalone pulse
            row[field] = field switch { "Unit" => JsonValue.Create("Kilogram"), "AcquisitionMethod" => JsonValue.Create("Unknown"),
                "Value" => JsonValue.Create(-1d), _ => JsonValue.Create(Guid.Empty) };
            string content = json.ToJsonString();
            await File.WriteAllTextAsync(LocalHealthNotebookPaths.MeasurementsFilePath, content);
            await ExpectFailure<InvalidDataException>(() => new JsonMeasurementRepository().AddAsync(Single()));
            Assert(await File.ReadAllTextAsync(LocalHealthNotebookPaths.MeasurementsFilePath) == content, "Invalid measurement metadata overwritten.");
        }
        foreach (string suffix in new[] { ".tmp", ".lock" })
        {
            FailureRoot(original); var repo = new JsonMeasurementRepository(); await repo.AddAsync(Single());
            byte[] before = await File.ReadAllBytesAsync(LocalHealthNotebookPaths.MeasurementsFilePath);
            string protectedPath = LocalHealthNotebookPaths.MeasurementsFilePath + suffix;
            await File.WriteAllTextAsync(protectedPath, "Synthetic unfinished operation");
            await ExpectFailure<IOException>(() => repo.AddAsync(Single()));
            Assert(await BytesUnchanged(before, LocalHealthNotebookPaths.MeasurementsFilePath)
                && await File.ReadAllTextAsync(protectedPath) == "Synthetic unfinished operation", "Existing temp/lock modified.");
        }
        FailureRoot(original);
        string empty = "{\"Store\":\"SASD.HealthNotebook.Measurements\",\"Version\":1,\"Measurements\":[]}";
        await File.WriteAllTextAsync(LocalHealthNotebookPaths.MeasurementsFilePath, empty);
        var emptyRepo = new JsonMeasurementRepository(); Assert((await emptyRepo.GetAllAsync()).Count == 0, "Valid empty store rejected.");
        await emptyRepo.AddAsync(Single()); Assert((await emptyRepo.GetAllAsync()).Count == 1, "Empty store cannot be appended.");
        string missingRoot = FailureRoot(original);
        await File.WriteAllBytesAsync(Path.Combine(missingRoot, "measurements.backup.json"), validStore);
        await ExpectFailure<InvalidDataException>(() => new JsonMeasurementRepository().AddAsync(Single()));
        Assert(!File.Exists(LocalHealthNotebookPaths.MeasurementsFilePath), "Missing primary silently recreated over surviving backup.");
        foreach (string backupContent in new[] { "foreign synthetic backup", empty.Replace("\"Version\":1", "\"Version\":2") })
        {
            string root = FailureRoot(original); var repo = new JsonMeasurementRepository(); await repo.AddAsync(Single());
            byte[] primary = await File.ReadAllBytesAsync(LocalHealthNotebookPaths.MeasurementsFilePath);
            string backup = Path.Combine(root, "measurements.backup.json"); await File.WriteAllTextAsync(backup, backupContent);
            await ExpectFailure<InvalidDataException>(() => repo.AddAsync(Single()));
            Assert(await BytesUnchanged(primary, LocalHealthNotebookPaths.MeasurementsFilePath)
                && await File.ReadAllTextAsync(backup) == backupContent, "Invalid backup overwritten.");
        }
    }
    private static string FailureRoot(string original)
    {
        string root = Path.Combine(original, "measurement-failure-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root); Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, root); return root;
    }
    private static Measurement Single() => Measurement.Create(MeasurementType.Pulse, Time, value: 70);
    private static async Task<bool> BytesUnchanged(byte[] before, string path)
    {
        byte[] after = await File.ReadAllBytesAsync(path);
        return before.SequenceEqual(after);
    }
    private static CreateMeasurementRequest Request(MeasurementType type, DateTimeOffset time, Guid? topic) => new()
    {
        MeasurementType = type, OccurredAt = time, HealthTopicId = topic,
        Value = type switch { MeasurementType.Pulse => 70, MeasurementType.Temperature => 36.5, MeasurementType.BloodGlucose => 123, MeasurementType.Weight => 80, _ => null },
        Systolic = type == MeasurementType.BloodPressure ? 120 : null, Diastolic = type == MeasurementType.BloodPressure ? 80 : null,
        Pulse = type == MeasurementType.BloodPressure ? 70 : null, Note = "CODEX TEST – Synthetic measurement only.", Context = "Synthetic measurement situation."
    };
    private static void ExpectArgument(Func<Measurement> create) { try { create(); } catch (ArgumentException) { return; } throw new InvalidOperationException("Invalid measurement accepted."); }
    private static async Task ExpectFailure<T>(Func<Task> action) where T : Exception { try { await action(); } catch (T) { return; } throw new InvalidOperationException("Expected safe measurement failure did not occur."); }
    private static void Assert(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
    private sealed class TopicStub : IHealthTopicRepository
    {
        public IReadOnlyList<HealthTopic> Topics { get; set; } = Array.Empty<HealthTopic>();
        public Task<IReadOnlyList<HealthTopic>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult(Topics);
        public Task AddAsync(HealthTopic topic, CancellationToken cancellationToken = default) => throw new InvalidOperationException("Measurement service must not write topics.");
        public Task SaveAllAsync(IReadOnlyList<HealthTopic> topics, CancellationToken cancellationToken = default) => throw new InvalidOperationException("Measurement service must not write topics.");
    }
}
