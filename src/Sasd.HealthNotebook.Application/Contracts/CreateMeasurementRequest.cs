using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.Application.Contracts;

/// <summary>User input for a numeric measurement; units are determined by its type.</summary>
public sealed class CreateMeasurementRequest
{
    /// <summary>Supported category.</summary>
    public MeasurementType MeasurementType { get; init; }
    /// <summary>Documented measurement time.</summary>
    public DateTimeOffset OccurredAt { get; init; }
    /// <summary>Optional existing topic.</summary>
    public Guid? HealthTopicId { get; init; }
    /// <summary>Single numeric value, absent for blood pressure.</summary>
    public double? Value { get; init; }
    /// <summary>Blood-pressure systolic number.</summary>
    public double? Systolic { get; init; }
    /// <summary>Blood-pressure diastolic number.</summary>
    public double? Diastolic { get; init; }
    /// <summary>Optional accompanying blood-pressure pulse.</summary>
    public double? Pulse { get; init; }
    /// <summary>Optional personal note.</summary>
    public string? Note { get; init; }
    /// <summary>Optional documented measurement situation.</summary>
    public string? Context { get; init; }
}
