namespace Sasd.HealthNotebook.WinForms.Localization;

/// <summary>Localized feedback for topics and session-child lifecycle operations.</summary>
public static partial class AppStrings
{
    /// <summary>Topic correction title.</summary>
    public static string EditTopic => Text("Edit health topic", "Gesundheitsthema bearbeiten");
    /// <summary>Topic editor guidance.</summary>
    public static string TopicEditHelp => Text("Correct your documentation. Linked entries remain unchanged.", "Eigene Dokumentation korrigieren. Verknüpfte Einträge bleiben erhalten.");
    /// <summary>Bounded editor validation guidance.</summary>
    public static string TopicEditValidation => Text("Check title, status, priority and text lengths.", "Titel, Status, Priorität und Textlängen prüfen.");
    /// <summary>Question correction title.</summary>
    public static string EditQuestion => Text("Edit question", "Frage bearbeiten");
    /// <summary>Follow-up correction title.</summary>
    public static string EditFollowUp => Text("Edit follow-up", "Nächsten Schritt bearbeiten");
    /// <summary>Blocked topic deletion explanation without technical identifiers.</summary>
    public static string TopicDeleteBlocked => Text("This health topic cannot be deleted because linked entries or revision history still exist. You can archive it.", "Dieses Gesundheitsthema kann nicht gelöscht werden, weil verknüpfte Einträge oder Änderungshistorie bestehen. Sie können es archivieren.");
    /// <summary>Answered questions must not silently lose documented answers.</summary>
    public static string QuestionDeleteBlocked => Text("This question cannot be deleted because an answer has been documented. You can edit it.", "Diese Frage kann nicht gelöscht werden, weil eine Antwort dokumentiert wurde. Sie können sie bearbeiten.");
    /// <summary>Concrete delete confirmation label.</summary>
    public static string DeleteTopicLabel(string title) => Text("Delete health topic: ", "Gesundheitsthema löschen: ") + title;
    /// <summary>Concrete question delete confirmation label.</summary>
    public static string DeleteQuestionLabel => Text("Delete selected unanswered question", "Ausgewählte unbeantwortete Frage löschen");
    /// <summary>Concrete next-step delete confirmation label.</summary>
    public static string DeleteFollowUpLabel => Text("Delete selected follow-up", "Ausgewählten nächsten Schritt löschen");
}
