using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;

namespace TruckSim_PM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            List<PlayerProfile> playerprofiles = new();
            playerprofiles.AddRange(PlayerProfile.GetEtsProfiles(game:"ets"));
            playerprofiles.AddRange(PlayerProfile.GetEtsProfiles(game: "ats"));
            if(playerprofiles.Count == 0) 
            { 
                throw new NotImplementedException();
            }
            InitializeComponent();
            dgProfiles.ItemsSource = playerprofiles;
        }

        private void dgProfiles_Loaded(object sender, RoutedEventArgs e)
        {
            dgProfiles.Columns[0].Visibility = Visibility.Collapsed;
            dgProfiles.Columns[3].Visibility = Visibility.Collapsed;
            dgProfiles.Columns[4].Header = "Sim";
        }

        private void Copyprofile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Deleteprofile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Backupprofile_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}