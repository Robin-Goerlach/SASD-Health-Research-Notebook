using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.WinForms.Views;

namespace Sasd.HealthNotebook.WinForms.Presentation;

/// <summary>
/// Coordinates topic-list loading for a WinForms health-topic view.
/// </summary>
public sealed class HealthTopicPresenter
{
    private readonly HealthTopicService _healthTopicService;
    private int _loadGeneration;
    private readonly HealthTopicsView _view;

    /// <summary>
    /// Initializes a new instance of the <see cref="HealthTopicPresenter" /> class.
    /// </summary>
    public HealthTopicPresenter(HealthTopicService healthTopicService, HealthTopicsView view)
    {
        _healthTopicService = healthTopicService ?? throw new ArgumentNullException(nameof(healthTopicService));
        _view = view ?? throw new ArgumentNullException(nameof(view));
    }

    /// <summary>
    /// Loads summaries from the application service and updates the grid.
    /// </summary>
    public async Task<IReadOnlyList<HealthTopicSummary>> LoadAsync(CancellationToken cancellationToken = default)
    {
        int generation = ++_loadGeneration;
        IReadOnlyList<HealthTopicSummary> summaries = await _healthTopicService
            .GetTopicSummariesAsync(cancellationToken)
            .ConfigureAwait(true);

        if (!_view.IsDisposed && generation == _loadGeneration) _view.SetTopics(summaries);
        return summaries;
    }
}
