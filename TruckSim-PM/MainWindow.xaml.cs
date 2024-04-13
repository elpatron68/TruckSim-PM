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
            InitializeComponent();
            List<PlayerProfile> playerprofiles = new();
            playerprofiles.AddRange(PlayerProfile.GetEtsProfiles(game:"ets"));
            playerprofiles.AddRange(PlayerProfile.GetEtsProfiles(game: "ats"));
            if(playerprofiles.Count == 0) 
            { 
                throw new NotImplementedException();
            }

        }
    }
}