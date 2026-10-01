using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.WinForms.Localization;

namespace Sasd.HealthNotebook.WinForms.Forms;

/// <summary>Documents a claim while keeping original wording and personal interpretation separate.</summary>
public sealed class CreateEvidenceNoteForm : SourceRecordForm
{
    private readonly SourceService _service;
    private readonly Guid _sourceId;
    private readonly ComboBox _locationComboBox = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox _statementTextBox = TextEditor(EvidenceNote.MaximumStatementLength);
    private readonly TextBox _excerptTextBox = TextEditor(EvidenceNote.MaximumExcerptLength);
    private readonly TextBox _assessmentTextBox = TextEditor(EvidenceNote.MaximumAssessmentLength);
    /// <summary>Shows only this source's locations; Application independently validates the relationship.</summary>
    public CreateEvidenceNoteForm(SourceService service, Source source, IReadOnlyList<SourceLocation> locations)
        : base(AppStrings.NewSourceNote, AppStrings.SourceSemantics, new Size(820, 680))
    {
        _service = service ?? throw new ArgumentNullException(nameof(service)); _sourceId = source.Id;
        ArgumentNullException.ThrowIfNull(locations);
        _locationComboBox.DataSource = new[] { new LocationChoice(null, AppStrings.NoSourceLocation) }
            .Concat(locations.Where(location => location.SourceId == source.Id).Select(location => new LocationChoice(location.Id,
                AppStrings.SourceLocationTypeText(location.LocationType) + ": " + location.Locator))).ToList();
        _locationComboBox.DisplayMember = nameof(LocationChoice.Label);
        AddField(AppStrings.NoteLocation, _locationComboBox, AppStrings.NoSourceLocation);
        AddField(AppStrings.SourceStatement, _statementTextBox, AppStrings.SourceStatementHelp, stretch: true, weight: 35);
        AddField(AppStrings.SourceExcerpt, _excerptTextBox, AppStrings.SourceExcerptHelp, stretch: true, weight: 25);
        AddField(AppStrings.SourceAssessment, _assessmentTextBox, AppStrings.SourceAssessmentHelp, stretch: true, weight: 40);
        Shown += (_, _) => _statementTextBox.Focus();
    }
    /// <inheritdoc />
    protected override async Task SaveRecordAsync() => await _service.CreateNoteAsync(new CreateEvidenceNoteRequest {
        SourceId = _sourceId, SourceLocationId = ((LocationChoice)_locationComboBox.SelectedItem!).Id,
        Statement = _statementTextBox.Text, Excerpt = _excerptTextBox.Text, OwnParaphraseOrAssessment = _assessmentTextBox.Text });
    private sealed record LocationChoice(Guid? Id, string Label);
}
