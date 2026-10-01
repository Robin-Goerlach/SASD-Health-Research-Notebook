using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Infrastructure.Storage;
using Sasd.HealthNotebook.WinForms.Controls;
using Sasd.HealthNotebook.WinForms.Forms;
using Sasd.HealthNotebook.WinForms.Localization;
using Sasd.HealthNotebook.WinForms.Views;

namespace Sasd.HealthNotebook.WinForms.SmokeTests;

internal static partial class Program
{
    private static SourceService CreateSourceService() => new(new JsonSourceRepository(), new JsonHealthTopicRepository());

    // UI-SRC-001: actual navigation/actions/dialogs, both languages, synthetic relationships and full reload.
    private static void CheckSources(HealthTopicService topics, string testPath, UiLanguage language)
    {
        var service = CreateSourceService();
        int before = service.GetSourcesAsync().GetAwaiter().GetResult().Count;
        byte[] topicsBefore = File.ReadAllBytes(LocalHealthNotebookPaths.HealthTopicsFilePath);
        byte[] entriesBefore = File.ReadAllBytes(LocalHealthNotebookPaths.HealthEntriesFilePath);
        using var form = new MainForm(topics, CreateEntryService(), service, CreateMeasurementService(), CreateSessionService());
        ShowOffScreen(form); form.Size = form.MinimumSize;
        var navigation = Field<NavigationControl>(form, "_navigation");
        var sourcesButton = Field<NavigationButton>(navigation, "_sourcesButton");
        Assert(sourcesButton.Text == AppStrings.Sources, "Source navigation text missing.");
        sourcesButton.PerformClick(); WaitForReload(form);
        var view = Field<SourcesView>(form, "_sourcesView");
        var sourceGrid = Field<DataGridView>(view, "_sourcesGrid");
        Assert(sourceGrid.Rows.Count == before && sourcesButton.IsSelected, "Source navigation load failed.");
        Assert(before != 0 || Field<Label>(view, "_emptyStateLabel").Visible, "Source empty state missing.");
        Assert(before != 0 || !Field<Button>(view, "_newLocationButton").Enabled, "Cannot add location without source.");
        Button action = Field<Button>(form, "_newTopicButton");
        Assert(action.Text == AppStrings.NewSource, "Source main action missing.");
        AssertWithinParent(action); AssertTextFits(action);
        string title = "CODEX TEST – Synthetic Research Source " + language;
        RunSourceDialog<CreateSourceForm>(action, dialog =>
        {
            CheckSourceDialogLayout(dialog, new[] { "_typeComboBox", "_titleTextBox", "_urlTextBox", "_authorTextBox",
                "_publicationPicker", "_accessedPicker", "_identifierTextBox", "_topicComboBox" });
            Field<Button>(dialog, "_saveButton").PerformClick();
            Assert(Field<Label>(dialog, "_validationLabel").Text == AppStrings.SourceValidationFailed, "Empty source not rejected.");
            Field<TextBox>(dialog, "_titleTextBox").Text = title;
            Field<TextBox>(dialog, "_urlTextBox").Text = "https://example.invalid/synthetic";
            Field<TextBox>(dialog, "_authorTextBox").Text = "Synthetic institution";
            Field<TextBox>(dialog, "_identifierTextBox").Text = "Synthetic ID";
            Field<ComboBox>(dialog, "_typeComboBox").SelectedIndex = 3;
            Field<ComboBox>(dialog, "_topicComboBox").SelectedIndex = language == UiLanguage.German ? 1 : 0;
            Assert(!Field<DateTimePicker>(dialog, "_publicationPicker").Checked
                && !Field<DateTimePicker>(dialog, "_accessedPicker").Checked, "Unknown source dates must stay unset.");
            Field<DateTimePicker>(dialog, "_publicationPicker").Value = new DateTime(2026, 9, 1);
            Field<DateTimePicker>(dialog, "_publicationPicker").Checked = true;
            Capture(dialog, Path.Combine(testPath, $"source-dialog-{language}.png"));
        });
        PumpUntil(() => sourceGrid.Rows.Count == before + 1 && view.SelectedSource?.Title == title, "new source selection");
        var source = view.SelectedSource!;
        Assert(source.HealthTopicId.HasValue == (language == UiLanguage.German), "Source optional topic failed.");
        Assert(source.PublicationDate == new DateOnly(2026, 9, 1) && source.AccessedAt is null, "Optional calendar dates failed.");
        var locationGrid = Field<DataGridView>(view, "_locationsGrid");
        Button locationAction = Field<Button>(view, "_newLocationButton");
        AssertWithinParent(locationAction); AssertTextFits(locationAction);
        RunSourceDialog<CreateSourceLocationForm>(locationAction, dialog =>
        {
            CheckSourceDialogLayout(dialog, new[] { "_typeComboBox", "_locatorTextBox", "_noteTextBox" });
            Field<Button>(dialog, "_saveButton").PerformClick();
            Assert(Field<Label>(dialog, "_validationLabel").Text == AppStrings.SourceValidationFailed, "Empty locator not rejected.");
            Field<TextBox>(dialog, "_locatorTextBox").Text = "Page 17";
            Field<TextBox>(dialog, "_noteTextBox").Text = "Synthetic locator note.";
            Capture(dialog, Path.Combine(testPath, $"location-dialog-{language}.png"));
        });
        PumpUntil(() => locationGrid.Rows.Count == 1, "immediate location refresh");
        var tabs = Descendants(view).OfType<TabControl>().Single();
        tabs.SelectedIndex = 1; System.Windows.Forms.Application.DoEvents();
        Button noteAction = Field<Button>(view, "_newNoteButton");
        var notesGrid = Field<DataGridView>(view, "_notesGrid");
        Assert(Field<Label>(view, "_notesEmptyLabel").Visible, "Note empty state missing.");
        AssertWithinParent(noteAction); AssertTextFits(noteAction);
        RunSourceDialog<CreateEvidenceNoteForm>(noteAction, dialog =>
        {
            CheckSourceDialogLayout(dialog, new[] { "_locationComboBox", "_statementTextBox", "_excerptTextBox", "_assessmentTextBox" });
            Field<Button>(dialog, "_saveButton").PerformClick();
            Assert(Field<Label>(dialog, "_validationLabel").Text == AppStrings.SourceValidationFailed, "Empty source note not rejected.");
            Assert(Field<ComboBox>(dialog, "_locationComboBox").Items.Count == 2, "Wrong source's locations offered.");
            Field<ComboBox>(dialog, "_locationComboBox").SelectedIndex = 1;
            Field<TextBox>(dialog, "_statementTextBox").Text = "Synthetic statement for automated verification.";
            Field<TextBox>(dialog, "_excerptTextBox").Text = "Synthetic excerpt.";
            Field<TextBox>(dialog, "_assessmentTextBox").Text = "Synthetic personal summary for test purposes only.";
            Capture(dialog, Path.Combine(testPath, $"source-note-dialog-{language}.png"));
        });
        PumpUntil(() => notesGrid.Rows.Count == 1, "immediate note refresh");
        var persistedNotes = service.GetNotesAsync(source.Id).GetAwaiter().GetResult();
        var persistedLocations = service.GetLocationsAsync(source.Id).GetAwaiter().GetResult();
        Assert(persistedNotes.Single().SourceLocationId == persistedLocations.Single().Id
            && persistedNotes.Single().Excerpt == "Synthetic excerpt."
            && persistedNotes.Single().OwnParaphraseOrAssessment == "Synthetic personal summary for test purposes only.", "UI note relationships or text separation failed.");
        string detailText = Field<TextBox>(view, "_noteDetails").Text;
        Assert(detailText.Contains(AppStrings.SourceStatement) && detailText.Contains(AppStrings.SourceExcerpt)
            && detailText.Contains(AppStrings.SourceAssessment) && detailText.Contains("Synthetic excerpt."), "Note details lack separate labels or full text.");
        var languageSelector = Field<ComboBox>(form, "_languageComboBox");
        foreach (int index in new[] { language == UiLanguage.German ? 1 : 0, language == UiLanguage.German ? 0 : 1 })
        {
            languageSelector.SelectedIndex = index; WaitForReload(form);
            Assert(action.Text == AppStrings.NewSource && sourcesButton.Text == AppStrings.Sources
                && noteAction.Text == AppStrings.NewSourceNote, "Sources language switch failed.");
            Assert(view.SelectedSource?.Id == source.Id && notesGrid.Rows.Count == 1, "Source refresh lost selection/dependents.");
        }
        foreach (var grid in new[] { sourceGrid, notesGrid })
            Assert(grid.Columns.Cast<DataGridViewColumn>().Sum(column => column.Width) <= grid.ClientSize.Width, "Source grid columns clip at minimum size.");
        Capture(form, Path.Combine(testPath, $"sources-notes-{language}-minimum.png"));
        tabs.SelectedIndex = 0; System.Windows.Forms.Application.DoEvents();
        Assert(locationGrid.Columns.Cast<DataGridViewColumn>().Sum(column => column.Width) <= locationGrid.ClientSize.Width, "Location columns clip.");
        Capture(form, Path.Combine(testPath, $"sources-locations-{language}-minimum.png"));
        CheckSourceDetailLayout(form, view, testPath, language);
        form.Close();
        using var restarted = new MainForm(new HealthTopicService(new JsonHealthTopicRepository()), CreateEntryService(), CreateSourceService(), CreateMeasurementService(), CreateSessionService());
        ShowOffScreen(restarted);
        Field<NavigationButton>(Field<NavigationControl>(restarted, "_navigation"), "_sourcesButton").PerformClick(); WaitForReload(restarted);
        var reloadedView = Field<SourcesView>(restarted, "_sourcesView");
        Assert(Field<DataGridView>(reloadedView, "_sourcesGrid").Rows.Count == before + 1 && reloadedView.SelectedSource?.Id == source.Id,
            "Sources not persisted after shell restart.");
        Assert(Field<DataGridView>(reloadedView, "_locationsGrid").Rows.Count == 1 && Field<DataGridView>(reloadedView, "_notesGrid").Rows.Count == 1,
            "Source relationships not reloaded.");
        restarted.Close();
        Assert(topicsBefore.SequenceEqual(File.ReadAllBytes(LocalHealthNotebookPaths.HealthTopicsFilePath)), "Source UI modified topic JSON.");
        Assert(entriesBefore.SequenceEqual(File.ReadAllBytes(LocalHealthNotebookPaths.HealthEntriesFilePath)), "Source UI modified entry JSON.");
    }
    private static void CheckSourceDialogLayout(SourceRecordForm dialog, string[] fields)
    {
        dialog.Size = dialog.MinimumSize; System.Windows.Forms.Application.DoEvents();
        for (int i = 0; i < fields.Length; i++)
        {
            var editor = Field<Control>(dialog, fields[i]); AssertWithinParent(editor);
            Assert(editor.TabIndex == i && !string.IsNullOrWhiteSpace(editor.AccessibleName), "Source dialog tab order/accessibility failed.");
            Assert(!string.IsNullOrWhiteSpace(Field<ToolTip>(dialog, "_toolTip").GetToolTip(editor)), "Source tooltip missing.");
        }
        foreach (var label in Descendants(dialog).OfType<Label>()) AssertTextFits(label);
        var save = Field<Button>(dialog, "_saveButton"); AssertWithinParent(save); AssertTextFits(save);
        Assert(dialog.AcceptButton == save && dialog.CancelButton is not null, "Source dialog Enter/Escape missing.");
    }
    // UI-SRC-LAYOUT-001: aligned proportional details at normal/minimum size, both tabs and languages.
    private static void CheckSourceDetailLayout(MainForm form, SourcesView view, string testPath, UiLanguage language)
    {
        var tabs = Descendants(view).OfType<TabControl>().Single();
        var sourceDetails = Field<TextBox>(view, "_sourceDetails");
        foreach (Size size in new[] { new Size(1280, 820), form.MinimumSize })
        {
            form.Size = size;
            foreach (int tab in new[] { 0, 1 })
            {
                tabs.SelectedIndex = tab;
                System.Windows.Forms.Application.DoEvents();
                var right = Field<TextBox>(view, tab == 0 ? "_locationDetails" : "_noteDetails");
                var inactive = Field<TextBox>(view, tab == 0 ? "_noteDetails" : "_locationDetails");
                Assert(sourceDetails.Visible && right.Visible && !inactive.Visible, "Source detail tab visibility changed.");
                AssertWithinParent(sourceDetails); AssertWithinParent(right);
                Point leftTop = view.PointToClient(sourceDetails.PointToScreen(Point.Empty));
                Point rightTop = view.PointToClient(right.PointToScreen(Point.Empty));
                Assert(Math.Abs(sourceDetails.Height - right.Height) <= 1 && Math.Abs(leftTop.Y - rightTop.Y) <= 1,
                    "Source detail areas must have equal heights and aligned tops.");
                AssertWithinParent(Field<Button>(form, "_newTopicButton"));
                Capture(form, Path.Combine(testPath, $"sources-aligned-{language}-{size.Width}-tab{tab}.png"));
            }
        }
    }
    private static void RunSourceDialog<T>(Button action, Action<T> fill) where T : SourceRecordForm
    {
        bool closed = false;
        Exception? failure = null;
        using var timer = new System.Windows.Forms.Timer { Interval = 30 };
        timer.Tick += (_, _) =>
        {
            var dialog = System.Windows.Forms.Application.OpenForms.OfType<T>().SingleOrDefault();
            if (dialog is null) return;
            timer.Stop();
            try
            {
                fill(dialog); dialog.FormClosed += (_, _) => closed = true;
                Field<Button>(dialog, "_saveButton").PerformClick();
            }
            catch (Exception ex) { failure = ex; dialog.Close(); closed = true; }
        };
        timer.Start(); action.PerformClick();
        PumpUntil(() => closed, "source dialog create");
        if (failure is not null) throw new InvalidOperationException(failure.Message, failure);
    }
}
