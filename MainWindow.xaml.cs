using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InfoRace
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient httpClient;
        public MainWindow()
        {
            InitializeComponent();
            httpClient = new HttpClient();
            listFormula.Visibility = Visibility.Collapsed;
            listSportsCars.Visibility = Visibility.Collapsed;
        }

        private void btFormula_Click(object sender, RoutedEventArgs e)
        {
            listSportsCars.Visibility = Visibility.Collapsed;
            if (listFormula.Visibility == Visibility.Visible)
            {
                listFormula.Visibility = Visibility.Collapsed;
            }
            else
            {
                listFormula.Visibility = Visibility.Visible;
            }

        }
        private void btSportsCars_Click(object sender, RoutedEventArgs e)
        {
            listFormula.Visibility = Visibility.Collapsed;
            if (listSportsCars.Visibility == Visibility.Visible)
            {
                listSportsCars.Visibility = Visibility.Collapsed;
            } 
            else 
            { 
                listSportsCars.Visibility = Visibility.Visible;
            }
        }

        private async void btFormula1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync("http://ergast.com/api/f1/current/driverStandings");
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                //TEST.NavigateToString(responseBody);

            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Error al acceder a la API: " + ex.Message);
            }
        }
    }
}