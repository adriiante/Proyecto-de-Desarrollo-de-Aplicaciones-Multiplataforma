using Google.Rpc;
using System.Diagnostics;
using System.Globalization;
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
            formula1Apis();
        }

        private async void formula1Apis()
        {
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

                // Fetch the circuit data
                HttpResponseMessage responseCircuits = await httpClient.GetAsync("http://ergast.com/api/f1/current");
                responseCircuits.EnsureSuccessStatusCode();
                string responseBodyCircuits = await responseCircuits.Content.ReadAsStringAsync();

                // Parse and format the circuit data
                string formattedDataCircuits = ParseXMLCircuits(responseBodyCircuits);

                // Assign the formatted HTML to the WebBrowser control
                Circuitos.NavigateToString(formattedDataCircuits);
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

        public string ParseXMLCircuits(string xmlContent)
        {
            // Create an XDocument from the provided XML content
            XDocument doc = XDocument.Parse(xmlContent);

            // Get the default namespace from the XML (xmlns="...")
            XElement root = doc.Root;
            XNamespace ns = root.GetDefaultNamespace();

            // Get a list of Race elements
            IEnumerable<XElement> races = doc.Descendants(ns + "Race");

            // Build the HTML output
            string htmlOutput = "<html><head><meta charset='UTF-8'></head><body><table border='1'><tr><th>Ronda</th><th>Nombre</th><th>Fecha</th><th>Hora</th><th>Localización</th></tr>";

            // Iterate over each Race element and build the HTML table
            foreach (var race in races)
            {
                // Get the relevant elements and attributes
                string round = race.Attribute("round")?.Value;
                string raceName = race.Element(ns + "RaceName")?.Value;
                string date = race.Element(ns + "Date")?.Value;
                string time = race.Element(ns + "Time")?.Value;

                // Convert the time to Spain time (CET/CEST)
                DateTime raceDateTimeUtc = DateTime.ParseExact(date + "T" + time, "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
                TimeZoneInfo spainTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
                DateTime raceDateTimeSpain = TimeZoneInfo.ConvertTimeFromUtc(raceDateTimeUtc, spainTimeZone);

                // Format the date and time for display
                string dateSpain = raceDateTimeSpain.ToString("yyyy-MM-dd");
                string timeSpain = raceDateTimeSpain.ToString("HH:mm");

                XElement circuit = race.Element(ns + "Circuit");
                string circuitName = circuit.Element(ns + "CircuitName")?.Value;
                string location = $"{circuit?.Element(ns + "Location")?.Element(ns + "Locality")?.Value}, {circuit?.Element(ns + "Location")?.Element(ns + "Country")?.Value}";

                // Build an HTML row for each Race
                htmlOutput += $"<tr><td>{round}</td><td>{raceName}</td><td>{dateSpain}</td><td>{timeSpain}</td><td>{circuitName} ({location})</td></tr>";
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

        private void btnLeft_Click(object sender, RoutedEventArgs e)
        {
            // Calculamos el ancho total del contenido en el canvas
            double totalWidth = canvasContainer.Children.Cast<UIElement>().Max(element => Canvas.GetLeft(element) + element.RenderSize.Width);

            // Calculamos el ancho visible del ScrollViewer
            double visibleWidth = scrollViewer.ActualWidth;

            // Calculamos el margen izquierdo actual del canvas
            double currentLeftMargin = canvasContainer.Margin.Left;

            // Calculamos el nuevo margen izquierdo considerando los límites
            double newLeftMargin = Math.Min(currentLeftMargin + 50, 0);

            // Aplicamos los límites teniendo en cuenta el ancho total del contenido y el ancho visible del ScrollViewer
            if (totalWidth - Math.Abs(newLeftMargin) < visibleWidth)
            {
                newLeftMargin = -(totalWidth - visibleWidth);
            }

            // Aplicamos el nuevo margen izquierdo al canvas
            canvasContainer.Margin = new Thickness(newLeftMargin, 0, 0, 0);
        }

        private void btnRight_Click(object sender, RoutedEventArgs e)
        {
            // Calculamos el ancho total del contenido en el canvas
            double totalWidth = canvasContainer.Children.Cast<UIElement>().Max(element => Canvas.GetLeft(element) + element.RenderSize.Width);

            // Calculamos el ancho visible del ScrollViewer
            double visibleWidth = scrollViewer.ActualWidth;

            // Calculamos el margen derecho actual del canvas
            double currentRightMargin = canvasContainer.Margin.Right;

            // Calculamos el nuevo margen derecho considerando los límites
            double newRightMargin = Math.Max(currentRightMargin + 50, 0);

            // Calculamos el margen izquierdo actual del canvas
            double currentLeftMargin = canvasContainer.Margin.Left;

            // Calculamos el nuevo margen izquierdo considerando los límites
            double newLeftMargin = Math.Max(currentLeftMargin - 50, -(totalWidth - visibleWidth + 50));

            // Aplicamos los nuevos márgenes al canvas
            canvasContainer.Margin = new Thickness(newLeftMargin, 0, newRightMargin, 0);
        }

        private void btAboutUs_Click(object sender, RoutedEventArgs e)
        {
            spHelp.Visibility = Visibility.Collapsed;
            spFormula1Page1.Visibility = Visibility.Collapsed;
            spFormula1Page2.Visibility = Visibility.Collapsed;
            spFormula2Page1.Visibility = Visibility.Collapsed;
            spFormula2Page2.Visibility = Visibility.Collapsed;
            spFormula3Page1.Visibility = Visibility.Collapsed;
            spFormula3Page2.Visibility = Visibility.Collapsed;
            spFormulaEPage1.Visibility = Visibility.Collapsed;
            spFormulaEPage2.Visibility = Visibility.Collapsed;
            spGT3Page1.Visibility = Visibility.Collapsed;
            spGT3Page2.Visibility = Visibility.Collapsed;
            spNascarPage1.Visibility = Visibility.Collapsed;
            spNascarPage2.Visibility = Visibility.Collapsed;
            spMotoGPPage1.Visibility = Visibility.Collapsed;
            spMotoGPPage2.Visibility = Visibility.Collapsed;
            spMoto2Page1.Visibility = Visibility.Collapsed;
            spMoto2Page2.Visibility = Visibility.Collapsed;
            spMoto3Page1.Visibility = Visibility.Collapsed;
            spMoto3Page2.Visibility = Visibility.Collapsed;
            spWRCPage1.Visibility = Visibility.Collapsed;
            spWRCPage2.Visibility = Visibility.Collapsed;
            spWECPage1.Visibility = Visibility.Collapsed;
            spWECPage2.Visibility = Visibility.Collapsed;
            spIMSAPage1.Visibility = Visibility.Collapsed;
            spIMSAPage2.Visibility = Visibility.Collapsed;
            spKWCPage1.Visibility = Visibility.Collapsed;
            spKWCPage2.Visibility = Visibility.Collapsed;
            spETRCPage1.Visibility= Visibility.Collapsed;
            spETRCPage2.Visibility = Visibility.Collapsed;
            spFormulaDriftPage1.Visibility = Visibility.Collapsed;
            spFormulaDriftPage2.Visibility = Visibility.Collapsed;
            spD1GPPage1.Visibility = Visibility.Collapsed;
            spD1GPPage2.Visibility = Visibility.Collapsed;
            spInicio.Visibility = Visibility.Visible;
        }

        private void btHelp_Click(object sender, RoutedEventArgs e)
        {
            spInicio.Visibility = Visibility.Collapsed;
            spFormula1Page1.Visibility = Visibility.Collapsed;
            spFormula1Page2.Visibility = Visibility.Collapsed;
            spFormula2Page1.Visibility = Visibility.Collapsed;
            spFormula2Page2.Visibility = Visibility.Collapsed;
            spFormula3Page1.Visibility = Visibility.Collapsed;
            spFormula3Page2.Visibility = Visibility.Collapsed;
            spFormulaEPage1.Visibility = Visibility.Collapsed;
            spFormulaEPage2.Visibility = Visibility.Collapsed;
            spGT3Page1.Visibility = Visibility.Collapsed;
            spGT3Page2.Visibility = Visibility.Collapsed;
            spNascarPage1.Visibility = Visibility.Collapsed;
            spNascarPage2.Visibility = Visibility.Collapsed;
            spMotoGPPage1.Visibility = Visibility.Collapsed;
            spMotoGPPage2.Visibility = Visibility.Collapsed;
            spMoto2Page1.Visibility = Visibility.Collapsed;
            spMoto2Page2.Visibility = Visibility.Collapsed;
            spMoto3Page1.Visibility = Visibility.Collapsed;
            spMoto3Page2.Visibility = Visibility.Collapsed;
            spWRCPage1.Visibility = Visibility.Collapsed;
            spWRCPage2.Visibility = Visibility.Collapsed;
            spWECPage1.Visibility = Visibility.Collapsed;
            spWECPage2.Visibility = Visibility.Collapsed;
            spIMSAPage1.Visibility = Visibility.Collapsed;
            spIMSAPage2.Visibility = Visibility.Collapsed;
            spKWCPage1.Visibility = Visibility.Collapsed;
            spKWCPage2.Visibility = Visibility.Collapsed;
            spETRCPage1.Visibility = Visibility.Collapsed;
            spETRCPage2.Visibility = Visibility.Collapsed;
            spFormulaDriftPage1.Visibility = Visibility.Collapsed;
            spFormulaDriftPage2.Visibility = Visibility.Collapsed;
            spD1GPPage1.Visibility = Visibility.Collapsed;
            spD1GPPage2.Visibility = Visibility.Collapsed;
            spHelp.Visibility = Visibility.Visible;
        }

        private void btFormula1_Click(object sender, RoutedEventArgs e)
        {
            spInicio.Visibility = Visibility.Collapsed;
            spHelp.Visibility = Visibility.Collapsed;
            spFormula1Page2.Visibility = Visibility.Collapsed;
            spFormula2Page1.Visibility = Visibility.Collapsed;
            spFormula2Page2.Visibility = Visibility.Collapsed;
            spFormula3Page1.Visibility = Visibility.Collapsed;
            spFormula3Page2.Visibility = Visibility.Collapsed;
            spFormulaEPage1.Visibility = Visibility.Collapsed;
            spFormulaEPage2.Visibility = Visibility.Collapsed;
            spGT3Page1.Visibility = Visibility.Collapsed;
            spGT3Page2.Visibility = Visibility.Collapsed;
            spNascarPage1.Visibility = Visibility.Collapsed;
            spNascarPage2.Visibility = Visibility.Collapsed;
            spMotoGPPage1.Visibility = Visibility.Collapsed;
            spMotoGPPage2.Visibility = Visibility.Collapsed;
            spMoto2Page1.Visibility = Visibility.Collapsed;
            spMoto2Page2.Visibility = Visibility.Collapsed;
            spMoto3Page1.Visibility = Visibility.Collapsed;
            spMoto3Page2.Visibility = Visibility.Collapsed;
            spWRCPage1.Visibility = Visibility.Collapsed;
            spWRCPage2.Visibility = Visibility.Collapsed;
            spWECPage1.Visibility = Visibility.Collapsed;
            spWECPage2.Visibility = Visibility.Collapsed;
            spIMSAPage1.Visibility = Visibility.Collapsed;
            spIMSAPage2.Visibility = Visibility.Collapsed;
            spKWCPage1.Visibility = Visibility.Collapsed;
            spKWCPage2.Visibility = Visibility.Collapsed;
            spETRCPage1.Visibility = Visibility.Collapsed;
            spETRCPage2.Visibility = Visibility.Collapsed;
            spFormulaDriftPage1.Visibility = Visibility.Collapsed;
            spFormulaDriftPage2.Visibility = Visibility.Collapsed;
            spD1GPPage1.Visibility = Visibility.Collapsed;
            spD1GPPage2.Visibility = Visibility.Collapsed;
            popupFormula.IsOpen = !popupFormula.IsOpen;
            spFormula1Page1.Visibility = Visibility.Visible;
        }

        private void btFormula2_Click(object sender, RoutedEventArgs e)
        {
            spInicio.Visibility = Visibility.Collapsed;
            spHelp.Visibility = Visibility.Collapsed;
            spFormula1Page1.Visibility = Visibility.Collapsed;
            spFormula1Page2.Visibility = Visibility.Collapsed;
            spFormula2Page2.Visibility = Visibility.Collapsed;
            spFormula3Page1.Visibility = Visibility.Collapsed;
            spFormula3Page2.Visibility = Visibility.Collapsed;
            spFormulaEPage1.Visibility = Visibility.Collapsed;
            spFormulaEPage2.Visibility = Visibility.Collapsed;
            spGT3Page1.Visibility = Visibility.Collapsed;
            spGT3Page2.Visibility = Visibility.Collapsed;
            spNascarPage1.Visibility = Visibility.Collapsed;
            spNascarPage2.Visibility = Visibility.Collapsed;
            spMotoGPPage1.Visibility = Visibility.Collapsed;
            spMotoGPPage2.Visibility = Visibility.Collapsed;
            spMoto2Page1.Visibility = Visibility.Collapsed;
            spMoto2Page2.Visibility = Visibility.Collapsed;
            spMoto3Page1.Visibility = Visibility.Collapsed;
            spMoto3Page2.Visibility = Visibility.Collapsed;
            spWRCPage1.Visibility = Visibility.Collapsed;
            spWRCPage2.Visibility = Visibility.Collapsed;
            spWECPage1.Visibility = Visibility.Collapsed;
            spWECPage2.Visibility = Visibility.Collapsed;
            spIMSAPage1.Visibility = Visibility.Collapsed;
            spIMSAPage2.Visibility = Visibility.Collapsed;
            spKWCPage1.Visibility = Visibility.Collapsed;
            spKWCPage2.Visibility = Visibility.Collapsed;
            spETRCPage1.Visibility = Visibility.Collapsed;
            spETRCPage2.Visibility = Visibility.Collapsed;
            spFormulaDriftPage1.Visibility = Visibility.Collapsed;
            spFormulaDriftPage2.Visibility = Visibility.Collapsed;
            spD1GPPage1.Visibility = Visibility.Collapsed;
            spD1GPPage2.Visibility = Visibility.Collapsed;
            popupFormula.IsOpen = !popupFormula.IsOpen;
            spFormula2Page1.Visibility = Visibility.Visible;
        }

        private void btFormula3_Click(object sender, RoutedEventArgs e)
        {
            spInicio.Visibility = Visibility.Collapsed;
            spHelp.Visibility = Visibility.Collapsed;
            spFormula1Page1.Visibility = Visibility.Collapsed;
            spFormula1Page2.Visibility = Visibility.Collapsed;
            spFormula2Page1.Visibility = Visibility.Collapsed;
            spFormula2Page2.Visibility = Visibility.Collapsed;
            spFormula3Page2.Visibility = Visibility.Collapsed;
            spFormulaEPage1.Visibility = Visibility.Collapsed;
            spFormulaEPage2.Visibility = Visibility.Collapsed;
            spGT3Page1.Visibility = Visibility.Collapsed;
            spGT3Page2.Visibility = Visibility.Collapsed;
            spNascarPage1.Visibility = Visibility.Collapsed;
            spNascarPage2.Visibility = Visibility.Collapsed;
            spMotoGPPage1.Visibility = Visibility.Collapsed;
            spMotoGPPage2.Visibility = Visibility.Collapsed;
            spMoto2Page1.Visibility = Visibility.Collapsed;
            spMoto2Page2.Visibility = Visibility.Collapsed;
            spMoto3Page1.Visibility = Visibility.Collapsed;
            spMoto3Page2.Visibility = Visibility.Collapsed;
            spWRCPage1.Visibility = Visibility.Collapsed;
            spWRCPage2.Visibility = Visibility.Collapsed;
            spWECPage1.Visibility = Visibility.Collapsed;
            spWECPage2.Visibility = Visibility.Collapsed;
            spIMSAPage1.Visibility = Visibility.Collapsed;
            spIMSAPage2.Visibility = Visibility.Collapsed;
            spKWCPage1.Visibility = Visibility.Collapsed;
            spKWCPage2.Visibility = Visibility.Collapsed;
            spETRCPage1.Visibility = Visibility.Collapsed;
            spETRCPage2.Visibility = Visibility.Collapsed;
            spFormulaDriftPage1.Visibility = Visibility.Collapsed;
            spFormulaDriftPage2.Visibility = Visibility.Collapsed;
            spD1GPPage1.Visibility = Visibility.Collapsed;
            spD1GPPage2.Visibility = Visibility.Collapsed;
            popupFormula.IsOpen = !popupFormula.IsOpen;
            spFormula3Page1.Visibility = Visibility.Visible;
        }

        private void btFormulaE_Click(object sender, RoutedEventArgs e)
        {
            spInicio.Visibility = Visibility.Collapsed;
            spHelp.Visibility = Visibility.Collapsed;
            spFormula1Page1.Visibility = Visibility.Collapsed;
            spFormula1Page2.Visibility = Visibility.Collapsed;
            spFormula2Page1.Visibility = Visibility.Collapsed;
            spFormula2Page2.Visibility = Visibility.Collapsed;
            spFormula3Page1.Visibility = Visibility.Collapsed;
            spFormula3Page2.Visibility = Visibility.Collapsed;
            spFormulaEPage2.Visibility = Visibility.Collapsed;
            spGT3Page1.Visibility = Visibility.Collapsed;
            spGT3Page2.Visibility = Visibility.Collapsed;
            spNascarPage1.Visibility = Visibility.Collapsed;
            spNascarPage2.Visibility = Visibility.Collapsed;
            spMotoGPPage1.Visibility = Visibility.Collapsed;
            spMotoGPPage2.Visibility = Visibility.Collapsed;
            spMoto2Page1.Visibility = Visibility.Collapsed;
            spMoto2Page2.Visibility = Visibility.Collapsed;
            spMoto3Page1.Visibility = Visibility.Collapsed;
            spMoto3Page2.Visibility = Visibility.Collapsed;
            spWRCPage1.Visibility = Visibility.Collapsed;
            spWRCPage2.Visibility = Visibility.Collapsed;
            spWECPage1.Visibility = Visibility.Collapsed;
            spWECPage2.Visibility = Visibility.Collapsed;
            spIMSAPage1.Visibility = Visibility.Collapsed;
            spIMSAPage2.Visibility = Visibility.Collapsed;
            spKWCPage1.Visibility = Visibility.Collapsed;
            spKWCPage2.Visibility = Visibility.Collapsed;
            spETRCPage1.Visibility = Visibility.Collapsed;
            spETRCPage2.Visibility = Visibility.Collapsed;
            spFormulaDriftPage1.Visibility = Visibility.Collapsed;
            spFormulaDriftPage2.Visibility = Visibility.Collapsed;
            spD1GPPage1.Visibility = Visibility.Collapsed;
            spD1GPPage2.Visibility = Visibility.Collapsed;
            popupFormula.IsOpen = !popupFormula.IsOpen;
            spFormulaEPage1.Visibility = Visibility.Visible;
        }

        private void btGT3_Click(object sender, RoutedEventArgs e)
        {
            spInicio.Visibility = Visibility.Collapsed;
            spHelp.Visibility = Visibility.Collapsed;
            spFormula1Page1.Visibility = Visibility.Collapsed;
            spFormula1Page2.Visibility = Visibility.Collapsed;
            spFormula2Page1.Visibility = Visibility.Collapsed;
            spFormula2Page2.Visibility = Visibility.Collapsed;
            spFormula3Page1.Visibility = Visibility.Collapsed;
            spFormula3Page2.Visibility = Visibility.Collapsed;
            spFormulaEPage1.Visibility = Visibility.Collapsed;
            spFormulaEPage2.Visibility = Visibility.Collapsed;
            spGT3Page2.Visibility = Visibility.Collapsed;
            spNascarPage1.Visibility = Visibility.Collapsed;
            spNascarPage2.Visibility = Visibility.Collapsed;
            spMotoGPPage1.Visibility = Visibility.Collapsed;
            spMotoGPPage2.Visibility = Visibility.Collapsed;
            spMoto2Page1.Visibility = Visibility.Collapsed;
            spMoto2Page2.Visibility = Visibility.Collapsed;
            spMoto3Page1.Visibility = Visibility.Collapsed;
            spMoto3Page2.Visibility = Visibility.Collapsed;
            spWRCPage1.Visibility = Visibility.Collapsed;
            spWRCPage2.Visibility = Visibility.Collapsed;
            spWECPage1.Visibility = Visibility.Collapsed;
            spWECPage2.Visibility = Visibility.Collapsed;
            spIMSAPage1.Visibility = Visibility.Collapsed;
            spIMSAPage2.Visibility = Visibility.Collapsed;
            spKWCPage1.Visibility = Visibility.Collapsed;
            spKWCPage2.Visibility = Visibility.Collapsed;
            spETRCPage1.Visibility = Visibility.Collapsed;
            spETRCPage2.Visibility = Visibility.Collapsed;
            spFormulaDriftPage1.Visibility = Visibility.Collapsed;
            spFormulaDriftPage2.Visibility = Visibility.Collapsed;
            spD1GPPage1.Visibility = Visibility.Collapsed;
            spD1GPPage2.Visibility = Visibility.Collapsed;
            popupSportsCars.IsOpen = !popupSportsCars.IsOpen;
            spGT3Page1.Visibility = Visibility.Visible;
        }

        private void btNascar_Click(object sender, RoutedEventArgs e)
        {
            spInicio.Visibility = Visibility.Collapsed;
            spHelp.Visibility = Visibility.Collapsed;
            spFormula1Page1.Visibility = Visibility.Collapsed;
            spFormula1Page2.Visibility = Visibility.Collapsed;
            spFormula2Page1.Visibility = Visibility.Collapsed;
            spFormula2Page2.Visibility = Visibility.Collapsed;
            spFormula3Page1.Visibility = Visibility.Collapsed;
            spFormula3Page2.Visibility = Visibility.Collapsed;
            spFormulaEPage1.Visibility = Visibility.Collapsed;
            spFormulaEPage2.Visibility = Visibility.Collapsed;
            spGT3Page1.Visibility = Visibility.Collapsed;
            spGT3Page2.Visibility = Visibility.Collapsed;
            spNascarPage2.Visibility = Visibility.Collapsed;
            spMotoGPPage1.Visibility = Visibility.Collapsed;
            spMotoGPPage2.Visibility = Visibility.Collapsed;
            spMoto2Page1.Visibility = Visibility.Collapsed;
            spMoto2Page2.Visibility = Visibility.Collapsed;
            spMoto3Page1.Visibility = Visibility.Collapsed;
            spMoto3Page2.Visibility = Visibility.Collapsed;
            spWRCPage1.Visibility = Visibility.Collapsed;
            spWRCPage2.Visibility = Visibility.Collapsed;
            spWECPage1.Visibility = Visibility.Collapsed;
            spWECPage2.Visibility = Visibility.Collapsed;
            spIMSAPage1.Visibility = Visibility.Collapsed;
            spIMSAPage2.Visibility = Visibility.Collapsed;
            spKWCPage1.Visibility = Visibility.Collapsed;
            spKWCPage2.Visibility = Visibility.Collapsed;
            spETRCPage1.Visibility = Visibility.Collapsed;
            spETRCPage2.Visibility = Visibility.Collapsed;
            spFormulaDriftPage1.Visibility = Visibility.Collapsed;
            spFormulaDriftPage2.Visibility = Visibility.Collapsed;
            spD1GPPage1.Visibility = Visibility.Collapsed;
            spD1GPPage2.Visibility = Visibility.Collapsed;
            popupSportsCars.IsOpen = !popupSportsCars.IsOpen;
            spNascarPage1.Visibility = Visibility.Visible;
        }

        private void btMotoGP_Click(object sender, RoutedEventArgs e)
        {
            spInicio.Visibility = Visibility.Collapsed;
            spHelp.Visibility = Visibility.Collapsed;
            spFormula1Page1.Visibility = Visibility.Collapsed;
            spFormula1Page2.Visibility = Visibility.Collapsed;
            spFormula2Page1.Visibility = Visibility.Collapsed;
            spFormula2Page2.Visibility = Visibility.Collapsed;
            spFormula3Page1.Visibility = Visibility.Collapsed;
            spFormula3Page2.Visibility = Visibility.Collapsed;
            spFormulaEPage1.Visibility = Visibility.Collapsed;
            spFormulaEPage2.Visibility = Visibility.Collapsed;
            spGT3Page1.Visibility = Visibility.Collapsed;
            spGT3Page2.Visibility = Visibility.Collapsed;
            spNascarPage1.Visibility = Visibility.Collapsed;
            spNascarPage2.Visibility = Visibility.Collapsed;
            spMotoGPPage2.Visibility = Visibility.Collapsed;
            spMoto2Page1.Visibility = Visibility.Collapsed;
            spMoto2Page2.Visibility = Visibility.Collapsed;
            spMoto3Page1.Visibility = Visibility.Collapsed;
            spMoto3Page2.Visibility = Visibility.Collapsed;
            spWRCPage1.Visibility = Visibility.Collapsed;
            spWRCPage2.Visibility = Visibility.Collapsed;
            spWECPage1.Visibility = Visibility.Collapsed;
            spWECPage2.Visibility = Visibility.Collapsed;
            spIMSAPage1.Visibility = Visibility.Collapsed;
            spIMSAPage2.Visibility = Visibility.Collapsed;
            spKWCPage1.Visibility = Visibility.Collapsed;
            spKWCPage2.Visibility = Visibility.Collapsed;
            spETRCPage1.Visibility = Visibility.Collapsed;
            spETRCPage2.Visibility = Visibility.Collapsed;
            spFormulaDriftPage1.Visibility = Visibility.Collapsed;
            spFormulaDriftPage2.Visibility = Visibility.Collapsed;
            spD1GPPage1.Visibility = Visibility.Collapsed;
            spD1GPPage2.Visibility = Visibility.Collapsed;
            popupMotos.IsOpen = !popupMotos.IsOpen;
            spMotoGPPage1.Visibility = Visibility.Visible;
        }

        private void btMoto2_Click(object sender, RoutedEventArgs e)
        {
            spInicio.Visibility = Visibility.Collapsed;
            spHelp.Visibility = Visibility.Collapsed;
            spFormula1Page1.Visibility = Visibility.Collapsed;
            spFormula1Page2.Visibility = Visibility.Collapsed;
            spFormula2Page1.Visibility = Visibility.Collapsed;
            spFormula2Page2.Visibility = Visibility.Collapsed;
            spFormula3Page1.Visibility = Visibility.Collapsed;
            spFormula3Page2.Visibility = Visibility.Collapsed;
            spFormulaEPage1.Visibility = Visibility.Collapsed;
            spFormulaEPage2.Visibility = Visibility.Collapsed;
            spGT3Page1.Visibility = Visibility.Collapsed;
            spGT3Page2.Visibility = Visibility.Collapsed;
            spNascarPage1.Visibility = Visibility.Collapsed;
            spNascarPage2.Visibility = Visibility.Collapsed;
            spMotoGPPage1.Visibility = Visibility.Collapsed;
            spMotoGPPage2.Visibility = Visibility.Collapsed;
            spMoto2Page2.Visibility = Visibility.Collapsed;
            spMoto3Page1.Visibility = Visibility.Collapsed;
            spMoto3Page2.Visibility = Visibility.Collapsed;
            spWRCPage1.Visibility = Visibility.Collapsed;
            spWRCPage2.Visibility = Visibility.Collapsed;
            spWECPage1.Visibility = Visibility.Collapsed;
            spWECPage2.Visibility = Visibility.Collapsed;
            spIMSAPage1.Visibility = Visibility.Collapsed;
            spIMSAPage2.Visibility = Visibility.Collapsed;
            spKWCPage1.Visibility = Visibility.Collapsed;
            spKWCPage2.Visibility = Visibility.Collapsed;
            spETRCPage1.Visibility = Visibility.Collapsed;
            spETRCPage2.Visibility = Visibility.Collapsed;
            spFormulaDriftPage1.Visibility = Visibility.Collapsed;
            spFormulaDriftPage2.Visibility = Visibility.Collapsed;
            spD1GPPage1.Visibility = Visibility.Collapsed;
            spD1GPPage2.Visibility = Visibility.Collapsed;
            popupMotos.IsOpen = !popupMotos.IsOpen;
            spMoto2Page1.Visibility = Visibility.Visible;
        }

        private void btMoto3_Click(object sender, RoutedEventArgs e)
        {
            spInicio.Visibility = Visibility.Collapsed;
            spHelp.Visibility = Visibility.Collapsed;
            spFormula1Page1.Visibility = Visibility.Collapsed;
            spFormula1Page2.Visibility = Visibility.Collapsed;
            spFormula2Page1.Visibility = Visibility.Collapsed;
            spFormula2Page2.Visibility = Visibility.Collapsed;
            spFormula3Page1.Visibility = Visibility.Collapsed;
            spFormula3Page2.Visibility = Visibility.Collapsed;
            spFormulaEPage1.Visibility = Visibility.Collapsed;
            spFormulaEPage2.Visibility = Visibility.Collapsed;
            spGT3Page1.Visibility = Visibility.Collapsed;
            spGT3Page2.Visibility = Visibility.Collapsed;
            spNascarPage1.Visibility = Visibility.Collapsed;
            spNascarPage2.Visibility = Visibility.Collapsed;
            spMotoGPPage1.Visibility = Visibility.Collapsed;
            spMotoGPPage2.Visibility = Visibility.Collapsed;
            spMoto2Page1.Visibility = Visibility.Collapsed;
            spMoto2Page2.Visibility = Visibility.Collapsed;
            spMoto3Page2.Visibility = Visibility.Collapsed;
            spWRCPage1.Visibility = Visibility.Collapsed;
            spWRCPage2.Visibility = Visibility.Collapsed;
            spWECPage1.Visibility = Visibility.Collapsed;
            spWECPage2.Visibility = Visibility.Collapsed;
            spIMSAPage1.Visibility = Visibility.Collapsed;
            spIMSAPage2.Visibility = Visibility.Collapsed;
            spKWCPage1.Visibility = Visibility.Collapsed;
            spKWCPage2.Visibility = Visibility.Collapsed;
            spETRCPage1.Visibility = Visibility.Collapsed;
            spETRCPage2.Visibility = Visibility.Collapsed;
            spFormulaDriftPage1.Visibility = Visibility.Collapsed;
            spFormulaDriftPage2.Visibility = Visibility.Collapsed;
            spD1GPPage1.Visibility = Visibility.Collapsed;
            spD1GPPage2.Visibility = Visibility.Collapsed;
            popupMotos.IsOpen = !popupMotos.IsOpen;
            spMoto3Page1.Visibility = Visibility.Visible;
        }

        private void btWRC_Click(object sender, RoutedEventArgs e)
        {
            spInicio.Visibility = Visibility.Collapsed;
            spHelp.Visibility = Visibility.Collapsed;
            spFormula1Page1.Visibility = Visibility.Collapsed;
            spFormula1Page2.Visibility = Visibility.Collapsed;
            spFormula2Page1.Visibility = Visibility.Collapsed;
            spFormula2Page2.Visibility = Visibility.Collapsed;
            spFormula3Page1.Visibility = Visibility.Collapsed;
            spFormula3Page2.Visibility = Visibility.Collapsed;
            spFormulaEPage1.Visibility = Visibility.Collapsed;
            spFormulaEPage2.Visibility = Visibility.Collapsed;
            spGT3Page1.Visibility = Visibility.Collapsed;
            spGT3Page2.Visibility = Visibility.Collapsed;
            spNascarPage1.Visibility = Visibility.Collapsed;
            spNascarPage2.Visibility = Visibility.Collapsed;
            spMotoGPPage1.Visibility = Visibility.Collapsed;
            spMotoGPPage2.Visibility = Visibility.Collapsed;
            spMoto2Page1.Visibility = Visibility.Collapsed;
            spMoto2Page2.Visibility = Visibility.Collapsed;
            spMoto3Page1.Visibility = Visibility.Collapsed;
            spMoto3Page2.Visibility = Visibility.Collapsed;
            spWRCPage2.Visibility = Visibility.Collapsed;
            spWECPage1.Visibility = Visibility.Collapsed;
            spWECPage2.Visibility = Visibility.Collapsed;
            spIMSAPage1.Visibility = Visibility.Collapsed;
            spIMSAPage2.Visibility = Visibility.Collapsed;
            spKWCPage1.Visibility = Visibility.Collapsed;
            spKWCPage2.Visibility = Visibility.Collapsed;
            spETRCPage1.Visibility = Visibility.Collapsed;
            spETRCPage2.Visibility = Visibility.Collapsed;
            spFormulaDriftPage1.Visibility = Visibility.Collapsed;
            spFormulaDriftPage2.Visibility = Visibility.Collapsed;
            spD1GPPage1.Visibility = Visibility.Collapsed;
            spD1GPPage2.Visibility = Visibility.Collapsed;
            popupRally.IsOpen = !popupRally.IsOpen;
            spWRCPage1.Visibility = Visibility.Visible;
        }

        private void btWEC_Click(object sender, RoutedEventArgs e)
        {
            spInicio.Visibility = Visibility.Collapsed;
            spHelp.Visibility = Visibility.Collapsed;
            spFormula1Page1.Visibility = Visibility.Collapsed;
            spFormula1Page2.Visibility = Visibility.Collapsed;
            spFormula2Page1.Visibility = Visibility.Collapsed;
            spFormula2Page2.Visibility = Visibility.Collapsed;
            spFormula3Page1.Visibility = Visibility.Collapsed;
            spFormula3Page2.Visibility = Visibility.Collapsed;
            spFormulaEPage1.Visibility = Visibility.Collapsed;
            spFormulaEPage2.Visibility = Visibility.Collapsed;
            spGT3Page1.Visibility = Visibility.Collapsed;
            spGT3Page2.Visibility = Visibility.Collapsed;
            spNascarPage1.Visibility = Visibility.Collapsed;
            spNascarPage2.Visibility = Visibility.Collapsed;
            spMotoGPPage1.Visibility = Visibility.Collapsed;
            spMotoGPPage2.Visibility = Visibility.Collapsed;
            spMoto2Page1.Visibility = Visibility.Collapsed;
            spMoto2Page2.Visibility = Visibility.Collapsed;
            spMoto3Page1.Visibility = Visibility.Collapsed;
            spMoto3Page2.Visibility = Visibility.Collapsed;
            spWRCPage1.Visibility = Visibility.Collapsed;
            spWRCPage2.Visibility = Visibility.Collapsed;
            spWECPage2.Visibility = Visibility.Collapsed;
            spIMSAPage1.Visibility = Visibility.Collapsed;
            spIMSAPage2.Visibility = Visibility.Collapsed;
            spKWCPage1.Visibility = Visibility.Collapsed;
            spKWCPage2.Visibility = Visibility.Collapsed;
            spETRCPage1.Visibility = Visibility.Collapsed;
            spETRCPage2.Visibility = Visibility.Collapsed;
            spFormulaDriftPage1.Visibility = Visibility.Collapsed;
            spFormulaDriftPage2.Visibility = Visibility.Collapsed;
            spD1GPPage1.Visibility = Visibility.Collapsed;
            spD1GPPage2.Visibility = Visibility.Collapsed;
            popupResistencia.IsOpen = !popupResistencia.IsOpen;
            spWECPage1.Visibility = Visibility.Visible;
        }

        private void btIMSA_Click(object sender, RoutedEventArgs e)
        {
            spInicio.Visibility = Visibility.Collapsed;
            spHelp.Visibility = Visibility.Collapsed;
            spFormula1Page1.Visibility = Visibility.Collapsed;
            spFormula1Page2.Visibility = Visibility.Collapsed;
            spFormula2Page1.Visibility = Visibility.Collapsed;
            spFormula2Page2.Visibility = Visibility.Collapsed;
            spFormula3Page1.Visibility = Visibility.Collapsed;
            spFormula3Page2.Visibility = Visibility.Collapsed;
            spFormulaEPage1.Visibility = Visibility.Collapsed;
            spFormulaEPage2.Visibility = Visibility.Collapsed;
            spGT3Page1.Visibility = Visibility.Collapsed;
            spGT3Page2.Visibility = Visibility.Collapsed;
            spNascarPage1.Visibility = Visibility.Collapsed;
            spNascarPage2.Visibility = Visibility.Collapsed;
            spMotoGPPage1.Visibility = Visibility.Collapsed;
            spMotoGPPage2.Visibility = Visibility.Collapsed;
            spMoto2Page1.Visibility = Visibility.Collapsed;
            spMoto2Page2.Visibility = Visibility.Collapsed;
            spMoto3Page1.Visibility = Visibility.Collapsed;
            spMoto3Page2.Visibility = Visibility.Collapsed;
            spWRCPage1.Visibility = Visibility.Collapsed;
            spWRCPage2.Visibility = Visibility.Collapsed;
            spWECPage1.Visibility = Visibility.Collapsed;
            spWECPage2.Visibility = Visibility.Collapsed;
            spIMSAPage2.Visibility = Visibility.Collapsed;
            spKWCPage1.Visibility = Visibility.Collapsed;
            spKWCPage2.Visibility = Visibility.Collapsed;
            spETRCPage1.Visibility = Visibility.Collapsed;
            spETRCPage2.Visibility = Visibility.Collapsed;
            spFormulaDriftPage1.Visibility = Visibility.Collapsed;
            spFormulaDriftPage2.Visibility = Visibility.Collapsed;
            spD1GPPage1.Visibility = Visibility.Collapsed;
            spD1GPPage2.Visibility = Visibility.Collapsed;
            popupResistencia.IsOpen = !popupResistencia.IsOpen;
            spIMSAPage1.Visibility = Visibility.Visible;
        }

        private void btKWC_Click(object sender, RoutedEventArgs e)
        {
            spInicio.Visibility = Visibility.Collapsed;
            spHelp.Visibility = Visibility.Collapsed;
            spFormula1Page1.Visibility = Visibility.Collapsed;
            spFormula1Page2.Visibility = Visibility.Collapsed;
            spFormula2Page1.Visibility = Visibility.Collapsed;
            spFormula2Page2.Visibility = Visibility.Collapsed;
            spFormula3Page1.Visibility = Visibility.Collapsed;
            spFormula3Page2.Visibility = Visibility.Collapsed;
            spFormulaEPage1.Visibility = Visibility.Collapsed;
            spFormulaEPage2.Visibility = Visibility.Collapsed;
            spGT3Page1.Visibility = Visibility.Collapsed;
            spGT3Page2.Visibility = Visibility.Collapsed;
            spNascarPage1.Visibility = Visibility.Collapsed;
            spNascarPage2.Visibility = Visibility.Collapsed;
            spMotoGPPage1.Visibility = Visibility.Collapsed;
            spMotoGPPage2.Visibility = Visibility.Collapsed;
            spMoto2Page1.Visibility = Visibility.Collapsed;
            spMoto2Page2.Visibility = Visibility.Collapsed;
            spMoto3Page1.Visibility = Visibility.Collapsed;
            spMoto3Page2.Visibility = Visibility.Collapsed;
            spWRCPage1.Visibility = Visibility.Collapsed;
            spWRCPage2.Visibility = Visibility.Collapsed;
            spWECPage1.Visibility = Visibility.Collapsed;
            spWECPage2.Visibility = Visibility.Collapsed;
            spIMSAPage1.Visibility = Visibility.Collapsed;
            spIMSAPage2.Visibility = Visibility.Collapsed;
            spKWCPage2.Visibility = Visibility.Collapsed;
            spETRCPage1.Visibility = Visibility.Collapsed;
            spETRCPage2.Visibility = Visibility.Collapsed;
            spFormulaDriftPage1.Visibility = Visibility.Collapsed;
            spFormulaDriftPage2.Visibility = Visibility.Collapsed;
            spD1GPPage1.Visibility = Visibility.Collapsed;
            spD1GPPage2.Visibility = Visibility.Collapsed;
            popupKarting.IsOpen = !popupKarting.IsOpen;
            spKWCPage1.Visibility = Visibility.Visible;
        }

        private void btETRC_Click(object sender, RoutedEventArgs e)
        {
            spInicio.Visibility = Visibility.Collapsed;
            spHelp.Visibility = Visibility.Collapsed;
            spFormula1Page1.Visibility = Visibility.Collapsed;
            spFormula1Page2.Visibility = Visibility.Collapsed;
            spFormula2Page1.Visibility = Visibility.Collapsed;
            spFormula2Page2.Visibility = Visibility.Collapsed;
            spFormula3Page1.Visibility = Visibility.Collapsed;
            spFormula3Page2.Visibility = Visibility.Collapsed;
            spFormulaEPage1.Visibility = Visibility.Collapsed;
            spFormulaEPage2.Visibility = Visibility.Collapsed;
            spGT3Page1.Visibility = Visibility.Collapsed;
            spGT3Page2.Visibility = Visibility.Collapsed;
            spNascarPage1.Visibility = Visibility.Collapsed;
            spNascarPage2.Visibility = Visibility.Collapsed;
            spMotoGPPage1.Visibility = Visibility.Collapsed;
            spMotoGPPage2.Visibility = Visibility.Collapsed;
            spMoto2Page1.Visibility = Visibility.Collapsed;
            spMoto2Page2.Visibility = Visibility.Collapsed;
            spMoto3Page1.Visibility = Visibility.Collapsed;
            spMoto3Page2.Visibility = Visibility.Collapsed;
            spWRCPage1.Visibility = Visibility.Collapsed;
            spWRCPage2.Visibility = Visibility.Collapsed;
            spWECPage1.Visibility = Visibility.Collapsed;
            spWECPage2.Visibility = Visibility.Collapsed;
            spIMSAPage1.Visibility = Visibility.Collapsed;
            spIMSAPage2.Visibility = Visibility.Collapsed;
            spKWCPage1.Visibility = Visibility.Collapsed;
            spKWCPage2.Visibility = Visibility.Collapsed;
            spETRCPage2.Visibility = Visibility.Collapsed;
            spFormulaDriftPage1.Visibility = Visibility.Collapsed;
            spFormulaDriftPage2.Visibility = Visibility.Collapsed;
            spD1GPPage1.Visibility = Visibility.Collapsed;
            spD1GPPage2.Visibility = Visibility.Collapsed;
            popupCamiones.IsOpen = !popupCamiones.IsOpen;
            spETRCPage1.Visibility = Visibility.Visible;
        }

        private void btFormulaDrift_Click(object sender, RoutedEventArgs e)
        {
            spInicio.Visibility = Visibility.Collapsed;
            spHelp.Visibility = Visibility.Collapsed;
            spFormula1Page1.Visibility = Visibility.Collapsed;
            spFormula1Page2.Visibility = Visibility.Collapsed;
            spFormula2Page1.Visibility = Visibility.Collapsed;
            spFormula2Page2.Visibility = Visibility.Collapsed;
            spFormula3Page1.Visibility = Visibility.Collapsed;
            spFormula3Page2.Visibility = Visibility.Collapsed;
            spFormulaEPage1.Visibility = Visibility.Collapsed;
            spFormulaEPage2.Visibility = Visibility.Collapsed;
            spGT3Page1.Visibility = Visibility.Collapsed;
            spGT3Page2.Visibility = Visibility.Collapsed;
            spNascarPage1.Visibility = Visibility.Collapsed;
            spNascarPage2.Visibility = Visibility.Collapsed;
            spMotoGPPage1.Visibility = Visibility.Collapsed;
            spMotoGPPage2.Visibility = Visibility.Collapsed;
            spMoto2Page1.Visibility = Visibility.Collapsed;
            spMoto2Page2.Visibility = Visibility.Collapsed;
            spMoto3Page1.Visibility = Visibility.Collapsed;
            spMoto3Page2.Visibility = Visibility.Collapsed;
            spWRCPage1.Visibility = Visibility.Collapsed;
            spWRCPage2.Visibility = Visibility.Collapsed;
            spWECPage1.Visibility = Visibility.Collapsed;
            spWECPage2.Visibility = Visibility.Collapsed;
            spIMSAPage1.Visibility = Visibility.Collapsed;
            spIMSAPage2.Visibility = Visibility.Collapsed;
            spKWCPage1.Visibility = Visibility.Collapsed;
            spKWCPage2.Visibility = Visibility.Collapsed;
            spETRCPage1.Visibility = Visibility.Collapsed;
            spETRCPage2.Visibility = Visibility.Collapsed;
            spFormulaDriftPage2.Visibility = Visibility.Collapsed;
            spD1GPPage1.Visibility = Visibility.Collapsed;
            spD1GPPage2.Visibility = Visibility.Collapsed;
            popupDrifting.IsOpen = !popupDrifting.IsOpen;
            spFormulaDriftPage1.Visibility = Visibility.Visible;
        }

        private void btD1GP_Click(object sender, RoutedEventArgs e)
        {
            spInicio.Visibility = Visibility.Collapsed;
            spHelp.Visibility = Visibility.Collapsed;
            spFormula1Page1.Visibility = Visibility.Collapsed;
            spFormula1Page2.Visibility = Visibility.Collapsed;
            spFormula2Page1.Visibility = Visibility.Collapsed;
            spFormula2Page2.Visibility = Visibility.Collapsed;
            spFormula3Page1.Visibility = Visibility.Collapsed;
            spFormula3Page2.Visibility = Visibility.Collapsed;
            spFormulaEPage1.Visibility = Visibility.Collapsed;
            spFormulaEPage2.Visibility = Visibility.Collapsed;
            spGT3Page1.Visibility = Visibility.Collapsed;
            spGT3Page2.Visibility = Visibility.Collapsed;
            spNascarPage1.Visibility = Visibility.Collapsed;
            spNascarPage2.Visibility = Visibility.Collapsed;
            spMotoGPPage1.Visibility = Visibility.Collapsed;
            spMotoGPPage2.Visibility = Visibility.Collapsed;
            spMoto2Page1.Visibility = Visibility.Collapsed;
            spMoto2Page2.Visibility = Visibility.Collapsed;
            spMoto3Page1.Visibility = Visibility.Collapsed;
            spMoto3Page2.Visibility = Visibility.Collapsed;
            spWRCPage1.Visibility = Visibility.Collapsed;
            spWRCPage2.Visibility = Visibility.Collapsed;
            spWECPage1.Visibility = Visibility.Collapsed;
            spWECPage2.Visibility = Visibility.Collapsed;
            spIMSAPage1.Visibility = Visibility.Collapsed;
            spIMSAPage2.Visibility = Visibility.Collapsed;
            spKWCPage1.Visibility = Visibility.Collapsed;
            spKWCPage2.Visibility = Visibility.Collapsed;
            spETRCPage1.Visibility = Visibility.Collapsed;
            spETRCPage2.Visibility = Visibility.Collapsed;
            spFormulaDriftPage1.Visibility = Visibility.Collapsed;
            spFormulaDriftPage2.Visibility = Visibility.Collapsed;
            spD1GPPage2.Visibility = Visibility.Collapsed;
            popupDrifting.IsOpen = !popupDrifting.IsOpen;
            spD1GPPage1.Visibility = Visibility.Visible;
        }

        private void btFormula_Click(object sender, RoutedEventArgs e)
        {
            popupFormula.IsOpen = !popupFormula.IsOpen;
        }

        private void btFormula1Page1_Click(object sender, RoutedEventArgs e)
        {
            spFormula1Page1.Visibility = Visibility.Visible;
            spFormula1Page2.Visibility = Visibility.Collapsed;
        }

        private void btFormula1Page2_Click(object sender, RoutedEventArgs e)
        {
            spFormula1Page2.Visibility = Visibility.Visible;
            spFormula1Page1.Visibility = Visibility.Collapsed;
        }

        private void btFormula2Page1_Click(object sender, RoutedEventArgs e)
        {
            spFormula2Page1.Visibility = Visibility.Visible;
            spFormula2Page2.Visibility = Visibility.Collapsed;
        }

        private void btFormula2Page2_Click(object sender, RoutedEventArgs e)
        {
            spFormula2Page2.Visibility = Visibility.Visible;
            spFormula2Page1.Visibility = Visibility.Collapsed;
        }

        private void btFormula3Page1_Click(object sender, RoutedEventArgs e)
        {
            spFormula3Page1.Visibility = Visibility.Visible;
            spFormula3Page2.Visibility = Visibility.Collapsed;
        }

        private void btFormula3Page2_Click(object sender, RoutedEventArgs e)
        {
            spFormula3Page2.Visibility = Visibility.Visible;
            spFormula3Page1.Visibility = Visibility.Collapsed;
        }

        private void btFormulaEPage1_Click(object sender, RoutedEventArgs e)
        {
            spFormulaEPage1.Visibility = Visibility.Visible;
            spFormulaEPage2.Visibility = Visibility.Collapsed;
        }

        private void btFormulaEPage2_Click(object sender, RoutedEventArgs e)
        {
            spFormulaEPage2.Visibility = Visibility.Visible;
            spFormulaEPage1.Visibility = Visibility.Collapsed;
        }

        private void btSportsCars_Click(object sender, RoutedEventArgs e)
        {
            popupSportsCars.IsOpen = !popupSportsCars.IsOpen;
        }

        private void btGT3Page1_Click(object sender, RoutedEventArgs e)
        {
            spGT3Page1.Visibility = Visibility.Visible;
            spGT3Page2.Visibility = Visibility.Collapsed;
        }

        private void btGT3Page2_Click(object sender, RoutedEventArgs e)
        {
            spGT3Page2.Visibility = Visibility.Visible;
            spGT3Page1.Visibility = Visibility.Collapsed;
        }

        private void btNascarPage1_Click(object sender, RoutedEventArgs e)
        {
            spNascarPage1.Visibility = Visibility.Visible;
            spNascarPage2.Visibility = Visibility.Collapsed;
        }

        private void btNascarPage2_Click(object sender, RoutedEventArgs e)
        {
            spNascarPage2.Visibility = Visibility.Visible;
            spNascarPage1.Visibility = Visibility.Collapsed;
        }

        private void btMoto_Click(object sender, RoutedEventArgs e)
        {
            popupMotos.IsOpen = !popupMotos.IsOpen;
        }

        private void btMotoGPPage1_Click(object sender, RoutedEventArgs e)
        {
            spMotoGPPage1.Visibility = Visibility.Visible;
            spMotoGPPage2.Visibility = Visibility.Collapsed;
        }

        private void btMotoGPPage2_Click(object sender, RoutedEventArgs e)
        {
            spMotoGPPage2.Visibility = Visibility.Visible;
            spMotoGPPage1.Visibility = Visibility.Collapsed;
        }

        private void btMoto2Page1_Click(object sender, RoutedEventArgs e)
        {
            spMoto2Page1.Visibility = Visibility.Visible;
            spMoto2Page2.Visibility = Visibility.Collapsed;
        }

        private void btMoto2Page2_Click(object sender, RoutedEventArgs e)
        {
            spMoto2Page2.Visibility = Visibility.Visible;
            spMoto2Page1.Visibility = Visibility.Collapsed;
        }

        private void btMoto3Page1_Click(object sender, RoutedEventArgs e)
        {
            spMoto3Page1.Visibility = Visibility.Visible;
            spMoto3Page2.Visibility = Visibility.Collapsed;
        }

        private void btMoto3Page2_Click(object sender, RoutedEventArgs e)
        {
            spMoto3Page2.Visibility = Visibility.Visible;
            spMoto3Page1.Visibility = Visibility.Collapsed;
        }

        private void btRally_Click(object sender, RoutedEventArgs e)
        {
            popupRally.IsOpen = !popupRally.IsOpen;
        }

        private void btWRCPage1_Click(object sender, RoutedEventArgs e)
        {
            spWRCPage1.Visibility = Visibility.Visible;
            spWRCPage2.Visibility = Visibility.Collapsed;
        }

        private void btWRCPage2_Click(object sender, RoutedEventArgs e)
        {
            spWRCPage2.Visibility = Visibility.Visible;
            spWRCPage1.Visibility = Visibility.Collapsed;
        }

        private void btResistencia_Click(object sender, RoutedEventArgs e)
        {
            popupResistencia.IsOpen = !popupResistencia.IsOpen;
        }

        private void btWECPage1_Click(object sender, RoutedEventArgs e)
        {
            spWECPage1.Visibility = Visibility.Visible;
            spWECPage2.Visibility = Visibility.Collapsed;
        }

        private void btWECPage2_Click(object sender, RoutedEventArgs e)
        {
            spWECPage2.Visibility = Visibility.Visible;
            spWECPage1.Visibility = Visibility.Collapsed;
        }

        private void btIMSAPage1_Click(object sender, RoutedEventArgs e)
        {
            spIMSAPage1.Visibility = Visibility.Visible;
            spIMSAPage2.Visibility = Visibility.Collapsed;
        }

        private void btIMSAPage2_Click(object sender, RoutedEventArgs e)
        {
            spIMSAPage2.Visibility = Visibility.Visible;
            spIMSAPage1.Visibility = Visibility.Collapsed;
        }

        private void btKarting_Click(object sender, RoutedEventArgs e)
        {
            popupKarting.IsOpen = !popupKarting.IsOpen;
        }

        private void btKWCPage1_Click(object sender, RoutedEventArgs e)
        {
            spKWCPage1.Visibility = Visibility.Visible;
            spKWCPage2.Visibility = Visibility.Collapsed;
        }

        private void btKWCPage2_Click(object sender, RoutedEventArgs e)
        {
            spKWCPage2.Visibility = Visibility.Visible;
            spKWCPage1.Visibility = Visibility.Collapsed;
        }

        private void btCamiones_Click(object sender, RoutedEventArgs e)
        {
            popupCamiones.IsOpen = !popupCamiones.IsOpen;
        }

        private void btETRCPage1_Click(object sender, RoutedEventArgs e)
        {
            spETRCPage1.Visibility = Visibility.Visible;
            spETRCPage2.Visibility = Visibility.Collapsed;
        }

        private void btETRCPage2_Click(object sender, RoutedEventArgs e)
        {
            spETRCPage2.Visibility = Visibility.Visible;
            spETRCPage1.Visibility = Visibility.Collapsed;
        }

        private void btDrifting_Click(object sender, RoutedEventArgs e)
        {
            popupDrifting.IsOpen = !popupDrifting.IsOpen;
        }

        private void btFormulaDriftPage1_Click(object sender, RoutedEventArgs e)
        {
            spFormulaDriftPage1.Visibility = Visibility.Visible;
            spFormulaDriftPage2.Visibility = Visibility.Collapsed;
        }

        private void btFormulaDriftPage2_Click(object sender, RoutedEventArgs e)
        {
            spFormulaDriftPage2.Visibility = Visibility.Visible;
            spFormulaDriftPage1.Visibility = Visibility.Collapsed;
        }

        private void btD1GPPage1_Click(object sender, RoutedEventArgs e)
        {
            spD1GPPage1.Visibility = Visibility.Visible;
            spD1GPPage2.Visibility = Visibility.Collapsed;
        }

        private void btD1GPPage2_Click(object sender, RoutedEventArgs e)
        {
            spD1GPPage2.Visibility = Visibility.Visible;
            spD1GPPage1.Visibility = Visibility.Collapsed;
        }

    }
}