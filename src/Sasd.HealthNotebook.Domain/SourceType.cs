namespace Sasd.HealthNotebook.Domain;

/// <summary>User-selected origin category, not a reliability rating.</summary>
public enum SourceType
{
    /// <summary>A web page.</summary>
    WebPage,
    /// <summary>A document or PDF; no file import in this slice.</summary>
    DocumentOrPdf,
    /// <summary>A book.</summary>
    Book,
    /// <summary>A study.</summary>
    Study,
    /// <summary>A conversation.</summary>
    Conversation,
    /// <summary>A documented professional statement, not verified by the app.</summary>
    ProfessionalStatement,
    /// <summary>The user's own observation.</summary>
    OwnObservation
}
