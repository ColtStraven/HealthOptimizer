using Avalonia.Controls;
using HealthOptimizer.ViewModels;
using ScottPlot;
using ScottPlot.Avalonia;
using System.Linq;

namespace HealthOptimizer.Views
{
    public partial class CorrelationView : UserControl
    {
        private readonly CorrelationViewModel _viewModel;

        public CorrelationView()
        {
            InitializeComponent();
            _viewModel = new CorrelationViewModel();
            DataContext = _viewModel;

            this.AttachedToVisualTree += CorrelationView_AttachedToVisualTree;
        }

        private void CorrelationView_AttachedToVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
        {
            CreateScatterPlot();
        }

        private void CreateScatterPlot()
        {
            var plotControl = new AvaPlot { Height = 420 };

            if (_viewModel.ScatterData.Any())
            {
                var carbs = _viewModel.ScatterData.Select(d => d.Carbs).ToArray();
                var systolic = _viewModel.ScatterData.Select(d => d.Systolic).ToArray();

                // Add scatter points
                var scatter = plotControl.Plot.Add.Scatter(carbs, systolic);
                scatter.MarkerSize = 12;
                scatter.MarkerShape = MarkerShape.FilledCircle;
                scatter.Color = ScottPlot.Color.FromHex("#2196F3");
                scatter.LineWidth = 0; // No connecting lines

                // Add trend line (linear regression)
                if (carbs.Length >= 2)
                {
                    // Manual linear regression calculation
                    double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0;
                    int n = carbs.Length;

                    for (int i = 0; i < n; i++)
                    {
                        sumX += carbs[i];
                        sumY += systolic[i];
                        sumXY += carbs[i] * systolic[i];
                        sumX2 += carbs[i] * carbs[i];
                    }

                    double slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
                    double intercept = (sumY - slope * sumX) / n;

                    var x1 = carbs.Min();
                    var x2 = carbs.Max();
                    var y1 = slope * x1 + intercept;
                    var y2 = slope * x2 + intercept;

                    var trendLine = plotControl.Plot.Add.Line(x1, y1, x2, y2);
                    trendLine.LineWidth = 2;
                    trendLine.Color = ScottPlot.Color.FromHex("#f44336");
                    trendLine.LinePattern = LinePattern.Dashed;
                    trendLine.LegendText = "Trend Line";
                }

                // Add reference line at 120 (normal BP threshold)
                var normalLine = plotControl.Plot.Add.HorizontalLine(120);
                normalLine.LinePattern = LinePattern.Dotted;
                normalLine.Color = ScottPlot.Color.FromHex("#4CAF50");
                normalLine.LineWidth = 2;
                normalLine.LegendText = "Normal BP (120)";

                // Highlight optimal carb range if available
                if (_viewModel.OptimalCarbsMin > 0 && _viewModel.OptimalCarbsMax > 0)
                {
                    var optimalRange = plotControl.Plot.Add.VerticalSpan(
                        _viewModel.OptimalCarbsMin,
                        _viewModel.OptimalCarbsMax);
                    optimalRange.FillColor = ScottPlot.Color.FromHex("#4CAF50").WithAlpha(0.2);
                    optimalRange.LegendText = "Optimal Range";
                }

                plotControl.Plot.XLabel("Carbs (g)");
                plotControl.Plot.YLabel("Systolic BP (mmHg)");
                plotControl.Plot.Title("Carbs vs Blood Pressure");
                plotControl.Plot.ShowLegend();
            }
            else
            {
                plotControl.Plot.Add.Text("No correlation data yet. Need more matched BP and carb data.", 0.5, 0.5);
            }

            var container = this.FindControl<Border>("ScatterChartContainer");
            if (container != null)
            {
                container.Child = plotControl;
            }
        }
    }
}