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
    /// <summary>
    /// Creates the shared health-topic application service.
    /// </summary>
    public static HealthTopicService CreateHealthTopicService()
    {
        var repository = new JsonHealthTopicRepository();
        return new HealthTopicService(repository);
    }
}
