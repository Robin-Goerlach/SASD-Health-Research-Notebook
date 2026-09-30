using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Repositories;
using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.Application.Services;

/// <summary>
/// Application service for health-topic use cases.
///
/// UI code should call this service instead of directly manipulating repositories.
/// This keeps validation and future workflow rules in one place.
/// </summary>
public sealed class HealthTopicService
{
    private readonly IHealthTopicRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="HealthTopicService" /> class.
    /// </summary>
    public HealthTopicService(IHealthTopicRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <summary>
    /// Creates a new health topic from a user request.
    /// </summary>
    public async Task<HealthTopicSummary> CreateTopicAsync(
        CreateHealthTopicRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        HealthTopic topic = HealthTopic.Create(
            request.Title,
            request.Status,
            request.Priority,
            request.ShortDescription,
            request.Notes);

        await _repository.AddAsync(topic, cancellationToken).ConfigureAwait(false);

        return HealthTopicSummary.FromTopic(topic);
    }

    /// <summary>
    /// Gets all health topics sorted by creation date, newest first.
    /// </summary>
    public async Task<IReadOnlyList<HealthTopicSummary>> GetTopicSummariesAsync(
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<HealthTopic> topics = await _repository.GetAllAsync(cancellationToken).ConfigureAwait(false);

        return topics
            .OrderByDescending(topic => topic.CreatedAt)
            .Select(HealthTopicSummary.FromTopic)
            .ToList();
    }

    /// <summary>
    /// Builds a small dashboard overview from the currently stored topics.
    /// </summary>
    public async Task<DashboardOverview> GetDashboardOverviewAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<HealthTopic> topics = await _repository.GetAllAsync(cancellationToken).ConfigureAwait(false);

        return new DashboardOverview
        {
            TotalTopics = topics.Count,
            PrepareForDoctorCount = topics.Count(topic => topic.Priority == HealthTopicPriority.PrepareForDoctor),
            ArchivedTopics = topics.Count(topic => topic.Status == HealthTopicStatus.Archived)
        };
    }
}
