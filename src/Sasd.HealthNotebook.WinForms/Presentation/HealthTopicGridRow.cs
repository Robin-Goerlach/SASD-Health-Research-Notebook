using Sasd.HealthNotebook.Application.Contracts;

namespace Sasd.HealthNotebook.WinForms.Presentation;

/// <summary>
/// Presentation-only row model for the WinForms health-topic grid.
/// </summary>
public sealed class HealthTopicGridRow
{
    /// <summary>Gets or sets the topic title.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets the documentation status text.</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>Gets or sets the personal priority text.</summary>
    public string Priority { get; set; } = string.Empty;

    /// <summary>Gets or sets the creation date text.</summary>
    public string CreatedAt { get; set; } = string.Empty;

    /// <summary>Gets or sets the short description text.</summary>
    public string ShortDescription { get; set; } = string.Empty;

    /// <summary>
    /// Creates a grid row from an application-layer summary.
    /// </summary>
    public static HealthTopicGridRow FromSummary(HealthTopicSummary summary)
    {
        ArgumentNullException.ThrowIfNull(summary);

        return new HealthTopicGridRow
        {
            Title = summary.Title,
            Status = summary.Status.ToString(),
            Priority = summary.Priority.ToString(),
            CreatedAt = summary.CreatedAt.LocalDateTime.ToString("yyyy-MM-dd HH:mm"),
            ShortDescription = summary.ShortDescription
        };
    }
}
