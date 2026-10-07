using System.Reflection;
using System.Text;
using Sasd.HealthNotebook.Application.Repositories;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.Infrastructure.Storage;
using Sasd.HealthNotebook.WinForms.Forms;
using Sasd.HealthNotebook.WinForms.Localization;
using Sasd.HealthNotebook.WinForms.Views;

namespace Sasd.HealthNotebook.WinForms.SmokeTests;

internal static partial class Program
{
    // UI-WIZARD-002: real controls, modal shell integration, JSON and delayed writes; DE/EN.
    private static void CheckWizard(HealthTopicService service, string testPath, UiLanguage language)
    {
        var data = new WizardRepository();
        using (var empty = new CreateHealthTopicWizardForm(new HealthTopicService(data)))
        {
            ShowOffScreen(empty);
            Assert(Field<TextBox>(empty, "_titleTextBox").Focused, "Opening focus is not title.");
            WizardKey(empty, Keys.Escape);
            Assert(empty.IsDisposed && data.AddCalls == 0, "Empty Escape persisted or failed to close.");
        }
        using (var wizard = new CreateHealthTopicWizardForm(new HealthTopicService(data)))
        {
            wizard.Size = wizard.MinimumSize;
            ShowOffScreen(wizard);
            var title = Field<TextBox>(wizard, "_titleTextBox");
            var next = Field<Button>(wizard, "_nextButton");
            var back = Field<Button>(wizard, "_backButton");
            Assert(!back.Enabled && !next.Enabled, "Empty title must block navigation.");
            title.Text = "   "; WizardKey(wizard, Keys.Enter);
            Assert(!next.Enabled && title.Focused && Field<Label>(wizard, "_validationLabel").Text == AppStrings.MissingTitleMessage,
                "Whitespace validation/focus failed.");
            title.Text = new string('T', 161); Assert(!next.Enabled, "Overlong title accepted.");
            title.Text = "Synthetic wizard delayed";
            var description = Field<TextBox>(wizard, "_shortDescriptionTextBox"); description.Text = new string('D', 501); Assert(!next.Enabled, "Overlong description accepted.");
            description.Text = "Synthetic description";
            CheckWizardLayout(wizard);
            Capture(wizard, Path.Combine(testPath, $"wizard-basic-{language}.png"));
            Assert(wizard.SelectNextControl(title, true, true, true, false) && description.Focused, "Tab order did not reach description.");
            Assert(wizard.SelectNextControl(description, false, true, true, false) && title.Focused, "Shift+Tab did not return to title.");
            Field<ListBox>(wizard, "_stepsListBox").SelectedIndex = 3;
            Assert(Field<ListBox>(wizard, "_stepsListBox").SelectedIndex == 0, "Progress bypassed navigation.");
            WizardKey(wizard, Keys.Enter);
            var status = Field<ComboBox>(wizard, "_statusComboBox"); var priority = Field<ComboBox>(wizard, "_priorityComboBox");
            foreach (HealthTopicStatus value in Enum.GetValues<HealthTopicStatus>())
                Assert(status.Items[(int)value]?.ToString() == AppStrings.HealthTopicStatusText(value), "Status localization mismatch.");
            foreach (HealthTopicPriority value in Enum.GetValues<HealthTopicPriority>())
                Assert(priority.Items[(int)value]?.ToString() == AppStrings.HealthTopicPriorityText(value), "Priority localization mismatch.");
            CheckWizardLayout(wizard);
            Capture(wizard, Path.Combine(testPath, $"wizard-classification-{language}.png"));
            Assert(status.Focused && !title.Visible, "Classification focus/visibility failed.");
            Assert(wizard.SelectNextControl(status, true, true, true, false) && priority.Focused, "Classification Tab failed.");
            status.SelectedIndex = -1; Assert(!next.Enabled, "Missing status accepted."); status.SelectedIndex = 2;
            priority.SelectedIndex = -1; Assert(!next.Enabled, "Missing priority accepted."); priority.SelectedIndex = 2;
            Assert(status.SelectedItem!.ToString() == AppStrings.HealthTopicStatusText(HealthTopicStatus.DoctorReported)
                && priority.SelectedItem!.ToString() == AppStrings.HealthTopicPriorityText(HealthTopicPriority.PrepareForDoctor), "Enums are not localized.");
            back.PerformClick(); Assert(title.Text == "Synthetic wizard delayed" && description.Text == "Synthetic description", "Back lost basic fields.");
            next.PerformClick(); Assert(status.SelectedIndex == 2 && priority.SelectedIndex == 2, "Forward lost classification.");
            next.PerformClick(); var notes = Field<TextBox>(wizard, "_notesTextBox"); notes.Text = new string('N', 4001); Assert(!next.Enabled, "Overlong notes accepted.");
            notes.Text = "Synthetic notes";
            CheckWizardLayout(wizard);
            Capture(wizard, Path.Combine(testPath, $"wizard-notes-{language}.png"));
            Assert(notes.Focused && notes.AcceptsReturn, "Notes focus/newline behavior failed.");
            back.PerformClick(); next.PerformClick(); Assert(notes.Text == "Synthetic notes", "Back/Forward lost notes.");
            next.PerformClick(); var summary = Field<TextBox>(wizard, "_summaryTextBox");
            Assert(summary.ReadOnly && summary.Focused && next.Text == AppStrings.WizardFinish, "Review/Finish focus failed.");
            foreach (string value in new[] { title.Text, description.Text, notes.Text, status.SelectedItem!.ToString()!, priority.SelectedItem!.ToString()! })
                Assert(summary.Text.Contains(value), "Review omits input.");
            foreach (var size in new[] { new System.Drawing.Size(1040, 720), wizard.MinimumSize })
            {
                wizard.Size = size; System.Windows.Forms.Application.DoEvents();
                AssertWithinParent(summary); AssertWithinParent(next); AssertTextFits(next);
                Capture(wizard, Path.Combine(testPath, $"wizard-review-{language}-{size.Width}.png"));
            }
            next.PerformClick();
            // Invoke again even though the button is disabled, exercising the lifecycle guard itself.
            InvokeWizardSave(wizard);
            WizardKey(wizard, Keys.Enter); wizard.Close();
            Assert(data.AddCalls == 1 && !wizard.IsDisposed && !back.Enabled && !Field<Button>(wizard, "_cancelButton").Enabled,
                "Pending write allowed duplicate save or cancellation.");
            data.Resume.SetResult(); PumpUntil(() => wizard.IsDisposed, "delayed wizard save");
            Assert(data.Topics.Count == 1 && wizard.CreatedTopicId == data.Topics[0].Id && data.Topics[0].Id != Guid.Empty
                && data.Topics[0].CreatedAt == data.Topics[0].ModifiedAt && data.Topics[0].CreatedAt != default,
                "Create identity/timestamps/count failed.");
        }
        using (var cancel = new CreateHealthTopicWizardForm(new HealthTopicService(data)))
        {
            ShowOffScreen(cancel); Field<TextBox>(cancel, "_titleTextBox").Text = "Synthetic discard";
            ConfirmWizardDiscard(cancel, false); Assert(!cancel.IsDisposed, "Declining discard closed wizard.");
            ConfirmWizardDiscard(cancel, false, () => Field<Button>(cancel, "_cancelButton").PerformClick());
            Assert(!cancel.IsDisposed, "Cancel button bypassed discard guard.");
            ConfirmWizardDiscard(cancel, true, cancel.Close); Assert(cancel.IsDisposed && data.AddCalls == 1, "Discard persisted inputs.");
        }
        // A failed write keeps inputs and localized inline feedback, then permits one retry.
        var retryData = new WizardRepository { Fail = true };
        using (var retry = new CreateHealthTopicWizardForm(new HealthTopicService(retryData)))
        {
            ShowOffScreen(retry); Field<TextBox>(retry, "_titleTextBox").Text = "Synthetic retry";
            var next = Field<Button>(retry, "_nextButton"); for (int i = 0; i < 3; i++) next.PerformClick();
            next.PerformClick(); PumpUntil(() => next.Enabled, "failed wizard write");
            Assert(!retry.IsDisposed && retry.CreatedTopicId is null && retryData.Topics.Count == 0
                && Field<Label>(retry, "_validationLabel").Text == AppStrings.FormatSafeError(AppStrings.OperationCreateHealthTopic), "Failure handling lost inputs or localization.");
            retryData.Fail = false; next.PerformClick(); retryData.Resume.SetResult(); PumpUntil(() => retry.IsDisposed, "wizard retry");
            Assert(retryData.Topics.Count == 1 && retryData.AddCalls == 2, "Retry count failed.");
        }
        using var shell = new MainForm(service, CreateEntryService(), CreateSourceService(), CreateMeasurementService(), CreateSessionService(), CreateHealthActionService());
        ShowOffScreen(shell); WaitForReload(shell);
        foreach (var archived in new[] { false, true })
        {
            if (archived) NavigateLifecycle(shell, "_healthTopicsButton");
            string title = $"Synthetic integrated wizard {language} {archived}";
            LifecycleDialog<CreateHealthTopicWizardForm>(Field<Button>(shell, "_newTopicButton"), dialog =>
            {
                Field<TextBox>(dialog, "_titleTextBox").Text = title;
                var next = Field<Button>(dialog, "_nextButton"); next.PerformClick();
                Field<ComboBox>(dialog, "_statusComboBox").SelectedIndex = archived ? 4 : 1;
                next.PerformClick(); next.PerformClick(); next.PerformClick();
            });
            var view = Field<HealthTopicsView>(shell, archived ? "_topicsView" : "_dashboardTopicsView");
            PumpUntil(() => view.SelectedTopic?.Title == title, "created topic selected after shell reload");
            var summaries = Task.Run(() => new HealthTopicService(new JsonHealthTopicRepository()).GetTopicSummariesAsync()).GetAwaiter().GetResult();
            Assert(summaries.Count(topic => topic.Title == title) == 1, "JSON create did not persist exactly once.");
            var createdId = view.SelectedTopic!.Id;
            NavigateLifecycle(shell, archived ? "_dashboardButton" : "_healthTopicsButton");
            PumpUntil(() => Field<HealthTopicsView>(shell, archived ? "_dashboardTopicsView" : "_topicsView").SelectedTopic?.Id == createdId,
                "created topic selection on hidden page activation");
            NavigateLifecycle(shell, "_dashboardButton");
            WaitForReload(shell); Assert(view.SelectedTopic?.Title == title, "Refresh lost new selection.");
        }
        Capture(shell, Path.Combine(testPath, $"wizard-after-create-{language}.png")); shell.Close();
        using (var host = new Form())
        {
            using var pendingView = new HealthTopicsView(); host.Controls.Add(pendingView); ShowOffScreen(host);
            var id = Guid.NewGuid(); pendingView.SelectCreatedTopic(id);
            pendingView.SetTopics(new[]
            {
                new Sasd.HealthNotebook.Application.Contracts.HealthTopicSummary { Id = Guid.NewGuid(), Title = "Synthetic other row" },
                new Sasd.HealthNotebook.Application.Contracts.HealthTopicSummary { Id = id, Title = "Synthetic pending create row" }
            });
            Assert(pendingView.SelectedTopic?.Id == id, "Later refresh lost pending create selection."); host.Close();
        }
        Console.WriteLine("UI-WIZARD-002 passed: " + language);
    }
    private static void CheckWizardLayout(Form wizard)
    {
        foreach (Control editor in Descendants(wizard).Where(control => control.Visible && control is TextBox or ComboBox or Button))
            AssertWithinParent(editor);
        foreach (Label label in Descendants(wizard).OfType<Label>().Where(label => label.Visible)) AssertTextFits(label);
    }
    private static void WizardKey(Form wizard, Keys key) => typeof(Form).GetMethod("ProcessDialogKey", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(wizard, new object[] { key });
    private static void InvokeWizardSave(Form wizard) => wizard.GetType().GetMethod("NextButton_Click", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(wizard, new object?[] { null, EventArgs.Empty });
    private static void ConfirmWizardDiscard(Form wizard, bool discard, Action? command = null)
    {
        bool handled = false; bool matched = false;
        using var timer = new System.Windows.Forms.Timer { Interval = 30 };
        timer.Tick += (_, _) => EnumThreadWindows(GetCurrentThreadId(), (window, _) =>
        {
            var name = new StringBuilder(64); GetClassName(window, name, name.Capacity);
            if (name.ToString() != "#32770") return true;
            EnumChildWindows(window, (child, _) => { var text = new StringBuilder(2048); GetWindowText(child, text, text.Capacity);
                if (text.ToString() == AppStrings.WizardDiscardMessage) matched = true; return true; }, IntPtr.Zero);
            handled = true; SendMessage(window, 0x0111, new IntPtr(discard ? 6 : 7), IntPtr.Zero); return true;
        }, IntPtr.Zero);
        timer.Start(); if (command is null) WizardKey(wizard, Keys.Escape); else command();
        Assert(handled && matched, "Discard confirmation/localization missing.");
    }
    private sealed class WizardRepository : IHealthTopicRepository
    {
        internal int AddCalls;
        internal bool Fail;
        internal List<HealthTopic> Topics { get; } = new();
        internal TaskCompletionSource Resume { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public Task<IReadOnlyList<HealthTopic>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<HealthTopic>>(Topics);
        public async Task AddAsync(HealthTopic topic, CancellationToken cancellationToken = default)
        {
            AddCalls++; if (Fail) throw new IOException("Synthetic failure");
            await Resume.Task; Topics.Add(topic);
        }
        public Task SaveAllAsync(IReadOnlyList<HealthTopic> topics, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
}
