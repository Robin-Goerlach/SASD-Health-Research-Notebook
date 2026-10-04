using System.Globalization;
using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.WinForms.Localization;
using Sasd.HealthNotebook.WinForms.Styling;

namespace Sasd.HealthNotebook.WinForms.Forms;

/// <summary>Manual numeric entry dialog; Domain/Application own all measurement invariants.</summary>
public sealed class CreateMeasurementForm : Form
{
    private readonly MeasurementService _service;
    private readonly DateTimePicker _datePicker = new() { Format = DateTimePickerFormat.Custom };
    private readonly DateTimePicker _timePicker = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "HH:mm", ShowUpDown = true };
    private readonly ComboBox _typeComboBox = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _topicComboBox = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox _valueTextBox = new();
    private readonly TextBox _diastolicTextBox = new();
    private readonly TextBox _pulseTextBox = new();
    private readonly TextBox _contextTextBox = new() { MaxLength = Measurement.MaximumContextLength };
    private readonly TextBox _noteTextBox = new() { Multiline = true, AcceptsReturn = true, ScrollBars = ScrollBars.Vertical, MaxLength = Measurement.MaximumNoteLength };
    private readonly Label _primaryLabel = new();
    private readonly Label _diastolicLabel = new();
    private readonly Label _pulseLabel = new();
    private readonly TableLayoutPanel _valuesPanel;
    private readonly TableLayoutPanel _root;
    private readonly Button _saveButton;
    private readonly Label _validationLabel;
    private readonly ToolTip _toolTip = new() { AutoPopDelay = 15000 };
    private bool _saving;
    private readonly Measurement? _existing;
    /// <summary>Uses shared Application summaries for optional linkage.</summary>
    public CreateMeasurementForm(MeasurementService service, IReadOnlyList<HealthTopicSummary> topics, Measurement? existing = null)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service)); ArgumentNullException.ThrowIfNull(topics);
        _existing = existing;
        // Establish binding before selecting prefilled choices; otherwise Items is still empty.
        BindingContext = new BindingContext();
        AutoScaleDimensions = new SizeF(96, 96); AutoScaleMode = AutoScaleMode.Dpi;
        Text = AppStrings.NewMeasurement; StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(800, 720); Size = new Size(840, 760); Font = UiFonts.Body; BackColor = UiColors.WindowBackground;
        _datePicker.CustomFormat = AppLanguage.Current == UiLanguage.German ? "dd.MM.yyyy" : "MM/dd/yyyy";
        _root = CreateLayout(9); _root.Padding = new Padding(UiMetrics.LargeSpacing);
        for (int row = 0; row < 3; row++) _root.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        _root.RowStyles.Add(new RowStyle(SizeType.Absolute, 126));
        _root.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        _root.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        _root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        _root.RowStyles.Add(new RowStyle(SizeType.Absolute, 66));
        _root.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        AddField(_root, 0, AppStrings.EntryDate, _datePicker, AppStrings.EntryTimeHelp);
        AddField(_root, 1, AppStrings.EntryClock, _timePicker, AppStrings.EntryTimeHelp);
        AddField(_root, 2, AppStrings.MeasurementKind, _typeComboBox, AppStrings.MeasurementsDescription);
        _valuesPanel = CreateLayout(3); _valuesPanel.TabIndex = 3;
        for (int row = 0; row < 3; row++) _valuesPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        AddField(_valuesPanel, 0, string.Empty, _valueTextBox, AppStrings.MeasurementNumberHelp, _primaryLabel);
        AddField(_valuesPanel, 1, AppStrings.MeasurementDiastolic, _diastolicTextBox, AppStrings.MeasurementNumberHelp, _diastolicLabel);
        AddField(_valuesPanel, 2, AppStrings.MeasurementPulseOptional, _pulseTextBox, AppStrings.MeasurementNumberHelp, _pulseLabel);
        _root.Controls.Add(_valuesPanel, 0, 3); _root.SetColumnSpan(_valuesPanel, 2);
        AddField(_root, 4, AppStrings.EntryTopic, _topicComboBox, AppStrings.EntryTopicHelp);
        AddField(_root, 5, AppStrings.MeasurementContext, _contextTextBox, AppStrings.MeasurementContextHelp);
        AddField(_root, 6, AppStrings.MeasurementNote, _noteTextBox, AppStrings.MeasurementNoteHelp);
        _typeComboBox.DataSource = Enum.GetValues<MeasurementType>().Select(type => new TypeChoice(type, AppStrings.MeasurementTypeText(type))).ToList();
        _typeComboBox.DisplayMember = nameof(TypeChoice.Label);
        _typeComboBox.SelectedIndexChanged += (_, _) => ApplyMeasurementType();
        _topicComboBox.DataSource = new[] { new TopicChoice(null, AppStrings.NoEntryTopic) }
            .Concat(topics.Select(topic => new TopicChoice(topic.Id, topic.Title))).ToList();
        _topicComboBox.DisplayMember = nameof(TopicChoice.Label);
        _validationLabel = new Label { Dock = DockStyle.Fill, ForeColor = UiColors.SecondaryText };
        _root.Controls.Add(_validationLabel, 0, 7); _root.SetColumnSpan(_validationLabel, 2);
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, WrapContents = false, TabIndex = 7 };
        _saveButton = new Button { Text = AppStrings.SaveMeasurement, AutoSize = true, MinimumSize = new Size(170, UiMetrics.ActionHeight),
            FlatStyle = FlatStyle.Flat, BackColor = UiColors.PrimaryAccent, ForeColor = UiColors.CardBackground, TabIndex = 0 };
        _saveButton.FlatAppearance.BorderSize = 0;
        var cancel = new Button { Text = AppStrings.Cancel, DialogResult = DialogResult.Cancel, Width = 110, Height = UiMetrics.ActionHeight, TabIndex = 1 };
        buttons.Controls.Add(_saveButton); buttons.Controls.Add(cancel); _root.Controls.Add(buttons, 0, 8); _root.SetColumnSpan(buttons, 2);
        Controls.Add(_root); AcceptButton = _saveButton; CancelButton = cancel;
        _saveButton.Click += async (_, _) => await SaveAsync();
        FormClosing += (_, args) => { if (_saving) args.Cancel = true; };
        // Data-bound Items are ready at Load, before users can interact with the editor.
        Load += (_, _) =>
        {
            if (existing is not null)
            {
                Text = AppStrings.EditMeasurement; _saveButton.Text = AppStrings.SaveChanges;
                _datePicker.Value = _timePicker.Value = existing.OccurredAt.LocalDateTime;
                _typeComboBox.SelectedIndex = _typeComboBox.Items.Cast<TypeChoice>().ToList().FindIndex(item => item.Type == existing.MeasurementType);
                ApplyMeasurementType();
                _valueTextBox.Text = (existing.Value ?? existing.Systolic)?.ToString("R", AppStrings.MeasurementCulture) ?? "";
                _diastolicTextBox.Text = existing.Diastolic?.ToString("R", AppStrings.MeasurementCulture) ?? "";
                _pulseTextBox.Text = existing.Pulse?.ToString("R", AppStrings.MeasurementCulture) ?? "";
                _contextTextBox.Text = existing.Context; _noteTextBox.Text = existing.Note;
                var choices = (List<TopicChoice>)_topicComboBox.DataSource!;
                if (existing.HealthTopicId.HasValue && !choices.Any(item => item.Id == existing.HealthTopicId))
                    _topicComboBox.DataSource = choices.Concat(new[] { new TopicChoice(existing.HealthTopicId, AppStrings.MissingEntryTopic) }).ToList();
                _topicComboBox.SelectedIndex = _topicComboBox.Items.Cast<TopicChoice>().ToList().FindIndex(item => item.Id == existing.HealthTopicId);
            }
        };
        if (existing is null) ApplyMeasurementType(); Shown += (_, _) => _valueTextBox.Focus();
    }
    private void ApplyMeasurementType()
    {
        if (_typeComboBox.SelectedItem is not TypeChoice choice) return;
        bool pressure = choice.Type == MeasurementType.BloodPressure;
        _primaryLabel.Text = AppStrings.MeasurementPrimaryLabel(choice.Type); _valueTextBox.AccessibleName = _primaryLabel.Text;
        _diastolicLabel.Visible = _diastolicTextBox.Visible = pressure;
        _pulseLabel.Visible = _pulseTextBox.Visible = pressure;
        _diastolicTextBox.TabStop = _pulseTextBox.TabStop = pressure;
        _root.RowStyles[3].Height = pressure ? 126 : 42;
        _valuesPanel.RowStyles[1].Height = _valuesPanel.RowStyles[2].Height = pressure ? 42 : 0;
        // Changing types starts a fresh numeric input, preventing hidden stale values from being saved.
        _valueTextBox.Clear(); _diastolicTextBox.Clear(); _pulseTextBox.Clear(); _validationLabel.Text = string.Empty;
    }
    private async Task SaveAsync()
    {
        if (_saving) return;
        _saving = true; _saveButton.Enabled = false; _validationLabel.Text = string.Empty;
        bool saved = false;
        try
        {
            var type = ((TypeChoice)_typeComboBox.SelectedItem!).Type;
            bool pressure = type == MeasurementType.BloodPressure;
            var instant = EditSupport.Instant(_datePicker.Value, _timePicker.Value, _existing?.OccurredAt);
            var request = new CreateMeasurementRequest { MeasurementType = type,
                OccurredAt = instant,
                Value = pressure ? null : ParseNumber(_valueTextBox.Text), Systolic = pressure ? ParseNumber(_valueTextBox.Text) : null,
                Diastolic = pressure ? ParseNumber(_diastolicTextBox.Text) : null,
                Pulse = pressure && !string.IsNullOrWhiteSpace(_pulseTextBox.Text) ? ParseNumber(_pulseTextBox.Text) : null,
                HealthTopicId = ((TopicChoice)_topicComboBox.SelectedItem!).Id, Note = EditSupport.Optional(_noteTextBox.Text, _existing?.Note), Context = EditSupport.Optional(_contextTextBox.Text, _existing?.Context) };
            if (_existing is null) await _service.CreateMeasurementAsync(request);
            else await _service.UpdateMeasurementAsync(_existing.Id, request, _existing.ModifiedAt);
            saved = true;
        }
        catch (LifecycleConflictException) { _validationLabel.Text = AppStrings.LifecycleConflict; }
        catch (ArgumentException) { _validationLabel.Text = AppStrings.MeasurementValidationFailed; }
        catch { UiErrorHandler.ShowSafeError(this, AppStrings.OperationSaveMeasurement); }
        finally { _saving = false; _saveButton.Enabled = true; }
        if (saved) { DialogResult = DialogResult.OK; Close(); }
    }
    private static double ParseNumber(string text)
    {
        // Float excludes grouping separators: "36.5" in German must not silently become 365.
        if (!double.TryParse(text, NumberStyles.Float, AppStrings.MeasurementCulture, out double value))
            throw new ArgumentException("Enter a number using the selected language's decimal separator.");
        return value;
    }
    private static TableLayoutPanel CreateLayout(int rows)
    {
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = rows, Margin = Padding.Empty };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 36)); layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 64));
        return layout;
    }
    private void AddField(TableLayoutPanel layout, int row, string text, Control editor, string help, Label? label = null)
    {
        label ??= new Label(); label.Text = text; label.Dock = DockStyle.Fill; label.Padding = new Padding(0, 6, 8, 0);
        label.ForeColor = UiColors.PrimaryText; label.TabStop = false;
        editor.Dock = DockStyle.Fill; editor.TabIndex = row; editor.AccessibleName = text; editor.AccessibleDescription = help;
        _toolTip.SetToolTip(label, help); _toolTip.SetToolTip(editor, help);
        layout.Controls.Add(label, 0, row); layout.Controls.Add(editor, 1, row);
    }
    /// <inheritdoc />
    protected override void Dispose(bool disposing) { if (disposing) _toolTip.Dispose(); base.Dispose(disposing); }
    private sealed record TypeChoice(MeasurementType Type, string Label);
    private sealed record TopicChoice(Guid? Id, string Label);
}
