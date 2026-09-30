namespace Sasd.HealthNotebook.Infrastructure.Storage;

/// <summary>
/// Provides local filesystem paths for the application.
///
/// This class keeps path decisions in one place so that later backup, export and
/// migration features do not have to duplicate path-building logic.
/// </summary>
public static class LocalHealthNotebookPaths
{
    /// <summary>
    /// Gets the application data directory under the user's local profile.
    /// </summary>
    public static string ApplicationDataDirectory
    {
        get
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(localAppData, "SASD-GmbH", "HealthResearchNotebook");
        }
    }

    /// <summary>
    /// Gets the default data directory used by the JSON repository.
    /// </summary>
    public static string DataDirectory => Path.Combine(ApplicationDataDirectory, "data");

    /// <summary>
    /// Gets the default JSON file path for health topics.
    /// </summary>
    public static string HealthTopicsFilePath => Path.Combine(DataDirectory, "health-topics.json");
}
