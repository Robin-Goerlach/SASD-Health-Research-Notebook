using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.Infrastructure.Storage;
using Sasd.HealthNotebook.WinForms.Controls;
using Sasd.HealthNotebook.WinForms.Forms;
using Sasd.HealthNotebook.WinForms.Localization;
using Sasd.HealthNotebook.WinForms.Views;

namespace Sasd.HealthNotebook.WinForms.SmokeTests;

internal static partial class Program
{
    private static HealthActionService CreateHealthActionService() => new(new JsonHealthActionRepository(), new JsonHealthTopicRepository(), new JsonSourceRepository(), new JsonSessionRepository());
    // UI-ACT-001/002: actual dialog workflow, fresh services, selection without Refresh, proportional layout.
    private static void CheckHealthActions(string testPath, UiLanguage language)
    {
        string root = Path.Combine(testPath, "actions-" + language); Directory.CreateDirectory(root);
        Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, root);
        try
        {
            var topics = new HealthTopicService(new JsonHealthTopicRepository());
            Guid topicId = Task.Run(() => topics.CreateTopicAsync(new() { Title = "CODEX TEST – Action topic" })).GetAwaiter().GetResult().Id;
            var source = Task.Run(() => CreateSourceService().CreateSourceAsync(new() { SourceType = SourceType.OwnObservation, Title = "CODEX TEST – Action source" })).GetAwaiter().GetResult();
            var session = Task.Run(() => CreateSessionService().CreateSessionAsync(new(DateTimeOffset.Now, "CODEX TEST – Action session"))).GetAwaiter().GetResult();
            Guid actionId, routineId, progressId;
            using (var form = NewActionShell())
            {
                ShowOffScreen(form); OpenActions(form);
                var view = Field<HealthActionsView>(form, "_actionsView");
                Assert(AppStrings.Actions == (language == UiLanguage.German ? "Maßnahmen & Routinen" : "Actions & Routines"), "Action navigation language incorrect.");
                Assert(Field<Label>(view, "_emptyStateLabel").Visible && !Field<Button>(view, "_newRoutineButton").Enabled
                    && !Field<Button>(view, "_newProgressButton").Enabled, "Action empty-state commands incorrect.");
                CheckActionLayout(form, view, root, language, "empty");
                RunSourceDialog<CreateHealthActionForm>(Field<Button>(form, "_newTopicButton"), dialog =>
                {
                    Field<TextBox>(dialog, "_titleTextBox").Text = "CODEX TEST – Action with topic";
                    Field<ComboBox>(dialog, "_topicComboBox").SelectedIndex = 1;
                    Field<ComboBox>(dialog, "_sourceComboBox").SelectedIndex = 1;
                    Field<ComboBox>(dialog, "_sessionComboBox").SelectedIndex = 1;
                    Field<ComboBox>(dialog, "_originComboBox").SelectedIndex = (int)HealthActionOrigin.Coach;
                    Field<TextBox>(dialog, "_originNoteTextBox").Text = "Synthetic reported origin.";
                    Field<TextBox>(dialog, "_descriptionTextBox").Text = "Synthetic action description.";
                    CheckActionDialog(dialog, root, "action-dialog");
                });
                PumpUntil(() => view.SelectedAction?.Title == "CODEX TEST – Action with topic", "created action");
                actionId = view.SelectedAction!.Id;
                Assert(view.SelectedAction.HealthTopicId == topicId && Field<RichTextBox>(view, "_actionDetails").Text.Contains("CODEX TEST – Action topic"), "Action topic missing.");
                Assert(view.SelectedAction.SourceId == source.Id && view.SelectedAction.SessionId == session.Id
                    && Field<RichTextBox>(view, "_actionDetails").Text.Contains(source.Title) && Field<RichTextBox>(view, "_actionDetails").Text.Contains(session.Title), "Minimal origin links not shown.");
                RunSourceDialog<CreateRoutineForm>(Field<Button>(view, "_newRoutineButton"), dialog =>
                {
                    Field<TextBox>(dialog, "_titleTextBox").Text = "CODEX TEST – Routine";
                    Field<TextBox>(dialog, "_scheduleTextBox").Text = "Synthetic selected days";
                    Field<TextBox>(dialog, "_descriptionTextBox").Text = "Synthetic routine description.";
                    CheckActionDialog(dialog, root, "routine-dialog");
                });
                PumpUntil(() => view.SelectedRoutine is not null && Field<Button>(view, "_newProgressButton").Enabled, "new routine");
                routineId = view.SelectedRoutine!.Id;
                RunSourceDialog<CreateProgressEntryForm>(Field<Button>(view, "_newProgressButton"), dialog =>
                {
                    Field<ComboBox>(dialog, "_completionComboBox").SelectedIndex = (int)ProgressCompletion.NotPerformed;
                    Field<CheckBox>(dialog, "_countCheckBox").Checked = true;
                    var countEditor = Field<NumericUpDown>(dialog, "_countEditor");
                    Assert(countEditor.Enabled && countEditor.Maximum == int.MaxValue, "Optional count editor invalid.");
                    countEditor.Value = 0; AssertWithinParent(countEditor);
                    Assert(Field<CheckBox>(dialog, "_countCheckBox").TabIndex < countEditor.TabIndex
                        && countEditor.Parent!.TabIndex < Field<TextBox>(dialog, "_noteTextBox").TabIndex, "Count field tab order invalid.");
                    Field<TextBox>(dialog, "_noteTextBox").Text = "Synthetic personal execution note.\r\nSecond line.";
                    CheckActionDialog(dialog, root, "progress-dialog");
                });
                PumpUntil(() => view.SelectedProgress is not null, "new execution"); progressId = view.SelectedProgress!.Id;
                Assert(view.SelectedProgress.Completion == ProgressCompletion.NotPerformed && view.SelectedProgress.RoutineId == routineId && view.SelectedProgress.Count == 0, "Wrong execution parent/state.");
                Field<Button>(view, "_routineStatusButton").PerformClick();
                PumpUntil(() => view.SelectedRoutine?.Status == RoutineStatus.Paused, "paused routine");
                Assert(view.SelectedProgress?.Id == progressId, "Pause lost history selection.");
                Field<Button>(view, "_routineStatusButton").PerformClick();
                PumpUntil(() => view.SelectedRoutine?.Status == RoutineStatus.Active, "reactivated routine");
                WaitForReload(form);
                Assert(view.SelectedAction?.Id == actionId && view.SelectedRoutine?.Id == routineId && view.SelectedProgress?.Id == progressId, "Refresh lost selection/history.");
                CheckActionLayout(form, view, root, language, "filled");
                // A second routine must not display the first routine's history.
                RunSourceDialog<CreateRoutineForm>(Field<Button>(view, "_newRoutineButton"), dialog => Field<TextBox>(dialog, "_titleTextBox").Text = "CODEX TEST – Second routine");
                PumpUntil(() => view.SelectedRoutine?.Title == "CODEX TEST – Second routine", "second routine");
                Assert(view.SelectedProgress is null && Field<Label>(view, "_progressEmpty").Visible, "Routine selection leaked history.");
                var routinesGrid = Field<DataGridView>(view, "_routinesGrid"); routinesGrid.CurrentCell = routinesGrid.Rows[0].Cells[0];
                PumpUntil(() => view.SelectedProgress?.Id == progressId, "routine history reselection");
                RunSourceDialog<CreateHealthActionForm>(Field<Button>(form, "_newTopicButton"), dialog => Field<TextBox>(dialog, "_titleTextBox").Text = "CODEX TEST – Independent action");
                PumpUntil(() => view.SelectedAction?.Title == "CODEX TEST – Independent action", "second action");
                Assert(view.SelectedAction!.HealthTopicId is null && view.SelectedRoutine is null && view.SelectedProgress is null, "Action switch leaked children.");
                var nav = Field<NavigationControl>(form, "_navigation"); Field<NavigationButton>(nav, "_dashboardButton").PerformClick();
                WaitForReload(form);
                var cards = Descendants(Field<HealthActionOverviewControl>(form, "_dashboardActionsOverview")).OfType<DashboardCardControl>().ToArray();
                Assert(Field<Label>(cards[0], "_valueLabel").Text == "2" && Field<Label>(cards[1], "_valueLabel").Text == "2" && Field<Label>(cards[2], "_valueLabel").Text == "1", "Dashboard counts incorrect.");
                Capture(form, Path.Combine(root, "dashboard-actions.png"));
                form.Close();
            }
            using var restarted = NewActionShell(); ShowOffScreen(restarted); OpenActions(restarted);
            var restartedView = Field<HealthActionsView>(restarted, "_actionsView");
            var grid = Field<DataGridView>(restartedView, "_actionsGrid");
            PumpUntil(() => grid.Rows.Count == 2, "fresh action reload"); grid.CurrentCell = grid.Rows[1].Cells[0];
            PumpUntil(() => restartedView.SelectedRoutine?.Id == routineId && restartedView.SelectedProgress?.Id == progressId, "older action children WITHOUT Refresh");
            Assert(restartedView.SelectedAction?.Id == actionId && restartedView.SelectedAction.Origin == HealthActionOrigin.Coach
                && restartedView.SelectedRoutine!.ScheduleText == "Synthetic selected days" && restartedView.SelectedProgress!.Note!.Contains("Second line."), "Fresh shell content lost.");
            grid.CurrentCell = grid.Rows[0].Cells[0]; PumpUntil(() => restartedView.SelectedRoutine is null, "other action clears children");
            grid.CurrentCell = grid.Rows[1].Cells[0]; PumpUntil(() => restartedView.SelectedProgress?.Id == progressId, "action reselection");
            var selector = Field<ComboBox>(restarted, "_languageComboBox");
            selector.SelectedIndex = language == UiLanguage.German ? 1 : 0; WaitForReload(restarted);
            Assert(restartedView.SelectedAction?.Id == actionId && restartedView.SelectedProgress?.Id == progressId, $"Language switch lost selection: action={restartedView.SelectedAction?.Id}, routine={restartedView.SelectedRoutine?.Id}, progress={restartedView.SelectedProgress?.Id}.");
            selector.SelectedIndex = language == UiLanguage.German ? 0 : 1; WaitForReload(restarted);
            CheckActionLayout(restarted, restartedView, root, language, "restarted");
            Console.WriteLine("HealthAction dialog/restart/reselection/layout/dashboard checks passed: " + language);
        }
        finally { Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, testPath); }
    }
    private static MainForm NewActionShell() => new(new HealthTopicService(new JsonHealthTopicRepository()), CreateEntryService(), CreateSourceService(), CreateMeasurementService(), CreateSessionService(), CreateHealthActionService());
    private static void OpenActions(MainForm form)
    {
        var button = Field<NavigationButton>(Field<NavigationControl>(form, "_navigation"), "_actionsButton");
        Assert(button.Text == AppStrings.Actions, "Action navigation text incorrect."); button.PerformClick();
        PumpUntil(() => Field<ToolStripStatusLabel>(form, "_statusLabel").Text!.StartsWith(AppStrings.LoadedActions(0).Split(' ')[0])
            || Field<ToolStripStatusLabel>(form, "_statusLabel").Text == AppStrings.LoadedActions(2), "action page load");
    }
    private static void CheckActionDialog(Form dialog, string root, string name)
    {
        dialog.Size = dialog.MinimumSize; System.Windows.Forms.Application.DoEvents();
        var editors = Descendants(dialog).Where(control => (control is TextBox && control.Parent is not NumericUpDown) || control is ComboBox or DateTimePicker).ToArray();
        Assert(editors.Select(control => control.TabIndex).Distinct().Count() == editors.Length, "Duplicate dialog tab order.");
        foreach (var editor in editors) { AssertWithinParent(editor); Assert(!string.IsNullOrWhiteSpace(editor.AccessibleDescription), "Missing field help."); }
        Assert(dialog.AcceptButton is Control save && save.Visible && save.Parent!.ClientRectangle.Contains(save.Bounds), "Action dialog Save clipped.");
        Capture(dialog, Path.Combine(root, name + ".png"));
    }
    private static void CheckActionLayout(MainForm form, HealthActionsView view, string root, UiLanguage language, string state)
    {
        foreach (var size in new[] { new Size(1280, 820), form.MinimumSize })
        {
            form.Size = size; System.Windows.Forms.Application.DoEvents();
            var headings = new[] { Field<Label>(view, "_actionsHeading"), Field<Label>(view, "_routinesHeading"), Field<Label>(view, "_progressHeading") };
            Assert(headings[0].PointToScreen(Point.Empty).Y == headings[1].PointToScreen(Point.Empty).Y, "Action/routine top edges misaligned.");
            Assert(Math.Abs(headings[1].Parent!.Height + 12 - headings[2].Parent!.Height) <= 2, "Right panel proportional heights changed.");
            foreach (var heading in headings) { AssertWithinParent(heading); AssertTextFits(heading); }
            foreach (var button in new[] { Field<Button>(view, "_newRoutineButton"), Field<Button>(view, "_routineStatusButton"), Field<Button>(view, "_newProgressButton"), Field<Button>(form, "_newTopicButton") })
            { Assert(button.Visible, "Action button hidden."); AssertWithinParent(button); AssertTextFits(button); }
            foreach (var card in Descendants(view).OfType<DashboardCardControl>())
            { AssertWithinParent(card); foreach (var label in card.Controls.OfType<Label>()) AssertTextFits(label); }
            Assert(!Descendants(view).OfType<ScrollableControl>().Any(control => control is Panel && (control.HorizontalScroll.Visible || control.VerticalScroll.Visible)), "Unexpected workspace scrollbar.");
            Capture(form, Path.Combine(root, $"actions-{language}-{state}-{size.Width}.png"));
        }
    }
}
