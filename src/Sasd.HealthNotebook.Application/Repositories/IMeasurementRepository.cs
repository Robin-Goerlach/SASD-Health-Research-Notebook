using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.Application.Repositories;

/// <summary>Storage contract for documented measurements and explicit corrections/deletions.</summary>
public interface IMeasurementRepository
{
    /// <summary>Loads all numeric measurements without interpreting them.</summary>
    Task<IReadOnlyList<Measurement>> GetAllAsync(CancellationToken cancellationToken = default);
    /// <summary>Adds one measurement, preserving existing data.</summary>
    Task AddAsync(Measurement measurement, CancellationToken cancellationToken = default);

    /// <summary>Replaces editable fields under the writer lock, preserving identity and creation time.</summary>
    Task UpdateAsync(Measurement replacement, DateTimeOffset expectedModifiedAt, CancellationToken cancellationToken = default);
    /// <summary>Deletes one childless record under the writer lock; rejects stale selection.</summary>
    Task DeleteAsync(Guid id, DateTimeOffset expectedModifiedAt, CancellationToken cancellationToken = default);

}
