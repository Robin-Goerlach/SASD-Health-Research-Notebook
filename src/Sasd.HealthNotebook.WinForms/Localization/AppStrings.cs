using System.Globalization;
using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.WinForms.Localization;

/// <summary>
/// Centralized user-interface strings for the WinForms frontend.
/// </summary>
/// <remarks>
/// The first WinForms baseline deliberately keeps localization simple and explicit.
/// This avoids scattered hard-coded labels and prepares the UI for additional
/// languages without introducing resource-generation complexity too early.
/// </remarks>
public static partial class AppStrings
{
    /// <summary>Timeline navigation title.</summary>
    public static string Timeline => Text("Timeline", "Verlauf");
    /// <summary>Timeline main action.</summary>
    public static string NewTimelineEntry => Text("New timeline entry", "Neuer Verlaufseintrag");
    /// <summary>Timeline explanatory text.</summary>
    public static string TimelineDescription => Text("Your notes, observations and research, newest event first.", "Ihre Notizen, Beobachtungen und Recherchen, neueste Ereignisse zuerst.");
    /// <summary>Empty timeline hint.</summary>
    public static string TimelineEmpty => Text("No entries yet. Choose New timeline entry to add a note.", "Noch keine Einträge. Mit Neuer Verlaufseintrag eine Notiz erfassen.");
    /// <summary>Documented event time label.</summary>
    public static string EntryTime => Text("When", "Zeitpunkt");
    /// <summary>Date editor label.</summary>
    public static string EntryDate => Text("Date", "Datum");
    /// <summary>Time editor label.</summary>
    public static string EntryClock => Text("Time", "Uhrzeit");
    /// <summary>Category editor label.</summary>
    public static string EntryType => Text("Type", "Typ");
    /// <summary>Optional topic label.</summary>
    public static string EntryTopic => Text("Health topic (optional)", "Gesundheitsthema (optional)");
    /// <summary>No topic choice.</summary>
    public static string NoEntryTopic => Text("No health topic", "Kein Gesundheitsthema");
    /// <summary>Unresolved stored topic reference.</summary>
    public static string MissingEntryTopic => Text("Health topic unavailable", "Gesundheitsthema nicht verfügbar");
    /// <summary>Specific entry title field label.</summary>
    public static string EntryTitle => Text("Timeline entry title", "Titel des Verlaufseintrags");
    /// <summary>Content editor label.</summary>
    public static string EntryContent => Text("Text", "Text");
    /// <summary>Save command.</summary>
    public static string SaveEntry => Text("Save entry", "Eintrag speichern");
    /// <summary>Entry title guidance.</summary>
    public static string EntryTitleHelp => Text("Required title, up to 160 characters.", "Erforderlicher Titel, höchstens 160 Zeichen.");
    /// <summary>Content guidance.</summary>
    public static string EntryContentHelp => Text("Optional text, up to 4000 characters.\r\nStored without medical assessment.", "Optionaler Text, höchstens 4000 Zeichen.\r\nSpeicherung ohne medizinische Bewertung.");
    /// <summary>Event date/time guidance.</summary>
    public static string EntryTimeHelp => Text("When did it happen? Enter local date and time.", "Wann war das Ereignis? Lokales Datum und Uhrzeit eingeben.");
    /// <summary>Topic choice guidance.</summary>
    public static string EntryTopicHelp => Text("Choose an existing topic or leave the entry unassigned.", "Vorhandenes Thema wählen oder den Eintrag ohne Zuordnung speichern.");
    /// <summary>Input validation guidance, without echoed user data.</summary>
    public static string EntryValidationFailed => Text("Check title, text length and the selected topic. For clock changes, choose an unambiguous local time.", "Titel, Textlänge und gewähltes Thema prüfen. Bei Zeitumstellungen eine eindeutige lokale Uhrzeit wählen.");
    /// <summary>Privacy-safe save operation.</summary>
    public static string OperationSaveEntry => Text("save timeline entry", "Verlaufseintrag speichern");
    /// <summary>Loaded entry count.</summary>
    public static string FormatLoadedEntries(int count) => Text($"Loaded {count} timeline entries.", $"{count} Verlaufseinträge geladen.");
    /// <summary>Localized domain category label.</summary>
    public static string HealthEntryTypeText(HealthEntryType type) => type switch
    {
        HealthEntryType.Note => Text("Note", "Notiz"),
        HealthEntryType.Observation => Text("Observation", "Beobachtung"),
        HealthEntryType.Research => Text("Research", "Recherche"),
        _ => throw new ArgumentOutOfRangeException(nameof(type))
    };
    /// <summary>Gets the display name of the application.</summary>
    public static string AppTitle => Text("SASD Health Research Notebook", "SASD Health Research Notebook");

