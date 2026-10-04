using System.Drawing;
using System.Windows.Forms;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.WinForms.Controls;
using Sasd.HealthNotebook.WinForms.Localization;
using Sasd.HealthNotebook.WinForms.Presentation;
using Sasd.HealthNotebook.WinForms.Styling;
using Sasd.HealthNotebook.WinForms.Views;

namespace Sasd.HealthNotebook.WinForms.Forms;

/// <summary>
/// Main window for the Windows Forms frontend.
/// </summary>
public sealed partial class MainForm : Form
{
    private readonly HealthTopicService _healthTopicService;
    private readonly HealthEntryService _healthEntryService;
    private readonly SourceService _sourceService;
    private readonly MeasurementService _measurementService;
    private readonly MeasurementsView _measurementsView;
    private readonly MeasurementsPresenter _measurementsPresenter;
    private readonly SourcesView _sourcesView;
    private readonly SourcesPresenter _sourcesPresenter;
    private readonly TimelineView _timelineView;
    private readonly TimelinePresenter _timelinePresenter;
    private readonly NavigationControl _navigation;
    private readonly Panel _contentPanel;
    private readonly Label _pageTitleLabel;
    private readonly Label _pageDescriptionLabel;
    private readonly Label _languageLabel;
    private readonly ComboBox _languageComboBox;
    private readonly Button _refreshButton;
    private readonly Button _newTopicButton;
    private readonly ToolStripStatusLabel _statusLabel;
    private readonly DashboardView _dashboardView;
    private readonly HealthTopicsView _dashboardTopicsView;
    private readonly HealthTopicsView _topicsView;
    private readonly DashboardPresenter _dashboardPresenter;
    private readonly HealthTopicPresenter _dashboardTopicsPresenter;
    private readonly HealthTopicPresenter _topicsPresenter;
    private readonly Control _dashboardPage;
    private bool _isApplyingLanguageSelection;
    private int? _lastLoadedTopicCount;
    private NavigationPage _currentPage = NavigationPage.Dashboard;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainForm" /> class.
    /// </summary>
    public MainForm(HealthTopicService healthTopicService, HealthEntryService healthEntryService, SourceService sourceService, MeasurementService measurementService, SessionService sessionService, HealthActionService healthActionService)
    {
        _healthTopicService = healthTopicService ?? throw new ArgumentNullException(nameof(healthTopicService));
        _healthEntryService = healthEntryService ?? throw new ArgumentNullException(nameof(healthEntryService));
        _sourceService = sourceService ?? throw new ArgumentNullException(nameof(sourceService));
        _measurementService = measurementService ?? throw new ArgumentNullException(nameof(measurementService));
        _healthActionService = healthActionService ?? throw new ArgumentNullException(nameof(healthActionService));
        _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));

        AutoScaleDimensions = new SizeF(96, 96);
        AutoScaleMode = AutoScaleMode.Dpi;
        StartPosition = FormStartPosition.CenterScreen;
        Text = AppStrings.AppTitle;
        MinimumSize = new Size(1120, 740);
        Size = new Size(1280, 820);
        BackColor = UiColors.WindowBackground;
        Font = UiFonts.Body;

        _navigation = new NavigationControl { TabIndex = 0 };
        _navigation.PageRequested += Navigation_PageRequested;

        _pageTitleLabel = new Label
        {
            Dock = DockStyle.Top,
            Height = 42,
            Font = UiFonts.PageTitle,
            ForeColor = UiColors.PrimaryText,
            TextAlign = ContentAlignment.BottomLeft
        };

        _pageDescriptionLabel = new Label
        {
            Dock = DockStyle.Top,
            Height = 42,
            Font = UiFonts.Body,
            ForeColor = UiColors.SecondaryText,
            TextAlign = ContentAlignment.TopLeft
        };

        _refreshButton = new Button
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            MinimumSize = new Size(120, UiMetrics.ActionHeight),
            Padding = new Padding(UiMetrics.StandardSpacing, 0, UiMetrics.StandardSpacing, 0),
            FlatStyle = FlatStyle.Flat,
            BackColor = UiColors.CardBackground,
            TabIndex = 3
        };
        _refreshButton.Click += async (_, _) => await ReloadSafeAsync();

        _newTopicButton = new Button
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            MinimumSize = new Size(220, UiMetrics.ActionHeight),
            Padding = new Padding(UiMetrics.StandardSpacing, 0, UiMetrics.StandardSpacing, 0),
            FlatStyle = FlatStyle.Flat,
            BackColor = UiColors.PrimaryAccent,
            ForeColor = UiColors.CardBackground,
            TabIndex = 4
        };
        _newTopicButton.Click += async (_, _) =>
        {
            if (_currentPage == NavigationPage.Timeline) await ShowCreateHealthEntryAsync();
            else if (_currentPage == NavigationPage.Sources) await ShowCreateSourceAsync();
            else if (_currentPage == NavigationPage.Measurements) await ShowCreateMeasurementAsync();
            else if (_currentPage == NavigationPage.Sessions) await ShowCreateSessionAsync();
            else if (_currentPage == NavigationPage.Actions) await ShowCreateActionAsync();
            else await ShowCreateHealthTopicWizardAsync();
        };

        _languageLabel = new Label
        {
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            TextAlign = ContentAlignment.MiddleRight,
            ForeColor = UiColors.SecondaryText,
            Font = UiFonts.Small
        };

        _languageComboBox = new ComboBox
        {
            Width = 120,
            Height = 34,
            DropDownStyle = ComboBoxStyle.DropDownList,
            TabIndex = 1
        };
        PopulateLanguageComboBox();
        _languageComboBox.SelectedIndexChanged += LanguageComboBox_SelectedIndexChanged;

        var actionsPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 5,
            RowCount = 1,
            Margin = Padding.Empty,
            TabIndex = 2,
            BackColor = UiColors.WindowBackground,
        };
        actionsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        actionsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        actionsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        actionsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        actionsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        actionsPanel.Controls.Add(_languageLabel, 0, 0);
        actionsPanel.Controls.Add(_languageComboBox, 1, 0);
        actionsPanel.Controls.Add(_refreshButton, 3, 0);
        actionsPanel.Controls.Add(_newTopicButton, 4, 0);
        _languageComboBox.Anchor = AnchorStyles.Left;
        _refreshButton.FlatAppearance.BorderColor = UiColors.BorderColor;
        _newTopicButton.FlatAppearance.BorderSize = 0;

        var headerTextPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Margin = Padding.Empty,
            BackColor = UiColors.WindowBackground
        };
        headerTextPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        headerTextPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        _pageTitleLabel.Dock = DockStyle.Fill;
        _pageDescriptionLabel.Dock = DockStyle.Fill;
        headerTextPanel.Controls.Add(_pageTitleLabel, 0, 0);
        headerTextPanel.Controls.Add(_pageDescriptionLabel, 0, 1);

        var headerPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = UiMetrics.HeaderHeight,
            ColumnCount = 1,
            RowCount = 2,
            TabIndex = 0,
            Padding = new Padding(UiMetrics.LargeSpacing, 14, UiMetrics.LargeSpacing, 0),
            BackColor = UiColors.WindowBackground
        };
        headerPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        headerPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        headerPanel.Controls.Add(headerTextPanel, 0, 0);
        headerPanel.Controls.Add(actionsPanel, 0, 1);

        _contentPanel = new Panel
        {
            Dock = DockStyle.Fill,
            TabIndex = 1,
            Padding = new Padding(UiMetrics.LargeSpacing, 0, UiMetrics.LargeSpacing, UiMetrics.StandardSpacing),
            BackColor = UiColors.WindowBackground
        };

        var statusStrip = new StatusStrip
        {
            Dock = DockStyle.Bottom,
            SizingGrip = false,
            BackColor = UiColors.CardBackground
        };
        _statusLabel = new ToolStripStatusLabel(AppStrings.Ready)
        {
            Spring = true,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = UiColors.SecondaryText
        };
        statusStrip.Items.Add(_statusLabel);

        var rightPanel = new Panel
        {
            Dock = DockStyle.Fill,
            TabIndex = 1,
            BackColor = UiColors.WindowBackground
        };
        rightPanel.Controls.Add(_contentPanel);
        rightPanel.Controls.Add(headerPanel);
        rightPanel.Controls.Add(statusStrip);

        Controls.Add(rightPanel);
        Controls.Add(_navigation);

        _dashboardView = new DashboardView();
        _dashboardTopicsView = new HealthTopicsView();
        _topicsView = new HealthTopicsView();

        _dashboardPresenter = new DashboardPresenter(_healthTopicService, _dashboardView);
        _dashboardTopicsPresenter = new HealthTopicPresenter(_healthTopicService, _dashboardTopicsView);
        _topicsPresenter = new HealthTopicPresenter(_healthTopicService, _topicsView);
        _dashboardPage = CreateDashboardPage();
        _timelineView = new TimelineView();
        _timelinePresenter = new TimelinePresenter(_healthEntryService, _timelineView);
        _sourcesView = new SourcesView();
        _sourcesPresenter = new SourcesPresenter(_sourceService, _sourcesView);
        _measurementsView = new MeasurementsView();
        _measurementsPresenter = new MeasurementsPresenter(_measurementService, _measurementsView);
        _sourcesView.SourceSelected += async (_, _) =>
        {
            try { await _sourcesPresenter.LoadSelectionAsync(); }
            catch { UiErrorHandler.ShowSafeError(this, AppStrings.OperationLoadLocalNotebookData); }
        };
        _sourcesView.NewLocationRequested += async (_, _) => await ShowCreateSourceLocationAsync();
        _sourcesView.NewNoteRequested += async (_, _) => await ShowCreateSourceNoteAsync();

        InitializeSessions();
        InitializeActions();
        InitializeLifecycle();
        InitializeParentLifecycle();
        ApplyTexts();

        Shown += async (_, _) =>
        {
            ShowPage(NavigationPage.Dashboard);
            await ReloadSafeAsync();
        };
    }

    private async void Navigation_PageRequested(object? sender, NavigationPageChangedEventArgs e)
    {
        ShowPage(e.Page);
        await ReloadSafeAsync();
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Navigation detaches inactive views; they are still owned by this form.
            _dashboardPage.Dispose();
            _topicsView.Dispose();
            _timelineView.Dispose();
            _sourcesView.Dispose();
            _measurementsView.Dispose();
            _sessionsView.Dispose();
            _actionsView.Dispose();
        }
        base.Dispose(disposing);
    }

    private async void LanguageComboBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_isApplyingLanguageSelection || _languageComboBox.SelectedItem is not LanguageSelectionItem item)
        {
            return;
        }

        AppLanguage.SetLanguage(item.Language);
        ApplyTexts();
        await ReloadSafeAsync();
    }

    private void ApplyTexts()
    {
        Text = AppStrings.AppTitle;
        _refreshButton.Text = AppStrings.Refresh;
        _newTopicButton.Text = AppStrings.NewHealthTopic;
        _languageLabel.Text = AppStrings.LanguageLabel + ":";
        _navigation.ApplyTexts();
        _dashboardView.ApplyTexts();
        _dashboardTopicsView.SetTitle(AppStrings.HealthTopics);
        _dashboardTopicsView.ApplyTexts();
        _topicsView.SetTitle(AppStrings.AllHealthTopics);
        _topicsView.ApplyTexts();
        _timelineView.ApplyTexts();
        _sourcesView.ApplyTexts();
        _measurementsView.ApplyTexts();
        _sessionsView.ApplyTexts();
        _actionsView.ApplyTexts();
        _dashboardActionsOverview.ApplyTexts();

        ShowPage(_currentPage);
        _statusLabel.Text = _lastLoadedTopicCount.HasValue
            ? AppStrings.FormatLoadedHealthTopics(_lastLoadedTopicCount.Value)
            : AppStrings.Ready;
    }

    private void PopulateLanguageComboBox()
    {
        _isApplyingLanguageSelection = true;
        try
        {
            _languageComboBox.Items.Clear();
            _languageComboBox.Items.Add(new LanguageSelectionItem(UiLanguage.German, AppStrings.LanguageGerman));
            _languageComboBox.Items.Add(new LanguageSelectionItem(UiLanguage.English, AppStrings.LanguageEnglish));

            UiLanguage current = AppLanguage.Current;
            for (int index = 0; index < _languageComboBox.Items.Count; index++)
            {
                if (_languageComboBox.Items[index] is LanguageSelectionItem item && item.Language == current)
                {
                    _languageComboBox.SelectedIndex = index;
                    break;
                }
            }
        }
        finally
        {
            _isApplyingLanguageSelection = false;
        }
    }

    private void ShowPage(NavigationPage page)
    {
        bool changingPage = _currentPage != page;
        _currentPage = page;
        _navigation.SelectPage(page);
        // Keep the active view attached when localizing; reparenting resets inherited binding managers.
        if (changingPage) _contentPanel.Controls.Clear();

        if (page == NavigationPage.Dashboard)
        {
            _pageTitleLabel.Text = AppStrings.Dashboard;
            _pageDescriptionLabel.Text = AppStrings.DashboardDescription;
            _contentPanel.Controls.Add(_dashboardPage);
        }
        else if (page == NavigationPage.HealthTopics)
        {
            _pageTitleLabel.Text = AppStrings.HealthTopics;
            _pageDescriptionLabel.Text = AppStrings.HealthTopicsDescription;
            _contentPanel.Controls.Add(_topicsView);
        }
        else if (page == NavigationPage.Timeline)
        {
            _pageTitleLabel.Text = AppStrings.Timeline;
            _pageDescriptionLabel.Text = AppStrings.TimelineDescription;
            _contentPanel.Controls.Add(_timelineView);
        }
        else if (page == NavigationPage.Sources)
        {
            _pageTitleLabel.Text = AppStrings.Sources;
            _pageDescriptionLabel.Text = AppStrings.SourcesDescription;
            _contentPanel.Controls.Add(_sourcesView);
        }
        else if (page == NavigationPage.Measurements)
        {
            _pageTitleLabel.Text = AppStrings.Measurements;
            _pageDescriptionLabel.Text = AppStrings.MeasurementsDescription;
            _contentPanel.Controls.Add(_measurementsView);
        }
        if (page == NavigationPage.Sessions)
        {
            _pageTitleLabel.UseMnemonic = false;
            _pageTitleLabel.Text = AppStrings.Sessions;
            _pageDescriptionLabel.Text = AppStrings.SessionsDescription;
            _contentPanel.Controls.Add(_sessionsView);
        }
        if (page == NavigationPage.Actions)
        {
            _pageTitleLabel.UseMnemonic = false;
            _pageTitleLabel.Text = AppStrings.Actions;
            _pageDescriptionLabel.Text = AppStrings.ActionsDescription;
            _contentPanel.Controls.Add(_actionsView);
        }
        _newTopicButton.Text = page == NavigationPage.Actions ? AppStrings.NewAction : page == NavigationPage.Timeline ? AppStrings.NewTimelineEntry
            : page == NavigationPage.Sources ? AppStrings.NewSource
            : page == NavigationPage.Measurements ? AppStrings.NewMeasurement
            : page == NavigationPage.Sessions ? AppStrings.NewSession : AppStrings.NewHealthTopic;
    }

    private Control CreateDashboardPage()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Margin = Padding.Empty,
            BackColor = UiColors.WindowBackground
        };

        // Reuse this page when navigating/localizing; do not accumulate abandoned containers.
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, UiMetrics.DashboardOverviewHeight));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, UiMetrics.CompactOverviewHeight));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        layout.Controls.Add(_dashboardView, 0, 0);
        _dashboardActionsOverview.Margin = new Padding(0, 0, 0, UiMetrics.StandardSpacing);
        layout.Controls.Add(_dashboardActionsOverview, 0, 1);
        layout.Controls.Add(_dashboardTopicsView, 0, 2);

        return layout;
    }

    private int _reloadGeneration;
    private async Task ReloadSafeAsync()
    {
        int generation = ++_reloadGeneration;
        var page = _currentPage;
        void SetStatus(string text)
        {
            // Older page loads must not overwrite the status of a later navigation/refresh.
            if (!IsDisposed && generation == _reloadGeneration && page == _currentPage) _statusLabel.Text = text;
        }
        try
        {
            if (_currentPage == NavigationPage.Actions)
            {
                SetStatus(AppStrings.LoadedActions(await _actionsPresenter.LoadAsync()));
                return;
            }
            if (_currentPage == NavigationPage.Sessions)
            {
                SetStatus(AppStrings.FormatLoadedSessions(await _sessionsPresenter.LoadAsync()));
                return;
            }
            if (_currentPage == NavigationPage.Measurements)
            {
                SetStatus(AppStrings.FormatLoadedMeasurements(await _measurementsPresenter.LoadAsync()));
                return;
            }
            if (_currentPage == NavigationPage.Sources)
            {
                SetStatus(AppStrings.FormatLoadedSources(await _sourcesPresenter.LoadAsync()));
                return;
            }
            if (_currentPage == NavigationPage.Timeline)
            {
                SetStatus(AppStrings.FormatLoadedEntries(await _timelinePresenter.LoadAsync()));
                return;
            }
            await _dashboardPresenter.LoadAsync().ConfigureAwait(true);
            if (IsDisposed || generation != _reloadGeneration) return;
            var dashboardTopics = await _dashboardTopicsPresenter.LoadAsync().ConfigureAwait(true);
            var topicPageTopics = await _topicsPresenter.LoadAsync().ConfigureAwait(true);
            if (IsDisposed || generation != _reloadGeneration) return;

            _lastLoadedTopicCount = _currentPage == NavigationPage.Dashboard
                ? dashboardTopics.Count
                : topicPageTopics.Count;

            SetStatus(AppStrings.FormatLoadedHealthTopics(_lastLoadedTopicCount.Value));
            if (_currentPage == NavigationPage.Dashboard)
            {
                var overview = await _healthActionService.GetOverviewAsync(DateOnly.FromDateTime(DateTime.Now));
                if (!IsDisposed && generation == _reloadGeneration && page == _currentPage)
                    _dashboardActionsOverview.SetOverview(overview);
            }
        }
        catch
        {
            if (IsDisposed || generation != _reloadGeneration) return;
            UiErrorHandler.ShowSafeError(this, AppStrings.OperationLoadLocalNotebookData);
            SetStatus(AppStrings.LoadingFailed);
        }
    }

    private async Task ShowCreateHealthTopicWizardAsync()
    {
        using var wizard = new CreateHealthTopicWizardForm(_healthTopicService);

        if (wizard.ShowDialog(this) == DialogResult.OK)
        {
            await ReloadSafeAsync();
        }
    }

    private async Task ShowCreateHealthEntryAsync()
    {
        try
        {
            var topics = await _healthTopicService.GetTopicSummariesAsync();
            using var dialog = new CreateHealthEntryForm(_healthEntryService, topics);
            if (dialog.ShowDialog(this) == DialogResult.OK) await ReloadSafeAsync();
        }
        catch { UiErrorHandler.ShowSafeError(this, AppStrings.OperationLoadLocalNotebookData); }
    }

    private async Task ShowCreateSourceAsync()
    {
        try
        {
            var topics = await _healthTopicService.GetTopicSummariesAsync();
            using var dialog = new CreateSourceForm(_sourceService, topics);
            if (dialog.ShowDialog(this) == DialogResult.OK)
                _statusLabel.Text = AppStrings.FormatLoadedSources(await _sourcesPresenter.LoadAsync(dialog.CreatedSourceId));
        }
        catch { UiErrorHandler.ShowSafeError(this, AppStrings.OperationLoadLocalNotebookData); }
    }

    private async Task ShowCreateMeasurementAsync()
    {
        try
        {
            var topics = await _healthTopicService.GetTopicSummariesAsync();
            using var dialog = new CreateMeasurementForm(_measurementService, topics);
            if (dialog.ShowDialog(this) == DialogResult.OK) await ReloadSafeAsync();
        }
        catch { UiErrorHandler.ShowSafeError(this, AppStrings.OperationLoadLocalNotebookData); }
    }

    private async Task ShowCreateSourceLocationAsync()
    {
        var source = _sourcesView.SelectedSource;
        if (source is null) return;
        try
        {
            using var dialog = new CreateSourceLocationForm(_sourceService, source);
            if (dialog.ShowDialog(this) == DialogResult.OK) await ReloadSafeAsync();
        }
        catch { UiErrorHandler.ShowSafeError(this, AppStrings.OperationLoadLocalNotebookData); }
    }

    private async Task ShowCreateSourceNoteAsync()
    {
        var source = _sourcesView.SelectedSource;
        if (source is null) return;
        try
        {
            var locations = await _sourceService.GetLocationsAsync(source.Id);
            using var dialog = new CreateEvidenceNoteForm(_sourceService, source, locations);
            if (dialog.ShowDialog(this) == DialogResult.OK) await ReloadSafeAsync();
        }
        catch { UiErrorHandler.ShowSafeError(this, AppStrings.OperationLoadLocalNotebookData); }
    }

    private sealed class LanguageSelectionItem
    {
        public LanguageSelectionItem(UiLanguage language, string displayName)
        {
            Language = language;
            DisplayName = displayName;
        }

        public UiLanguage Language { get; }

        private string DisplayName { get; }

        public override string ToString() => DisplayName;
    }
}
