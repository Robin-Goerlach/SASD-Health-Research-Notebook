using Sasd.HealthNotebook.WinForms.Localization;
using Sasd.HealthNotebook.WinForms.Styling;

namespace Sasd.HealthNotebook.WinForms.Forms;

/// <summary>Shared presentation layout and save lifecycle for the three small source dialogs.</summary>
public abstract class SourceRecordForm : Form
{
    private readonly TableLayoutPanel _fields;
    private readonly Button _saveButton;
    private readonly Label _validationLabel;
    private readonly ToolTip _toolTip = new() { AutoPopDelay = 15000 };
    private bool _saving;
    /// <summary>Creates the common dialog chrome; derived forms add only their editors.</summary>
    protected SourceRecordForm(string title, string guidance, Size minimumSize)
    {
        AutoScaleDimensions = new SizeF(96, 96); AutoScaleMode = AutoScaleMode.Dpi;
        Text = title; StartPosition = FormStartPosition.CenterParent;
        MinimumSize = minimumSize; Size = new Size(minimumSize.Width + 40, minimumSize.Height + 40);
        Font = UiFonts.Body; BackColor = UiColors.WindowBackground;
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 4, ColumnCount = 1,
            Padding = new Padding(UiMetrics.LargeSpacing) };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 76));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        root.Controls.Add(new Label { Text = guidance, Dock = DockStyle.Fill, ForeColor = UiColors.SecondaryText }, 0, 0);
        _fields = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 0, Margin = Padding.Empty, TabIndex = 0 };
        _fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
        _fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 66));
        root.Controls.Add(_fields, 0, 1);
        _validationLabel = new Label { Dock = DockStyle.Fill, ForeColor = UiColors.SecondaryText };
        root.Controls.Add(_validationLabel, 0, 2);
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false, TabIndex = 1 };
        _saveButton = new Button { Text = AppStrings.SaveSourceRecord, AutoSize = true, MinimumSize = new Size(140, UiMetrics.ActionHeight),
            FlatStyle = FlatStyle.Flat, BackColor = UiColors.PrimaryAccent, ForeColor = UiColors.CardBackground, TabIndex = 0 };
        _saveButton.FlatAppearance.BorderSize = 0;
        var cancel = new Button { Text = AppStrings.Cancel, DialogResult = DialogResult.Cancel,
            Width = 110, Height = UiMetrics.ActionHeight, TabIndex = 1 };
        buttons.Controls.Add(_saveButton); buttons.Controls.Add(cancel); root.Controls.Add(buttons, 0, 3);
        Controls.Add(root); AcceptButton = _saveButton; CancelButton = cancel;
        _saveButton.Click += async (_, _) => await SaveAsync();
        FormClosing += (_, args) => { if (_saving) args.Cancel = true; };
    }
    /// <summary>Adds a localized accessible editor in natural tab order.</summary>
    protected void AddField(string labelText, Control editor, string help, bool stretch = false, float weight = 100)
    {
        int row = _fields.RowCount++;
        _fields.RowStyles.Add(new RowStyle(stretch ? SizeType.Percent : SizeType.Absolute, stretch ? weight : 46));
        var label = new Label { Text = labelText, Dock = DockStyle.Fill, Padding = new Padding(0, 6, 8, 0),
            ForeColor = UiColors.PrimaryText, TabStop = false };
        editor.Dock = DockStyle.Fill; editor.TabIndex = row;
        editor.AccessibleName = labelText; editor.AccessibleDescription = help;
        _toolTip.SetToolTip(editor, help); _toolTip.SetToolTip(label, help);
        _fields.Controls.Add(label, 0, row); _fields.Controls.Add(editor, 1, row);
    }
    /// <summary>Calls the specific Application use case. No business rules live in the dialog.</summary>
    protected abstract Task SaveRecordAsync();
    private async Task SaveAsync()
    {
        if (_saving) return;
        _saving = true; _saveButton.Enabled = false; _validationLabel.Text = string.Empty;
        bool saved = false;
        try { await SaveRecordAsync(); saved = true; }
        catch (ArgumentException) { _validationLabel.Text = AppStrings.SourceValidationFailed; }
        catch { UiErrorHandler.ShowSafeError(this, AppStrings.OperationSaveSourceRecord); }
        finally { _saving = false; _saveButton.Enabled = true; }
        if (saved) { DialogResult = DialogResult.OK; Close(); }
    }
    /// <inheritdoc />
    protected override void Dispose(bool disposing) { if (disposing) _toolTip.Dispose(); base.Dispose(disposing); }
    /// <summary>Creates a bounded multiline text editor.</summary>
    protected static TextBox TextEditor(int maximumLength) => new() { Multiline = true, AcceptsReturn = true,
        ScrollBars = ScrollBars.Vertical, MaxLength = maximumLength };
}