    /// <summary>Gets the display name for German.</summary>
    public static string LanguageGerman => "Deutsch";

    /// <summary>Gets the display name for English.</summary>
    public static string LanguageEnglish => "English";

    /// <summary>Gets the language selector label.</summary>
    public static string LanguageLabel => Text("Language", "Sprache");

    /// <summary>Gets the dashboard page title.</summary>
    public static string Dashboard => Text("Dashboard", "Dashboard");

    /// <summary>Gets the health topics page title.</summary>
    public static string HealthTopics => Text("Health topics", "Gesundheitsthemen");

    /// <summary>Gets the all-health-topics title.</summary>
    public static string AllHealthTopics => Text("All health topics", "Alle Gesundheitsthemen");

    /// <summary>Gets the refresh button text.</summary>
    public static string Refresh => Text("Refresh", "Aktualisieren");

    /// <summary>Gets the new-health-topic action text.</summary>
    public static string NewHealthTopic => Text("New Health Topic", "Neues Gesundheitsthema");

    /// <summary>Gets the dashboard page description.</summary>
    public static string DashboardDescription => Text(
        "Local-first overview of documented health topics.",
        "Lokaler Überblick über dokumentierte Gesundheitsthemen.");

    /// <summary>Gets the health-topic page description.</summary>
    public static string HealthTopicsDescription => Text(
        "List of locally stored documentation topics.",
        "Liste lokal gespeicherter Dokumentationsthemen.");

    /// <summary>Gets the compact dashboard explanation.</summary>
    public static string DashboardNotebookBoundary => Text(
        "Personal local-first documentation workspace. This is a notebook, not a diagnosis or therapy system.",
        "Persönlicher lokaler Dokumentationsarbeitsplatz. Dies ist ein Notizbuch, kein Diagnose- oder Therapiesystem.");

    /// <summary>Gets the sidebar footer text.</summary>
    public static string SidebarFooter => Text("Local-first\r\nNo cloud transfer", "Lokal zuerst\r\nKeine Cloud-Übertragung");

    /// <summary>Gets the dashboard card title for all health topics.</summary>
    public static string CardHealthTopicsTitle => Text("Health topics", "Gesundheitsthemen");

    /// <summary>Gets the dashboard card description for all health topics.</summary>
    public static string CardHealthTopicsDescription => Text(
        "Documented topics in the local notebook",
        "Dokumentierte Themen im lokalen Notizbuch");

    /// <summary>Gets the dashboard card title for doctor preparation.</summary>
    public static string CardPrepareForDoctorTitle => Text("Prepare for doctor", "Für Arzttermin vorbereiten");

    /// <summary>Gets the dashboard card description for doctor preparation.</summary>
    public static string CardPrepareForDoctorDescription => Text(
        "Topics marked for appointment preparation",
        "Themen zur Vorbereitung eines Termins");

    /// <summary>Gets the dashboard card title for archived topics.</summary>
    public static string CardArchivedTitle => Text("Archived", "Archiviert");

    /// <summary>Gets the dashboard card description for archived topics.</summary>
    public static string CardArchivedDescription => Text(
        "Topics no longer shown as active",
        "Nicht mehr als aktiv angezeigte Themen");

    /// <summary>Gets the grid column title for the topic title.</summary>
    public static string ColumnTitle => Text("Title", "Titel");

