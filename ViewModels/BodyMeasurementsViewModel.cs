using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ReactiveUI;
using HealthOptimizer.Models;
using HealthOptimizer.Data;

namespace HealthOptimizer.ViewModels
{
    public class BodyMeasurementsViewModel : ViewModelBase
    {
        private DateTimeOffset _measurementDate = DateTimeOffset.Now;
        private double _waistInches;
        private double _chestInches;
        private double _leftArmInches;
        private double _rightArmInches;
        private double _leftThighInches;
        private double _rightThighInches;
        private double _neckInches;
        private double _hipsInches;
        private string _notes = string.Empty;
        private string _statusMessage = string.Empty;

        // Latest measurements for display
        private double _latestWaist;
        private double _latestChest;
        private double _latestArms;
        private double _latestThighs;

        // Changes from previous measurement
        private double _waistChange;
        private double _chestChange;
        private double _armsChange;
        private double _thighsChange;

        public DateTimeOffset MeasurementDate
        {
            get => _measurementDate;
            set => this.RaiseAndSetIfChanged(ref _measurementDate, value);
        }

        public double WaistInches
        {
            get => _waistInches;
            set => this.RaiseAndSetIfChanged(ref _waistInches, value);
        }

        public double ChestInches
        {
            get => _chestInches;
            set => this.RaiseAndSetIfChanged(ref _chestInches, value);
        }

        public double LeftArmInches
        {
            get => _leftArmInches;
            set => this.RaiseAndSetIfChanged(ref _leftArmInches, value);
        }

        public double RightArmInches
        {
            get => _rightArmInches;
            set => this.RaiseAndSetIfChanged(ref _rightArmInches, value);
        }

        public double LeftThighInches
        {
            get => _leftThighInches;
            set => this.RaiseAndSetIfChanged(ref _leftThighInches, value);
        }

        public double RightThighInches
        {
            get => _rightThighInches;
            set => this.RaiseAndSetIfChanged(ref _rightThighInches, value);
        }

        public double NeckInches
        {
            get => _neckInches;
            set => this.RaiseAndSetIfChanged(ref _neckInches, value);
        }

