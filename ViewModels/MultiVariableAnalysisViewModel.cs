using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;
using HealthOptimizer.Models;
using HealthOptimizer.Data;
using MathNet.Numerics.Statistics;

namespace HealthOptimizer.ViewModels
{
    public class MultiVariableAnalysisViewModel : ViewModelBase
    {
        private string _statusMessage = "Analyzing data...";

        // Protein vs Strength
        private double _proteinStrengthCorr;
        private string _proteinStrengthStrength = "N/A";
        private int _optimalProteinMin;
        private int _optimalProteinMax;

        // Calories vs Weight
        private double _caloriesWeightCorr;
        private string _caloriesWeightStrength = "N/A";
        private int _optimalCaloriesMin;
        private int _optimalCaloriesMax;
        private string _weightTrend = "N/A";

        // Combined Analysis
        private string _combinedRecommendation = string.Empty;
        private string _recompStatus = "N/A";

        public string StatusMessage
        {
            get => _statusMessage;
            set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
        }

        // Protein vs Strength
        public double ProteinStrengthCorr
        {
            get => _proteinStrengthCorr;
            set => this.RaiseAndSetIfChanged(ref _proteinStrengthCorr, value);
        }

        public string ProteinStrengthStrength
        {
            get => _proteinStrengthStrength;
            set => this.RaiseAndSetIfChanged(ref _proteinStrengthStrength, value);
        }

        public int OptimalProteinMin
        {
            get => _optimalProteinMin;
            set => this.RaiseAndSetIfChanged(ref _optimalProteinMin, value);
        }

        public int OptimalProteinMax
        {
            get => _optimalProteinMax;
            set => this.RaiseAndSetIfChanged(ref _optimalProteinMax, value);
        }

        // Calories vs Weight
        public double CaloriesWeightCorr
        {
            get => _caloriesWeightCorr;
            set => this.RaiseAndSetIfChanged(ref _caloriesWeightCorr, value);
        }

        public string CaloriesWeightStrength
        {
            get => _caloriesWeightStrength;
            set => this.RaiseAndSetIfChanged(ref _caloriesWeightStrength, value);
        }

        public int OptimalCaloriesMin
        {
            get => _optimalCaloriesMin;
            set => this.RaiseAndSetIfChanged(ref _optimalCaloriesMin, value);
        }

        public int OptimalCaloriesMax
        {
            get => _optimalCaloriesMax;
            set => this.RaiseAndSetIfChanged(ref _optimalCaloriesMax, value);
        }

        public string WeightTrend
        {
            get => _weightTrend;
            set => this.RaiseAndSetIfChanged(ref _weightTrend, value);
        }

        // Combined
        public string CombinedRecommendation
        {
            get => _combinedRecommendation;
            set => this.RaiseAndSetIfChanged(ref _combinedRecommendation, value);
        }

        public string RecompStatus
        {
            get => _recompStatus;
            set => this.RaiseAndSetIfChanged(ref _recompStatus, value);
        }

        // Chart data
        public List<(double Protein, double StrengthGain)> ProteinStrengthData { get; private set; } = new();
        public List<(int Calories, double Weight)> CaloriesWeightData { get; private set; } = new();

        public MultiVariableAnalysisViewModel()
        {
            AnalyzeAllVariables();
        }

