using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.WinForms.Localization;
using Sasd.HealthNotebook.WinForms.Styling;

namespace Sasd.HealthNotebook.WinForms.Controls;

/// <summary>Reusable compact documentative count strip for dashboard and action page.</summary>
public sealed class HealthActionOverviewControl : UserControl
{
    private readonly DashboardCardControl _actions = new(compact: true);
    private readonly DashboardCardControl _routines = new(compact: true);
    private readonly DashboardCardControl _today = new(compact: true);
    private HealthActionOverview _overview = new(0, 0, 0);
    /// <summary>Uses existing card styling and equal proportional columns.</summary>
    public HealthActionOverviewControl()
    {
        Dock = DockStyle.Fill; Margin = Padding.Empty; BackColor = UiColors.WindowBackground; TabStop = false;
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1, Margin = Padding.Empty };
        layout.RowStyles.Add(new(SizeType.Percent, 100));
        for (int i = 0; i < 3; i++) layout.ColumnStyles.Add(new(SizeType.Percent, 100F / 3));
        _actions.Dock = _routines.Dock = _today.Dock = DockStyle.Fill;
        _today.Margin = Padding.Empty;
        layout.Controls.Add(_actions, 0, 0); layout.Controls.Add(_routines, 1, 0); layout.Controls.Add(_today, 2, 0);
        Controls.Add(layout); ApplyTexts();
    }
    /// <summary>Displays only persisted documentation counts.</summary>
    public void SetOverview(HealthActionOverview overview) { _overview = overview; ApplyTexts(); }
    /// <summary>Refreshes labels without changing the counts.</summary>
    public void ApplyTexts()
    {
        _actions.SetContent(AppStrings.ActiveActions, _overview.ActiveActions.ToString(), AppStrings.ActionCountCaption);
        _routines.SetContent(AppStrings.ActiveRoutines, _overview.ActiveRoutines.ToString(), AppStrings.ActionCountCaption);
        _today.SetContent(AppStrings.ProgressToday, _overview.EntriesToday.ToString(), AppStrings.ActionCountCaption);
    }
}
