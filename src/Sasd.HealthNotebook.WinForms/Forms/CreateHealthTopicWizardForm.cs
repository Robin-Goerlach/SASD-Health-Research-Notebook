using System.Drawing;
using System.Windows.Forms;
using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.WinForms.Localization;
using Sasd.HealthNotebook.WinForms.Styling;

namespace Sasd.HealthNotebook.WinForms.Forms;

/// <summary>
/// Windows Forms wizard for creating a health topic.
/// </summary>
/// <remarks>
/// Inputs remain in memory until the final review is confirmed. The shared service
/// owns identity, timestamps and persistence. No medical decisions are derived.
/// </remarks>
public sealed class CreateHealthTopicWizardForm : Form
{
    private readonly HealthTopicService _healthTopicService;
    private readonly ToolTip _toolTip;
    private readonly List<WizardStepText> _steps;
    private readonly ListBox _stepsListBox;
    private readonly Label _stepTitleLabel;
    private readonly Label _stepDescriptionLabel;
    private readonly Panel _basicDataPanel;
    private readonly Panel _classificationPanel;
    private readonly Panel _notesPanel;
    private readonly Panel _summaryPanel;
    private readonly TextBox _summaryTextBox;
    private readonly Label _validationLabel;
    private readonly Button _cancelButton;
    private bool _saving;
    private bool _saved;

    private readonly TextBox _titleTextBox;
    private readonly ComboBox _statusComboBox;
    private readonly ComboBox _priorityComboBox;
    private readonly TextBox _shortDescriptionTextBox;
    private readonly TextBox _notesTextBox;
    private readonly Button _backButton;
    private readonly Button _nextButton;
    private int _currentStepIndex;

    /// <summary>Identity returned by the shared service after a successful final save.</summary>
    public Guid? CreatedTopicId { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateHealthTopicWizardForm" /> class.
    /// </summary>
    public CreateHealthTopicWizardForm(HealthTopicService healthTopicService)
    {
        _healthTopicService = healthTopicService ?? throw new ArgumentNullException(nameof(healthTopicService));
        _toolTip = new ToolTip
        {
            AutoPopDelay = 15000,
            InitialDelay = 400,
            ReshowDelay = 100,
            ShowAlways = true
        };
        _steps = AppStrings.CreateWizardSteps().ToList();

        AutoScaleDimensions = new SizeF(96, 96);
        AutoScaleMode = AutoScaleMode.Dpi;
        Text = AppStrings.NewHealthTopic;
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(960, 660);
        Size = new Size(1040, 720);
        BackColor = UiColors.WindowBackground;
        Font = UiFonts.Body;

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2,
            Padding = new Padding(UiMetrics.Padding),
            BackColor = UiColors.WindowBackground
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 68));

        _stepsListBox = new ListBox
        {
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.FixedSingle,
            IntegralHeight = false,
            Font = UiFonts.Body,
            DrawMode = DrawMode.OwnerDrawFixed,
            ItemHeight = UiMetrics.NavigationButtonHeight,
            TabIndex = 0,
            TabStop = false
        };
        _stepsListBox.DrawItem += DrawStep;
        _stepsListBox.Items.AddRange(_steps.Select(step => step.Title).Cast<object>().ToArray());
        // Progress is informational; Back/Next are the only navigation commands.
        _stepsListBox.SelectedIndexChanged += (_, _) =>
        {
            if (_stepsListBox.SelectedIndex != _currentStepIndex)
                _stepsListBox.SelectedIndex = _currentStepIndex;
        };

