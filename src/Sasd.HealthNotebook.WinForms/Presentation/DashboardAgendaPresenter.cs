using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.WinForms.Views;

namespace Sasd.HealthNotebook.WinForms.Presentation;

/// <summary>Loads agenda projections and prevents older responses/errors from replacing newer UI.</summary>
public sealed class DashboardAgendaPresenter
{
    private readonly SessionService _service;
    private readonly DashboardAgendaView _view;
    private int _generation;
    /// <summary>Connects the existing session use cases to the read-only agenda view.</summary>
    public DashboardAgendaPresenter(SessionService service, DashboardAgendaView view)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _view = view ?? throw new ArgumentNullException(nameof(view));
    }
    /// <summary>Invalidates pending work when navigating away; no timer or persistent cache.</summary>
    public void Invalidate() => ++_generation;
    /// <summary>Loads with a caller-supplied local time; only the latest response may publish.</summary>
    public async Task LoadAsync(DateTimeOffset now)
    {
        int generation = ++_generation;
        _view.SetLoading();
        try
        {
            var agenda = await _service.GetDashboardAgendaAsync(now);
            if (!_view.IsDisposed && generation == _generation) _view.SetAgenda(agenda);
        }
        catch
        {
            // An obsolete failure is as stale as obsolete data. Current failures keep
            // a visible error state and propagate to the shell's existing safe handler.
            if (_view.IsDisposed || generation != _generation) return;
            _view.SetFailed(); throw;
        }
    }
}