    /// <summary>Gets the grid column title for the status.</summary>
    public static string ColumnStatus => Text("Status", "Status");

    /// <summary>Gets the grid column title for priority.</summary>
    public static string ColumnPriority => Text("Priority", "Priorität");

    /// <summary>Gets the grid column title for the creation date.</summary>
    public static string ColumnCreated => Text("Created", "Angelegt");

    /// <summary>Gets the grid column title for the short description.</summary>
    public static string ColumnShortDescription => Text("Short description", "Kurzbeschreibung");

    /// <summary>Gets the empty health-topic list message.</summary>
    public static string EmptyHealthTopics => Text(
        "No health topics yet. Use \"New Health Topic\" to create the first topic.",
        "Noch keine Gesundheitsthemen vorhanden. Lege über „Neues Gesundheitsthema“ das erste Thema an.");

    /// <summary>Gets the ready status text.</summary>
    public static string Ready => Text("Ready.", "Bereit.");

    /// <summary>Gets a privacy-safe load failure status.</summary>
    public static string LoadingFailed => Text("Loading failed. See message.", "Laden fehlgeschlagen. Siehe Meldung.");

    /// <summary>Gets a privacy-safe save failure status.</summary>
    public static string SavingFailed => Text("Saving failed. See message.", "Speichern fehlgeschlagen. Siehe Meldung.");

    /// <summary>Gets the cancel button text.</summary>
    public static string Cancel => Text("Cancel", "Abbrechen");

    /// <summary>Gets the back button text.</summary>
    public static string Back => Text("Back", "Zurück");

    /// <summary>Gets the next button text.</summary>
    public static string Next => Text("Next", "Weiter");

    /// <summary>Gets the create button text.</summary>
    public static string Create => Text("Create", "Anlegen");

    /// <summary>Gets the title label in the wizard.</summary>
    public static string FieldTitle => Text("Health topic title", "Titel des Gesundheitsthemas");

    /// <summary>Gets the status label in the wizard.</summary>
    public static string FieldStatus => Text("Documentation status", "Dokumentationsstatus");

    /// <summary>Gets the priority label in the wizard.</summary>
    public static string FieldPriority => Text("Personal priority", "Persönliche Priorität");

    /// <summary>Gets the short-description label in the wizard.</summary>
    public static string FieldShortDescription => Text("Short description", "Kurzbeschreibung");

    /// <summary>Gets the notes label in the wizard.</summary>
    public static string FieldNotes => Text("First notes", "Erste Notizen");

    /// <summary>Gets the longer explanation for the basic-data wizard step.</summary>
    public static string BasicDataStepDescription => Text(
        "Basic data are the small identifying details that help you find this health topic later: a clear title, documentation status, personal priority, short description and optional first notes.",
        "Grunddaten sind die wenigen Angaben, mit denen du dieses Gesundheitsthema später eindeutig wiederfindest: klarer Titel, Dokumentationsstatus, persönliche Priorität, Kurzbeschreibung und optionale erste Notizen.");

    /// <summary>Gets the tooltip for the health-topic title field.</summary>
    public static string ToolTipHealthTopicTitle => Text(
        Lines(
            "A short, recognizable name for this health topic.",
            "Example: \"Blood pressure observations\".",
            "Put sensitive details into notes rather than into the title,",
            "because titles are visible in lists."),
        Lines(
            "Kurzer, wiedererkennbarer Name für dieses Gesundheitsthema.",
            "Beispiel: „Blutdruck-Beobachtung“.",
            "Sensible Details gehören eher in die Notizen,",
            "weil Titel in Listen sichtbar sind."));

    /// <summary>Gets the tooltip for the documentation status field.</summary>
    public static string ToolTipStatus => Text(
        Lines(
            "How you want to document the current status.",
            "The app does not confirm or create a diagnosis."),
        Lines(
            "Wie du den aktuellen Stand dokumentieren möchtest.",
            "Die App bestätigt oder erstellt keine Diagnose."));

