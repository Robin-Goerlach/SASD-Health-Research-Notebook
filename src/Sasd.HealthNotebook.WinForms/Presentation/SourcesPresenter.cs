using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.WinForms.Views;

namespace Sasd.HealthNotebook.WinForms.Presentation;

/// <summary>Loads the source workspace and guards against stale asynchronous selection results.</summary>
public sealed class SourcesPresenter
{
    private readonly SourceService _service;
    private readonly SourcesView _view;
    private int _selectionGeneration;
    /// <summary>Connects the shared service to the view.</summary>
    public SourcesPresenter(SourceService service, SourcesView view)
    { _service = service ?? throw new ArgumentNullException(nameof(service)); _view = view ?? throw new ArgumentNullException(nameof(view)); }
    /// <summary>Loads sources, preserving or explicitly selecting a source, then its dependents.</summary>
    public async Task<int> LoadAsync(Guid? preferredId = null, CancellationToken cancellationToken = default)
    {
        var sources = await _service.GetSourcesAsync(cancellationToken).ConfigureAwait(true);
        if (_view.IsDisposed) return sources.Count;
        _view.SetSources(sources, preferredId);
        await LoadSelectionAsync(cancellationToken).ConfigureAwait(true);
        return sources.Count;
    }
    /// <summary>Clears stale detail rows immediately, then loads the selected source's records.</summary>
    public async Task LoadSelectionAsync(CancellationToken cancellationToken = default)
    {
        int generation = ++_selectionGeneration;
        Source? source = _view.SelectedSource;
        _view.SetDependents(Array.Empty<SourceLocation>(), Array.Empty<EvidenceNote>());
        if (source is null) return;
        var details = await _service.GetSourceDetailsAsync(source.Id, cancellationToken).ConfigureAwait(true);
        if (!_view.IsDisposed && generation == _selectionGeneration && _view.SelectedSource?.Id == source.Id)
            _view.SetDependents(details.Locations, details.Notes);
    }
}
