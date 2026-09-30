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

        if (!IsSelected)
        {
            BackColor = UiColors.SidebarHoverBackground;
        }
    }

    /// <inheritdoc />
    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        ApplyVisualState();
    }

    /// <inheritdoc />
    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);

        if (!IsSelected)
        {
            BackColor = UiColors.SidebarHoverBackground;
        }
    }

    /// <inheritdoc />
    protected override void OnLostFocus(EventArgs e)
    {
        base.OnLostFocus(e);
        ApplyVisualState();
    }

    private void ApplyVisualState()
    {
        BackColor = IsSelected ? UiColors.SidebarSelectedBackground : UiColors.SidebarBackground;
    }
}
