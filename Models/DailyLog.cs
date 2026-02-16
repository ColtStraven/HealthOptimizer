using System;
using System.Collections.Generic;
using System.Text;

namespace HealthOptimizer.Models
{
    public class DailyLog
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double Weight { get; set; }
        public int Calories { get; set; }
        public double ProteinGrams { get; set; }
        public double CarbsGrams { get; set; }
        public double FatsGrams { get; set; }
        public int Steps { get; set; }
        public int? EnergyLevel { get; set; }  // 1-10 scale, nullable
        public double? SleepHours { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
