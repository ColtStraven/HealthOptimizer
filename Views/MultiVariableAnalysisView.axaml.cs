using Avalonia.Controls;
using HealthOptimizer.ViewModels;
using ScottPlot;
using ScottPlot.Avalonia;
using System.Linq;

namespace HealthOptimizer.Views
{
    public partial class MultiVariableAnalysisView : UserControl
    {
        private readonly MultiVariableAnalysisViewModel _viewModel;

        public MultiVariableAnalysisView()
        {
            InitializeComponent();
            _viewModel = new MultiVariableAnalysisViewModel();
            DataContext = _viewModel;

            this.AttachedToVisualTree += MultiVariableAnalysisView_AttachedToVisualTree;
        }

        private void MultiVariableAnalysisView_AttachedToVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
        {
            CreateCharts();
        }

        private void CreateCharts()
        {
            CreateProteinStrengthChart();
            CreateCaloriesWeightChart();
        }

        private void CreateProteinStrengthChart()
        {
            var plotControl = new AvaPlot { Height = 320 };

            if (_viewModel.ProteinStrengthData.Any())
            {
                var protein = _viewModel.ProteinStrengthData.Select(d => d.Protein).ToArray();
                var strengthGain = _viewModel.ProteinStrengthData.Select(d => d.StrengthGain).ToArray();

                var scatter = plotControl.Plot.Add.Scatter(protein, strengthGain);
                scatter.MarkerSize = 12;
                scatter.MarkerShape = MarkerShape.FilledCircle;
                scatter.Color = ScottPlot.Color.FromHex("#10B981");
                scatter.LineWidth = 0;

                // Add trend line
                if (protein.Length >= 2)
                {
                    double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0;
                    int n = protein.Length;

                    for (int i = 0; i < n; i++)
                    {
                        sumX += protein[i];
                        sumY += strengthGain[i];
                        sumXY += protein[i] * strengthGain[i];
                        sumX2 += protein[i] * protein[i];
                    }

                    double slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
                    double intercept = (sumY - slope * sumX) / n;

                    var x1 = protein.Min();
                    var x2 = protein.Max();
                    var y1 = slope * x1 + intercept;
                    var y2 = slope * x2 + intercept;

                    var trendLine = plotControl.Plot.Add.Line(x1, y1, x2, y2);
                    trendLine.LineWidth = 2;
                    trendLine.Color = ScottPlot.Color.FromHex("#059669");
                    trendLine.LinePattern = LinePattern.Dashed;
                    trendLine.LegendText = "Trend";
                }

                // Add zero line
                var zeroLine = plotControl.Plot.Add.HorizontalLine(0);
                zeroLine.LinePattern = LinePattern.Dotted;
                zeroLine.Color = ScottPlot.Color.FromHex("#94A3B8");
                zeroLine.LineWidth = 1;

                plotControl.Plot.XLabel("Protein (g/day)");
                plotControl.Plot.YLabel("Strength Gain (e1RM change)");
                plotControl.Plot.Title("Protein Intake vs Strength Progress");
                plotControl.Plot.ShowLegend();
            }
            else
            {
                plotControl.Plot.Add.Text("Not enough data. Need workout and nutrition logs.", 0.5, 0.5);
            }

            var container = this.FindControl<Border>("ProteinStrengthChartContainer");
            if (container != null)
            {
                container.Child = plotControl;
            }
        }

        private void CreateCaloriesWeightChart()
        {
            var plotControl = new AvaPlot { Height = 320 };

            if (_viewModel.CaloriesWeightData.Any())
            {
                var calories = _viewModel.CaloriesWeightData.Select(d => (double)d.Calories).ToArray();
                var weight = _viewModel.CaloriesWeightData.Select(d => d.Weight).ToArray();

                var scatter = plotControl.Plot.Add.Scatter(calories, weight);
                scatter.MarkerSize = 12;
                scatter.MarkerShape = MarkerShape.FilledCircle;
                scatter.Color = ScottPlot.Color.FromHex("#3B82F6");
                scatter.LineWidth = 2;

                // Add trend line
                if (calories.Length >= 2)
                {
                    double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0;
                    int n = calories.Length;

                    for (int i = 0; i < n; i++)
                    {
                        sumX += calories[i];
                        sumY += weight[i];
                        sumXY += calories[i] * weight[i];
                        sumX2 += calories[i] * calories[i];
                    }

                    double slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
                    double intercept = (sumY - slope * sumX) / n;

                    var x1 = calories.Min();
                    var x2 = calories.Max();
                    var y1 = slope * x1 + intercept;
                    var y2 = slope * x2 + intercept;

                    var trendLine = plotControl.Plot.Add.Line(x1, y1, x2, y2);
                    trendLine.LineWidth = 2;
                    trendLine.Color = ScottPlot.Color.FromHex("#1E40AF");
                    trendLine.LinePattern = LinePattern.Dashed;
                    trendLine.LegendText = "Trend";
                }

                plotControl.Plot.XLabel("Calories (per day)");
                plotControl.Plot.YLabel("Weight (lbs)");
                plotControl.Plot.Title("Calorie Intake vs Weight Trend");
                plotControl.Plot.ShowLegend();
            }
            else
            {
                plotControl.Plot.Add.Text("Not enough data. Need daily logs with calories and weight.", 0.5, 0.5);
            }

            var container = this.FindControl<Border>("CaloriesWeightChartContainer");
            if (container != null)
            {
                container.Child = plotControl;
            }
        }
    }
}