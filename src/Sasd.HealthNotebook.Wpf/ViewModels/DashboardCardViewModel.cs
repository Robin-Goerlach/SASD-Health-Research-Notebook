namespace Sasd.HealthNotebook.Wpf.ViewModels;

/// <summary>
/// Represents one simple dashboard card.
/// </summary>
public sealed class DashboardCardViewModel
{
    /// <summary>
    /// Gets or sets the card title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the large display value.
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a short explanatory text.
    /// </summary>
    public string Description { get; set; } = string.Empty;
}
