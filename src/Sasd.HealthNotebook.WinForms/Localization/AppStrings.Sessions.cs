using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.WinForms.Localization;

public static partial class AppStrings
{
    public static string Sessions => SessionText("Termine & Fragen", "Sessions");
    public static string SessionsDescription => SessionText("Gespräche vorbereiten, eigene Antworten und nächste Schritte dokumentieren.", "Prepare conversations and document your answers and next steps.");
    public static string NewSession => SessionText("Neuer Termin", "New session");
    public static string NewSessionQuestion => SessionText("Neue Frage", "New question");
    public static string NewSessionFollowUp => SessionText("Neue Nachbereitung", "New follow-up");
    public static string SessionTitle => SessionText("Titel / Anlass des Termins", "Session title / reason");
    public static string SessionContact => SessionText("Ansprechpartner / Einrichtung (optional)", "Contact / institution (optional)");
    public static string SessionNotes => SessionText("Eigene Gesprächsnotiz (optional)", "Personal conversation note (optional)");
    public static string SessionKind => SessionText("Art des Gesprächs", "Conversation type");
    public static string SessionState => SessionText("Status", "Status");
    public static string SessionQuestions => SessionText("Fragen", "Questions");
    public static string SessionFollowUps => SessionText("Nachbereitung", "Follow-ups");
    public static string SessionQuestionText => SessionText("Ihre Frage", "Your question");
    public static string SessionAnswerNote => SessionText("Dokumentierte Antwort (optional)", "Recorded answer (optional)");
    public static string SessionAnswered => SessionText("Beantwortet", "Answered");
    public static string SessionOpen => SessionText("Offen", "Open");
    public static string SessionOrder => SessionText("Reihenfolge", "Display order");
    public static string SessionNextStep => SessionText("Eigener nächster Schritt / offener Punkt", "Your next step / open issue");
    public static string SessionDueDate => SessionText("Fälligkeitsdatum (optional)", "Due date (optional)");
    public static string SessionSemantics => SessionText("Dokumentieren Sie Ihre eigenen Fragen und das Gesagte.\r\nDie App bestätigt keine medizinischen Aussagen und empfiehlt keine Behandlung.", "Document your questions and what was said.\r\nThe app does not verify medical statements or recommend treatment.");
    public static string SessionsEmpty => SessionText("Noch keine Termine. Legen Sie einen Termin zur Gesprächsvorbereitung an.", "No sessions yet. Create a session to prepare a conversation.");
    public static string SessionSelectionEmpty => SessionText("Wählen Sie einen Termin aus.", "Select a session.");
    public static string SessionQuestionsEmpty => SessionText("Noch keine Fragen für diesen Termin.", "No questions for this session yet.");
    public static string SessionFollowUpsEmpty => SessionText("Noch keine Nachbereitung für diesen Termin.", "No follow-ups for this session yet.");
    public static string EditSessionAnswer => SessionText("Antwort dokumentieren", "Record answer");
    public static string ToggleSessionFollowUp => SessionText("Offen / erledigt ändern", "Change open / done");
    public static string SessionValidation => SessionText("Bitte Pflichttexte, Datum/Uhrzeit und Textlängen prüfen (Titel/Kontakt 160, Notizen 4000 Zeichen).", "Check required text, date/time and lengths (title/contact 160, notes 4000 characters).");
    public static string SaveSessionOperation => SessionText("Termine und Fragen speichern", "Save sessions and questions");
    public static string FormatLoadedSessions(int count) => SessionText($"{count} Termine geladen.", $"{count} sessions loaded.");
    public static string SessionTypeText(SessionType type) => type switch {
        SessionType.DoctorVisit => SessionText("Arztbesuch", "Doctor visit"), SessionType.Checkup => SessionText("Kontrolle", "Checkup"),
        SessionType.Coaching => SessionText("Coaching", "Coaching"), SessionType.Physiotherapy => SessionText("Physiotherapie", "Physiotherapy"),
        SessionType.NutritionConsultation => SessionText("Ernährungsberatung", "Nutrition consultation"), _ => SessionText("Weitere Beratung", "Other consultation") };
    public static string SessionStatusText(SessionStatus status) => status switch {
        SessionStatus.Planned => SessionText("Geplant", "Planned"), SessionStatus.Completed => SessionText("Abgeschlossen", "Completed"), _ => SessionText("Abgesagt", "Cancelled") };
    public static string FollowUpStatusText(SessionFollowUpStatus status) => status == SessionFollowUpStatus.Done ? SessionText("Erledigt", "Done") : SessionOpen;
    private static string SessionText(string german, string english) => Text(english, german);
}
