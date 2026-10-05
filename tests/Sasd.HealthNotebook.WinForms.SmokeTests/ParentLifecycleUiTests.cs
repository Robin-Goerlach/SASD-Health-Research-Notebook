using System.Runtime.InteropServices;
using System.Text;
using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.Infrastructure.Storage;
using Sasd.HealthNotebook.WinForms.Forms;
using Sasd.HealthNotebook.WinForms.Localization;
using Sasd.HealthNotebook.WinForms.Views;

namespace Sasd.HealthNotebook.WinForms.SmokeTests;

internal static partial class Program
{
    private static void CheckParentLifecycle(string testPath, UiLanguage language)
    {
        string root = Path.Combine(testPath, "parent-lifecycle-" + language); Directory.CreateDirectory(root);
        Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, root);
        try
        {
            var topics = new HealthTopicService(new JsonHealthTopicRepository());
            var topic = Task.Run(() => topics.CreateTopicAsync(new() { Title = "CODEX TEST – editable topic", Status = HealthTopicStatus.Suspected, Notes = "Synthetic original note" })).GetAwaiter().GetResult();
            var sessions = CreateSessionService(); var session = Task.Run(() => sessions.CreateSessionAsync(new(DateTimeOffset.Now, "CODEX TEST – session"))).GetAwaiter().GetResult();
            var unanswered = Task.Run(() => sessions.CreateQuestionAsync(new(session.Id, "Synthetic unanswered"))).GetAwaiter().GetResult();
            var answered = Task.Run(() => sessions.CreateQuestionAsync(new(session.Id, "Synthetic answered", 1, true, "Synthetic answer to retain"))).GetAwaiter().GetResult();
            var follow = Task.Run(() => sessions.CreateFollowUpAsync(new(session.Id, "Synthetic next step", DueDate: new DateOnly(2026, 10, 20)))).GetAwaiter().GetResult();
            using (var shell = NewActionShell())
            {
                ShowOffScreen(shell); NavigateLifecycle(shell, "_healthTopicsButton");
                var view = Field<HealthTopicsView>(shell, "_topicsView");
                LifecycleDialog<EditHealthTopicForm>(Field<Button>(view, "_editButton"), dialog =>
                {
                    Assert(Field<TextBox>(dialog, "_notesTextBox").Text == "Synthetic original note", "Topic edit prefill lost note.");
                    Field<TextBox>(dialog, "_titleTextBox").Text = "CODEX TEST – corrected topic";
                    Field<TextBox>(dialog, "_notesTextBox").Text = "Synthetic corrected note";
                    CheckLifecycleDialogLayout(dialog, root, "topic-edit"); ((Button)dialog.AcceptButton!).PerformClick();
                });
                PumpUntil(() => view.SelectedTopic?.Title == "CODEX TEST – corrected topic", "topic correction");
                CheckLifecycleWorkspace(shell, view, root, "topic-edited");
                Field<Button>(view, "_archiveButton").PerformClick(); PumpUntil(() => view.SelectedTopic is null, "topic archive hides");
                Field<CheckBox>(view, "_showArchived").Checked = true;
                PumpUntil(() => view.SelectedTopic?.Status == HealthTopicStatus.Archived, "topic archive filter");
                Field<Button>(view, "_archiveButton").PerformClick(); PumpUntil(() => view.SelectedTopic?.Status == HealthTopicStatus.Suspected, "topic original status restored");
                // A real reference blocks delete with comprehensible localized feedback.
                var measurements = CreateMeasurementService();
                Task.Run(() => measurements.CreateMeasurementAsync(new() { MeasurementType = MeasurementType.Weight, Value = 13, OccurredAt = DateTimeOffset.Now, HealthTopicId = topic.Id })).GetAwaiter().GetResult();
                ParentBlockedFeedback(Field<Button>(view, "_deleteButton"), AppStrings.TopicDeleteBlocked, true);
                Assert(view.SelectedTopic?.Id == topic.Id && Task.Run(() => topics.GetByIdAsync(topic.Id)).GetAwaiter().GetResult().Notes == "Synthetic corrected note", "Blocked topic deletion lost record.");
                var m = Task.Run(() => new JsonMeasurementRepository().GetAllAsync()).GetAwaiter().GetResult().Single();
                Task.Run(() => measurements.DeleteMeasurementAsync(m.Id, m.ModifiedAt)).GetAwaiter().GetResult();
                ConfirmLifecycleDelete(Field<Button>(view, "_deleteButton"), false); Assert(view.SelectedTopic?.Id == topic.Id, "Cancelled topic delete lost record.");
                ConfirmLifecycleDelete(Field<Button>(view, "_deleteButton"), true); PumpUntil(() => view.SelectedTopic is null, "unreferenced topic deletion");
                NavigateLifecycle(shell, "_sessionsButton"); var sv = Field<SessionsView>(shell, "_sessionsView");
                PumpUntil(() => sv.SelectedQuestion?.Id == unanswered.Id, "unanswered selection");
                LifecycleDialog<SessionQuestionForm>(Field<Button>(sv, "_editQuestionButton"), dialog =>
                { Field<TextBox>(dialog, "_questionTextBox").Text = "Synthetic corrected question"; CheckLifecycleDialogLayout(dialog, root, "question-edit"); ((Button)dialog.AcceptButton!).PerformClick(); });
                PumpUntil(() => sv.SelectedQuestion?.Text == "Synthetic corrected question", "question edit refresh");
                ConfirmLifecycleDelete(Field<Button>(sv, "_deleteQuestionButton"), false); Assert(sv.SelectedQuestion?.Id == unanswered.Id, "Cancelled question delete lost selection.");
                ConfirmLifecycleDelete(Field<Button>(sv, "_deleteQuestionButton"), true); PumpUntil(() => sv.SelectedQuestion?.Id == answered.Id, "question delete chooses survivor");
                ParentBlockedFeedback(Field<Button>(sv, "_deleteQuestionButton"), AppStrings.QuestionDeleteBlocked, false);
                LifecycleDialog<SessionQuestionForm>(Field<Button>(sv, "_editQuestionButton"), dialog =>
                { Assert(Field<TextBox>(dialog, "_answerTextBox").Text == "Synthetic answer to retain", "Answered question edit lost answer.");
                    Field<TextBox>(dialog, "_questionTextBox").Text = "Synthetic corrected answered question"; ((Button)dialog.AcceptButton!).PerformClick(); });
                PumpUntil(() => sv.SelectedQuestion?.Text == "Synthetic corrected answered question", "answered question edit");
                var followTab = Field<TabPage>(sv, "_followUpsTab"); ((TabControl)followTab.Parent!).SelectedTab = followTab;
                PumpUntil(() => sv.SelectedFollowUp?.Id == follow.Id, "follow-up tab");
                LifecycleDialog<CreateSessionFollowUpForm>(Field<Button>(sv, "_editFollowUpButton"), dialog =>
                { Assert(Field<DateTimePicker>(dialog, "_duePicker").Checked, "Follow-up due date not prefilled.");
                    Field<TextBox>(dialog, "_textBox").Text = "Synthetic corrected follow-up"; CheckLifecycleDialogLayout(dialog, root, "followup-edit"); ((Button)dialog.AcceptButton!).PerformClick(); });
                PumpUntil(() => sv.SelectedFollowUp?.Text == "Synthetic corrected follow-up", "follow-up correction");
                CheckLifecycleWorkspace(shell, sv, root, "children-edited");
                ConfirmLifecycleDelete(Field<Button>(sv, "_deleteFollowUpButton"), false); Assert(sv.SelectedFollowUp?.Id == follow.Id, "Cancelled follow-up delete lost record.");
                ConfirmLifecycleDelete(Field<Button>(sv, "_deleteFollowUpButton"), true); PumpUntil(() => sv.SelectedFollowUp is null, "follow-up deletion");
                var languageSelector = Field<ComboBox>(shell, "_languageComboBox"); languageSelector.SelectedIndex = language == UiLanguage.German ? 1 : 0; WaitForReload(shell);
                Assert(Field<Button>(sv, "_editQuestionButton").Text == AppStrings.Edit && Field<Button>(sv, "_deleteFollowUpButton").Text == AppStrings.Delete, "Child lifecycle language switch failed.");
                languageSelector.SelectedIndex = language == UiLanguage.German ? 0 : 1; WaitForReload(shell); shell.Close();
            }
            using var restarted = NewActionShell(); ShowOffScreen(restarted); NavigateLifecycle(restarted, "_sessionsButton");
            var reloaded = Field<SessionsView>(restarted, "_sessionsView");
            PumpUntil(() => reloaded.SelectedQuestion?.Id == answered.Id, "child restart selection");
            Assert(reloaded.SelectedQuestion!.IsAnswered && reloaded.SelectedQuestion.AnswerNote == "Synthetic answer to retain"
                && Task.Run(() => sessions.GetSessionDetailsAsync(session.Id)).GetAwaiter().GetResult().FollowUps.Count == 0
                && Task.Run(() => topics.GetTopicSummariesAsync()).GetAwaiter().GetResult().Count == 0, "Parent/child lifecycle restart lost documentation.");
            restarted.Close(); CheckTopicRefreshRace(); Console.WriteLine("Topic/question/follow-up lifecycle and blocked-delete UX passed: " + language);
        }
        finally { Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, testPath); }
    }
    private static void ParentBlockedFeedback(Button command, string expected, bool confirm)
    {
        bool handled = false; Exception? failure = null;
        using var timer = new System.Windows.Forms.Timer { Interval = 30 };
        timer.Tick += (_, _) =>
        {
            var confirmation = System.Windows.Forms.Application.OpenForms.OfType<DeleteConfirmationForm>().SingleOrDefault();
            if (confirmation is not null && confirm) { confirm = false; Field<Button>(confirmation, "_deleteButton").PerformClick(); }
            EnumThreadWindows(GetCurrentThreadId(), (window, _) =>
            {
                var name = new StringBuilder(64); GetClassName(window, name, name.Capacity); if (name.ToString() != "#32770") return true;
                var messages = new List<string>(); EnumChildWindows(window, (child, _) => { var text = new StringBuilder(2048); GetWindowText(child, text, text.Capacity); messages.Add(text.ToString()); return true; }, IntPtr.Zero);
                if (!messages.Any(text => text == expected)) failure = new InvalidOperationException("Blocked delete showed wrong localized guidance.");
                handled = true; SendMessage(window, 0x0010, IntPtr.Zero, IntPtr.Zero); return true;
            }, IntPtr.Zero);
        };
        timer.Start(); command.PerformClick(); PumpUntil(() => handled, "blocked deletion feedback");
        if (failure is not null) throw failure;
    }
    private static void CheckTopicRefreshRace()
    {
        var data = new DelayedTopicRefresh(); var service = new HealthTopicService(data);
        using var view = new HealthTopicsView(); using var host = new Form(); host.Controls.Add(view); ShowOffScreen(host);
        var presenter = new Sasd.HealthNotebook.WinForms.Presentation.HealthTopicPresenter(service, view);
        var older = presenter.LoadAsync(); PumpUntil(() => data.Entered.Task.IsCompleted, "delayed topic refresh");
        var latest = presenter.LoadAsync(); PumpUntil(() => latest.IsCompleted, "latest topic refresh"); latest.GetAwaiter().GetResult();
        Assert(view.SelectedTopic is null, "Archive was not filtered in latest topic response.");
        data.Resume.SetResult(); PumpUntil(() => older.IsCompleted, "old topic refresh completion"); older.GetAwaiter().GetResult();
        Assert(view.SelectedTopic is null, "Old topic response restored archived selection."); host.Close();
    }
    private sealed class DelayedTopicRefresh : Sasd.HealthNotebook.Application.Repositories.IHealthTopicRepository
    {
        private int _calls;
        private readonly HealthTopic _active = HealthTopic.Create("CODEX TEST – delayed topic", HealthTopicStatus.Observation, HealthTopicPriority.Normal, null, null);
        internal TaskCompletionSource Entered { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        internal TaskCompletionSource Resume { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public async Task<IReadOnlyList<HealthTopic>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            if (++_calls == 1) { Entered.SetResult(); await Resume.Task; return new[] { _active }; }
            var archived = HealthTopic.Create("CODEX TEST – delayed topic", HealthTopicStatus.Archived, HealthTopicPriority.Normal, null, null); archived.Id = _active.Id; return new[] { archived };
        }
        public Task AddAsync(HealthTopic value, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task SaveAllAsync(IReadOnlyList<HealthTopic> values, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
    [DllImport("user32.dll")] private static extern bool EnumChildWindows(IntPtr parent, WindowCallback callback, IntPtr parameter);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int GetWindowText(IntPtr window, StringBuilder text, int maximum);
}
