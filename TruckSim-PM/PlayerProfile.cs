
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;

namespace TruckSim_PM
{
    internal class PlayerProfile
    {
        private string? _username;
        private string? _directory;
        private string? _directoryshort;
        private bool _decrypted = false;
        private string? _etsats;
        private DateTime? _lastaccess;

        public string Directory
        {
            get => _directory ?? "none";
            set => _directory = value;
        }

        public string DirectoryShort
        {
            get => _directoryshort ?? "none";
            set => _directoryshort = value;
        }
        public string Username
        {
            get => _username ?? "none";
            set => _username = value;
        }

        public bool Decrypted
        {
            get => _decrypted;
            set => _decrypted = value;
        }

        public string EtsAts
        {
            get => _etsats ?? "none";
            set => _etsats = value;
        }

        public DateTime? LastAccess
        {
            get => _lastaccess ?? null;
            set => _lastaccess = value;
        }

        public static List<PlayerProfile> GetEtsProfiles(string game="ets")
        {
            List<PlayerProfile> pf = new();
            string profiledirectory = string.Empty;
            if (game == "ets")
            {
                profiledirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                @"Euro Truck Simulator 2\profiles");
            }
            else
            {
                profiledirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                @"American Truck Simulator\profiles");
            }
            
            if(System.IO.Directory.Exists(profiledirectory))
            {
                string[] profilesubdirectories = System.IO.Directory.GetDirectories(profiledirectory);
                if(profilesubdirectories.Length > 0)
                {
                    foreach (string subdirectory in profilesubdirectories)
                    {
                        DirectoryInfo di =new(subdirectory);
                        string shortdir = di.Name;
                        if (shortdir.IsHex() & File.Exists(Path.Combine(subdirectory, "profile.sii")))
                        {
                            TimeZoneInfo systemTimeZone = TimeZoneInfo.Local;
                            PlayerProfile p = new()
                            {
                                Directory = subdirectory,
                                DirectoryShort = shortdir,
                                Decrypted = IsDecrypted(Path.Combine(subdirectory, "profile.sii")),
                                EtsAts = game.ToUpper(),
                                Username = subdirectory.DirectoryToScsUsername(),
                                LastAccess = di.LastAccessTime,
                            };
                            pf.Add(p);
                        }
                    }
                }
            }
            return pf;
        }

        public static void CopyProfile(PlayerProfile profile, string newusername)
        {
            string newDirectoryShort = newusername.ScsUsernameToDirectory();

            string profileDirectoryBase;
            if (profile.EtsAts.ToLower() == "ets")
            {
                profileDirectoryBase = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                @"Euro Truck Simulator 2\profiles");
            }
            else
            {
                profileDirectoryBase = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                @"American Truck Simulator\profiles");
            }
            string newDirectoryFull = Path.Combine(profileDirectoryBase, newDirectoryShort);
            if (System.IO.Directory.Exists(newDirectoryFull))
            {
                return;
            }
            else
            {
                // Copy profile directory to new directory
                CopyDirectory(profile.Directory, newDirectoryFull, true);
                if (!profile.Decrypted)
                {
                    bool result = DecryptFile(newDirectoryFull, "profile.sii");
                    if (!result) 
                    {
                        System.IO.Directory.Delete(newDirectoryFull, true);
                        return;
                    }
                }
                string filename = Path.Combine(newDirectoryFull, "profile.sii");
                using StreamReader sr = new(filename);
                string content = sr.ReadToEnd();
                sr.Close();
                if (content.Contains(profile.Username))
                {
                    content = content.Replace(profile.Username, newusername);
                }
                using StreamWriter sw = new StreamWriter(filename, false);
                sw.Write(content);
                sw.Close();
            }
        }

        // Delete a profile directory
        public static bool DeleteProfile(PlayerProfile profile)
        {
            try
            {
                System.IO.Directory.Delete(profile.Directory, true);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Save a profile directory to a zip file
        public static void BackupProfile(PlayerProfile profile, string zipfilename)
        {
            string startPath = profile.Directory;
            string zipPath = zipfilename;
            ZipFile.CreateFromDirectory(startPath, zipPath);
        }

        // Check the first line of a file to detect if it is encrypted or not
        private static bool IsDecrypted(string filename)
        {
            using StreamReader sr = new(filename);
            string text = sr.ReadLine() ?? string.Empty;
            sr.Close();
            return text.Contains("SiiNunit");
        }

        // Use SII_Decrypt.exe for decrypting sii files
        public static bool DecryptFile(string directory, string filename)
        {
            using Process process = new();
            process.StartInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SII_Decrypt.exe");
            process.StartInfo.Arguments = filename;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WorkingDirectory = directory;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            if (output.Contains("Result: File is a plain-text SII (1)") ^
                output.Contains("Result: Success (0)"))
            { return true; }
            else
            { return false; }
        }

        // https://learn.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories
        private static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            DirectoryInfo dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            System.IO.Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }
    }
}
