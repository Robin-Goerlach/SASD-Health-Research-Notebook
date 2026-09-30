namespace Sasd.HealthNotebook.Domain;

/// <summary>
/// Describes how important a health topic is for the user's organization.
///
/// This priority is not a medical triage category. It is only a personal
/// planning and organization marker.
/// </summary>
public enum HealthTopicPriority
{
    /// <summary>
    /// No special priority was selected.
    /// </summary>
    Normal = 0,

    /// <summary>
    /// The user wants to keep an eye on the topic.
    /// </summary>
    Watch = 1,

    /// <summary>
    /// The user wants to prepare this topic for a doctor appointment.
    /// </summary>
    PrepareForDoctor = 2,

    /// <summary>
    /// The user considers the documentation urgent from an organizational point of view.
    /// This is not emergency advice.
    /// </summary>
    OrganizationallyUrgent = 3
}
