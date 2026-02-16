using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ReactiveUI;
using HealthOptimizer.Models;
using HealthOptimizer.Data;

namespace HealthOptimizer.ViewModels
{
    public class WorkoutLogViewModel : ViewModelBase
    {
        private DateTimeOffset _workoutDate = DateTimeOffset.Now;
        private string _workoutType = "Push";
        private string _selectedExerciseName = string.Empty;
        private int _sets = 3;
        private int _reps = 8;
        private double _weight = 0;
        private int _rpe = 7;
        private string _notes = string.Empty;
        private string _statusMessage = string.Empty;

        public DateTimeOffset WorkoutDate
        {
            get => _workoutDate;
            set
            {
                this.RaiseAndSetIfChanged(ref _workoutDate, value);
                LoadWorkoutForDate();
            }
        }

        public string WorkoutType
        {
            get => _workoutType;
            set => this.RaiseAndSetIfChanged(ref _workoutType, value);
        }

        public string SelectedExerciseName
        {
            get => _selectedExerciseName;
            set => this.RaiseAndSetIfChanged(ref _selectedExerciseName, value);
        }

        public int Sets
        {
            get => _sets;
            set => this.RaiseAndSetIfChanged(ref _sets, value);
        }

        public int Reps
        {
            get => _reps;
            set => this.RaiseAndSetIfChanged(ref _reps, value);
        }

        public double Weight
        {
            get => _weight;
            set => this.RaiseAndSetIfChanged(ref _weight, value);
        }

        public int RPE
        {
            get => _rpe;
            set => this.RaiseAndSetIfChanged(ref _rpe, value);
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

        public ObservableCollection<string> WorkoutTypes { get; } = new()
        {
            "Push", "Pull", "Legs", "Upper", "Lower", "Full Body", "Other"
        };

        public ObservableCollection<string> CommonExercises { get; } = new()
        {
            "Bench Press", "Squat", "Deadlift", "Overhead Press",
            "Barbell Row", "Pull-ups", "Dips", "Lat Pulldown",
            "Leg Press", "Romanian Deadlift", "Lunges",
            "Bicep Curls", "Tricep Extensions", "Lateral Raises",
            "Leg Curls", "Leg Extensions", "Calf Raises"
        };

        public ObservableCollection<WorkoutSetDisplay> WorkoutSets { get; } = new();

        public ICommand LogExerciseCommand { get; }

        public WorkoutLogViewModel()
        {
            LogExerciseCommand = ReactiveCommand.Create(LogExercise);
            LoadWorkoutForDate();
        }

        private void LogExercise()
        {
            if (string.IsNullOrWhiteSpace(SelectedExerciseName))
            {
                StatusMessage = "⚠ Select an exercise first!";
                return;
            }

            if (Weight <= 0)
            {
                StatusMessage = "⚠ Enter a weight!";
                return;
            }

            try
            {
                using var db = new HealthOptimizerDbContext();

                // Get or create session for this date
                var session = db.WorkoutSessions
                    .FirstOrDefault(s => s.Date.Date == WorkoutDate.DateTime.Date);

                if (session == null)
                {
                    session = new WorkoutSession
                    {
                        Date = WorkoutDate.DateTime.Date,
                        WorkoutType = WorkoutType,
                        Notes = Notes
                    };
                    db.WorkoutSessions.Add(session);
                    db.SaveChanges();
                }

                // Find or create exercise
                var exercise = db.Exercises.FirstOrDefault(e => e.Name == SelectedExerciseName);
                if (exercise == null)
                {
                    exercise = new Exercise
                    {
                        Name = SelectedExerciseName,
                        Category = "Compound",
                        MuscleGroup = "Various"
                    };
                    db.Exercises.Add(exercise);
                    db.SaveChanges();
                }

                // Get current set number for this exercise in this session
                var existingSets = db.WorkoutSets
                    .Where(s => s.SessionId == session.Id && s.ExerciseId == exercise.Id)
                    .Count();

                // Add sets
                for (int i = 1; i <= Sets; i++)
                {
                    var workoutSet = new WorkoutSet
                    {
                        SessionId = session.Id,
                        ExerciseId = exercise.Id,
                        SetNumber = existingSets + i,
                        Reps = Reps,
                        Weight = Weight,
                        RPE = RPE,
                        IsWarmup = false,
                        IsFailure = false
                    };

                    db.WorkoutSets.Add(workoutSet);
                }

                db.SaveChanges();

                StatusMessage = $"✓ Logged {Sets}x{Reps} @ {Weight}lbs - {SelectedExerciseName}";
                LoadWorkoutForDate();

                // Reset for next exercise
                Reps = 8;
                Weight = 0;
                SelectedExerciseName = string.Empty;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
        }

        private void LoadWorkoutForDate()
        {
            try
            {
                using var db = new HealthOptimizerDbContext();

                var session = db.WorkoutSessions
                    .FirstOrDefault(s => s.Date.Date == WorkoutDate.DateTime.Date);

                WorkoutSets.Clear();

                if (session != null)
                {
                    var sets = db.WorkoutSets
                        .Where(s => s.SessionId == session.Id)
                        .OrderBy(s => s.Id)
                        .ToList();

                    foreach (var set in sets)
                    {
                        var exercise = db.Exercises.Find(set.ExerciseId);
                        var e1RM = CalculateE1RM(set.Weight, set.Reps);

                        WorkoutSets.Add(new WorkoutSetDisplay
                        {
                            ExerciseName = exercise?.Name ?? "Unknown",
                            SetNumber = set.SetNumber,
                            Reps = set.Reps,
                            Weight = set.Weight,
                            RPE = set.RPE ?? 0,
                            EstimatedOneRM = e1RM
                        });
                    }

                    WorkoutType = session.WorkoutType;
                    Notes = session.Notes;
                    StatusMessage = $"Loaded {WorkoutSets.Count} sets from this workout";
                }
                else
                {
                    StatusMessage = "No workout logged for this date";
                }
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

    public class WorkoutSetDisplay
    {
        public string ExerciseName { get; set; } = string.Empty;
        public int SetNumber { get; set; }
        public int Reps { get; set; }
        public double Weight { get; set; }
        public int RPE { get; set; }
        public double EstimatedOneRM { get; set; }
    }
}