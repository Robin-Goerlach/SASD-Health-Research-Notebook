using System.Text.Json;
using System.Text.Json.Serialization;
using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Repositories;
using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.Infrastructure.Storage;

/// <summary>One owned session/child store with atomic updates and a last-good backup.</summary>
public sealed class JsonSessionRepository : ISessionRepository
{
    private const string StoreKind = "SASD.HealthNotebook.Sessions";
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true, UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        Converters = { new JsonStringEnumConverter(allowIntegerValues: false) }
    };
    private readonly string _path;
    /// <summary>Resolves the same shared data directory as topics and entries.</summary>
    public JsonSessionRepository() => _path = LocalHealthNotebookPaths.SessionsFilePath;
    /// <inheritdoc />
    public async Task<SessionNotebook> LoadAsync(CancellationToken cancellationToken = default)
    {
        var store = await LoadPrimaryAsync(cancellationToken).ConfigureAwait(false);
        return new SessionNotebook(store.Sessions.AsReadOnly(), store.Questions.AsReadOnly(), store.FollowUps.AsReadOnly());
    }
    /// <inheritdoc />
    public Task AddSessionAsync(Session session, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(session); session.Validate();
        return WriteAsync(store => { store.Sessions.Add(session); return true; }, cancellationToken);
    }
    /// <inheritdoc />
    public Task AddQuestionAsync(SessionQuestion question, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(question); question.Validate();
        return WriteAsync(store => { store.Questions.Add(question); return true; }, cancellationToken);
    }
    /// <inheritdoc />
    public Task AddFollowUpAsync(SessionFollowUp followUp, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(followUp); followUp.Validate();
        return WriteAsync(store => { store.FollowUps.Add(followUp); return true; }, cancellationToken);
    }

    /// <inheritdoc />
    public Task SetQuestionAnswerAsync(Guid id, bool answered, string? note, CancellationToken cancellationToken = default) =>
        WriteAsync(store =>
        {
            int index = store.Questions.FindIndex(question => question.Id == id);
            if (index < 0) throw new ArgumentException("The selected question does not exist.");
            // Apply only answer fields to the latest stored record under the lock.
            if (store.Questions[index].IsAnswered == answered && store.Questions[index].AnswerNote == note) return false;
            store.Questions[index] = store.Questions[index].WithAnswer(answered, note); return true;
        }, cancellationToken);

    /// <inheritdoc />
    public Task SetFollowUpStatusAsync(Guid id, SessionFollowUpStatus status, CancellationToken cancellationToken = default) =>
        WriteAsync(store =>
        {
            int index = store.FollowUps.FindIndex(followUp => followUp.Id == id);
            if (index < 0) throw new ArgumentException("The selected follow-up does not exist.");
            if (store.FollowUps[index].Status == status) return false;
            store.FollowUps[index] = store.FollowUps[index].WithStatus(status); return true;
        }, cancellationToken);

    private async Task WriteAsync(Func<SessionStore, bool> append, CancellationToken cancellationToken)
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
        string backupPath = Path.Combine(directory, "sessions.backup.json");
        RejectLink(backupPath);
        if (Directory.Exists(backupPath)) throw new InvalidDataException("Session backup is unavailable; existing files retained.");
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
    private Task<SessionStore> LoadPrimaryAsync(CancellationToken cancellationToken)
    {
        string backupPath = Path.Combine(Path.GetDirectoryName(_path)!, "sessions.backup.json");
        RejectLink(_path); RejectLink(backupPath);
        if (!File.Exists(_path) && (File.Exists(backupPath) || Directory.Exists(backupPath) || Directory.Exists(_path)))
            throw new InvalidDataException("Session store unavailable. Existing files retained; recovery is required.");
        return LoadStoreAsync(_path, cancellationToken);
    }
    private static async Task<SessionStore> LoadStoreAsync(string path, CancellationToken cancellationToken)
    {
        RejectLink(path);
        if (!File.Exists(path)) return new SessionStore { Store = StoreKind, Version = 1, Sessions = new(), Questions = new(), FollowUps = new() };
        try
        {
            await using var stream = File.OpenRead(path);
            var store = await JsonSerializer.DeserializeAsync<SessionStore>(stream, Options, cancellationToken).ConfigureAwait(false);
            if (store is null || store.Store != StoreKind || store.Version is not (1 or 2))
                throw new InvalidDataException("The session store is not a supported HealthNotebook store.");
            ValidateStore(store);
            return store;
        }
        // Serializer diagnostics can contain session text; do not propagate them or log them.
        catch (JsonException) { throw new InvalidDataException("Cannot read session store. Existing data retained."); }
        catch (ArgumentException) { throw new InvalidDataException("Invalid session metadata. Existing data retained."); }
    }
    private static void ValidateStore(SessionStore store)
    {
        if (store.Sessions is null || store.Questions is null || store.FollowUps is null)
            throw new InvalidDataException("Invalid session store collections.");
        foreach (var session in store.Sessions)
        {
            if (session is null) throw new InvalidDataException("Invalid session record.");
            session.Validate();
        }
        foreach (var question in store.Questions)
        {
            if (question is null) throw new InvalidDataException("Invalid question record.");
            question.Validate();
        }
        foreach (var followUp in store.FollowUps)
        {
            if (followUp is null) throw new InvalidDataException("Invalid followUp record.");
            followUp.Validate();
        }
        var sessionIds = store.Sessions.Select(session => session.Id).ToHashSet();
        var questionIds = store.Questions.Select(question => question.Id).ToHashSet();
        if (sessionIds.Count != store.Sessions.Count || questionIds.Count != store.Questions.Count
            || store.FollowUps.Select(followUp => followUp.Id).Distinct().Count() != store.FollowUps.Count
            || store.Questions.Any(question => !sessionIds.Contains(question.SessionId))
            || store.FollowUps.Any(followUp => !sessionIds.Contains(followUp.SessionId)))
            throw new InvalidDataException("Inconsistent session references or duplicate identifiers. Existing data retained.");
    }
    private static void RejectLink(string path)
    {
        for (string? current = Path.GetFullPath(path); current is not null; current = Path.GetDirectoryName(current))
            if ((File.Exists(current) || Directory.Exists(current)) && (File.GetAttributes(current) & FileAttributes.ReparsePoint) != 0)
                throw new IOException("Session storage must not traverse filesystem links.");
    }
    private sealed class SessionStore
    {
        public required string Store { get; init; }
        public required int Version { get; set; }
        public required List<Session> Sessions { get; init; }
        public required List<SessionQuestion> Questions { get; init; }
        public required List<SessionFollowUp> FollowUps { get; init; }
    }

    /// <inheritdoc />
    public Task UpdateSessionAsync(Session replacement, DateTimeOffset expectedModifiedAt, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(replacement); replacement.Validate();
        return WriteAsync(store =>
        {
            int index = store.Sessions.FindIndex(item => item.Id == replacement.Id);
            if (index < 0) throw new LifecycleConflictException();
            var current = store.Sessions[index];
            if (current.ModifiedAt != expectedModifiedAt) throw new LifecycleConflictException();
            var changed = replacement with { IsArchived = current.IsArchived, CreatedAt = current.CreatedAt, ModifiedAt = current.ModifiedAt };
            if (changed == current && changed.ScheduledAt.EqualsExact(current.ScheduledAt)) return false;
            changed = changed with { ModifiedAt = NextTime(current.ModifiedAt) };
            changed.Validate();
            store.Sessions[index] = changed; return true;
        }, cancellationToken);
    }


    /// <inheritdoc />
    public Task SetSessionArchivedAsync(Guid id, bool archived, DateTimeOffset expectedModifiedAt, CancellationToken cancellationToken = default) =>
        WriteAsync(store =>
        {
            int index = store.Sessions.FindIndex(item => item.Id == id);
            if (index < 0 || store.Sessions[index].ModifiedAt != expectedModifiedAt) throw new LifecycleConflictException();
            var current = store.Sessions[index]; if (current.IsArchived == archived) return false;
            store.Sessions[index] = current with { IsArchived = archived, ModifiedAt = NextTime(current.ModifiedAt) }; return true;
        }, cancellationToken);
    private static DateTimeOffset NextTime(DateTimeOffset previous)
    {
        var now = DateTimeOffset.Now;
        return now > previous ? now : previous.AddTicks(1);
    }

}
