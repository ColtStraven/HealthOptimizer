using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;
using HealthOptimizer.Models;
using HealthOptimizer.Data;

namespace HealthOptimizer.ViewModels
{
    public class StrengthProgressViewModel : ViewModelBase
    {
        private string _selectedExercise = "Bench Press";
        private string _statusMessage = "Loading data...";
        private double _currentE1RM = 0;
        private double _allTimePR = 0;
        private double _percentChange = 0;
        private int _totalSetsLogged = 0;
        private DateTime _lastWorkout = DateTime.MinValue;

        public string SelectedExercise
        {
            get => _selectedExercise;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedExercise, value);
                LoadExerciseData();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
        }

        public double CurrentE1RM
        {
            get => _currentE1RM;
            set => this.RaiseAndSetIfChanged(ref _currentE1RM, value);
        }

        public double AllTimePR
        {
            get => _allTimePR;
            set => this.RaiseAndSetIfChanged(ref _allTimePR, value);
        }

        public double PercentChange
        {
            get => _percentChange;
            set => this.RaiseAndSetIfChanged(ref _percentChange, value);
        }

        public int TotalSetsLogged
        {
            get => _totalSetsLogged;
            set => this.RaiseAndSetIfChanged(ref _totalSetsLogged, value);
        }

        public DateTime LastWorkout
        {
            get => _lastWorkout;
            set => this.RaiseAndSetIfChanged(ref _lastWorkout, value);
        }

        public List<string> AvailableExercises { get; private set; } = new();

        // Data for charts
        public List<(DateTime Date, double E1RM)> E1RMData { get; private set; } = new();
        public List<(DateTime Date, double Volume)> VolumeData { get; private set; } = new();
        public List<(DateTime Date, double MaxWeight)> MaxWeightData { get; private set; } = new();

        public StrengthProgressViewModel()
        {
            LoadAvailableExercises();
            LoadExerciseData();
        }

        private void LoadAvailableExercises()
        {
            try
            {
                using var db = new HealthOptimizerDbContext();

                AvailableExercises = db.Exercises
                    .OrderBy(e => e.Name)
                    .Select(e => e.Name)
                    .ToList();

                if (!AvailableExercises.Any())
                {
                    AvailableExercises = new List<string> { "No exercises logged yet" };
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading exercises: {ex.Message}";
            }
        }

        private void LoadExerciseData()
        {
            try
            {
                using var db = new HealthOptimizerDbContext();

                var exercise = db.Exercises.FirstOrDefault(e => e.Name == SelectedExercise);
                if (exercise == null)
                {
                    StatusMessage = "Exercise not found. Log some workouts first!";
                    return;
                }

                // Get all sets for this exercise
                var sets = db.WorkoutSets
                    .Where(s => s.ExerciseId == exercise.Id)
                    .OrderBy(s => s.Session.Date)
                    .Select(s => new
                    {
                        s.Session.Date,
                        s.Weight,
                        s.Reps,
                        s.SetNumber
                    })
                    .ToList();

                if (!sets.Any())
                {
                    StatusMessage = $"No data for {SelectedExercise} yet";
                    return;
                }

                TotalSetsLogged = sets.Count;
                LastWorkout = sets.Max(s => s.Date);

                // Calculate e1RM for each set
                var e1rmByDate = sets
                    .Select(s => new
                    {
                        s.Date,
                        E1RM = CalculateE1RM(s.Weight, s.Reps)
                    })
                    .GroupBy(s => s.Date.Date)
                    .Select(g => (
                        Date: g.Key,
                        E1RM: g.Max(x => x.E1RM) // Best e1RM for that day
                    ))
                    .OrderBy(x => x.Date)
                    .ToList();

                E1RMData = e1rmByDate;

                // Current e1RM (from last workout)
                CurrentE1RM = e1rmByDate.Last().E1RM;

                // All-time PR
                AllTimePR = e1rmByDate.Max(x => x.E1RM);

                // Calculate percent change (last 8 weeks vs previous 8 weeks)
                var eightWeeksAgo = DateTime.Now.AddDays(-56);
                var sixteenWeeksAgo = DateTime.Now.AddDays(-112);

                var recent = e1rmByDate.Where(x => x.Date >= eightWeeksAgo).ToList();
                var previous = e1rmByDate.Where(x => x.Date >= sixteenWeeksAgo && x.Date < eightWeeksAgo).ToList();

                if (recent.Any() && previous.Any())
                {
                    var recentAvg = recent.Average(x => x.E1RM);
                    var previousAvg = previous.Average(x => x.E1RM);
                    PercentChange = Math.Round(((recentAvg - previousAvg) / previousAvg) * 100, 1);
                }
                else if (e1rmByDate.Count >= 2)
                {
                    // If not enough data for 16 weeks, compare first vs last
                    var first = e1rmByDate.First().E1RM;
                    var last = e1rmByDate.Last().E1RM;
                    PercentChange = Math.Round(((last - first) / first) * 100, 1);
                }

                // Calculate volume per workout (sets × reps × weight)
                var volumeByDate = sets
                    .GroupBy(s => s.Date.Date)
                    .Select(g => (
                        Date: g.Key,
                        Volume: g.Sum(s => s.Weight * s.Reps)
                    ))
                    .OrderBy(x => x.Date)
                    .ToList();

                VolumeData = volumeByDate;

                // Max weight lifted per workout
                var maxWeightByDate = sets
                    .GroupBy(s => s.Date.Date)
                    .Select(g => (
                        Date: g.Key,
                        MaxWeight: g.Max(s => s.Weight)
                    ))
                    .OrderBy(x => x.Date)
                    .ToList();

                MaxWeightData = maxWeightByDate;

                StatusMessage = $"Loaded {TotalSetsLogged} sets across {e1rmByDate.Count} workouts";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
        }

        private double CalculateE1RM(double weight, int reps)
        {
            if (reps == 1) return weight;
            return Math.Round(weight * (1 + reps / 30.0), 1);
        }
    }
}