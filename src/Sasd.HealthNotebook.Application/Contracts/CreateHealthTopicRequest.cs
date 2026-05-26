using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.Application.Contracts;

/// <summary>
/// Request object used by the UI to create a new health topic.
///
/// The request deliberately contains user-entered documentation only. It must not be
/// interpreted as a medical diagnosis or treatment instruction.
/// </summary>
public sealed class CreateHealthTopicRequest
{
    /// <summary>
    /// Gets or sets the user-visible topic title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the documentation status selected by the user.
    /// </summary>
    public HealthTopicStatus Status { get; set; } = HealthTopicStatus.Observation;

    /// <summary>
    /// Gets or sets the personal organization priority selected by the user.
    /// </summary>
    public HealthTopicPriority Priority { get; set; } = HealthTopicPriority.Normal;

    /// <summary>
    /// Gets or sets an optional short description.
    /// </summary>
    public string ShortDescription { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets optional notes.
    /// </summary>
    public string Notes { get; set; } = string.Empty;
}
