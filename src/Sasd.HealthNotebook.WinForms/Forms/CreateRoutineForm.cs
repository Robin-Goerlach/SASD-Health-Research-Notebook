using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.WinForms.Localization;

namespace Sasd.HealthNotebook.WinForms.Forms;

/// <summary>Explicitly configured routine; rhythm remains personal text.</summary>
public sealed class CreateRoutineForm : SourceRecordForm
{
    private readonly HealthActionService _service;
    private readonly Guid _actionId;
    private readonly TextBox _titleTextBox = new() { MaxLength = HealthAction.MaximumTitleLength };
    private readonly TextBox _scheduleTextBox = new() { MaxLength = HealthAction.MaximumTitleLength };
    private readonly TextBox _descriptionTextBox = TextEditor(HealthAction.MaximumTextLength);
    private readonly ComboBox _statusComboBox = new() { DropDownStyle = ComboBoxStyle.DropDownList, DisplayMember = "Label" };
    /// <summary>Creates a routine belonging to the selected action.</summary>
    public CreateRoutineForm(HealthActionService service, Guid actionId)
        : base(AppStrings.NewRoutine, AppStrings.RoutineScheduleHelp, new Size(720, 540))
    {
        _service = service; _actionId = actionId;
        _statusComboBox.DataSource = Enum.GetValues<RoutineStatus>().Select(value => new StatusChoice(value, AppStrings.RoutineStatusText(value))).ToList();
        AddField(AppStrings.RoutineTitle, _titleTextBox, AppStrings.RoutineTitle);
        AddField(AppStrings.RoutineSchedule, _scheduleTextBox, AppStrings.RoutineScheduleHelp);
        AddField(AppStrings.ActionState, _statusComboBox, AppStrings.ActionState);
        AddField(AppStrings.ActionDescription, _descriptionTextBox, AppStrings.ActionDescription, true);
        Shown += (_, _) => _titleTextBox.Focus();
    }
    /// <summary>Identity to select after creation.</summary>
    public Guid? CreatedRoutineId { get; private set; }
    protected override string ValidationMessage => AppStrings.ActionValidation;
    protected override string SaveOperation => AppStrings.SaveActionOperation;
    protected override async Task SaveRecordAsync()
    {
        var routine = await _service.CreateRoutineAsync(new(_actionId, _titleTextBox.Text, _descriptionTextBox.Text, _scheduleTextBox.Text,
            ((StatusChoice)_statusComboBox.SelectedItem!).Status));
        CreatedRoutineId = routine.Id;
    }
    private sealed record StatusChoice(RoutineStatus Status, string Label);
}
