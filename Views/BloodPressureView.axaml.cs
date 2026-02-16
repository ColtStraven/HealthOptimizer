using Avalonia.Controls;
using HealthOptimizer.ViewModels;

namespace HealthOptimizer.Views
{
    public partial class BloodPressureView : UserControl
    {
        public BloodPressureView()
        {
            InitializeComponent();
            DataContext = new BloodPressureViewModel();
        }
    }
}