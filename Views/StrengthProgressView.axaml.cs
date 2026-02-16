using Avalonia.Controls;
using HealthOptimizer.ViewModels;
using ScottPlot;
using ScottPlot.Avalonia;
using System.Linq;

namespace HealthOptimizer.Views
{
    public partial class StrengthProgressView : UserControl
    {
        private readonly StrengthProgressViewModel _viewModel;

        public StrengthProgressView()
        {
            InitializeComponent();
            _viewModel = new StrengthProgressViewModel();
            DataContext = _viewModel;

            this.AttachedToVisualTree += StrengthProgressView_AttachedToVisualTree;
        }

        private void StrengthProgressView_AttachedToVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
        {
            CreateCharts();

            // Re-create charts when exercise selection changes
            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_viewModel.SelectedExercise))
                {
                    CreateCharts();
                }
            };
        }

        private void CreateCharts()
        {
            CreateE1RMChart();
            CreateVolumeChart();
            CreateMaxWeightChart();
        }

        private void CreateE1RMChart()
        {
            var plotControl = new AvaPlot { Height = 320 };

            if (_viewModel.E1RMData.Any())
            {
                var dates = _viewModel.E1RMData.Select(d => d.Date.ToOADate()).ToArray();
                var e1rms = _viewModel.E1RMData.Select(d => d.E1RM).ToArray();

                var scatter = plotControl.Plot.Add.Scatter(dates, e1rms);
                scatter.LineWidth = 3;
                scatter.Color = ScottPlot.Color.FromHex("#2196F3");
                scatter.MarkerSize = 10;
                scatter.MarkerShape = MarkerShape.FilledCircle;

                // Add trend line
                if (dates.Length >= 2)
                {
                    double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0;
                    int n = dates.Length;

                    for (int i = 0; i < n; i++)
                    {
                        sumX += dates[i];
                        sumY += e1rms[i];
                        sumXY += dates[i] * e1rms[i];
                        sumX2 += dates[i] * dates[i];
                    }

                    double slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
                    double intercept = (sumY - slope * sumX) / n;

                    var x1 = dates.Min();
                    var x2 = dates.Max();
                    var y1 = slope * x1 + intercept;
                    var y2 = slope * x2 + intercept;

                    var trendLine = plotControl.Plot.Add.Line(x1, y1, x2, y2);
                    trendLine.LineWidth = 2;
                    trendLine.Color = ScottPlot.Color.FromHex("#4CAF50");
                    trendLine.LinePattern = LinePattern.Dashed;
                    trendLine.LegendText = "Trend";
                }

                // Add PR line
                var prLine = plotControl.Plot.Add.HorizontalLine(_viewModel.AllTimePR);
                prLine.LinePattern = LinePattern.Dotted;
                prLine.Color = ScottPlot.Color.FromHex("#FF9800");
                prLine.LineWidth = 2;
                prLine.LegendText = $"PR: {_viewModel.AllTimePR:F1} lbs";

                plotControl.Plot.Axes.DateTimeTicksBottom();
                plotControl.Plot.YLabel("Estimated 1RM (lbs)");
                plotControl.Plot.XLabel("Date");
                plotControl.Plot.ShowLegend();
            }
            else
            {
                plotControl.Plot.Add.Text("No data yet. Log some workouts!", 0.5, 0.5);
            }

            var container = this.FindControl<Border>("E1RMChartContainer");
            if (container != null)
            {
                container.Child = plotControl;
            }
        }

        private void CreateVolumeChart()
        {
            var plotControl = new AvaPlot { Height = 320 };

            if (_viewModel.VolumeData.Any())
            {
                var dates = _viewModel.VolumeData.Select(d => d.Date.ToOADate()).ToArray();
                var volumes = _viewModel.VolumeData.Select(d => d.Volume).ToArray();

                var bars = plotControl.Plot.Add.Bars(dates, volumes);
                bars.Color = ScottPlot.Color.FromHex("#9C27B0");

                plotControl.Plot.Axes.DateTimeTicksBottom();
                plotControl.Plot.YLabel("Volume (lbs × reps)");
                plotControl.Plot.XLabel("Date");
            }
            else
            {
                plotControl.Plot.Add.Text("No data yet", 0.5, 0.5);
            }

            var container = this.FindControl<Border>("VolumeChartContainer");
            if (container != null)
            {
                container.Child = plotControl;
            }
        }

        private void CreateMaxWeightChart()
        {
            var plotControl = new AvaPlot { Height = 320 };

            if (_viewModel.MaxWeightData.Any())
            {
                var dates = _viewModel.MaxWeightData.Select(d => d.Date.ToOADate()).ToArray();
                var weights = _viewModel.MaxWeightData.Select(d => d.MaxWeight).ToArray();

                var scatter = plotControl.Plot.Add.Scatter(dates, weights);
                scatter.LineWidth = 2;
                scatter.Color = ScottPlot.Color.FromHex("#f44336");
                scatter.MarkerSize = 10;
                scatter.MarkerShape = MarkerShape.FilledSquare;

                plotControl.Plot.Axes.DateTimeTicksBottom();
                plotControl.Plot.YLabel("Max Weight (lbs)");
                plotControl.Plot.XLabel("Date");
            }
            else
            {
                plotControl.Plot.Add.Text("No data yet", 0.5, 0.5);
            }

            var container = this.FindControl<Border>("MaxWeightChartContainer");
            if (container != null)
            {
                container.Child = plotControl;
            }
        }
    }
}