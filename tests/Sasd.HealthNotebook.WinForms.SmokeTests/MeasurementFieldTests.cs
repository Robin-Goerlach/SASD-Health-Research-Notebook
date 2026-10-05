using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.Infrastructure.Storage;
using Sasd.HealthNotebook.WinForms.Forms;
using Sasd.HealthNotebook.WinForms.Localization;

namespace Sasd.HealthNotebook.WinForms.SmokeTests;

internal static partial class Program
{
    // UI-MEA-002: initialization without a type change, all create/edit fields and stale input isolation.
    private static void CheckMeasurementFields(string testPath, UiLanguage language)
    {
        string root = Path.Combine(testPath, "measurement-fields-" + language);
        Directory.CreateDirectory(root);
        Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, root);
        try
        {
            var service = CreateMeasurementService();
            foreach (var type in Enum.GetValues<MeasurementType>())
            {
                using var create = new CreateMeasurementForm(service, Array.Empty<Sasd.HealthNotebook.Application.Contracts.HealthTopicSummary>());
                ShowOffScreen(create);
                // The initial BloodPressure selection must already be fully labeled.
                CheckNumericFields(create, MeasurementType.BloodPressure);
                Field<ComboBox>(create, "_typeComboBox").SelectedIndex = (int)type;
                CheckNumericFields(create, type);
                FillMeasurementNumbers(create, type);
                Capture(create, Path.Combine(root, "create-" + type + ".png"));
                SaveMeasurementFields(create);
                var stored = Task.Run(() => new JsonMeasurementRepository().GetAllAsync()).GetAwaiter().GetResult().Single(item => item.MeasurementType == type);
                CheckStoredNumbers(stored, type);
                using var edit = new CreateMeasurementForm(service, Array.Empty<Sasd.HealthNotebook.Application.Contracts.HealthTopicSummary>(), stored);
                ShowOffScreen(edit); CheckNumericFields(edit, type);
                Assert(Field<TextBox>(edit, type == MeasurementType.BloodPressure ? "_systolicTextBox" : "_valueTextBox").Text == "130", "Edit primary numeric prefill failed.");
                if (type == MeasurementType.BloodPressure)
                    Assert(Field<TextBox>(edit, "_diastolicTextBox").Text == "60" && Field<TextBox>(edit, "_pulseTextBox").Text == "60", "Pressure edit components mixed up.");
                Assert(!Descendants(edit).OfType<TextBox>().Any(box => box.Text.Contains("130/60")), "Combined pressure string in editor.");
                Capture(edit, Path.Combine(root, "edit-" + type + ".png"));
                byte[] original = File.ReadAllBytes(LocalHealthNotebookPaths.MeasurementsFilePath);
                SaveMeasurementFields(edit);
                Assert(original.SequenceEqual(File.ReadAllBytes(LocalHealthNotebookPaths.MeasurementsFilePath)), "Unchanged numeric edit rewrote store.");
            }
            foreach (var pair in new[] { (MeasurementType.Weight, MeasurementType.BloodPressure),
                (MeasurementType.BloodPressure, MeasurementType.Weight), (MeasurementType.Pulse, MeasurementType.BloodPressure),
                (MeasurementType.BloodPressure, MeasurementType.Temperature) })
            {
                var existing = Task.Run(() => new JsonMeasurementRepository().GetAllAsync()).GetAwaiter().GetResult().Single(item => item.MeasurementType == pair.Item1);
                using var edit = new CreateMeasurementForm(service, Array.Empty<Sasd.HealthNotebook.Application.Contracts.HealthTopicSummary>(), existing);
                ShowOffScreen(edit);
                Field<ComboBox>(edit, "_typeComboBox").SelectedIndex = (int)pair.Item2;
                CheckNumericFields(edit, pair.Item2);
                Assert(new[] { "_valueTextBox", "_systolicTextBox", "_diastolicTextBox", "_pulseTextBox" }
                    .All(name => Field<TextBox>(edit, name).Text.Length == 0), "Type switch retained incompatible prefill.");
                FillMeasurementNumbers(edit, pair.Item2); SaveMeasurementFields(edit);
                var changed = Task.Run(() => new JsonMeasurementRepository().GetAllAsync()).GetAwaiter().GetResult().Single(item => item.Id == existing.Id);
                CheckStoredNumbers(changed, pair.Item2);
                Assert(changed.CreatedAt == existing.CreatedAt && changed.ModifiedAt > existing.ModifiedAt, "Type correction changed identity timestamps.");
                // Restore through the service so each switching case has an independent starting type.
                Task.Run(() => service.UpdateMeasurementAsync(changed.Id, new() { MeasurementType = existing.MeasurementType,
                    OccurredAt = existing.OccurredAt, Value = existing.Value, Systolic = existing.Systolic,
                    Diastolic = existing.Diastolic, Pulse = existing.Pulse }, changed.ModifiedAt)).GetAwaiter().GetResult();
            }
            using var optional = new CreateMeasurementForm(service, Array.Empty<Sasd.HealthNotebook.Application.Contracts.HealthTopicSummary>());
            ShowOffScreen(optional); FillMeasurementNumbers(optional, MeasurementType.BloodPressure);
            Field<TextBox>(optional, "_pulseTextBox").Clear(); SaveMeasurementFields(optional);
            Assert(Task.Run(() => new JsonMeasurementRepository().GetAllAsync()).GetAwaiter().GetResult().Count(item => item.MeasurementType == MeasurementType.BloodPressure && item.Pulse is null) == 1,
                "Optional pressure pulse became required or inherited stale input.");
            Console.WriteLine("Measurement create/edit/field-label/tab/type-switch checks passed: " + language);
        }
        finally { Environment.SetEnvironmentVariable(LocalHealthNotebookPaths.DataPathEnvironmentVariable, testPath); }
    }
    private static void CheckNumericFields(CreateMeasurementForm dialog, MeasurementType type)
    {
        dialog.Size = dialog.MinimumSize; System.Windows.Forms.Application.DoEvents();
        bool pressure = type == MeasurementType.BloodPressure;
        string[] fields = { "_valueTextBox", "_systolicTextBox", "_diastolicTextBox", "_pulseTextBox" };
        string[] labels = { "_primaryLabel", "_systolicLabel", "_diastolicLabel", "_pulseLabel" };
        for (int index = 0; index < fields.Length; index++)
        {
            bool visible = index == 0 ? !pressure : pressure;
            var editor = Field<TextBox>(dialog, fields[index]); var label = Field<Label>(dialog, labels[index]);
            Assert(editor.Visible == visible && editor.Enabled == visible && editor.TabStop == visible && label.Visible == visible, "Wrong numeric field visibility/enabled/tab state.");
            if (!visible) continue;
            string expected = index switch { 0 => AppStrings.MeasurementPrimaryLabel(type),
                1 => AppStrings.MeasurementPrimaryLabel(MeasurementType.BloodPressure), 2 => AppStrings.MeasurementDiastolic, _ => AppStrings.MeasurementPulseOptional };
            Assert(label.Text == expected && editor.AccessibleName == expected && !string.IsNullOrWhiteSpace(label.Text), "Measurement label belongs to wrong editor.");
            var panel = (TableLayoutPanel)editor.Parent!;
            Assert(panel.GetRow(label) == panel.GetRow(editor), "Numeric label/editor row mismatch.");
            AssertWithinParent(editor); AssertWithinParent(label); AssertTextFits(label);
        }
        Control current = Field<ComboBox>(dialog, "_typeComboBox"); current.Focus();
        foreach (string name in pressure ? fields.Skip(1) : fields.Take(1))
        {
            Assert(dialog.SelectNextControl(current, true, true, true, false), "Measurement keyboard traversal stopped.");
            current = Field<TextBox>(dialog, name);
            Assert(current.Focused, "Tab reached irrelevant or incorrectly ordered numeric input.");
        }
        Assert(dialog.SelectNextControl(current, true, true, true, false) && Field<ComboBox>(dialog, "_topicComboBox").Focused, "Tab included a hidden numeric editor.");
    }
    private static void FillMeasurementNumbers(CreateMeasurementForm dialog, MeasurementType type)
    {
        bool pressure = type == MeasurementType.BloodPressure;
        // Deliberately poison hidden controls: save must select only type-relevant numbers.
        Field<TextBox>(dialog, "_valueTextBox").Text = pressure ? "130/60" : "130";
        Field<TextBox>(dialog, "_systolicTextBox").Text = pressure ? "130" : "hidden invalid";
        Field<TextBox>(dialog, "_diastolicTextBox").Text = pressure ? "60" : "hidden invalid";
        Field<TextBox>(dialog, "_pulseTextBox").Text = pressure ? "60" : "hidden invalid";
    }
    private static void SaveMeasurementFields(CreateMeasurementForm dialog)
    {
        Field<Button>(dialog, "_saveButton").PerformClick();
        PumpUntil(() => dialog.IsDisposed, "structured measurement save");
    }
    private static void CheckStoredNumbers(Measurement stored, MeasurementType type)
    {
        Assert(stored.MeasurementType == type, "Measurement type was not saved.");
        if (type == MeasurementType.BloodPressure)
            Assert(stored.Value is null && stored.Systolic == 130 && stored.Diastolic == 60 && stored.Pulse == 60, "Structured blood pressure numbers were mixed up.");
        else Assert(stored.Value == 130 && stored.Systolic is null && stored.Diastolic is null && stored.Pulse is null, "Hidden pressure fields leaked into single measurement.");
    }
}
