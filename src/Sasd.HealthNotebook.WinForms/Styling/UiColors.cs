using System.Drawing;

namespace Sasd.HealthNotebook.WinForms.Styling;

/// <summary>
/// Central color palette for the Windows Forms frontend.
/// </summary>
public static class UiColors
{
    /// <summary>Dark sidebar background.</summary>
    public static Color SidebarBackground => Color.FromArgb(22, 28, 38);

    /// <summary>Sidebar hover background.</summary>
    public static Color SidebarHoverBackground => Color.FromArgb(38, 50, 68);

    /// <summary>Selected navigation background.</summary>
    public static Color SidebarSelectedBackground => Color.FromArgb(44, 87, 122);

    /// <summary>Primary accent color for actions and highlights.</summary>
    public static Color PrimaryAccent => Color.FromArgb(45, 125, 180);

    /// <summary>Main window background.</summary>
    public static Color WindowBackground => Color.FromArgb(245, 247, 250);

    /// <summary>Card and panel background.</summary>
    public static Color CardBackground => Color.White;

    /// <summary>Subtle border color.</summary>
    public static Color BorderColor => Color.FromArgb(221, 227, 235);

    /// <summary>Main text color.</summary>
    public static Color PrimaryText => Color.FromArgb(26, 32, 44);

    /// <summary>Secondary explanatory text color.</summary>
    public static Color SecondaryText => Color.FromArgb(92, 107, 126);

    /// <summary>Sidebar text color.</summary>
    public static Color SidebarText => Color.FromArgb(236, 242, 250);
}
