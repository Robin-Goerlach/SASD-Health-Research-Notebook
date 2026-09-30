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
public sealed class MainForm : Form
{
    private readonly HealthTopicService _healthTopicService;
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
    private bool _isApplyingLanguageSelection;
    private int? _lastLoadedTopicCount;
    private NavigationPage _currentPage = NavigationPage.Dashboard;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainForm" /> class.
    /// </summary>
    public MainForm(HealthTopicService healthTopicService)
    {
        _healthTopicService = healthTopicService ?? throw new ArgumentNullException(nameof(healthTopicService));

        Text = AppStrings.AppTitle;
        MinimumSize = new Size(1120, 740);
        Size = new Size(1280, 820);
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

        _refreshButton = new Button
        {
            Width = 120,
            Height = 34
        };
        _refreshButton.Click += async (_, _) => await ReloadSafeAsync();

        _newTopicButton = new Button
        {
            Width = 190,
            Height = 34
        };
        _newTopicButton.Click += async (_, _) => await ShowCreateHealthTopicWizardAsync();

        _languageLabel = new Label
        {
            AutoSize = false,
            Width = 70,
            Height = 34,
            TextAlign = ContentAlignment.MiddleRight,
            ForeColor = UiColors.SecondaryText,
            Font = UiFonts.Small
        };

        _languageComboBox = new ComboBox
        {
            Width = 120,
            Height = 34,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        PopulateLanguageComboBox();
        _languageComboBox.SelectedIndexChanged += LanguageComboBox_SelectedIndexChanged;

        var actionsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            Width = 540,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(0, 34, 0, 0),
            BackColor = UiColors.WindowBackground,
            WrapContents = false
        };
        actionsPanel.Controls.Add(_newTopicButton);
        actionsPanel.Controls.Add(_refreshButton);
        actionsPanel.Controls.Add(_languageComboBox);
        actionsPanel.Controls.Add(_languageLabel);

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
        _statusLabel = new ToolStripStatusLabel(AppStrings.Ready);
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
        _topicsView = new HealthTopicsView();

        _dashboardPresenter = new DashboardPresenter(_healthTopicService, _dashboardView);
        _dashboardTopicsPresenter = new HealthTopicPresenter(_healthTopicService, _dashboardTopicsView);
        _topicsPresenter = new HealthTopicPresenter(_healthTopicService, _topicsView);

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
        _currentPage = page;
        _navigation.SelectPage(page);
        _contentPanel.Controls.Clear();

        if (page == NavigationPage.Dashboard)
        {
            _pageTitleLabel.Text = AppStrings.Dashboard;
            _pageDescriptionLabel.Text = AppStrings.DashboardDescription;
            _contentPanel.Controls.Add(CreateDashboardPage());
        }
        else
        {
            _pageTitleLabel.Text = AppStrings.HealthTopics;
            _pageDescriptionLabel.Text = AppStrings.HealthTopicsDescription;
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

        // Keep the card row high enough for the full card content. The previous
        // version used an inner scroll area, which made the cards look clipped on
        // first start even at normal desktop window sizes.
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 184));
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

            _lastLoadedTopicCount = _currentPage == NavigationPage.Dashboard
                ? dashboardTopics.Count
                : topicPageTopics.Count;

            _statusLabel.Text = AppStrings.FormatLoadedHealthTopics(_lastLoadedTopicCount.Value);
        }
        catch
        {
            UiErrorHandler.ShowSafeError(this, AppStrings.OperationLoadLocalNotebookData);
            _statusLabel.Text = AppStrings.LoadingFailed;
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
