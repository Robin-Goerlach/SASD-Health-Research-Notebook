using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.Infrastructure.Storage;
using Sasd.HealthNotebook.WinForms.Controls;
using Sasd.HealthNotebook.WinForms.Forms;
using Sasd.HealthNotebook.WinForms.Localization;
using Sasd.HealthNotebook.WinForms.Views;
using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Repositories;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.WinForms.Presentation;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Sasd.HealthNotebook.WinForms.SmokeTests;

internal static partial class Program
{
    // UI-LIF-001: actual shell commands/dialogs, confirmation defaults, restart and both languages.
    private static void CheckLifecycle(string testPath, UiLanguage language)
    {
        string root = Path.Combine(testPath, "lifecycle-" + language); Directory.CreateDirectory(root);
        Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, root);
        try
        {
            var time = DateTimeOffset.Now.AddSeconds(-20);
            var measurements = CreateMeasurementService(); var entries = CreateEntryService(); var sessions = CreateSessionService(); var actions = CreateHealthActionService();
            Task.Run(() => measurements.CreateMeasurementAsync(new() { MeasurementType = MeasurementType.Weight, OccurredAt = time, Value = 12.5 })).GetAwaiter().GetResult();
            var measurement = Task.Run(() => new JsonMeasurementRepository().GetAllAsync()).GetAwaiter().GetResult().Single();
            Task.Run(() => entries.CreateEntryAsync(new() { EntryType = HealthEntryType.Note, OccurredAt = time, Title = "CODEX TEST – lifecycle entry", Content = "Synthetic original entry" })).GetAwaiter().GetResult();
            var entry = Task.Run(() => new JsonHealthEntryRepository().GetAllAsync()).GetAwaiter().GetResult().Single();
            var session = Task.Run(() => sessions.CreateSessionAsync(new(time, "CODEX TEST – lifecycle session", Status: SessionStatus.Completed))).GetAwaiter().GetResult();
            var question = Task.Run(() => sessions.CreateQuestionAsync(new(session.Id, "Synthetic lifecycle question"))).GetAwaiter().GetResult();
            var followUp = Task.Run(() => sessions.CreateFollowUpAsync(new(session.Id, "Synthetic lifecycle follow-up"))).GetAwaiter().GetResult();
            var action = Task.Run(() => actions.CreateActionAsync(new("CODEX TEST – professional original", Origin: HealthActionOrigin.Doctor, Description: "Synthetic original instruction", SessionId: session.Id))).GetAwaiter().GetResult();
            var routine = Task.Run(() => actions.CreateRoutineAsync(new(action.Id, "CODEX TEST – lifecycle routine"))).GetAwaiter().GetResult();
            var progress = Task.Run(() => actions.CreateProgressEntryAsync(new(routine.Id, time, Note: "Synthetic original progress"))).GetAwaiter().GetResult();
            using (var shell = NewActionShell())
            {
                ShowOffScreen(shell);
                NavigateLifecycle(shell, "_measurementsButton");
                var view = Field<MeasurementsView>(shell, "_measurementsView");
                Assert(view.SelectedMeasurement?.Id == measurement.Id, "Measurement selected wrong record.");
                byte[] noOp = File.ReadAllBytes(LocalHealthNotebookPaths.MeasurementsFilePath);
                LifecycleDialog<CreateMeasurementForm>(Field<Button>(view, "_editButton"), dialog =>
                {
                    Assert(dialog.Text == AppStrings.EditMeasurement && Field<TextBox>(dialog, "_valueTextBox").Text == 12.5.ToString("R", AppStrings.MeasurementCulture), "Measurement editor not prefilled.");
                    Assert(Field<ComboBox>(dialog, "_typeComboBox").Text == AppStrings.MeasurementTypeText(MeasurementType.Weight), "Measurement edit category not prefilled.");
                    CheckLifecycleDialogLayout(dialog, root, "measurement-edit");
                    ((Button)dialog.AcceptButton!).PerformClick();
                });
                WaitForReload(shell);
                Assert(noOp.SequenceEqual(File.ReadAllBytes(LocalHealthNotebookPaths.MeasurementsFilePath)), "Unchanged UI save rewrote measurement/precision.");
                LifecycleDialog<CreateMeasurementForm>(Field<Button>(view, "_editButton"), dialog =>
                { Field<TextBox>(dialog, "_valueTextBox").Text = 13.5.ToString("R", AppStrings.MeasurementCulture); ((Button)dialog.AcceptButton!).PerformClick(); });
                PumpUntil(() => view.SelectedMeasurement?.Value == 13.5, "measurement edit refresh");
                Assert(view.SelectedMeasurement?.Id == measurement.Id && view.SelectedMeasurement.OccurredAt.EqualsExact(time), "Measurement edit lost selection/time precision.");
                NavigateLifecycle(shell, "_timelineButton");
                var timeline = Field<TimelineView>(shell, "_timelineView");
                var timelineGrid = Field<DataGridView>(timeline, "_grid");
                var measurementRow = timelineGrid.Rows.Cast<DataGridViewRow>().Single(row => Equals(row.Cells[1].Value, AppStrings.Measurements));
                timelineGrid.CurrentCell = measurementRow.Cells[0];
                LifecycleDialog<CreateMeasurementForm>(Field<Button>(timeline, "_editButton"), dialog =>
                {
                    Assert(Field<TextBox>(dialog, "_valueTextBox").Text == 13.5.ToString("R", AppStrings.MeasurementCulture),
                        "Mixed timeline opened the wrong measurement.");
                    Field<TextBox>(dialog, "_noteTextBox").Text = "Synthetic timeline correction";
                    ((Button)dialog.AcceptButton!).PerformClick();
                });
                WaitForReload(shell);
                Assert(timeline.SelectedItem?.Measurement?.Measurement.Id == measurement.Id
                    && timeline.SelectedItem.Measurement.Measurement.Note == "Synthetic timeline correction",
                    "Mixed timeline edit lost source or selection.");
                var entryRow = timelineGrid.Rows.Cast<DataGridViewRow>().Single(row => !Equals(row.Cells[1].Value, AppStrings.Measurements));
                timelineGrid.CurrentCell = entryRow.Cells[0];

                LifecycleDialog<CreateHealthEntryForm>(Field<Button>(timeline, "_editButton"), dialog =>
                {
                    Assert(dialog.Text == AppStrings.EditEntry && Field<TextBox>(dialog, "_contentTextBox").Text == entry.Content, "Entry editor wrong content.");
                    Field<TextBox>(dialog, "_titleTextBox").Text = "CODEX TEST – corrected entry";
                    Field<TextBox>(dialog, "_contentTextBox").Text = "Synthetic corrected entry";
                    CheckLifecycleDialogLayout(dialog, root, "entry-edit"); ((Button)dialog.AcceptButton!).PerformClick();
                });
                WaitForReload(shell); Assert(timeline.SelectedEntryId == entry.Id, "Entry edit lost selection.");
                var displayed = timeline.SelectedEntry!;
                var latest = Task.Run(() => entries.GetByIdAsync(entry.Id)).GetAwaiter().GetResult();
                Task.Run(() => entries.UpdateHealthEntryAsync(entry.Id, new() { Title = "CODEX TEST – concurrent entry", Content = latest.Content,
                    EntryType = latest.EntryType, OccurredAt = latest.OccurredAt }, latest.ModifiedAt)).GetAwaiter().GetResult();
                Assert(timeline.SelectedEntry!.ModifiedAt == displayed.ModifiedAt, "Timeline substituted fresh token for displayed version.");
                bool staleDeleteBlocked = Task.Run(async () =>
                {
                    try { await entries.DeleteHealthEntryAsync(displayed.Id, displayed.ModifiedAt); return false; }
                    catch (LifecycleConflictException) { return true; }
                }).GetAwaiter().GetResult();
                Assert(staleDeleteBlocked, "Stale displayed timeline deletion was accepted.");
                NavigateLifecycle(shell, "_sessionsButton");
                var sessionView = Field<SessionsView>(shell, "_sessionsView");
                LifecycleDialog<CreateSessionForm>(Field<Button>(sessionView, "_editButton"), dialog =>
                { Assert(dialog.Text == AppStrings.EditSession, "Session edit title."); Field<TextBox>(dialog, "_notesTextBox").Text = "Synthetic corrected session notes"; CheckLifecycleDialogLayout(dialog, root, "session-edit"); ((Button)dialog.AcceptButton!).PerformClick(); });
                PumpUntil(() => sessionView.SelectedSession?.Notes == "Synthetic corrected session notes", "session edit refresh");
                Assert(sessionView.SelectedSession?.Id == session.Id, "Session edit lost selection.");
                NavigateLifecycle(shell, "_actionsButton");
                var actionView = Field<HealthActionsView>(shell, "_actionsView");
                PumpUntil(() => actionView.SelectedProgress?.Id == progress.Id, "lifecycle action children");
                LifecycleDialog<CreateHealthActionForm>(Field<Button>(actionView, "_editButton"), dialog =>
                {
                    Assert(dialog.Text == AppStrings.EditAction && Field<TextBox>(dialog, "_descriptionTextBox").Text == action.Description, "Action editor wrong content.");
                    Field<TextBox>(dialog, "_titleTextBox").Text = "CODEX TEST – professional corrected";
                    Field<TextBox>(dialog, "_descriptionTextBox").Text = "Synthetic corrected instruction";
                    Field<TextBox>(dialog, "_changeReasonTextBox").Text = "Synthetic typo correction";
                    CheckLifecycleDialogLayout(dialog, root, "action-edit"); ((Button)dialog.AcceptButton!).PerformClick();
                });
                PumpUntil(() => actionView.SelectedAction?.Title == "CODEX TEST – professional corrected" && actionView.SelectedProgress?.Id == progress.Id, "action edit selection");
                LifecycleDialog<CreateHealthActionForm>(Field<Button>(actionView, "_editButton"), dialog =>
                { Field<ComboBox>(dialog, "_sessionComboBox").SelectedIndex = 0; ((Button)dialog.AcceptButton!).PerformClick(); });
                PumpUntil(() => actionView.SelectedAction?.SessionId is null && actionView.SelectedProgress?.Id == progress.Id, "link-only action correction");
                Assert(Task.Run(() => actions.GetActionDetailsAsync(action.Id)).GetAwaiter().GetResult().Revisions.Count == 2, "Link-only professional correction lacks revision.");
                LifecycleDialog<HealthActionHistoryForm>(Field<Button>(actionView, "_historyButton"), dialog =>
                {
                    string text = Descendants(dialog).OfType<RichTextBox>().Single().Text;
                    Assert(text.Contains(action.Title) && text.Contains(action.Description!) && text.Contains(session.Title)
                        && text.Contains(AppStrings.ActionOriginText(action.Origin)) && text.Contains("Synthetic typo correction"), "Previous instruction/provenance not visible in history.");
                    Capture(dialog, Path.Combine(root, "professional-history.png")); dialog.Close();
                });
                LifecycleDialog<CreateRoutineForm>(Field<Button>(actionView, "_editRoutineButton"), dialog =>
                { Assert(dialog.Text == AppStrings.EditRoutine, "Routine edit title."); Field<TextBox>(dialog, "_scheduleTextBox").Text = "Synthetic corrected rhythm"; CheckLifecycleDialogLayout(dialog, root, "routine-edit"); ((Button)dialog.AcceptButton!).PerformClick(); });
                PumpUntil(() => actionView.SelectedRoutine?.ScheduleText == "Synthetic corrected rhythm" && actionView.SelectedProgress?.Id == progress.Id, "routine edit selection");
                LifecycleDialog<CreateProgressEntryForm>(Field<Button>(actionView, "_editProgressButton"), dialog =>
                { Assert(dialog.Text == AppStrings.EditProgress, "Progress edit title."); Field<TextBox>(dialog, "_noteTextBox").Text = "Synthetic corrected progress"; CheckLifecycleDialogLayout(dialog, root, "progress-edit"); ((Button)dialog.AcceptButton!).PerformClick(); });
                PumpUntil(() => actionView.SelectedProgress?.Note == "Synthetic corrected progress", "progress edit selection");
                var selector = Field<ComboBox>(shell, "_languageComboBox"); selector.SelectedIndex = language == UiLanguage.German ? 1 : 0; WaitForReload(shell);
                Assert(actionView.SelectedAction?.Id == action.Id && actionView.SelectedProgress?.Id == progress.Id, "Lifecycle language switch lost selection.");
                selector.SelectedIndex = language == UiLanguage.German ? 0 : 1; WaitForReload(shell);
                CheckLifecycleWorkspace(shell, actionView, root, "actions-edited");
                shell.Close();
            }
            using var restarted = NewActionShell(); ShowOffScreen(restarted);
            NavigateLifecycle(restarted, "_measurementsButton");
            var mv = Field<MeasurementsView>(restarted, "_measurementsView");
            Assert(mv.SelectedMeasurement?.Id == measurement.Id && mv.SelectedMeasurement.Value == 13.5, "Measurement correction lost on restart.");
            NavigateLifecycle(restarted, "_timelineButton");
            var mixedView = Field<TimelineView>(restarted, "_timelineView");
            var mixedGrid = Field<DataGridView>(mixedView, "_grid");
            mixedGrid.CurrentCell = mixedGrid.Rows.Cast<DataGridViewRow>().Single(row => Equals(row.Cells[1].Value, AppStrings.Measurements)).Cells[0];
            ConfirmLifecycleDelete(Field<Button>(mixedView, "_deleteButton"), false);
            Assert(mv.SelectedMeasurement?.Id == measurement.Id && Task.Run(() => new JsonMeasurementRepository().GetAllAsync()).GetAwaiter().GetResult().Count == 1, "Cancel deleted measurement.");
            ConfirmLifecycleDelete(Field<Button>(mixedView, "_deleteButton"), true);
            NavigateLifecycle(restarted, "_measurementsButton");
            PumpUntil(() => mv.SelectedMeasurement is null, "measurement deletion selection");
            Assert(!Field<Button>(mv, "_deleteButton").Enabled, "Delete remains enabled on empty list.");
            NavigateLifecycle(restarted, "_timelineButton");
            var tv = Field<TimelineView>(restarted, "_timelineView");
            Assert(Task.Run(() => CreateEntryService().GetByIdAsync(entry.Id)).GetAwaiter().GetResult().Content == "Synthetic corrected entry", "Entry correction lost on restart.");
            ConfirmLifecycleDelete(Field<Button>(tv, "_deleteButton"), false); Assert(tv.SelectedEntryId == entry.Id, "Cancel deleted entry.");
            ConfirmLifecycleDelete(Field<Button>(tv, "_deleteButton"), true); PumpUntil(() => tv.SelectedEntryId is null, "entry deletion selection");
            NavigateLifecycle(restarted, "_sessionsButton");
            var sv = Field<SessionsView>(restarted, "_sessionsView");
            Assert(sv.SelectedSession?.Notes == "Synthetic corrected session notes", "Session correction lost on restart.");
            Field<Button>(sv, "_archiveButton").PerformClick(); PumpUntil(() => sv.SelectedSession is null, "archive hides session");
            Field<CheckBox>(sv, "_showArchivedCheckBox").Checked = true;
            PumpUntil(() => sv.SelectedSession?.IsArchived == true && sv.SelectedQuestion?.Id == question.Id, "show archived session children");
            Assert(Field<Button>(sv, "_archiveButton").Text == AppStrings.Reactivate && sv.SelectedSession?.Status == SessionStatus.Completed, "Session archive status semantics.");
            var followUpTab = Field<TabPage>(sv, "_followUpsTab");
            ((TabControl)followUpTab.Parent!).SelectedTab = followUpTab;
            PumpUntil(() => sv.SelectedFollowUp?.Id == followUp.Id, "archived session follow-up tab binding");
            CheckLifecycleWorkspace(restarted, sv, root, "session-archived");
            Field<Button>(sv, "_archiveButton").PerformClick(); PumpUntil(() => sv.SelectedSession?.IsArchived == false, "reactivated session");
            NavigateLifecycle(restarted, "_actionsButton");
            var av = Field<HealthActionsView>(restarted, "_actionsView");
            PumpUntil(() => av.SelectedProgress?.Id == progress.Id, "restart corrected action children");
            Assert(av.SelectedAction?.Title == "CODEX TEST – professional corrected" && av.SelectedRoutine?.ScheduleText == "Synthetic corrected rhythm" && av.SelectedProgress?.Note == "Synthetic corrected progress", "Action/routine/progress correction lost on restart.");
            Field<Button>(av, "_archiveButton").PerformClick(); PumpUntil(() => av.SelectedAction is null, "archive hides action");
            Assert(Task.Run(() => CreateHealthActionService().GetOverviewAsync(DateOnly.FromDateTime(DateTime.Now))).GetAwaiter().GetResult().ActiveActions == 0, "Archived action counted active.");
            Field<CheckBox>(av, "_showArchivedCheckBox").Checked = true;
            PumpUntil(() => av.SelectedAction?.IsArchived == true && av.SelectedProgress?.Id == progress.Id, "show archived action children");
            Assert(Field<Button>(av, "_archiveButton").Text == AppStrings.Reactivate, "Action reactivate label.");
            CheckLifecycleWorkspace(restarted, av, root, "action-archived");
            Field<Button>(av, "_archiveButton").PerformClick(); PumpUntil(() => av.SelectedAction?.IsArchived == false && av.SelectedProgress?.Id == progress.Id, "reactivated action children");
            Field<Button>(av, "_routineStatusButton").PerformClick(); PumpUntil(() => av.SelectedRoutine?.Status == RoutineStatus.Paused && av.SelectedProgress?.Id == progress.Id, "pause preserves progress");
            ConfirmLifecycleDelete(Field<Button>(av, "_deleteProgressButton"), false); Assert(av.SelectedProgress?.Id == progress.Id, "Cancel deleted progress.");
            ConfirmLifecycleDelete(Field<Button>(av, "_deleteProgressButton"), true); PumpUntil(() => av.SelectedProgress is null && av.SelectedRoutine?.Id == routine.Id, "progress deletion selection");
            var overview = Task.Run(() => CreateHealthActionService().GetOverviewAsync(DateOnly.FromDateTime(DateTime.Now))).GetAwaiter().GetResult();
            Assert(overview.ActiveActions == 1 && overview.ActiveRoutines == 0 && overview.EntriesToday == 0, "Post-lifecycle dashboard counts.");
            NavigateLifecycle(restarted, "_dashboardButton");
            var cards = Descendants(Field<HealthActionOverviewControl>(restarted, "_dashboardActionsOverview")).OfType<DashboardCardControl>().ToArray();
            Assert(Field<Label>(cards[0], "_valueLabel").Text == "1" && Field<Label>(cards[1], "_valueLabel").Text == "0" && Field<Label>(cards[2], "_valueLabel").Text == "0", "Dashboard did not refresh lifecycle counts.");
            Assert(Task.Run(() => new JsonMeasurementRepository().GetAllAsync()).GetAwaiter().GetResult().Count == 0 && Task.Run(() => new JsonHealthEntryRepository().GetAllAsync()).GetAwaiter().GetResult().Count == 0, "Deletion did not persist.");
            restarted.Close();
            CheckSessionRefreshRace();
            Console.WriteLine("Lifecycle UI edit/archive/history/delete/restart checks passed: " + language);
        }
        finally { Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, testPath); }
    }
    private static void CheckSessionRefreshRace()
    {
        var repo = new JsonSessionRepository();
        var current = Task.Run(() => repo.LoadAsync()).GetAwaiter().GetResult().Sessions.Single();
        var delayedTopics = new DelayedLifecycleTopics();
        var service = new SessionService(repo, delayedTopics);
        using var view = new SessionsView(); using var host = new Form(); host.Controls.Add(view); ShowOffScreen(host);
        var presenter = new SessionsPresenter(service, view);
        Task oldRefresh = presenter.LoadAsync(); PumpUntil(() => delayedTopics.Calls > 0, "delayed old session refresh");
        Task.Run(() => service.ArchiveSessionAsync(current.Id, current.ModifiedAt)).GetAwaiter().GetResult();
        Task newRefresh = presenter.LoadAsync(); PumpUntil(() => newRefresh.IsCompleted, "current archived session refresh"); newRefresh.GetAwaiter().GetResult();
        delayedTopics.First.SetResult(Array.Empty<HealthTopic>());
        PumpUntil(() => oldRefresh.IsCompleted, "obsolete session refresh completion"); oldRefresh.GetAwaiter().GetResult();
        Assert(view.SelectedSession is null && Field<DataGridView>(view, "_sessionsGrid").Rows.Count == 0, "Old refresh resurrected archived session.");
        host.Close();
    }
    private sealed class DelayedLifecycleTopics : IHealthTopicRepository
    {
        public int Calls;
        public TaskCompletionSource<IReadOnlyList<HealthTopic>> First { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public Task<IReadOnlyList<HealthTopic>> GetAllAsync(CancellationToken cancellationToken = default) =>
            Interlocked.Increment(ref Calls) == 1 ? First.Task : Task.FromResult<IReadOnlyList<HealthTopic>>(Array.Empty<HealthTopic>());
        public Task AddAsync(HealthTopic topic, CancellationToken cancellationToken = default) => throw new InvalidOperationException("Read-only fixture.");
        public Task SaveAllAsync(IReadOnlyList<HealthTopic> topics, CancellationToken cancellationToken = default) => throw new InvalidOperationException("Read-only fixture.");
    }
    private static void NavigateLifecycle(MainForm shell, string button)
    { Field<NavigationButton>(Field<NavigationControl>(shell, "_navigation"), button).PerformClick(); WaitForReload(shell); }
    private static void CheckLifecycleDialogLayout(Form dialog, string root, string name)
    {
        dialog.Size = dialog.MinimumSize; System.Windows.Forms.Application.DoEvents();
        foreach (var editor in Descendants(dialog).Where(item => item.Visible && (item is TextBox or ComboBox or DateTimePicker)))
        { AssertWithinParent(editor); Assert(editor.Height >= 20, "Edit field collapsed."); }
        Assert(dialog.AcceptButton is Button save && save.Text == AppStrings.SaveChanges, "Edit save label ambiguous.");
        foreach (var button in Descendants(dialog).OfType<Button>()) { AssertWithinParent(button); AssertTextFits(button); }
        Capture(dialog, Path.Combine(root, name + ".png"));
    }
    private static void CheckLifecycleWorkspace(MainForm shell, Control view, string root, string name)
    {
        foreach (var size in new[] { new Size(1280, 820), shell.MinimumSize })
        {
            shell.Size = size; System.Windows.Forms.Application.DoEvents();
            foreach (var button in Descendants(view).OfType<Button>().Where(button => button.Visible)) { AssertWithinParent(button); AssertTextFits(button); }
            foreach (var check in Descendants(view).OfType<CheckBox>()) { AssertWithinParent(check); AssertTextFits(check); }
            Capture(shell, Path.Combine(root, name + "-" + size.Width + ".png"));
        }
    }
    private static void ConfirmLifecycleDelete(Button command, bool confirm) => LifecycleDialog<DeleteConfirmationForm>(command, dialog =>
    {
        Assert(dialog.AcceptButton == dialog.CancelButton && dialog.AcceptButton != Field<Button>(dialog, "_deleteButton"), "Enter can trigger Delete.");
        Assert(Descendants(dialog).OfType<Label>().Single().Text.Contains(AppStrings.ConfirmDelete("").Split('\r')[2]), "Delete warning missing.");
        Field<Button>(dialog, confirm ? "_deleteButton" : "_cancelButton").PerformClick();
    });
    private static void LifecycleDialog<T>(Button command, Action<T> handle) where T : Form
    {
        bool closed = false, handled = false; Exception? failure = null;
        var elapsed = Stopwatch.StartNew();
        Console.WriteLine("Lifecycle dialog: " + typeof(T).Name);
        using var timer = new System.Windows.Forms.Timer { Interval = 30 };
        timer.Tick += (_, _) =>
        {
            // Unexpected native error dialogs must fail the synthetic test, not leave CI hung.
            EnumThreadWindows(GetCurrentThreadId(), (window, _) =>
            {
                var className = new StringBuilder(64); GetClassName(window, className, className.Capacity);
                if (className.ToString() == "#32770")
                { failure = new InvalidOperationException("Unexpected native error dialog during " + typeof(T).Name); SendMessage(window, 0x0010, IntPtr.Zero, IntPtr.Zero); }
                return true;
            }, IntPtr.Zero);
            var dialog = System.Windows.Forms.Application.OpenForms.OfType<T>().SingleOrDefault(); if (dialog is null) return;
            if (failure is not null || elapsed.Elapsed > TimeSpan.FromSeconds(15))
            {
                string validation = dialog is CreateMeasurementForm or CreateHealthEntryForm or SourceRecordForm
                    ? Field<Label>(dialog, "_validationLabel").Text : string.Empty;
                failure ??= new InvalidOperationException("Lifecycle dialog did not save/close: " + typeof(T).Name + "; validation: " + validation);
                dialog.Close(); return;
            }
            if (handled) return;
            handled = true; dialog.FormClosed += (_, _) => closed = true;
            try { handle(dialog); } catch (Exception error) { failure = error; dialog.Close(); closed = true; }
        };
        timer.Start(); command.PerformClick(); PumpUntil(() => closed, typeof(T).Name + " lifecycle dialog");
        if (failure is not null) throw new InvalidOperationException(failure.Message, failure);
    }
    private delegate bool WindowCallback(IntPtr window, IntPtr parameter);
    [DllImport("user32.dll")] private static extern bool EnumThreadWindows(uint threadId, WindowCallback callback, IntPtr parameter);
    [DllImport("kernel32.dll")] private static extern uint GetCurrentThreadId();
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int GetClassName(IntPtr window, StringBuilder value, int maximum);
    [DllImport("user32.dll")] private static extern IntPtr SendMessage(IntPtr window, uint message, IntPtr wParam, IntPtr lParam);
}
