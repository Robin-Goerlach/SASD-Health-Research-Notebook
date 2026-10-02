using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.WinForms.Localization;

namespace Sasd.HealthNotebook.WinForms.Forms;

/// <summary>Accessible appointment editor; invariants remain in Domain/Application.</summary>
public sealed class CreateSessionForm : SourceRecordForm
{
    private readonly SessionService _service;
    private readonly DateTimePicker _datePicker = new() { Format = DateTimePickerFormat.Custom };
    private readonly DateTimePicker _timePicker = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "HH:mm", ShowUpDown = true };
    private readonly TextBox _titleTextBox = new() { MaxLength = Session.MaximumTitleLength };
    private readonly TextBox _contactTextBox = new() { MaxLength = Session.MaximumTitleLength };
    private readonly TextBox _notesTextBox = TextEditor(Session.MaximumTextLength);
    private readonly ComboBox _typeComboBox = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _statusComboBox = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _topicComboBox = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    /// <summary>Uses shared summaries for the optional topic choice.</summary>
    public CreateSessionForm(SessionService service, IReadOnlyList<HealthTopicSummary> topics)
        : base(AppStrings.NewSession, AppStrings.SessionSemantics, new Size(820, 760))
    {
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
    }
    /// <summary>New ID to select after refresh.</summary>
    public Guid? CreatedSessionId { get; private set; }
    protected override string ValidationMessage => AppStrings.SessionValidation;
    protected override string SaveOperation => AppStrings.SaveSessionOperation;
    protected override async Task SaveRecordAsync()
    {
        var local = DateTime.SpecifyKind(_datePicker.Value.Date + _timePicker.Value.TimeOfDay, DateTimeKind.Unspecified);
        if (TimeZoneInfo.Local.IsInvalidTime(local) || TimeZoneInfo.Local.IsAmbiguousTime(local)) throw new ArgumentException("Ambiguous or invalid local time.");
        var session = await _service.CreateSessionAsync(new(new DateTimeOffset(local, TimeZoneInfo.Local.GetUtcOffset(local)), _titleTextBox.Text,
            ((TypeChoice)_typeComboBox.SelectedItem!).Type, ((StatusChoice)_statusComboBox.SelectedItem!).Status,
            ((TopicChoice)_topicComboBox.SelectedItem!).Id, _contactTextBox.Text, _notesTextBox.Text));
        CreatedSessionId = session.Id;
    }
    private sealed record TypeChoice(SessionType Type, string Label);
    private sealed record StatusChoice(SessionStatus Status, string Label);
    private sealed record TopicChoice(Guid? Id, string Label);
}
