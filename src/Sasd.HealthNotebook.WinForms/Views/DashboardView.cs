using System.Drawing;
using System.Windows.Forms;
using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.WinForms.Controls;
using Sasd.HealthNotebook.WinForms.Localization;
using Sasd.HealthNotebook.WinForms.Styling;

namespace Sasd.HealthNotebook.WinForms.Views;

/// <summary>
/// Dashboard overview-card area.
/// </summary>
public sealed class DashboardView : UserControl
{
    private readonly Label _descriptionLabel;
    private readonly DashboardCardControl _topicsCard;
    private readonly DashboardCardControl _prepareCard;
    private readonly DashboardCardControl _archivedCard;
    private DashboardOverview _lastOverview = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="DashboardView" /> class.
    /// </summary>
    public DashboardView()
    {
        BackColor = UiColors.WindowBackground;
        Dock = DockStyle.Fill;
        Padding = new Padding(0, 0, 0, UiMetrics.StandardSpacing);

        _descriptionLabel = new Label
        {
            Dock = DockStyle.Top,
            Height = 36,
            Font = UiFonts.Body,
            ForeColor = UiColors.SecondaryText,
            AutoEllipsis = true
        };

        var cardsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoScroll = false,
            BackColor = UiColors.WindowBackground,
            Padding = new Padding(0, UiMetrics.StandardSpacing, 0, 0)
        };

        _topicsCard = new DashboardCardControl();
        _prepareCard = new DashboardCardControl();
        _archivedCard = new DashboardCardControl();

        cardsPanel.Controls.Add(_topicsCard);
        cardsPanel.Controls.Add(_prepareCard);
        cardsPanel.Controls.Add(_archivedCard);

        Controls.Add(cardsPanel);
        Controls.Add(_descriptionLabel);

        ApplyTexts();
    }

    /// <summary>
    /// Applies localized labels to the dashboard.
    /// </summary>
    public void ApplyTexts()
    {
        _descriptionLabel.Text = AppStrings.DashboardNotebookBoundary;
        SetOverview(_lastOverview);
    }

    /// <summary>
    /// Updates dashboard cards from the application service overview.
    /// </summary>
    public void SetOverview(DashboardOverview overview)
    {
        ArgumentNullException.ThrowIfNull(overview);
        _lastOverview = overview;

        _topicsCard.SetContent(
            AppStrings.CardHealthTopicsTitle,
            overview.TotalTopics.ToString(),
            AppStrings.CardHealthTopicsDescription);

        _prepareCard.SetContent(
            AppStrings.CardPrepareForDoctorTitle,
            overview.PrepareForDoctorCount.ToString(),
            AppStrings.CardPrepareForDoctorDescription);

        _archivedCard.SetContent(
            AppStrings.CardArchivedTitle,
            overview.ArchivedTopics.ToString(),
            AppStrings.CardArchivedDescription);
    }
}
