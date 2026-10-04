using System.Text.Json;
using System.Text.Json.Serialization;
using Sasd.HealthNotebook.Application.Repositories;
using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.Infrastructure.Storage;

/// <summary>Compatible bare-list topic store with atomic, conflict-checked lifecycle writes.</summary>
public sealed class JsonHealthTopicRepository : IHealthTopicRepository
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true, PropertyNameCaseInsensitive = true, UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        Converters = { new JsonStringEnumConverter() } };
    private readonly string _path;
    /// <summary>Captures one directory; optional paths keep synthetic tests isolated.</summary>
    public JsonHealthTopicRepository(string? filePath = null) => _path = Path.GetFullPath(filePath ?? LocalHealthNotebookPaths.HealthTopicsFilePath);
    /// <inheritdoc />
    public async Task<IReadOnlyList<HealthTopic>> GetAllAsync(CancellationToken cancellationToken = default) => await LoadAsync(_path, cancellationToken).ConfigureAwait(false);
    /// <inheritdoc />
    public Task AddAsync(HealthTopic topic, CancellationToken cancellationToken = default) => WriteAsync(topics =>
    { Validate(topic); if (topics.Any(item => item.Id == topic.Id)) throw new ArgumentException("Duplicate topic identifier."); topics.Add(topic); return Task.FromResult(true); }, false, cancellationToken);
    /// <summary>Initialization only; bulk replacement cannot bypass reference integrity.</summary>
    public Task SaveAllAsync(IReadOnlyList<HealthTopic> topics, CancellationToken cancellationToken = default) => WriteAsync(current =>
    { if (current.Count != 0) throw new InvalidOperationException("Bulk replacement of existing topics is not supported."); current.AddRange(topics); return Task.FromResult(current.Count != 0); }, false, cancellationToken);
    /// <inheritdoc />
    public Task UpdateAsync(HealthTopic replacement, DateTimeOffset expectedModifiedAt, CancellationToken cancellationToken = default) => WriteAsync(topics =>
    {
        var current = Require(topics, replacement.Id, expectedModifiedAt); Validate(replacement);
        if (current.Title == replacement.Title && current.Status == replacement.Status && current.Priority == replacement.Priority && current.ShortDescription == replacement.ShortDescription && current.Notes == replacement.Notes) return Task.FromResult(false);
        replacement.CreatedAt = current.CreatedAt; replacement.ModifiedAt = NextTime(current.ModifiedAt);
        replacement.StatusBeforeArchive = replacement.Status == HealthTopicStatus.Archived ? (current.Status == HealthTopicStatus.Archived ? current.StatusBeforeArchive : current.Status) : null;
        topics[topics.IndexOf(current)] = replacement; return Task.FromResult(true);
    }, false, cancellationToken);
    /// <inheritdoc />
    public Task SetArchivedAsync(Guid id, bool archived, DateTimeOffset expectedModifiedAt, CancellationToken cancellationToken = default) => WriteAsync(topics =>
    {
        var current = Require(topics, id, expectedModifiedAt); if ((current.Status == HealthTopicStatus.Archived) == archived) return Task.FromResult(false);
        if (archived) { current.StatusBeforeArchive = current.Status; current.Status = HealthTopicStatus.Archived; }
        else { current.Status = current.StatusBeforeArchive ?? HealthTopicStatus.Observation; current.StatusBeforeArchive = null; }
        current.ModifiedAt = NextTime(current.ModifiedAt); return Task.FromResult(true);
    }, false, cancellationToken);
    /// <inheritdoc />
    public Task DeleteIfUnreferencedAsync(Guid id, DateTimeOffset expectedModifiedAt, CancellationToken cancellationToken = default) => WriteAsync(async topics =>
    {
        var current = Require(topics, id, expectedModifiedAt); string directory = Path.GetDirectoryName(_path)!;
        // Hold all related writer locks through the complete check and atomic deletion.
        // Archived parents and historical instruction snapshots remain real references.
        var entries = await new JsonHealthEntryRepository(Path.Combine(directory, "health-entries.json")).GetAllAsync(cancellationToken).ConfigureAwait(false);
        var measurements = await new JsonMeasurementRepository(Path.Combine(directory, "measurements.json")).GetAllAsync(cancellationToken).ConfigureAwait(false);
        var sources = await new JsonSourceRepository(Path.Combine(directory, "sources.json")).LoadAsync(cancellationToken).ConfigureAwait(false);
        var sessions = await new JsonSessionRepository(Path.Combine(directory, "sessions.json")).LoadAsync(cancellationToken).ConfigureAwait(false);
        var actions = await new JsonHealthActionRepository(Path.Combine(directory, "health-actions.json")).LoadAsync(cancellationToken).ConfigureAwait(false);
        if (entries.Any(item => item.HealthTopicId == id) || measurements.Any(item => item.HealthTopicId == id) || sources.Sources.Any(item => item.HealthTopicId == id)
            || sessions.Sessions.Any(item => item.HealthTopicId == id) || actions.Actions.Any(item => item.HealthTopicId == id) || actions.Revisions.Any(item => item.Previous.HealthTopicId == id)) throw new LifecycleDeleteBlockedException();
        topics.Remove(current); return true;
    }, true, cancellationToken);
    private async Task WriteAsync(Func<List<HealthTopic>, Task<bool>> change, bool allLocks, CancellationToken cancellationToken)
    {
        string directory = Path.GetDirectoryName(_path)!; TopicReferenceGuard.RejectLink(directory); Directory.CreateDirectory(directory); var locks = new List<FileStream>();
        try
        {
            // Fixed order, fail-fast CreateNew; partial acquisition never changes data.
            string[] files = allLocks ? new[] { Path.GetFileName(_path), "health-entries.json", "measurements.json", "sources.json", "sessions.json", "health-actions.json" } : new[] { Path.GetFileName(_path) };
            foreach (string file in files)
            { cancellationToken.ThrowIfCancellationRequested(); string path = Path.Combine(directory, file + ".lock"); TopicReferenceGuard.RejectLink(path); locks.Add(new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, FileOptions.DeleteOnClose)); }
            var topics = await LoadAsync(_path, cancellationToken).ConfigureAwait(false);
            string backup = Path.Combine(directory, Path.GetFileNameWithoutExtension(_path) + ".backup.json"); TopicReferenceGuard.RejectLink(backup);
            if (Directory.Exists(backup)) throw new InvalidDataException("Topic backup unavailable.");
            if (File.Exists(backup)) await LoadAsync(backup, cancellationToken).ConfigureAwait(false);
            if (!await change(topics).ConfigureAwait(false)) return;
            ValidateList(topics); string temp = _path + ".tmp"; TopicReferenceGuard.RejectLink(temp); bool created = false;
            try
            {
                await using (var stream = new FileStream(temp, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                { created = true; await JsonSerializer.SerializeAsync(stream, topics, Options, cancellationToken).ConfigureAwait(false); await stream.FlushAsync(cancellationToken).ConfigureAwait(false); stream.Flush(true); }
                cancellationToken.ThrowIfCancellationRequested(); TopicReferenceGuard.RejectLink(_path);
                if (File.Exists(_path)) File.Replace(temp, _path, backup); else File.Move(temp, _path);
            }
            finally { if (created && File.Exists(temp)) File.Delete(temp); }
        }
        finally { foreach (var held in Enumerable.Reverse(locks)) held.Dispose(); }
    }
    private static async Task<List<HealthTopic>> LoadAsync(string path, CancellationToken cancellationToken)
    {
        TopicReferenceGuard.RejectLink(path);
        if (!File.Exists(path))
        { string backup = Path.Combine(Path.GetDirectoryName(path)!, Path.GetFileNameWithoutExtension(path) + ".backup.json"); TopicReferenceGuard.RejectLink(backup); if (Directory.Exists(path) || File.Exists(backup) || Directory.Exists(backup)) throw new InvalidDataException("Topic store unavailable; existing data retained."); return new(); }
        try
        { await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete); var topics = await JsonSerializer.DeserializeAsync<List<HealthTopic>>(stream, Options, cancellationToken).ConfigureAwait(false); if (topics is null) throw new InvalidDataException("Invalid topic collection."); ValidateList(topics); return topics; }
        catch (JsonException) { throw new InvalidDataException("Cannot read topic store; existing data retained."); }
        catch (ArgumentException) { throw new InvalidDataException("Invalid topic metadata; existing data retained."); }
    }
    private static void ValidateList(List<HealthTopic> topics) { foreach (var topic in topics) Validate(topic); if (topics.Select(item => item.Id).Distinct().Count() != topics.Count) throw new InvalidDataException("Duplicate topics."); }
    private static void Validate(HealthTopic topic)
    { if (topic is null || topic.Id == Guid.Empty || string.IsNullOrWhiteSpace(topic.Title) || topic.Title.Length > 160 || topic.ShortDescription is null || topic.ShortDescription.Length > 500 || topic.Notes is null || topic.Notes.Length > 4000 || !Enum.IsDefined(topic.Status) || !Enum.IsDefined(topic.Priority) || topic.CreatedAt == default || topic.ModifiedAt < topic.CreatedAt || (topic.StatusBeforeArchive.HasValue && (!Enum.IsDefined(topic.StatusBeforeArchive.Value) || topic.StatusBeforeArchive == HealthTopicStatus.Archived))) throw new ArgumentException("Invalid topic fields."); }
    private static HealthTopic Require(List<HealthTopic> topics, Guid id, DateTimeOffset expected) { var topic = topics.SingleOrDefault(item => item.Id == id); if (topic is null || topic.ModifiedAt != expected) throw new LifecycleConflictException(); return topic; }
    private static DateTimeOffset NextTime(DateTimeOffset previous) { var now = DateTimeOffset.Now; return now > previous ? now : previous.AddTicks(1); }
}
