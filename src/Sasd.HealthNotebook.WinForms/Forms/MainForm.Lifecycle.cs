using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.WinForms.Localization;

namespace Sasd.HealthNotebook.WinForms.Forms;

public sealed partial class MainForm
{
    private void InitializeLifecycle()
    {
        _measurementsView.EditRequested += async (_, _) => await WithLifecycleErrorAsync(async () =>
        {
            var item = _measurementsView.SelectedMeasurement; if (item is null) return;
            using var dialog = new CreateMeasurementForm(_measurementService, await _healthTopicService.GetTopicSummariesAsync(), item);
            if (dialog.ShowDialog(this) == DialogResult.OK) await ReloadSafeAsync();
        });
        _measurementsView.DeleteRequested += async (_, _) => await WithLifecycleErrorAsync(async () =>
        {
            var item = _measurementsView.SelectedMeasurement; if (item is null || !ConfirmDeletion(AppStrings.DeleteMeasurementLabel(item.OccurredAt))) return;
            await _measurementService.DeleteMeasurementAsync(item.Id, item.ModifiedAt); await ReloadSafeAsync();
        });
        _timelineView.EditRequested += async (_, _) => await WithLifecycleErrorAsync(async () =>
        {
            var id = _timelineView.SelectedEntryId; if (!id.HasValue) return;
            var item = await _healthEntryService.GetByIdAsync(id.Value);
            using var dialog = new CreateHealthEntryForm(_healthEntryService, await _healthTopicService.GetTopicSummariesAsync(), item);
            if (dialog.ShowDialog(this) == DialogResult.OK) await ReloadSafeAsync();
        });
        _timelineView.DeleteRequested += async (_, _) => await WithLifecycleErrorAsync(async () =>
        {
            var item = _timelineView.SelectedEntry; if (item is null) return;
            if (!ConfirmDeletion(AppStrings.DeleteEntryLabel(item.Title))) return;
            await _healthEntryService.DeleteHealthEntryAsync(item.Id, item.ModifiedAt); await ReloadSafeAsync();
        });
        _sessionsView.EditRequested += async (_, _) => await WithLifecycleErrorAsync(async () =>
        {
            var item = _sessionsView.SelectedSession; if (item is null) return;
            using var dialog = new CreateSessionForm(_sessionService, await _healthTopicService.GetTopicSummariesAsync(), item);
            if (dialog.ShowDialog(this) == DialogResult.OK) await _sessionsPresenter.LoadAsync(item.Id);
        });
        _sessionsView.ArchiveRequested += async (_, _) => await WithLifecycleErrorAsync(async () =>
        {
            var item = _sessionsView.SelectedSession; if (item is null) return;
            if (item.IsArchived) await _sessionService.ReactivateSessionAsync(item.Id, item.ModifiedAt);
            else await _sessionService.ArchiveSessionAsync(item.Id, item.ModifiedAt);
            await _sessionsPresenter.LoadAsync(item.Id);
        });
        _sessionsView.ArchiveFilterChanged += async (_, _) => await WithLifecycleErrorAsync(() => _sessionsPresenter.LoadAsync());
        _actionsView.EditRequested += async (_, _) => await WithLifecycleErrorAsync(async () =>
        {
            var item = _actionsView.SelectedAction; if (item is null) return;
            using var dialog = new CreateHealthActionForm(_healthActionService, await _healthTopicService.GetTopicSummariesAsync(), await _sourceService.GetSourcesAsync(), await _sessionService.GetSessionsAsync(), item);
            if (dialog.ShowDialog(this) == DialogResult.OK) await _actionsPresenter.LoadAsync(item.Id);
        });
        _actionsView.ArchiveRequested += async (_, _) => await WithLifecycleErrorAsync(async () =>
        {
            var item = _actionsView.SelectedAction; if (item is null) return;
            if (item.IsArchived) await _healthActionService.ReactivateHealthActionAsync(item.Id, item.ModifiedAt);
            else await _healthActionService.ArchiveHealthActionAsync(item.Id, item.ModifiedAt);
            await _actionsPresenter.LoadAsync(item.Id);
        });
        _actionsView.ArchiveFilterChanged += async (_, _) => await WithLifecycleErrorAsync(() => _actionsPresenter.LoadAsync());
        _actionsView.EditRoutineRequested += async (_, _) => await WithLifecycleErrorAsync(async () =>
        {
            var item = _actionsView.SelectedRoutine; if (item is null) return;
            using var dialog = new CreateRoutineForm(_healthActionService, item.HealthActionId, item);
            if (dialog.ShowDialog(this) == DialogResult.OK) await _actionsPresenter.LoadAsync();
        });
        _actionsView.EditProgressRequested += async (_, _) => await WithLifecycleErrorAsync(async () =>
        {
            var item = _actionsView.SelectedProgress; if (item is null) return;
            using var dialog = new CreateProgressEntryForm(_healthActionService, item.RoutineId, item);
            if (dialog.ShowDialog(this) == DialogResult.OK) await _actionsPresenter.LoadAsync();
        });
        _actionsView.DeleteProgressRequested += async (_, _) => await WithLifecycleErrorAsync(async () =>
        {
            var item = _actionsView.SelectedProgress; if (item is null || !ConfirmDeletion(AppStrings.DeleteProgressLabel(item.OccurredAt))) return;
            await _healthActionService.DeleteProgressEntryAsync(item.Id, item.ModifiedAt); await _actionsPresenter.LoadAsync();
        });
        _actionsView.HistoryRequested += async (_, _) => await WithLifecycleErrorAsync(async () =>
        {
            var item = _actionsView.SelectedAction; if (item is null) return;
            using var dialog = new HealthActionHistoryForm((await _healthActionService.GetActionDetailsAsync(item.Id)).Revisions,
                await _healthTopicService.GetTopicSummariesAsync(), await _sourceService.GetSourcesAsync(), await _sessionService.GetSessionsAsync());
            dialog.ShowDialog(this);
        });
    }
    private bool ConfirmDeletion(string record)
    { using var dialog = new DeleteConfirmationForm(record); return dialog.ShowDialog(this) == DialogResult.OK; }
    private async Task WithLifecycleErrorAsync(Func<Task> action)
    {
        try { await action(); }
        catch (LifecycleConflictException) { MessageBox.Show(this, AppStrings.LifecycleConflict, AppStrings.Edit, MessageBoxButtons.OK, MessageBoxIcon.Information); }
        catch { UiErrorHandler.ShowSafeError(this, AppStrings.OperationLoadLocalNotebookData); }
    }
}
