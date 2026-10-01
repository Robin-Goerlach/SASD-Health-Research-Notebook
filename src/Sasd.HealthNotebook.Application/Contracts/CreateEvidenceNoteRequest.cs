namespace Sasd.HealthNotebook.Application.Contracts;

/// <summary>User-entered claim, original excerpt and separate personal summary.</summary>
public sealed class CreateEvidenceNoteRequest
{
    /// <summary>Existing source.</summary>
    public Guid SourceId { get; init; }
    /// <summary>Optional locator within this source.</summary>
    public Guid? SourceLocationId { get; init; }
    /// <summary>Documented claim.</summary>
    public string Statement { get; init; } = string.Empty;
    /// <summary>Optional original excerpt.</summary>
    public string? Excerpt { get; init; }
    /// <summary>User's own required summary.</summary>
    public string OwnParaphraseOrAssessment { get; init; } = string.Empty;
}
