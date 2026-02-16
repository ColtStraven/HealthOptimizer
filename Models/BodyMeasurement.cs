using System;
using System.Collections.Generic;
using System.Text;

namespace HealthOptimizer.Models
{
    public class BodyMeasurement
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double? WaistInches { get; set; }
        public double? ChestInches { get; set; }
        public double? LeftArmInches { get; set; }
        public double? RightArmInches { get; set; }
        public double? LeftThighInches { get; set; }
        public double? RightThighInches { get; set; }
        public double? NeckInches { get; set; }
        public double? HipsInches { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
