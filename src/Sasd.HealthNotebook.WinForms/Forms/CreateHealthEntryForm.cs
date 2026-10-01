using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.WinForms.Localization;
using Sasd.HealthNotebook.WinForms.Styling;

namespace Sasd.HealthNotebook.WinForms.Forms;

/// <summary>Small entry dialog; validation and persistence belong to the Application service.</summary>
public sealed class CreateHealthEntryForm : Form
{
    private readonly HealthEntryService _service;
    private readonly DateTimePicker _datePicker = new() { Format = DateTimePickerFormat.Custom };
    private readonly DateTimePicker _timePicker = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "HH:mm", ShowUpDown = true };
    private readonly ComboBox _typeComboBox = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _topicComboBox = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox _titleTextBox = new() { MaxLength = HealthEntry.MaximumTitleLength };
    private readonly TextBox _contentTextBox = new() { Multiline = true, AcceptsReturn = true,
        ScrollBars = ScrollBars.Vertical, MaxLength = HealthEntry.MaximumContentLength };
    private readonly Button _saveButton;
    private readonly Label _validationLabel;
    private readonly ToolTip _toolTip = new() { AutoPopDelay = 15000 };
    private bool _saving;

    /// <summary>Uses existing Application topic summaries for optional linkage.</summary>
    public CreateHealthEntryForm(HealthEntryService service, IReadOnlyList<HealthTopicSummary> topics)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        ArgumentNullException.ThrowIfNull(topics);
        AutoScaleDimensions = new SizeF(96, 96);
        AutoScaleMode = AutoScaleMode.Dpi;
        Text = AppStrings.NewTimelineEntry;
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(760, 620);
        Size = new Size(820, 680);
        Font = UiFonts.Body;
        BackColor = UiColors.WindowBackground;
        _datePicker.CustomFormat = AppLanguage.Current == UiLanguage.German ? "dd.MM.yyyy" : "MM/dd/yyyy";
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 8,
            Padding = new Padding(UiMetrics.LargeSpacing) };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68));
        for (int row = 0; row < 5; row++) root.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 66));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        AddField(root, 0, AppStrings.EntryDate, _datePicker, AppStrings.EntryTimeHelp);
        AddField(root, 1, AppStrings.EntryClock, _timePicker, AppStrings.EntryTimeHelp);
        AddField(root, 2, AppStrings.EntryType, _typeComboBox, AppStrings.TimelineDescription);
        AddField(root, 3, AppStrings.EntryTopic, _topicComboBox, AppStrings.EntryTopicHelp);
        AddField(root, 4, AppStrings.EntryTitle, _titleTextBox, AppStrings.EntryTitleHelp);
        AddField(root, 5, AppStrings.EntryContent, _contentTextBox, AppStrings.EntryContentHelp);
        _typeComboBox.DataSource = Enum.GetValues<HealthEntryType>().Select(type =>
            new TypeChoice(type, AppStrings.HealthEntryTypeText(type))).ToList();
        _typeComboBox.DisplayMember = nameof(TypeChoice.Label);
        _topicComboBox.DataSource = new[] { new TopicChoice(null, AppStrings.NoEntryTopic) }
            .Concat(topics.Select(topic => new TopicChoice(topic.Id, topic.Title))).ToList();
        _topicComboBox.DisplayMember = nameof(TopicChoice.Label);
        _validationLabel = new Label { Dock = DockStyle.Fill, ForeColor = UiColors.SecondaryText };
        root.Controls.Add(_validationLabel, 0, 6); root.SetColumnSpan(_validationLabel, 2);
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false, TabIndex = 6 };
        _saveButton = new Button { Text = AppStrings.SaveEntry, AutoSize = true, MinimumSize = new Size(160, UiMetrics.ActionHeight),
            BackColor = UiColors.PrimaryAccent, ForeColor = UiColors.CardBackground, FlatStyle = FlatStyle.Flat, TabIndex = 0 };
        _saveButton.FlatAppearance.BorderSize = 0;
        var cancel = new Button { Text = AppStrings.Cancel, DialogResult = DialogResult.Cancel,
            Width = 110, Height = UiMetrics.ActionHeight, TabIndex = 1 };
        _saveButton.Click += async (_, _) => await SaveAsync();
        buttons.Controls.Add(_saveButton); buttons.Controls.Add(cancel);
        root.Controls.Add(buttons, 0, 7); root.SetColumnSpan(buttons, 2);
        Controls.Add(root);
        AcceptButton = _saveButton; CancelButton = cancel;
        FormClosing += (_, args) => { if (_saving) args.Cancel = true; };
        Shown += (_, _) => _titleTextBox.Focus();
    }
    private void AddField(TableLayoutPanel layout, int row, string text, Control editor, string help)
    {
        var label = new Label { Text = text, Dock = DockStyle.Fill, Padding = new Padding(0, 6, 8, 0),
            ForeColor = UiColors.PrimaryText, TabStop = false };
        editor.Dock = DockStyle.Fill; editor.TabIndex = row;
        editor.AccessibleName = text; editor.AccessibleDescription = help;
        _toolTip.SetToolTip(editor, help); _toolTip.SetToolTip(label, help);
        layout.Controls.Add(label, 0, row); layout.Controls.Add(editor, 1, row);
    }
    private async Task SaveAsync()
    {
        if (_saving) return;
        _saving = true; _saveButton.Enabled = false;
        _validationLabel.Text = string.Empty;
        bool saved = false;
        try
        {
            // Date/time editors are local presentation values. Reject clock-transition
            // gaps/ambiguities rather than silently assigning a different instant.
            DateTime localTime = DateTime.SpecifyKind(_datePicker.Value.Date
                .AddHours(_timePicker.Value.Hour).AddMinutes(_timePicker.Value.Minute), DateTimeKind.Unspecified);
            if (TimeZoneInfo.Local.IsInvalidTime(localTime) || TimeZoneInfo.Local.IsAmbiguousTime(localTime))
                throw new ArgumentException("Local time is ambiguous or invalid.");
            await _service.CreateEntryAsync(new CreateHealthEntryRequest { Title = _titleTextBox.Text,
                Content = _contentTextBox.Text, EntryType = ((TypeChoice)_typeComboBox.SelectedItem!).Type,
                HealthTopicId = ((TopicChoice)_topicComboBox.SelectedItem!).Id,
                OccurredAt = new DateTimeOffset(localTime, TimeZoneInfo.Local.GetUtcOffset(localTime)) });
            saved = true;
        }
        catch (ArgumentException) { _validationLabel.Text = AppStrings.EntryValidationFailed; }
        catch { UiErrorHandler.ShowSafeError(this, AppStrings.OperationSaveEntry); }
        finally { _saving = false; _saveButton.Enabled = true; }
        if (saved) { DialogResult = DialogResult.OK; Close(); }
    }
    /// <inheritdoc />
    protected override void Dispose(bool disposing) { if (disposing) _toolTip.Dispose(); base.Dispose(disposing); }
    private sealed record TypeChoice(HealthEntryType Type, string Label);
    private sealed record TopicChoice(Guid? Id, string Label);
}
