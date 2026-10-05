using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.Application.Repositories;

/// <summary>Storage contract for notebook entries and explicit corrections/deletions.</summary>
public interface IHealthEntryRepository
{
    /// <summary>Loads all entries without interpreting their content.</summary>
    Task<IReadOnlyList<HealthEntry>> GetAllAsync(CancellationToken cancellationToken = default);
    /// <summary>Adds an entry without replacing existing entries.</summary>
    Task AddAsync(HealthEntry entry, CancellationToken cancellationToken = default);

    /// <summary>Replaces editable fields under the writer lock, preserving identity and creation time.</summary>
    Task UpdateAsync(HealthEntry replacement, DateTimeOffset expectedModifiedAt, CancellationToken cancellationToken = default);
    /// <summary>Deletes one childless record under the writer lock; rejects stale selection.</summary>
    Task DeleteAsync(Guid id, DateTimeOffset expectedModifiedAt, CancellationToken cancellationToken = default);

}
