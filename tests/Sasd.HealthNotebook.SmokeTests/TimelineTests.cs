using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.Infrastructure.Storage;

namespace Sasd.HealthNotebook.SmokeTests;

/// <summary>IT-TIM-002: mixed chronology, source identity, read-only loading and failure propagation.</summary>
internal static class TimelineTests
{
    internal static async Task RunAsync()
    {
        string? previous = Environment.GetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable);
        try
        {
            string root = Path.Combine(previous!, "timeline-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(root);
            Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, root);
            var topics = new JsonHealthTopicRepository();
            var entryRepository = new JsonHealthEntryRepository();
            var measurementRepository = new JsonMeasurementRepository();
            var entries = new HealthEntryService(entryRepository, topics);
            var measurements = new MeasurementService(measurementRepository, topics);
            DateTimeOffset time = new(2026, 10, 1, 12, 0, 0, TimeSpan.FromHours(2));
            var entry = HealthEntry.Create(HealthEntryType.Note, time, "CODEX TEST – Mixed chronology", "Synthetic");
            var measurement = Measurement.Create(MeasurementType.Pulse, time.ToOffset(TimeSpan.FromHours(-4)).AddMinutes(1), value: 42)
                with { Id = entry.Id };
            await entryRepository.AddAsync(entry);
            await measurementRepository.AddAsync(measurement);
            var tiedMeasurement = Measurement.Create(MeasurementType.Weight, time, value: 20)
                with { CreatedAt = entry.CreatedAt, ModifiedAt = entry.CreatedAt };
            await measurementRepository.AddAsync(tiedMeasurement);
            var before = Directory.GetFiles(root).ToDictionary(path => path, File.ReadAllBytes);
            var service = new TimelineService(entries, measurements);
            var items = await service.GetItemsAsync();
            Assert(items.Count == 3 && items[0].Measurement is not null && items[1].Entry is not null && items[2].Id == tiedMeasurement.Id,
                "Mixed timeline did not order by absolute documented instant.");
            Assert(items[0].Id == items[1].Id && items[0].Measurement!.Measurement.ModifiedAt == measurement.ModifiedAt,
                "Source collision or displayed conflict token was lost.");
            Assert((await service.GetItemsAsync()).SequenceEqual(items), "Timeline order changed across reload.");
            Assert(Directory.GetFiles(root).Length == before.Count && before.All(pair => pair.Value.SequenceEqual(File.ReadAllBytes(pair.Key))),
                "Read-only timeline modified source files.");
            await File.WriteAllTextAsync(LocalHealthNotebookPaths.MeasurementsFilePath, "invalid synthetic store");
            bool rejected = false;
            try { await service.GetItemsAsync(); } catch { rejected = true; }
            Assert(rejected, "Failed measurement load appeared as a partial timeline.");
            Assert(await File.ReadAllTextAsync(LocalHealthNotebookPaths.MeasurementsFilePath) == "invalid synthetic store",
                "Timeline rewrote damaged source.");
        }
        finally { Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, previous); }
    }
    private static void Assert(bool valid, string message)
    { if (!valid) throw new InvalidOperationException(message); }
}
