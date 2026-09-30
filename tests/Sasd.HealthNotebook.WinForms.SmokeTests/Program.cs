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
internal static class Program
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
                        var service = new HealthTopicService(new JsonHealthTopicRepository());
                        CheckShell(service, testPath, language);
                        CheckWizard(service, testPath, language);
                    }
                }
                catch (Exception ex) { uiFailure = ex; }
                finally { host.Close(); }
            };
            System.Windows.Forms.Application.Run(host);
            if (uiFailure is not null) { throw new InvalidOperationException(uiFailure.Message, uiFailure); }
            Console.WriteLine("WinForms smoke tests passed (FR-UI-001).");
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
        using var form = new MainForm(service);
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
        using var restarted = new MainForm(new HealthTopicService(new JsonHealthTopicRepository()));
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

    private static T Field<T>(object target, string name) where T : class =>
        (T)(target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(target)
            ?? throw new InvalidOperationException($"Missing UI field {name}."));

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
        Assert(measured.Height <= available.Height, $"Text is clipped in {control.GetType().Name}.");
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
