using System.Globalization;

namespace Sasd.HealthNotebook.WinForms.Localization;

/// <summary>
/// Supported user-interface languages for the first WinForms baseline.
/// </summary>
public enum UiLanguage
{
    /// <summary>English user-interface texts.</summary>
    English,

    /// <summary>German user-interface texts.</summary>
    German
}

/// <summary>
/// Provides the currently selected UI language.
/// </summary>
/// <remarks>
/// The application starts with a small, local-only language decision:
/// German is used on German Windows installations, otherwise English is used.
/// A later settings screen can persist the explicit choice without changing the
/// public surface of this class.
/// </remarks>
public static class AppLanguage
{
    private static UiLanguage? _selectedLanguage;

    /// <summary>
    /// Gets the active UI language.
    /// </summary>
    public static UiLanguage Current => _selectedLanguage ?? DetectFromOperatingSystemCulture();

    /// <summary>
    /// Selects the active UI language for the current process.
    /// </summary>
    public static void SetLanguage(UiLanguage language)
    {
        _selectedLanguage = language;
    }

    private static UiLanguage DetectFromOperatingSystemCulture()
    {
        string twoLetterName = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

        return string.Equals(twoLetterName, "de", StringComparison.OrdinalIgnoreCase)
            ? UiLanguage.German
            : UiLanguage.English;
    }
}
