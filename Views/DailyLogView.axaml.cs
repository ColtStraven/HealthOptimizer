using Avalonia.Controls;
using HealthOptimizer.ViewModels;

namespace HealthOptimizer.Views
{
    public partial class DailyLogView : UserControl
    {
        public DailyLogView()
        {
            InitializeComponent();
            DataContext = new DailyLogViewModel();
        }
    }
}