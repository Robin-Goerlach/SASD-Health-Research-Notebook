using System.Windows.Forms;

namespace Sasd.HealthNotebook.WinForms;

/// <summary>
/// Shows privacy-safe error messages for the WinForms UI.
/// </summary>
public static class UiErrorHandler
{
    /// <summary>
    /// Shows a generic error message without exposing user-entered health details.
    /// </summary>
    /// <param name="owner">The owning window.</param>
    /// <param name="safeOperationName">A short technical operation description.</param>
    public static void ShowSafeError(IWin32Window owner, string safeOperationName)
    {
        MessageBox.Show(
            owner,
            $"The application could not {safeOperationName}. Please try again. No health details were written to logs.",
            "SASD Health Research Notebook",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }
}
