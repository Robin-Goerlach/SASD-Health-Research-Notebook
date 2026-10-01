using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.Application.Repositories;

/// <summary>Create-only storage of a source notebook; writes preserve internal reference integrity.</summary>
public interface ISourceRepository
{
    /// <summary>Loads a coherent snapshot.</summary>
    Task<SourceNotebook> LoadAsync(CancellationToken cancellationToken = default);
    /// <summary>Adds a source, retaining all existing records.</summary>
    Task AddSourceAsync(Source source, CancellationToken cancellationToken = default);
    /// <summary>Adds a location only to an existing source.</summary>
    Task AddLocationAsync(SourceLocation location, CancellationToken cancellationToken = default);
    /// <summary>Adds a note only with consistent source/locator references.</summary>
    Task AddNoteAsync(EvidenceNote note, CancellationToken cancellationToken = default);
}
