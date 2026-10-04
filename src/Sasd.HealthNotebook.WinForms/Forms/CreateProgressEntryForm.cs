using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.WinForms.Localization;

namespace Sasd.HealthNotebook.WinForms.Forms;

/// <summary>Records a user's execution and note without evaluating health effects.</summary>
public sealed class CreateProgressEntryForm : SourceRecordForm
{
    private readonly HealthActionService _service;
    private readonly ProgressEntry? _existing;
    private readonly Guid _routineId;
    private readonly DateTimePicker _datePicker = new() { Format = DateTimePickerFormat.Custom };
    private readonly DateTimePicker _timePicker = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "HH:mm", ShowUpDown = true };
    private readonly ComboBox _completionComboBox = new() { DropDownStyle = ComboBoxStyle.DropDownList, DisplayMember = "Label" };
    private readonly CheckBox _countCheckBox = new() { AutoSize = true, Text = AppStrings.RecordProgressCount, TabIndex = 0 };
    private readonly NumericUpDown _countEditor = new() { Minimum = 0, Maximum = int.MaxValue, Enabled = false, Width = 140, TabIndex = 1, AccessibleName = AppStrings.ProgressCount, AccessibleDescription = AppStrings.ProgressCountHelp };
    private readonly TextBox _noteTextBox = TextEditor(HealthAction.MaximumTextLength);
    /// <summary>Records history only for the explicitly selected routine.</summary>
    public CreateProgressEntryForm(HealthActionService service, Guid routineId, ProgressEntry? existing = null)
        : base(existing is null ? AppStrings.NewProgress : AppStrings.EditProgress, AppStrings.ActionSemantics, new Size(740, 560))
    {
        _existing = existing;
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
        // Data-bound Items are ready at Load, before users can interact with the editor.
        Load += (_, _) =>
        {
            if (existing is not null)
            {
                SetEditMode();
                _datePicker.Value = _timePicker.Value = existing.OccurredAt.LocalDateTime;
                _completionComboBox.SelectedIndex = _completionComboBox.Items.Cast<CompletionChoice>().ToList().FindIndex(item => item.Completion == existing.Completion);
                _countCheckBox.Checked = existing.Count.HasValue; _countEditor.Value = existing.Count ?? 0; _noteTextBox.Text = existing.Note;
            }
        };
    }
    /// <summary>Identity to select after creation.</summary>
    public Guid? CreatedProgressId { get; private set; }
    protected override string ValidationMessage => AppStrings.ActionValidation;
    protected override string SaveOperation => AppStrings.SaveActionOperation;
    protected override async Task SaveRecordAsync()
    {
        var instant = EditSupport.Instant(_datePicker.Value, _timePicker.Value, _existing?.OccurredAt);
        var request = new CreateProgressEntryRequest(_routineId, instant,
            ((CompletionChoice)_completionComboBox.SelectedItem!).Completion, EditSupport.Optional(_noteTextBox.Text, _existing?.Note), _countCheckBox.Checked ? (int?)_countEditor.Value : null);

        if (_existing is not null)
        {
            await _service.UpdateProgressEntryAsync(_existing.Id, request, _existing.ModifiedAt);
            CreatedProgressId = _existing.Id; return;
        }
        var entry = await _service.CreateProgressEntryAsync(request);
        CreatedProgressId = entry.Id;
    }
    private sealed record CompletionChoice(ProgressCompletion Completion, string Label);
}