    /// <summary>Gets the tooltip for the priority field.</summary>
    public static string ToolTipPriority => Text(
        Lines(
            "Personal organization priority for your notebook.",
            "This is not medical urgency and not triage."),
        Lines(
            "Persönliche Organisationspriorität für dein Notizbuch.",
            "Das ist keine medizinische Dringlichkeit und keine Triage."));

    /// <summary>Gets the tooltip for the short-description field.</summary>
    public static string ToolTipShortDescription => Text(
        Lines(
            "One or two neutral sentences for the overview list.",
            "Details, open questions and context can go into notes."),
        Lines(
            "Ein bis zwei neutrale Sätze für die Übersichtsliste.",
            "Details, offene Fragen und Kontext können in die Notizen."));

    /// <summary>Gets the tooltip for the notes field.</summary>
    public static string ToolTipNotes => Text(
        Lines(
            "Optional first notes, context or questions.",
            "These notes are saved as documentation only.",
            "They are not used to generate medical recommendations."),
        Lines(
            "Optionale erste Notizen, Kontext oder Fragen.",
            "Diese Notizen werden nur dokumentiert.",
            "Sie werden nicht für medizinische Empfehlungen verwendet."));

    /// <summary>Gets the placeholder text for later wizard steps.</summary>
    public static string WizardPlaceholder => Text(
        "This step is part of the long-term wizard design. In the first WinForms baseline, only Basic data are stored.",
        "Dieser Schritt gehört zum langfristigen Wizard-Zielbild. In der ersten WinForms-Basis werden nur Grunddaten gespeichert.");

    /// <summary>Gets the validation message for a missing title.</summary>
    public static string MissingTitleMessage => Text(
        "Please enter a title for the health topic.",
        "Bitte gib einen Titel für das Gesundheitsthema ein.");

    /// <summary>Gets a privacy-safe operation name for loading local notebook data.</summary>
    public static string OperationLoadLocalNotebookData => Text("load local notebook data", "lokale Notizbuchdaten laden");

    /// <summary>Gets a privacy-safe operation name for creating a health topic.</summary>
    public static string OperationCreateHealthTopic => Text("create health topic", "ein Gesundheitsthema anlegen");

    /// <summary>
    /// Formats the loaded-topic status text.
    /// </summary>
    public static string FormatLoadedHealthTopics(int topicCount)
    {
        return AppLanguage.Current == UiLanguage.German
            ? $"{topicCount} Gesundheitsthema/-themen geladen."
            : $"Loaded {topicCount} health topic(s).";
    }

    /// <summary>
    /// Formats a privacy-safe error message for a failed operation.
    /// </summary>
    public static string FormatSafeError(string safeOperationName)
    {
        return AppLanguage.Current == UiLanguage.German
            ? $"Die Anwendung konnte {safeOperationName} nicht. Bitte erneut versuchen. Es wurden keine Gesundheitsdetails in Logs geschrieben."
            : $"The application could not {safeOperationName}. Please try again. No health details were written to logs.";
    }

    /// <summary>
    /// Returns a localized status label for the supplied domain value.
    /// </summary>
    public static string HealthTopicStatusText(HealthTopicStatus status)
    {
        if (AppLanguage.Current == UiLanguage.German)
        {
            return status switch
            {
                HealthTopicStatus.Observation => "Beobachtung",
                HealthTopicStatus.Suspected => "Vermutet",
                HealthTopicStatus.DoctorReported => "Ärztlich berichtet",
                HealthTopicStatus.Historical => "Historisch",
                HealthTopicStatus.Archived => "Archiviert",
                _ => status.ToString()
            };
        }

        return status switch
        {
            HealthTopicStatus.Observation => "Observation",
            HealthTopicStatus.Suspected => "Suspected",
            HealthTopicStatus.DoctorReported => "Doctor-reported",
            HealthTopicStatus.Historical => "Historical",
            HealthTopicStatus.Archived => "Archived",
            _ => status.ToString()
        };
    }

