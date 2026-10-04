using Sasd.HealthNotebook.WinForms.Localization;
using Sasd.HealthNotebook.WinForms.Styling;

namespace Sasd.HealthNotebook.WinForms.Forms;

/// <summary>Explicit single-record deletion; Enter and initial focus select Cancel, never Delete.</summary>
public sealed class DeleteConfirmationForm : Form
{
    private readonly Button _deleteButton;
    private readonly Button _cancelButton;
    /// <summary>Describes the currently selected record in the confirmation, without technical IDs.</summary>
    public DeleteConfirmationForm(string record)
    {
        Text = AppStrings.Delete; Font = UiFonts.Body; BackColor = UiColors.WindowBackground;
        StartPosition = FormStartPosition.CenterParent; MinimizeBox = MaximizeBox = false;
        FormBorderStyle = FormBorderStyle.FixedDialog; ClientSize = new Size(540, 220);
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new(UiMetrics.LargeSpacing), RowCount = 2, ColumnCount = 1 };
        root.RowStyles.Add(new(SizeType.Percent, 100)); root.RowStyles.Add(new(SizeType.Absolute, 52));
        root.Controls.Add(new Label { Text = AppStrings.ConfirmDelete(record), Dock = DockStyle.Fill, AutoEllipsis = false }, 0, 0);
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, WrapContents = false };
        _cancelButton = new() { Text = AppStrings.Cancel, DialogResult = DialogResult.Cancel, AutoSize = true, MinimumSize = new(110, UiMetrics.ActionHeight), TabIndex = 0 };
        _deleteButton = new() { Text = AppStrings.Delete, DialogResult = DialogResult.OK, AutoSize = true, MinimumSize = new(110, UiMetrics.ActionHeight), TabIndex = 1 };
        buttons.Controls.Add(_cancelButton); buttons.Controls.Add(_deleteButton); root.Controls.Add(buttons, 0, 1); Controls.Add(root);
        AcceptButton = CancelButton = _cancelButton; Shown += (_, _) => _cancelButton.Focus();
    }
}
