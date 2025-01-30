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
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Ark_Tools
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    class Server
    {
        public string Name { get; set; }
        public string MapName { get; set; }
        public string ClusterID { get; set; }

    }

    public partial class MainWindow : Window
    {
        private List<Server> ServersFiltered = new List<Server>();
        public MainWindow()
        {
            InitializeComponent();
            LoadData();
        }

        public async Task LoadData()
        {
            string url = "https://cdn2.arkdedicated.com/servers/asa/officialserverlist.json";
            try
            {
                using HttpClient client = new HttpClient();
                List<Server> jsonData = await client.GetFromJsonAsync<List<Server>>(url);
                string[] excludeServers = { "Modded", "Expire", "Club", "SOTF", "Console", "QA", "Arkpocalypse", "Conquest", "Isolated", "Dev" };
                if (jsonData != null)
                {
                    foreach (var server in jsonData)
                    {
                        if (!excludeServers.Any(excludeWord => server.Name.Contains(excludeWord)))
                        {
                            this.ServersFiltered.Add(server);
                        }
                    }
                }

                foreach (var server in this.ServersFiltered)
                {

                    System.Diagnostics.Debug.WriteLine($"Name: {server.Name}, MapName: {server.MapName}, ClusterID: {server.ClusterID}");
                    
                    string region = server.Name.Split('-')[0];
                    string serverID = server.Name.Length >= 4 ? server.Name[^4..] : server.Name;
                    if (!RegionCombobox.Items.Contains(region))
                    {
                        RegionCombobox.Items.Add(region);
                    }
                    if (!TypeCombobox.Items.Contains(server.ClusterID))
                    {
                        TypeCombobox.Items.Add(server.ClusterID);
                    }
                    if (!MapCombobox.Items.Contains(server.MapName))
                    {
                        MapCombobox.Items.Add(server.MapName);
                    }
                }
                FilterServersAndShowListBox();
                System.Diagnostics.Debug.WriteLine($"skib { jsonData}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("CLICKED");
        }
        public void FilterServersAndShowListBox()
        {
            var RegionComboboxVal = RegionCombobox.SelectedItem;
            var TypeComboboxVal = TypeCombobox.SelectedItem;
            var MapComboboxVal = MapCombobox.SelectedItem;
            ListBoxItems.Items.Clear();

            var filteredServers = this.ServersFiltered.Where(server =>
            (RegionComboboxVal == null || server.Name.Split('-')[0] == RegionComboboxVal.ToString()) &&
            (TypeComboboxVal == null || server.ClusterID == TypeComboboxVal.ToString()) &&
            (MapComboboxVal == null || server.MapName == MapComboboxVal.ToString())
                ).ToList();

            foreach (var server in filteredServers)
            {
                ListBoxItems.Items.Add(server.Name);
            }
            return;
        }

        private void RegionCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterServersAndShowListBox();
        }

        private void TypeCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterServersAndShowListBox();
        }

        private void MapCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterServersAndShowListBox();
        }
    }
}