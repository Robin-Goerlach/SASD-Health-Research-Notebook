using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.WinForms.Localization;

/// <summary>Source notebook localization; original and personal text are explicitly separated.</summary>
public static partial class AppStrings
{
    /// <summary>Navigation title.</summary>
    public static string Sources => Text("Sources", "Quellen");
    /// <summary>Page description.</summary>
    public static string SourcesDescription => Text("Sources, exact locations and your own notes. No automatic assessment.", "Quellen, konkrete Fundstellen und eigene Notizen. Keine automatische Bewertung.");
    /// <summary>Create source action.</summary>
    public static string NewSource => Text("New source", "Neue Quelle");
    /// <summary>Create location action.</summary>
    public static string NewSourceLocation => Text("New location", "Neue Fundstelle");
    /// <summary>Create note action.</summary>
    public static string NewSourceNote => Text("New source note", "Neue Quellen-Notiz");
    /// <summary>No sources yet.</summary>
    public static string SourcesEmpty => Text("No sources yet. Choose New source to document one.", "Noch keine Quellen. Mit Neue Quelle eine Quelle dokumentieren.");
    /// <summary>Locations section heading.</summary>
    public static string SourceLocations => Text("Locations", "Fundstellen");
    /// <summary>Notes section heading.</summary>
    public static string SourceNotes => Text("Source notes", "Quellen-Notizen");
    /// <summary>No selected source for dependent records.</summary>
    public static string SourceSelectionEmpty => Text("Select a source to view its locations and notes.", "Quelle auswählen, um ihre Fundstellen und Notizen anzuzeigen.");
    /// <summary>Selected source has no locations.</summary>
    public static string LocationsEmpty => Text("No locations for this source yet. Add one with New location.", "Noch keine Fundstellen für diese Quelle. Mit Neue Fundstelle eine Stelle erfassen.");
    /// <summary>Selected source has no notes.</summary>
    public static string NotesEmpty => Text("No notes for this source yet. Add one with New source note.", "Noch keine Notizen für diese Quelle. Mit Neue Quellen-Notiz eine Notiz erfassen.");
    /// <summary>Source title editor.</summary>
    public static string SourceTitle => Text("Source title", "Titel der Quelle");
    /// <summary>URL editor.</summary>
    public static string SourceUrl => Text("URL (optional)", "URL (optional)");
    /// <summary>Provider editor.</summary>
    public static string SourceAuthor => Text("Author / institution", "Autor / Institution");
    /// <summary>Publication calendar date.</summary>
    public static string SourcePublicationDate => Text("Publication date (optional)", "Publikationsdatum (optional)");
    /// <summary>Access calendar date.</summary>
    public static string SourceAccessedAt => Text("Access date (optional)", "Abrufdatum (optional)");
    /// <summary>External identifier.</summary>
    public static string SourceIdentifier => Text("External ID (optional)", "Externe ID (optional)");
    /// <summary>Metadata guidance.</summary>
    public static string SourceMetadataHelp => Text("Enter at least a title, URL or author/institution.\r\nURLs must start with http:// or https://; no automatic download.", "Mindestens Titel, URL oder Autor/Institution eingeben.\r\nURL mit http:// oder https://; kein automatischer Abruf.");
    /// <summary>Optional date checkbox help.</summary>
    public static string SourceDateHelp => Text("Check the box only if the calendar date is known.", "Häkchen nur setzen, wenn das Kalenderdatum bekannt ist.");
    /// <summary>Locator category.</summary>
    public static string LocationType => Text("Location type", "Typ der Fundstelle");
    /// <summary>Explicit locator editor.</summary>
    public static string SourceLocator => Text("Exact location", "Konkrete Fundstelle");
    /// <summary>Locator guidance.</summary>
    public static string SourceLocatorHelp => Text("For example: Page 17, Chapter 3, Paragraph 4 or #results. Up to 160 characters.", "Zum Beispiel: S. 17, Kapitel 3, Absatz 4 oder #results. Höchstens 160 Zeichen.");
    /// <summary>Optional location note.</summary>
    public static string LocationNote => Text("Location note (optional)", "Notiz zur Fundstelle (optional)");
    /// <summary>Optional locator selector.</summary>
    public static string NoteLocation => Text("Location (optional)", "Fundstelle (optional)");
    /// <summary>No locator option.</summary>
    public static string NoSourceLocation => Text("Whole source / no exact location", "Gesamte Quelle / keine Fundstelle");
    /// <summary>Claim field.</summary>
    public static string SourceStatement => Text("Documented claim / key statement", "Dokumentierte Aussage / Kernaussage");
    /// <summary>Original quotation field.</summary>
    public static string SourceExcerpt => Text("Original quote / excerpt (optional)", "Originalzitat / Exzerpt (optional)");
    /// <summary>Personal interpretation field.</summary>
    public static string SourceAssessment => Text("My own summary / assessment", "Meine eigene Zusammenfassung / Einordnung");
    /// <summary>Semantics guidance.</summary>
    public static string SourceSemantics => Text("A documented claim or quote is not confirmed by the app.\r\nKeep original wording separate from your own summary.", "Die App bestätigt dokumentierte Aussagen oder Zitate nicht.\r\nOriginalwortlaut und eigene Zusammenfassung getrennt halten.");
    /// <summary>Claim guidance.</summary>
    public static string SourceStatementHelp => Text("What does the source say? Required; up to 4000 characters.", "Was sagt die Quelle aus? Erforderlich; höchstens 4000 Zeichen.");
    /// <summary>Quote guidance.</summary>
    public static string SourceExcerptHelp => Text("Optional original wording, up to 1000 characters.\r\nDo not enter your own interpretation here.", "Optionaler Originalwortlaut, höchstens 1000 Zeichen.\r\nHier keine eigene Einordnung eingeben.");
    /// <summary>Personal summary guidance.</summary>
    public static string SourceAssessmentHelp => Text("Your own words, required; up to 4000 characters.\r\nThe app does not evaluate this text.", "Eigene Worte, erforderlich; höchstens 4000 Zeichen.\r\nDie App bewertet diesen Text nicht.");
    /// <summary>Validation feedback.</summary>
    public static string SourceValidationFailed => Text("Check required fields, text lengths, URL and selected references.", "Pflichtfelder, Textlängen, URL und gewählte Zuordnungen prüfen.");
    /// <summary>Save command.</summary>
    public static string SaveSourceRecord => Text("Save", "Speichern");
    /// <summary>Privacy-safe operation name.</summary>
    public static string OperationSaveSourceRecord => Text("save source documentation", "Quellendokumentation speichern");
    /// <summary>Loaded source count.</summary>
    public static string FormatLoadedSources(int count) => Text($"Loaded {count} sources.", $"{count} Quellen geladen.");
    /// <summary>Domain source category labels.</summary>
    public static string SourceTypeText(SourceType type) => type switch
    {
        SourceType.WebPage => Text("Web page", "Webseite"),
        SourceType.DocumentOrPdf => Text("Document / PDF", "Dokument / PDF"),
        SourceType.Book => Text("Book", "Buch"),
        SourceType.Study => Text("Study", "Studie"),
        SourceType.Conversation => Text("Conversation", "Gespräch"),
        SourceType.ProfessionalStatement => Text("Documented professional statement", "Dokumentierte professionelle Aussage"),
        SourceType.OwnObservation => Text("Own observation", "Eigene Beobachtung"),
        _ => throw new ArgumentOutOfRangeException(nameof(type))
    };
    /// <summary>Domain locator category labels.</summary>
    public static string SourceLocationTypeText(SourceLocationType type) => type switch
    {
        SourceLocationType.Page => Text("Page", "Seite"),
        SourceLocationType.Section => Text("Section", "Abschnitt"),
        SourceLocationType.Chapter => Text("Chapter", "Kapitel"),
        SourceLocationType.Paragraph => Text("Paragraph", "Absatz"),
        SourceLocationType.UrlAnchor => Text("URL anchor", "URL-Anker"),
        SourceLocationType.Other => Text("Other", "Sonstige"),
        _ => throw new ArgumentOutOfRangeException(nameof(type))
    };
    /// <summary>Uses existing identifying metadata for titleless sources, without inventing titles.</summary>
    public static string SourceDisplayName(Source source) => !string.IsNullOrWhiteSpace(source.Title)
        ? source.Title : source.AuthorOrInstitution ?? source.Url ?? string.Empty;
}
