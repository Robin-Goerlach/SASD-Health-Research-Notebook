namespace Sasd.HealthNotebook.Domain;

/// <summary>
/// Describes the documentation status of a health topic.
///
/// The values are intentionally non-medical. They help the user organize information,
/// but they do not represent a diagnosis made by the application.
/// </summary>
public enum HealthTopicStatus
{
    /// <summary>
    /// The topic is only an observation or an open question.
    /// </summary>
    Observation = 0,

    /// <summary>
    /// The topic is suspected by the user or mentioned as a possibility, but not confirmed in this app.
    /// </summary>
    Suspected = 1,

    /// <summary>
    /// The topic was reported by a doctor or medical document.
    /// The application itself never creates this confirmation.
    /// </summary>
    DoctorReported = 2,

    /// <summary>
    /// The topic is historical or currently inactive.
    /// </summary>
    Historical = 3,

    /// <summary>
    /// The topic is archived and should normally not appear in active views.
    /// </summary>
    Archived = 4
}
