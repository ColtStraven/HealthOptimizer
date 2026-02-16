using System;
using System.Collections.Generic;
using System.Text;

namespace HealthOptimizer.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public double HeightInches { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
