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

    /// <summary>Updates an action and any required professional revision atomically.</summary>
    Task UpdateActionAsync(HealthAction replacement, DateTimeOffset expectedModifiedAt, string? changeReason = null, CancellationToken cancellationToken = default);
    /// <summary>Changes archive state only; retains routines, progress and revisions.</summary>
    Task SetActionArchivedAsync(Guid id, bool archived, DateTimeOffset expectedModifiedAt, CancellationToken cancellationToken = default);
    /// <summary>Edits a routine without moving it to another action.</summary>
    Task UpdateRoutineAsync(Routine replacement, DateTimeOffset expectedModifiedAt, CancellationToken cancellationToken = default);
    /// <summary>Corrects one execution without moving it to another routine.</summary>
    Task UpdateProgressEntryAsync(ProgressEntry replacement, DateTimeOffset expectedModifiedAt, CancellationToken cancellationToken = default);
    /// <summary>Deletes only the selected execution; never its parents or siblings.</summary>
    Task DeleteProgressEntryAsync(Guid id, DateTimeOffset expectedModifiedAt, CancellationToken cancellationToken = default);

}
