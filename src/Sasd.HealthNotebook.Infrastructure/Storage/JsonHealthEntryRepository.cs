using Sasd.HealthNotebook.Application.Contracts;
using System.Text.Json;
using System.Text.Json.Serialization;
using Sasd.HealthNotebook.Application.Repositories;
using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.Infrastructure.Storage;

/// <summary>Separate owned JSON entry store with last-good backup and atomic replacement.</summary>
public sealed class JsonHealthEntryRepository : IHealthEntryRepository
{
    private const string StoreKind = "SASD.HealthNotebook.HealthEntries";
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true, UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        Converters = { new JsonStringEnumConverter(allowIntegerValues: false) }
    };
    private readonly string _path;

    /// <summary>Resolves the shared data directory; no alternate arbitrary filename is accepted.</summary>
    public JsonHealthEntryRepository() => _path = LocalHealthNotebookPaths.HealthEntriesFilePath;

    /// <inheritdoc />
    public async Task<IReadOnlyList<HealthEntry>> GetAllAsync(CancellationToken cancellationToken = default) =>
        (await LoadPrimaryAsync(cancellationToken).ConfigureAwait(false)).Entries;

    /// <inheritdoc />
    public async Task AddAsync(HealthEntry entry, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entry); entry.Validate();
        await WriteAsync(store =>
        {
            if (store.Entries.Any(existing => existing.Id == entry.Id))
                throw new InvalidOperationException("Duplicate identifier.");
            store.Entries.Add(entry); return true;
        }, cancellationToken).ConfigureAwait(false);
    }

    private async Task WriteAsync(Func<EntryStore, bool> mutate, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string directory = Path.GetDirectoryName(_path)!;
        RejectLink(directory);
        Directory.CreateDirectory(directory);
        string lockPath = _path + ".lock";
        // CreateNew provides cross-instance/process exclusion. An abandoned lock is
        // never removed automatically; concurrent writers fail safely instead of losing data.
        await using var writeLock = new FileStream(lockPath, FileMode.CreateNew, FileAccess.Write, FileShare.None,
            4096, FileOptions.DeleteOnClose);
        var store = await LoadPrimaryAsync(cancellationToken).ConfigureAwait(false);
        if (!mutate(store)) return;
        foreach (var item in store.Entries) item.Validate();
        string backupPath = Path.Combine(directory, "health-entries.backup.json");
        if (Directory.Exists(backupPath)) throw new InvalidDataException("Entry backup unavailable; existing files retained.");
        if (File.Exists(backupPath)) await LoadAsync(backupPath, cancellationToken).ConfigureAwait(false);
        RejectLink(backupPath);
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
            // Windows atomic replacement preserves a last-good copy like the topic store.
            if (File.Exists(_path)) File.Replace(temporaryPath, _path, backupPath);
            else File.Move(temporaryPath, _path);
        }
        finally
        {
            // CreateNew and the writer lock establish ownership. A pre-existing
            // temporary file is never overwritten or deleted by this operation.
            if (temporaryCreated && File.Exists(temporaryPath)) File.Delete(temporaryPath);
        }
    }

    private static async Task<EntryStore> LoadAsync(string path, CancellationToken cancellationToken)
    {
        RejectLink(path);
        if (!File.Exists(path)) return new EntryStore { Store = StoreKind, Version = 1, Entries = new() };
        try
        {
            await using var stream = File.OpenRead(path);
            var store = await JsonSerializer.DeserializeAsync<EntryStore>(stream, Options, cancellationToken).ConfigureAwait(false);
            if (store is null || store.Store != StoreKind || store.Version != 1 || store.Entries is null)
                throw new InvalidDataException("The entry store is not a supported HealthNotebook store.");
            foreach (var entry in store.Entries)
            {
                if (entry is null) throw new InvalidDataException("Invalid entry store.");
                entry.Validate();
            }
            if (store.Entries.Select(entry => entry.Id).Distinct().Count() != store.Entries.Count)
                throw new InvalidDataException("Duplicate identifiers in entry store.");
            return store;
        }
        // Do not propagate serializer messages that can include user content.
        catch (JsonException) { throw new InvalidDataException("Cannot read the entry store. Existing data was retained."); }
        catch (ArgumentException) { throw new InvalidDataException("Invalid entry store metadata. Existing data was retained."); }
    }

    private Task<EntryStore> LoadPrimaryAsync(CancellationToken cancellationToken)
    {
        string backupPath = Path.Combine(Path.GetDirectoryName(_path)!, "health-entries.backup.json");
        // A surviving backup indicates an existing store. Never silently start an
        // empty collection when its primary file disappeared.
        if (!File.Exists(_path) && (File.Exists(backupPath) || Directory.Exists(_path)))
            throw new InvalidDataException("Entry store unavailable. Existing files were retained; recovery is required.");
        return LoadAsync(_path, cancellationToken);
    }

    private static void RejectLink(string path)
    {
        for (string? current = Path.GetFullPath(path); current is not null; current = Path.GetDirectoryName(current))
        {
            if ((File.Exists(current) || Directory.Exists(current))
                && (File.GetAttributes(current) & FileAttributes.ReparsePoint) != 0)
                throw new IOException("Entry storage must not traverse filesystem links.");
        }
    }

    private sealed class EntryStore
    {
        // Required metadata prevents unrelated JSON from being treated as an empty entry collection.
        public required string Store { get; init; }
        public required int Version { get; init; }
        public required List<HealthEntry> Entries { get; init; }
    }

    /// <inheritdoc />
    public Task UpdateAsync(HealthEntry replacement, DateTimeOffset expectedModifiedAt, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(replacement); replacement.Validate();
        return WriteAsync(store =>
        {
            int index = store.Entries.FindIndex(item => item.Id == replacement.Id);
            if (index < 0) throw new LifecycleConflictException();
            var current = store.Entries[index];
            if (current.ModifiedAt != expectedModifiedAt) throw new LifecycleConflictException();
            var changed = replacement with { CreatedAt = current.CreatedAt, ModifiedAt = current.ModifiedAt };
            if (changed == current && changed.OccurredAt.EqualsExact(current.OccurredAt)) return false;
            changed = changed with { ModifiedAt = NextTime(current.ModifiedAt) };
            changed.Validate(); store.Entries[index] = changed; return true;
        }, cancellationToken);
    }
    /// <inheritdoc />
    public Task DeleteAsync(Guid id, DateTimeOffset expectedModifiedAt, CancellationToken cancellationToken = default) =>
        WriteAsync(store =>
        {
            var current = store.Entries.SingleOrDefault(item => item.Id == id);
            if (current is null || current.ModifiedAt != expectedModifiedAt) throw new LifecycleConflictException();
            store.Entries.Remove(current); return true;
        }, cancellationToken);
    private static DateTimeOffset NextTime(DateTimeOffset previous) => DateTimeOffset.Now > previous ? DateTimeOffset.Now : previous.AddTicks(1);

}
