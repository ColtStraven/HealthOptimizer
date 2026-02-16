using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;
using HealthOptimizer.Models;
using HealthOptimizer.Data;
using MathNet.Numerics.Statistics;

namespace HealthOptimizer.ViewModels
{
    public class CorrelationViewModel : ViewModelBase
    {
        private string _statusMessage = "Analyzing data...";
        private double _correlationCoefficient;
        private string _correlationStrength = "N/A";
        private string _recommendation = string.Empty;
        private int _optimalCarbsMin;
        private int _optimalCarbsMax;
        private double _avgBPInOptimalRange = 0;
        private double _avgBPOutsideRange = 0;

        public string StatusMessage
        {
            get => _statusMessage;
            set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
        }

        public double CorrelationCoefficient
        {
            get => _correlationCoefficient;
            set => this.RaiseAndSetIfChanged(ref _correlationCoefficient, value);
        }

        public string CorrelationStrength
        {
            get => _correlationStrength;
            set => this.RaiseAndSetIfChanged(ref _correlationStrength, value);
        }

        public string Recommendation
        {
            get => _recommendation;
            set => this.RaiseAndSetIfChanged(ref _recommendation, value);
        }

        public int OptimalCarbsMin
        {
            get => _optimalCarbsMin;
            set => this.RaiseAndSetIfChanged(ref _optimalCarbsMin, value);
        }

        public int OptimalCarbsMax
        {
            get => _optimalCarbsMax;
            set => this.RaiseAndSetIfChanged(ref _optimalCarbsMax, value);
        }

        public double AvgBPInOptimalRange
        {
            get => _avgBPInOptimalRange;
            set => this.RaiseAndSetIfChanged(ref _avgBPInOptimalRange, value);
        }

        public double AvgBPOutsideRange
        {
            get => _avgBPOutsideRange;
            set => this.RaiseAndSetIfChanged(ref _avgBPOutsideRange, value);
        }

        // Data for scatter plot
        public List<(double Carbs, double Systolic)> ScatterData { get; private set; } = new();

        public CorrelationViewModel()
        {
            AnalyzeCorrelation();
        }

        private void AnalyzeCorrelation()
        {
            try
            {
                using var db = new HealthOptimizerDbContext();

                // Get daily logs and BP readings
                var dailyLogs = db.DailyLogs.OrderBy(d => d.Date).ToList();
                var bpReadings = db.BloodPressureReadings.OrderBy(b => b.DateTime).ToList();

                if (!dailyLogs.Any() || !bpReadings.Any())
                {
                    StatusMessage = "Not enough data. Log at least 5 days of nutrition and 3 BP readings.";
                    return;
                }

                // Match BP readings with carb intake (using same day or previous day)
                var matchedData = new List<(double Carbs, int Systolic)>();

                foreach (var bp in bpReadings)
                {
                    // Try to find carb intake from same day
                    var sameDay = dailyLogs.FirstOrDefault(d => d.Date.Date == bp.DateTime.Date);
                    if (sameDay != null && sameDay.CarbsGrams > 0)
                    {
                        matchedData.Add((sameDay.CarbsGrams, bp.Systolic));
                    }
                    else
                    {
                        // Try previous day (carbs can affect BP next day)
                        var prevDay = dailyLogs.FirstOrDefault(d => d.Date.Date == bp.DateTime.Date.AddDays(-1));
                        if (prevDay != null && prevDay.CarbsGrams > 0)
                        {
                            matchedData.Add((prevDay.CarbsGrams, bp.Systolic));
                        }
                    }
                }

                if (matchedData.Count < 3)
                {
                    StatusMessage = "Not enough matched data. Need at least 3 BP readings with corresponding carb data.";
                    return;
                }

                // Prepare data for scatter plot
                ScatterData = matchedData.Select(m => (m.Carbs, (double)m.Systolic)).ToList();

                // Calculate correlation coefficient
                var carbsArray = matchedData.Select(m => m.Carbs).ToArray();
                var systolicArray = matchedData.Select(m => (double)m.Systolic).ToArray();

                CorrelationCoefficient = Correlation.Pearson(carbsArray, systolicArray);

                // Interpret correlation strength
                var absCorr = Math.Abs(CorrelationCoefficient);
                if (absCorr < 0.3)
                    CorrelationStrength = "Weak";
                else if (absCorr < 0.7)
                    CorrelationStrength = "Moderate";
                else
                    CorrelationStrength = "Strong";

                // Find optimal carb range (where BP is consistently normal)
                var normalBPReadings = matchedData.Where(m => m.Systolic < 120).ToList();

                if (normalBPReadings.Any())
                {
                    var normalCarbs = normalBPReadings.Select(m => m.Carbs).ToList();
                    OptimalCarbsMin = (int)Math.Floor(normalCarbs.Min());
                    OptimalCarbsMax = (int)Math.Ceiling(normalCarbs.Max());

                    // Calculate average BP in optimal range
                    var inRange = matchedData.Where(m => m.Carbs >= OptimalCarbsMin && m.Carbs <= OptimalCarbsMax);
                    if (inRange.Any())
                    {
                        AvgBPInOptimalRange = Math.Round(inRange.Average(m => m.Systolic), 1);
                    }

                    // Calculate average BP outside range
                    var outRange = matchedData.Where(m => m.Carbs < OptimalCarbsMin || m.Carbs > OptimalCarbsMax);
                    if (outRange.Any())
                    {
                        AvgBPOutsideRange = Math.Round(outRange.Average(m => m.Systolic), 1);
                    }

                    // Generate recommendation
                    GenerateRecommendation();
                }
                else
                {
                    Recommendation = "Your BP has been elevated in all readings. Consider reducing carbs further and consult your doctor.";
                }

                StatusMessage = $"Analysis complete. Analyzed {matchedData.Count} data points.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
        }

        private void GenerateRecommendation()
        {
            var corrDirection = CorrelationCoefficient > 0 ? "increases" : "decreases";

            Recommendation = $"Based on your data:\n\n";
            Recommendation += $"• Correlation: {CorrelationStrength} ({CorrelationCoefficient:F2})\n";
            Recommendation += $"• Your BP {corrDirection} as carb intake increases\n\n";

            if (OptimalCarbsMin > 0 && OptimalCarbsMax > 0)
            {
                Recommendation += $"🎯 OPTIMAL CARB RANGE: {OptimalCarbsMin}-{OptimalCarbsMax}g per day\n\n";

                if (AvgBPInOptimalRange > 0 && AvgBPOutsideRange > 0)
                {
                    var difference = AvgBPOutsideRange - AvgBPInOptimalRange;
                    Recommendation += $"• Avg BP in range: {AvgBPInOptimalRange} mmHg (Normal ✓)\n";
                    Recommendation += $"• Avg BP outside range: {AvgBPOutsideRange} mmHg\n";
                    Recommendation += $"• Difference: {difference:F1} mmHg\n\n";
                }

                Recommendation += $"💡 Stay within {OptimalCarbsMin}-{OptimalCarbsMax}g carbs daily to maintain healthy BP.";
            }
            else
            {
                Recommendation += "Continue tracking to identify your optimal carb range.";
            }
        }
    }
}