        var rightPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(UiMetrics.LargeSpacing, 0, 0, 0),
            TabIndex = 1,
            BackColor = UiColors.WindowBackground
        };
        rightPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
        rightPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 76));
        rightPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        _stepTitleLabel = new Label
        {
            Dock = DockStyle.Fill,
            Font = UiFonts.PageTitle,
            ForeColor = UiColors.PrimaryText,
            TextAlign = ContentAlignment.BottomLeft
        };

        _stepDescriptionLabel = new Label
        {
            Dock = DockStyle.Fill,
            Font = UiFonts.Body,
            ForeColor = UiColors.SecondaryText,
            TextAlign = ContentAlignment.TopLeft
        };

        var contentPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = UiColors.CardBackground,
            Padding = new Padding(UiMetrics.Padding)
        };

        _basicDataPanel = CreateBasicDataPanel(
            out _titleTextBox,
            out _statusComboBox,
            out _priorityComboBox,
            out _shortDescriptionTextBox,
            out _notesTextBox);

        _classificationPanel = CreateFieldPanel(
            (AppStrings.FieldStatus, _statusComboBox, AppStrings.ToolTipStatus),
            (AppStrings.FieldPriority, _priorityComboBox, AppStrings.ToolTipPriority));
        _notesPanel = CreateFieldPanel((AppStrings.FieldNotes, _notesTextBox, AppStrings.ToolTipNotes));
        _summaryTextBox = new TextBox { Dock = DockStyle.Fill, Multiline = true, ReadOnly = true,
            ScrollBars = ScrollBars.Vertical, BackColor = UiColors.CardBackground,
            BorderStyle = BorderStyle.None, AccessibleName = AppStrings.WizardReviewTitle };
        _summaryPanel = new Panel { Dock = DockStyle.Fill };
        _summaryPanel.Controls.Add(_summaryTextBox);
        _validationLabel = new Label { Dock = DockStyle.Bottom, Height = 52,
            ForeColor = UiColors.SecondaryText, AccessibleName = AppStrings.WizardValidationLabel };
        contentPanel.Controls.AddRange(new Control[] { _basicDataPanel, _classificationPanel, _notesPanel, _summaryPanel });
        contentPanel.Controls.Add(_validationLabel);

        rightPanel.Controls.Add(_stepTitleLabel, 0, 0);
        rightPanel.Controls.Add(_stepDescriptionLabel, 0, 1);
        rightPanel.Controls.Add(contentPanel, 0, 2);

        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            BackColor = UiColors.WindowBackground,
            Padding = new Padding(0, UiMetrics.StandardSpacing, 0, 0),
            WrapContents = false,
            TabIndex = 2
        };

        _cancelButton = new Button
        {
            Text = AppStrings.Cancel,
            Width = 110,
            Height = UiMetrics.ActionHeight,
            DialogResult = DialogResult.None,
            TabIndex = 2
        };
        _cancelButton.Click += (_, _) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };

        _nextButton = new Button
        {
            Width = 150,
            Height = UiMetrics.ActionHeight,
            BackColor = UiColors.PrimaryAccent,
            ForeColor = UiColors.CardBackground,
            FlatStyle = FlatStyle.Flat,
            TabIndex = 1
        };
        _nextButton.Click += NextButton_Click;

        _backButton = new Button
        {
            Text = AppStrings.Back,
            Width = 110,
            Height = UiMetrics.ActionHeight,
            TabIndex = 0
        };
        _backButton.Click += (_, _) => ShowStep(_currentStepIndex - 1);

        buttonPanel.Controls.Add(_nextButton);
        buttonPanel.Controls.Add(_backButton);
        buttonPanel.Controls.Add(_cancelButton);
        _nextButton.FlatAppearance.BorderSize = 0;
        AcceptButton = _nextButton;
        CancelButton = _cancelButton;

        root.Controls.Add(_stepsListBox, 0, 0);
        root.SetRowSpan(_stepsListBox, 2);
        root.Controls.Add(rightPanel, 1, 0);
        root.Controls.Add(buttonPanel, 1, 1);

        Controls.Add(root);

        FillStatusComboBox();
        FillPriorityComboBox();

        _titleTextBox.TextChanged += (_, _) => UpdateValidation();
        _shortDescriptionTextBox.TextChanged += (_, _) => UpdateValidation();
        _notesTextBox.TextChanged += (_, _) => UpdateValidation();
        _statusComboBox.SelectedIndexChanged += (_, _) => UpdateValidation();
        _priorityComboBox.SelectedIndexChanged += (_, _) => UpdateValidation();
        FormClosing += (_, args) =>
        {
            // A pending write cannot be cancelled: closing must never report cancellation
            // while the repository is committing a topic.
            if (_saving) { args.Cancel = true; return; }
            if (!_saved && HasChanges() && MessageBox.Show(this, AppStrings.WizardDiscardMessage,
                AppStrings.NewHealthTopic, MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            { args.Cancel = true; DialogResult = DialogResult.None; }
        };
        ShowStep(0);
        Shown += (_, _) => _titleTextBox.Focus();
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _toolTip.Dispose();
        }

        base.Dispose(disposing);
    }

    private Panel CreateBasicDataPanel(
        out TextBox titleTextBox,
        out ComboBox statusComboBox,
        out ComboBox priorityComboBox,
        out TextBox shortDescriptionTextBox,
        out TextBox notesTextBox)
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = UiColors.CardBackground,
            AutoScroll = true
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            ColumnCount = 2,
            RowCount = 2,
            AutoSize = true,
            BackColor = UiColors.CardBackground
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 36));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 64));

        titleTextBox = new TextBox { Dock = DockStyle.Fill, MaxLength = 160, TabIndex = 0 };
        statusComboBox = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, TabIndex = 1 };
        priorityComboBox = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, TabIndex = 2 };
        shortDescriptionTextBox = new TextBox { Dock = DockStyle.Fill, MaxLength = 500, TabIndex = 3 };
        notesTextBox = new TextBox
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            Height = 150,
            ScrollBars = ScrollBars.Vertical,
            MaxLength = 4000,
            TabIndex = 4,
            AcceptsReturn = true
        };

        AddRow(layout, 0, AppStrings.FieldTitle, titleTextBox, AppStrings.ToolTipHealthTopicTitle);
        AddRow(layout, 1, AppStrings.FieldShortDescription, shortDescriptionTextBox, AppStrings.ToolTipShortDescription);
        panel.Controls.Add(layout);
        return panel;
    }

    private Panel CreateFieldPanel(params (string Label, Control Editor, string Help)[] fields)
    {
        var panel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = UiColors.CardBackground };
        var layout = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, RowCount = fields.Length };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 36));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 64));
        for (int index = 0; index < fields.Length; index++)
            AddRow(layout, index, fields[index].Label, fields[index].Editor, fields[index].Help);
        panel.Controls.Add(layout);
        return panel;
    }

    private void AddRow(TableLayoutPanel layout, int rowIndex, string labelText, Control editor, string helpText)
    {
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var label = new Label
        {
            Text = labelText,
            Dock = DockStyle.Fill,
            AutoSize = true,
            MinimumSize = new Size(0, 34),
            TextAlign = ContentAlignment.TopLeft,
            Padding = new Padding(0, 6, UiMetrics.StandardSpacing, 0),
            TabStop = false,
            ForeColor = UiColors.PrimaryText,
            Font = UiFonts.Body
        };

        editor.Margin = new Padding(0, 4, 0, 8);
        editor.AccessibleName = labelText;
        editor.AccessibleDescription = helpText;

        _toolTip.SetToolTip(label, helpText);
        _toolTip.SetToolTip(editor, helpText);

        layout.Controls.Add(label, 0, rowIndex);
        layout.Controls.Add(editor, 1, rowIndex);
    }

    private void DrawStep(object? sender, DrawItemEventArgs e)
    {
        if (e.Index < 0) { return; }
        bool selected = (e.State & DrawItemState.Selected) != 0;
        using var background = new SolidBrush(selected ? UiColors.ListSelectionBackground : UiColors.CardBackground);
        e.Graphics.FillRectangle(background, e.Bounds);
        Rectangle textBounds = Rectangle.Inflate(e.Bounds, -UiMetrics.StandardSpacing, -3);
        TextRenderer.DrawText(e.Graphics, _steps[e.Index].Title, _stepsListBox.Font,
            textBounds, selected ? UiColors.PrimaryAccent : UiColors.PrimaryText,
            TextFormatFlags.WordBreak | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        e.DrawFocusRectangle();
    }

    private void FillStatusComboBox()
    {
        var values = Enum.GetValues<HealthTopicStatus>()
            .Select(value => new EnumDisplayItem<HealthTopicStatus>(value, AppStrings.HealthTopicStatusText(value)))
            .Cast<object>()
            .ToArray();

        _statusComboBox.Items.AddRange(values);
        _statusComboBox.SelectedIndex = 0;
    }

    private void FillPriorityComboBox()
    {
        var values = Enum.GetValues<HealthTopicPriority>()
            .Select(value => new EnumDisplayItem<HealthTopicPriority>(value, AppStrings.HealthTopicPriorityText(value)))
            .Cast<object>()
            .ToArray();

        _priorityComboBox.Items.AddRange(values);
        _priorityComboBox.SelectedIndex = 0;
    }

    private void ShowStep(int stepIndex)
    {
        if (_saving || _saved) return;
        // Move focus before hiding a panel so it never remains in an invisible editor.
        _cancelButton.Focus();
        _currentStepIndex = Math.Clamp(stepIndex, 0, _steps.Count - 1);
        WizardStepText step = _steps[_currentStepIndex];
        _stepTitleLabel.Text = step.Title;
        _stepDescriptionLabel.Text = step.Description;
        _stepsListBox.SelectedIndex = _currentStepIndex;
        _basicDataPanel.Visible = _currentStepIndex == 0;
        _classificationPanel.Visible = _currentStepIndex == 1;
        _notesPanel.Visible = _currentStepIndex == 2;
        _summaryPanel.Visible = _currentStepIndex == 3;
        _backButton.Enabled = _currentStepIndex > 0;
        _nextButton.Text = _currentStepIndex == 3 ? AppStrings.WizardFinish : AppStrings.Next;
        if (_currentStepIndex == 3)
        {
            _summaryTextBox.Text = string.Join(Environment.NewLine + Environment.NewLine,
                $"{AppStrings.FieldTitle}: {_titleTextBox.Text.Trim()}",
                $"{AppStrings.FieldStatus}: {_statusComboBox.SelectedItem}",
                $"{AppStrings.FieldPriority}: {_priorityComboBox.SelectedItem}",
                $"{AppStrings.FieldShortDescription}: {_shortDescriptionTextBox.Text.Trim()}",
                string.IsNullOrWhiteSpace(_notesTextBox.Text) ? string.Empty : $"{AppStrings.FieldNotes}: {_notesTextBox.Text.Trim()}").Trim();
            _summaryTextBox.SelectionStart = 0;
        }
        UpdateValidation();
        Control focus = _currentStepIndex switch { 0 => _titleTextBox, 1 => _statusComboBox, 2 => _notesTextBox, _ => _summaryTextBox };
        focus.Focus();
    }

    private string ValidationForStep(int step) => step switch
    {
        0 when string.IsNullOrWhiteSpace(_titleTextBox.Text) => AppStrings.MissingTitleMessage,
        0 when _titleTextBox.Text.Trim().Length > 160 || _shortDescriptionTextBox.Text.Length > 500 => AppStrings.WizardTextLimits,
        1 when _statusComboBox.SelectedItem is not EnumDisplayItem<HealthTopicStatus> status || !Enum.IsDefined(status.Value)
            || _priorityComboBox.SelectedItem is not EnumDisplayItem<HealthTopicPriority> priority || !Enum.IsDefined(priority.Value) => AppStrings.WizardChooseClassification,
        2 when _notesTextBox.Text.Length > 4000 => AppStrings.WizardTextLimits,
        _ => string.Empty
    };

    private void UpdateValidation()
    {
        string message = _currentStepIndex == 3
            ? Enumerable.Range(0, 3).Select(ValidationForStep).FirstOrDefault(text => text.Length > 0) ?? string.Empty
            : ValidationForStep(_currentStepIndex);
        _validationLabel.Text = message;
        _nextButton.Enabled = !_saving && !_saved && message.Length == 0;
    }

    private bool HasChanges() => _titleTextBox.Text.Length > 0 || _shortDescriptionTextBox.Text.Length > 0
        || _notesTextBox.Text.Length > 0 || _statusComboBox.SelectedIndex != 0 || _priorityComboBox.SelectedIndex != 0;

    private async void NextButton_Click(object? sender, EventArgs e)
    {
        if (_saving || _saved) return;
        UpdateValidation();
        if (!_nextButton.Enabled) return;
        if (_currentStepIndex < 3) { ShowStep(_currentStepIndex + 1); return; }
        await CreateTopicSafeAsync();
    }

    private async Task CreateTopicSafeAsync()
    {
        if (_saving || _saved || _currentStepIndex != 3) return;
        for (int step = 0; step < 3; step++)
            if (ValidationForStep(step).Length > 0) { ShowStep(step); return; }

        _saving = true;
        _nextButton.Enabled = _backButton.Enabled = _cancelButton.Enabled = false;
        try
        {
            var request = new CreateHealthTopicRequest
            {
                Title = _titleTextBox.Text,
                Status = ((EnumDisplayItem<HealthTopicStatus>)_statusComboBox.SelectedItem!).Value,
                Priority = ((EnumDisplayItem<HealthTopicPriority>)_priorityComboBox.SelectedItem!).Value,
                ShortDescription = _shortDescriptionTextBox.Text,
                Notes = _notesTextBox.Text
            };
            var created = await _healthTopicService.CreateTopicAsync(request).ConfigureAwait(true);
            CreatedTopicId = created.Id;
            _saved = true;
        }
        catch (ArgumentException) { _validationLabel.Text = AppStrings.TopicEditValidation; }
        catch { _validationLabel.Text = AppStrings.FormatSafeError(AppStrings.OperationCreateHealthTopic); }
        finally { _saving = false; }
        if (_saved) { DialogResult = DialogResult.OK; Close(); }
        else
        {
            _backButton.Enabled = _cancelButton.Enabled = true;
            _nextButton.Enabled = true;
            _nextButton.Focus();
        }
    }

    private sealed class EnumDisplayItem<TEnum>
        where TEnum : struct, Enum
    {
        public EnumDisplayItem(TEnum value, string text)
        {
            Value = value;
            Text = text;
        }

        public TEnum Value { get; }

        private string Text { get; }

        public override string ToString() => Text;
    }
}
