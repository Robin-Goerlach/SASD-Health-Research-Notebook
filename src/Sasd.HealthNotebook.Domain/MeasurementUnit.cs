namespace Sasd.HealthNotebook.Domain;

/// <summary>Explicit fixed units in Slice 1; no automatic conversions.</summary>
public enum MeasurementUnit
{
    /// <summary>Millimetres of mercury.</summary>
    MmHg,
    /// <summary>Beats per minute.</summary>
    PerMinute,
    /// <summary>Degrees Celsius.</summary>
    Celsius,
    /// <summary>Milligrams per decilitre.</summary>
    MgPerDeciliter,
    /// <summary>Kilograms.</summary>
    Kilogram
}
