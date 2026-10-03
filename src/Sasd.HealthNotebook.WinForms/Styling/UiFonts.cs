using System.Drawing;

namespace Sasd.HealthNotebook.WinForms.Styling;

/// <summary>
/// Central font definitions for the Windows Forms frontend.
/// </summary>
public static class UiFonts
{
    /// <summary>Large page title font.</summary>
    public static Font PageTitle => new("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point);

    /// <summary>Normal body font.</summary>
    public static Font Body => new("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);

    /// <summary>Small explanatory text font.</summary>
    public static Font Small => new("Segoe UI", 8.5F, FontStyle.Regular, GraphicsUnit.Point);

    /// <summary>Dashboard card value font.</summary>
    public static Font CardValue => new("Segoe UI", 20F, FontStyle.Bold, GraphicsUnit.Point);

    /// <summary>Compact documentative count font.</summary>
    public static Font CompactCardValue => new("Segoe UI", 16F, FontStyle.Bold, GraphicsUnit.Point);

    /// <summary>Dashboard card title font.</summary>
    public static Font CardTitle => new("Segoe UI Semibold", 9.5F, FontStyle.Regular, GraphicsUnit.Point);

    /// <summary>Navigation item font.</summary>
    public static Font Navigation => new("Segoe UI Semibold", 10F, FontStyle.Regular, GraphicsUnit.Point);
}
