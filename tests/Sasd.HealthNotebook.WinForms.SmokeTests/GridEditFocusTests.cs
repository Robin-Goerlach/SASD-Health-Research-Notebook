using System.Runtime.ExceptionServices;
using System.Text;
using Sasd.HealthNotebook.Application.Contracts;
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
    // UI-GRID-002: actual modal edit, native focus return, async reload and message pump.
    // Existing PerformClick-only tests did not focus the command as a real mouse click does.
    private static void CheckMeasurementEditFocus(string testPath, UiLanguage language)
    {
        string root = Path.Combine(testPath, "grid-edit-focus-" + language);
        Directory.CreateDirectory(root);
        string? priorPath = Environment.GetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable);
        Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, root);
        Exception? reentrancy = null; bool errorDialog = false;
        EventHandler<FirstChanceExceptionEventArgs> observe = (_, e) =>
        {
            if (e.Exception is InvalidOperationException && e.Exception.Message.Contains("reentrant call", StringComparison.OrdinalIgnoreCase))
                reentrancy ??= e.Exception;
        };
        AppDomain.CurrentDomain.FirstChanceException += observe;
        System.Threading.ThreadExceptionEventHandler threadError = (_, e) => reentrancy ??= e.Exception;
        System.Windows.Forms.Application.ThreadException += threadError;
        // Test-only modal automation: unexpected error dialogs fail the test instead of
        // hanging CI. This is not a production timer or suppressed production exception.
        using var errors = new System.Windows.Forms.Timer { Interval = 30 };
        errors.Tick += (_, _) => EnumThreadWindows(GetCurrentThreadId(), (window, _) =>
        {
            var name = new StringBuilder(64); GetClassName(window, name, name.Capacity);
            if (name.ToString() == "#32770") { errorDialog = true; SendMessage(window, 0x0010, IntPtr.Zero, IntPtr.Zero); }
            return true;
        }, IntPtr.Zero);
        errors.Start();
        try
        {
            var topics = new HealthTopicService(new JsonHealthTopicRepository());
            var topic = Task.Run(() => topics.CreateTopicAsync(new() { Title = "CODEX TEST – linked topic" })).GetAwaiter().GetResult();
            var counted = new CountedMeasurements(new JsonMeasurementRepository());
            var service = new MeasurementService(counted, new JsonHealthTopicRepository());
            var time = new DateTimeOffset(2026, 10, 5, 12, 0, 0, TimeSpan.FromHours(2));
            for (int index = 0; index < 3; index++)
                Task.Run(() => service.CreateMeasurementAsync(new() { MeasurementType = index < 2 ? MeasurementType.BloodPressure : MeasurementType.Weight,
                    OccurredAt = time.AddHours(index), Systolic = index < 2 ? 120 + index : null, Diastolic = index < 2 ? 80 : null,
                    Value = index == 2 ? 10 : null })).GetAwaiter().GetResult();
            using var shell = new MainForm(topics, CreateEntryService(), CreateSourceService(), service, CreateSessionService(), CreateHealthActionService());
            ShowOffScreen(shell); shell.Activate(); NavigateLifecycle(shell, "_measurementsButton");
            var view = Field<MeasurementsView>(shell, "_measurementsView");
            var grid = Field<DataGridView>(view, "_grid");
            Field<ComboBox>(view, "_typeFilter").SelectedIndex = (int)MeasurementType.BloodPressure + 1;
            Guid target = view.SelectedMeasurement!.Id;
            // No sort, Asc, Desc, then Original. Each iteration edits the same stable ID.
            for (int state = 0; state < 4; state++)
            {
                if (state > 0) Header(grid, 0);
                var expectedGlyph = state == 1 ? SortOrder.Ascending : state == 2 ? SortOrder.Descending : SortOrder.None;
                var before = Task.Run(() => counted.GetAllAsync()).GetAwaiter().GetResult();
                var original = before.Single(item => item.Id == target);
                string primary = Path.Combine(root, "measurements.json"), backup = Path.Combine(root, "measurements.backup.json");
                byte[] previousPrimary = File.ReadAllBytes(primary);
                var otherFiles = Directory.GetFiles(root).Where(path => path != primary && path != backup).ToDictionary(path => path, File.ReadAllBytes);
                int updateCount = counted.Updates;
                var edit = Field<Button>(view, "_editButton");
                bool disabledInsideBinding = false;
                EventHandler commandState = (_, _) =>
                {
                    var sorter = Field<object>(view, "_sort");
                    if (!edit.Enabled && (bool)sorter.GetType().GetProperty("IsRebinding")!.GetValue(sorter)!) disabledInsideBinding = true;
                };
                edit.EnabledChanged += commandState;
                Assert(edit.Focus() && edit.Focused, "Edit command must own native focus before opening the dialog.");
                LifecycleDialog<CreateMeasurementForm>(edit, dialog =>
                {
                    var topicChoice = Field<ComboBox>(dialog, "_topicComboBox");
                    topicChoice.SelectedIndex = state % 2 == 0 ? 1 : 0;
                    Field<Button>(dialog, "_saveButton").Focus();
                    Field<Button>(dialog, "_saveButton").PerformClick();
                });
                Guid? expectedTopic = state % 2 == 0 ? topic.Id : null;
                PumpUntil(() => counted.Updates == updateCount + 1 && (errorDialog || reentrancy is not null || view.SelectedMeasurement?.ModifiedAt > original.ModifiedAt), "focused measurement edit refresh");
                System.Windows.Forms.Application.DoEvents();
                edit.EnabledChanged -= commandState;
                var after = Task.Run(() => counted.GetAllAsync()).GetAwaiter().GetResult();
                var changed = after.Single(item => item.Id == target);
                Assert(counted.Updates == updateCount + 1 && after.Count == before.Count && after.Select(item => item.Id).Distinct().Count() == after.Count,
                    "Measurement edit was duplicated.");
                Assert(changed.HealthTopicId == expectedTopic && changed.Id == original.Id && changed.CreatedAt == original.CreatedAt
                    && changed.ModifiedAt > original.ModifiedAt, "Edit changed identity/timestamps or lost topic.");
                Assert(after.Where(item => item.Id != target).SequenceEqual(before.Where(item => item.Id != target)), "Edit changed another measurement.");
                Assert(File.ReadAllBytes(backup).SequenceEqual(previousPrimary), "Backup does not match the single preceding transaction.");
                Assert(otherFiles.All(file => File.ReadAllBytes(file.Key).SequenceEqual(file.Value)) && Directory.GetFiles(root).Length == otherFiles.Count + 2,
                    "Edit wrote another store or left transaction artifacts.");
                // Verify readable JSON through a NEW repository, not a cached UI result.
                Assert(Task.Run(() => new JsonMeasurementRepository().GetAllAsync()).GetAwaiter().GetResult().SequenceEqual(after), "Persisted JSON is damaged.");
                if (reentrancy is not null) throw new InvalidOperationException("Confirmed save before focus reentrancy: " + reentrancy.StackTrace, reentrancy);
                Assert(!errorDialog, "Unexpected error dialog after successful edit.");
                Assert(!disabledInsideBinding, "Transient binding selection disabled the focused lifecycle command.");
                Assert(view.SelectedMeasurementType == MeasurementType.BloodPressure && view.SelectedMeasurement?.Id == target && grid.Rows.Count == 2,
                    "Focused edit lost filter or ID selection.");
                Assert(grid.Columns[0].HeaderCell.SortGlyphDirection == expectedGlyph, "Edit lost sort state/glyph.");
                if (expectedGlyph != SortOrder.None)
                {
                    var times = grid.Rows.Cast<DataGridViewRow>().Select(row => after.Single(item => item.Id == RowId(row.DataBoundItem)).OccurredAt).ToArray();
                    Assert(times.SequenceEqual(expectedGlyph == SortOrder.Ascending ? times.Order() : times.OrderDescending()), "Edit lost chronological ordering.");
                }
                else
                {
                    var baseline = Task.Run(() => service.GetMeasurementsAsync()).GetAwaiter().GetResult()
                        .Where(item => item.Measurement.MeasurementType == MeasurementType.BloodPressure).Select(item => item.Measurement.Id);
                    Assert(GridIds(grid).SequenceEqual(baseline), "Original did not retain the refreshed presenter order.");
                }
                Assert(edit.Focused || grid.Focused, "Focus lost after modal edit/reload.");
                var bytes = File.ReadAllBytes(primary); WaitForReload(shell);
                Assert(File.ReadAllBytes(primary).SequenceEqual(bytes) && counted.Updates == updateCount + 1, "Refresh repeated the save.");
            }
            // Exercise the legitimate fallback after an edit removes a selected row
            // from the filter, then the genuinely empty last-row case with native focus.
            for (int remaining = 1; remaining >= 0; remaining--)
            {
                var selected = view.SelectedMeasurement!;
                int updates = counted.Updates;
                var edit = Field<Button>(view, "_editButton"); Assert(edit.Focus(), "Filtered edit cannot receive focus.");
                LifecycleDialog<CreateMeasurementForm>(edit, dialog =>
                {
                    Field<ComboBox>(dialog, "_typeComboBox").SelectedIndex = (int)MeasurementType.Weight;
                    Field<TextBox>(dialog, "_valueTextBox").Text = "11";
                    ((Button)dialog.AcceptButton!).Focus(); ((Button)dialog.AcceptButton!).PerformClick();
                });
                PumpUntil(() => reentrancy is not null || errorDialog || grid.Rows.Count == remaining, "focused edit leaving type filter");
                Assert(reentrancy is null && !errorDialog && counted.Updates == updates + 1, "Filtered fallback reentered or repeated save.");
                Assert(view.SelectedMeasurementType == MeasurementType.BloodPressure && view.SelectedMeasurement?.Id != selected.Id,
                    "Excluded measurement retained stale selection/filter.");
                if (remaining == 0) Assert(view.SelectedMeasurement is null && grid.CurrentCell is null
                    && Field<Label>(view, "_emptyStateLabel").Visible && !edit.Enabled, "Last excluded row did not produce a clean empty state.");
                Assert(Descendants(shell).Any(control => control.Focused && control.Visible && control.Enabled), "Filtered fallback lost usable focus.");
            }
            CheckOtherEditFocus(shell, topics, time, () => reentrancy is not null || errorDialog);
            Assert(reentrancy is null && !errorDialog, "Cross-view focused edit/refresh reported an exception.");
            Console.WriteLine("Focused blood-pressure edit/save/refresh integrity and four sort states passed: " + language);
        }
        finally
        {
            AppDomain.CurrentDomain.FirstChanceException -= observe;
            System.Windows.Forms.Application.ThreadException -= threadError;
            Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, priorPath);
        }
    }

    private static void CheckOtherEditFocus(MainForm shell, HealthTopicService topics, DateTimeOffset time, Func<bool> failed)
    {
        Task.Run(() => topics.CreateTopicAsync(new() { Title = "CODEX TEST – second focus topic" })).GetAwaiter().GetResult();
        NavigateLifecycle(shell, "_healthTopicsButton");
        var topicView = Field<HealthTopicsView>(shell, "_topicsView"); var topicGrid = Field<DataGridView>(topicView, "_grid");
        Header(topicGrid, 0); var topicId = topicView.SelectedTopic!.Id;
        var topicEdit = Field<Button>(topicView, "_editButton"); Assert(topicEdit.Focus(), "Topic edit focus missing.");
        LifecycleDialog<EditHealthTopicForm>(topicEdit, dialog =>
        { Field<TextBox>(dialog, "_titleTextBox").Text = "CODEX TEST – Z edited topic"; ((Button)dialog.AcceptButton!).Focus(); ((Button)dialog.AcceptButton!).PerformClick(); });
        PumpUntil(() => failed() || topicView.SelectedTopic?.Title == "CODEX TEST – Z edited topic", "focused topic edit reload");
        WaitForReload(shell);
        Assert(!failed() && topicView.SelectedTopic?.Id == topicId && topicGrid.Columns[0].HeaderCell.SortGlyphDirection == SortOrder.Ascending,
            "Topic focused edit/refresh lost ID/sort or reentered.");

        var sessions = CreateSessionService();
        for (int i = 0; i < 2; i++) Task.Run(() => sessions.CreateSessionAsync(new(time.AddHours(i), "CODEX TEST – focus session " + i))).GetAwaiter().GetResult();
        NavigateLifecycle(shell, "_sessionsButton");
        var sessionView = Field<SessionsView>(shell, "_sessionsView"); var sessionGrid = Field<DataGridView>(sessionView, "_sessionsGrid");
        Header(sessionGrid, 1); var sessionId = sessionView.SelectedSession!.Id;
        var sessionEdit = Field<Button>(sessionView, "_editButton"); Assert(sessionEdit.Focus(), "Session edit focus missing.");
        LifecycleDialog<CreateSessionForm>(sessionEdit, dialog =>
        { Field<TextBox>(dialog, "_titleTextBox").Text = "CODEX TEST – Z edited session"; ((Button)dialog.AcceptButton!).Focus(); ((Button)dialog.AcceptButton!).PerformClick(); });
        PumpUntil(() => failed() || sessionView.SelectedSession?.Title == "CODEX TEST – Z edited session", "focused session edit reload");
        WaitForReload(shell);
        Assert(!failed() && sessionView.SelectedSession?.Id == sessionId && sessionGrid.Columns[1].HeaderCell.SortGlyphDirection == SortOrder.Ascending,
            "Session focused edit/refresh lost ID/sort or reentered.");

        var actions = CreateHealthActionService();
        for (int i = 0; i < 2; i++) Task.Run(() => actions.CreateActionAsync(new("CODEX TEST – focus action " + i))).GetAwaiter().GetResult();
        NavigateLifecycle(shell, "_actionsButton");
        var actionView = Field<HealthActionsView>(shell, "_actionsView"); var actionGrid = Field<DataGridView>(actionView, "_actionsGrid");
        Header(actionGrid, 0); var actionId = actionView.SelectedAction!.Id;
        var actionEdit = Field<Button>(actionView, "_editButton"); Assert(actionEdit.Focus(), "Action edit focus missing.");
        LifecycleDialog<CreateHealthActionForm>(actionEdit, dialog =>
        { Field<TextBox>(dialog, "_titleTextBox").Text = "CODEX TEST – Z edited action"; ((Button)dialog.AcceptButton!).Focus(); ((Button)dialog.AcceptButton!).PerformClick(); });
        PumpUntil(() => failed() || actionView.SelectedAction?.Title == "CODEX TEST – Z edited action", "focused action edit reload");
        WaitForReload(shell);
        Assert(!failed() && actionView.SelectedAction?.Id == actionId && actionGrid.Columns[0].HeaderCell.SortGlyphDirection == SortOrder.Ascending,
            "Action focused edit/refresh lost ID/sort or reentered.");

        // Sources currently has no edit command/service. Exercise the existing real
        // location dialog and a full source rebind with the returned command focus.
        var sources = CreateSourceService();
        for (int i = 0; i < 2; i++) Task.Run(() => sources.CreateSourceAsync(new() { SourceType = SourceType.Book, Title = "CODEX TEST – focus source " + i })).GetAwaiter().GetResult();
        NavigateLifecycle(shell, "_sourcesButton");
        var sourceView = Field<SourcesView>(shell, "_sourcesView"); var sourceGrid = Field<DataGridView>(sourceView, "_sourcesGrid");
        Header(sourceGrid, 0); var sourceId = sourceView.SelectedSource!.Id;
        var location = Field<Button>(sourceView, "_newLocationButton"); Assert(location.Focus(), "Source location command focus missing.");
        LifecycleDialog<CreateSourceLocationForm>(location, dialog =>
        { Field<TextBox>(dialog, "_locatorTextBox").Text = "CODEX TEST – focus locator"; ((Button)dialog.AcceptButton!).Focus(); ((Button)dialog.AcceptButton!).PerformClick(); });
        PumpUntil(() => failed() || Field<DataGridView>(sourceView, "_locationsGrid").Rows.Count == 1, "focused source location reload");
        WaitForReload(shell);
        Assert(!failed() && sourceView.SelectedSource?.Id == sourceId && sourceGrid.Columns[0].HeaderCell.SortGlyphDirection == SortOrder.Ascending,
            "Source focused modal return/refresh lost ID/sort or reentered.");
    }

    private sealed class CountedMeasurements(IMeasurementRepository inner) : IMeasurementRepository
    {
        public int Updates { get; private set; }
        public Task<IReadOnlyList<Measurement>> GetAllAsync(CancellationToken token = default) => inner.GetAllAsync(token);
        public Task AddAsync(Measurement item, CancellationToken token = default) => inner.AddAsync(item, token);
        public async Task UpdateAsync(Measurement item, DateTimeOffset expected, CancellationToken token = default)
        { await inner.UpdateAsync(item, expected, token); Updates++; }
        public Task DeleteAsync(Guid id, DateTimeOffset expected, CancellationToken token = default) => inner.DeleteAsync(id, expected, token);
    }
}
