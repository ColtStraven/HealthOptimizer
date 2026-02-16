using System;
using System.Collections.Generic;
using System.Text;

namespace HealthOptimizer.Models
{
    public class WorkoutSession
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string WorkoutType { get; set; } = string.Empty; // Push, Pull, Legs, etc.
        public int? OverallRPE { get; set; } // 1-10
        public int? FatigueLevel { get; set; } // 1-10
        public string Notes { get; set; } = string.Empty;

        // Navigation property
        public List<WorkoutSet> Sets { get; set; } = new();
    }
}
