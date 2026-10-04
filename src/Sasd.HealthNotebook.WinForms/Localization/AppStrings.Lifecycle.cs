namespace Sasd.HealthNotebook.WinForms.Localization;

public static partial class AppStrings
{
    private static string LifecycleText(string de, string en) => AppLanguage.Current == UiLanguage.German ? de : en;
    /// <summary>Edit command.</summary>
    public static string Edit => LifecycleText("Bearbeiten", "Edit");
    /// <summary>Archive command.</summary>
    public static string Archive => LifecycleText("Archivieren", "Archive");
    /// <summary>Reactivate command.</summary>
    public static string Reactivate => LifecycleText("Reaktivieren", "Reactivate");
    /// <summary>Delete command.</summary>
    public static string Delete => LifecycleText("Löschen", "Delete");
    /// <summary>Archive filter.</summary>
    public static string ShowArchived => LifecycleText("Archivierte anzeigen", "Show archived");
    /// <summary>Separate archive state label.</summary>
    public static string Archived => LifecycleText("Archiviert", "Archived");
    /// <summary>Save an edited record.</summary>
    public static string SaveChanges => LifecycleText("Änderungen speichern", "Save changes");
    /// <summary>Edit dialog title.</summary>
    public static string EditMeasurement => LifecycleText("Messwert bearbeiten", "Edit measurement");
    /// <summary>Edit dialog title.</summary>
    public static string EditEntry => LifecycleText("Verlaufseintrag bearbeiten", "Edit timeline entry");
    /// <summary>Edit dialog title.</summary>
    public static string EditSession => LifecycleText("Termin bearbeiten", "Edit session");
    /// <summary>Edit dialog title.</summary>
    public static string EditAction => LifecycleText("Maßnahme bearbeiten", "Edit action");
    /// <summary>Edit dialog title.</summary>
    public static string EditRoutine => LifecycleText("Routine bearbeiten", "Edit routine");
    /// <summary>Edit dialog title.</summary>
    public static string EditProgress => LifecycleText("Durchführung bearbeiten", "Edit execution");
    /// <summary>Compact history command.</summary>
    public static string RevisionCommand => LifecycleText("Historie", "History");
    /// <summary>Previous professional instruction snapshots.</summary>
    public static string Revisions => LifecycleText("Änderungshistorie", "Revision history");
    /// <summary>Optional correction explanation.</summary>
    public static string ChangeReason => LifecycleText("Änderungsgrund (optional)", "Change reason (optional)");
    /// <summary>No previous professional corrections.</summary>
    public static string NoRevisions => LifecycleText("Noch keine Änderungen an einer professionell dokumentierten Anweisung.", "No corrections to a documented professional instruction yet.");
    /// <summary>Stale editor guidance, without user content or IDs.</summary>
    public static string LifecycleConflict => LifecycleText("Dieser Datensatz wurde inzwischen geändert. Bitte schließen Sie den Dialog, aktualisieren Sie die Liste und versuchen Sie es erneut.", "This record has changed. Close the dialog, refresh the list and try again.");
    /// <summary>Concrete irreversible delete confirmation.</summary>
    public static string ConfirmDelete(string record) => LifecycleText(record + " wirklich löschen?\r\n\r\nDiese Aktion kann nicht rückgängig gemacht werden.", "Delete " + record + "?\r\n\r\nThis action cannot be undone.");
    /// <summary>Identifies the selected measurement in a delete confirmation.</summary>
    public static string DeleteMeasurementLabel(DateTimeOffset time) => LifecycleText("Messwert vom ", "measurement from ") + FormatDateTime(time);
    /// <summary>Identifies the selected timeline entry in a delete confirmation.</summary>
    public static string DeleteEntryLabel(string title) => LifecycleText("Verlaufseintrag „", "timeline entry “") + title + LifecycleText("“", "”");
    /// <summary>Identifies the selected execution in a delete confirmation.</summary>
    public static string DeleteProgressLabel(DateTimeOffset time) => LifecycleText("Durchführung vom ", "execution from ") + FormatDateTime(time);
}
