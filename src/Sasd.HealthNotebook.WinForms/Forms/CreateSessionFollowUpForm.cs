using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.WinForms.Localization;

namespace Sasd.HealthNotebook.WinForms.Forms;

/// <summary>Records the user's next step and optional due date; no notifications.</summary>
public sealed class CreateSessionFollowUpForm : SourceRecordForm
{
    private readonly SessionService _service;
    private readonly Guid _sessionId;
    private readonly SessionFollowUp? _existing;
    private readonly TextBox _textBox = TextEditor(Session.MaximumTextLength);
    private readonly ComboBox _statusComboBox = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly DateTimePicker _duePicker = new() { Format = DateTimePickerFormat.Custom, ShowCheckBox = true, Checked = false };
    /// <summary>Creates a functional user-input dialog.</summary>
    public CreateSessionFollowUpForm(SessionService service, Guid sessionId, SessionFollowUp? existing = null)
        : base(existing is null ? AppStrings.NewSessionFollowUp : AppStrings.EditFollowUp, AppStrings.SessionSemantics, new Size(800, 560))
    {
        _service = service; _sessionId = sessionId; _existing = existing;
        _duePicker.CustomFormat = AppLanguage.Current == UiLanguage.German ? "dd.MM.yyyy" : "MM/dd/yyyy";
        _statusComboBox.DataSource = Enum.GetValues<SessionFollowUpStatus>().Select(status => new StatusChoice(status, AppStrings.FollowUpStatusText(status))).ToList(); _statusComboBox.DisplayMember = nameof(StatusChoice.Label);
        AddField(AppStrings.SessionNextStep, _textBox, AppStrings.SessionSemantics, true);
        AddField(AppStrings.SessionState, _statusComboBox, AppStrings.SessionState);
        AddField(AppStrings.SessionDueDate, _duePicker, AppStrings.SourceDateHelp);
        Load += (_, _) =>
        {
            if (existing is null) return;
            SetEditMode(); _textBox.Text = existing.Text;
            _statusComboBox.SelectedIndex = (int)existing.Status;
            _duePicker.Checked = existing.DueDate.HasValue;
            if (existing.DueDate.HasValue) _duePicker.Value = existing.DueDate.Value.ToDateTime(TimeOnly.MinValue);
        };
    }
    protected override string ValidationMessage => AppStrings.SessionValidation;
    protected override string SaveOperation => AppStrings.SaveSessionOperation;
    protected override async Task SaveRecordAsync()
    {
        var status = ((StatusChoice)_statusComboBox.SelectedItem!).Status;
        DateOnly? due = _duePicker.Checked ? DateOnly.FromDateTime(_duePicker.Value) : null;
        if (_existing is null) await _service.CreateFollowUpAsync(new(_sessionId, _textBox.Text, status, due));
        else await _service.UpdateFollowUpAsync(_existing with { Text = _textBox.Text, Status = status, DueDate = due }, _existing.ModifiedAt);
    }
    private sealed record StatusChoice(SessionFollowUpStatus Status, string Label);
}
