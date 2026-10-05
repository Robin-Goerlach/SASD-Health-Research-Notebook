using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.Infrastructure.Storage;
using Sasd.HealthNotebook.WinForms.Controls;
using Sasd.HealthNotebook.WinForms.Forms;
using Sasd.HealthNotebook.WinForms.Localization;
using Sasd.HealthNotebook.WinForms.Presentation;
using Sasd.HealthNotebook.WinForms.Views;

namespace Sasd.HealthNotebook.WinForms.SmokeTests;

/// <summary>FR-UI-001: integration checks against real WinForms controls, without a test framework.</summary>
internal static partial class Program
{
    [STAThread]
    private static int Main()
    {
        string? originalPath = Environment.GetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable);
        try
        {
            string repositoryRoot = Environment.CurrentDirectory;
            Assert(File.Exists(Path.Combine(repositoryRoot, "Sasd.HealthNotebook.sln")), "Run from repository root.");
            string safeRoot = Path.Combine(repositoryRoot, ".codex");
            string configured = originalPath ?? throw new InvalidOperationException("UI tests require an explicit safe data override.");
            string fullPath = Path.GetFullPath(configured);
            Assert(fullPath.StartsWith(safeRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase),
                "UI test data must be under repository .codex.");
            for (DirectoryInfo? ancestor = new(fullPath); ancestor is not null; ancestor = ancestor.Parent)
            {
                Assert(!ancestor.Exists || (ancestor.Attributes & FileAttributes.ReparsePoint) == 0, "No links in UI test path.");
            }
            string testPath = Path.Combine(fullPath, "ui-tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(testPath);
            Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, testPath);
            Console.WriteLine($"Synthetic UI test data: {testPath}");
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            // Keep one message-loop owner alive across recreated forms. Otherwise
            // async UI continuations may target a closed harness window.
            Exception? uiFailure = null;
            using var host = new Form { ShowInTaskbar = false, StartPosition = FormStartPosition.Manual,
                Location = new Point(-20000, -20000), Size = new Size(1, 1) };
            host.Shown += (_, _) =>
            {
                try
                {
                    foreach (UiLanguage language in Enum.GetValues<UiLanguage>())
                    {
                        AppLanguage.SetLanguage(language);
                        CheckParentLifecycle(testPath, language);
                        CheckMeasurementFields(testPath, language);
                        CheckMeasurementTypeFilter(testPath, language);
                        CheckLifecycle(testPath, language);
                        var service = new HealthTopicService(new JsonHealthTopicRepository());
                        CheckShell(service, testPath, language);
                        CheckWizard(service, testPath, language);
                        CheckTimeline(service, testPath, language);
                        CheckSources(service, testPath, language);
                        CheckSourceLocationRestart(testPath, language);
                        CheckMeasurements(service, testPath, language);
                        CheckSessions(service, testPath, language);
                        CheckHealthActions(testPath, language);
                        CheckDashboardAgenda(testPath, language);
                    }
                }
                catch (Exception ex) { uiFailure = ex; }
                finally { host.Close(); }
            };
            System.Windows.Forms.Application.Run(host);
            if (uiFailure is not null) { throw new InvalidOperationException(uiFailure.Message, uiFailure); }
            Console.WriteLine("WinForms smoke tests passed (FR-UI-001, FR-OBS-001, FR-SRC-001/002/004/005/006, FR-MEA-001/002/004/005/007, FR-SES-001/002/003/004/007/008).");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"WinForms smoke tests failed: {ex.Message}");
            return 1;
        }
        finally
        {
            Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, originalPath);
        }
    }

    // UI-LAYOUT-001: header/cards/grid at start and minimum sizes, in both languages.
    private static void CheckShell(HealthTopicService service, string testPath, UiLanguage language)
    {
        using var form = new MainForm(service, CreateEntryService(), CreateSourceService(), CreateMeasurementService(), CreateSessionService(), CreateHealthActionService());
        ShowOffScreen(form);
        PumpUntil(() => Field<ToolStripStatusLabel>(form, "_statusLabel").Text != AppStrings.Ready, "initial shell load");
        foreach (Size size in new[] { new Size(1280, 820), form.MinimumSize })
        {
            form.Size = size;
            System.Windows.Forms.Application.DoEvents();
            Capture(form, Path.Combine(testPath, $"dashboard-{language}-{size.Width}.png"));
            AssertWithinParent(Field<Button>(form, "_newTopicButton"));
            AssertWithinParent(Field<Button>(form, "_refreshButton"));
            AssertTextFits(Field<Button>(form, "_newTopicButton"));
            foreach (DashboardCardControl card in Descendants(form).OfType<DashboardCardControl>())
            {
                AssertWithinParent(card);
                foreach (Label label in card.Controls.OfType<Label>()) { AssertTextFits(label); }
            }
        }
        var navigation = Field<NavigationControl>(form, "_navigation");
        var dashboardButton = Field<NavigationButton>(navigation, "_dashboardButton");
        var topicsButton = Field<NavigationButton>(navigation, "_healthTopicsButton");
        Control dashboardPage = Field<Control>(form, "_dashboardPage");
        dashboardButton.Focus();
        typeof(Control).GetMethod("OnKeyDown", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(dashboardButton, new object[] { new KeyEventArgs(Keys.Down) });
        Assert(topicsButton.Focused && dashboardButton.IsSelected, "Arrow navigation must move focus without changing page.");
        topicsButton.PerformClick();
        Assert(topicsButton.IsSelected && !dashboardButton.IsSelected, "Topic navigation selection failed.");
        dashboardButton.PerformClick();
        Assert(dashboardPage.Parent is not null, "Navigation must reuse the dashboard page.");
        WaitForReload(form);
        var topicsView = Field<HealthTopicsView>(form, "_dashboardTopicsView");
        var grid = Field<DataGridView>(topicsView, "_grid");
        topicsView.SetTopics(Array.Empty<HealthTopicSummary>());
        Assert(Field<Label>(topicsView, "_emptyStateLabel").Visible && !grid.Visible, "Empty state must replace blank grid.");
        var summaries = new[]
        {
            new HealthTopicSummary { Id = Guid.NewGuid(), Title = "Synthetic layout topic A", CreatedAt = DateTimeOffset.Now },
            new HealthTopicSummary { Id = Guid.NewGuid(), Title = "Synthetic layout topic B", CreatedAt = DateTimeOffset.Now,
                Priority = HealthTopicPriority.PrepareForDoctor, ShortDescription = "Synthetic description to verify wrapping and readability." }
        };
        topicsView.SetTopics(summaries);
        grid.CurrentCell = grid.Rows[1].Cells[0];
        topicsView.SetTopics(summaries.AsEnumerable().Reverse().ToArray());
        Assert(((HealthTopicGridRow)grid.CurrentRow!.DataBoundItem).Id == summaries[1].Id, "Refresh lost topic selection.");
        Assert(grid.Columns.Cast<DataGridViewColumn>().Sum(column => column.Width) <= grid.ClientSize.Width,
            "Grid needs horizontal scrolling at the minimum window size.");
        Capture(form, Path.Combine(testPath, $"topics-{language}-minimum.png"));
        var languageSelector = Field<ComboBox>(form, "_languageComboBox");
        languageSelector.SelectedIndex = language == UiLanguage.German ? 1 : 0;
        Assert(Field<Button>(form, "_newTopicButton").Text == AppStrings.NewHealthTopic, "Language change did not update actions.");
        languageSelector.SelectedIndex = language == UiLanguage.German ? 0 : 1;
        WaitForReload(form);
        form.Close();
    }

    // UI-WIZARD-001: basic editors, keyboard order and real create/reload through shared JSON.
    private static void CheckWizard(HealthTopicService service, string testPath, UiLanguage language)
    {
        using var wizard = new CreateHealthTopicWizardForm(service);
        wizard.Size = wizard.MinimumSize;
        ShowOffScreen(wizard);
        TextBox title = Field<TextBox>(wizard, "_titleTextBox");
        foreach (string field in new[] { "_titleTextBox", "_statusComboBox", "_priorityComboBox", "_shortDescriptionTextBox" })
        {
            AssertWithinParent(Field<Control>(wizard, field));
        }
        Assert(title.TabIndex < Field<ComboBox>(wizard, "_statusComboBox").TabIndex, "Title must precede status in tab order.");
        AssertWithinParent(Field<Button>(wizard, "_nextButton"));
        Assert(wizard.AcceptButton == Field<Button>(wizard, "_nextButton") && wizard.CancelButton is not null,
            "Wizard Enter/Escape commands are missing.");
        Capture(wizard, Path.Combine(testPath, $"wizard-{language}-minimum.png"));
        string syntheticTitle = $"Synthetic UI workflow {language}";
        title.Text = syntheticTitle;
        var steps = Field<ListBox>(wizard, "_stepsListBox");
        steps.SelectedIndex = steps.Items.Count - 1;
        Field<Button>(wizard, "_nextButton").PerformClick();
        PumpUntil(() => wizard.IsDisposed || wizard.DialogResult == DialogResult.OK, "wizard save");
        var reloaded = new HealthTopicService(new JsonHealthTopicRepository()).GetTopicSummariesAsync().GetAwaiter().GetResult();
        Assert(reloaded.Any(topic => topic.Title == syntheticTitle), "Wizard topic did not survive repository recreation.");
        using var restarted = new MainForm(new HealthTopicService(new JsonHealthTopicRepository()), CreateEntryService(), CreateSourceService(), CreateMeasurementService(), CreateSessionService(), CreateHealthActionService());
        ShowOffScreen(restarted);
        WaitForReload(restarted);
        var reloadedGrid = Field<DataGridView>(Field<HealthTopicsView>(restarted, "_dashboardTopicsView"), "_grid");
        Assert(reloadedGrid.Rows.Count == reloaded.Count, "Recreated shell did not load persisted wizard topics.");
        Capture(restarted, Path.Combine(testPath, $"after-create-{language}.png"));
        restarted.Close();
    }

    private static void WaitForReload(MainForm form)
    {
        Task reload = (Task)form.GetType().GetMethod("ReloadSafeAsync", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(form, null)!;
        PumpUntil(() => reload.IsCompleted, "shell refresh");
        reload.GetAwaiter().GetResult();
    }

    private static HealthEntryService CreateEntryService() => new(new JsonHealthEntryRepository(), new JsonHealthTopicRepository());

    // FR-OBS-001 / UI-ENTRY-001: real navigation/action/dialog, both languages, isolated JSON reload.
    private static void CheckTimeline(HealthTopicService topics, string testPath, UiLanguage language)
    {
        var entries = CreateEntryService();
        int beforeCount = entries.GetEntriesAsync().GetAwaiter().GetResult().Count;
        using var form = new MainForm(topics, entries, CreateSourceService(), CreateMeasurementService(), CreateSessionService(), CreateHealthActionService());
        ShowOffScreen(form);
        form.Size = form.MinimumSize;
        var navigation = Field<NavigationControl>(form, "_navigation");
        var timelineButton = Field<NavigationButton>(navigation, "_timelineButton");
        Assert(timelineButton.Text == AppStrings.Timeline, "Timeline navigation localization failed.");
        timelineButton.PerformClick();
        WaitForReload(form);
        var view = Field<TimelineView>(form, "_timelineView");
        var grid = Field<DataGridView>(view, "_grid");
        Assert(grid.Rows.Count == beforeCount, "Timeline initial load failed.");
        Assert(beforeCount != 0 || Field<Label>(view, "_emptyStateLabel").Visible, "Timeline empty state is missing.");
        Button action = Field<Button>(form, "_newTopicButton");
        Assert(action.Text == AppStrings.NewTimelineEntry, "Timeline main action failed.");
        AssertWithinParent(action); AssertTextFits(action);
        Capture(form, Path.Combine(testPath, $"timeline-{language}-before.png"));
        bool dialogClosed = false;
        string syntheticTitle = $"CODEX TEST – Timeline Entry {language}";
        using var timer = new System.Windows.Forms.Timer { Interval = 30 };
        timer.Tick += (_, _) =>
        {
            var dialog = System.Windows.Forms.Application.OpenForms.OfType<CreateHealthEntryForm>().SingleOrDefault();
            if (dialog is null) return;
            timer.Stop();
            dialog.Size = dialog.MinimumSize;
            foreach (string field in new[] { "_datePicker", "_timePicker", "_typeComboBox", "_topicComboBox", "_titleTextBox", "_contentTextBox", "_saveButton" })
                AssertWithinParent(Field<Control>(dialog, field));
            Assert(Field<TextBox>(dialog, "_titleTextBox").AccessibleName == AppStrings.EntryTitle, "Entry-specific title label missing.");
            int[] tabOrder = new[] { "_datePicker", "_timePicker", "_typeComboBox", "_topicComboBox", "_titleTextBox", "_contentTextBox" }
                .Select(field => Field<Control>(dialog, field).TabIndex).ToArray();
            Assert(tabOrder.SequenceEqual(Enumerable.Range(0, 6)), "Entry field tab order failed.");
            AssertTextFits(Field<Button>(dialog, "_saveButton"));
            Assert(dialog.AcceptButton == Field<Button>(dialog, "_saveButton") && dialog.CancelButton is not null,
                "Entry dialog keyboard commands missing.");
            Field<Button>(dialog, "_saveButton").PerformClick();
            Assert(Field<Label>(dialog, "_validationLabel").Text == AppStrings.EntryValidationFailed,
                "Missing title was not rejected in dialog.");
            Field<TextBox>(dialog, "_titleTextBox").Text = syntheticTitle;
            Field<TextBox>(dialog, "_contentTextBox").Text = "Synthetic notebook entry for automated UI verification.";
            Field<ComboBox>(dialog, "_typeComboBox").SelectedIndex = 1;
            Field<ComboBox>(dialog, "_topicComboBox").SelectedIndex = language == UiLanguage.German ? 1 : 0;
            Capture(dialog, Path.Combine(testPath, $"entry-dialog-{language}-minimum.png"));
            dialog.FormClosed += (_, _) => dialogClosed = true;
            Field<Button>(dialog, "_saveButton").PerformClick();
        };
        timer.Start();
        action.PerformClick();
        PumpUntil(() => dialogClosed && grid.Rows.Count == beforeCount + 1, "entry dialog and immediate timeline refresh");
        var persisted = entries.GetEntriesAsync().GetAwaiter().GetResult();
        Assert(persisted[0].Title == syntheticTitle && persisted[0].EntryType == HealthEntryType.Observation,
            "Entry dialog persisted wrong data.");
        Assert(persisted[0].HealthTopicId.HasValue == (language == UiLanguage.German), "Optional topic selection failed.");
        var displayedEntry = grid.Rows.Cast<DataGridViewRow>().Single(row => Equals(row.Cells[2].Value, syntheticTitle));
        string expectedTopic = persisted[0].HealthTopicTitle ?? AppStrings.NoEntryTopic;
        Assert(Equals(displayedEntry.Cells[3].Value, expectedTopic), "Timeline does not display the resolved topic title.");
        var missingReference = new HealthEntrySummary(Guid.NewGuid(), Guid.NewGuid(), null,
            HealthEntryType.Note, DateTimeOffset.Now, "Synthetic missing-topic entry", "Synthetic content.");
        view.SetEntries(new[] { missingReference });
        Assert(grid.Rows.Count == 1 && Equals(grid.Rows[0].Cells[3].Value, AppStrings.MissingEntryTopic),
            "Missing topic reference is not displayed neutrally.");
        WaitForReload(form);
        Assert(grid.Columns.Cast<DataGridViewColumn>().Sum(column => column.Width) <= grid.ClientSize.Width,
            "Timeline columns overflow minimum size.");
        var selector = Field<ComboBox>(form, "_languageComboBox");
        selector.SelectedIndex = language == UiLanguage.German ? 1 : 0;
        WaitForReload(form);
        Assert(action.Text == AppStrings.NewTimelineEntry && timelineButton.Text == AppStrings.Timeline,
            "Timeline language switch failed.");
        selector.SelectedIndex = language == UiLanguage.German ? 0 : 1;
        WaitForReload(form);
        Capture(form, Path.Combine(testPath, $"timeline-{language}-after.png"));
        form.Close();
        using var restarted = new MainForm(new HealthTopicService(new JsonHealthTopicRepository()), CreateEntryService(), CreateSourceService(), CreateMeasurementService(), CreateSessionService(), CreateHealthActionService());
        ShowOffScreen(restarted);
        Field<NavigationButton>(Field<NavigationControl>(restarted, "_navigation"), "_timelineButton").PerformClick();
        WaitForReload(restarted);
        Assert(Field<DataGridView>(Field<TimelineView>(restarted, "_timelineView"), "_grid").Rows.Count == beforeCount + 1,
            "Timeline did not survive shell/repository recreation.");
        restarted.Close();
    }

    private static void ShowOffScreen(Form form)
    {
        form.StartPosition = FormStartPosition.Manual;
        form.Location = new Point(-20000, -20000);
        form.Show();
        System.Windows.Forms.Application.DoEvents();
    }

    private static void PumpUntil(Func<bool> condition, string operation)
    {
        var timeout = Stopwatch.StartNew();
        while (!condition())
        {
            Assert(timeout.Elapsed < TimeSpan.FromSeconds(15), $"UI operation timed out: {operation}.");
            System.Windows.Forms.Application.DoEvents();
            Thread.Sleep(10);
        }
    }

    private static T Field<T>(object target, string name) where T : class
    {
        for (Type? type = target.GetType(); type is not null; type = type.BaseType)
            if (type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)?.GetValue(target) is T field)
                return field;
        throw new InvalidOperationException($"Missing UI field {name}.");
    }

    private static IEnumerable<Control> Descendants(Control parent) =>
        parent.Controls.Cast<Control>().SelectMany(child => new[] { child }.Concat(Descendants(child)));

    private static void AssertWithinParent(Control control) => Assert(
        control.Parent is not null && control.Parent.ClientRectangle.Contains(control.Bounds),
        $"Clipped {control.GetType().Name}: bounds={control.Bounds}, parent={control.Parent?.ClientRectangle}.");

    private static void AssertTextFits(Control control)
    {
        Size available = new(Math.Max(1, control.ClientSize.Width - control.Padding.Horizontal),
            Math.Max(1, control.ClientSize.Height - control.Padding.Vertical));
        Size measured = TextRenderer.MeasureText(control.Text, control.Font,
            new Size(available.Width, int.MaxValue), TextFormatFlags.WordBreak);
        Assert(measured.Height <= available.Height, $"Text is clipped in {control.GetType().Name}: available={available}, measured={measured}.");
    }

    private static void Capture(Form form, string path)
    {
        using var bitmap = new Bitmap(form.Width, form.Height);
        form.DrawToBitmap(bitmap, new Rectangle(Point.Empty, form.Size));
        bitmap.Save(path);
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition) { throw new InvalidOperationException(message); }
    }
}
