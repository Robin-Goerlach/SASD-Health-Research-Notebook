using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Infrastructure.Storage;

namespace Sasd.HealthNotebook.Wpf;

/// <summary>
/// Very small composition root for Milestone 1.
///
/// The application intentionally avoids a dependency-injection package for now.
/// This class wires repositories and services in one visible place.
/// </summary>
public static class Bootstrapper
{
    /// <summary>
    /// Creates the health-topic service used by the UI.
    /// </summary>
    public static HealthTopicService CreateHealthTopicService()
    {
        var repository = new JsonHealthTopicRepository();
        return new HealthTopicService(repository);
    }
}
