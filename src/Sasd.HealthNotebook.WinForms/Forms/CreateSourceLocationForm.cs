using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.WinForms.Localization;

namespace Sasd.HealthNotebook.WinForms.Forms;

/// <summary>Adds an exact location to the selected source.</summary>
public sealed class CreateSourceLocationForm : SourceRecordForm
{
    private readonly SourceService _service;
    private readonly Guid _sourceId;
    private readonly ComboBox _typeComboBox = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox _locatorTextBox = new() { MaxLength = SourceLocation.MaximumLocatorLength };
    private readonly TextBox _noteTextBox = TextEditor(SourceLocation.MaximumNoteLength);
    /// <summary>Labels the owning source explicitly; it is referenced by ID only when saved.</summary>
    public CreateSourceLocationForm(SourceService service, Source source)
        : base(AppStrings.NewSourceLocation, AppStrings.SourceDisplayName(source), new Size(760, 480))
    {
        _service = service ?? throw new ArgumentNullException(nameof(service)); _sourceId = source.Id;
        _typeComboBox.DataSource = Enum.GetValues<SourceLocationType>().Select(type =>
            new TypeChoice(type, AppStrings.SourceLocationTypeText(type))).ToList();
        _typeComboBox.DisplayMember = nameof(TypeChoice.Label);
        AddField(AppStrings.LocationType, _typeComboBox, AppStrings.SourceLocatorHelp);
        AddField(AppStrings.SourceLocator, _locatorTextBox, AppStrings.SourceLocatorHelp);
        AddField(AppStrings.LocationNote, _noteTextBox, AppStrings.EntryContentHelp, stretch: true);
        Shown += (_, _) => _locatorTextBox.Focus();
    }
    /// <inheritdoc />
    protected override async Task SaveRecordAsync() => await _service.CreateLocationAsync(new CreateSourceLocationRequest {
        SourceId = _sourceId, LocationType = ((TypeChoice)_typeComboBox.SelectedItem!).Type,
        Locator = _locatorTextBox.Text, Note = _noteTextBox.Text });
    private sealed record TypeChoice(SourceLocationType Type, string Label);
}
