namespace Sasd.HealthNotebook.Domain;

/// <summary>Simple categories for user-entered source locators.</summary>
public enum SourceLocationType
{
    /// <summary>Page number or range.</summary>
    Page,
    /// <summary>Section heading.</summary>
    Section,
    /// <summary>Chapter.</summary>
    Chapter,
    /// <summary>Paragraph.</summary>
    Paragraph,
    /// <summary>URL fragment.</summary>
    UrlAnchor,
    /// <summary>Another explicit locator.</summary>
    Other
}
