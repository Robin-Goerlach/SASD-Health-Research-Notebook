using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Repositories;
using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.Application.Services;

/// <summary>Creates and chronologically loads measurements without clinical evaluation or duplicated timeline records.</summary>
public sealed class MeasurementService
{
    private readonly IMeasurementRepository _measurements;
    private readonly IHealthTopicRepository _topics;
    /// <summary>Uses the shared UI-independent storage contracts.</summary>
    public MeasurementService(IMeasurementRepository measurements, IHealthTopicRepository topics)
    {
        _measurements = measurements ?? throw new ArgumentNullException(nameof(measurements));
        _topics = topics ?? throw new ArgumentNullException(nameof(topics));
    }
    /// <summary>Validates numeric structure and any topic before adding one measurement.</summary>
    public async Task CreateMeasurementAsync(CreateMeasurementRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var measurement = Measurement.Create(request.MeasurementType, request.OccurredAt, request.Value,
            request.Systolic, request.Diastolic, request.Pulse, request.HealthTopicId, request.Note, request.Context);
        if (measurement.HealthTopicId.HasValue && !(await _topics.GetAllAsync(cancellationToken).ConfigureAwait(false))
            .Any(topic => topic.Id == measurement.HealthTopicId))
            throw new ArgumentException("The selected topic does not exist.");
        await _measurements.AddAsync(measurement, cancellationToken).ConfigureAwait(false);
    }
    /// <summary>Loads latest documented instants first; resolves current topic names without persisting copies.</summary>
    public async Task<IReadOnlyList<MeasurementSummary>> GetMeasurementsAsync(Guid? healthTopicId = null, CancellationToken cancellationToken = default)
    {
        var measurements = await _measurements.GetAllAsync(cancellationToken).ConfigureAwait(false);
        var topics = (await _topics.GetAllAsync(cancellationToken).ConfigureAwait(false)).ToDictionary(topic => topic.Id);
        return measurements.Where(item => !healthTopicId.HasValue || item.HealthTopicId == healthTopicId)
            .OrderByDescending(item => item.OccurredAt).ThenByDescending(item => item.CreatedAt).ThenBy(item => item.Id)
            .Select(item => new MeasurementSummary(item, item.HealthTopicId.HasValue && topics.TryGetValue(item.HealthTopicId.Value, out var topic)
                ? topic.Title : null)).ToList();
    }
}
