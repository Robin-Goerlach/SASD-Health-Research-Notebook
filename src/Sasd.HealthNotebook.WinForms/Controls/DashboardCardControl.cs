using System.Drawing;
using System.Windows.Forms;
using Sasd.HealthNotebook.WinForms.Styling;

namespace Sasd.HealthNotebook.WinForms.Controls;

/// <summary>
/// Reusable dashboard card for a small title, large value and explanation.
/// </summary>
public sealed class DashboardCardControl : UserControl
{
    private readonly Label _titleLabel;
    private readonly Label _valueLabel;
    private readonly Label _descriptionLabel;

    /// <summary>
    /// Initializes a new instance of the <see cref="DashboardCardControl" /> class.
    /// </summary>
    public DashboardCardControl()
    {
        BackColor = UiColors.CardBackground;
        Margin = new Padding(0, 0, UiMetrics.StandardSpacing, 0);
        Padding = new Padding(UiMetrics.Padding);
        Width = 220;
        Height = 126;

        _titleLabel = new Label
        {
            Dock = DockStyle.Top,
            Height = 24,
            Font = UiFonts.CardTitle,
            ForeColor = UiColors.SecondaryText
        };

        _valueLabel = new Label
        {
            Dock = DockStyle.Top,
            Height = 42,
            Font = UiFonts.CardValue,
            ForeColor = UiColors.PrimaryText
        };

        _descriptionLabel = new Label
        {
            Dock = DockStyle.Fill,
            Font = UiFonts.Small,
            ForeColor = UiColors.SecondaryText
        };

        Controls.Add(_descriptionLabel);
        Controls.Add(_valueLabel);
        Controls.Add(_titleLabel);
    }

    /// <summary>
    /// Updates the card text.
    /// </summary>
    public void SetContent(string title, string value, string description)
    {
        _titleLabel.Text = title;
        _valueLabel.Text = value;
        _descriptionLabel.Text = description;
    }

    /// <inheritdoc />
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        using var borderPen = new Pen(UiColors.BorderColor);
        Rectangle borderRectangle = new(0, 0, Width - 1, Height - 1);
        e.Graphics.DrawRectangle(borderPen, borderRectangle);
    }
}
