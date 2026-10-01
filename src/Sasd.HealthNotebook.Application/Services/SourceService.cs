using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Repositories;
using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.Application.Services;

/// <summary>Source documentation use cases; no fetching, trust scoring or medical interpretation.</summary>
public sealed class SourceService
{
    private readonly ISourceRepository _sources;
    private readonly IHealthTopicRepository _topics;
    /// <summary>Uses UI-independent shared repository contracts.</summary>
    public SourceService(ISourceRepository sources, IHealthTopicRepository topics)
    {
        _sources = sources ?? throw new ArgumentNullException(nameof(sources));
        _topics = topics ?? throw new ArgumentNullException(nameof(topics));
    }
    /// <summary>Creates a source with an optional validated topic reference.</summary>
    public async Task<Source> CreateSourceAsync(CreateSourceRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var source = Source.Create(request.SourceType, request.Title, request.Url, request.AuthorOrInstitution,
            request.PublicationDate, request.AccessedAt, request.ExternalIdentifier, request.HealthTopicId);
        if (source.HealthTopicId.HasValue && !(await _topics.GetAllAsync(cancellationToken).ConfigureAwait(false))
            .Any(topic => topic.Id == source.HealthTopicId))
            throw new ArgumentException("The selected topic does not exist.");
        await _sources.AddSourceAsync(source, cancellationToken).ConfigureAwait(false);
        return source;
    }
    /// <summary>Loads one source or returns null when it is absent.</summary>
    public async Task<Source?> GetSourceAsync(Guid id, CancellationToken cancellationToken = default) =>
        (await _sources.LoadAsync(cancellationToken).ConfigureAwait(false)).Sources.SingleOrDefault(source => source.Id == id);
    /// <summary>Lists newest sources first, resolving current topic titles without copying them into persistence.</summary>
    public async Task<IReadOnlyList<SourceSummary>> GetSourcesAsync(CancellationToken cancellationToken = default)
    {
        var store = await _sources.LoadAsync(cancellationToken).ConfigureAwait(false);
        var topics = (await _topics.GetAllAsync(cancellationToken).ConfigureAwait(false)).ToDictionary(topic => topic.Id);
        return store.Sources.OrderByDescending(source => source.CreatedAt).ThenBy(source => source.Id)
            .Select(source => new SourceSummary(source, source.HealthTopicId.HasValue
                && topics.TryGetValue(source.HealthTopicId.Value, out var topic) ? topic.Title : null)).ToList();
    }
    /// <summary>Adds a location to an existing source.</summary>
    public async Task<SourceLocation> CreateLocationAsync(CreateSourceLocationRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var location = SourceLocation.Create(request.SourceId, request.LocationType, request.Locator, request.Note);
        var store = await _sources.LoadAsync(cancellationToken).ConfigureAwait(false);
        RequireSource(store, location.SourceId);
        await _sources.AddLocationAsync(location, cancellationToken).ConfigureAwait(false);
        return location;
    }
    /// <summary>Creates a note only when the optional locator belongs to the chosen source.</summary>
    public async Task<EvidenceNote> CreateNoteAsync(CreateEvidenceNoteRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var note = EvidenceNote.Create(request.SourceId, request.SourceLocationId, request.Statement, request.Excerpt, request.OwnParaphraseOrAssessment);
        var store = await _sources.LoadAsync(cancellationToken).ConfigureAwait(false);
        RequireSource(store, note.SourceId);
        if (note.SourceLocationId.HasValue && !store.Locations.Any(location => location.Id == note.SourceLocationId && location.SourceId == note.SourceId))
            throw new ArgumentException("The selected location does not belong to this source.");
        await _sources.AddNoteAsync(note, cancellationToken).ConfigureAwait(false);
        return note;
    }
    /// <summary>Returns locations for one source in creation order.</summary>
    public async Task<IReadOnlyList<SourceLocation>> GetLocationsAsync(Guid sourceId, CancellationToken cancellationToken = default) =>
        (await _sources.LoadAsync(cancellationToken).ConfigureAwait(false)).Locations.Where(location => location.SourceId == sourceId)
            .OrderBy(location => location.CreatedAt).ThenBy(location => location.Id).ToList();
    /// <summary>Returns newest notes first for one source, preserving separate original/personal fields.</summary>
    public async Task<IReadOnlyList<EvidenceNote>> GetNotesAsync(Guid sourceId, CancellationToken cancellationToken = default) =>
        (await _sources.LoadAsync(cancellationToken).ConfigureAwait(false)).Notes.Where(note => note.SourceId == sourceId)
            .OrderByDescending(note => note.CreatedAt).ThenBy(note => note.Id).ToList();
    private static void RequireSource(SourceNotebook store, Guid sourceId)
    {
        if (!store.Sources.Any(source => source.Id == sourceId)) throw new ArgumentException("The selected source does not exist.");
    }

    /// <summary>Loads a coherent selected-source snapshot so a newly added note cannot outpace its locator in the UI.</summary>
    public async Task<SourceNotebook> GetSourceDetailsAsync(Guid sourceId, CancellationToken cancellationToken = default)
    {
        var store = await _sources.LoadAsync(cancellationToken).ConfigureAwait(false);
        return new SourceNotebook(store.Sources.Where(source => source.Id == sourceId).ToList(),
            store.Locations.Where(location => location.SourceId == sourceId).OrderBy(location => location.CreatedAt).ThenBy(location => location.Id).ToList(),
            store.Notes.Where(note => note.SourceId == sourceId).OrderByDescending(note => note.CreatedAt).ThenBy(note => note.Id).ToList());
    }
}
