using Sasd.HealthNotebook.Application.Contracts;

namespace Sasd.HealthNotebook.Wpf.ViewModels;

/// <summary>
/// View model for one row in the health-topic list.
/// </summary>
public sealed class HealthTopicListItemViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HealthTopicListItemViewModel" /> class.
    /// </summary>
    public HealthTopicListItemViewModel(HealthTopicSummary summary)
    {
        Id = summary.Id;
        Title = summary.Title;
        Status = summary.Status.ToString();
        Priority = summary.Priority.ToString();
        ShortDescription = summary.ShortDescription;
        CreatedAtText = summary.CreatedAt.LocalDateTime.ToString("yyyy-MM-dd HH:mm");
    }

    /// <summary>
    /// Gets the technical identifier.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Gets the topic title.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the display status.
    /// </summary>
    public string Status { get; }

    /// <summary>
    /// Gets the display priority.
    /// </summary>
    public string Priority { get; }

    /// <summary>
    /// Gets the short description.
    /// </summary>
    public string ShortDescription { get; }

    /// <summary>
    /// Gets the formatted creation date.
    /// </summary>
    public string CreatedAtText { get; }
}
