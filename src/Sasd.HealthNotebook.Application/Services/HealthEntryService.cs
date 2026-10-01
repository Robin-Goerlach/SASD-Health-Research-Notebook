using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Repositories;
using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.Application.Services;

/// <summary>Creates notebook entries and derives their timeline without duplicated events.</summary>
public sealed class HealthEntryService
{
    private readonly IHealthEntryRepository _entries;
    private readonly IHealthTopicRepository _topics;
    /// <summary>Uses shared storage contracts, independently of the frontend.</summary>
    public HealthEntryService(IHealthEntryRepository entries, IHealthTopicRepository topics)
    {
        _entries = entries ?? throw new ArgumentNullException(nameof(entries));
        _topics = topics ?? throw new ArgumentNullException(nameof(topics));
    }

    /// <summary>Validates input and any chosen topic before persisting the entry.</summary>
    public async Task CreateEntryAsync(CreateHealthEntryRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var entry = HealthEntry.Create(request.EntryType, request.OccurredAt, request.Title, request.Content, request.HealthTopicId);
        if (entry.HealthTopicId.HasValue && !(await _topics.GetAllAsync(cancellationToken).ConfigureAwait(false))
            .Any(topic => topic.Id == entry.HealthTopicId))
            throw new ArgumentException("The selected topic does not exist.", nameof(request.HealthTopicId));
        await _entries.AddAsync(entry, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Loads newest documented events first; optionally restricts to one topic.</summary>
    public async Task<IReadOnlyList<HealthEntrySummary>> GetEntriesAsync(Guid? healthTopicId = null,
        CancellationToken cancellationToken = default)
    {
        var entries = await _entries.GetAllAsync(cancellationToken).ConfigureAwait(false);
        var topics = (await _topics.GetAllAsync(cancellationToken).ConfigureAwait(false)).ToDictionary(topic => topic.Id);
        return entries.Where(entry => !healthTopicId.HasValue || entry.HealthTopicId == healthTopicId)
            .OrderByDescending(entry => entry.OccurredAt).ThenByDescending(entry => entry.CreatedAt).ThenBy(entry => entry.Id)
            .Select(entry => new HealthEntrySummary(entry.Id, entry.HealthTopicId,
                entry.HealthTopicId.HasValue && topics.TryGetValue(entry.HealthTopicId.Value, out var topic) ? topic.Title : null,
                entry.EntryType, entry.OccurredAt, entry.Title, entry.Content)).ToList();
    }
}
