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
/// The first WinForms milestone mirrors the currently functional WPF wizard:
/// only basic topic data are stored now, while later steps are visible as a
/// roadmap. The app does not diagnose, recommend therapy, or derive actions.
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
    private readonly Panel _placeholderPanel;
    private readonly TextBox _titleTextBox;
    private readonly ComboBox _statusComboBox;
    private readonly ComboBox _priorityComboBox;
    private readonly TextBox _shortDescriptionTextBox;
    private readonly TextBox _notesTextBox;
    private readonly Button _backButton;
    private readonly Button _nextButton;
    private int _currentStepIndex;

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
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));

        _stepsListBox = new ListBox
        {
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.FixedSingle,
            IntegralHeight = false,
            Font = UiFonts.Body
        };
        _stepsListBox.Items.AddRange(_steps.Select(step => step.Title).Cast<object>().ToArray());
        _stepsListBox.SelectedIndexChanged += StepsListBox_SelectedIndexChanged;

        var rightPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(UiMetrics.LargeSpacing, 0, 0, 0),
            BackColor = UiColors.WindowBackground
        };
        rightPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
        rightPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 88));
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

        _placeholderPanel = CreatePlaceholderPanel();

        contentPanel.Controls.Add(_placeholderPanel);
        contentPanel.Controls.Add(_basicDataPanel);

        rightPanel.Controls.Add(_stepTitleLabel, 0, 0);
        rightPanel.Controls.Add(_stepDescriptionLabel, 0, 1);
        rightPanel.Controls.Add(contentPanel, 0, 2);

        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            BackColor = UiColors.WindowBackground,
            Padding = new Padding(0, UiMetrics.StandardSpacing, 0, 0)
        };

        var cancelButton = new Button
        {
            Text = AppStrings.Cancel,
            Width = 110,
            Height = 34
        };
        cancelButton.Click += (_, _) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };

        _nextButton = new Button
        {
            Width = 120,
            Height = 34
        };
        _nextButton.Click += NextButton_Click;

        _backButton = new Button
        {
            Text = AppStrings.Back,
            Width = 110,
            Height = 34
        };
        _backButton.Click += (_, _) => ShowStep(_currentStepIndex - 1);

        buttonPanel.Controls.Add(cancelButton);
        buttonPanel.Controls.Add(_nextButton);
        buttonPanel.Controls.Add(_backButton);

        root.Controls.Add(_stepsListBox, 0, 0);
        root.SetRowSpan(_stepsListBox, 2);
        root.Controls.Add(rightPanel, 1, 0);
        root.Controls.Add(buttonPanel, 1, 1);

        Controls.Add(root);

        FillStatusComboBox();
        FillPriorityComboBox();

        ShowStep(0);
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
            BackColor = UiColors.CardBackground
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            ColumnCount = 2,
            RowCount = 5,
            AutoSize = true,
            BackColor = UiColors.CardBackground
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 210));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        titleTextBox = new TextBox { Dock = DockStyle.Fill, MaxLength = 160 };
        statusComboBox = new ComboBox { Dock = DockStyle.Left, Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
        priorityComboBox = new ComboBox { Dock = DockStyle.Left, Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
        shortDescriptionTextBox = new TextBox { Dock = DockStyle.Fill, MaxLength = 500 };
        notesTextBox = new TextBox
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            Height = 150,
            ScrollBars = ScrollBars.Vertical,
            MaxLength = 4000
        };

        AddRow(layout, 0, AppStrings.FieldTitle, titleTextBox, AppStrings.ToolTipHealthTopicTitle);
        AddRow(layout, 1, AppStrings.FieldStatus, statusComboBox, AppStrings.ToolTipStatus);
        AddRow(layout, 2, AppStrings.FieldPriority, priorityComboBox, AppStrings.ToolTipPriority);
        AddRow(layout, 3, AppStrings.FieldShortDescription, shortDescriptionTextBox, AppStrings.ToolTipShortDescription);
        AddRow(layout, 4, AppStrings.FieldNotes, notesTextBox, AppStrings.ToolTipNotes);

        panel.Controls.Add(layout);
        return panel;
    }

    private static Panel CreatePlaceholderPanel()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = UiColors.CardBackground
        };

        var label = new Label
        {
            Dock = DockStyle.Fill,
            Font = UiFonts.Body,
            ForeColor = UiColors.SecondaryText,
            Text = AppStrings.WizardPlaceholder,
            TextAlign = ContentAlignment.MiddleCenter
        };

        panel.Controls.Add(label);
        return panel;
    }

    private void AddRow(TableLayoutPanel layout, int rowIndex, string labelText, Control editor, string helpText)
    {
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var label = new Label
        {
            Text = labelText,
            Dock = DockStyle.Fill,
            Height = rowIndex == 4 ? 160 : 34,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = UiColors.PrimaryText,
            Font = UiFonts.Body
        };

        editor.Margin = new Padding(0, 4, 0, 8);

        _toolTip.SetToolTip(label, helpText);
        _toolTip.SetToolTip(editor, helpText);

        layout.Controls.Add(label, 0, rowIndex);
        layout.Controls.Add(editor, 1, rowIndex);
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

    private void StepsListBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_stepsListBox.SelectedIndex >= 0 && _stepsListBox.SelectedIndex != _currentStepIndex)
        {
            ShowStep(_stepsListBox.SelectedIndex);
        }
    }

    private void ShowStep(int stepIndex)
    {
        _currentStepIndex = Math.Clamp(stepIndex, 0, _steps.Count - 1);

        WizardStepText step = _steps[_currentStepIndex];
        _stepTitleLabel.Text = step.Title;
        _stepDescriptionLabel.Text = step.Description;

        if (_stepsListBox.SelectedIndex != _currentStepIndex)
        {
            _stepsListBox.SelectedIndex = _currentStepIndex;
        }

        _basicDataPanel.Visible = _currentStepIndex == 0;
        _placeholderPanel.Visible = _currentStepIndex != 0;

        _backButton.Enabled = _currentStepIndex > 0;
        _nextButton.Text = _currentStepIndex == _steps.Count - 1 ? AppStrings.Create : AppStrings.Next;
    }

    private async void NextButton_Click(object? sender, EventArgs e)
    {
        if (_currentStepIndex < _steps.Count - 1)
        {
            ShowStep(_currentStepIndex + 1);
            return;
        }

        await CreateTopicSafeAsync();
    }

    private async Task CreateTopicSafeAsync()
    {
        if (string.IsNullOrWhiteSpace(_titleTextBox.Text))
        {
            MessageBox.Show(
                this,
                AppStrings.MissingTitleMessage,
                AppStrings.AppTitle,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            ShowStep(0);
            _titleTextBox.Focus();
            return;
        }

        try
        {
            var request = new CreateHealthTopicRequest
            {
                Title = _titleTextBox.Text,
                Status = GetSelectedValue(_statusComboBox, HealthTopicStatus.Observation),
                Priority = GetSelectedValue(_priorityComboBox, HealthTopicPriority.Normal),
                ShortDescription = _shortDescriptionTextBox.Text,
                Notes = _notesTextBox.Text
            };

            await _healthTopicService.CreateTopicAsync(request).ConfigureAwait(true);

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (ArgumentException ex)
        {
            // Domain validation messages are deliberately generic and do not include
            // the user's medical content. They are safe to show as direct feedback.
            MessageBox.Show(
                this,
                ex.Message,
                AppStrings.AppTitle,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch
        {
            UiErrorHandler.ShowSafeError(this, AppStrings.OperationCreateHealthTopic);
        }
    }

    private static TEnum GetSelectedValue<TEnum>(ComboBox comboBox, TEnum fallback)
        where TEnum : struct, Enum
    {
        return comboBox.SelectedItem is EnumDisplayItem<TEnum> selected
            ? selected.Value
            : fallback;
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
