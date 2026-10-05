using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.WinForms.Localization;
using Sasd.HealthNotebook.WinForms.Styling;
using Sasd.HealthNotebook.WinForms.Views;

namespace Sasd.HealthNotebook.WinForms.Forms;

/// <summary>Complete read-only list from the accepted projection; activation returns exact IDs.</summary>
public sealed class AgendaListForm : Form
{
    /// <summary>Creates a bounded-window full list without loading or writing any store.</summary>
    public AgendaListForm(DashboardAgenda agenda, bool sessions)
    {
        Text = sessions ? AppStrings.UpcomingSessions : AppStrings.OpenFollowUps;
        StartPosition = FormStartPosition.CenterParent; Size = new(680, 500); MinimumSize = new(520, 360);
        BackColor = UiColors.WindowBackground; Font = UiFonts.Body; Padding = new Padding(UiMetrics.StandardSpacing);
        var close = new Button { Text = AppStrings.Cancel, Dock = DockStyle.Bottom, Height = UiMetrics.ActionHeight, DialogResult = DialogResult.Cancel };
        var view = new DashboardAgendaView(preview: false, sessionsOnly: sessions);
        view.SetAgenda(agenda);
        view.TargetRequested += (_, target) => { Target = target; DialogResult = DialogResult.OK; Close(); };
        Controls.Add(view); Controls.Add(close); CancelButton = close;
    }
    /// <summary>Explicitly activated stable target; null when cancelled.</summary>
    public AgendaTargetEventArgs? Target { get; private set; }
}
