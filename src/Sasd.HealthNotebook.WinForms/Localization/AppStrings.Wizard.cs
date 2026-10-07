namespace Sasd.HealthNotebook.WinForms.Localization;

public static partial class AppStrings
{
    /// <summary>Final create command.</summary>
    public static string WizardFinish => Text("Finish", "Fertigstellen");
    /// <summary>Review heading.</summary>
    public static string WizardReviewTitle => Text("4. Review", "4. Prüfen");
    /// <summary>Accessible validation region.</summary>
    public static string WizardValidationLabel => Text("Input guidance", "Eingabehinweis");
    /// <summary>Existing field length limits.</summary>
    public static string WizardTextLimits => Text("Use at most 160 characters for the title, 500 for the description and 4000 for notes.", "Verwende höchstens 160 Zeichen für den Titel, 500 für die Kurzbeschreibung und 4000 für Notizen.");
    /// <summary>Required explicit enum selections.</summary>
    public static string WizardChooseClassification => Text("Please select a status and a priority.", "Bitte wähle einen Status und eine Priorität.");
    /// <summary>Confirmation only when inputs differ from the initial defaults.</summary>
    public static string WizardDiscardMessage => Text("Discard your entries? The topic has not been saved.", "Angaben verwerfen? Das Thema wurde noch nicht gespeichert.");
}
