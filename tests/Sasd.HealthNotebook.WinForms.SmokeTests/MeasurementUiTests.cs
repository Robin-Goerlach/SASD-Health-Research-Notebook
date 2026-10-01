using Sasd.HealthNotebook.Application.Contracts;
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
    private static MeasurementService CreateMeasurementService() => new(new JsonMeasurementRepository(), new JsonHealthTopicRepository());

    // UI-MEA-001: real navigation/dialogs, five categories, units, languages, immediate display and restart.
    private static void CheckMeasurements(HealthTopicService topics, string testPath, UiLanguage language)
    {
        string[] priorFiles = { "health-topics.json", "health-entries.json", "sources.json" };
        var priorBytes = priorFiles.ToDictionary(file => file, file => File.ReadAllBytes(Path.Combine(testPath, file)));
        var service = CreateMeasurementService();
        int before = service.GetMeasurementsAsync().GetAwaiter().GetResult().Count;
        using var form = new MainForm(topics, CreateEntryService(), CreateSourceService(), service);
        ShowOffScreen(form); form.Size = form.MinimumSize;
        var navigation = Field<NavigationControl>(form, "_navigation");
        var button = Field<NavigationButton>(navigation, "_measurementsButton");
        Assert(button.Text == AppStrings.Measurements, "Measurement navigation localization failed.");
        Field<NavigationButton>(navigation, "_sourcesButton").Focus();
        typeof(Control).GetMethod("OnKeyDown", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
            .Invoke(Field<NavigationButton>(navigation, "_sourcesButton"), new object[] { new KeyEventArgs(Keys.Down) });
        Assert(button.Focused, "Keyboard navigation did not reach measurements.");
        button.PerformClick(); WaitForReload(form);
        Assert(button.IsSelected, "Measurement navigation selection failed.");
        var view = Field<MeasurementsView>(form, "_measurementsView");
        var grid = Field<DataGridView>(view, "_grid");
        Assert(grid.Rows.Count == before && (before != 0 || Field<Label>(view, "_emptyStateLabel").Visible), "Measurement empty state/load failed.");
        var action = Field<Button>(form, "_newTopicButton");
        Assert(action.Text == AppStrings.NewMeasurement, "Measurement main action failed.");
        AssertWithinParent(action); AssertTextFits(action);
        Capture(form, Path.Combine(testPath, $"measurements-{language}-before.png"));
        Guid? expectedTopic = language == UiLanguage.German ? topics.GetTopicSummariesAsync().GetAwaiter().GetResult()[0].Id : null;
        foreach (MeasurementType type in Enum.GetValues<MeasurementType>())
        {
            RunMeasurementDialog(action, dialog =>
            {
                dialog.Size = dialog.MinimumSize;
                var kind = Field<ComboBox>(dialog, "_typeComboBox");
                Field<TextBox>(dialog, "_valueTextBox").Text = "999";
                Field<TextBox>(dialog, "_diastolicTextBox").Text = "888";
                Field<TextBox>(dialog, "_pulseTextBox").Text = "777";
                // Change away and back: hidden values must never leak across categories.
                kind.SelectedIndex = type == MeasurementType.Weight ? 0 : (int)MeasurementType.Weight;
                kind.SelectedIndex = (int)type;
                var primary = Field<TextBox>(dialog, "_valueTextBox");
                var diastolic = Field<TextBox>(dialog, "_diastolicTextBox");
                var pulse = Field<TextBox>(dialog, "_pulseTextBox");
                Assert(primary.Text.Length == 0 && diastolic.Text.Length == 0 && pulse.Text.Length == 0, "Type switch retained stale numbers.");
                bool pressure = type == MeasurementType.BloodPressure;
                Assert(diastolic.Visible == pressure && pulse.Visible == pressure && pulse.TabStop == pressure, "Type-dependent editors failed.");
                Assert(primary.AccessibleName == AppStrings.MeasurementPrimaryLabel(type), "Specific measurement label/unit missing.");
                foreach (string field in new[] { "_datePicker", "_timePicker", "_typeComboBox", "_valueTextBox", "_topicComboBox", "_contextTextBox", "_noteTextBox", "_saveButton" })
                {
                    var control = Field<Control>(dialog, field); AssertWithinParent(control);
                    if (field != "_saveButton") Assert(!string.IsNullOrWhiteSpace(Field<ToolTip>(dialog, "_toolTip").GetToolTip(control)), "Measurement tooltip missing.");
                }
                foreach (Label label in Descendants(dialog).OfType<Label>().Where(label => label.Visible)) AssertTextFits(label);
                Assert(primary.TabIndex == 0 && diastolic.TabIndex == 1 && pulse.TabIndex == 2
                    && Field<Control>(dialog, "_valuesPanel").TabIndex == 3 && Field<Control>(dialog, "_topicComboBox").TabIndex == 4
                    && Field<Control>(dialog, "_noteTextBox").TabIndex == 6, "Measurement tab sequence failed.");
                Assert(dialog.AcceptButton == Field<Button>(dialog, "_saveButton") && dialog.CancelButton is not null, "Measurement Enter/Escape missing.");
                AssertTextFits(Field<Button>(dialog, "_saveButton"));
                Field<Button>(dialog, "_saveButton").PerformClick();
                Assert(Field<Label>(dialog, "_validationLabel").Text == AppStrings.MeasurementValidationFailed, "Missing measurement not rejected.");
                foreach (string invalid in new[] { "-1", "NaN", language == UiLanguage.German ? "36.5" : "36,5", "1e999" })
                {
                    primary.Text = invalid;
                    if (pressure) diastolic.Text = "80";
                    Field<Button>(dialog, "_saveButton").PerformClick();
                    Assert(Field<Label>(dialog, "_validationLabel").Text == AppStrings.MeasurementValidationFailed, "Invalid or wrong-locale number accepted.");
                }
                double value = type switch { MeasurementType.BloodPressure => 120, MeasurementType.Pulse => 70,
                    MeasurementType.Temperature => 36.5, MeasurementType.BloodGlucose => 123, _ => 80 };
                primary.Text = value.ToString(AppStrings.MeasurementCulture);
                if (pressure) { diastolic.Text = "80"; pulse.Text = language == UiLanguage.German ? "70" : ""; }
                Field<DateTimePicker>(dialog, "_datePicker").Value = new DateTime(2026, 9, 20).AddDays((int)type);
                Field<DateTimePicker>(dialog, "_timePicker").Value = new DateTime(2026, 9, 20, 14, 0, 0);
                Field<ComboBox>(dialog, "_topicComboBox").SelectedIndex = expectedTopic.HasValue ? 1 : 0;
                Field<TextBox>(dialog, "_contextTextBox").Text = "Synthetic measurement situation.";
                Field<TextBox>(dialog, "_noteTextBox").Text = $"CODEX TEST – Synthetic measurement {language}/{type}.";
                Capture(dialog, Path.Combine(testPath, $"measurement-dialog-{language}-{type}-minimum.png"));
            });
            int expected = before + (int)type + 1;
            PumpUntil(() => grid.Rows.Count == expected, "measurement immediate refresh");
            var stored = service.GetMeasurementsAsync().GetAwaiter().GetResult().Single(item => item.Measurement.Note == $"CODEX TEST – Synthetic measurement {language}/{type}.");
            Assert(stored.Measurement.HealthTopicId == expectedTopic && stored.Measurement.Unit == Measurement.UnitFor(type), "Measurement linkage/unit failed.");
            double? expectedValue = type switch { MeasurementType.Pulse => 70, MeasurementType.Temperature => 36.5,
                MeasurementType.BloodGlucose => 123, MeasurementType.Weight => 80, _ => null };
            string expectedUnit = type switch { MeasurementType.BloodPressure => "mmHg", MeasurementType.Pulse => "/min",
                MeasurementType.Temperature => "°C", MeasurementType.BloodGlucose => "mg/dL", _ => "kg" };
            Assert(stored.Measurement.Value == expectedValue, "Dialog changed the numeric value or decimal separator.");
            var row = grid.Rows.Cast<DataGridViewRow>().Single(item => Equals(item.Cells[5].Value, stored.Measurement.Note));
            Assert(Equals(row.Cells[1].Value, AppStrings.MeasurementTypeText(type))
                && Equals(row.Cells[2].Value, AppStrings.FormatMeasurementValues(stored.Measurement))
                && Equals(row.Cells[3].Value, expectedUnit)
                && Equals(row.Cells[4].Value, stored.HealthTopicTitle ?? AppStrings.NoEntryTopic), "Displayed measurement/units/topic failed.");
            if (type == MeasurementType.BloodPressure)
                Assert(stored.Measurement.Systolic == 120 && stored.Measurement.Diastolic == 80
                    && stored.Measurement.Pulse == (language == UiLanguage.German ? 70d : null), "Optional pressure pulse failed.");
        }
        var selected = grid.CurrentRow!.Cells[5].Value;
        Field<Button>(form, "_refreshButton").PerformClick(); WaitForReload(form);
        Assert(Equals(grid.CurrentRow!.Cells[5].Value, selected), "Measurement refresh lost selection.");
        view.SetMeasurements(new[] { new MeasurementSummary(Measurement.Create(MeasurementType.Pulse, DateTimeOffset.Now,
            value: 70, healthTopicId: Guid.NewGuid()), null) });
        Assert(grid.Rows.Count == 1 && Equals(grid.Rows[0].Cells[4].Value, AppStrings.MissingEntryTopic), "Missing topic measurement not displayed neutrally.");
        WaitForReload(form);
        Assert(grid.Columns.Cast<DataGridViewColumn>().Sum(column => column.Width) <= grid.ClientSize.Width, "Measurement columns clip at minimum size.");
        var selector = Field<ComboBox>(form, "_languageComboBox");
        selector.SelectedIndex = language == UiLanguage.German ? 1 : 0; WaitForReload(form);
        Assert(action.Text == AppStrings.NewMeasurement && button.Text == AppStrings.Measurements, "Measurement language switch failed.");
        AssertWithinParent(action); AssertTextFits(action);
        selector.SelectedIndex = language == UiLanguage.German ? 0 : 1; WaitForReload(form);
        Capture(form, Path.Combine(testPath, $"measurements-{language}-after.png")); form.Close();
        using var restarted = new MainForm(new HealthTopicService(new JsonHealthTopicRepository()), CreateEntryService(), CreateSourceService(), CreateMeasurementService());
        ShowOffScreen(restarted);
        Field<NavigationButton>(Field<NavigationControl>(restarted, "_navigation"), "_measurementsButton").PerformClick(); WaitForReload(restarted);
        Assert(Field<DataGridView>(Field<MeasurementsView>(restarted, "_measurementsView"), "_grid").Rows.Count == before + 5, "Measurements failed shell/repository restart.");
        restarted.Close();
        foreach (string file in priorFiles) Assert(priorBytes[file].SequenceEqual(File.ReadAllBytes(Path.Combine(testPath, file))), "Measurement UI changed another notebook store.");
    }
    private static void RunMeasurementDialog(Button action, Action<CreateMeasurementForm> fill)
    {
        bool closed = false; Exception? failure = null;
        using var timer = new System.Windows.Forms.Timer { Interval = 30 };
        timer.Tick += (_, _) =>
        {
            var dialog = System.Windows.Forms.Application.OpenForms.OfType<CreateMeasurementForm>().SingleOrDefault();
            if (dialog is null) return; timer.Stop();
            try { fill(dialog); dialog.FormClosed += (_, _) => closed = true; Field<Button>(dialog, "_saveButton").PerformClick(); }
            catch (Exception ex) { failure = ex; dialog.Close(); closed = true; }
        };
        timer.Start(); action.PerformClick(); PumpUntil(() => closed, "measurement dialog create");
        if (failure is not null) throw new InvalidOperationException(failure.Message, failure);
    }
}
