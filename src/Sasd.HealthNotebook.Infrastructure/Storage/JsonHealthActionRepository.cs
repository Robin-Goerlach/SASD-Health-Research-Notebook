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
        return new HealthActionNotebook(store.Actions.AsReadOnly(), store.Routines.AsReadOnly(), store.ProgressEntries.AsReadOnly()) { Revisions = store.Revisions!.AsReadOnly() };
    }
    /// <inheritdoc />
    public Task AddActionAsync(HealthAction action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action); action.Validate();
        return WriteAsync(store => { store.Actions.Add(action); return true; }, cancellationToken);
    }
    /// <inheritdoc />
    public Task AddRoutineAsync(Routine routine, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(routine); routine.Validate();
        return WriteAsync(store => { store.Routines.Add(routine); return true; }, cancellationToken);
    }
    /// <inheritdoc />
    public Task AddProgressEntryAsync(ProgressEntry entry, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entry); entry.Validate();
        return WriteAsync(store => { store.ProgressEntries.Add(entry); return true; }, cancellationToken);
    }

    /// <inheritdoc />
    public Task SetRoutineStatusAsync(Guid id, RoutineStatus status, CancellationToken cancellationToken = default) =>
        WriteAsync(store =>
        {
            int index = store.Routines.FindIndex(routine => routine.Id == id);
            if (index < 0) throw new ArgumentException("The selected routine does not exist.");
            // Apply only status to the latest record, preserving provenance and history.
            if (store.Routines[index].Status == status) return false;
            store.Routines[index] = store.Routines[index].WithStatus(status); return true;
        }, cancellationToken);

    private async Task WriteAsync(Func<HealthActionStore, bool> append, CancellationToken cancellationToken)
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
        if (!append(store)) return;
        store.Version = 2;
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
        if (!File.Exists(path)) return new HealthActionStore { Store = StoreKind, Version = 1, Actions = new(), Routines = new(), ProgressEntries = new(), Revisions = new() };
        try
        {
            await using var stream = File.OpenRead(path);
            var store = await JsonSerializer.DeserializeAsync<HealthActionStore>(stream, Options, cancellationToken).ConfigureAwait(false);
            if (store is null || store.Store != StoreKind || store.Version is not (1 or 2))
                throw new InvalidDataException("The action store is not a supported HealthNotebook store.");
            if (store.Version == 1) store.Revisions ??= new();
            ValidateStore(store);
            return store;
        }
        // Serializer diagnostics can contain action text; do not propagate them or log them.
        catch (JsonException) { throw new InvalidDataException("Cannot read action store. Existing data retained."); }
        catch (ArgumentException) { throw new InvalidDataException("Invalid action metadata. Existing data retained."); }
    }
    private static void ValidateStore(HealthActionStore store)
    {
        if (store.Actions is null || store.Routines is null || store.ProgressEntries is null || store.Revisions is null)
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
        foreach (var revision in store.Revisions)
        {
            if (revision is null) throw new InvalidDataException("Invalid revision record.");
            revision.Validate();
            if (!store.Actions.Any(action => action.Id == revision.HealthActionId && action.ModifiedAt >= revision.ChangedAt))
                throw new InvalidDataException("Invalid revision parent or chronology.");
        }
        if (store.Revisions.Select(item => item.Id).Distinct().Count() != store.Revisions.Count)
            throw new InvalidDataException("Duplicate revision identifiers.");
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
        public required int Version { get; set; }
        public required List<HealthAction> Actions { get; init; }
        public required List<Routine> Routines { get; init; }
        public required List<ProgressEntry> ProgressEntries { get; init; }
        public List<HealthActionRevision>? Revisions { get; set; }
    }

    /// <inheritdoc />
    public Task UpdateActionAsync(HealthAction replacement, DateTimeOffset expectedModifiedAt, string? changeReason = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(replacement); replacement.Validate();
        return WriteAsync(store =>
        {
            int index = store.Actions.FindIndex(item => item.Id == replacement.Id);
            if (index < 0) throw new LifecycleConflictException();
            var current = store.Actions[index];
            if (current.ModifiedAt != expectedModifiedAt) throw new LifecycleConflictException();
            var changed = replacement with { IsArchived = current.IsArchived, CreatedAt = current.CreatedAt, ModifiedAt = current.ModifiedAt };
            if (changed == current) return false;
            changed = changed with { ModifiedAt = NextTime(current.ModifiedAt) };
            changed.Validate();
            if (changeReason?.Length > HealthAction.MaximumTextLength) throw new ArgumentException("Change reason too long.");
            // Auditing occurs under the same lock and replacement as the corrected instruction.
            // Entering or leaving professional provenance cannot erase the previous content.
            if (HasProtectedProvenance(current) || HasProtectedProvenance(changed))
                store.Revisions!.Add(new HealthActionRevision { Id = Guid.NewGuid(), HealthActionId = current.Id,
                    ChangedAt = changed.ModifiedAt, Previous = current, ChangeReason = changeReason });
            store.Actions[index] = changed; return true;
        }, cancellationToken);
    }


    /// <inheritdoc />
    public Task UpdateRoutineAsync(Routine replacement, DateTimeOffset expectedModifiedAt, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(replacement); replacement.Validate();
        return WriteAsync(store =>
        {
            int index = store.Routines.FindIndex(item => item.Id == replacement.Id);
            if (index < 0) throw new LifecycleConflictException();
            var current = store.Routines[index];
            if (current.ModifiedAt != expectedModifiedAt) throw new LifecycleConflictException();
            if (replacement.HealthActionId != current.HealthActionId) throw new ArgumentException("Parent cannot change.");
            var changed = replacement with { CreatedAt = current.CreatedAt, ModifiedAt = current.ModifiedAt };
            if (changed == current) return false;
            changed = changed with { ModifiedAt = NextTime(current.ModifiedAt) };
            changed.Validate();
            store.Routines[index] = changed; return true;
        }, cancellationToken);
    }


    /// <inheritdoc />
    public Task UpdateProgressEntryAsync(ProgressEntry replacement, DateTimeOffset expectedModifiedAt, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(replacement); replacement.Validate();
        return WriteAsync(store =>
        {
            int index = store.ProgressEntries.FindIndex(item => item.Id == replacement.Id);
            if (index < 0) throw new LifecycleConflictException();
            var current = store.ProgressEntries[index];
            if (current.ModifiedAt != expectedModifiedAt) throw new LifecycleConflictException();
            if (replacement.RoutineId != current.RoutineId) throw new ArgumentException("Parent cannot change.");
            var changed = replacement with { CreatedAt = current.CreatedAt, ModifiedAt = current.ModifiedAt };
            if (changed == current && changed.OccurredAt.EqualsExact(current.OccurredAt)) return false;
            changed = changed with { ModifiedAt = NextTime(current.ModifiedAt) };
            changed.Validate();
            store.ProgressEntries[index] = changed; return true;
        }, cancellationToken);
    }


    /// <inheritdoc />
    public Task SetActionArchivedAsync(Guid id, bool archived, DateTimeOffset expectedModifiedAt, CancellationToken cancellationToken = default) =>
        WriteAsync(store =>
        {
            int index = store.Actions.FindIndex(item => item.Id == id);
            if (index < 0 || store.Actions[index].ModifiedAt != expectedModifiedAt) throw new LifecycleConflictException();
            var current = store.Actions[index]; if (current.IsArchived == archived) return false;
            store.Actions[index] = current with { IsArchived = archived, ModifiedAt = NextTime(current.ModifiedAt) }; return true;
        }, cancellationToken);
    private static DateTimeOffset NextTime(DateTimeOffset previous)
    {
        var now = DateTimeOffset.Now;
        return now > previous ? now : previous.AddTicks(1);
    }


    // Sources can document professional statements. Retain all source-backed corrections
    // conservatively, including missing sources, without cross-store races or content inference.
    private static bool HasProtectedProvenance(HealthAction action) => action.SourceId.HasValue
        || action.Origin is HealthActionOrigin.Doctor or HealthActionOrigin.Therapist or HealthActionOrigin.Coach or HealthActionOrigin.Source;
    /// <inheritdoc />
    public Task DeleteProgressEntryAsync(Guid id, DateTimeOffset expectedModifiedAt, CancellationToken cancellationToken = default) =>
        WriteAsync(store =>
        {
            var current = store.ProgressEntries.SingleOrDefault(item => item.Id == id);
            if (current is null || current.ModifiedAt != expectedModifiedAt) throw new LifecycleConflictException();
            store.ProgressEntries.Remove(current); return true;
        }, cancellationToken);

}
