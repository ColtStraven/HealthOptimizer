using HealthOptimizer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace HealthOptimizer.Data
{
    public class HealthOptimizerDbContext : DbContext
    {
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<DailyLog> DailyLogs { get; set; }
        public DbSet<BloodPressureReading> BloodPressureReadings { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<WorkoutSession> WorkoutSessions { get; set; }
        public DbSet<WorkoutSet> WorkoutSets { get; set; }
        public DbSet<BodyMeasurement> BodyMeasurements { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Get the path to the user's Documents folder
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string dbPath = Path.Combine(documentsPath, "HealthOptimizer", "healthoptimizer.db");

            // Create directory if it doesn't exist
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<WorkoutSet>()
                .HasOne(ws => ws.Session)
                .WithMany(s => s.Sets)
                .HasForeignKey(ws => ws.SessionId);

            modelBuilder.Entity<WorkoutSet>()
                .HasOne(ws => ws.Exercise)
                .WithMany(e => e.Sets)
                .HasForeignKey(ws => ws.ExerciseId);

            // Configure indexes for better query performance
            modelBuilder.Entity<DailyLog>()
                .HasIndex(d => d.Date);

            modelBuilder.Entity<BloodPressureReading>()
                .HasIndex(b => b.DateTime);

            modelBuilder.Entity<WorkoutSession>()
                .HasIndex(w => w.Date);

            modelBuilder.Entity<BodyMeasurement>()
                .HasIndex(b => b.Date);
        }
    }
}