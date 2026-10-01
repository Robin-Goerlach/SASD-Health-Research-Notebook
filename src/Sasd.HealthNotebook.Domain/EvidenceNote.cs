namespace Sasd.HealthNotebook.Domain;

/// <summary>A documented claim with provenance and separate personal interpretation, not proof of truth.</summary>
public sealed class EvidenceNote
{
    /// <summary>Maximum documented claim length.</summary>
    public const int MaximumStatementLength = 4000;
    /// <summary>Maximum short original excerpt length.</summary>
    public const int MaximumExcerptLength = 1000;
    /// <summary>Maximum personal summary length.</summary>
    public const int MaximumAssessmentLength = 4000;
    /// <summary>Stable identifier.</summary>
    public required Guid Id { get; init; }
    /// <summary>Required source reference.</summary>
    public required Guid SourceId { get; init; }
    /// <summary>Optional locator, which must belong to the referenced source.</summary>
    public Guid? SourceLocationId { get; init; }
    /// <summary>Documented external claim; must not be logged.</summary>
    public required string Statement { get; init; }
    /// <summary>Optional original excerpt, distinct from the user's own words.</summary>
    public string? Excerpt { get; init; }
    /// <summary>Required user's own summary or assessment; not an app-generated judgment.</summary>
    public required string OwnParaphraseOrAssessment { get; init; }
    /// <summary>Technical creation time.</summary>
    public required DateTimeOffset CreatedAt { get; init; }
    /// <summary>Technical modification time; create-only in this slice.</summary>
    public required DateTimeOffset ModifiedAt { get; init; }
    /// <summary>Creates a note with separate fields, without evaluating any claim.</summary>
    public static EvidenceNote Create(Guid sourceId, Guid? sourceLocationId, string statement,
        string? excerpt, string ownParaphraseOrAssessment)
    {
        var now = DateTimeOffset.Now;
        var note = new EvidenceNote { Id = Guid.NewGuid(), SourceId = sourceId, SourceLocationId = sourceLocationId,
            Statement = statement ?? string.Empty, Excerpt = excerpt,
            OwnParaphraseOrAssessment = ownParaphraseOrAssessment ?? string.Empty, CreatedAt = now, ModifiedAt = now };
        note.Validate();
        return note;
    }
    /// <summary>Checks text and metadata invariants without altering source quotations.</summary>
    public void Validate()
    {
        if (Id == Guid.Empty || SourceId == Guid.Empty || SourceLocationId == Guid.Empty || CreatedAt == default || ModifiedAt < CreatedAt
            || string.IsNullOrWhiteSpace(Statement) || Statement.Length > MaximumStatementLength
            || string.IsNullOrWhiteSpace(OwnParaphraseOrAssessment) || OwnParaphraseOrAssessment.Length > MaximumAssessmentLength
            || Excerpt?.Length > MaximumExcerptLength)
            throw new ArgumentException("Invalid source note; check required claim, personal summary and text lengths.");
    }
}
