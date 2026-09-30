using System.Drawing;
using System.Windows.Forms;
using Sasd.HealthNotebook.WinForms.Styling;

namespace Sasd.HealthNotebook.WinForms.Controls;

/// <summary>
/// Sidebar button with selected and hover states.
/// </summary>
public sealed class NavigationButton : Button
{
    private bool _isSelected;
    private bool _isHovered;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationButton" /> class.
    /// </summary>
    public NavigationButton()
    {
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        TextAlign = ContentAlignment.MiddleLeft;
        Height = UiMetrics.NavigationButtonHeight;
        Dock = DockStyle.Top;
        Padding = new Padding(18, 0, 0, 0);
        Font = UiFonts.Navigation;
        ForeColor = UiColors.SidebarText;
        BackColor = UiColors.SidebarBackground;
        TabStop = true;
    }

    /// <summary>
    /// Gets or sets whether this navigation item is currently selected.
    /// </summary>
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            ApplyVisualState();
        }
    }

    /// <inheritdoc />
    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);

        _isHovered = true;
        ApplyVisualState();
    }

    /// <inheritdoc />
    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _isHovered = false;
        ApplyVisualState();
    }

    /// <inheritdoc />
    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);

        ApplyVisualState();
    }

    /// <inheritdoc />
    protected override void OnLostFocus(EventArgs e)
    {
        base.OnLostFocus(e);
        ApplyVisualState();
    }

    private void ApplyVisualState()
    {
        BackColor = IsSelected ? UiColors.SidebarSelectedBackground
            : _isHovered || Focused ? UiColors.SidebarHoverBackground : UiColors.SidebarBackground;
        FlatAppearance.MouseOverBackColor = BackColor;
        FlatAppearance.MouseDownBackColor = UiColors.SidebarSelectedBackground;
        Invalidate();
    }

    /// <inheritdoc />
    protected override bool IsInputKey(Keys keyData) =>
        (keyData & Keys.KeyCode) is Keys.Up or Keys.Down or Keys.Home or Keys.End || base.IsInputKey(keyData);

    /// <inheritdoc />
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        if (IsSelected)
        {
            using var accent = new SolidBrush(UiColors.PrimaryAccent);
            e.Graphics.FillRectangle(accent, 0, 8, 3, Height - 16);
        }
        if (Focused)
        {
            ControlPaint.DrawFocusRectangle(e.Graphics,
                Rectangle.Inflate(ClientRectangle, -7, -7), UiColors.SidebarText, BackColor);
        }
    }
}
