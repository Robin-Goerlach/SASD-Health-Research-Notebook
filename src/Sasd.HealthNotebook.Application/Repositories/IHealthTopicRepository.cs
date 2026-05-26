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
    /// Replaces the persisted topic collection.
    /// This method is useful for future import, migration and bulk update scenarios.
    /// </summary>
    Task SaveAllAsync(IReadOnlyList<HealthTopic> topics, CancellationToken cancellationToken = default);
}
