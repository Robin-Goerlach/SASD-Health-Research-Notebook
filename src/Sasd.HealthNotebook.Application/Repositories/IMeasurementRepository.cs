using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.Application.Repositories;

/// <summary>Create-only storage contract for documented measurements.</summary>
public interface IMeasurementRepository
{
    /// <summary>Loads all numeric measurements without interpreting them.</summary>
    Task<IReadOnlyList<Measurement>> GetAllAsync(CancellationToken cancellationToken = default);
    /// <summary>Adds one measurement, preserving existing data.</summary>
    Task AddAsync(Measurement measurement, CancellationToken cancellationToken = default);
}
