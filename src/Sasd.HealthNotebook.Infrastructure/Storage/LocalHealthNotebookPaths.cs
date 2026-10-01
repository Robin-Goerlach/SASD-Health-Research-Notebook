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
    /// Process configuration for an explicit data directory (not a JSON file).
    /// </summary>
    public const string DataPathEnvironmentVariable = "SASD_HEALTHNOTEBOOK_DATA_PATH";

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
    /// Gets the configured data directory, or the unchanged product default.
    /// A configured value must be fully qualified; invalid values never fall back
    /// to personal data. Resolution itself does not create or migrate files.
    /// </summary>
    public static string DataDirectory
    {
        get
        {
            string? configuredPath = Environment.GetEnvironmentVariable(DataPathEnvironmentVariable);
            if (configuredPath is null)
            {
                return Path.Combine(ApplicationDataDirectory, "data");
            }

            if (string.IsNullOrWhiteSpace(configuredPath) || !Path.IsPathFullyQualified(configuredPath))
            {
                throw new InvalidOperationException(
                    $"{DataPathEnvironmentVariable} must specify a fully qualified data directory.");
            }

            // Ordinary drive and UNC directories are supported. Device namespaces,
            // alternate streams and Win32-normalized aliases are not data folders.
            configuredPath = configuredPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            if (configuredPath.StartsWith(@"\\?\", StringComparison.Ordinal)
                || configuredPath.StartsWith(@"\\.\", StringComparison.Ordinal))
            {
                throw InvalidDataPath();
            }

            string normalizedPath;
            try { normalizedPath = Path.GetFullPath(configuredPath); }
            catch (ArgumentException) { throw InvalidDataPath(); }
            catch (NotSupportedException) { throw InvalidDataPath(); }

            string root = Path.GetPathRoot(configuredPath)!;
            if (root.StartsWith(@"\\", StringComparison.Ordinal))
            {
                string[] shareParts = root.Split('\\', StringSplitOptions.RemoveEmptyEntries);
                if (shareParts.Length != 2 || shareParts.Any(part => part.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0))
                {
                    throw InvalidDataPath();
                }
            }
            foreach (string component in configuredPath[root.Length..].Split(
                new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (component is "." or "..") { continue; }
                string stem = component.Split('.')[0].TrimEnd(' ');
                if (component.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0
                    || component.EndsWith(' ') || component.EndsWith('.')
                    || IsReservedWindowsName(stem))
                {
                    throw InvalidDataPath();
                }
            }
            if (File.Exists(normalizedPath))
            {
                throw InvalidDataPath();
            }
            return normalizedPath;
        }
    }

    private static InvalidOperationException InvalidDataPath() => new(
        $"{DataPathEnvironmentVariable} must specify a valid fully qualified data directory, not a file or device path.");

    private static bool IsReservedWindowsName(string name) =>
        name.Equals("CON", StringComparison.OrdinalIgnoreCase)
        || name.Equals("PRN", StringComparison.OrdinalIgnoreCase)
        || name.Equals("AUX", StringComparison.OrdinalIgnoreCase)
        || name.Equals("NUL", StringComparison.OrdinalIgnoreCase)
        || name.Equals("CONIN$", StringComparison.OrdinalIgnoreCase)
        || name.Equals("CONOUT$", StringComparison.OrdinalIgnoreCase)
        || (name.Length == 4
            && (name.StartsWith("COM", StringComparison.OrdinalIgnoreCase) || name.StartsWith("LPT", StringComparison.OrdinalIgnoreCase))
            && "123456789¹²³".Contains(name[3]));

    /// <summary>
    /// Gets the JSON file path under the resolved data directory.
    /// </summary>
    public static string HealthTopicsFilePath => Path.Combine(DataDirectory, "health-topics.json");

    /// <summary>Gets the separate entry store in the same resolved data directory.</summary>
    public static string HealthEntriesFilePath => Path.Combine(DataDirectory, "health-entries.json");

    /// <summary>Gets the source notebook store in the same resolved data directory.</summary>
    public static string SourcesFilePath => Path.Combine(DataDirectory, "sources.json");

    /// <summary>Gets the separate numeric measurement store in the resolved shared directory.</summary>
    public static string MeasurementsFilePath => Path.Combine(DataDirectory, "measurements.json");
}
