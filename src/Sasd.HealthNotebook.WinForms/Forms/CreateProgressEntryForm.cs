using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.WinForms.Localization;

namespace Sasd.HealthNotebook.WinForms.Forms;

/// <summary>Records a user's execution and note without evaluating health effects.</summary>
public sealed class CreateProgressEntryForm : SourceRecordForm
{
    private readonly HealthActionService _service;
    private readonly Guid _routineId;
    private readonly DateTimePicker _datePicker = new() { Format = DateTimePickerFormat.Custom };
    private readonly DateTimePicker _timePicker = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "HH:mm", ShowUpDown = true };
    private readonly ComboBox _completionComboBox = new() { DropDownStyle = ComboBoxStyle.DropDownList, DisplayMember = "Label" };
    private readonly CheckBox _countCheckBox = new() { AutoSize = true, Text = AppStrings.RecordProgressCount, TabIndex = 0 };
    private readonly NumericUpDown _countEditor = new() { Minimum = 0, Maximum = int.MaxValue, Enabled = false, Width = 140, TabIndex = 1, AccessibleName = AppStrings.ProgressCount, AccessibleDescription = AppStrings.ProgressCountHelp };
    private readonly TextBox _noteTextBox = TextEditor(HealthAction.MaximumTextLength);
    /// <summary>Records history only for the explicitly selected routine.</summary>
    public CreateProgressEntryForm(HealthActionService service, Guid routineId)
        : base(AppStrings.NewProgress, AppStrings.ActionSemantics, new Size(740, 560))
    {
        _service = service; _routineId = routineId;
        _datePicker.CustomFormat = AppLanguage.Current == UiLanguage.German ? "dd.MM.yyyy" : "MM/dd/yyyy";
        _completionComboBox.DataSource = Enum.GetValues<ProgressCompletion>().Select(value => new CompletionChoice(value, AppStrings.CompletionText(value))).ToList();
        AddField(AppStrings.EntryDate, _datePicker, AppStrings.EntryTimeHelp);
        AddField(AppStrings.EntryClock, _timePicker, AppStrings.EntryTimeHelp);
        AddField(AppStrings.ProgressState, _completionComboBox, AppStrings.ProgressState);
        var countPanel = new FlowLayoutPanel { WrapContents = false };
        countPanel.Controls.Add(_countCheckBox); countPanel.Controls.Add(_countEditor);
        _countCheckBox.CheckedChanged += (_, _) => _countEditor.Enabled = _countCheckBox.Checked;
        AddField(AppStrings.ProgressCount, countPanel, AppStrings.ProgressCountHelp);
        AddField(AppStrings.ProgressNote, _noteTextBox, AppStrings.ActionSemantics, true);
    }
    /// <summary>Identity to select after creation.</summary>
    public Guid? CreatedProgressId { get; private set; }
    protected override string ValidationMessage => AppStrings.ActionValidation;
    protected override string SaveOperation => AppStrings.SaveActionOperation;
    protected override async Task SaveRecordAsync()
    {
        var local = DateTime.SpecifyKind(_datePicker.Value.Date + _timePicker.Value.TimeOfDay, DateTimeKind.Unspecified);
        if (TimeZoneInfo.Local.IsInvalidTime(local) || TimeZoneInfo.Local.IsAmbiguousTime(local)) throw new ArgumentException("Ambiguous or invalid local time.");
        var entry = await _service.CreateProgressEntryAsync(new(_routineId, new DateTimeOffset(local, TimeZoneInfo.Local.GetUtcOffset(local)),
            ((CompletionChoice)_completionComboBox.SelectedItem!).Completion, _noteTextBox.Text, _countCheckBox.Checked ? (int?)_countEditor.Value : null));
        CreatedProgressId = entry.Id;
    }
    private sealed record CompletionChoice(ProgressCompletion Completion, string Label);
}
