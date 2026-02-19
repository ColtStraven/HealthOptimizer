using System;
using System.Windows.Input;
using ReactiveUI;
using HealthOptimizer.Models;
using HealthOptimizer.Data;
using System.Linq;

namespace HealthOptimizer.ViewModels
{
    public class DailyLogViewModel : ViewModelBase
    {
        private DateTimeOffset _selectedDate = DateTimeOffset.Now;
        private double _weight;
        private double _calories;
        private double _proteinGrams;
        private double _carbsGrams;
        private double _fatsGrams;
        private int _steps;
        private string _statusMessage = string.Empty;

        public DateTimeOffset SelectedDate
        {
            get => _selectedDate;
            set => this.RaiseAndSetIfChanged(ref _selectedDate, value);
        }

        public double Weight
        {
            get => _weight;
            set => this.RaiseAndSetIfChanged(ref _weight, value);
        }

        public double Calories
        {
            get => _calories;
            set => this.RaiseAndSetIfChanged(ref _calories, value);
        }

        public double ProteinGrams
        {
            get => _proteinGrams;
            set => this.RaiseAndSetIfChanged(ref _proteinGrams, value);
        }

        public double CarbsGrams
        {
            get => _carbsGrams;
            set => this.RaiseAndSetIfChanged(ref _carbsGrams, value);
        }

        public double FatsGrams
        {
            get => _fatsGrams;
            set => this.RaiseAndSetIfChanged(ref _fatsGrams, value);
        }

        public int Steps
        {
            get => _steps;
            set => this.RaiseAndSetIfChanged(ref _steps, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand LoadTodayCommand { get; }

        public DailyLogViewModel()
        {
            SaveCommand = ReactiveCommand.Create(SaveDailyLog);
            LoadTodayCommand = ReactiveCommand.Create(LoadTodayData);

            // Load today's data on startup
            LoadTodayData();
        }

        private void SaveDailyLog()
        {
            try
            {
                using var db = new HealthOptimizerDbContext();

                // Check if entry already exists for this date
                var existing = db.DailyLogs.FirstOrDefault(d => d.Date.Date == SelectedDate.DateTime.Date);

                if (existing != null)
                {
                    // Update existing
                    existing.Weight = Weight;
                    existing.Calories = Calories;
                    existing.ProteinGrams = ProteinGrams;
                    existing.CarbsGrams = CarbsGrams;
                    existing.FatsGrams = FatsGrams;
                    existing.Steps = Steps;
                }
                else
                {
                    // Create new
                    var dailyLog = new DailyLog
                    {
                        Date = SelectedDate.DateTime.Date,
                        Weight = Weight,
                        Calories = Calories,
                        ProteinGrams = ProteinGrams,
                        CarbsGrams = CarbsGrams,
                        FatsGrams = FatsGrams,
                        Steps = Steps
                    };
                    db.DailyLogs.Add(dailyLog);
                }

                db.SaveChanges();
                StatusMessage = $"✓ Saved successfully at {DateTime.Now:h:mm tt}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Error saving: {ex.Message}";
                Console.WriteLine($"Full error: {ex}");
            }
        }

        private void LoadTodayData()
        {
            try
            {
                using var db = new HealthOptimizerDbContext();
                var todayLog = db.DailyLogs.FirstOrDefault(d => d.Date.Date == SelectedDate.DateTime.Date);

                if (todayLog != null)
                {
                    Weight = todayLog.Weight;
                    Calories = todayLog.Calories;
                    ProteinGrams = todayLog.ProteinGrams;
                    CarbsGrams = todayLog.CarbsGrams;
                    FatsGrams = todayLog.FatsGrams;
                    Steps = todayLog.Steps;
                    StatusMessage = "Loaded existing data for today";
                }
                else
                {
                    StatusMessage = "No data for this date - enter new data";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading: {ex.Message}";
            }
        }
    }
}