using Sasd.HealthNotebook.WinForms.Controls;
using Sasd.HealthNotebook.WinForms.Localization;
using Sasd.HealthNotebook.WinForms.Views;

namespace Sasd.HealthNotebook.WinForms.Forms;

public sealed partial class MainForm
{
    private void InitializeAgenda()
    {
        _agendaView.TargetRequested += async (_, target) => await OpenAgendaTargetAsync(target);
        _agendaView.ShowAllSessionsRequested += async (_, _) => await ShowCompleteAgendaAsync(true);
        _agendaView.ShowAllFollowUpsRequested += async (_, _) => await ShowCompleteAgendaAsync(false);
    }

    private async Task ShowCompleteAgendaAsync(bool sessions)
    {
        using var dialog = new AgendaListForm(_agendaView.Agenda, sessions);
        if (dialog.ShowDialog(this) == DialogResult.OK && dialog.Target is not null)
            await OpenAgendaTargetAsync(dialog.Target);
    }

    private async Task OpenAgendaTargetAsync(AgendaTargetEventArgs target)
    {
        // Navigation supersedes shell refresh immediately. Presenter APIs perform
        // exact awaited parent/child selection; MainForm contains no agenda rules.
        int generation = ++_reloadGeneration;
        ShowPage(NavigationPage.Sessions);
        try
        {
            await _sessionsPresenter.NavigateAsync(target.SessionId, target.FollowUpId);
            if (!IsDisposed && generation == _reloadGeneration)
                _statusLabel.Text = AppStrings.Ready;
        }
        catch
        {
            if (IsDisposed || generation != _reloadGeneration) return;
            UiErrorHandler.ShowSafeError(this, AppStrings.OperationLoadLocalNotebookData);
            _statusLabel.Text = AppStrings.LoadingFailed;
        }
    }
}
