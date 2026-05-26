using System.Windows;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Wpf.ViewModels;

namespace Sasd.HealthNotebook.Wpf;

/// <summary>
/// Main application window.
/// </summary>
public partial class MainWindow : Window
{
    private readonly HealthTopicService _healthTopicService;
    private readonly MainWindowViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow" /> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();

        _healthTopicService = Bootstrapper.CreateHealthTopicService();
        _viewModel = new MainWindowViewModel(_healthTopicService);
        DataContext = _viewModel;

        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await ReloadSafeAsync();
    }

    private async void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        await ReloadSafeAsync();
    }

    private async void NewHealthTopicButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new CreateHealthTopicWizardWindow(_healthTopicService)
        {
            Owner = this
        };

        bool? result = dialog.ShowDialog();

        if (result == true)
        {
            await ReloadSafeAsync();
        }
    }

    private async Task ReloadSafeAsync()
    {
        try
        {
            await _viewModel.LoadAsync();
        }
        catch
        {
            UiErrorHandler.ShowSafeError(this, "load local notebook data");
        }
    }
}
