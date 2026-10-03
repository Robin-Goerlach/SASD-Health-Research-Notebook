using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.Application.Repositories;

/// <summary>Atomic action/routine/history storage with checked parent references.</summary>
public interface IHealthActionRepository
{
    /// <summary>Loads one coherent snapshot.</summary>
    Task<HealthActionNotebook> LoadAsync(CancellationToken cancellationToken = default);
    /// <summary>Adds user-entered action documentation.</summary>
    Task AddActionAsync(HealthAction action, CancellationToken cancellationToken = default);
    /// <summary>Adds a routine to an existing action.</summary>
    Task AddRoutineAsync(Routine routine, CancellationToken cancellationToken = default);
    /// <summary>Appends history to an existing routine.</summary>
    Task AddProgressEntryAsync(ProgressEntry entry, CancellationToken cancellationToken = default);
    /// <summary>Changes only routine status on the latest record under the writer lock.</summary>
    Task SetRoutineStatusAsync(Guid id, RoutineStatus status, CancellationToken cancellationToken = default);
}
