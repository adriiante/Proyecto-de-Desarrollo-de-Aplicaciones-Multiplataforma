using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace InfoRace
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            FirestoreHelper.SetEnvironmentVariable();
        }

        private void tbCrearCuenta_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }

        private void btInicio_Click(object sender, RoutedEventArgs e)
        {
            bool canCreateAccount = true;
            if (string.IsNullOrWhiteSpace(tbCorreo.Text))
            {
                tbCorreo.ToolTip = "El correo electrónico no puede estar vacío";
                tbCorreo.BorderBrush = Brushes.Red;
                canCreateAccount = false;
            }
            else if (!tbCorreo.Text.Contains("@") || !tbCorreo.Text.Contains("."))
            {
                tbCorreo.ToolTip = "El correo electrónico debe contener un '@' y un '.'";
                tbCorreo.BorderBrush = Brushes.Red;
                canCreateAccount = false;
            }
            else
            {
                tbCorreo.ClearValue(Border.BorderBrushProperty);
            }

            if (string.IsNullOrWhiteSpace(pbContraseña.Password))
            {
                pbContraseña.ToolTip = "La contraseña no puede estar vacía";
                pbContraseña.BorderBrush = Brushes.Red;
                canCreateAccount = false;
            }
            else if (!pbContraseña.Password.Any(char.IsUpper) || pbContraseña.Password.Length < 8)
            {
                pbContraseña.ToolTip = "La contraseña debe tener al menos una mayúscula y 8 caracteres";
                pbContraseña.BorderBrush = Brushes.Red;
                canCreateAccount = false;
            }
            else
            {
                pbContraseña.ClearValue(Border.BorderBrushProperty);
            }

            if (canCreateAccount == true)
            {
                string email = tbCorreo.Text.Trim();
                string password = pbContraseña.Password;

                var db = FirestoreHelper.Database;
                Google.Cloud.Firestore.DocumentReference docRef = db.Collection("UserData").Document(email);
                UserData data = docRef.GetSnapshotAsync().Result.ConvertTo<UserData>();

                if (data != null)
                {
                    if (password == Security.Decrypt(data.Password))
                    {
                        SessionManager.Login(data);
                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("El usuario no existe");
                    }
                }
                else
                {
                    MessageBox.Show("El usuario no existe");
                }
            }
        }
    }
}
