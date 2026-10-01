using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.WinForms.Views;

namespace Sasd.HealthNotebook.WinForms.Presentation;

/// <summary>Coordinates timeline loading through the shared Application service.</summary>
public sealed class TimelinePresenter
{
    private readonly HealthEntryService _service;
    private readonly TimelineView _view;
    /// <summary>Connects service and view without persistence logic in the Form.</summary>
    public TimelinePresenter(HealthEntryService service, TimelineView view)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _view = view ?? throw new ArgumentNullException(nameof(view));
    }
    /// <summary>Loads entries and returns their displayed count.</summary>
    public async Task<int> LoadAsync(CancellationToken cancellationToken = default)
    {
        var entries = await _service.GetEntriesAsync(cancellationToken: cancellationToken).ConfigureAwait(true);
        _view.SetEntries(entries);
        return entries.Count;
    }
}
