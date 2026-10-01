using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.Application.Contracts;

/// <summary>User-entered locator within one source.</summary>
public sealed class CreateSourceLocationRequest
{
    /// <summary>Existing source.</summary>
    public Guid SourceId { get; init; }
    /// <summary>Locator category.</summary>
    public SourceLocationType LocationType { get; init; }
    /// <summary>Required locator text.</summary>
    public string Locator { get; init; } = string.Empty;
    /// <summary>Optional personal note.</summary>
    public string? Note { get; init; }
}
