# Health Optimizer

A comprehensive health and fitness tracking application built with C# and Avalonia UI. Track your nutrition, workouts, blood pressure, and body measurements while discovering optimal ranges for your unique physiology through advanced correlation analysis.

![Health Optimizer](screenshot-placeholder.png)

## 🌟 Features

### Data Tracking
- **Daily Log** - Track calories, macros (protein, carbs, fats), weight, and daily steps
- **Blood Pressure** - Monitor BP readings with automatic health category classification
- **Workout Logger** - Log lifting sessions with sets, reps, weight, and RPE
- **Body Measurements** - Track waist, chest, arms, thighs, and more for body recomposition

### Analytics & Visualizations
- **Dashboard** - Visual trends for weight, blood pressure, and carb intake
- **Carbs vs BP Analysis** - Find your optimal carb range for healthy blood pressure
- **Strength Progress** - Track estimated 1RM and progressive overload for every exercise
- **Multi-Variable Analysis** - Advanced correlations:
  - Protein vs Strength gains
  - Calories vs Weight trends
  - Combined optimization for body recomposition

### Smart Insights
- Automatic correlation calculations using statistical analysis
- Personalized "sweet spot" recommendations based on YOUR data
- Body recomp status tracking (gaining strength while losing fat)
- Trend lines and predictive analysis

## 🎨 User Interface

Modern, professional design with:
- Sidebar navigation for easy access to all features
- Card-based layouts with proper spacing
- Interactive charts powered by ScottPlot
- Clean, intuitive user experience
- Large, spacious window (1400x900)

## 🛠️ Technology Stack

- **Framework:** .NET 8.0
- **UI:** Avalonia UI 11.x (Cross-platform)
- **Database:** SQLite with Entity Framework Core
- **Charting:** ScottPlot.Avalonia
- **Statistics:** MathNet.Numerics
- **Architecture:** MVVM Pattern

## 📋 Requirements

- .NET 8.0 SDK or later
- Windows 10/11, macOS, or Linux
- ~50MB disk space for application and database

## 🚀 Getting Started

### Installation

1. Clone the repository:
```bash
git clone https://github.com/ColtStraven/HealthOptimizer.git
cd HealthOptimizer
```

2. Restore NuGet packages:
```bash
dotnet restore
```

3. Build the application:
```bash
dotnet build
```

4. Run the application:
```bash
dotnet run
```

### First-Time Setup

1. Start logging your daily nutrition in the **Daily Log** tab
2. Record blood pressure readings 3x per week in the **Blood Pressure** tab
3. Log your workouts in the **Workout Logger** tab
4. Take body measurements weekly in the **Body Measurements** tab
5. After 2-3 weeks of consistent logging, check the analytics tabs for insights!

## 📊 How It Works

### Finding Your Sweet Spots

**Carbs vs Blood Pressure:**
- Tracks correlation between daily carb intake and BP readings
- Identifies the carb range where your BP stays in the healthy zone (<120/80)
- Provides personalized recommendations

**Protein vs Strength:**
- Analyzes weekly protein intake vs strength gains
- Shows optimal protein range for maximum muscle building
- Helps avoid over-consuming protein unnecessarily

**Calories vs Weight:**
- Tracks relationship between calorie intake and weight trends
- Identifies maintenance calories and optimal deficit/surplus
- Supports body recomposition goals

### Body Recomposition Tracking

The app monitors three key indicators:
1. **Strength trending up** (building/maintaining muscle)
2. **Weight trending down** (losing overall mass)
3. **Waist measurement decreasing** (losing fat specifically)

When strength increases while waist/weight decreases = **successful recomp!** 🎉

## 📁 Database Location

Your data is stored locally in:
```
~/Documents/HealthOptimizer/healthoptimizer.db
```

All data stays on your machine - no cloud services, complete privacy.

## 🔧 Configuration

The app works out-of-the-box with sensible defaults. No configuration needed!

## 📸 Screenshots

