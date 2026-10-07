namespace Sasd.HealthNotebook.Application.Contracts;

/// <summary>Read-only timeline source snapshot. Identity includes its source kind.</summary>
public sealed record TimelineItem
{
    /// <summary>Requires exactly one source snapshot.</summary>
    public TimelineItem(HealthEntrySummary? entry, MeasurementSummary? measurement)
    {
        if ((entry is null) == (measurement is null))
            throw new ArgumentException("Exactly one timeline source is required.");
        Entry = entry;
        Measurement = measurement;
    }
    /// <summary>Displayed notebook entry, when this is an entry row.</summary>
    public HealthEntrySummary? Entry { get; }
    /// <summary>Displayed measurement, when this is a measurement row.</summary>
    public MeasurementSummary? Measurement { get; }
    /// <summary>Source record identity; never a persisted timeline event.</summary>
    public Guid Id => Entry?.Id ?? Measurement!.Measurement.Id;
    /// <summary>Documented instant, compared across UTC offsets.</summary>
    public DateTimeOffset OccurredAt => Entry?.OccurredAt ?? Measurement!.Measurement.OccurredAt;
    /// <summary>Optional topic reference.</summary>
    public Guid? HealthTopicId => Entry?.HealthTopicId ?? Measurement?.Measurement.HealthTopicId;
    /// <summary>Current topic title, without copying it into storage.</summary>
    public string? HealthTopicTitle => Entry?.HealthTopicTitle ?? Measurement?.HealthTopicTitle;
}
