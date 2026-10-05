using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.Infrastructure.Storage;
using Sasd.HealthNotebook.WinForms.Forms;
using Sasd.HealthNotebook.WinForms.Localization;
using Sasd.HealthNotebook.WinForms.Views;

namespace Sasd.HealthNotebook.WinForms.SmokeTests;

internal static partial class Program
{
    // UI-MEA-003: display-only filtering, semantic localization and lifecycle under a filter.
    private static void CheckMeasurementTypeFilter(string testPath, UiLanguage language)
    {
        string root = Path.Combine(testPath, "measurement-filter-" + language);
        Directory.CreateDirectory(root);
        Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, root);
        try
        {
            var service = CreateMeasurementService();
            foreach (var type in Enum.GetValues<MeasurementType>())
                Task.Run(() => service.CreateMeasurementAsync(new CreateMeasurementRequest { MeasurementType = type,
                    OccurredAt = DateTimeOffset.Now.AddMinutes(-(int)type),
                    Value = type == MeasurementType.BloodPressure ? null : 13,
                    Systolic = type == MeasurementType.BloodPressure ? 130 : null,
                    Diastolic = type == MeasurementType.BloodPressure ? 60 : null,
                    Pulse = type == MeasurementType.BloodPressure ? 60 : null,
                    Note = "CODEX TEST – filter fixture" })).GetAwaiter().GetResult();
            var initialFiles = Directory.GetFiles(root).ToDictionary(path => path, File.ReadAllBytes);
            var original = Task.Run(() => new JsonMeasurementRepository().GetAllAsync()).GetAwaiter().GetResult();
            using (var shell = NewActionShell())
            {
                ShowOffScreen(shell); NavigateLifecycle(shell, "_measurementsButton");
                var view = Field<MeasurementsView>(shell, "_measurementsView");
                var filter = Field<ComboBox>(view, "_typeFilter");
                var grid = Field<DataGridView>(view, "_grid");
                Assert(view.SelectedMeasurementType is null && filter.Text == AppStrings.MeasurementsFilterAll && grid.Rows.Count == 5, "Default All did not show complete measurement list.");
                Assert(filter.Items.Count == 6 && filter.DropDownStyle == ComboBoxStyle.DropDownList, "Filter choices missing or editable.");
                foreach (var type in Enum.GetValues<MeasurementType>())
                {
                    filter.SelectedIndex = (int)type + 1;
                    Assert(view.SelectedMeasurementType == type && filter.Text == AppStrings.MeasurementFilterTypeText(type), "Incorrect type filter label/selection.");
                    Assert(grid.Rows.Count == 1 && view.SelectedMeasurement?.MeasurementType == type
                        && Equals(grid.Rows[0].Cells[1].Value, AppStrings.MeasurementTypeText(type)), "Filter retained excluded row or stale selection.");
                }
                var weightId = view.SelectedMeasurement!.Id;
                Field<Button>(shell, "_refreshButton").PerformClick(); WaitForReload(shell);
                Assert(view.SelectedMeasurementType == MeasurementType.Weight && view.SelectedMeasurement?.Id == weightId, "Refresh reset filter/selection.");
                var languageSelector = Field<ComboBox>(shell, "_languageComboBox");
                languageSelector.SelectedIndex = language == UiLanguage.German ? 1 : 0; WaitForReload(shell);
                Assert(view.SelectedMeasurementType == MeasurementType.Weight && filter.Text == AppStrings.MeasurementFilterTypeText(MeasurementType.Weight)
                    && view.SelectedMeasurement?.Id == weightId && Field<Label>(view, "_filterLabel").Text == AppStrings.MeasurementFilterLabel,
                    "Language switch lost semantic filter, row or translated labels.");
                languageSelector.SelectedIndex = language == UiLanguage.German ? 0 : 1; WaitForReload(shell);
                filter.SelectedIndex = 0; Assert(grid.Rows.Count == 5 && view.SelectedMeasurement?.Id == weightId, "All projection lost data or visible selection.");
                filter.SelectedIndex = (int)MeasurementType.Weight + 1;
                CheckFilterLayout(shell, view, root);
                // The layout check deliberately creates PNGs; every other filename and
                // every original store byte must still match before any lifecycle edit.
                Assert(initialFiles.Keys.Order().SequenceEqual(Directory.GetFiles(root).Where(path => !path.EndsWith(".png", StringComparison.OrdinalIgnoreCase)).Order())
                    && initialFiles.All(pair => pair.Value.SequenceEqual(File.ReadAllBytes(pair.Key))), "Filtering/localization/refresh modified or created a store.");
                Assert(original.SequenceEqual(Task.Run(() => new JsonMeasurementRepository().GetAllAsync()).GetAwaiter().GetResult()), "Filter changed complete domain list.");
                LifecycleDialog<CreateMeasurementForm>(Field<Button>(view, "_editButton"), dialog =>
                { Field<TextBox>(dialog, "_valueTextBox").Text = "14"; ((Button)dialog.AcceptButton!).PerformClick(); });
                PumpUntil(() => view.SelectedMeasurement?.Value == 14, "filtered measurement edit");
                Assert(view.SelectedMeasurementType == MeasurementType.Weight && view.SelectedMeasurement?.Id == weightId && grid.Rows.Count == 1,
                    "Edit reset filter or row selection.");
                // Changing the edited type can legitimately remove it from this projection.
                LifecycleDialog<CreateMeasurementForm>(Field<Button>(view, "_editButton"), dialog =>
                { Field<ComboBox>(dialog, "_typeComboBox").SelectedIndex = (int)MeasurementType.Temperature;
                    Field<TextBox>(dialog, "_valueTextBox").Text = "15"; ((Button)dialog.AcceptButton!).PerformClick(); });
                PumpUntil(() => view.SelectedMeasurement is null, "edited type excluded from filter");
                CheckFilteredEmpty(view);
                Capture(shell, Path.Combine(root, "filter-empty.png"));
                filter.SelectedIndex = (int)MeasurementType.Pulse + 1;
                Assert(view.SelectedMeasurement?.MeasurementType == MeasurementType.Pulse && grid.Rows.Count == 1, "Filter did not recover visible selection.");
                var pulseId = view.SelectedMeasurement!.Id;
                ConfirmLifecycleDelete(Field<Button>(view, "_deleteButton"), false);
                Assert(view.SelectedMeasurementType == MeasurementType.Pulse && view.SelectedMeasurement?.Id == pulseId, "Cancelled delete reset filter/selection.");
                ConfirmLifecycleDelete(Field<Button>(view, "_deleteButton"), true);
                PumpUntil(() => view.SelectedMeasurement is null, "filtered measurement delete"); CheckFilteredEmpty(view);
                Assert(view.SelectedMeasurementType == MeasurementType.Pulse, "Delete reset filter.");
                Field<Button>(shell, "_refreshButton").PerformClick(); WaitForReload(shell); CheckFilteredEmpty(view);
                filter.SelectedIndex = 0;
                Assert(grid.Rows.Count == 4 && view.SelectedMeasurement is not null, "All lost unrelated rows after filtered lifecycle.");
                Assert(Task.Run(() => new JsonMeasurementRepository().GetAllAsync()).GetAwaiter().GetResult().All(item => item.Id != pulseId), "Filtered deletion did not persist.");
                shell.Close();
            }
            using var restarted = NewActionShell(); ShowOffScreen(restarted); NavigateLifecycle(restarted, "_measurementsButton");
            var restartedView = Field<MeasurementsView>(restarted, "_measurementsView");
            Assert(restartedView.SelectedMeasurementType is null && Field<DataGridView>(restartedView, "_grid").Rows.Count == 4, "Restart did not default to All or lost persisted data.");
            restarted.Close();
            Console.WriteLine("Measurement type filter/lifecycle/localization/no-write/layout checks passed: " + language);
        }
        finally { Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, testPath); }
    }
    private static void CheckFilteredEmpty(MeasurementsView view)
    {
        Assert(Field<DataGridView>(view, "_grid").Rows.Count == 0 && view.SelectedMeasurement is null
            && Field<Label>(view, "_emptyStateLabel").Visible && Field<Label>(view, "_emptyStateLabel").Text == AppStrings.MeasurementsFilteredEmpty
            && !Field<Button>(view, "_editButton").Enabled && !Field<Button>(view, "_deleteButton").Enabled, "Filtered empty state/commands/stale selection incorrect.");
    }
    private static void CheckFilterLayout(MainForm shell, MeasurementsView view, string root)
    {
        shell.Size = shell.MinimumSize; System.Windows.Forms.Application.DoEvents();
        foreach (string name in new[] { "_filterLabel", "_typeFilter", "_editButton", "_deleteButton" }) AssertWithinParent(Field<Control>(view, name));
        AssertTextFits(Field<Label>(view, "_filterLabel"));
        foreach (var button in Descendants(view).OfType<Button>()) AssertTextFits(button);
        var combo = Field<ComboBox>(view, "_typeFilter");
        foreach (var type in Enum.GetValues<MeasurementType>())
            Assert(TextRenderer.MeasureText(AppStrings.MeasurementFilterTypeText(type), combo.Font).Width + SystemInformation.VerticalScrollBarWidth + 12 <= combo.Width, "Filter text clipped.");
        Control current = combo; current.Focus();
        foreach (string name in new[] { "_editButton", "_deleteButton", "_grid" })
        {
            Assert(view.SelectNextControl(current, true, true, true, false), "Filter keyboard traversal stopped.");
            current = Field<Control>(view, name); Assert(current.Focused, "Filter tab order incorrect.");
        }
        Capture(shell, Path.Combine(root, "filter-minimum.png"));
    }
}
