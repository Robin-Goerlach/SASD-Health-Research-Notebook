using System.Text.Json;
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
    // UI-SRC-RELOAD-001: real dialogs, disposed shell, fresh services and reselecting an older source WITHOUT Refresh.
    private static void CheckSourceLocationRestart(string testPath, UiLanguage language)
    {
        string root = Path.Combine(testPath, "source-restart-" + language);
        Directory.CreateDirectory(root);
        Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, root);
        try
        {
            var expected = CreateLocationThroughDialogs();
            byte[] persisted = File.ReadAllBytes(LocalHealthNotebookPaths.SourcesFilePath);
            using (var json = JsonDocument.Parse(persisted))
                Assert(json.RootElement.GetProperty("Locations").EnumerateArray().Single().GetProperty("Id").GetGuid() == expected.Id,
                    "Dialog location was not written to sources.json.");
            // A newer source becomes the default selection after restart. Reselecting the
            // older source must load its dependents without an explicit Refresh to mask events.
            using var restarted = NewSourceShell();
            ShowOffScreen(restarted);
            Field<NavigationButton>(Field<NavigationControl>(restarted, "_navigation"), "_sourcesButton").PerformClick();
            var view = Field<SourcesView>(restarted, "_sourcesView");
            var sources = Field<DataGridView>(view, "_sourcesGrid");
            PumpUntil(() => sources.Rows.Count == 2 && view.SelectedSource is not null, "startup sources load");
            Assert(view.SelectedSource!.Id != expected.SourceId, "Restart fixture must initially select the other source.");
            var locations = Field<DataGridView>(view, "_locationsGrid");
            Guid? notifiedSourceId = null;
            view.SourceSelected += (_, _) => notifiedSourceId = view.SelectedSource?.Id;
            int selectedIndex = Enumerable.Range(0, sources.Rows.Count).Single(index => Equals(sources.Rows[index].Cells[0].Value, "CODEX TEST – Location reload"));
            sources.CurrentCell = sources.Rows[selectedIndex].Cells[0];
            Assert(view.SelectedSource?.Id == expected.SourceId, "Source selection did not change.");
            Assert(notifiedSourceId == expected.SourceId, "SourceSelected notified the previous current row instead of the newly selected source.");
            PumpUntil(() => locations.Rows.Count == 1, "reselected source location after restart without Refresh");
            Assert(locations.Visible && !Field<Label>(view, "_locationsEmptyLabel").Visible
                && Equals(locations.Rows[0].Cells[1].Value, expected.Locator), "Persisted location is hidden by the source empty state.");
            var displayed = Field<IReadOnlyList<SourceLocation>>(view, "_locations").Single();
            Assert(displayed.Id == expected.Id && displayed.SourceId == expected.SourceId
                && displayed.LocationType == expected.LocationType && displayed.Locator == expected.Locator,
                "Restarted view displayed a different location.");
            Capture(restarted, Path.Combine(root, "source-location-reselected.png"));
            // Switching to a source with no locations must also clear the old details.
            sources.CurrentCell = sources.Rows[1 - selectedIndex].Cells[0];
            PumpUntil(() => locations.Rows.Count == 0 && Field<Label>(view, "_locationsEmptyLabel").Visible, "empty source after reselection");
            Assert(Field<Label>(view, "_locationsEmptyLabel").Text == AppStrings.LocationsEmpty
                && Field<Label>(view, "_notesEmptyLabel").Text == AppStrings.NotesEmpty,
                "Selected empty source incorrectly asks the user to select a source.");
            sources.CurrentCell = sources.Rows[selectedIndex].Cells[0];
            PumpUntil(() => locations.Rows.Count == 1, "repeat location reselection");
            restarted.Close();
            Assert(persisted.SequenceEqual(File.ReadAllBytes(LocalHealthNotebookPaths.SourcesFilePath)), "Restart/selection wrote to sources.json.");
            Console.WriteLine("SourceLocation dialog/restart/reselection regression passed: " + language);
        }
        finally { Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, testPath); }
    }
    private static MainForm NewSourceShell() => new(new HealthTopicService(new JsonHealthTopicRepository()), CreateEntryService(), CreateSourceService());
    private static SourceLocation CreateLocationThroughDialogs()
    {
        using var form = NewSourceShell();
        ShowOffScreen(form);
        Field<NavigationButton>(Field<NavigationControl>(form, "_navigation"), "_sourcesButton").PerformClick();
        var view = Field<SourcesView>(form, "_sourcesView");
        Assert(Field<Label>(view, "_locationsEmptyLabel").Text == AppStrings.SourceSelectionEmpty,
            "No-selection empty state missing.");
        var action = Field<Button>(form, "_newTopicButton");
        RunSourceDialog<CreateSourceForm>(action, dialog => Field<TextBox>(dialog, "_titleTextBox").Text = "CODEX TEST – Location reload");
        PumpUntil(() => view.SelectedSource?.Title == "CODEX TEST – Location reload", "source dialog selection");
        Guid sourceId = view.SelectedSource!.Id;
        RunSourceDialog<CreateSourceLocationForm>(Field<Button>(view, "_newLocationButton"), dialog =>
        {
            Field<TextBox>(dialog, "_locatorTextBox").Text = "Page 17";
            Field<TextBox>(dialog, "_noteTextBox").Text = "Synthetic persisted location note.";
        });
        PumpUntil(() => Field<DataGridView>(view, "_locationsGrid").Rows.Count == 1, "created location display");
        var expected = new SourceService(new JsonSourceRepository(), new JsonHealthTopicRepository())
            .GetSourceDetailsAsync(sourceId).GetAwaiter().GetResult().Locations.Single();
        RunSourceDialog<CreateSourceForm>(action, dialog => Field<TextBox>(dialog, "_titleTextBox").Text = "CODEX TEST – Newer source without locations");
        PumpUntil(() => view.SelectedSource?.Id != sourceId, "newer source selection");
        form.Close(); // The using scope fully disposes MainForm and all its views before returning.
        return expected;
    }
}
