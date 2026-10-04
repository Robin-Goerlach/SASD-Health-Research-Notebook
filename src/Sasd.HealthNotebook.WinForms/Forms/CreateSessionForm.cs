using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.WinForms.Localization;

namespace Sasd.HealthNotebook.WinForms.Forms;

/// <summary>Accessible appointment editor; invariants remain in Domain/Application.</summary>
public sealed class CreateSessionForm : SourceRecordForm
{
    private readonly SessionService _service;
    private readonly Session? _existing;
    private readonly DateTimePicker _datePicker = new() { Format = DateTimePickerFormat.Custom };
    private readonly DateTimePicker _timePicker = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "HH:mm", ShowUpDown = true };
    private readonly TextBox _titleTextBox = new() { MaxLength = Session.MaximumTitleLength };
    private readonly TextBox _contactTextBox = new() { MaxLength = Session.MaximumTitleLength };
    private readonly TextBox _notesTextBox = TextEditor(Session.MaximumTextLength);
    private readonly ComboBox _typeComboBox = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _statusComboBox = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _topicComboBox = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    /// <summary>Uses shared summaries for the optional topic choice.</summary>
    public CreateSessionForm(SessionService service, IReadOnlyList<HealthTopicSummary> topics, Session? existing = null)
        : base(existing is null ? AppStrings.NewSession : AppStrings.EditSession, AppStrings.SessionSemantics, new Size(820, 760))
    {
        _existing = existing;
        _service = service ?? throw new ArgumentNullException(nameof(service)); ArgumentNullException.ThrowIfNull(topics);
        _datePicker.CustomFormat = AppLanguage.Current == UiLanguage.German ? "dd.MM.yyyy" : "MM/dd/yyyy";
        _typeComboBox.DataSource = Enum.GetValues<SessionType>().Select(type => new TypeChoice(type, AppStrings.SessionTypeText(type))).ToList(); _typeComboBox.DisplayMember = nameof(TypeChoice.Label);
        _statusComboBox.DataSource = Enum.GetValues<SessionStatus>().Select(status => new StatusChoice(status, AppStrings.SessionStatusText(status))).ToList(); _statusComboBox.DisplayMember = nameof(StatusChoice.Label);
        _topicComboBox.DataSource = new[] { new TopicChoice(null, AppStrings.NoEntryTopic) }.Concat(topics.Select(topic => new TopicChoice(topic.Id, topic.Title))).ToList(); _topicComboBox.DisplayMember = nameof(TopicChoice.Label);
        AddField(AppStrings.EntryDate, _datePicker, AppStrings.EntryTimeHelp); AddField(AppStrings.EntryClock, _timePicker, AppStrings.EntryTimeHelp);
        AddField(AppStrings.SessionTitle, _titleTextBox, AppStrings.SessionTitle); AddField(AppStrings.SessionKind, _typeComboBox, AppStrings.SessionKind);
        AddField(AppStrings.SessionState, _statusComboBox, AppStrings.SessionState); AddField(AppStrings.EntryTopic, _topicComboBox, AppStrings.EntryTopicHelp);
        AddField(AppStrings.SessionContact, _contactTextBox, AppStrings.SessionContact); AddField(AppStrings.SessionNotes, _notesTextBox, AppStrings.SessionSemantics, true);
        Shown += (_, _) => _titleTextBox.Focus();
        // Data-bound Items are ready at Load, before users can interact with the editor.
        Load += (_, _) =>
        {
            if (existing is not null)
            {
                SetEditMode();
                _datePicker.Value = _timePicker.Value = existing.ScheduledAt.LocalDateTime;
                _titleTextBox.Text = existing.Title; _contactTextBox.Text = existing.ContactText; _notesTextBox.Text = existing.Notes;
                _typeComboBox.SelectedIndex = _typeComboBox.Items.Cast<TypeChoice>().ToList().FindIndex(item => item.Type == existing.SessionType);
                _statusComboBox.SelectedIndex = _statusComboBox.Items.Cast<StatusChoice>().ToList().FindIndex(item => item.Status == existing.Status);
                var choices = (List<TopicChoice>)_topicComboBox.DataSource!;
                if (existing.HealthTopicId.HasValue && !choices.Any(item => item.Id == existing.HealthTopicId))
                    _topicComboBox.DataSource = choices.Concat(new[] { new TopicChoice(existing.HealthTopicId, AppStrings.MissingEntryTopic) }).ToList();
                _topicComboBox.SelectedIndex = _topicComboBox.Items.Cast<TopicChoice>().ToList().FindIndex(item => item.Id == existing.HealthTopicId);
            }
        };
    }
    /// <summary>New ID to select after refresh.</summary>
    public Guid? CreatedSessionId { get; private set; }
    protected override string ValidationMessage => AppStrings.SessionValidation;
    protected override string SaveOperation => AppStrings.SaveSessionOperation;
    protected override async Task SaveRecordAsync()
    {
        var instant = EditSupport.Instant(_datePicker.Value, _timePicker.Value, _existing?.ScheduledAt);
        var request = new CreateSessionRequest(instant, _titleTextBox.Text,
            ((TypeChoice)_typeComboBox.SelectedItem!).Type, ((StatusChoice)_statusComboBox.SelectedItem!).Status,
            ((TopicChoice)_topicComboBox.SelectedItem!).Id, EditSupport.Optional(_contactTextBox.Text, _existing?.ContactText), EditSupport.Optional(_notesTextBox.Text, _existing?.Notes));

        if (_existing is not null)
        {
            await _service.UpdateSessionAsync(_existing.Id, request, _existing.ModifiedAt);
            CreatedSessionId = _existing.Id; return;
        }
        var session = await _service.CreateSessionAsync(request);
        CreatedSessionId = session.Id;
    }
    private sealed record TypeChoice(SessionType Type, string Label);
    private sealed record StatusChoice(SessionStatus Status, string Label);
    private sealed record TopicChoice(Guid? Id, string Label);
}
