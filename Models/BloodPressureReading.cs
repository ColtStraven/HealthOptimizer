using System;
using System.Collections.Generic;
using System.Text;

namespace HealthOptimizer.Models
{
    public class BloodPressureReading
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public int Systolic { get; set; }
        public int Diastolic { get; set; }
        public int? Pulse { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
