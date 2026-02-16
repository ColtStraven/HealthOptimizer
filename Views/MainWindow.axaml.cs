using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.Generic;
using System.Linq;

namespace HealthOptimizer.Views
{
    public partial class MainWindow : Window
    {
        private readonly List<Button> _navButtons = new();
        private readonly Dictionary<Button, Control> _buttonToContent = new();

        public MainWindow()
        {
            InitializeComponent();
            SetupNavigation();
        }

        private void SetupNavigation()
        {
            // Get all navigation buttons
            _navButtons.Add(this.FindControl<Button>("DashboardBtn")!);
            _navButtons.Add(this.FindControl<Button>("AnalysisBtn")!);
            _navButtons.Add(this.FindControl<Button>("StrengthBtn")!);
            _navButtons.Add(this.FindControl<Button>("DailyLogBtn")!);
            _navButtons.Add(this.FindControl<Button>("BPBtn")!);
            _navButtons.Add(this.FindControl<Button>("WorkoutBtn")!);
            _navButtons.Add(this.FindControl<Button>("MeasurementsBtn")!);
            _navButtons.Add(this.FindControl<Button>("MultiVarBtn")!);

            // Map buttons to content
            _buttonToContent[this.FindControl<Button>("DashboardBtn")!] = this.FindControl<Control>("DashboardContent")!;
            _buttonToContent[this.FindControl<Button>("AnalysisBtn")!] = this.FindControl<Control>("AnalysisContent")!;
            _buttonToContent[this.FindControl<Button>("StrengthBtn")!] = this.FindControl<Control>("StrengthContent")!;
            _buttonToContent[this.FindControl<Button>("DailyLogBtn")!] = this.FindControl<Control>("DailyLogContent")!;
            _buttonToContent[this.FindControl<Button>("BPBtn")!] = this.FindControl<Control>("BPContent")!;
            _buttonToContent[this.FindControl<Button>("WorkoutBtn")!] = this.FindControl<Control>("WorkoutContent")!;
            _buttonToContent[this.FindControl<Button>("MeasurementsBtn")!] = this.FindControl<Control>("MeasurementsContent")!;
            _buttonToContent[this.FindControl<Button>("MultiVarBtn")!] = this.FindControl<Control>("MultiVarContent")!;

            // Set Dashboard as active by default
            SetActiveButton(this.FindControl<Button>("DashboardBtn")!);
        }

        private void ShowDashboard(object? sender, RoutedEventArgs e) => NavigateTo((Button)sender!);
        private void ShowAnalysis(object? sender, RoutedEventArgs e) => NavigateTo((Button)sender!);
        private void ShowStrength(object? sender, RoutedEventArgs e) => NavigateTo((Button)sender!);
        private void ShowDailyLog(object? sender, RoutedEventArgs e) => NavigateTo((Button)sender!);
        private void ShowBloodPressure(object? sender, RoutedEventArgs e) => NavigateTo((Button)sender!);
        private void ShowWorkout(object? sender, RoutedEventArgs e) => NavigateTo((Button)sender!);
        private void ShowMeasurements(object? sender, RoutedEventArgs e) => NavigateTo((Button)sender!);
        private void ShowMultiVar(object? sender, RoutedEventArgs e) => NavigateTo((Button)sender!);

        private void NavigateTo(Button clickedButton)
        {
            SetActiveButton(clickedButton);
            ShowContent(_buttonToContent[clickedButton]);
        }

        private void SetActiveButton(Button activeButton)
        {
            foreach (var button in _navButtons)
            {
                if (button.Classes.Contains("active"))
                {
                    button.Classes.Remove("active");
                }
            }
            activeButton.Classes.Add("active");
        }

        private void ShowContent(Control content)
        {
            foreach (var kvp in _buttonToContent.Values)
            {
                kvp.IsVisible = false;
            }
            content.IsVisible = true;
        }
    }
}