using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.WinForms.Localization;

namespace Sasd.HealthNotebook.WinForms.Forms;

public sealed partial class MainForm
{
    private void InitializeParentLifecycle()
    {
        foreach (var view in new[] { _topicsView, _dashboardTopicsView })
        {
            view.EditRequested += async (_, _) => await WithLifecycleErrorAsync(async () =>
            {
                var selected = view.SelectedTopic; if (selected is null) return;
                using var dialog = new EditHealthTopicForm(_healthTopicService, await _healthTopicService.GetByIdAsync(selected.Id));
                if (dialog.ShowDialog(this) == DialogResult.OK) await ReloadSafeAsync();
            });
            view.ArchiveRequested += async (_, _) => await WithLifecycleErrorAsync(async () =>
            {
                var selected = view.SelectedTopic; if (selected is null) return;
                if (selected.Status == Domain.HealthTopicStatus.Archived) await _healthTopicService.ReactivateHealthTopicAsync(selected.Id, selected.ModifiedAt);
                else await _healthTopicService.ArchiveHealthTopicAsync(selected.Id, selected.ModifiedAt);
                await ReloadSafeAsync();
            });
            view.DeleteRequested += async (_, _) => await WithLifecycleErrorAsync(async () =>
            {
                var selected = view.SelectedTopic; if (selected is null || !ConfirmDeletion(AppStrings.DeleteTopicLabel(selected.Title))) return;
                try { await _healthTopicService.DeleteHealthTopicAsync(selected.Id, selected.ModifiedAt); await ReloadSafeAsync(); }
                catch (LifecycleDeleteBlockedException) { MessageBox.Show(this, AppStrings.TopicDeleteBlocked, AppStrings.Delete, MessageBoxButtons.OK, MessageBoxIcon.Information); }
            });
        }
        _sessionsView.EditQuestionRequested += async (_, _) => await WithLifecycleErrorAsync(async () =>
        {
            var selected = _sessionsView.SelectedQuestion; if (selected is null) return;
            using var dialog = new SessionQuestionForm(_sessionService, selected.SessionId, selected);
            if (dialog.ShowDialog(this) == DialogResult.OK) await _sessionsPresenter.LoadSelectionAsync();
        });
        _sessionsView.DeleteQuestionRequested += async (_, _) => await WithLifecycleErrorAsync(async () =>
        {
            var selected = _sessionsView.SelectedQuestion; if (selected is null) return;
            if (selected.IsAnswered || !string.IsNullOrWhiteSpace(selected.AnswerNote)) { ShowQuestionDeleteBlocked(); return; }
            if (!ConfirmDeletion(AppStrings.DeleteQuestionLabel)) return;
            try { await _sessionService.DeleteQuestionAsync(selected.Id, selected.ModifiedAt); await _sessionsPresenter.LoadSelectionAsync(); }
            catch (LifecycleDeleteBlockedException) { ShowQuestionDeleteBlocked(); }
        });
        _sessionsView.EditFollowUpRequested += async (_, _) => await WithLifecycleErrorAsync(async () =>
        {
            var selected = _sessionsView.SelectedFollowUp; if (selected is null) return;
            using var dialog = new CreateSessionFollowUpForm(_sessionService, selected.SessionId, selected);
            if (dialog.ShowDialog(this) == DialogResult.OK) await _sessionsPresenter.LoadSelectionAsync();
        });
        _sessionsView.DeleteFollowUpRequested += async (_, _) => await WithLifecycleErrorAsync(async () =>
        {
            var selected = _sessionsView.SelectedFollowUp; if (selected is null || !ConfirmDeletion(AppStrings.DeleteFollowUpLabel)) return;
            await _sessionService.DeleteFollowUpAsync(selected.Id, selected.ModifiedAt); await _sessionsPresenter.LoadSelectionAsync();
        });
    }
    private void ShowQuestionDeleteBlocked() => MessageBox.Show(this, AppStrings.QuestionDeleteBlocked, AppStrings.Delete, MessageBoxButtons.OK, MessageBoxIcon.Information);
}
