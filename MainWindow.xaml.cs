using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InfoRace
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            listFormula.Visibility = Visibility.Collapsed;
        }

        private void btFormula_Click(object sender, RoutedEventArgs e)
        {
            if (listFormula.Visibility == Visibility.Visible)
            {
                listFormula.Visibility = Visibility.Collapsed;
            }
            else
            {
                listFormula.Visibility = Visibility.Visible;
            }

        }
        private void listFormula_MouseEnter(object sender, MouseEventArgs e)
        {
            // Cuando el mouse entra en la lista, mantenla visible
            listFormula.Visibility = Visibility.Visible;
        }

        private void listFormula_MouseLeave(object sender, MouseEventArgs e)
        {
            // Cuando el mouse sale de la lista, oculta la lista
            listFormula.Visibility = Visibility.Collapsed;
        }

        private void btSportsCars_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}