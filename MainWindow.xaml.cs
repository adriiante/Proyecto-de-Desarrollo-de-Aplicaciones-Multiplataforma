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
    }
}