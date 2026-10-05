using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.WinForms.Views;

namespace Sasd.HealthNotebook.WinForms.Presentation;

/// <summary>Coordinates measurement loading through the shared Application service.</summary>
public sealed class MeasurementsPresenter
{
    private readonly MeasurementService _service;
    private readonly MeasurementsView _view;
    /// <summary>Connects service and view without persistence logic in the Form.</summary>
    public MeasurementsPresenter(MeasurementService service, MeasurementsView view)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _view = view ?? throw new ArgumentNullException(nameof(view));
    }
    /// <summary>Loads the complete list and returns its total count; the View preserves its display filter.</summary>
    public async Task<int> LoadAsync(CancellationToken cancellationToken = default)
    {
        var entries = await _service.GetMeasurementsAsync(cancellationToken: cancellationToken).ConfigureAwait(true);
        _view.SetMeasurements(entries);
        return entries.Count;
    }
}
