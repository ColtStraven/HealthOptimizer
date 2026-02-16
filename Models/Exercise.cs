using System;
using System.Collections.Generic;
using System.Text;

namespace HealthOptimizer.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // Compound, Isolation
        public string MuscleGroup { get; set; } = string.Empty; // Chest, Back, Legs, etc.
        public string MovementPattern { get; set; } = string.Empty; // Push, Pull, Hinge, Squat
        public string Equipment { get; set; } = string.Empty; // Barbell, Dumbbell, Machine, Bodyweight

        // Navigation property
        public List<WorkoutSet> Sets { get; set; } = new();
    }
}
