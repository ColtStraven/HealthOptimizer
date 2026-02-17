using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using HealthOptimizer.Services;

namespace HealthOptimizer.Views
{
    public partial class UpdateWindow : Window
    {
        private readonly UpdateService _updateService;
        private readonly UpdateInfo _updateInfo;

        public UpdateWindow(UpdateInfo updateInfo)
        {
            InitializeComponent();
            _updateService = new UpdateService();
            _updateInfo = updateInfo;

            LoadUpdateInfo();
        }

        private void LoadUpdateInfo()
        {
            this.FindControl<TextBlock>("VersionText")!.Text = $"Version {_updateInfo.Version} is available!";
            this.FindControl<TextBlock>("ReleaseDateText")!.Text = $"Released: {_updateInfo.ReleaseDate:MMMM dd, yyyy}";
            this.FindControl<TextBlock>("ReleaseNotesText")!.Text = _updateInfo.ReleaseNotes;
        }

        private async void UpdateButton_Click(object? sender, RoutedEventArgs e)
        {
            // Disable buttons
            this.FindControl<Button>("UpdateButton")!.IsEnabled = false;
            this.FindControl<Button>("LaterButton")!.IsEnabled = false;

            // Show progress
            var progressPanel = this.FindControl<StackPanel>("ProgressPanel")!;
            progressPanel.IsVisible = true;

            var progressBar = this.FindControl<ProgressBar>("DownloadProgress")!;
            var progressText = this.FindControl<TextBlock>("ProgressText")!;

            var progress = new Progress<int>(percentage =>
            {
                progressBar.Value = percentage;
                progressText.Text = $"Downloading update... {percentage}%";
            });

            // Download to temp folder
            var tempPath = Path.Combine(Path.GetTempPath(), $"HealthOptimizer-Update-{_updateInfo.Version}.zip");

            progressText.Text = "Downloading update...";
            var success = await _updateService.DownloadUpdateAsync(_updateInfo.DownloadUrl, tempPath, progress);

            if (success)
            {
                progressText.Text = "Download complete! Installing...";

                // Install and restart
                _updateService.InstallUpdate(tempPath);
            }
            else
            {
                progressText.Text = "Download failed. Please try again later.";
                this.FindControl<Button>("UpdateButton")!.IsEnabled = true;
                this.FindControl<Button>("LaterButton")!.IsEnabled = true;
            }
        }

        private void LaterButton_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}