using Sasd.HealthNotebook.WinForms.Controls;

namespace Sasd.HealthNotebook.WinForms.Localization;

public static partial class AppStrings
{
    /// <summary>Short descriptions of implemented navigation targets, without medical assessment.</summary>
    public static string NavigationHint(NavigationPage page) => page switch
    {
        NavigationPage.Dashboard => SessionText("Themen, Termine, offene nächste Schritte und heutige Einträge im Überblick.", "Overview of topics, upcoming sessions, open follow-ups and today's entries."),
        NavigationPage.HealthTopics => SessionText("Gesundheitsthemen erfassen, ordnen, bearbeiten und archivieren.", "Record, organize, edit and archive health topics."),
        NavigationPage.Timeline => SessionText("Notizen, Beobachtungen, Rechercheeinträge und Messwerte im zeitlichen Verlauf anzeigen.", "View notes, observations, research entries and measurements in chronological order."),
        NavigationPage.Sources => SessionText("Quellen, genaue Fundstellen und eigene Einordnungen dokumentieren.", "Document sources, exact source locations and your own notes."),
        NavigationPage.Measurements => SessionText("Messwerte erfassen, nach Messart filtern und korrigieren.", "Record measurements, filter by type and correct entries."),
        NavigationPage.Sessions => SessionText("Termine, Gesprächsnotizen, Fragen, Antworten und nächste Schritte verwalten.", "Manage sessions, conversation notes, questions, answers and follow-ups."),
        NavigationPage.Actions => SessionText("Eigene Maßnahmen, Routinen und deren Durchführung dokumentieren.", "Document personal actions, routines and their execution."),
        _ => throw new ArgumentOutOfRangeException(nameof(page))
    };
}
