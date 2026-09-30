using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.WinForms.Localization;

namespace Sasd.HealthNotebook.WinForms.Presentation;

/// <summary>
/// Presentation-only row model for the WinForms health-topic grid.
/// </summary>
public sealed class HealthTopicGridRow
{
    /// <summary>Gets or sets the stable identifier used to preserve selection after reload.</summary>
    public Guid Id { get; set; }

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
            Id = summary.Id,
            Title = summary.Title,
            Status = AppStrings.HealthTopicStatusText(summary.Status),
            Priority = AppStrings.HealthTopicPriorityText(summary.Priority),
            CreatedAt = AppStrings.FormatDateTime(summary.CreatedAt),
            ShortDescription = summary.ShortDescription
        };
    }
}