        public double HipsInches
        {
            get => _hipsInches;
            set => this.RaiseAndSetIfChanged(ref _hipsInches, value);
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

        // Latest measurements
        public double LatestWaist
        {
            get => _latestWaist;
            set => this.RaiseAndSetIfChanged(ref _latestWaist, value);
        }

        public double LatestChest
        {
            get => _latestChest;
            set => this.RaiseAndSetIfChanged(ref _latestChest, value);
        }

        public double LatestArms
        {
            get => _latestArms;
            set => this.RaiseAndSetIfChanged(ref _latestArms, value);
        }

        public double LatestThighs
        {
            get => _latestThighs;
            set => this.RaiseAndSetIfChanged(ref _latestThighs, value);
        }

        // Changes
        public double WaistChange
        {
            get => _waistChange;
            set => this.RaiseAndSetIfChanged(ref _waistChange, value);
        }

        public double ChestChange
        {
            get => _chestChange;
            set => this.RaiseAndSetIfChanged(ref _chestChange, value);
        }

        public double ArmsChange
        {
            get => _armsChange;
            set => this.RaiseAndSetIfChanged(ref _armsChange, value);
        }

        public double ThighsChange
        {
            get => _thighsChange;
            set => this.RaiseAndSetIfChanged(ref _thighsChange, value);
        }

        public ObservableCollection<BodyMeasurement> RecentMeasurements { get; } = new();

        public ICommand SaveCommand { get; }

        public BodyMeasurementsViewModel()
        {
            SaveCommand = ReactiveCommand.Create(SaveMeasurement);
            LoadRecentMeasurements();
            LoadLatestMeasurements();
        }

        private void SaveMeasurement()
        {
            try
            {
                using var db = new HealthOptimizerDbContext();

                // Check if measurement already exists for this date
                var existing = db.BodyMeasurements
                    .FirstOrDefault(m => m.Date.Date == MeasurementDate.DateTime.Date);

                if (existing != null)
                {
                    // Update existing
                    existing.WaistInches = WaistInches > 0 ? WaistInches : existing.WaistInches;
                    existing.ChestInches = ChestInches > 0 ? ChestInches : existing.ChestInches;
                    existing.LeftArmInches = LeftArmInches > 0 ? LeftArmInches : existing.LeftArmInches;
                    existing.RightArmInches = RightArmInches > 0 ? RightArmInches : existing.RightArmInches;
                    existing.LeftThighInches = LeftThighInches > 0 ? LeftThighInches : existing.LeftThighInches;
                    existing.RightThighInches = RightThighInches > 0 ? RightThighInches : existing.RightThighInches;
                    existing.NeckInches = NeckInches > 0 ? NeckInches : existing.NeckInches;
                    existing.HipsInches = HipsInches > 0 ? HipsInches : existing.HipsInches;
                    existing.Notes = Notes;
                }
                else
                {
                    // Create new
                    var measurement = new BodyMeasurement
                    {
                        Date = MeasurementDate.DateTime.Date,
                        WaistInches = WaistInches > 0 ? WaistInches : null,
                        ChestInches = ChestInches > 0 ? ChestInches : null,
                        LeftArmInches = LeftArmInches > 0 ? LeftArmInches : null,
                        RightArmInches = RightArmInches > 0 ? RightArmInches : null,
                        LeftThighInches = LeftThighInches > 0 ? LeftThighInches : null,
                        RightThighInches = RightThighInches > 0 ? RightThighInches : null,
                        NeckInches = NeckInches > 0 ? NeckInches : null,
                        HipsInches = HipsInches > 0 ? HipsInches : null,
                        Notes = Notes
                    };

                    db.BodyMeasurements.Add(measurement);
                }

                db.SaveChanges();

                StatusMessage = $"✓ Measurements saved at {DateTime.Now:h:mm tt}";

                LoadRecentMeasurements();
                LoadLatestMeasurements();

                // Clear form
                WaistInches = 0;
                ChestInches = 0;
                LeftArmInches = 0;
                RightArmInches = 0;
                LeftThighInches = 0;
                RightThighInches = 0;
                NeckInches = 0;
                HipsInches = 0;
                Notes = string.Empty;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
        }

        private void LoadRecentMeasurements()
        {
            try
            {
                using var db = new HealthOptimizerDbContext();

                var measurements = db.BodyMeasurements
                    .OrderByDescending(m => m.Date)
                    .Take(10)
                    .ToList();

                RecentMeasurements.Clear();
                foreach (var m in measurements)
                {
                    RecentMeasurements.Add(m);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading measurements: {ex.Message}";
            }
        }

        private void LoadLatestMeasurements()
        {
            try
            {
                using var db = new HealthOptimizerDbContext();

                var measurements = db.BodyMeasurements
                    .OrderByDescending(m => m.Date)
                    .Take(2)
                    .ToList();

                if (measurements.Any())
                {
                    var latest = measurements.First();
                    LatestWaist = latest.WaistInches ?? 0;
                    LatestChest = latest.ChestInches ?? 0;
                    LatestArms = ((latest.LeftArmInches ?? 0) + (latest.RightArmInches ?? 0)) / 2;
                    LatestThighs = ((latest.LeftThighInches ?? 0) + (latest.RightThighInches ?? 0)) / 2;

                    if (measurements.Count > 1)
                    {
                        var previous = measurements[1];
                        WaistChange = Math.Round((latest.WaistInches ?? 0) - (previous.WaistInches ?? 0), 2);
                        ChestChange = Math.Round((latest.ChestInches ?? 0) - (previous.ChestInches ?? 0), 2);

                        var latestAvgArm = ((latest.LeftArmInches ?? 0) + (latest.RightArmInches ?? 0)) / 2;
                        var prevAvgArm = ((previous.LeftArmInches ?? 0) + (previous.RightArmInches ?? 0)) / 2;
                        ArmsChange = Math.Round(latestAvgArm - prevAvgArm, 2);

                        var latestAvgThigh = ((latest.LeftThighInches ?? 0) + (latest.RightThighInches ?? 0)) / 2;
                        var prevAvgThigh = ((previous.LeftThighInches ?? 0) + (previous.RightThighInches ?? 0)) / 2;
                        ThighsChange = Math.Round(latestAvgThigh - prevAvgThigh, 2);
                    }
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
        }
    }
}