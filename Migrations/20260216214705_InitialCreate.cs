using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthOptimizer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BloodPressureReadings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Systolic = table.Column<int>(type: "INTEGER", nullable: false),
                    Diastolic = table.Column<int>(type: "INTEGER", nullable: false),
                    Pulse = table.Column<int>(type: "INTEGER", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BloodPressureReadings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BodyMeasurements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    WaistInches = table.Column<double>(type: "REAL", nullable: true),
                    ChestInches = table.Column<double>(type: "REAL", nullable: true),
                    LeftArmInches = table.Column<double>(type: "REAL", nullable: true),
                    RightArmInches = table.Column<double>(type: "REAL", nullable: true),
                    LeftThighInches = table.Column<double>(type: "REAL", nullable: true),
                    RightThighInches = table.Column<double>(type: "REAL", nullable: true),
                    NeckInches = table.Column<double>(type: "REAL", nullable: true),
                    HipsInches = table.Column<double>(type: "REAL", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodyMeasurements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DailyLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Weight = table.Column<double>(type: "REAL", nullable: false),
                    Calories = table.Column<int>(type: "INTEGER", nullable: false),
                    ProteinGrams = table.Column<double>(type: "REAL", nullable: false),
                    CarbsGrams = table.Column<double>(type: "REAL", nullable: false),
                    FatsGrams = table.Column<double>(type: "REAL", nullable: false),
                    Steps = table.Column<int>(type: "INTEGER", nullable: false),
                    EnergyLevel = table.Column<int>(type: "INTEGER", nullable: true),
                    SleepHours = table.Column<double>(type: "REAL", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Exercises",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    MuscleGroup = table.Column<string>(type: "TEXT", nullable: false),
                    MovementPattern = table.Column<string>(type: "TEXT", nullable: false),
                    Equipment = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercises", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Age = table.Column<int>(type: "INTEGER", nullable: false),
                    HeightInches = table.Column<double>(type: "REAL", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    EndTime = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    WorkoutType = table.Column<string>(type: "TEXT", nullable: false),
                    OverallRPE = table.Column<int>(type: "INTEGER", nullable: true),
                    FatigueLevel = table.Column<int>(type: "INTEGER", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    ExerciseId = table.Column<int>(type: "INTEGER", nullable: false),
                    SetNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Reps = table.Column<int>(type: "INTEGER", nullable: false),
                    Weight = table.Column<double>(type: "REAL", nullable: false),
                    RPE = table.Column<int>(type: "INTEGER", nullable: true),
                    IsWarmup = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsFailure = table.Column<bool>(type: "INTEGER", nullable: false),
                    RestSeconds = table.Column<int>(type: "INTEGER", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutSets_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkoutSets_WorkoutSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "WorkoutSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BloodPressureReadings_DateTime",
                table: "BloodPressureReadings",
                column: "DateTime");

            migrationBuilder.CreateIndex(
                name: "IX_BodyMeasurements_Date",
                table: "BodyMeasurements",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_DailyLogs_Date",
                table: "DailyLogs",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSessions_Date",
                table: "WorkoutSessions",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSets_ExerciseId",
                table: "WorkoutSets",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSets_SessionId",
                table: "WorkoutSets",
                column: "SessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BloodPressureReadings");

            migrationBuilder.DropTable(
                name: "BodyMeasurements");

            migrationBuilder.DropTable(
                name: "DailyLogs");

            migrationBuilder.DropTable(
                name: "UserProfiles");

            migrationBuilder.DropTable(
                name: "WorkoutSets");

            migrationBuilder.DropTable(
                name: "Exercises");

            migrationBuilder.DropTable(
                name: "WorkoutSessions");
        }
    }
}
