using Sasd.HealthNotebook.Application.Contracts;

namespace Sasd.HealthNotebook.Application.Services;

/// <summary>Combines existing stores read-only; a source load failure fails the whole load.</summary>
public sealed class TimelineService(HealthEntryService entries, MeasurementService measurements)
{
    /// <summary>Loads newest documented instants first, with deterministic source/ID ties.</summary>
    public async Task<IReadOnlyList<TimelineItem>> GetItemsAsync(CancellationToken cancellationToken = default)
    {
        var notes = await entries.GetEntriesAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        var values = await measurements.GetMeasurementsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        return notes.Select(item => new TimelineItem(item, null))
            .Concat(values.Select(item => new TimelineItem(null, item)))
            .OrderByDescending(item => item.OccurredAt)
            .ThenByDescending(item => item.Entry?.CreatedAt ?? item.Measurement!.Measurement.CreatedAt)
            .ThenBy(item => item.Entry is null ? 1 : 0).ThenBy(item => item.Id).ToList();
    }
}
