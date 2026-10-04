using Sasd.HealthNotebook.Domain;
using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.WinForms.Localization;
using Sasd.HealthNotebook.WinForms.Styling;

namespace Sasd.HealthNotebook.WinForms.Forms;

/// <summary>Read-only local instruction history; no restore or deletion of revisions.</summary>
public sealed class HealthActionHistoryForm : Form
{
    /// <summary>Displays previous full content and reported provenance, oldest correction first.</summary>
    public HealthActionHistoryForm(IReadOnlyList<HealthActionRevision> revisions, IReadOnlyList<HealthTopicSummary> topics,
        IReadOnlyList<SourceSummary> sources, IReadOnlyList<SessionSummary> sessions)
    {
        Text = AppStrings.Revisions; StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new(720, 540); Size = new(820, 640); Font = UiFonts.Body;
        var history = new RichTextBox { Dock = DockStyle.Fill, ReadOnly = true, DetectUrls = false, AccessibleName = AppStrings.Revisions };
        history.Text = revisions.Count == 0 ? AppStrings.NoRevisions : string.Join("\r\n\r\n────────────────────────\r\n\r\n", revisions.Select(revision =>
        {
            var previous = revision.Previous;
            return string.Join(Environment.NewLine, AppStrings.FormatDateTime(revision.ChangedAt),
                AppStrings.ActionTitle + ": " + previous.Title,
                AppStrings.ActionKind + ": " + AppStrings.ActionTypeText(previous.ActionType),
                AppStrings.ActionState + ": " + AppStrings.ActionStatusText(previous.Status),
                AppStrings.ActionOrigin + ": " + AppStrings.ActionOriginText(previous.Origin),
                AppStrings.EntryTopic + ": " + (previous.HealthTopicId.HasValue ? topics.SingleOrDefault(item => item.Id == previous.HealthTopicId)?.Title ?? AppStrings.MissingEntryTopic : AppStrings.NoEntryTopic),
                AppStrings.ActionSourceLink + ": " + (previous.SourceId.HasValue ? SourceLabel(sources.SingleOrDefault(item => item.Source.Id == previous.SourceId)) : AppStrings.NoActionLink),
                AppStrings.ActionSessionLink + ": " + (previous.SessionId.HasValue ? sessions.SingleOrDefault(item => item.Session.Id == previous.SessionId)?.Session.Title ?? AppStrings.MissingActionLink : AppStrings.NoActionLink),
                AppStrings.ActionOriginNote + ": " + previous.OriginNote,
                AppStrings.ActionDescription + ": " + previous.Description,
                AppStrings.ChangeReason + ": " + revision.ChangeReason);
        }));
        var close = new Button { Text = AppStrings.Cancel, Dock = DockStyle.Bottom, Height = UiMetrics.ActionHeight, DialogResult = DialogResult.Cancel };
        Controls.Add(history); Controls.Add(close); CancelButton = close;
    }
    private static string SourceLabel(SourceSummary? summary) => summary is null ? AppStrings.MissingActionLink
        : new[] { summary.Source.Title, summary.Source.AuthorOrInstitution, summary.Source.Url }
            .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value)) ?? AppStrings.MissingActionLink;
}
