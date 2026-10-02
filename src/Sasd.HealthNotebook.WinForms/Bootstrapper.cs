using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Infrastructure.Storage;

namespace Sasd.HealthNotebook.WinForms;

/// <summary>
/// Small composition root for the Windows Forms frontend.
/// </summary>
/// <remarks>
/// The project intentionally does not introduce a dependency-injection package yet.
/// Keeping the wiring in one visible place makes it easy to verify that WinForms and
/// WPF reuse the same Application and Infrastructure services.
/// </remarks>
public static class Bootstrapper
{
    /// <summary>Creates session documentation using the shared resolver.</summary>
    public static SessionService CreateSessionService() => new(new JsonSessionRepository(), new JsonHealthTopicRepository());
    /// <summary>Creates manual measurement documentation with the shared data resolver.</summary>
    public static MeasurementService CreateMeasurementService() => new(new JsonMeasurementRepository(), new JsonHealthTopicRepository());
    /// <summary>Creates source documentation with the shared path and topic contract.</summary>
    public static SourceService CreateSourceService() => new(new JsonSourceRepository(), new JsonHealthTopicRepository());
    /// <summary>Creates the entry service using the same topic repository and data resolver.</summary>
    public static HealthEntryService CreateHealthEntryService() =>
        new(new JsonHealthEntryRepository(), new JsonHealthTopicRepository());
    /// <summary>
    /// Creates the shared health-topic application service.
    /// </summary>
    public static HealthTopicService CreateHealthTopicService()
    {
        var repository = new JsonHealthTopicRepository();
        return new HealthTopicService(repository);
    }
}
