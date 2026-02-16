using System;
using System.Collections.Generic;
using System.Text;

namespace HealthOptimizer.Models
{
    public class WorkoutSet
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public int ExerciseId { get; set; }
        public int SetNumber { get; set; }
        public int Reps { get; set; }
        public double Weight { get; set; }
        public int? RPE { get; set; } // 1-10
        public bool IsWarmup { get; set; }
        public bool IsFailure { get; set; }
        public int? RestSeconds { get; set; }
        public string Notes { get; set; } = string.Empty;

        // Navigation properties
        public WorkoutSession Session { get; set; } = null!;
        public Exercise Exercise { get; set; } = null!;
    }
}
