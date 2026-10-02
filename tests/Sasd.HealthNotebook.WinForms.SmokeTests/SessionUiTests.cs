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
    private static SessionService CreateSessionService() => new(new JsonSessionRepository(), new JsonHealthTopicRepository());
    // UI-SES-001: actual dialogs, new shell/services and selection WITHOUT Refresh after restart.
    private static void CheckSessions(HealthTopicService topicService, string testPath, UiLanguage language)
    {
        string root = Path.Combine(testPath, "sessions-" + language); Directory.CreateDirectory(root);
        Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, root);
        try
        {
            var topics = new HealthTopicService(new JsonHealthTopicRepository());
            // Fixture creation must not block a repository continuation on the UI thread.
            Guid topicId = Task.Run(() => topics.CreateTopicAsync(new() { Title = "CODEX TEST – Session topic" })).GetAwaiter().GetResult().Id;
            using (var form = NewSessionShell())
            {
                ShowOffScreen(form); OpenSessions(form);
                var view = Field<SessionsView>(form, "_sessionsView");
                Assert(Field<Label>(view, "_emptyStateLabel").Visible && Field<Label>(view, "_emptyStateLabel").Text == AppStrings.SessionsEmpty, "Session empty state missing.");
                Assert(AppStrings.Sessions == (language == UiLanguage.German ? "Termine & Fragen" : "Sessions"), "Session language reversed.");
                RunSourceDialog<CreateSessionForm>(Field<Button>(form, "_newTopicButton"), dialog =>
                {
                    Field<TextBox>(dialog, "_titleTextBox").Text = "CODEX TEST – Session with topic";
                    Field<ComboBox>(dialog, "_topicComboBox").SelectedIndex = 1;
                    Field<TextBox>(dialog, "_notesTextBox").Text = "Synthetic conversation note.";
                    Assert(Field<TextBox>(dialog, "_titleTextBox").TabIndex < Field<TextBox>(dialog, "_notesTextBox").TabIndex, "Session tab order incorrect.");
                    dialog.Size = dialog.MinimumSize;
                    foreach (var editor in new Control[] { Field<TextBox>(dialog, "_titleTextBox"), Field<TextBox>(dialog, "_notesTextBox"), Field<ComboBox>(dialog, "_topicComboBox") })
                    {
                        AssertWithinParent(editor); Assert(!string.IsNullOrWhiteSpace(editor.AccessibleDescription), "Session field guidance missing.");
                    }
                    Assert(dialog.AcceptButton is Control save && save.Visible && save.Parent!.ClientRectangle.Contains(save.Bounds), "Session save clipped.");
                });
                PumpUntil(() => view.SelectedSession?.Title == "CODEX TEST – Session with topic", "created session");
                Assert(view.SelectedSession!.HealthTopicId == topicId && Field<TextBox>(view, "_sessionDetails").Text.Contains("CODEX TEST – Session topic"), "Session topic title missing.");
                RunSourceDialog<SessionQuestionForm>(Field<Button>(view, "_newQuestionButton"), dialog => Field<TextBox>(dialog, "_questionTextBox").Text = "Synthetic question?");
                PumpUntil(() => view.SelectedQuestion is not null, "new question");
                RunSourceDialog<SessionQuestionForm>(Field<Button>(view, "_answerButton"), dialog =>
                {
                    Field<CheckBox>(dialog, "_answeredCheckBox").Checked = true;
                    Field<TextBox>(dialog, "_answerTextBox").Text = "Synthetic recorded answer.";
                });
                PumpUntil(() => view.SelectedQuestion?.IsAnswered == true, "recorded answer");
                var tabs = Field<TabPage>(view, "_followUpsTab").Parent as TabControl; tabs!.SelectedTab = Field<TabPage>(view, "_followUpsTab");
                RunSourceDialog<CreateSessionFollowUpForm>(Field<Button>(view, "_newFollowUpButton"), dialog =>
                {
                    Field<TextBox>(dialog, "_textBox").Text = "Synthetic next step.";
                    var due = Field<DateTimePicker>(dialog, "_duePicker"); due.Value = new DateTime(2026, 10, 10); due.Checked = true;
                });
                PumpUntil(() => view.SelectedFollowUp is not null, "new follow-up");
                Field<Button>(view, "_statusButton").PerformClick();
                PumpUntil(() => view.SelectedFollowUp?.Status == SessionFollowUpStatus.Done, "done follow-up");
                foreach (var size in new[] { new Size(1280, 820), form.MinimumSize })
                {
                    form.Size = size; System.Windows.Forms.Application.DoEvents();
                    Assert(Field<TextBox>(view, "_sessionDetails").Height == Field<TextBox>(view, "_followUpDetails").Height, "Session details heights differ.");
                    Assert(Field<Button>(view, "_newFollowUpButton").Visible && Field<Button>(view, "_statusButton").Visible, "Follow-up actions hidden.");
                    AssertWithinParent(Field<Button>(view, "_statusButton")); AssertWithinParent(Field<Button>(form, "_newTopicButton"));
                    AssertTextFits(Field<Button>(form, "_newTopicButton"));
                    foreach (string field in new[] { "_sessionsGrid", "_followUpsGrid" })
                    {
                        var displayed = Field<DataGridView>(view, field);
                        Assert(displayed.Columns.Cast<DataGridViewColumn>().Sum(column => column.Width) <= displayed.ClientSize.Width, "Session columns clipped.");
                        Assert(displayed.ColumnHeadersHeightSizeMode == DataGridViewColumnHeadersHeightSizeMode.AutoSize, "Session headers must wrap without clipping.");
                    }
                }
                Capture(form, Path.Combine(root, "sessions.png"));
                RunSourceDialog<CreateSessionForm>(Field<Button>(form, "_newTopicButton"), dialog => Field<TextBox>(dialog, "_titleTextBox").Text = "CODEX TEST – Session without topic");
                PumpUntil(() => view.SelectedSession?.Title == "CODEX TEST – Session without topic", "unlinked session");
                Assert(view.SelectedSession!.HealthTopicId is null, "Optional topic became required.");
                form.Close();
            }
            using var restarted = NewSessionShell(); ShowOffScreen(restarted); OpenSessions(restarted);
            var reloaded = Field<SessionsView>(restarted, "_sessionsView"); var grid = Field<DataGridView>(reloaded, "_sessionsGrid");
            PumpUntil(() => grid.Rows.Count == 2, "session restart");
            int index = Enumerable.Range(0, grid.Rows.Count).Single(i => Equals(grid.Rows[i].Cells[1].Value, "CODEX TEST – Session with topic"));
            grid.CurrentCell = grid.Rows[index].Cells[0];
            PumpUntil(() => reloaded.SelectedQuestion?.IsAnswered == true, "reselection questions without refresh");
            var followUpTab = Field<TabPage>(reloaded, "_followUpsTab");
            ((TabControl)followUpTab.Parent!).SelectedTab = followUpTab;
            PumpUntil(() => reloaded.SelectedFollowUp?.Status == SessionFollowUpStatus.Done, "reselection follow-ups without refresh");
            Assert(reloaded.SelectedQuestion!.AnswerNote == "Synthetic recorded answer." && reloaded.SelectedFollowUp!.DueDate == new DateOnly(2026, 10, 10), "Answer/due date lost on restart.");
            WaitForReload(restarted);
            Assert(reloaded.SelectedSession!.Title == "CODEX TEST – Session with topic" && reloaded.SelectedQuestion is not null, "Refresh selection lost.");
            var selector = Field<ComboBox>(restarted, "_languageComboBox");
            selector.SelectedIndex = language == UiLanguage.German ? 1 : 0; WaitForReload(restarted);
            Assert(Field<Button>(restarted, "_newTopicButton").Text == AppStrings.NewSession && reloaded.SelectedSession!.Title == "CODEX TEST – Session with topic", "Session language switch lost state.");
            selector.SelectedIndex = language == UiLanguage.German ? 0 : 1; WaitForReload(restarted);
            restarted.Close(); Console.WriteLine("Session dialog/restart/reselection checks passed: " + language);
        }
        finally { Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, testPath); }
    }
    private static MainForm NewSessionShell() => new(new HealthTopicService(new JsonHealthTopicRepository()), CreateEntryService(), CreateSourceService(), CreateMeasurementService(), CreateSessionService());
    private static void OpenSessions(MainForm form)
    {
        var button = Field<NavigationButton>(Field<NavigationControl>(form, "_navigation"), "_sessionsButton");
        Assert(button.Text == AppStrings.Sessions, "Session navigation localization missing."); button.PerformClick();
    }
}