    /// <summary>
    /// Returns a localized priority label for the supplied domain value.
    /// </summary>
    public static string HealthTopicPriorityText(HealthTopicPriority priority)
    {
        if (AppLanguage.Current == UiLanguage.German)
        {
            return priority switch
            {
                HealthTopicPriority.Normal => "Normal",
                HealthTopicPriority.Watch => "Beobachten",
                HealthTopicPriority.PrepareForDoctor => "Für Arzttermin vorbereiten",
                HealthTopicPriority.OrganizationallyUrgent => "Organisatorisch dringend",
                _ => priority.ToString()
            };
        }

        return priority switch
        {
            HealthTopicPriority.Normal => "Normal",
            HealthTopicPriority.Watch => "Watch",
            HealthTopicPriority.PrepareForDoctor => "Prepare for doctor",
            HealthTopicPriority.OrganizationallyUrgent => "Organizationally urgent",
            _ => priority.ToString()
        };
    }

    /// <summary>
    /// Formats a date for the active UI culture.
    /// </summary>
    public static string FormatDateTime(DateTimeOffset dateTimeOffset)
    {
        CultureInfo culture = AppLanguage.Current == UiLanguage.German
            ? CultureInfo.GetCultureInfo("de-DE")
            : CultureInfo.GetCultureInfo("en-US");

        return dateTimeOffset.LocalDateTime.ToString("g", culture);
    }

    /// <summary>
    /// Creates the visible wizard step list for the active language.
    /// </summary>
    public static IReadOnlyList<WizardStepText> CreateWizardSteps()
    {
        if (AppLanguage.Current == UiLanguage.German)
        {
            return new[]
            {
                new WizardStepText("1. Grunddaten", BasicDataStepDescription),
                new WizardStepText("2. Diagnose / Status", "Später: externe Diagnoseinformationen oder offenen Status dokumentieren, ohne App-Diagnose."),
                new WizardStepText("3. Symptome", "Später: Symptome und Beobachtungen dokumentieren."),
                new WizardStepText("4. Dokumente", "Später: Briefe, PDFs, Laborberichte und Bilder verbinden."),
                new WizardStepText("5. Quellen & Informationen", "Später: Quellen und persönliche Recherchenotizen sammeln."),
                new WizardStepText("6. Ärzte / Kontakte", "Später: Ärzte, Praxen, Kliniken und Kontaktpersonen verbinden."),
                new WizardStepText("7. Medikamente / Maßnahmen", "Später: extern berichtete oder verordnete Informationen dokumentieren, nicht als App-Empfehlung."),
                new WizardStepText("8. Messwerte / Laborwerte", "Später: Werte und Einheiten dokumentieren."),
                new WizardStepText("9. Offene Fragen", "Später: Fragen für medizinische Termine sammeln."),
                new WizardStepText("10. Zusammenfassung", "Eingegebene Informationen vor dem Speichern prüfen.")
            };
        }

        return new[]
        {
            new WizardStepText("1. Basic data", BasicDataStepDescription),
            new WizardStepText("2. Diagnosis / status", "Later: document external diagnosis information or open status without app-generated diagnosis."),
            new WizardStepText("3. Symptoms", "Later: document symptoms and observations."),
            new WizardStepText("4. Documents", "Later: connect letters, PDFs, lab reports and images."),
            new WizardStepText("5. Sources & information", "Later: collect reliable sources and personal research notes."),
            new WizardStepText("6. Doctors / contacts", "Later: connect doctors, clinics and contact persons."),
            new WizardStepText("7. Medication / measures", "Later: document what was reported or prescribed elsewhere, not as an app recommendation."),
            new WizardStepText("8. Measurements / lab values", "Later: document values and units."),
            new WizardStepText("9. Open questions", "Later: collect questions for medical appointments."),
            new WizardStepText("10. Summary", "Review the entered information before saving.")
        };
    }

    private static string Text(string english, string german)
    {
        return AppLanguage.Current == UiLanguage.German ? german : english;
    }

    private static string Lines(params string[] lines)
    {
        return string.Join("\r\n", lines);
    }
}

/// <summary>
/// Localized title and explanation for one wizard step.
/// </summary>
public sealed record WizardStepText(string Title, string Description);
