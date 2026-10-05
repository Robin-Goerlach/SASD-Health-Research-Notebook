using Sasd.HealthNotebook.Application.Contracts;
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
    private int _loadGeneration;
    /// <summary>Connects the shared session use cases to the WinForms view.</summary>
    public SessionsPresenter(SessionService service, SessionsView view)
    { _service = service ?? throw new ArgumentNullException(nameof(service)); _view = view ?? throw new ArgumentNullException(nameof(view)); }
    /// <summary>Refreshes sessions and their selected children.</summary>
    public async Task<int> LoadAsync(Guid? preferredId = null)
    {
        int loadGeneration = ++_loadGeneration;
        // A list refresh supersedes an in-flight child read immediately, before the
        // replacement parent list arrives; otherwise old children could reappear.
        ++_generation;
        var sessions = (await _service.GetSessionsAsync()).Where(item => _view.IncludeArchived || !item.Session.IsArchived).ToList();
        if (_view.IsDisposed || loadGeneration != _loadGeneration) return sessions.Count;
        _view.SetSessions(sessions, preferredId); await LoadSelectionAsync(); return sessions.Count;
    }
    /// <summary>Clears stale children and rejects responses for obsolete selections.</summary>
    public async Task LoadSelectionAsync(Guid? preferredFollowUpId = null, bool openFollowUps = false, bool requireLiveTarget = false)
    {
        int generation = ++_generation; var session = _view.SelectedSession;
        Guid? questionId = _view.SelectedQuestion?.Id, followUpId = preferredFollowUpId ?? _view.SelectedFollowUp?.Id;
        _view.SetDependents(Array.Empty<SessionQuestion>(), Array.Empty<SessionFollowUp>());
        if (session is null) return;
        var details = await _service.GetSessionDetailsAsync(session.Id);
        if (!_view.IsDisposed && generation == _generation && _view.SelectedSession?.Id == session.Id)
        {
            // Check the final coherent detail snapshot before binding: a target can
            // disappear or be archived between list and detail reads. Binding first
            // would silently select the first surviving child before showing a conflict.
            if (requireLiveTarget && !details.Sessions.Any(item => item.Id == session.Id && !item.IsArchived))
                throw new LifecycleConflictException();
            if (openFollowUps && !details.FollowUps.Any(item => item.Id == preferredFollowUpId))
                throw new LifecycleConflictException();
            _view.SetDependents(details.Questions, details.FollowUps, questionId, followUpId);
            if (openFollowUps)
            {
                _view.OpenFollowUps();
            }
        }
    }

    /// <summary>
    /// Opens a specific live parent/child, using awaited reads and explicit selection.
    /// No delay/timer guessing: a missing or archived target is a safe conflict.
    /// </summary>
    public async Task NavigateAsync(Guid sessionId, Guid? followUpId = null)
    {
        int loadGeneration = ++_loadGeneration;
        ++_generation;
        var sessions = await _service.GetSessionsAsync();
        if (_view.IsDisposed || loadGeneration != _loadGeneration) return;
        if (!sessions.Any(item => item.Session.Id == sessionId && !item.Session.IsArchived))
            throw new LifecycleConflictException();
        _view.SetSessions(sessions.Where(item => _view.IncludeArchived || !item.Session.IsArchived).ToList(), sessionId);
        await LoadSelectionAsync(followUpId, followUpId.HasValue, requireLiveTarget: true);
    }
}
