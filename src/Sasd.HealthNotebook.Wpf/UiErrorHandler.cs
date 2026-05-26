using System.Windows;

namespace Sasd.HealthNotebook.Wpf;

/// <summary>
/// Central helper for showing safe UI error messages.
///
/// This class deliberately avoids showing exception details because exception text
/// could include file paths or other technical details. User-entered health data
/// must never be copied into logs or detailed crash reports by default.
/// </summary>
public static class UiErrorHandler
{
    /// <summary>
    /// Shows a generic error message that is safe for sensitive health-data workflows.
    /// </summary>
    public static void ShowSafeError(Window owner, string actionDescription)
    {
        MessageBox.Show(
            owner,
            $"The action could not be completed: {actionDescription}. Please check the input and try again.",
            "SASD Health Research Notebook",
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
    }
}
