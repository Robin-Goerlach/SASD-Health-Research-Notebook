using Sasd.HealthNotebook.Application.Services;

namespace Sasd.HealthNotebook.WinForms;

/// <summary>
/// Application entry point for the Windows Forms frontend.
/// </summary>
internal static class Program
{
    /// <summary>
    /// Starts the WinForms application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        // Keep the WinForms entry point explicit because this solution also contains
        // a namespace named Sasd.HealthNotebook.Application. Using fully qualified
        // framework types avoids the same kind of name collision that affected WPF.
        // The classic initialization calls are used because they are available in
        // the WinForms target profile used by the CI runner.
        System.Windows.Forms.Application.EnableVisualStyles();
        System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

        HealthTopicService healthTopicService = Bootstrapper.CreateHealthTopicService();

        using var mainForm = new Forms.MainForm(healthTopicService, Bootstrapper.CreateHealthEntryService(), Bootstrapper.CreateSourceService(), Bootstrapper.CreateMeasurementService(), Bootstrapper.CreateSessionService());
        System.Windows.Forms.Application.Run(mainForm);
    }
}
