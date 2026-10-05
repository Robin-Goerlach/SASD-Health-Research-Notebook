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
        Margin = Padding.Empty;
        Padding = new Padding(0, 0, 0, UiMetrics.StandardSpacing);

        var cardsPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1,
            Margin = Padding.Empty,
            BackColor = UiColors.WindowBackground,
            Padding = Padding.Empty
        };
        for (int column = 0; column < 3; column++)
        {
            cardsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / 3));
        }
        cardsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        _topicsCard = new DashboardCardControl(compact: true) { Dock = DockStyle.Fill };
        _prepareCard = new DashboardCardControl(compact: true) { Dock = DockStyle.Fill };
        _archivedCard = new DashboardCardControl(compact: true) { Dock = DockStyle.Fill, Margin = Padding.Empty };

        cardsPanel.Controls.Add(_topicsCard, 0, 0);
        cardsPanel.Controls.Add(_prepareCard, 1, 0);
        cardsPanel.Controls.Add(_archivedCard, 2, 0);

        Controls.Add(cardsPanel);

        ApplyTexts();
    }

    /// <summary>
    /// Applies localized labels to the dashboard.
    /// </summary>
    public void ApplyTexts()
    {
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
            AppStrings.AgendaTopicsTitle,
            overview.TotalTopics.ToString(),
            AppStrings.AgendaTopicsCaption);

        _prepareCard.SetContent(
            AppStrings.AgendaPreparation,
            overview.PrepareForDoctorCount.ToString(),
            AppStrings.AgendaPreparationCaption);

        _archivedCard.SetContent(
            AppStrings.CardArchivedTitle,
            overview.ArchivedTopics.ToString(),
            AppStrings.AgendaArchiveCaption);
    }
}
