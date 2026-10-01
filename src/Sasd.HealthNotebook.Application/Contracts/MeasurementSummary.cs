using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.Application.Contracts;

/// <summary>Display projection of immutable numeric data with the currently resolved topic title.</summary>
public sealed record MeasurementSummary(Measurement Measurement, string? HealthTopicTitle);
