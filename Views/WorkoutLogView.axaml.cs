using Avalonia.Controls;
using HealthOptimizer.ViewModels;

namespace HealthOptimizer.Views
{
    public partial class WorkoutLogView : UserControl
    {
        public WorkoutLogView()
        {
            InitializeComponent();
            DataContext = new WorkoutLogViewModel();
        }
    }
}