using System.Collections.ObjectModel;
using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Services;

namespace Sasd.HealthNotebook.Wpf.ViewModels;

/// <summary>
/// Main view model for the application shell.
/// </summary>
public sealed class MainWindowViewModel : ObservableObject
{
    private readonly HealthTopicService _healthTopicService;
    private string _statusText = "Ready.";
    private HealthTopicListItemViewModel? _selectedTopic;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel" /> class.
    /// </summary>
    public MainWindowViewModel(HealthTopicService healthTopicService)
    {
        _healthTopicService = healthTopicService ?? throw new ArgumentNullException(nameof(healthTopicService));
    }

    /// <summary>
    /// Gets the dashboard cards shown at the top of the main window.
    /// </summary>
    public ObservableCollection<DashboardCardViewModel> DashboardCards { get; } = new();

    /// <summary>
    /// Gets the health-topic list shown in the main window.
    /// </summary>
    public ObservableCollection<HealthTopicListItemViewModel> Topics { get; } = new();

    /// <summary>
    /// Gets or sets the selected topic.
    /// </summary>
    public HealthTopicListItemViewModel? SelectedTopic
    {
        get => _selectedTopic;
        set => SetProperty(ref _selectedTopic, value);
    }

    /// <summary>
    /// Gets or sets a short status text for the footer.
    /// </summary>
    public string StatusText
    {
        get => _statusText;
        set => SetProperty(ref _statusText, value);
    }

    /// <summary>
    /// Loads topics and dashboard values from the application service.
    /// </summary>
    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<HealthTopicSummary> summaries = await _healthTopicService
            .GetTopicSummariesAsync(cancellationToken)
            .ConfigureAwait(true);

        DashboardOverview overview = await _healthTopicService
            .GetDashboardOverviewAsync(cancellationToken)
            .ConfigureAwait(true);

        Topics.Clear();
        foreach (HealthTopicSummary summary in summaries)
        {
            Topics.Add(new HealthTopicListItemViewModel(summary));
        }

        DashboardCards.Clear();
        DashboardCards.Add(new DashboardCardViewModel
        {
            Title = "Health topics",
            Value = overview.TotalTopics.ToString(),
            Description = "Documented topics in the local notebook"
        });
        DashboardCards.Add(new DashboardCardViewModel
        {
            Title = "Prepare for doctor",
            Value = overview.PrepareForDoctorCount.ToString(),
            Description = "Topics marked for appointment preparation"
        });
        DashboardCards.Add(new DashboardCardViewModel
        {
            Title = "Archived",
            Value = overview.ArchivedTopics.ToString(),
            Description = "Topics no longer shown as active"
        });

        StatusText = $"Loaded {overview.TotalTopics} health topic(s).";
    }
}
