using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.WinForms.Localization;
using Sasd.HealthNotebook.WinForms.Presentation;
using Sasd.HealthNotebook.WinForms.Views;

namespace Sasd.HealthNotebook.WinForms.Forms;

public sealed partial class MainForm
{
    private readonly SessionService _sessionService;
    private SessionsView _sessionsView = null!;
    private SessionsPresenter _sessionsPresenter = null!;
    private void InitializeSessions()
    {
        _sessionsView = new SessionsView(); _sessionsPresenter = new SessionsPresenter(_sessionService, _sessionsView);
        _sessionsView.SessionSelected += async (_, _) => await WithSessionErrorAsync(() => _sessionsPresenter.LoadSelectionAsync());
        _sessionsView.NewQuestionRequested += async (_, _) => await ShowSessionQuestionAsync(false);
        _sessionsView.AnswerRequested += async (_, _) => await ShowSessionQuestionAsync(true);
        _sessionsView.NewFollowUpRequested += async (_, _) => await WithSessionErrorAsync(async () =>
        {
            var session = _sessionsView.SelectedSession; if (session is null) return;
            using var dialog = new CreateSessionFollowUpForm(_sessionService, session.Id);
            if (dialog.ShowDialog(this) == DialogResult.OK) await _sessionsPresenter.LoadSelectionAsync();
        });
        _sessionsView.FollowUpStatusRequested += async (_, _) => await WithSessionErrorAsync(async () =>
        {
            var followUp = _sessionsView.SelectedFollowUp; if (followUp is null) return;
            await _sessionService.SetFollowUpStatusAsync(followUp.Id, followUp.Status == SessionFollowUpStatus.Open ? SessionFollowUpStatus.Done : SessionFollowUpStatus.Open);
            await _sessionsPresenter.LoadSelectionAsync();
        });
    }
    private async Task ShowCreateSessionAsync() => await WithSessionErrorAsync(async () =>
    {
        var topics = await _healthTopicService.GetTopicSummariesAsync();
        using var dialog = new CreateSessionForm(_sessionService, topics);
        if (dialog.ShowDialog(this) == DialogResult.OK)
            _statusLabel.Text = AppStrings.FormatLoadedSessions(await _sessionsPresenter.LoadAsync(dialog.CreatedSessionId));
    });
    private async Task ShowSessionQuestionAsync(bool editAnswer) => await WithSessionErrorAsync(async () =>
    {
        var session = _sessionsView.SelectedSession; if (session is null) return;
        var question = editAnswer ? _sessionsView.SelectedQuestion : null; if (editAnswer && question is null) return;
        using var dialog = new SessionQuestionForm(_sessionService, session.Id, question);
        if (dialog.ShowDialog(this) == DialogResult.OK) await _sessionsPresenter.LoadSelectionAsync();
    });
    private async Task WithSessionErrorAsync(Func<Task> action)
    { try { await action(); } catch { UiErrorHandler.ShowSafeError(this, AppStrings.OperationLoadLocalNotebookData); } }
}
