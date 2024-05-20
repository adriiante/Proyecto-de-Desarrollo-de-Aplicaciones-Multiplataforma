using System.Diagnostics;
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
using System.Xml;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Windows.Controls.Image;

namespace InfoRace
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient httpClient;
        public MainWindow()
        {
            InitializeComponent();
            httpClient = new HttpClient();
            if (!SessionManager.IsLoggedIn)
            {
                MessageBox.Show("Debe iniciar sesión primero");
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
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
            spInicio.Visibility = Visibility.Collapsed;
            spFormula1.Visibility = Visibility.Visible;
            listFormula.Visibility = Visibility.Collapsed;

            try
            {
                HttpResponseMessage responseDrivers = await httpClient.GetAsync("http://ergast.com/api/f1/current/driverStandings");
                responseDrivers.EnsureSuccessStatusCode();
                string responseBodyDrivers = await responseDrivers.Content.ReadAsStringAsync();

                // Llama a la función ParseXML para procesar el contenido XML y obtener HTML formateado
                string formattedDataDrivers = ParseXMLDrivers(responseBodyDrivers);

                // Asigna el HTML formateado al control WebBrowser
                Pilotos.NavigateToString(formattedDataDrivers);

                HttpResponseMessage responseConstructor = await httpClient.GetAsync("http://ergast.com/api/f1/current/constructorStandings");
                responseConstructor.EnsureSuccessStatusCode();
                string responseBodyConstructor = await responseConstructor.Content.ReadAsStringAsync();

                // Llama a la función ParseXML para procesar el contenido XML y obtener HTML formateado
                string formattedDataConstructor = ParseXMLConstructors(responseBodyConstructor);

                // Asigna el HTML formateado al control WebBrowser
                Constructores.NavigateToString(formattedDataConstructor);
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Error al acceder a la API: " + ex.Message);
            }
        }

        public string ParseXMLDrivers(string xmlContent)
        {
            // Crea un documento XDocument a partir del contenido XML proporcionado
            XDocument doc = XDocument.Parse(xmlContent);

            // Obtiene el espacio de nombres predeterminado del XML (xmlns="...")
            XElement root = doc.Root;
            XNamespace ns = root.GetDefaultNamespace();

            // Obtiene una lista de elementos DriverStanding
            IEnumerable<XElement> driverStandings = doc.Descendants(ns + "DriverStanding");

            // Construye el HTML de salida
            string htmlOutput = "<html><head><meta charset='UTF-8'></head><body><table border='1'><tr><th>Posición</th><th>Puntos</th><th>Victorias</th><th>Piloto</th><th>Constructor</th></tr>";

            // Itera sobre cada elemento DriverStanding y construye la tabla HTML
            foreach (var standing in driverStandings)
            {
                // Obtiene los atributos relevantes del elemento DriverStanding
                string position = standing.Attribute("position")?.Value;
                string points = standing.Attribute("points")?.Value;
                string wins = standing.Attribute("wins")?.Value;

                // Obtiene los elementos Driver y Constructor dentro de DriverStanding
                XElement driver = standing.Element(ns + "Driver");
                XElement constructor = standing.Element(ns + "Constructor");

                // Obtiene los detalles del conductor y del constructor
                string driverName = $"{driver?.Element(ns + "GivenName")?.Value} {driver?.Element(ns + "FamilyName")?.Value}";
                string constructorName = constructor?.Element(ns + "Name")?.Value;

                // Construye una fila de la tabla HTML para cada DriverStanding
                htmlOutput += $"<tr><td>{position}</td><td>{points}</td><td>{wins}</td><td>{driverName}</td><td>{constructorName}</td></tr>";
            }

            htmlOutput += "</table></body></html>";

            return htmlOutput;
        }

        public string ParseXMLConstructors(string xmlContent)
        {
            // Create an XDocument from the provided XML content
            XDocument doc = XDocument.Parse(xmlContent);

            // Get the default namespace from the XML (xmlns="...")
            XElement root = doc.Root;
            XNamespace ns = root.GetDefaultNamespace();

            // Get a list of ConstructorStanding elements
            IEnumerable<XElement> constructorStandings = doc.Descendants(ns + "ConstructorStanding");

            // Build the HTML output
            string htmlOutput = "<html><head><meta charset='UTF-8'></head><body><table border='1'><tr><th>Nombre</th><th>Posición</th><th>Puntos</th><th>Victorias</th></tr>";

            // Iterate over each ConstructorStanding element and build the HTML table
            foreach (var standing in constructorStandings)
            {
                // Get the relevant elements and attributes
                string position = standing.Attribute("position")?.Value;
                string points = standing.Attribute("points")?.Value;
                string wins = standing.Attribute("wins")?.Value;

                XElement constructor = standing.Element(ns + "Constructor");
                string name = constructor.Element(ns + "Name")?.Value;

                // Build an HTML row for each ConstructorStanding
                htmlOutput += $"<tr><td>{name}</td><td>{position}</td><td>{points}</td><td>{wins}</td></tr>";
            }

            htmlOutput += "</table></body></html>";

            return htmlOutput;
        }

        private void btTwitter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd",
                    Arguments = $"/c start https://twitter.com/inforaceinfo",
                    CreateNoWindow = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al abrir la página web: {ex.Message}");
            }
        }

        private void btInstagram_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd",
                    Arguments = $"/c start https://www.instagram.com/inforaceinfo/",
                    CreateNoWindow = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al abrir la página web: {ex.Message}");
            }
        }

        private void btFacebook_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd",
                    Arguments = $"/c start https://www.facebook.com/profile.php?id=61559147424922",
                    CreateNoWindow = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al abrir la página web: {ex.Message}");
            }
        }

        private void btAboutUs_Click(object sender, RoutedEventArgs e)
        {
            spFormula1.Visibility = Visibility.Collapsed;
            spHelp.Visibility = Visibility.Collapsed;
            spInicio.Visibility = Visibility.Visible;
        }

        private void btHelp_Click(object sender, RoutedEventArgs e)
        {
            spFormula1.Visibility = Visibility.Collapsed;
            spInicio.Visibility = Visibility.Collapsed;
            spHelp.Visibility = Visibility.Visible;
        }

        private void btWeb_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd",
                    Arguments = $"/c start https://WEB/",
                    CreateNoWindow = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al abrir la página web: {ex.Message}");
            }
        }

        private void Perfil_Click(object sender, RoutedEventArgs e)
        {
            // Aquí puedes abrir la ventana del perfil
            // Por ejemplo:
            //PerfilWindow perfilWindow = new PerfilWindow();
            //perfilWindow.ShowDialog();
        }

        private void CerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            SessionManager.Logout();
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
              //Mostrar el menú contextual en la posición del ratón
            if (sender is Image image)
            {
                if (image.ContextMenu != null)
                {
                    image.ContextMenu.PlacementTarget = image;
                    image.ContextMenu.IsOpen = true;
                    e.Handled = true;
                }
            }
        }

        private void formula1Notifications_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void formula1Notifications_Unchecked(object sender, RoutedEventArgs e)
        {

        }
    }
}