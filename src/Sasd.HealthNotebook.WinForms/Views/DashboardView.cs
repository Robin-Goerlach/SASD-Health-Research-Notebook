using System.Drawing;
using System.Windows.Forms;
using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.WinForms.Controls;
using Sasd.HealthNotebook.WinForms.Styling;

namespace Sasd.HealthNotebook.WinForms.Views;

/// <summary>
/// Dashboard header and overview-card area.
/// </summary>
public sealed class DashboardView : UserControl
{
    private readonly DashboardCardControl _topicsCard;
    private readonly DashboardCardControl _prepareCard;
    private readonly DashboardCardControl _archivedCard;

    /// <summary>
    /// Initializes a new instance of the <see cref="DashboardView" /> class.
    /// </summary>
    public DashboardView()
    {
        BackColor = UiColors.WindowBackground;
        Dock = DockStyle.Fill;
        Padding = new Padding(0, 0, 0, UiMetrics.StandardSpacing);

        var descriptionLabel = new Label
        {
            Dock = DockStyle.Top,
            Height = 42,
            Font = UiFonts.Body,
            ForeColor = UiColors.SecondaryText,
            Text = "Personal local-first documentation workspace. This is a notebook, not a diagnosis or therapy system."
        };

        var titleLabel = new Label
        {
            Dock = DockStyle.Top,
            Height = 42,
            Font = UiFonts.PageTitle,
            ForeColor = UiColors.PrimaryText,
            Text = "Dashboard"
        };

        var cardsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoScroll = true,
            BackColor = UiColors.WindowBackground
        };

        _topicsCard = new DashboardCardControl();
        _prepareCard = new DashboardCardControl();
        _archivedCard = new DashboardCardControl();

        cardsPanel.Controls.Add(_topicsCard);
        cardsPanel.Controls.Add(_prepareCard);
        cardsPanel.Controls.Add(_archivedCard);

        Controls.Add(cardsPanel);
        Controls.Add(descriptionLabel);
        Controls.Add(titleLabel);

        SetOverview(new DashboardOverview());
    }

    /// <summary>
    /// Updates dashboard cards from the application service overview.
    /// </summary>
    public void SetOverview(DashboardOverview overview)
    {
        ArgumentNullException.ThrowIfNull(overview);

        _topicsCard.SetContent(
            "Health topics",
            overview.TotalTopics.ToString(),
            "Documented topics in the local notebook");

        _prepareCard.SetContent(
            "Prepare for doctor",
            overview.PrepareForDoctorCount.ToString(),
            "Topics marked for appointment preparation");

        _archivedCard.SetContent(
            "Archived",
            overview.ArchivedTopics.ToString(),
            "Topics no longer shown as active");
    }
}
