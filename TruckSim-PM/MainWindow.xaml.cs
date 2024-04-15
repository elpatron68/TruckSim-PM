using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;

namespace TruckSim_PM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private List<PlayerProfile>? profiles;
        public MainWindow()
        {
            InitializeComponent();
            LoadProfiles();
            statusBarText.Text = "Right click on a row to copy, delete or backup a profile.";
        }

        private async void LoadProfiles()
        {
            profiles = [.. PlayerProfile.GetEtsProfiles(game: "ets"),
                .. PlayerProfile.GetEtsProfiles(game: "ats")];

            if (profiles.Count == 0)
            {
                await this.ShowMessageAsync("No profiles found", "No profiles found. You should deactivate Steam Cloud synchronization in the game´s profile settings.");
            }
            dgProfiles.ItemsSource = profiles;
            dgProfiles.Items.Refresh();
            statusBarText.Text = string.Format("{0} profiles found.", profiles.Count);
        }

        private void dgProfiles_Loaded(object sender, RoutedEventArgs e)
        {
            if (profiles.Count > 0)
            {
                UpdateDatagrid();
            }            
        }
 
        private void UpdateDatagrid()
        {
            if (profiles.Count > 0)
            {
                dgProfiles.Columns[0].Visibility = Visibility.Collapsed;
                dgProfiles.Columns[3].Visibility = Visibility.Collapsed;
                dgProfiles.Columns[4].Header = "Sim";
            }
        }

        private async void Copyprofile_Click(object sender, RoutedEventArgs e)
        {
            if (IsTrucksimRunning() == true)
            {
                await this.ShowMessageAsync("Game is running", "You should end the game before copying a profile.");
                statusBarText.Text = "Copy cancelled, game is running.";
                return;
            }
            MenuItem menuItem = (MenuItem)sender;
            ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
            DataGrid item = (DataGrid)contextMenu.PlacementTarget;
            PlayerProfile toCopy = (PlayerProfile)item.SelectedCells[0].Item;
            string newusername = await this.ShowInputAsync("New User Name", "Enter your new user name (must be changed):") ?? string.Empty;

            if (newusername == null ^
                newusername == toCopy.Username ^
                newusername == string.Empty)
            {
                // cancel
                statusBarText.Text = string.Format("Copying canceled.");
                return;
            }
            else if(newusername != null)
            {
                PlayerProfile.CopyProfile(toCopy, newusername);
                //if (dgProfiles.ItemsSource != null)
                //{
                //    dgProfiles.ItemsSource = null;
                //}
                LoadProfiles();
                UpdateDatagrid();
                statusBarText.Text = string.Format("Profile {0} copied to {1}.", 
                    toCopy.DirectoryShort, newusername.ScsUsernameToDirectory());
            }
        }

        private async void Deleteprofile_Click(object sender, RoutedEventArgs e)
        {
            if (IsTrucksimRunning() == true)
            {
                await this.ShowMessageAsync("Game is running", "You should end the game before deleting a profile.");
                statusBarText.Text = "Delete profile cancelled, game is running.";
                return;
            }
            MenuItem menuItem = (MenuItem)sender;
            ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
            DataGrid item = (DataGrid)contextMenu.PlacementTarget;
            PlayerProfile toDelete = (PlayerProfile)item.SelectedCells[0].Item;

            MessageDialogResult result = await this.ShowMessageAsync("ARE YOU SURE?",
                string.Format("Are you shure to delete the profile of user {0}?", 
                    toDelete.Username), MessageDialogStyle.AffirmativeAndNegative);

            if(result == MessageDialogResult.Affirmative)
            {
                bool delresult = PlayerProfile.DeleteProfile(toDelete);
                if (delresult == false)
                {
                    await this.ShowMessageAsync("Delete failed", "Deleting the profile failed. Maybe you have any file(s) openened, " + 
                        "e.g. in a text editor or the directory is locked by another process.?");
                    return;
                }
                LoadProfiles();
                UpdateDatagrid();
                statusBarText.Text = string.Format("Profile {0} deleted.",
                    toDelete.DirectoryShort);
            }
            else
            {
                statusBarText.Text = "Deleting a profile cancelled.";
            }
        }

        private async void Backupprofile_Click(object sender, RoutedEventArgs e)
        {
            if (IsTrucksimRunning() == true)
            {
                await this.ShowMessageAsync("Game is running", "You should end the game before copying a profile.");
                statusBarText.Text = "Backup cancelled, game is running.";
                return;
            }
            MenuItem menuItem = (MenuItem)sender;
            ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
            DataGrid item = (DataGrid)contextMenu.PlacementTarget;
            PlayerProfile profile = (PlayerProfile)item.SelectedCells[0].Item;

            string profiledirectory;
            if (profile.EtsAts.ToLower() == "ets")
            {
                profiledirectory = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                @"Euro Truck Simulator 2\profiles");
            }
            else
            {
                profiledirectory = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                @"American Truck Simulator\profiles");
            }

            // Configure save file dialog box
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                FileName = string.Format("{0}-{1}.zip", profile.DirectoryShort, DateTime.Now.ToString("yyyy-dd-M_HH-mm-ss")),
                DefaultExt = ".zip", // Default file extension
                Filter = "Zip files (.zip)|*.zip", // Filter files by extension
                Title = "Select directory and file name",
                InitialDirectory = profiledirectory
            };

            // Show save file dialog box
            bool? result = saveFileDialog.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                string filename = saveFileDialog.FileName;
                if (filename != null)
                {
                    PlayerProfile.BackupProfile(profile, filename);
                    statusBarText.Text = string.Format("Profile {0} saved to {1}.", 
                        profile.DirectoryShort, filename);
                }
                else
                {
                    statusBarText.Text = "Backup canceled, empty file name.";
                }
            }
            else
            {
                statusBarText.Text = "Backup canceled.";
            }
        }

        private void Openpath_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
            DataGrid item = (DataGrid)contextMenu.PlacementTarget;
            PlayerProfile profile = (PlayerProfile)item.SelectedCells[0].Item;
            Process.Start("explorer.exe", profile.Directory);
            statusBarText.Text = string.Format("Started Explorer in profile {0}.", 
                profile.DirectoryShort);
        }

        private void Decryptprofile_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
            DataGrid item = (DataGrid)contextMenu.PlacementTarget;
            PlayerProfile profile = (PlayerProfile)item.SelectedCells[0].Item;
            _ = PlayerProfile.DecryptFile(profile.Directory, "profile.sii");
            LoadProfiles();
            UpdateDatagrid();
            statusBarText.Text = string.Format("Profile {0} decrypted.", 
                profile.DirectoryShort);
        }

        private bool IsTrucksimRunning()
        {
            Process[] localAll = Process.GetProcesses();
            foreach (Process p in localAll)
            {
                if(p.ProcessName.ToLower().Contains("eurotrucks2") ^ p.ProcessName.ToLower().Contains("amtrucks"))
                {
                    return true;
                }
            }
            return false;
        }
    }
}