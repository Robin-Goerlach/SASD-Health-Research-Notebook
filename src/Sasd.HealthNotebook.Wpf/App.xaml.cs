namespace Sasd.HealthNotebook.Wpf;

/// <summary>
/// Interaction logic for the WPF application.
/// </summary>
/// <remarks>
/// The fully qualified WPF base type avoids the name collision between
/// <c>System.Windows.Application</c> and the project's
/// <c>Sasd.HealthNotebook.Application</c> namespace.
/// </remarks>
public partial class App : System.Windows.Application
{
}
