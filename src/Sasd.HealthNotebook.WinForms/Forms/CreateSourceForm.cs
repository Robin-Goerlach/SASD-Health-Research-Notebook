using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.WinForms.Localization;

namespace Sasd.HealthNotebook.WinForms.Forms;

/// <summary>Functional metadata dialog using the shared source service.</summary>
public sealed class CreateSourceForm : SourceRecordForm
{
    private readonly SourceService _service;
    private readonly ComboBox _typeComboBox = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox _titleTextBox = new() { MaxLength = Source.MaximumTitleLength };
    private readonly TextBox _urlTextBox = new() { MaxLength = Source.MaximumUrlLength };
    private readonly TextBox _authorTextBox = new() { MaxLength = Source.MaximumAuthorLength };
    private readonly DateTimePicker _publicationPicker = DateEditor();
    private readonly DateTimePicker _accessedPicker = DateEditor();
    private readonly TextBox _identifierTextBox = new() { MaxLength = Source.MaximumIdentifierLength };
    private readonly ComboBox _topicComboBox = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    /// <summary>Offers existing topics without duplicating their Domain type.</summary>
    public CreateSourceForm(SourceService service, IReadOnlyList<HealthTopicSummary> topics)
        : base(AppStrings.NewSource, AppStrings.SourceMetadataHelp, new Size(820, 650))
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        ArgumentNullException.ThrowIfNull(topics);
        _typeComboBox.DataSource = Enum.GetValues<SourceType>().Select(type => new TypeChoice(type, AppStrings.SourceTypeText(type))).ToList();
        _typeComboBox.DisplayMember = nameof(TypeChoice.Label);
        _topicComboBox.DataSource = new[] { new TopicChoice(null, AppStrings.NoEntryTopic) }
            .Concat(topics.Select(topic => new TopicChoice(topic.Id, topic.Title))).ToList();
        _topicComboBox.DisplayMember = nameof(TopicChoice.Label);
        AddField(AppStrings.EntryType, _typeComboBox, AppStrings.SourceSemantics);
        AddField(AppStrings.SourceTitle, _titleTextBox, AppStrings.SourceMetadataHelp);
        AddField(AppStrings.SourceUrl, _urlTextBox, AppStrings.SourceMetadataHelp);
        AddField(AppStrings.SourceAuthor, _authorTextBox, AppStrings.SourceMetadataHelp);
        AddField(AppStrings.SourcePublicationDate, _publicationPicker, AppStrings.SourceDateHelp);
        AddField(AppStrings.SourceAccessedAt, _accessedPicker, AppStrings.SourceDateHelp);
        AddField(AppStrings.SourceIdentifier, _identifierTextBox, AppStrings.SourceIdentifier);
        AddField(AppStrings.EntryTopic, _topicComboBox, AppStrings.EntryTopicHelp);
        Shown += (_, _) => _titleTextBox.Focus();
    }
    /// <inheritdoc />
    protected override async Task SaveRecordAsync()
    {
        var created = await _service.CreateSourceAsync(new CreateSourceRequest {
        SourceType = ((TypeChoice)_typeComboBox.SelectedItem!).Type, Title = _titleTextBox.Text, Url = _urlTextBox.Text,
        AuthorOrInstitution = _authorTextBox.Text, PublicationDate = DateValue(_publicationPicker), AccessedAt = DateValue(_accessedPicker),
        ExternalIdentifier = _identifierTextBox.Text, HealthTopicId = ((TopicChoice)_topicComboBox.SelectedItem!).Id });
        CreatedSourceId = created.Id;
    }
    /// <summary>Allows the caller to select the newly created source after refreshing.</summary>
    public Guid? CreatedSourceId { get; private set; }
    private static DateTimePicker DateEditor() => new() { Format = DateTimePickerFormat.Custom,
        CustomFormat = AppLanguage.Current == UiLanguage.German ? "dd.MM.yyyy" : "MM/dd/yyyy", ShowCheckBox = true, Checked = false };
    private static DateOnly? DateValue(DateTimePicker picker) => picker.Checked ? DateOnly.FromDateTime(picker.Value) : null;
    private sealed record TypeChoice(SourceType Type, string Label);
    private sealed record TopicChoice(Guid? Id, string Label);
}