### Dashboard
*Main dashboard showing key metrics and trends*

### Carbs vs BP Analysis
*Correlation analysis showing your optimal carb range*

### Strength Progress
*Track estimated 1RM progression for any exercise*

### Multi-Variable Analysis
*Advanced insights combining all variables*

## 🚀 Releases & Updates

### Creating a Release

1. Update version in `UpdateService.cs`:
```csharp
private const string CURRENT_VERSION = "1.1.0"; // Update this
```

2. Build for multiple platforms:
```bash
# Windows
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true

# Linux
dotnet publish -c Release -r linux-x64 --self-contained -p:PublishSingleFile=true

# macOS
dotnet publish -c Release -r osx-x64 --self-contained -p:PublishSingleFile=true
```

3. Create ZIP files:
   - `HealthOptimizer-v1.1.0-win-x64.zip`
   - `HealthOptimizer-v1.1.0-linux-x64.zip`
   - `HealthOptimizer-v1.1.0-osx-x64.zip`

4. Create GitHub Release:
   - Go to Releases → Draft a new release
   - Tag: `v1.1.0`
   - Title: `Version 1.1.0`
   - Attach all ZIP files
   - Publish release

### Auto-Update

The app automatically checks for updates on startup and notifies users when a new version is available. Users can choose to update immediately or continue using the current version.

## 🤝 Contributing

This is a personal project, but suggestions and bug reports are welcome!

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙏 Acknowledgments

- Built with [Avalonia UI](https://avaloniaui.net/) - Cross-platform .NET UI framework
- Charts powered by [ScottPlot](https://scottplot.net/)
- Statistics using [MathNet.Numerics](https://numerics.mathdotnet.com/)
- Database management with [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

## 📧 Contact

Project Link: [https://github.com/ColtStraven/HealthOptimizer](https://github.com/ColtStraven/HealthOptimizer)

## 🎯 Roadmap

Potential future features:
- [ ] Dark mode toggle
- [ ] Export reports to PDF/CSV
- [ ] Progress photos with timeline
- [ ] Cloud backup and sync
- [ ] Mobile companion app
- [ ] Meal planning integration
- [ ] Apple Health / Google Fit integration

## ⚠️ Disclaimer

This application is for informational and tracking purposes only. It is not intended to diagnose, treat, cure, or prevent any disease. Always consult with a qualified healthcare provider before making changes to your diet, exercise routine, or if you have concerns about your health, especially regarding blood pressure or cardiovascular health.

## Changelog

### v1.1.1 (February 2026)
**New Features:**
- ✨ Added comprehensive bodyweight exercise library (50+ exercises)
- 💪 Organized by movement patterns:
  - Push — Horizontal (7 progressions)
  - Push — Vertical (7 progressions)
  - Pull — Horizontal (6 progressions)
  - Pull — Vertical (8 progressions)
  - Squat & Hinge (8 variations)
  - Core (7 exercises)
- 🎯 Auto-detection of bodyweight exercises (weight set to 0)

**Improvements:**
- Enhanced exercise dropdown with full calisthenics progression paths
- Better categorization of exercises by movement type

### v1.1.0 (February 2026)
**New Features:**
- ✨ Added bodyweight exercise support to Workout Logger
- 💪 Expanded exercise library with 15+ bodyweight movements
- 🎯 Auto-detection of bodyweight exercises
- 📊 Bodyweight exercises tracked by reps when weight is 0

**Improvements:**
- Enhanced exercise dropdown with categorized exercises
- Better handling of bodyweight movements in strength progress charts

### v1.0.0 (February 2026)
- 🎉 Initial release with all core features

---

**Built with ❤️ for personal health optimization**
```

**Save it (Ctrl+S).**

**Now let's also create a LICENSE file:**

**Right-click on project → Add → New Item → Text File**
- Name it: **"LICENSE"**

**Paste this (MIT License):**
```
MIT License

Copyright (c) 2026 Colt Straven

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
