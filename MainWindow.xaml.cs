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
            LoadJsonData();
            LoadData();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveData();
        }

        public void SaveData()
        {

        }

        public void LoadData()
        {

        }

        public async Task LoadJsonData()
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


        public void FilterServersAndShowListBox()
        {
            var RegionComboboxVal = RegionCombobox.SelectedItem;
            var TypeComboboxVal = TypeCombobox.SelectedItem;
            var MapComboboxVal = MapCombobox.SelectedItem;
            FilterListBoxItems.Items.Clear();

            var filteredServers = this.ServersFiltered.Where(server =>
            (RegionComboboxVal == null || server.Name.Split('-')[0] == RegionComboboxVal.ToString()) &&
            (TypeComboboxVal == null || server.ClusterID == TypeComboboxVal.ToString()) &&
            (MapComboboxVal == null || server.MapName == MapComboboxVal.ToString()) &&
            (SearchTextBox == null || server.Name.Contains(SearchTextBox.Text))
                ).ToList();

            //var sortedServers = ListB

            foreach (var server in filteredServers)
            {
                FilterListBoxItems.Items.Add(server.Name);
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

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedServer = FilterListBoxItems.SelectedItem;
            if (selectedServer != null)
            {   
                if (!SaveLoadListBoxItems.Items.Contains(selectedServer))
                    SaveLoadListBoxItems.Items.Add(selectedServer);
                else
                {
                    MessageBox.Show("Server already saved");
                }
            }
            else
            {
                MessageBox.Show("Select a server from Server Filter list to save");
            }
        }
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            RegionCombobox.SelectedIndex = -1;
            TypeCombobox.SelectedIndex = -1;
            MapCombobox.SelectedIndex = -1;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedServer = SaveLoadListBoxItems.SelectedItem;
            if (selectedServer != null) 
            { 
                SaveLoadListBoxItems.Items.Remove(selectedServer);
            }
            else
            {
                MessageBox.Show("Select a server to delete from saved list");
            }

        }

        private void SaveLoadListBoxItems_GotFocus(object sender, RoutedEventArgs e)
        {
            FilterListBoxItems.UnselectAll();
        }
        private void FilterListBoxItems_GotFocus(object sender, RoutedEventArgs e)
        {
            SaveLoadListBoxItems.UnselectAll();
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Foreground = Brushes.Black;
        }
        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Foreground = Brushes.Gray;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterServersAndShowListBox();
        }
    }
}