        private void AnalyzeAllVariables()
        {
            try
            {
                using var db = new HealthOptimizerDbContext();

                AnalyzeProteinVsStrength(db);
                AnalyzeCaloriesVsWeight(db);
                GenerateCombinedRecommendation(db);

                StatusMessage = "Analysis complete";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
        }

        private void AnalyzeProteinVsStrength(HealthOptimizerDbContext db)
        {
            // Get daily logs with protein data
            var dailyLogs = db.DailyLogs
                .Where(d => d.ProteinGrams > 0)
                .OrderBy(d => d.Date)
                .ToList();

            if (!dailyLogs.Any())
            {
                StatusMessage = "Not enough protein data";
                return;
            }

            // Get workout data and calculate strength gains over time
            var workoutSessions = db.WorkoutSessions
                .OrderBy(s => s.Date)
                .ToList();

            if (!workoutSessions.Any())
            {
                StatusMessage = "Not enough workout data";
                return;
            }

            // Calculate strength gains per week
            var strengthByWeek = new Dictionary<int, double>();
            var proteinByWeek = new Dictionary<int, double>();

            foreach (var session in workoutSessions)
            {
                var weekNumber = GetWeekNumber(session.Date);

                // Get total e1RM for this session
                var sets = db.WorkoutSets
                    .Where(s => s.SessionId == session.Id)
                    .ToList();

                var totalE1RM = sets.Sum(s => CalculateE1RM(s.Weight, s.Reps));

                if (!strengthByWeek.ContainsKey(weekNumber))
                    strengthByWeek[weekNumber] = 0;

                strengthByWeek[weekNumber] = Math.Max(strengthByWeek[weekNumber], totalE1RM);

                // Get protein for this week
                var weekStart = GetWeekStart(session.Date);
                var weekEnd = weekStart.AddDays(7);

                var weekProtein = dailyLogs
                    .Where(d => d.Date >= weekStart && d.Date < weekEnd)
                    .Average(d => d.ProteinGrams);

                if (weekProtein > 0 && !proteinByWeek.ContainsKey(weekNumber))
                    proteinByWeek[weekNumber] = weekProtein;
            }

            // Calculate week-over-week strength gains
            var proteinStrengthPairs = new List<(double Protein, double StrengthGain)>();
            var weeks = strengthByWeek.Keys.OrderBy(w => w).ToList();

            for (int i = 1; i < weeks.Count; i++)
            {
                var currentWeek = weeks[i];
                var previousWeek = weeks[i - 1];

                if (strengthByWeek.ContainsKey(currentWeek) &&
                    strengthByWeek.ContainsKey(previousWeek) &&
                    proteinByWeek.ContainsKey(currentWeek))
                {
                    var strengthGain = strengthByWeek[currentWeek] - strengthByWeek[previousWeek];
                    var protein = proteinByWeek[currentWeek];

                    if (strengthGain > -100 && strengthGain < 100) // Filter outliers
                        proteinStrengthPairs.Add((protein, strengthGain));
                }
            }

            if (proteinStrengthPairs.Count >= 3)
            {
                ProteinStrengthData = proteinStrengthPairs;

                var proteins = proteinStrengthPairs.Select(p => p.Protein).ToArray();
                var gains = proteinStrengthPairs.Select(p => p.StrengthGain).ToArray();

                ProteinStrengthCorr = Correlation.Pearson(proteins, gains);

                var absCorr = Math.Abs(ProteinStrengthCorr);
                ProteinStrengthStrength = absCorr < 0.3 ? "Weak" : absCorr < 0.7 ? "Moderate" : "Strong";

                // Find optimal protein range (weeks with positive strength gains)
                var positiveGains = proteinStrengthPairs.Where(p => p.StrengthGain > 0).ToList();
                if (positiveGains.Any())
                {
                    OptimalProteinMin = (int)Math.Floor(positiveGains.Min(p => p.Protein));
                    OptimalProteinMax = (int)Math.Ceiling(positiveGains.Max(p => p.Protein));
                }
            }
        }

        private void AnalyzeCaloriesVsWeight(HealthOptimizerDbContext db)
        {
            var dailyLogs = db.DailyLogs
                .Where(d => d.Calories > 0 && d.Weight > 0)
                .OrderBy(d => d.Date)
                .ToList();

            if (dailyLogs.Count < 10)
            {
                StatusMessage = "Not enough calorie/weight data (need at least 10 days)";
                return;
            }

            // Group by week and average
            var weeklyData = dailyLogs
                .GroupBy(d => GetWeekNumber(d.Date))
                .Select(g => new
                {
                    Week = g.Key,
                    AvgCalories = (int)g.Average(d => d.Calories),
                    AvgWeight = g.Average(d => d.Weight)
                })
                .OrderBy(w => w.Week)
                .ToList();

            if (weeklyData.Count >= 3)
            {
                CaloriesWeightData = weeklyData.Select(w => (w.AvgCalories, w.AvgWeight)).ToList();

                var calories = weeklyData.Select(w => (double)w.AvgCalories).ToArray();
                var weights = weeklyData.Select(w => w.AvgWeight).ToArray();

                CaloriesWeightCorr = Correlation.Pearson(calories, weights);

                var absCorr = Math.Abs(CaloriesWeightCorr);
                CaloriesWeightStrength = absCorr < 0.3 ? "Weak" : absCorr < 0.7 ? "Moderate" : "Strong";

                // Calculate weight trend
                var firstWeight = weeklyData.First().AvgWeight;
                var lastWeight = weeklyData.Last().AvgWeight;
                var weightChange = lastWeight - firstWeight;
                var weekCount = weeklyData.Count;

                WeightTrend = weightChange < -1 ? $"Losing {Math.Abs(weightChange / weekCount):F1} lbs/week" :
                              weightChange > 1 ? $"Gaining {weightChange / weekCount:F1} lbs/week" :
                              "Maintaining";

                // Find optimal calorie range for weight maintenance/slight deficit
                OptimalCaloriesMin = (int)weeklyData.Min(w => w.AvgCalories);
                OptimalCaloriesMax = (int)weeklyData.Max(w => w.AvgCalories);
            }
        }

        private void GenerateCombinedRecommendation(HealthOptimizerDbContext db)
        {
            // Check recomp status: strength up, weight/waist down
            var recentWeeks = 8;
            var cutoffDate = DateTime.Now.AddDays(-7 * recentWeeks);

            var recentLogs = db.DailyLogs.Where(d => d.Date >= cutoffDate).ToList();
            var oldLogs = db.DailyLogs.Where(d => d.Date < cutoffDate).Take(20).ToList();

            var recentSessions = db.WorkoutSessions.Where(s => s.Date >= cutoffDate).ToList();
            var oldSessions = db.WorkoutSessions.Where(s => s.Date < cutoffDate).Take(10).ToList();

            var recentMeasurements = db.BodyMeasurements.Where(m => m.Date >= cutoffDate).ToList();
            var oldMeasurements = db.BodyMeasurements.Where(m => m.Date < cutoffDate).Take(5).ToList();

            bool strengthIncreasing = false;
            bool weightDecreasing = false;
            bool waistDecreasing = false;

            // Check strength trend
            if (recentSessions.Any() && oldSessions.Any())
            {
                var recentStrength = CalculateAverageSessionStrength(db, recentSessions);
                var oldStrength = CalculateAverageSessionStrength(db, oldSessions);
                strengthIncreasing = recentStrength > oldStrength;
            }

            // Check weight trend
            if (recentLogs.Any() && oldLogs.Any())
            {
                var recentWeight = recentLogs.Average(l => l.Weight);
                var oldWeight = oldLogs.Average(l => l.Weight);
                weightDecreasing = recentWeight < oldWeight - 1; // At least 1 lb loss
            }

            // Check waist trend
            if (recentMeasurements.Any() && oldMeasurements.Any())
            {
                var recentWaist = recentMeasurements.Where(m => m.WaistInches.HasValue)
    .Average(m => m.WaistInches!.Value);
                var oldWaist = oldMeasurements.Where(m => m.WaistInches.HasValue)
                    .Average(m => m.WaistInches!.Value);
                waistDecreasing = recentWaist < oldWaist;
            }

            // Determine recomp status
            if (strengthIncreasing && (weightDecreasing || waistDecreasing))
            {
                RecompStatus = "🎉 Excellent Recomp!";
            }
            else if (strengthIncreasing)
            {
                RecompStatus = "💪 Building Strength";
            }
            else if (weightDecreasing || waistDecreasing)
            {
                RecompStatus = "📉 Losing Weight";
            }
            else
            {
                RecompStatus = "📊 Maintaining";
            }

            // Generate combined recommendation
            CombinedRecommendation = "COMBINED OPTIMIZATION SUMMARY:\n\n";

            if (OptimalProteinMin > 0 && OptimalProteinMax > 0)
            {
                CombinedRecommendation += $"🥩 PROTEIN: {OptimalProteinMin}-{OptimalProteinMax}g/day\n";
                CombinedRecommendation += $"   Correlation with strength: {ProteinStrengthStrength} ({ProteinStrengthCorr:F2})\n\n";
            }

            if (OptimalCaloriesMin > 0 && OptimalCaloriesMax > 0)
            {
                CombinedRecommendation += $"🔥 CALORIES: {OptimalCaloriesMin}-{OptimalCaloriesMax}/day\n";
                CombinedRecommendation += $"   Weight trend: {WeightTrend}\n\n";
            }

            CombinedRecommendation += $"📊 RECOMP STATUS: {RecompStatus}\n\n";

            CombinedRecommendation += "💡 RECOMMENDATIONS:\n";

            if (strengthIncreasing && (weightDecreasing || waistDecreasing))
            {
                CombinedRecommendation += "• Keep doing what you're doing! You're achieving ideal recomp.\n";
                CombinedRecommendation += "• Maintain current protein and calorie ranges.\n";
            }
            else if (!strengthIncreasing && OptimalProteinMin > 0)
            {
                CombinedRecommendation += $"• Consider increasing protein toward {OptimalProteinMax}g for better strength gains.\n";
            }

            if (!weightDecreasing && !waistDecreasing && OptimalCaloriesMin > 0)
            {
                CombinedRecommendation += $"• If fat loss is your goal, try lower end of calorie range ({OptimalCaloriesMin}).\n";
            }
        }

        private double CalculateAverageSessionStrength(HealthOptimizerDbContext db, List<WorkoutSession> sessions)
        {
            double totalStrength = 0;
            int count = 0;

            foreach (var session in sessions)
            {
                var sets = db.WorkoutSets.Where(s => s.SessionId == session.Id).ToList();
                var sessionStrength = sets.Sum(s => CalculateE1RM(s.Weight, s.Reps));

                if (sessionStrength > 0)
                {
                    totalStrength += sessionStrength;
                    count++;
                }
            }

            return count > 0 ? totalStrength / count : 0;
        }

        private double CalculateE1RM(double weight, int reps)
        {
            if (reps == 1) return weight;
            return weight * (1 + reps / 30.0);
        }

        private int GetWeekNumber(DateTime date)
        {
            var jan1 = new DateTime(date.Year, 1, 1);
            return (int)Math.Ceiling((date - jan1).TotalDays / 7.0);
        }

        private DateTime GetWeekStart(DateTime date)
        {
            var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-diff).Date;
        }
    }
}