using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace HealthOptimizer.Services
{
    public class UpdateService
    {
        private const string GITHUB_API_URL = "https://api.github.com/repos/ColtStraven/HealthOptimizer/releases/latest";
        private const string CURRENT_VERSION = "1.1.1"; // Update this with each release
        private readonly HttpClient _httpClient;

        public UpdateService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "HealthOptimizer");
        }

        public async Task<UpdateInfo?> CheckForUpdatesAsync()
        {
            try
            {
                Console.WriteLine($"Checking for updates at: {GITHUB_API_URL}");
                Console.WriteLine($"Current version: {CURRENT_VERSION}");

                var response = await _httpClient.GetStringAsync(GITHUB_API_URL);
                Console.WriteLine("Successfully fetched release info from GitHub");

                var release = JsonSerializer.Deserialize<GitHubRelease>(response);

                if (release == null)
                {
                    Console.WriteLine("Failed to parse release JSON");
                    return null;
                }

                var latestVersion = release.tag_name.TrimStart('v');
                Console.WriteLine($"Latest version on GitHub: {latestVersion}");

                if (IsNewerVersion(latestVersion, CURRENT_VERSION))
                {
                    Console.WriteLine($"Update available: {latestVersion} > {CURRENT_VERSION}");

                    var assetUrl = GetAssetUrlForPlatform(release);

                    if (assetUrl != null)
                    {
                        Console.WriteLine($"Found download URL: {assetUrl}");
                        return new UpdateInfo
                        {
                            Version = latestVersion,
                            DownloadUrl = assetUrl,
                            ReleaseNotes = release.body,
                            ReleaseDate = release.published_at
                        };
                    }
                    else
                    {
                        Console.WriteLine("No compatible asset found for this platform");
                    }
                }
                else
                {
                    Console.WriteLine("Already running latest version");
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking for updates: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return null;
            }
        }

        private string? GetAssetUrlForPlatform(GitHubRelease release)
        {
            if (release.assets == null || release.assets.Length == 0)
                return null;

            string platformIdentifier;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                platformIdentifier = "Setup"; // Look for installer
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                platformIdentifier = "linux-x64";
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                platformIdentifier = "osx-x64";
            else
                return null;

            foreach (var asset in release.assets)
            {
                if (asset.name.Contains(platformIdentifier, StringComparison.OrdinalIgnoreCase) &&
                    asset.name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                {
                    return asset.browser_download_url;
                }
            }

            // Fallback to ZIP if installer not found
            platformIdentifier = "win-x64";
            foreach (var asset in release.assets)
            {
                if (asset.name.Contains(platformIdentifier, StringComparison.OrdinalIgnoreCase))
                {
                    return asset.browser_download_url;
                }
            }

            return null;
        }

        private bool IsNewerVersion(string latestVersion, string currentVersion)
        {
            var latest = ParseVersion(latestVersion);
            var current = ParseVersion(currentVersion);

            if (latest.Major > current.Major) return true;
            if (latest.Major < current.Major) return false;

            if (latest.Minor > current.Minor) return true;
            if (latest.Minor < current.Minor) return false;

            if (latest.Patch > current.Patch) return true;

            return false;
        }

        private (int Major, int Minor, int Patch) ParseVersion(string version)
        {
            var parts = version.Split('.');
            return (
                parts.Length > 0 ? int.Parse(parts[0]) : 0,
                parts.Length > 1 ? int.Parse(parts[1]) : 0,
                parts.Length > 2 ? int.Parse(parts[2]) : 0
            );
        }

        public async Task<bool> DownloadUpdateAsync(string downloadUrl, string destinationPath, IProgress<int>? progress = null)
        {
            try
            {
                using var response = await _httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                var totalBytes = response.Content.Headers.ContentLength ?? 0;
                var downloadedBytes = 0L;

                using var contentStream = await response.Content.ReadAsStreamAsync();
                using var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

                var buffer = new byte[8192];
                int bytesRead;

                while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                    downloadedBytes += bytesRead;

                    if (totalBytes > 0)
                    {
                        var percentage = (int)((downloadedBytes * 100) / totalBytes);
                        progress?.Report(percentage);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading update: {ex.Message}");
                return false;
            }
        }

        public void InstallUpdate(string installerPath)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    // Launch installer and exit current app
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = installerPath,
                        UseShellExecute = true
                    });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                         RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    // Make executable and run
                    Process.Start("chmod", $"+x \"{installerPath}\"");
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = installerPath,
                        UseShellExecute = true
                    });
                }

                // Exit the current application
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error installing update: {ex.Message}");
            }
        }
    }

    public class UpdateInfo
    {
        public string Version { get; set; } = string.Empty;
        public string DownloadUrl { get; set; } = string.Empty;
        public string ReleaseNotes { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
    }

    // JSON models for GitHub API
    public class GitHubRelease
    {
        public string tag_name { get; set; } = string.Empty;
        public string body { get; set; } = string.Empty;
        public DateTime published_at { get; set; }
        public GitHubAsset[]? assets { get; set; }
    }

    public class GitHubAsset
    {
        public string name { get; set; } = string.Empty;
        public string browser_download_url { get; set; } = string.Empty;
    }
}