using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RobloxFlagInjector
{
    public class RobloxInstall
    {
        public string VersionFolderName { get; set; } = "";
        public string FullPath { get; set; } = "";
        public bool IsStudio { get; set; }
        public bool IsPlayer { get; set; }
        public string ClientSettingsPath => Path.Combine(FullPath, "ClientSettings");
        public string ClientAppSettingsFile => Path.Combine(ClientSettingsPath, "ClientAppSettings.json");

        public override string ToString()
        {
            var kind = IsStudio ? "Studio" : IsPlayer ? "Player" : "Unknown";
            return $"{VersionFolderName}  [{kind}]";
        }
    }

    public static class RobloxLocator
    {
        /// <summary>
        /// Finds all Roblox version install folders under the standard
        /// %LOCALAPPDATA%\Roblox\Versions directory.
        /// </summary>
        public static List<RobloxInstall> FindInstalls()
        {
            var results = new List<RobloxInstall>();

            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string versionsRoot = Path.Combine(localAppData, "Roblox", "Versions");

            if (!Directory.Exists(versionsRoot))
                return results;

            foreach (var dir in Directory.GetDirectories(versionsRoot))
            {
                bool hasPlayer = File.Exists(Path.Combine(dir, "RobloxPlayerBeta.exe"));
                bool hasStudio = File.Exists(Path.Combine(dir, "RobloxStudioBeta.exe"));

                // Skip folders that aren't actual Roblox binaries (e.g. leftover/empty dirs)
                if (!hasPlayer && !hasStudio)
                    continue;

                results.Add(new RobloxInstall
                {
                    VersionFolderName = Path.GetFileName(dir),
                    FullPath = dir,
                    IsPlayer = hasPlayer,
                    IsStudio = hasStudio
                });
            }

            return results;
        }

        /// <summary>
        /// Ensures the ClientSettings folder exists for a given install.
        /// </summary>
        public static void EnsureClientSettingsFolder(RobloxInstall install)
        {
            if (!Directory.Exists(install.ClientSettingsPath))
                Directory.CreateDirectory(install.ClientSettingsPath);
        }

        /// <summary>
        /// Creates a timestamped backup copy of a file, if it exists.
        /// Returns the backup path, or null if there was nothing to back up.
        /// </summary>
        public static string? BackupFile(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            string backupPath = filePath + $".backup_{DateTime.Now:yyyyMMdd_HHmmss}";
            File.Copy(filePath, backupPath, overwrite: false);
            return backupPath;
        }
    }
}
