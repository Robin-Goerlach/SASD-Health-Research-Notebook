using System.Drawing;
using System.Windows.Forms;
using Sasd.HealthNotebook.WinForms.Localization;
using Sasd.HealthNotebook.WinForms.Styling;

namespace Sasd.HealthNotebook.WinForms.Controls;

/// <summary>
/// Left-hand application navigation.
/// </summary>
public sealed class NavigationControl : UserControl
{
    private readonly ToolTip _navigationToolTip = new() { AutoPopDelay = 10000, InitialDelay = 500, ReshowDelay = 100, ShowAlways = true };
    private readonly Label _footerLabel;
    private readonly Label _titleLabel;
    private readonly NavigationButton _dashboardButton;
    private readonly NavigationButton _healthTopicsButton;
    private readonly NavigationButton _timelineButton;
    private readonly NavigationButton _sourcesButton;
    private readonly NavigationButton _measurementsButton;
    private readonly NavigationButton _sessionsButton;
    private readonly NavigationButton _actionsButton;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationControl" /> class.
    /// </summary>
    public NavigationControl()
    {
        BackColor = UiColors.SidebarBackground;
        Width = UiMetrics.SidebarWidth;
        Dock = DockStyle.Left;

        _footerLabel = new Label
        {
            Dock = DockStyle.Bottom,
            Height = 72,
            Padding = new Padding(UiMetrics.Padding),
            Font = UiFonts.Small,
            ForeColor = UiColors.SidebarText,
            TextAlign = ContentAlignment.MiddleLeft
        };

        _titleLabel = new Label
        {
            Dock = DockStyle.Top,
            Height = 116,
            Padding = new Padding(UiMetrics.Padding, 22, UiMetrics.Padding, 0),
            Font = new Font("Segoe UI Semibold", 14F, FontStyle.Regular, GraphicsUnit.Point),
            ForeColor = UiColors.SidebarText,
            Text = "SASD\r\nHealth Research\r\nNotebook",
            TextAlign = ContentAlignment.TopLeft
        };

        _healthTopicsButton = new NavigationButton { TabIndex = 1 };
        _healthTopicsButton.Click += (_, _) => RequestPage(NavigationPage.HealthTopics);

        _dashboardButton = new NavigationButton { TabIndex = 0 };
        _dashboardButton.Click += (_, _) => RequestPage(NavigationPage.Dashboard);
        _dashboardButton.KeyDown += Navigation_KeyDown;
        _healthTopicsButton.KeyDown += Navigation_KeyDown;
        _timelineButton = new NavigationButton { TabIndex = 2 };
        _timelineButton.Click += (_, _) => RequestPage(NavigationPage.Timeline);
        _timelineButton.KeyDown += Navigation_KeyDown;
        _sourcesButton = new NavigationButton { TabIndex = 3 };
        _sourcesButton.Click += (_, _) => RequestPage(NavigationPage.Sources);
        _sourcesButton.KeyDown += Navigation_KeyDown;
        _measurementsButton = new NavigationButton { TabIndex = 4 };
        _measurementsButton.Click += (_, _) => RequestPage(NavigationPage.Measurements);
        _measurementsButton.KeyDown += Navigation_KeyDown;

        _sessionsButton = new NavigationButton { TabIndex = 5, UseMnemonic = false };
        _sessionsButton.Click += (_, _) => RequestPage(NavigationPage.Sessions);
        _sessionsButton.KeyDown += Navigation_KeyDown;
        _actionsButton = new NavigationButton { TabIndex = 6, UseMnemonic = false };
        _actionsButton.Click += (_, _) => RequestPage(NavigationPage.Actions);
        _actionsButton.KeyDown += Navigation_KeyDown;
        Controls.Add(_actionsButton);
        Controls.Add(_sessionsButton);
        Controls.Add(_footerLabel);
        Controls.Add(_measurementsButton);
        Controls.Add(_sourcesButton);
        Controls.Add(_timelineButton);
        Controls.Add(_healthTopicsButton);
        Controls.Add(_dashboardButton);
        Controls.Add(_titleLabel);

        ApplyTexts();
        SelectPage(NavigationPage.Dashboard);
    }

