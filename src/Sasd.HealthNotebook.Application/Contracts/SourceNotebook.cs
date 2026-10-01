using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.Application.Contracts;

/// <summary>A coherent read snapshot of sources and their dependent records.</summary>
public sealed record SourceNotebook(IReadOnlyList<Source> Sources, IReadOnlyList<SourceLocation> Locations,
    IReadOnlyList<EvidenceNote> Notes);

/// <summary>Display projection; topic title is resolved at read time, never persisted twice.</summary>
public sealed record SourceSummary(Source Source, string? HealthTopicTitle);
