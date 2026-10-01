namespace Sasd.HealthNotebook.Domain;

/// <summary>A manually documented numeric measurement, without medical interpretation.</summary>
public sealed class Measurement
{
    /// <summary>Maximum personal note length, matching notebook entries.</summary>
    public const int MaximumNoteLength = 4000;
    /// <summary>Maximum optional measurement-situation length.</summary>
    public const int MaximumContextLength = 500;
    /// <summary>The only supported acquisition method in this slice.</summary>
    public const string ManualAcquisitionMethod = "Manual";
    /// <summary>Stable identifier.</summary>
    public required Guid Id { get; init; }
    /// <summary>User-selected category.</summary>
    public required MeasurementType MeasurementType { get; init; }
    /// <summary>Documented measurement time, including UTC offset.</summary>
    public required DateTimeOffset OccurredAt { get; init; }
    /// <summary>Optional single health-topic reference.</summary>
    public Guid? HealthTopicId { get; init; }
    /// <summary>Numeric value for standalone pulse, temperature, glucose or weight; absent for blood pressure.</summary>
    public double? Value { get; init; }
    /// <summary>Required for blood pressure only.</summary>
    public double? Systolic { get; init; }
    /// <summary>Required for blood pressure only.</summary>
    public double? Diastolic { get; init; }
    /// <summary>Optional pulse accompanying blood pressure, in beats per minute. Not a second persisted measurement.</summary>
    public double? Pulse { get; init; }
    /// <summary>Unit of the primary numeric measurement; the optional blood-pressure pulse always uses PerMinute.</summary>
    public required MeasurementUnit Unit { get; init; }
    /// <summary>Manual entry only; no device or document import in this slice.</summary>
    public required string AcquisitionMethod { get; init; }
    /// <summary>Optional user-documented measurement situation, not interpreted by the app.</summary>
    public string? Context { get; init; }
    /// <summary>Optional personal note, separate from numbers; must never be logged.</summary>
    public string? Note { get; init; }
    /// <summary>Technical creation time.</summary>
    public required DateTimeOffset CreatedAt { get; init; }
    /// <summary>Technical modification time; equal to CreatedAt in this create-only slice.</summary>
    public required DateTimeOffset ModifiedAt { get; init; }

    /// <summary>Creates a measurement with fixed units and no truncation or clinical thresholds.</summary>
    public static Measurement Create(MeasurementType type, DateTimeOffset occurredAt, double? value = null,
        double? systolic = null, double? diastolic = null, double? pulse = null, Guid? healthTopicId = null,
        string? note = null, string? context = null)
    {
        var now = DateTimeOffset.Now;
        var measurement = new Measurement { Id = Guid.NewGuid(), MeasurementType = type, OccurredAt = occurredAt,
            Value = value, Systolic = systolic, Diastolic = diastolic, Pulse = pulse, Unit = UnitFor(type),
            AcquisitionMethod = ManualAcquisitionMethod, HealthTopicId = healthTopicId, Note = note, Context = context,
            CreatedAt = now, ModifiedAt = now };
        measurement.Validate();
        return measurement;
    }
    /// <summary>Defines the supported unit per type. This is an explicit contract, not a conversion.</summary>
    public static MeasurementUnit UnitFor(MeasurementType type) => type switch
    {
        MeasurementType.BloodPressure => MeasurementUnit.MmHg,
        MeasurementType.Pulse => MeasurementUnit.PerMinute,
        MeasurementType.Temperature => MeasurementUnit.Celsius,
        MeasurementType.BloodGlucose => MeasurementUnit.MgPerDeciliter,
        MeasurementType.Weight => MeasurementUnit.Kilogram,
        _ => throw new ArgumentException("Unsupported measurement type.")
    };
    /// <summary>Checks structure, finiteness and nonnegative numbers; never classifies a measurement medically.</summary>
    public void Validate()
    {
        if (!Enum.IsDefined(MeasurementType) || Unit != UnitFor(MeasurementType) || Id == Guid.Empty
            || HealthTopicId == Guid.Empty || OccurredAt == default || CreatedAt == default || ModifiedAt < CreatedAt
            || AcquisitionMethod != ManualAcquisitionMethod || Note?.Length > MaximumNoteLength || Context?.Length > MaximumContextLength)
            throw new ArgumentException("Invalid measurement metadata or text length.");
        if (MeasurementType == MeasurementType.BloodPressure)
        {
            if (!Systolic.HasValue || !Diastolic.HasValue || Value.HasValue)
                throw new ArgumentException("Blood pressure requires separate systolic and diastolic numbers.");
        }
        else if (!Value.HasValue || Systolic.HasValue || Diastolic.HasValue || Pulse.HasValue)
            throw new ArgumentException("A single-value measurement requires only its numeric value.");
        // No clinical minimum/maximum, relationship between pressure components, or alarm logic.
        foreach (double? number in new[] { Value, Systolic, Diastolic, Pulse })
            if (number.HasValue && (!double.IsFinite(number.Value) || number.Value < 0))
                throw new ArgumentException("Measurement numbers must be finite and nonnegative.");
    }
}
