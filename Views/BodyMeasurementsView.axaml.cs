using Avalonia.Controls;
using HealthOptimizer.ViewModels;

namespace HealthOptimizer.Views
{
    public partial class BodyMeasurementsView : UserControl
    {
        public BodyMeasurementsView()
        {
            InitializeComponent();
            DataContext = new BodyMeasurementsViewModel();
        }
    }
}