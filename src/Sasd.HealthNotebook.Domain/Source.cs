namespace Sasd.HealthNotebook.Domain;

/// <summary>Source metadata only; never evaluates truth or medical reliability.</summary>
public sealed class Source
{
    /// <summary>Maximum identifying title length.</summary>
    public const int MaximumTitleLength = 160;
    /// <summary>Maximum URL length.</summary>
    public const int MaximumUrlLength = 2048;
    /// <summary>Maximum author/institution length.</summary>
    public const int MaximumAuthorLength = 160;
    /// <summary>Maximum external identifier length.</summary>
    public const int MaximumIdentifierLength = 160;
    /// <summary>Stable identifier.</summary>
    public required Guid Id { get; init; }
    /// <summary>User-selected origin.</summary>
    public required SourceType SourceType { get; init; }
    /// <summary>Optional title; URL or provider may identify a source instead.</summary>
    public required string Title { get; init; }
    /// <summary>Optional absolute HTTP(S) URL; the app does not fetch it.</summary>
    public string? Url { get; init; }
    /// <summary>Optional author or institution.</summary>
    public string? AuthorOrInstitution { get; init; }
    /// <summary>Publication calendar date, independent of a time zone.</summary>
    public DateOnly? PublicationDate { get; init; }
    /// <summary>Optional documented access date, independent of a time zone.</summary>
    public DateOnly? AccessedAt { get; init; }
    /// <summary>Optional DOI, ISBN or other identifier; no automatic lookup.</summary>
    public string? ExternalIdentifier { get; init; }
    /// <summary>Optional single health-topic reference.</summary>
    public Guid? HealthTopicId { get; init; }
    /// <summary>Technical creation time.</summary>
    public required DateTimeOffset CreatedAt { get; init; }
    /// <summary>Technical modification time; create-only in this slice.</summary>
    public required DateTimeOffset ModifiedAt { get; init; }

    /// <summary>Creates metadata without truncating input.</summary>
    public static Source Create(SourceType type, string? title, string? url = null, string? authorOrInstitution = null,
        DateOnly? publicationDate = null, DateOnly? accessedAt = null, string? externalIdentifier = null, Guid? healthTopicId = null)
    {
        var now = DateTimeOffset.Now;
        var source = new Source { Id = Guid.NewGuid(), SourceType = type, Title = title?.Trim() ?? string.Empty,
            Url = Normalize(url), AuthorOrInstitution = Normalize(authorOrInstitution), PublicationDate = publicationDate,
            AccessedAt = accessedAt, ExternalIdentifier = Normalize(externalIdentifier), HealthTopicId = healthTopicId,
            CreatedAt = now, ModifiedAt = now };
        source.Validate();
        return source;
    }

    /// <summary>Checks metadata including deserialized values, without echoing user content.</summary>
    public void Validate()
    {
        if (Title is null || Title.Length > MaximumTitleLength || Url?.Length > MaximumUrlLength
            || AuthorOrInstitution?.Length > MaximumAuthorLength || ExternalIdentifier?.Length > MaximumIdentifierLength
            || (string.IsNullOrWhiteSpace(Title) && string.IsNullOrWhiteSpace(Url) && string.IsNullOrWhiteSpace(AuthorOrInstitution)))
            throw new ArgumentException("A source needs a title, URL or author/institution within the allowed lengths.");
        if (Url is not null && (!Uri.TryCreate(Url, UriKind.Absolute, out var uri)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps) || string.IsNullOrEmpty(uri.Host)))
            throw new ArgumentException("Source URL must be an absolute HTTP or HTTPS address.");
        if (Id == Guid.Empty || HealthTopicId == Guid.Empty || !Enum.IsDefined(SourceType)
            || CreatedAt == default || ModifiedAt < CreatedAt)
            throw new ArgumentException("Invalid source metadata.");
    }
    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
