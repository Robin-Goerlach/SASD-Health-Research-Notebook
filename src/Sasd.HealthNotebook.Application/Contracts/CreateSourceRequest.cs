using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.Application.Contracts;

/// <summary>User-entered source metadata.</summary>
public sealed class CreateSourceRequest
{
    /// <summary>Origin category, not trust.</summary>
    public SourceType SourceType { get; init; }
    /// <summary>Optional title.</summary>
    public string Title { get; init; } = string.Empty;
    /// <summary>Optional HTTP(S) URL.</summary>
    public string? Url { get; init; }
    /// <summary>Optional author or institution.</summary>
    public string? AuthorOrInstitution { get; init; }
    /// <summary>Optional publication date.</summary>
    public DateOnly? PublicationDate { get; init; }
    /// <summary>Optional access date.</summary>
    public DateOnly? AccessedAt { get; init; }
    /// <summary>Optional DOI, ISBN or another identifier.</summary>
    public string? ExternalIdentifier { get; init; }
    /// <summary>Optional existing health topic.</summary>
    public Guid? HealthTopicId { get; init; }
}
