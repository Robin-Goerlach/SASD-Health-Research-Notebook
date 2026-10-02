using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.WinForms.Controls;
using Sasd.HealthNotebook.WinForms.Localization;
using Sasd.HealthNotebook.WinForms.Presentation;
using Sasd.HealthNotebook.WinForms.Views;

namespace Sasd.HealthNotebook.WinForms.Forms;

public sealed partial class MainForm
{
    private readonly HealthActionService _healthActionService;
    private HealthActionsView _actionsView = null!;
    private HealthActionsPresenter _actionsPresenter = null!;
    private readonly HealthActionOverviewControl _dashboardActionsOverview = new();
    private void InitializeActions()
    {
        _actionsView = new(); _actionsPresenter = new(_healthActionService, _actionsView);
        _actionsView.ActionSelected += async (_, _) => await WithActionErrorAsync(() => _actionsPresenter.LoadSelectionAsync());
        _actionsView.NewRoutineRequested += async (_, _) => await WithActionErrorAsync(async () =>
        {
            var action = _actionsView.SelectedAction; if (action is null) return;
            using var dialog = new CreateRoutineForm(_healthActionService, action.Id);
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                await _actionsPresenter.LoadSelectionAsync(dialog.CreatedRoutineId);
                _actionsView.SetOverview(await _healthActionService.GetOverviewAsync(DateOnly.FromDateTime(DateTime.Now)));
            }
        });
        _actionsView.RoutineStatusRequested += async (_, _) => await WithActionErrorAsync(async () =>
        {
            var routine = _actionsView.SelectedRoutine; if (routine is null) return;
            await _healthActionService.SetRoutineStatusAsync(routine.Id, routine.Status == RoutineStatus.Active ? RoutineStatus.Paused : RoutineStatus.Active);
            await _actionsPresenter.LoadAsync();
        });
        _actionsView.NewProgressRequested += async (_, _) => await WithActionErrorAsync(async () =>
        {
            var routine = _actionsView.SelectedRoutine; if (routine is null) return;
            using var dialog = new CreateProgressEntryForm(_healthActionService, routine.Id);
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                await _actionsPresenter.LoadSelectionAsync(routine.Id, dialog.CreatedProgressId);
                _actionsView.SetOverview(await _healthActionService.GetOverviewAsync(DateOnly.FromDateTime(DateTime.Now)));
            }
        });
    }
    private async Task ShowCreateActionAsync() => await WithActionErrorAsync(async () =>
    {
        using var dialog = new CreateHealthActionForm(_healthActionService, await _healthTopicService.GetTopicSummariesAsync(),
            await _sourceService.GetSourcesAsync(), await _sessionService.GetSessionsAsync());
        if (dialog.ShowDialog(this) == DialogResult.OK)
            _statusLabel.Text = AppStrings.LoadedActions(await _actionsPresenter.LoadAsync(dialog.CreatedActionId));
    });
    private async Task WithActionErrorAsync(Func<Task> action)
    { try { await action(); } catch { UiErrorHandler.ShowSafeError(this, AppStrings.OperationLoadLocalNotebookData); } }
}
