using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.Application.Repositories;

/// <summary>
/// Abstraction for storing and loading health topics.
///
/// The application layer depends on this interface instead of knowing whether the
/// underlying storage is JSON, SQLite or something else. This keeps the first JSON
/// implementation replaceable in a later milestone.
/// </summary>
public interface IHealthTopicRepository
{
    /// <summary>
    /// Loads all health topics from the configured storage.
    /// </summary>
    Task<IReadOnlyList<HealthTopic>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds one health topic and persists the changed collection.
    /// </summary>
    Task AddAsync(HealthTopic topic, CancellationToken cancellationToken = default);

    /// <summary>
    /// Initializes an empty topic store. Existing collections must use the explicit
    /// lifecycle methods so bulk replacement cannot bypass reference protection.
    /// </summary>
    Task SaveAllAsync(IReadOnlyList<HealthTopic> topics, CancellationToken cancellationToken = default);

    /// <summary>Updates one topic, rejecting stale editors.</summary>
    Task UpdateAsync(HealthTopic replacement, DateTimeOffset expectedModifiedAt, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    /// <summary>Archives/reactivates a topic without changing references.</summary>
    Task SetArchivedAsync(Guid id, bool archived, DateTimeOffset expectedModifiedAt, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    /// <summary>Deletes only when every current and historical topic reference is absent.</summary>
    Task DeleteIfUnreferencedAsync(Guid id, DateTimeOffset expectedModifiedAt, CancellationToken cancellationToken = default) => throw new NotSupportedException();

}
