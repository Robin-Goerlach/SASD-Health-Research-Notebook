using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.WinForms.Localization;

namespace Sasd.HealthNotebook.WinForms.Forms;

/// <summary>Personal action editor with explicit user-reported provenance.</summary>
public sealed class CreateHealthActionForm : SourceRecordForm
{
    private readonly HealthActionService _service;
    private readonly HealthAction? _existing;
    private readonly TextBox _titleTextBox = new() { MaxLength = HealthAction.MaximumTitleLength };
    private readonly TextBox _descriptionTextBox = TextEditor(HealthAction.MaximumTextLength);
    private readonly TextBox _originNoteTextBox = TextEditor(HealthAction.MaximumTextLength);
    private readonly TextBox _changeReasonTextBox = new() { MaxLength = HealthAction.MaximumTextLength };
    private readonly ComboBox _typeComboBox = Choice();
    private readonly ComboBox _statusComboBox = Choice();
    private readonly ComboBox _originComboBox = Choice();
    private readonly ComboBox _topicComboBox = Choice();
    private readonly ComboBox _sourceComboBox = Choice();
    private readonly ComboBox _sessionComboBox = Choice();
    /// <summary>Uses shared service and current topic summaries.</summary>
    public CreateHealthActionForm(HealthActionService service, IReadOnlyList<HealthTopicSummary> topics, IReadOnlyList<SourceSummary> sources, IReadOnlyList<SessionSummary> sessions, HealthAction? existing = null)
        : base(existing is null ? AppStrings.NewAction : AppStrings.EditAction, AppStrings.ActionSemantics, new Size(780, 740))
    {
        _existing = existing;
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
        // Data-bound Items are ready at Load, before users can interact with the editor.
        Load += (_, _) =>
        {
            if (existing is not null)
            {
                SetEditMode();
                _titleTextBox.Text = existing.Title; _descriptionTextBox.Text = existing.Description; _originNoteTextBox.Text = existing.OriginNote;
                SelectOption(_typeComboBox, existing.ActionType); SelectOption(_statusComboBox, existing.Status); SelectOption(_originComboBox, existing.Origin);
                SelectReference(_topicComboBox, existing.HealthTopicId); SelectReference(_sourceComboBox, existing.SourceId); SelectReference(_sessionComboBox, existing.SessionId);
                AddField(AppStrings.ChangeReason, _changeReasonTextBox, AppStrings.ChangeReason);
            }
        };
    }
    /// <summary>Identity to select after creation.</summary>
    public Guid? CreatedActionId { get; private set; }
    protected override string ValidationMessage => AppStrings.ActionValidation;
    protected override string SaveOperation => AppStrings.SaveActionOperation;
    protected override async Task SaveRecordAsync()
    {
        var request = new CreateHealthActionRequest(_titleTextBox.Text, ((Option<HealthActionType>)_typeComboBox.SelectedItem!).Value,
            ((Option<HealthActionStatus>)_statusComboBox.SelectedItem!).Value, ((Option<Guid?>)_topicComboBox.SelectedItem!).Value,
            EditSupport.Optional(_descriptionTextBox.Text, _existing?.Description), ((Option<HealthActionOrigin>)_originComboBox.SelectedItem!).Value, EditSupport.Optional(_originNoteTextBox.Text, _existing?.OriginNote), ((Option<Guid?>)_sourceComboBox.SelectedItem!).Value, ((Option<Guid?>)_sessionComboBox.SelectedItem!).Value);

        if (_existing is not null)
        {
            await _service.UpdateHealthActionAsync(_existing.Id, request, _existing.ModifiedAt, EditSupport.Optional(_changeReasonTextBox.Text, null));
            CreatedActionId = _existing.Id; return;
        }
        var action = await _service.CreateActionAsync(request);
        CreatedActionId = action.Id;
    }
    private static ComboBox Choice() => new() { DropDownStyle = ComboBoxStyle.DropDownList, DisplayMember = "Label" };
    private sealed record Option<T>(T Value, string Label);
    private static void SelectOption<T>(ComboBox box, T value) => box.SelectedIndex = box.Items.Cast<Option<T>>().ToList().FindIndex(item => EqualityComparer<T>.Default.Equals(item.Value, value));
    private static void SelectReference(ComboBox box, Guid? id)
    {
        var choices = (List<Option<Guid?>>)box.DataSource!;
        if (id.HasValue && !choices.Any(item => item.Value == id)) box.DataSource = choices.Concat(new[] { new Option<Guid?>(id, AppStrings.MissingActionLink) }).ToList();
        SelectOption(box, id);
    }

}
