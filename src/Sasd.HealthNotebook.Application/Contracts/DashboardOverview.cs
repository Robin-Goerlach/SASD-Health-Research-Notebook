namespace Sasd.HealthNotebook.Application.Contracts;

/// <summary>
/// Small overview model for the dashboard.
/// </summary>
public sealed class DashboardOverview
{
    /// <summary>
    /// Gets or sets the total number of documented health topics.
    /// </summary>
    public int TotalTopics { get; set; }

    /// <summary>
    /// Gets or sets the number of topics that should be prepared for a doctor appointment.
    /// </summary>
    public int PrepareForDoctorCount { get; set; }

    /// <summary>
    /// Gets or sets the number of archived topics.
    /// </summary>
    public int ArchivedTopics { get; set; }
}
