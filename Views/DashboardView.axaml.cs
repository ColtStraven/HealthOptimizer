using Avalonia.Controls;
using HealthOptimizer.ViewModels;
using ScottPlot;
using ScottPlot.Avalonia;
using System.Linq;

namespace HealthOptimizer.Views
{
    public partial class DashboardView : UserControl
    {
        private readonly DashboardViewModel _viewModel;

        public DashboardView()
        {
            InitializeComponent();
            _viewModel = new DashboardViewModel();
            DataContext = _viewModel;

            this.AttachedToVisualTree += DashboardView_AttachedToVisualTree;
        }

        private void DashboardView_AttachedToVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
        {
            CreateCharts();
        }

        private void CreateCharts()
        {
            CreateWeightChart();
            CreateBPChart();
            CreateCarbsChart();
        }

        private void CreateWeightChart()
        {
            var plotControl = new AvaPlot { Height = 280 };

            if (_viewModel.WeightData.Any())
            {
                var dates = _viewModel.WeightData.Select(d => d.Date.ToOADate()).ToArray();
                var weights = _viewModel.WeightData.Select(d => d.Weight).ToArray();

                var scatter = plotControl.Plot.Add.Scatter(dates, weights);
                scatter.LineWidth = 2;
                scatter.Color = ScottPlot.Color.FromHex("#2196F3");
                scatter.MarkerSize = 8;

                plotControl.Plot.Axes.DateTimeTicksBottom();
                plotControl.Plot.YLabel("Weight (lbs)");
                plotControl.Plot.XLabel("Date");
            }
            else
            {
                plotControl.Plot.Add.Text("No weight data yet", 0.5, 0.5);
            }

            var container = this.FindControl<Border>("WeightChartContainer");
            if (container != null)
            {
                container.Child = plotControl;
            }
        }

        private void CreateBPChart()
        {
            var plotControl = new AvaPlot { Height = 280 };

            if (_viewModel.BPData.Any())
            {
                var dates = _viewModel.BPData.Select(d => d.Date.ToOADate()).ToArray();
                var systolic = _viewModel.BPData.Select(d => (double)d.Systolic).ToArray();
                var diastolic = _viewModel.BPData.Select(d => (double)d.Diastolic).ToArray();

                var systolicLine = plotControl.Plot.Add.Scatter(dates, systolic);
                systolicLine.LineWidth = 2;
                systolicLine.Color = ScottPlot.Color.FromHex("#f44336");
                systolicLine.MarkerSize = 8;
                systolicLine.LegendText = "Systolic";

                var diastolicLine = plotControl.Plot.Add.Scatter(dates, diastolic);
                diastolicLine.LineWidth = 2;
                diastolicLine.Color = ScottPlot.Color.FromHex("#2196F3");
                diastolicLine.MarkerSize = 8;
                diastolicLine.LegendText = "Diastolic";

                var normalLine = plotControl.Plot.Add.HorizontalLine(120);
                normalLine.LinePattern = LinePattern.Dashed;
                normalLine.Color = ScottPlot.Color.FromHex("#4CAF50");
                normalLine.LegendText = "Normal (120)";

                plotControl.Plot.Axes.DateTimeTicksBottom();
                plotControl.Plot.YLabel("Blood Pressure (mmHg)");
                plotControl.Plot.XLabel("Date");
                plotControl.Plot.ShowLegend();
            }
            else
            {
                plotControl.Plot.Add.Text("No BP data yet", 0.5, 0.5);
            }

            var container = this.FindControl<Border>("BPChartContainer");
            if (container != null)
            {
                container.Child = plotControl;
            }
        }

        private void CreateCarbsChart()
        {
            var plotControl = new AvaPlot { Height = 280 };

            if (_viewModel.CarbsData.Any())
            {
                var dates = _viewModel.CarbsData.Select(d => d.Date.ToOADate()).ToArray();
                var carbs = _viewModel.CarbsData.Select(d => d.Carbs).ToArray();

                var scatter = plotControl.Plot.Add.Scatter(dates, carbs);
                scatter.LineWidth = 2;
                scatter.Color = ScottPlot.Color.FromHex("#FF9800");
                scatter.MarkerSize = 8;

                plotControl.Plot.Axes.DateTimeTicksBottom();
                plotControl.Plot.YLabel("Carbs (g)");
                plotControl.Plot.XLabel("Date");
            }
            else
            {
                plotControl.Plot.Add.Text("No carbs data yet", 0.5, 0.5);
            }

            var container = this.FindControl<Border>("CarbsChartContainer");
            if (container != null)
            {
                container.Child = plotControl;
            }
        }
    }
}