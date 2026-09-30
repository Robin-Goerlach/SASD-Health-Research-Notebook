using System.Drawing;
using System.Windows.Forms;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.WinForms.Controls;
using Sasd.HealthNotebook.WinForms.Presentation;
using Sasd.HealthNotebook.WinForms.Styling;
using Sasd.HealthNotebook.WinForms.Views;

namespace Sasd.HealthNotebook.WinForms.Forms;

/// <summary>
/// Main window for the Windows Forms frontend.
/// </summary>
public sealed class MainForm : Form
{
    private readonly HealthTopicService _healthTopicService;
    private readonly NavigationControl _navigation;
    private readonly Panel _contentPanel;
    private readonly Label _pageTitleLabel;
    private readonly Label _pageDescriptionLabel;
    private readonly ToolStripStatusLabel _statusLabel;
    private readonly DashboardView _dashboardView;
    private readonly HealthTopicsView _dashboardTopicsView;
    private readonly HealthTopicsView _topicsView;
    private readonly DashboardPresenter _dashboardPresenter;
    private readonly HealthTopicPresenter _dashboardTopicsPresenter;
    private readonly HealthTopicPresenter _topicsPresenter;
    private NavigationPage _currentPage = NavigationPage.Dashboard;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainForm" /> class.
    /// </summary>
    public MainForm(HealthTopicService healthTopicService)
    {
        _healthTopicService = healthTopicService ?? throw new ArgumentNullException(nameof(healthTopicService));

        Text = "SASD Health Research Notebook";
        MinimumSize = new Size(1080, 720);
        Size = new Size(1220, 780);
        BackColor = UiColors.WindowBackground;
        Font = UiFonts.Body;

        _navigation = new NavigationControl();
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

        var refreshButton = new Button
        {
            Text = "Refresh",
            Width = 110,
            Height = 34
        };
        refreshButton.Click += async (_, _) => await ReloadSafeAsync();

        var newTopicButton = new Button
        {
            Text = "New Health Topic",
            Width = 160,
            Height = 34
        };
        newTopicButton.Click += async (_, _) => await ShowCreateHealthTopicWizardAsync();

        var actionsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            Width = 300,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(0, 34, 0, 0),
            BackColor = UiColors.WindowBackground
        };
        actionsPanel.Controls.Add(newTopicButton);
        actionsPanel.Controls.Add(refreshButton);

        var headerTextPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = UiColors.WindowBackground
        };
        headerTextPanel.Controls.Add(_pageDescriptionLabel);
        headerTextPanel.Controls.Add(_pageTitleLabel);

        var headerPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = UiMetrics.HeaderHeight,
            Padding = new Padding(UiMetrics.LargeSpacing, 14, UiMetrics.LargeSpacing, 0),
            BackColor = UiColors.WindowBackground
        };
        headerPanel.Controls.Add(headerTextPanel);
        headerPanel.Controls.Add(actionsPanel);

        _contentPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(UiMetrics.LargeSpacing, 0, UiMetrics.LargeSpacing, UiMetrics.StandardSpacing),
            BackColor = UiColors.WindowBackground
        };

        var statusStrip = new StatusStrip
        {
            Dock = DockStyle.Bottom,
            SizingGrip = false,
            BackColor = UiColors.CardBackground
        };
        _statusLabel = new ToolStripStatusLabel("Ready.");
        statusStrip.Items.Add(_statusLabel);

        var rightPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = UiColors.WindowBackground
        };
        rightPanel.Controls.Add(_contentPanel);
        rightPanel.Controls.Add(headerPanel);
        rightPanel.Controls.Add(statusStrip);

        Controls.Add(rightPanel);
        Controls.Add(_navigation);

        _dashboardView = new DashboardView();
        _dashboardTopicsView = new HealthTopicsView();
        _dashboardTopicsView.SetTitle("Gesundheitsthemen");
        _topicsView = new HealthTopicsView();
        _topicsView.SetTitle("Alle Gesundheitsthemen");

        _dashboardPresenter = new DashboardPresenter(_healthTopicService, _dashboardView);
        _dashboardTopicsPresenter = new HealthTopicPresenter(_healthTopicService, _dashboardTopicsView);
        _topicsPresenter = new HealthTopicPresenter(_healthTopicService, _topicsView);

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

    private void ShowPage(NavigationPage page)
    {
        _currentPage = page;
        _navigation.SelectPage(page);
        _contentPanel.Controls.Clear();

        if (page == NavigationPage.Dashboard)
        {
            _pageTitleLabel.Text = "Dashboard";
            _pageDescriptionLabel.Text = "Local-first overview of documented health topics.";
            _contentPanel.Controls.Add(CreateDashboardPage());
        }
        else
        {
            _pageTitleLabel.Text = "Gesundheitsthemen";
            _pageDescriptionLabel.Text = "List of locally stored documentation topics.";
            _contentPanel.Controls.Add(_topicsView);
        }
    }

    private Control CreateDashboardPage()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = UiColors.WindowBackground
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 192));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        layout.Controls.Add(_dashboardView, 0, 0);
        layout.Controls.Add(_dashboardTopicsView, 0, 1);

        return layout;
    }

    private async Task ReloadSafeAsync()
    {
        try
        {
            await _dashboardPresenter.LoadAsync().ConfigureAwait(true);
            var dashboardTopics = await _dashboardTopicsPresenter.LoadAsync().ConfigureAwait(true);
            var topicPageTopics = await _topicsPresenter.LoadAsync().ConfigureAwait(true);

            int topicCount = _currentPage == NavigationPage.Dashboard
                ? dashboardTopics.Count
                : topicPageTopics.Count;

            _statusLabel.Text = $"Loaded {topicCount} health topic(s).";
        }
        catch
        {
            UiErrorHandler.ShowSafeError(this, "load local notebook data");
            _statusLabel.Text = "Loading failed. See message.";
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
}
