namespace Sasd.HealthNotebook.WinForms.Styling;

/// <summary>
/// Shared layout measurements for the Windows Forms frontend.
/// </summary>
public static class UiMetrics
{
    /// <summary>Width of the left navigation area.</summary>
    public const int SidebarWidth = 240;

    /// <summary>Height of the page header.</summary>
    public const int HeaderHeight = 144;

    /// <summary>Default distance between related controls.</summary>
    public const int StandardSpacing = 12;

    /// <summary>Larger spacing used between main sections.</summary>
    public const int LargeSpacing = 20;

    /// <summary>Standard inner padding for panels and cards.</summary>
    public const int Padding = 16;

    /// <summary>Height of navigation buttons.</summary>
    public const int NavigationButtonHeight = 44;

    /// <summary>Height reserved for readable two-line dashboard card explanations.</summary>
    public const int DashboardCardHeight = 156;

    /// <summary>Height of the six-count dashboard strip, allowing two-line DE/EN labels.</summary>
    public const int DashboardOverviewHeight = 132;

    /// <summary>Compact three-card strip including section spacing.</summary>
    public const int CompactOverviewHeight = 116;

    /// <summary>Two complete date/group lines plus cell padding; verified in DE/EN renders.</summary>
    public const int AgendaRowHeight = 40;

    /// <summary>Minimum height of primary and secondary command buttons.</summary>
    public const int ActionHeight = 38;
}
