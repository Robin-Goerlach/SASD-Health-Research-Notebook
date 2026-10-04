using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.WinForms.Localization;

namespace Sasd.HealthNotebook.WinForms.Forms;

/// <summary>Small documentation editor; archive metadata and identity remain in the shared service.</summary>
public sealed class EditHealthTopicForm : SourceRecordForm
{
    private readonly HealthTopicService _service;
    private readonly HealthTopic _topic;
    private readonly TextBox _titleTextBox = new() { MaxLength = 160 };
    private readonly TextBox _descriptionTextBox = TextEditor(500);
    private readonly TextBox _notesTextBox = TextEditor(4000);
    private readonly ComboBox _statusComboBox = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _priorityComboBox = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    /// <summary>Edits a loaded snapshot with an optimistic conflict token.</summary>
    public EditHealthTopicForm(HealthTopicService service, HealthTopic topic) : base(AppStrings.EditTopic, AppStrings.TopicEditHelp, new Size(800, 660))
    {
        _service = service; _topic = topic;
        _statusComboBox.DisplayMember = _priorityComboBox.DisplayMember = nameof(Choice<HealthTopicStatus>.Label);
        _statusComboBox.Items.AddRange(Enum.GetValues<HealthTopicStatus>().Select(value => new Choice<HealthTopicStatus>(value, AppStrings.HealthTopicStatusText(value))).Cast<object>().ToArray());
        _priorityComboBox.Items.AddRange(Enum.GetValues<HealthTopicPriority>().Select(value => new Choice<HealthTopicPriority>(value, AppStrings.HealthTopicPriorityText(value))).Cast<object>().ToArray());
        _statusComboBox.SelectedIndex = (int)topic.Status; _priorityComboBox.SelectedIndex = (int)topic.Priority;
        _titleTextBox.Text = topic.Title; _descriptionTextBox.Text = topic.ShortDescription; _notesTextBox.Text = topic.Notes;
        AddField(AppStrings.ColumnTitle, _titleTextBox, AppStrings.ColumnTitle);
        AddField(AppStrings.ColumnStatus, _statusComboBox, AppStrings.ColumnStatus);
        AddField(AppStrings.ColumnPriority, _priorityComboBox, AppStrings.ColumnPriority);
        AddField(AppStrings.ColumnShortDescription, _descriptionTextBox, AppStrings.ColumnShortDescription, true);
        AddField(AppStrings.MeasurementNote, _notesTextBox, AppStrings.MeasurementNote, true); SetEditMode();
    }
    protected override string ValidationMessage => AppStrings.TopicEditValidation;
    protected override async Task SaveRecordAsync() => await _service.UpdateHealthTopicAsync(_topic.Id, new CreateHealthTopicRequest { Title = _titleTextBox.Text,
        Status = ((Choice<HealthTopicStatus>)_statusComboBox.SelectedItem!).Value, Priority = ((Choice<HealthTopicPriority>)_priorityComboBox.SelectedItem!).Value,
        ShortDescription = _descriptionTextBox.Text, Notes = _notesTextBox.Text }, _topic.ModifiedAt);
    private sealed record Choice<T>(T Value, string Label);
}
