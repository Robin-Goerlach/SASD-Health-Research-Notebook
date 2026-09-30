namespace Sasd.HealthNotebook.Wpf;

/// <summary>
/// Interaction logic for the WPF application.
/// </summary>
/// <remarks>
/// This class intentionally uses the fully qualified type name
/// <see cref="System.Windows.Application" />.
///
/// The project contains a namespace named <c>Sasd.HealthNotebook.Application</c>
/// for the application layer. In a WPF project this can collide with the
/// WPF type name <c>Application</c>. Using the fully qualified type name keeps
/// the architectural namespace and avoids ambiguous compiler resolution.
/// </remarks>
public partial class App : System.Windows.Application
{
}
