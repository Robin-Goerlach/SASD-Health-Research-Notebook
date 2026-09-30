using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.WinForms.Views;

namespace Sasd.HealthNotebook.WinForms.Presentation;

/// <summary>
/// Coordinates dashboard data loading for the WinForms dashboard view.
/// </summary>
public sealed class DashboardPresenter
{
    private readonly HealthTopicService _healthTopicService;
    private readonly DashboardView _view;

    /// <summary>
    /// Initializes a new instance of the <see cref="DashboardPresenter" /> class.
    /// </summary>
    public DashboardPresenter(HealthTopicService healthTopicService, DashboardView view)
    {
        _healthTopicService = healthTopicService ?? throw new ArgumentNullException(nameof(healthTopicService));
        _view = view ?? throw new ArgumentNullException(nameof(view));
    }

    /// <summary>
    /// Loads the overview data and updates the dashboard view.
    /// </summary>
    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        DashboardOverview overview = await _healthTopicService
            .GetDashboardOverviewAsync(cancellationToken)
            .ConfigureAwait(true);

        _view.SetOverview(overview);
    }
}
