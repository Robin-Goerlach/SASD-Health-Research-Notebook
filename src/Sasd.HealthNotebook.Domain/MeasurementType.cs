namespace Sasd.HealthNotebook.Domain;

/// <summary>Supported user-documented vital measurement categories.</summary>
public enum MeasurementType
{
    /// <summary>Separate systolic/diastolic pressure and optionally pulse.</summary>
    BloodPressure,
    /// <summary>A standalone pulse rate.</summary>
    Pulse,
    /// <summary>Body temperature.</summary>
    Temperature,
    /// <summary>Blood glucose.</summary>
    BloodGlucose,
    /// <summary>Body weight.</summary>
    Weight
}
