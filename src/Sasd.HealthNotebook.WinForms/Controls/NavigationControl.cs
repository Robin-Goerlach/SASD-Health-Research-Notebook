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
    private readonly Label _footerLabel;
    private readonly Label _titleLabel;
    private readonly NavigationButton _dashboardButton;
    private readonly NavigationButton _healthTopicsButton;

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

        _healthTopicsButton = new NavigationButton();
        _healthTopicsButton.Click += (_, _) => RequestPage(NavigationPage.HealthTopics);

        _dashboardButton = new NavigationButton();
        _dashboardButton.Click += (_, _) => RequestPage(NavigationPage.Dashboard);

        Controls.Add(_footerLabel);
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
        _footerLabel.Text = AppStrings.SidebarFooter;
    }

    /// <summary>
    /// Updates the selected navigation state.
    /// </summary>
    public void SelectPage(NavigationPage page)
    {
        _dashboardButton.IsSelected = page == NavigationPage.Dashboard;
        _healthTopicsButton.IsSelected = page == NavigationPage.HealthTopics;
    }

    private void RequestPage(NavigationPage page)
    {
        SelectPage(page);
        PageRequested?.Invoke(this, new NavigationPageChangedEventArgs(page));
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
    HealthTopics
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
