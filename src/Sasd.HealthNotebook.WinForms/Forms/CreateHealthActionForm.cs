using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.WinForms.Localization;

namespace Sasd.HealthNotebook.WinForms.Forms;

/// <summary>Personal action editor with explicit user-reported provenance.</summary>
public sealed class CreateHealthActionForm : SourceRecordForm
{
    private readonly HealthActionService _service;
    private readonly TextBox _titleTextBox = new() { MaxLength = HealthAction.MaximumTitleLength };
    private readonly TextBox _descriptionTextBox = TextEditor(HealthAction.MaximumTextLength);
    private readonly TextBox _originNoteTextBox = TextEditor(HealthAction.MaximumTextLength);
    private readonly ComboBox _typeComboBox = Choice();
    private readonly ComboBox _statusComboBox = Choice();
    private readonly ComboBox _originComboBox = Choice();
    private readonly ComboBox _topicComboBox = Choice();
    private readonly ComboBox _sourceComboBox = Choice();
    private readonly ComboBox _sessionComboBox = Choice();
    /// <summary>Uses shared service and current topic summaries.</summary>
    public CreateHealthActionForm(HealthActionService service, IReadOnlyList<HealthTopicSummary> topics, IReadOnlyList<SourceSummary> sources, IReadOnlyList<SessionSummary> sessions)
        : base(AppStrings.NewAction, AppStrings.ActionSemantics, new Size(780, 740))
    {
        _service = service;
        _typeComboBox.DataSource = Enum.GetValues<HealthActionType>().OrderBy(value => value == HealthActionType.Other ? 0 : 1).Select(value => new Option<HealthActionType>(value, AppStrings.ActionTypeText(value))).ToList();
        _statusComboBox.DataSource = Enum.GetValues<HealthActionStatus>().Select(value => new Option<HealthActionStatus>(value, AppStrings.ActionStatusText(value))).ToList();
        _originComboBox.DataSource = Enum.GetValues<HealthActionOrigin>().Select(value => new Option<HealthActionOrigin>(value, AppStrings.ActionOriginText(value))).ToList();
        _topicComboBox.DataSource = new[] { new Option<Guid?>(null, AppStrings.NoEntryTopic) }.Concat(topics.Select(topic => new Option<Guid?>(topic.Id, topic.Title))).ToList();
        _sourceComboBox.DataSource = new[] { new Option<Guid?>(null, AppStrings.NoActionLink) }.Concat(sources.Select(item => new Option<Guid?>(item.Source.Id, string.IsNullOrWhiteSpace(item.Source.Title) ? item.Source.AuthorOrInstitution ?? item.Source.Url ?? AppStrings.Sources : item.Source.Title))).ToList();
        _sessionComboBox.DataSource = new[] { new Option<Guid?>(null, AppStrings.NoActionLink) }.Concat(sessions.Select(item => new Option<Guid?>(item.Session.Id, item.Session.Title))).ToList();
        AddField(AppStrings.ActionTitle, _titleTextBox, AppStrings.ActionTitle);
        AddField(AppStrings.ActionKind, _typeComboBox, AppStrings.ActionKind);
        AddField(AppStrings.EntryTopic, _topicComboBox, AppStrings.EntryTopicHelp);
        AddField(AppStrings.ActionState, _statusComboBox, AppStrings.ActionState);
        AddField(AppStrings.ActionOrigin, _originComboBox, AppStrings.ActionSemantics);
        AddField(AppStrings.ActionSourceLink, _sourceComboBox, AppStrings.ActionLinkHelp);
        AddField(AppStrings.ActionSessionLink, _sessionComboBox, AppStrings.ActionLinkHelp);
        AddField(AppStrings.ActionOriginNote, _originNoteTextBox, AppStrings.ActionSemantics, true, 40);
        AddField(AppStrings.ActionDescription, _descriptionTextBox, AppStrings.ActionDescription, true, 60);
        Shown += (_, _) => _titleTextBox.Focus();
    }
    /// <summary>Identity to select after creation.</summary>
    public Guid? CreatedActionId { get; private set; }
    protected override string ValidationMessage => AppStrings.ActionValidation;
    protected override string SaveOperation => AppStrings.SaveActionOperation;
    protected override async Task SaveRecordAsync()
    {
        var action = await _service.CreateActionAsync(new(_titleTextBox.Text, ((Option<HealthActionType>)_typeComboBox.SelectedItem!).Value,
            ((Option<HealthActionStatus>)_statusComboBox.SelectedItem!).Value, ((Option<Guid?>)_topicComboBox.SelectedItem!).Value,
            _descriptionTextBox.Text, ((Option<HealthActionOrigin>)_originComboBox.SelectedItem!).Value, _originNoteTextBox.Text, ((Option<Guid?>)_sourceComboBox.SelectedItem!).Value, ((Option<Guid?>)_sessionComboBox.SelectedItem!).Value));
        CreatedActionId = action.Id;
    }
    private static ComboBox Choice() => new() { DropDownStyle = ComboBoxStyle.DropDownList, DisplayMember = "Label" };
    private sealed record Option<T>(T Value, string Label);
}
