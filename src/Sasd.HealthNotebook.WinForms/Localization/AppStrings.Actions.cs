using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.WinForms.Localization;

public static partial class AppStrings
{
    private static string ActionText(string de, string en) => AppLanguage.Current == UiLanguage.German ? de : en;
    /// <summary>Navigation and page title.</summary>
    public static string Actions => ActionText("Maßnahmen & Routinen", "Actions & Routines");
    /// <summary>Short page guidance.</summary>
    public static string ActionsDescription => ActionText("Eigene Maßnahmen organisieren und die Durchführung dokumentieren.", "Organize personal actions and document their execution.");
    /// <summary>Action list heading.</summary>
    public static string ActionList => ActionText("Maßnahmen", "Actions");
    /// <summary>Routine list heading.</summary>
    public static string RoutineList => ActionText("Routinen", "Routines");
    /// <summary>History heading.</summary>
    public static string ProgressHistory => ActionText("Durchführung & Verlauf", "Execution & History");
    /// <summary>Main action.</summary>
    public static string NewAction => ActionText("Neue Maßnahme", "New action");
    /// <summary>Add routine.</summary>
    public static string NewRoutine => ActionText("Neue Routine", "New routine");
    /// <summary>Add execution record.</summary>
    public static string NewProgress => ActionText("Durchführung eintragen", "Record execution");
    /// <summary>Explicit pause/reactivate command.</summary>
    public static string PauseRoutine => ActionText("Pausieren", "Pause");
    /// <summary>Explicit reactivation command.</summary>
    public static string ActivateRoutine => ActionText("Aktivieren", "Activate");
    /// <summary>First-use guidance.</summary>
    public static string ActionsEmpty => ActionText("Noch keine Maßnahmen.\r\nBeginnen Sie mit „Neue Maßnahme“.", "No actions yet.\r\nStart with “New action”.");
    /// <summary>Routine first-use guidance.</summary>
    public static string RoutinesEmpty => ActionText("Noch keine Routinen für diese Maßnahme.\r\nLegen Sie bei Bedarf eine eigene Routine an.", "No routines for this action yet.\r\nAdd a personal routine if needed.");
    /// <summary>Selection guidance.</summary>
    public static string SelectAction => ActionText("Wählen Sie links eine Maßnahme.", "Select an action on the left.");
    /// <summary>Selection guidance for routine history.</summary>
    public static string SelectRoutine => ActionText("Wählen Sie oben eine Routine.", "Select a routine above.");
    /// <summary>Empty routine history.</summary>
    public static string ProgressEmpty => ActionText("Noch keine Durchführung dokumentiert.\r\nEinträge bleiben als eigener Verlauf erhalten.", "No execution recorded yet.\r\nEntries remain in your personal history.");
    /// <summary>Dialog guidance and safety boundary.</summary>
    public static string ActionSemantics => ActionText("Persönliche Dokumentation: Erfassen Sie eigene Angaben und berichtete Herkunft.\r\nDie App gibt keine medizinische Empfehlung und bewertet keine Wirksamkeit.", "Personal documentation: record your own information and reported origin.\r\nThe app gives no medical advice or effectiveness assessment.");
    /// <summary>Title editor label.</summary>
    public static string ActionTitle => ActionText("Titel der Maßnahme", "Action title");
    /// <summary>Routine title editor label.</summary>
    public static string RoutineTitle => ActionText("Titel der Routine", "Routine title");
    /// <summary>Personal category editor label.</summary>
    public static string ActionKind => ActionText("Kategorie", "Category");
    /// <summary>Reported origin editor label.</summary>
    public static string ActionOrigin => ActionText("Herkunft laut Nutzer", "User-reported origin");
    /// <summary>Reported origin detail editor label.</summary>
    public static string ActionOriginNote => ActionText("Herkunftsnotiz", "Origin note");
    /// <summary>Optional source link.</summary>
    public static string ActionSourceLink => ActionText("Quelle (optional)", "Source (optional)");
    /// <summary>Optional conversation link.</summary>
    public static string ActionSessionLink => ActionText("Termin (optional)", "Session (optional)");
    /// <summary>Unlinked choice.</summary>
    public static string NoActionLink => ActionText("Keine Verknüpfung", "No link");
    /// <summary>Missing origin record; never drop the action.</summary>
    public static string MissingActionLink => ActionText("Verknüpfung nicht verfügbar", "Linked record unavailable");
    /// <summary>Explicit optional provenance without copied medical contents.</summary>
    public static string ActionLinkHelp => ActionText("Optionaler Herkunftsverweis. Inhalte werden nicht übernommen oder bewertet.", "Optional origin reference. Contents are not copied or evaluated.");
    /// <summary>Personal description editor label.</summary>
    public static string ActionDescription => ActionText("Beschreibung", "Description");
    /// <summary>User-selected status editor label.</summary>
    public static string ActionState => ActionText("Status", "Status");
    /// <summary>Optional personal rhythm editor label.</summary>
    public static string RoutineSchedule => ActionText("Rhythmus (optional)", "Rhythm (optional)");
    /// <summary>No automatic reminder semantics.</summary>
    public static string RoutineScheduleHelp => ActionText("Eigener Rhythmus als Text, etwa täglich oder ausgewählte Tage. Keine Benachrichtigung.", "Your rhythm as text, such as daily or selected days. No notifications.");
    /// <summary>Execution editor label.</summary>
    public static string ProgressState => ActionText("Durchführung", "Execution");
    /// <summary>Optional personal execution count.</summary>
    public static string ProgressCount => ActionText("Anzahl (optional)", "Count (optional)");
    /// <summary>Compact history column heading.</summary>
    public static string ProgressCountColumn => ActionText("Anzahl", "Count");
    /// <summary>Enable optional count entry.</summary>
    public static string RecordProgressCount => ActionText("Anzahl erfassen", "Record count");
    /// <summary>Optional count has no quota or effectiveness meaning.</summary>
    public static string ProgressCountHelp => ActionText("Eigene Anzahl von Durchführungen, ohne Zielquote oder Bewertung.", "Your own execution count, without a target or assessment.");
    /// <summary>Personal history note editor label.</summary>
    public static string ProgressNote => ActionText("Eigene Notiz", "Personal note");
    /// <summary>Count card title.</summary>
    public static string ActiveActions => ActionText("Aktive Maßnahmen", "Active actions");
    /// <summary>Count card title.</summary>
    public static string ActiveRoutines => ActionText("Aktive Routinen", "Active routines");
    /// <summary>Count card title; counts all statuses, not completions.</summary>
    public static string ProgressToday => ActionText("Einträge heute", "Entries today");
    /// <summary>Documentative overview caption.</summary>
    public static string ActionCountCaption => ActionText("Ihre Dokumentation", "Your documentation");
    /// <summary>Safe validation message.</summary>
    public static string ActionValidation => ActionText("Bitte Titel, gültige Auswahl, Zeitpunkt und Textlängen prüfen (Titel/Rhythmus 160, Notizen 4000 Zeichen).", "Check title, selection, time and text lengths (title/rhythm 160, notes 4000 characters).");
    /// <summary>Content-free save operation.</summary>
    public static string SaveActionOperation => ActionText("Maßnahme / Routine / Durchführung speichern", "Save action / routine / execution");
    /// <summary>Content-free status count.</summary>
    public static string LoadedActions(int count) => ActionText($"{count} Maßnahmen geladen.", $"{count} actions loaded.");
    /// <summary>Localized personal category.</summary>
    public static string ActionTypeText(HealthActionType type) => type switch
    {
        HealthActionType.Movement => ActionText("Bewegung", "Movement"), HealthActionType.Nutrition => ActionText("Ernährung", "Nutrition"),
        HealthActionType.Sleep => ActionText("Schlaf", "Sleep"), HealthActionType.Relaxation => ActionText("Entspannung", "Relaxation"),
        HealthActionType.Organization => ActionText("Organisation", "Organization"), _ => ActionText("Sonstiges", "Other")
    };
    /// <summary>Localized reported provenance.</summary>
    public static string ActionOriginText(HealthActionOrigin origin) => origin switch
    {
        HealthActionOrigin.SelfDefined => ActionText("Selbst definiert", "Self-defined"), HealthActionOrigin.Doctor => ActionText("Arzt", "Doctor"),
        HealthActionOrigin.Therapist => ActionText("Therapeut", "Therapist"), HealthActionOrigin.Coach => "Coach",
        HealthActionOrigin.Source => ActionText("Quelle", "Source"), _ => ActionText("Sonstiges", "Other")
    };
    /// <summary>Localized action state.</summary>
    public static string ActionStatusText(HealthActionStatus status) => status == HealthActionStatus.Active ? ActionText("Aktiv", "Active") : ActionText("Inaktiv", "Inactive");
    /// <summary>Localized routine state.</summary>
    public static string RoutineStatusText(RoutineStatus status) => status == RoutineStatus.Active ? ActionText("Aktiv", "Active") : ActionText("Pausiert", "Paused");
    /// <summary>Localized execution without effectiveness claims.</summary>
    public static string CompletionText(ProgressCompletion completion) => completion switch
    {
        ProgressCompletion.Performed => ActionText("Durchgeführt", "Performed"),
        ProgressCompletion.NotPerformed => ActionText("Nicht durchgeführt", "Not performed"), _ => ActionText("Übersprungen", "Skipped")
    };
}
