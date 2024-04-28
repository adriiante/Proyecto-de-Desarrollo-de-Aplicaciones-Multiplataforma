using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace InfoRace
{
    /// <summary>
    /// Lógica de interacción para RegisterWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void tbCrearCuenta_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Aquí puedes abrir la nueva ventana o realizar cualquier acción que desees.
            // Por ejemplo, abrir una nueva ventana:
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();

            this.Close();
        }
    }
}