    /// <summary>
    /// Occurs when the user requests a navigation target.
    /// </summary>
    public event EventHandler<NavigationPageChangedEventArgs>? PageRequested;

    /// <summary>
    /// Applies localized labels to the navigation.
    /// </summary>
    public void ApplyTexts()
    {
        _dashboardButton.Text = AppStrings.Dashboard;
        _healthTopicsButton.Text = AppStrings.HealthTopics;
        _timelineButton.Text = AppStrings.Timeline;
        _sourcesButton.Text = AppStrings.Sources;
        _measurementsButton.Text = AppStrings.Measurements;
        _sessionsButton.Text = AppStrings.Sessions;
        _actionsButton.Text = AppStrings.Actions;
        _footerLabel.Text = AppStrings.SidebarFooter;
        NavigationButton[] buttons = { _dashboardButton, _healthTopicsButton, _timelineButton,
            _sourcesButton, _measurementsButton, _sessionsButton, _actionsButton };
        NavigationPage[] pages = { NavigationPage.Dashboard, NavigationPage.HealthTopics, NavigationPage.Timeline,
            NavigationPage.Sources, NavigationPage.Measurements, NavigationPage.Sessions, NavigationPage.Actions };
        for (int index = 0; index < buttons.Length; index++)
            _navigationToolTip.SetToolTip(buttons[index], AppStrings.NavigationHint(pages[index]));
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    { if (disposing) _navigationToolTip.Dispose(); base.Dispose(disposing); }

    /// <summary>
    /// Updates the selected navigation state.
    /// </summary>
    public void SelectPage(NavigationPage page)
    {
        _dashboardButton.IsSelected = page == NavigationPage.Dashboard;
        _healthTopicsButton.IsSelected = page == NavigationPage.HealthTopics;
        _timelineButton.IsSelected = page == NavigationPage.Timeline;
        _sourcesButton.IsSelected = page == NavigationPage.Sources;
        _measurementsButton.IsSelected = page == NavigationPage.Measurements;
        _sessionsButton.IsSelected = page == NavigationPage.Sessions;
        _actionsButton.IsSelected = page == NavigationPage.Actions;
    }

    private void RequestPage(NavigationPage page)
    {
        SelectPage(page);
        PageRequested?.Invoke(this, new NavigationPageChangedEventArgs(page));
    }

    private void Navigation_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode is Keys.Up or Keys.Down or Keys.Home or Keys.End)
        {
            // Move focus without loading a page until the user presses Enter/Space.
            NavigationButton[] buttons = { _dashboardButton, _healthTopicsButton, _timelineButton, _sourcesButton, _measurementsButton, _sessionsButton, _actionsButton };
            int index = Array.IndexOf(buttons, sender);
            int target = e.KeyCode == Keys.Home ? 0 : e.KeyCode == Keys.End ? buttons.Length - 1
                : (index + (e.KeyCode == Keys.Down ? 1 : -1) + buttons.Length) % buttons.Length;
            buttons[target].Focus();
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
    }
}

/// <summary>
/// Supported first-milestone navigation pages.
/// </summary>
public enum NavigationPage
{
    /// <summary>Dashboard overview page.</summary>
    Dashboard,

    /// <summary>Health-topic list page.</summary>
    HealthTopics,

    /// <summary>User-entered notes, observations and research.</summary>
    Timeline,

    /// <summary>Sources, concrete locations and separate personal notes.</summary>
    Sources,

    /// <summary>Manually documented numeric measurements.</summary>
    Measurements,
    /// <summary>User-documented conversations, questions and next steps.</summary>
    Sessions,
    /// <summary>Personal actions, routines and execution history.</summary>
    Actions
}

/// <summary>
/// Event arguments for navigation changes.
/// </summary>
public sealed class NavigationPageChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationPageChangedEventArgs" /> class.
    /// </summary>
    public NavigationPageChangedEventArgs(NavigationPage page)
    {
        Page = page;
    }

    /// <summary>
    /// Gets the requested page.
    /// </summary>
    public NavigationPage Page { get; }
}
