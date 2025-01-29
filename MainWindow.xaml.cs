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
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Ark_Tools
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public Array EUServers { get; set; }
        public Array NAServers { get; set; }
        public Array ASIAServers { get; set; }
        public Array OCServers { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            FetchAndStoreData();
        }

        public async Task FetchAndStoreData()
        {
            string url = "https://cdn2.arkdedicated.com/servers/asa/officialserverlist.json";
            string rootPath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = System.IO.Path.Combine(rootPath, "servers.json");

            try
            {
                using HttpClient client = new HttpClient();
                var jsonData = await client.GetFromJsonAsync<object>(url);

                if (jsonData != null)
                {
                    string jsonString = JsonSerializer.Serialize(jsonData, new JsonSerializerOptions { WriteIndented = true });
                    
                    await System.IO.File.WriteAllTextAsync(filePath, jsonString);
                    MessageBox.Show($"Data saved to {filePath}");
                }
                else
                {
                    MessageBox.Show("No data received.");
                }    

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}