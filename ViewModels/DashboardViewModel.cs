using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;
using HealthOptimizer.Models;
using HealthOptimizer.Data;

namespace HealthOptimizer.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private string _statusMessage = "Loading data...";
        private int _totalDaysLogged;
        private double _averageWeight;
        private double _averageCalories;
        private double _averageCarbs;
        private string _averageBP = "N/A";
        private int _bpReadingsCount;

        public string StatusMessage
        {
            get => _statusMessage;
            set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
        }

        public int TotalDaysLogged
        {
            get => _totalDaysLogged;
            set => this.RaiseAndSetIfChanged(ref _totalDaysLogged, value);
        }

        public double AverageWeight
        {
            get => _averageWeight;
            set => this.RaiseAndSetIfChanged(ref _averageWeight, value);
        }

        public double AverageCalories
        {
            get => _averageCalories;
            set => this.RaiseAndSetIfChanged(ref _averageCalories, value);
        }

        public double AverageCarbs
        {
            get => _averageCarbs;
            set => this.RaiseAndSetIfChanged(ref _averageCarbs, value);
        }

        public string AverageBP
        {
            get => _averageBP;
            set => this.RaiseAndSetIfChanged(ref _averageBP, value);
        }

        public int BPReadingsCount
        {
            get => _bpReadingsCount;
            set => this.RaiseAndSetIfChanged(ref _bpReadingsCount, value);
        }

        // Data for charts
        public List<(DateTime Date, double Weight)> WeightData { get; private set; } = new();
        public List<(DateTime Date, int Systolic, int Diastolic)> BPData { get; private set; } = new();
        public List<(DateTime Date, double Carbs)> CarbsData { get; private set; } = new();

        public DashboardViewModel()
        {
            LoadDashboardData();
        }

        public void LoadDashboardData()
        {
            try
            {
                using var db = new HealthOptimizerDbContext();

                // Load daily logs
                var dailyLogs = db.DailyLogs
                    .OrderBy(d => d.Date)
                    .ToList();

                TotalDaysLogged = dailyLogs.Count;

                if (dailyLogs.Any())
                {
                    AverageWeight = Math.Round(dailyLogs.Average(d => d.Weight), 1);
                    AverageCalories = Math.Round(dailyLogs.Average(d => d.Calories), 0);
                    AverageCarbs = Math.Round(dailyLogs.Average(d => d.CarbsGrams), 1);

                    // Prepare weight chart data
                    WeightData = dailyLogs
                        .Where(d => d.Weight > 0)
                        .Select(d => (d.Date, d.Weight))
                        .ToList();

                    // Prepare carbs chart data
                    CarbsData = dailyLogs
                        .Where(d => d.CarbsGrams > 0)
                        .Select(d => (d.Date, d.CarbsGrams))
                        .ToList();
                }

                // Load BP readings
                var bpReadings = db.BloodPressureReadings
                    .OrderBy(b => b.DateTime)
                    .ToList();

                BPReadingsCount = bpReadings.Count;

                if (bpReadings.Any())
                {
                    var avgSystolic = (int)Math.Round(bpReadings.Average(b => b.Systolic));
                    var avgDiastolic = (int)Math.Round(bpReadings.Average(b => b.Diastolic));
                    AverageBP = $"{avgSystolic}/{avgDiastolic}";

                    // Prepare BP chart data
                    BPData = bpReadings
                        .Select(b => (b.DateTime, b.Systolic, b.Diastolic))
                        .ToList();
                }

                StatusMessage = $"Loaded {TotalDaysLogged} days of logs and {BPReadingsCount} BP readings";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading data: {ex.Message}";
            }
        }
    }
}