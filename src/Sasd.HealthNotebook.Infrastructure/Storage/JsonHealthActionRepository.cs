using System.Text.Json;
using System.Text.Json.Serialization;
using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Repositories;
using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.Infrastructure.Storage;

/// <summary>One owned action/routine/history store with atomic updates and a last-good backup.</summary>
public sealed class JsonHealthActionRepository : IHealthActionRepository
{
    private const string StoreKind = "SASD.HealthNotebook.HealthActions";
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true, UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        Converters = { new JsonStringEnumConverter(allowIntegerValues: false) }
    };
    private readonly string _path;
    /// <summary>Resolves the same shared data directory as topics and entries.</summary>
    public JsonHealthActionRepository() => _path = LocalHealthNotebookPaths.HealthActionsFilePath;
    /// <inheritdoc />
    public async Task<HealthActionNotebook> LoadAsync(CancellationToken cancellationToken = default)
    {
        var store = await LoadPrimaryAsync(cancellationToken).ConfigureAwait(false);
        return new HealthActionNotebook(store.Actions.AsReadOnly(), store.Routines.AsReadOnly(), store.ProgressEntries.AsReadOnly());
    }
    /// <inheritdoc />
    public Task AddActionAsync(HealthAction action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action); action.Validate();
        return WriteAsync(store => store.Actions.Add(action), cancellationToken);
    }
    /// <inheritdoc />
    public Task AddRoutineAsync(Routine routine, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(routine); routine.Validate();
        return WriteAsync(store => store.Routines.Add(routine), cancellationToken);
    }
    /// <inheritdoc />
    public Task AddProgressEntryAsync(ProgressEntry entry, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entry); entry.Validate();
        return WriteAsync(store => store.ProgressEntries.Add(entry), cancellationToken);
    }

    /// <inheritdoc />
    public Task SetRoutineStatusAsync(Guid id, RoutineStatus status, CancellationToken cancellationToken = default) =>
        WriteAsync(store =>
        {
            int index = store.Routines.FindIndex(routine => routine.Id == id);
            if (index < 0) throw new ArgumentException("The selected routine does not exist.");
            // Apply only status to the latest record, preserving provenance and history.
            store.Routines[index] = store.Routines[index].WithStatus(status);
        }, cancellationToken);

    private async Task WriteAsync(Action<HealthActionStore> append, CancellationToken cancellationToken)
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
        string backupPath = Path.Combine(directory, "health-actions.backup.json");
        RejectLink(backupPath);
        if (Directory.Exists(backupPath)) throw new InvalidDataException("Action backup is unavailable; existing files retained.");
        if (File.Exists(backupPath)) await LoadStoreAsync(backupPath, cancellationToken).ConfigureAwait(false);
        append(store);
        // Direct repository clients must also preserve IDs and parent references.
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
    private Task<HealthActionStore> LoadPrimaryAsync(CancellationToken cancellationToken)
    {
        string backupPath = Path.Combine(Path.GetDirectoryName(_path)!, "health-actions.backup.json");
        RejectLink(_path); RejectLink(backupPath);
        if (!File.Exists(_path) && (File.Exists(backupPath) || Directory.Exists(backupPath) || Directory.Exists(_path)))
            throw new InvalidDataException("Action store unavailable. Existing files retained; recovery is required.");
        return LoadStoreAsync(_path, cancellationToken);
    }
    private static async Task<HealthActionStore> LoadStoreAsync(string path, CancellationToken cancellationToken)
    {
        RejectLink(path);
        if (!File.Exists(path)) return new HealthActionStore { Store = StoreKind, Version = 1, Actions = new(), Routines = new(), ProgressEntries = new() };
        try
        {
            await using var stream = File.OpenRead(path);
            var store = await JsonSerializer.DeserializeAsync<HealthActionStore>(stream, Options, cancellationToken).ConfigureAwait(false);
            if (store is null || store.Store != StoreKind || store.Version != 1)
                throw new InvalidDataException("The action store is not a supported HealthNotebook store.");
            ValidateStore(store);
            return store;
        }
        // Serializer diagnostics can contain action text; do not propagate them or log them.
        catch (JsonException) { throw new InvalidDataException("Cannot read action store. Existing data retained."); }
        catch (ArgumentException) { throw new InvalidDataException("Invalid action metadata. Existing data retained."); }
    }
    private static void ValidateStore(HealthActionStore store)
    {
        if (store.Actions is null || store.Routines is null || store.ProgressEntries is null)
            throw new InvalidDataException("Invalid action store collections.");
        foreach (var action in store.Actions)
        {
            if (action is null) throw new InvalidDataException("Invalid action record.");
            action.Validate();
        }
        foreach (var routine in store.Routines)
        {
            if (routine is null) throw new InvalidDataException("Invalid routine record.");
            routine.Validate();
        }
        foreach (var entry in store.ProgressEntries)
        {
            if (entry is null) throw new InvalidDataException("Invalid entry record.");
            entry.Validate();
        }
        var actionIds = store.Actions.Select(action => action.Id).ToHashSet();
        var routineIds = store.Routines.Select(routine => routine.Id).ToHashSet();
        if (actionIds.Count != store.Actions.Count || routineIds.Count != store.Routines.Count
            || store.ProgressEntries.Select(entry => entry.Id).Distinct().Count() != store.ProgressEntries.Count
            || store.Routines.Any(routine => !actionIds.Contains(routine.HealthActionId))
            || store.ProgressEntries.Any(entry => !routineIds.Contains(entry.RoutineId)))
            throw new InvalidDataException("Inconsistent action references or duplicate identifiers. Existing data retained.");
    }
    private static void RejectLink(string path)
    {
        for (string? current = Path.GetFullPath(path); current is not null; current = Path.GetDirectoryName(current))
            if ((File.Exists(current) || Directory.Exists(current)) && (File.GetAttributes(current) & FileAttributes.ReparsePoint) != 0)
                throw new IOException("Action storage must not traverse filesystem links.");
    }
    private sealed class HealthActionStore
    {
        public required string Store { get; init; }
        public required int Version { get; init; }
        public required List<HealthAction> Actions { get; init; }
        public required List<Routine> Routines { get; init; }
        public required List<ProgressEntry> ProgressEntries { get; init; }
    }
}
