namespace Sasd.HealthNotebook.Domain;

/// <summary>User-selected documentation category; carries no medical interpretation.</summary>
public enum HealthEntryType
{
    /// <summary>A general notebook note.</summary>
    Note,
    /// <summary>A user-entered observation.</summary>
    Observation,
    /// <summary>A research note, without an evidence assessment.</summary>
    Research
}
