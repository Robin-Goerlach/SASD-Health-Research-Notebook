using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.WinForms.Views;

namespace Sasd.HealthNotebook.WinForms.Presentation;

/// <summary>Loads session snapshots and rejects stale asynchronous selections.</summary>
public sealed class SessionsPresenter
{
    private readonly SessionService _service;
    private readonly SessionsView _view;
    private int _generation;
    /// <summary>Connects the shared session use cases to the WinForms view.</summary>
    public SessionsPresenter(SessionService service, SessionsView view)
    { _service = service ?? throw new ArgumentNullException(nameof(service)); _view = view ?? throw new ArgumentNullException(nameof(view)); }
    /// <summary>Refreshes sessions and their selected children.</summary>
    public async Task<int> LoadAsync(Guid? preferredId = null)
    {
        var sessions = await _service.GetSessionsAsync();
        if (_view.IsDisposed) return sessions.Count;
        _view.SetSessions(sessions, preferredId); await LoadSelectionAsync(); return sessions.Count;
    }
    /// <summary>Clears stale children and rejects responses for obsolete selections.</summary>
    public async Task LoadSelectionAsync()
    {
        int generation = ++_generation; var session = _view.SelectedSession;
        Guid? questionId = _view.SelectedQuestion?.Id, followUpId = _view.SelectedFollowUp?.Id;
        _view.SetDependents(Array.Empty<SessionQuestion>(), Array.Empty<SessionFollowUp>());
        if (session is null) return;
        var details = await _service.GetSessionDetailsAsync(session.Id);
        if (!_view.IsDisposed && generation == _generation && _view.SelectedSession?.Id == session.Id)
            _view.SetDependents(details.Questions, details.FollowUps, questionId, followUpId);
    }
}
