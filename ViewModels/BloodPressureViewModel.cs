using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ReactiveUI;
using HealthOptimizer.Models;
using HealthOptimizer.Data;

namespace HealthOptimizer.ViewModels
{
    public class BloodPressureViewModel : ViewModelBase
    {
        private DateTimeOffset _readingDateTime = DateTimeOffset.Now;
        private int _systolic = 120;
        private int _diastolic = 80;
        private int _pulse = 70;
        private string _notes = string.Empty;
        private string _statusMessage = string.Empty;

        public DateTimeOffset ReadingDateTime
        {
            get => _readingDateTime;
            set => this.RaiseAndSetIfChanged(ref _readingDateTime, value);
        }

        public int Systolic
        {
            get => _systolic;
            set => this.RaiseAndSetIfChanged(ref _systolic, value);
        }

        public int Diastolic
        {
            get => _diastolic;
            set => this.RaiseAndSetIfChanged(ref _diastolic, value);
        }

        public int Pulse
        {
            get => _pulse;
            set => this.RaiseAndSetIfChanged(ref _pulse, value);
        }

        public string Notes
        {
            get => _notes;
            set => this.RaiseAndSetIfChanged(ref _notes, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
        }

        public string BPCategory => GetBPCategory();

        public ObservableCollection<BloodPressureReading> RecentReadings { get; set; } = new();

        public ICommand SaveCommand { get; }
        public ICommand RefreshCommand { get; }

        public BloodPressureViewModel()
        {
            SaveCommand = ReactiveCommand.Create(SaveReading);
            RefreshCommand = ReactiveCommand.Create(LoadRecentReadings);

            LoadRecentReadings();
        }

        private string GetBPCategory()
        {
            if (Systolic < 120 && Diastolic < 80)
                return "✓ Normal";
            else if (Systolic < 130 && Diastolic < 80)
                return "⚠ Elevated";
            else if (Systolic < 140 || Diastolic < 90)
                return "⚠ High BP Stage 1";
            else if (Systolic < 180 || Diastolic < 120)
                return "⚠ High BP Stage 2";
            else
                return "🚨 Hypertensive Crisis";
        }

        private void SaveReading()
        {
            try
            {
                using var db = new HealthOptimizerDbContext();

                var reading = new BloodPressureReading
                {
                    DateTime = ReadingDateTime.DateTime,
                    Systolic = Systolic,
                    Diastolic = Diastolic,
                    Pulse = Pulse,
                    Notes = Notes
                };

                db.BloodPressureReadings.Add(reading);
                db.SaveChanges();

                StatusMessage = $"✓ Reading saved at {DateTime.Now:h:mm tt} - {BPCategory}";

                // Clear form for next reading
                Notes = string.Empty;
                ReadingDateTime = DateTimeOffset.Now;

                // Refresh the list
                LoadRecentReadings();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
        }

        private void LoadRecentReadings()
        {
            try
            {
                using var db = new HealthOptimizerDbContext();
                var readings = db.BloodPressureReadings
                    .OrderByDescending(r => r.DateTime)
                    .Take(10)
                    .ToList();

                RecentReadings.Clear();
                foreach (var reading in readings)
                {
                    RecentReadings.Add(reading);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading readings: {ex.Message}";
            }
        }
    }
}