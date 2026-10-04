using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.WinForms.Localization;

namespace Sasd.HealthNotebook.WinForms.Forms;

/// <summary>Explicitly configured routine; rhythm remains personal text.</summary>
public sealed class CreateRoutineForm : SourceRecordForm
{
    private readonly HealthActionService _service;
    private readonly Routine? _existing;
    private readonly Guid _actionId;
    private readonly TextBox _titleTextBox = new() { MaxLength = HealthAction.MaximumTitleLength };
    private readonly TextBox _scheduleTextBox = new() { MaxLength = HealthAction.MaximumTitleLength };
    private readonly TextBox _descriptionTextBox = TextEditor(HealthAction.MaximumTextLength);
    private readonly ComboBox _statusComboBox = new() { DropDownStyle = ComboBoxStyle.DropDownList, DisplayMember = "Label" };
    /// <summary>Creates a routine belonging to the selected action.</summary>
    public CreateRoutineForm(HealthActionService service, Guid actionId, Routine? existing = null)
        : base(existing is null ? AppStrings.NewRoutine : AppStrings.EditRoutine, AppStrings.RoutineScheduleHelp, new Size(720, 540))
    {
        _existing = existing;
        _service = service; _actionId = actionId;
        _statusComboBox.DataSource = Enum.GetValues<RoutineStatus>().Select(value => new StatusChoice(value, AppStrings.RoutineStatusText(value))).ToList();
        AddField(AppStrings.RoutineTitle, _titleTextBox, AppStrings.RoutineTitle);
        AddField(AppStrings.RoutineSchedule, _scheduleTextBox, AppStrings.RoutineScheduleHelp);
        AddField(AppStrings.ActionState, _statusComboBox, AppStrings.ActionState);
        AddField(AppStrings.ActionDescription, _descriptionTextBox, AppStrings.ActionDescription, true);
        Shown += (_, _) => _titleTextBox.Focus();
        // Data-bound Items are ready at Load, before users can interact with the editor.
        Load += (_, _) =>
        {
            if (existing is not null)
            {
                SetEditMode();
                _titleTextBox.Text = existing.Title; _descriptionTextBox.Text = existing.Description; _scheduleTextBox.Text = existing.ScheduleText;
                _statusComboBox.SelectedIndex = _statusComboBox.Items.Cast<StatusChoice>().ToList().FindIndex(item => item.Status == existing.Status);
            }
        };
    }
    /// <summary>Identity to select after creation.</summary>
    public Guid? CreatedRoutineId { get; private set; }
    protected override string ValidationMessage => AppStrings.ActionValidation;
    protected override string SaveOperation => AppStrings.SaveActionOperation;
    protected override async Task SaveRecordAsync()
    {
        var request = new CreateRoutineRequest(_actionId, _titleTextBox.Text, EditSupport.Optional(_descriptionTextBox.Text, _existing?.Description), EditSupport.Optional(_scheduleTextBox.Text, _existing?.ScheduleText),
            ((StatusChoice)_statusComboBox.SelectedItem!).Status);

        if (_existing is not null)
        {
            await _service.UpdateRoutineAsync(_existing.Id, request, _existing.ModifiedAt);
            CreatedRoutineId = _existing.Id; return;
        }
        var routine = await _service.CreateRoutineAsync(request);
        CreatedRoutineId = routine.Id;
    }
    private sealed record StatusChoice(RoutineStatus Status, string Label);
}
