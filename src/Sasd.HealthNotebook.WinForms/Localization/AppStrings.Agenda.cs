using Sasd.HealthNotebook.Application.Contracts;

namespace Sasd.HealthNotebook.WinForms.Localization;

public static partial class AppStrings
{
    public static string UpcomingSessions => SessionText("Nächste Termine", "Upcoming sessions");
    public static string OpenFollowUps => SessionText("Offene nächste Schritte", "Open follow-ups");
    public static string UpcomingSessionsEmpty => SessionText("Keine kommenden Termine.", "No upcoming sessions.");
    public static string OpenFollowUpsEmpty => SessionText("Keine offenen nächsten Schritte.", "No open follow-ups.");
    public static string AgendaLoading => SessionText("Übersicht wird geladen…", "Loading agenda…");
    public static string AgendaFailed => SessionText("Übersicht konnte nicht geladen werden. Bitte aktualisieren.", "Agenda could not be loaded. Please refresh.");
    public static string AgendaShowAll => SessionText("Alle anzeigen", "Show all");
    public static string AgendaBoundary => SessionText("Ihre dokumentierten Themen, Termine und nächsten Schritte.", "Your documented topics, sessions and next steps.");
    public static string AgendaPreparation => SessionText("Vorbereitung", "Preparation");
    public static string AgendaTopicsTitle => SessionText("Themen", "Health topics");
    public static string AgendaTopicsCaption => SessionText("Dokumentierte Themen", "Documented topics");
    public static string AgendaPreparationCaption => SessionText("Für den Arzttermin", "For a doctor session");
    public static string AgendaArchiveCaption => SessionText("Archivierte Themen", "Archived topics");
    public static string AgendaDate => SessionText("Datum / Zeit", "Date / time");
    public static string AgendaGrouping => SessionText("Fälligkeit", "Due date");
    public static string AgendaDueGroup(FollowUpDueGroup group) => group switch
    {
        FollowUpDueGroup.Overdue => SessionText("Überfällig", "Overdue"),
        FollowUpDueGroup.Today => SessionText("Heute", "Today"),
        FollowUpDueGroup.Later => SessionText("Später", "Later"),
        FollowUpDueGroup.NoDate => SessionText("Ohne Datum", "No date"),
        _ => throw new ArgumentOutOfRangeException(nameof(group))
    };
}
