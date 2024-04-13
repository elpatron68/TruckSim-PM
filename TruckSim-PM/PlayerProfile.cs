using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace TruckSim_PM
{
    internal class PlayerProfile
    {
        private string? _username;
        private string? _directory;
        private bool _decrypted = false;
        private string? _etsats;
        public string Directory
        {
            get => _directory ?? "none";
            set => _directory = value;
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
            
            string[] profilesubdirectories = System.IO.Directory.GetDirectories(profiledirectory);
            foreach (string subdirectory in profilesubdirectories)
            {
                PlayerProfile p = new()
                {
                    Directory = subdirectory,
                    Decrypted = false,
                    EtsAts = game.ToUpper(),
                    Username = subdirectory.DirectoryToScsUsername()
                };
                pf.Add(p);
            }
            return pf;
        }
    }
}
