using System.Text.Json;
using System.Text.Json.Serialization;
using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Repositories;
using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.Infrastructure.Storage;

/// <summary>One owned store for sources and dependents, with atomic updates and a last-good backup.</summary>
public sealed class JsonSourceRepository : ISourceRepository
{
    private const string StoreKind = "SASD.HealthNotebook.Sources";
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true, UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        Converters = { new JsonStringEnumConverter(allowIntegerValues: false) }
    };
    private readonly string _path;
    /// <summary>Resolves the same shared data directory as topics and entries.</summary>
    public JsonSourceRepository(string? filePath = null) => _path = Path.GetFullPath(filePath ?? LocalHealthNotebookPaths.SourcesFilePath);
    /// <inheritdoc />
    public async Task<SourceNotebook> LoadAsync(CancellationToken cancellationToken = default)
    {
        var store = await LoadPrimaryAsync(cancellationToken).ConfigureAwait(false);
        return new SourceNotebook(store.Sources.AsReadOnly(), store.Locations.AsReadOnly(), store.Notes.AsReadOnly());
    }
    /// <inheritdoc />
    public Task AddSourceAsync(Source source, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source); source.Validate();
        return WriteAsync(store => store.Sources.Add(source), cancellationToken);
    }
    /// <inheritdoc />
    public Task AddLocationAsync(SourceLocation location, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(location); location.Validate();
        return WriteAsync(store => store.Locations.Add(location), cancellationToken);
    }
    /// <inheritdoc />
    public Task AddNoteAsync(EvidenceNote note, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(note); note.Validate();
        return WriteAsync(store => store.Notes.Add(note), cancellationToken);
    }

    private async Task WriteAsync(Action<SourceStore> append, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string directory = Path.GetDirectoryName(_path)!;
        RejectLink(directory);
        Directory.CreateDirectory(directory);
        RejectLink(_path + ".lock");
        // Same fail-safe, create-only writer exclusion as the entry store. No lock recovery or retries.
        await using var writeLock = new FileStream(_path + ".lock", FileMode.CreateNew, FileAccess.Write,
            FileShare.None, 4096, FileOptions.DeleteOnClose);
        var store = await LoadPrimaryAsync(cancellationToken).ConfigureAwait(false);
        string backupPath = Path.Combine(directory, "sources.backup.json");
        RejectLink(backupPath);
        if (Directory.Exists(backupPath)) throw new InvalidDataException("Source backup is unavailable; existing files retained.");
        if (File.Exists(backupPath)) await LoadStoreAsync(backupPath, cancellationToken).ConfigureAwait(false);
        var previousTopics = store.Sources.ToDictionary(item => item.Id, item => item.HealthTopicId);
        append(store);
        await TopicReferenceGuard.ValidateAsync(_path, previousTopics, store.Sources.Select(item => (item.Id, item.HealthTopicId)), cancellationToken).ConfigureAwait(false);
        // Revalidate under the writer lock: even direct repository clients cannot persist
        // duplicate IDs or a note referencing a locator in a different source.
        ValidateStore(store);
        string temporaryPath = _path + ".tmp";
        RejectLink(temporaryPath);
        bool temporaryCreated = false;
        try
        {
            await using (var stream = new FileStream(temporaryPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                temporaryCreated = true;
                await JsonSerializer.SerializeAsync(stream, store, Options, cancellationToken).ConfigureAwait(false);
                await stream.FlushAsync(cancellationToken).ConfigureAwait(false);
                stream.Flush(flushToDisk: true);
            }
            cancellationToken.ThrowIfCancellationRequested();
            RejectLink(_path);
            if (File.Exists(_path)) File.Replace(temporaryPath, _path, backupPath);
            else File.Move(temporaryPath, _path);
        }
        finally
        {
            // Only remove the temporary file created by this operation. Never clean abandoned files.
            if (temporaryCreated && File.Exists(temporaryPath)) File.Delete(temporaryPath);
        }
    }
    private Task<SourceStore> LoadPrimaryAsync(CancellationToken cancellationToken)
    {
        string backupPath = Path.Combine(Path.GetDirectoryName(_path)!, "sources.backup.json");
        RejectLink(_path); RejectLink(backupPath);
        if (!File.Exists(_path) && (File.Exists(backupPath) || Directory.Exists(backupPath) || Directory.Exists(_path)))
            throw new InvalidDataException("Source store unavailable. Existing files retained; recovery is required.");
        return LoadStoreAsync(_path, cancellationToken);
    }
    private static async Task<SourceStore> LoadStoreAsync(string path, CancellationToken cancellationToken)
    {
        RejectLink(path);
        if (!File.Exists(path)) return new SourceStore { Store = StoreKind, Version = 1, Sources = new(), Locations = new(), Notes = new() };
        try
        {
            await using var stream = File.OpenRead(path);
            var store = await JsonSerializer.DeserializeAsync<SourceStore>(stream, Options, cancellationToken).ConfigureAwait(false);
            if (store is null || store.Store != StoreKind || store.Version != 1)
                throw new InvalidDataException("The source store is not a supported HealthNotebook store.");
            ValidateStore(store);
            return store;
        }
        // Serializer diagnostics can contain source quotations; do not propagate them or log them.
        catch (JsonException) { throw new InvalidDataException("Cannot read source store. Existing data retained."); }
        catch (ArgumentException) { throw new InvalidDataException("Invalid source metadata. Existing data retained."); }
    }
    private static void ValidateStore(SourceStore store)
    {
        if (store.Sources is null || store.Locations is null || store.Notes is null)
            throw new InvalidDataException("Invalid source store collections.");
        foreach (var source in store.Sources)
        {
            if (source is null) throw new InvalidDataException("Invalid source record.");
            source.Validate();
        }
        foreach (var location in store.Locations)
        {
            if (location is null) throw new InvalidDataException("Invalid location record.");
            location.Validate();
        }
        foreach (var note in store.Notes)
        {
            if (note is null) throw new InvalidDataException("Invalid note record.");
            note.Validate();
        }
        var sources = store.Sources.Select(source => source.Id).ToHashSet();
        var locations = store.Locations.Select(location => location.Id).ToHashSet();
        if (sources.Count != store.Sources.Count || locations.Count != store.Locations.Count
            || store.Notes.Select(note => note.Id).Distinct().Count() != store.Notes.Count
            || store.Locations.Any(location => !sources.Contains(location.SourceId))
            || store.Notes.Any(note => !sources.Contains(note.SourceId)
                || (note.SourceLocationId.HasValue && !store.Locations.Any(location => location.Id == note.SourceLocationId && location.SourceId == note.SourceId))))
            throw new InvalidDataException("Inconsistent source references or duplicate identifiers. Existing data retained.");
    }
    private static void RejectLink(string path)
    {
        for (string? current = Path.GetFullPath(path); current is not null; current = Path.GetDirectoryName(current))
            if ((File.Exists(current) || Directory.Exists(current)) && (File.GetAttributes(current) & FileAttributes.ReparsePoint) != 0)
                throw new IOException("Source storage must not traverse filesystem links.");
    }
    private sealed class SourceStore
    {
        public required string Store { get; init; }
        public required int Version { get; init; }
        public required List<Source> Sources { get; init; }
        public required List<SourceLocation> Locations { get; init; }
        public required List<EvidenceNote> Notes { get; init; }
    }
}
