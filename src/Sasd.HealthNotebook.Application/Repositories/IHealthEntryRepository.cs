using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.Application.Repositories;

/// <summary>Storage contract for create-only notebook entries.</summary>
public interface IHealthEntryRepository
{
    /// <summary>Loads all entries without interpreting their content.</summary>
    Task<IReadOnlyList<HealthEntry>> GetAllAsync(CancellationToken cancellationToken = default);
    /// <summary>Adds an entry without replacing existing entries.</summary>
    Task AddAsync(HealthEntry entry, CancellationToken cancellationToken = default);
}
