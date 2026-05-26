using System.Windows;
using Sasd.HealthNotebook.Application.Contracts;
using Sasd.HealthNotebook.Application.Services;
using Sasd.HealthNotebook.Domain;

namespace Sasd.HealthNotebook.Wpf;

/// <summary>
/// Simple wizard for creating a new health topic.
///
/// Milestone 1 stores only basic topic information. The step list already mirrors
/// the long-term product idea, so later milestones can replace placeholders with
/// real forms step by step.
/// </summary>
public partial class CreateHealthTopicWizardWindow : Window
{
    private readonly HealthTopicService _healthTopicService;
    private readonly List<WizardStep> _steps;
    private int _currentStepIndex;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateHealthTopicWizardWindow" /> class.
    /// </summary>
    public CreateHealthTopicWizardWindow(HealthTopicService healthTopicService)
    {
        InitializeComponent();

        _healthTopicService = healthTopicService ?? throw new ArgumentNullException(nameof(healthTopicService));
        _steps = CreateSteps();

        StatusComboBox.ItemsSource = Enum.GetValues<HealthTopicStatus>();
        StatusComboBox.SelectedItem = HealthTopicStatus.Observation;

        PriorityComboBox.ItemsSource = Enum.GetValues<HealthTopicPriority>();
        PriorityComboBox.SelectedItem = HealthTopicPriority.Normal;

        StepsListBox.ItemsSource = _steps.Select(step => step.Title).ToList();

        ShowStep(0);
    }

    private static List<WizardStep> CreateSteps()
    {
        return new List<WizardStep>
        {
            new("1. Basic data", "Create the main topic record with title, status, priority and first notes."),
            new("2. Diagnosis / status", "Later: document external diagnosis information or open status without app-generated diagnosis."),
            new("3. Symptoms", "Later: document symptoms and observations."),
            new("4. Documents", "Later: connect letters, PDFs, lab reports and images."),
            new("5. Sources & information", "Later: collect reliable sources and personal research notes."),
            new("6. Doctors / contacts", "Later: connect doctors, clinics and contact persons."),
            new("7. Medication / measures", "Later: document what was reported or prescribed elsewhere, not as an app recommendation."),
            new("8. Measurements / lab values", "Later: document values and units."),
            new("9. Open questions", "Later: collect questions for medical appointments."),
            new("10. Summary", "Review the entered information before saving.")
        };
    }

    private void ShowStep(int stepIndex)
    {
        _currentStepIndex = Math.Clamp(stepIndex, 0, _steps.Count - 1);

        WizardStep step = _steps[_currentStepIndex];
        StepTitleTextBlock.Text = step.Title;
        StepDescriptionTextBlock.Text = step.Description;
        StepsListBox.SelectedIndex = _currentStepIndex;

        BasicDataPanel.Visibility = _currentStepIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
        PlaceholderPanel.Visibility = _currentStepIndex == 0 ? Visibility.Collapsed : Visibility.Visible;

        BackButton.IsEnabled = _currentStepIndex > 0;
        NextButton.Content = _currentStepIndex == _steps.Count - 1 ? "Create" : "Next";
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        ShowStep(_currentStepIndex - 1);
    }

    private async void NextButton_Click(object sender, RoutedEventArgs e)
    {
        if (_currentStepIndex < _steps.Count - 1)
        {
            ShowStep(_currentStepIndex + 1);
            return;
        }

        await CreateTopicSafeAsync();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private async Task CreateTopicSafeAsync()
    {
        if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
        {
            MessageBox.Show(
                this,
                "Please enter a title for the health topic.",
                "SASD Health Research Notebook",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            ShowStep(0);
            TitleTextBox.Focus();
            return;
        }

        try
        {
            var request = new CreateHealthTopicRequest
            {
                Title = TitleTextBox.Text,
                Status = (HealthTopicStatus)(StatusComboBox.SelectedItem ?? HealthTopicStatus.Observation),
                Priority = (HealthTopicPriority)(PriorityComboBox.SelectedItem ?? HealthTopicPriority.Normal),
                ShortDescription = ShortDescriptionTextBox.Text,
                Notes = NotesTextBox.Text
            };

            await _healthTopicService.CreateTopicAsync(request);

            DialogResult = true;
            Close();
        }
        catch (ArgumentException ex)
        {
            // Validation errors are expected user-feedback cases. The message is generic
            // and does not contain medical content.
            MessageBox.Show(
                this,
                ex.Message,
                "SASD Health Research Notebook",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch
        {
            UiErrorHandler.ShowSafeError(this, "create health topic");
        }
    }

    private sealed record WizardStep(string Title, string Description);
}
