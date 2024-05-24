using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace InfoRace
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void tbIniciarSesion_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void btCrearCuenta_Click(object sender, RoutedEventArgs e)
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

            if (string.IsNullOrWhiteSpace(pbConfirmarContraseña.Password))
            {
                pbConfirmarContraseña.ToolTip = "La confirmación de contraseña no puede estar vacía";
                pbConfirmarContraseña.BorderBrush = Brushes.Red;
                canCreateAccount = false;
            }
            else if (pbConfirmarContraseña.Password != pbContraseña.Password)
            {
                pbConfirmarContraseña.ToolTip = "La confirmación de contraseña debe ser igual a la contraseña";
                pbConfirmarContraseña.BorderBrush = Brushes.Red;
                canCreateAccount = false;
            }
            else
            {
                pbConfirmarContraseña.ClearValue(Border.BorderBrushProperty);
            }

            if (string.IsNullOrWhiteSpace(tbTelefono.Text))
            {
                tbTelefono.ToolTip = "El teléfono no puede estar vacío";
                tbTelefono.BorderBrush = Brushes.Red;
                canCreateAccount = false;
            }
            else if (tbTelefono.Text.Length != 9 || !tbTelefono.Text.All(char.IsDigit))
            {
                tbTelefono.ToolTip = "El teléfono debe contener 9 números";
                tbTelefono.BorderBrush = Brushes.Red;
                canCreateAccount = false;
            }
            else
            {
                tbTelefono.ClearValue(Border.BorderBrushProperty);
            }

            if (string.IsNullOrWhiteSpace(tbUsuario.Text))
            {
                tbUsuario.ToolTip = "El usuario no puede estar vacío";
                tbUsuario.BorderBrush = Brushes.Red;
                canCreateAccount = false;
            }
            else
            {
                tbUsuario.ClearValue(Border.BorderBrushProperty);
            }

            if (!cbCondiciones.IsChecked == true)
            {
                MessageBox.Show("Debes aceptar las condiciones del servicio y política de privacidad para continuar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                canCreateAccount = false;
            }

            if (canCreateAccount == true)
            {
                var db = FirestoreHelper.Database;
                if (CheckEmailAlreadyExist())
                {
                    MessageBox.Show("El email ya existe");
                    return;
                }
                if (CheckUserAlreadyExist())
                {
                    MessageBox.Show("El usuario ya existe");
                    return;
                }
                if (CheckPhoneAlreadyExist())
                {
                    MessageBox.Show("El teléfono ya existe");
                    return;
                }
                var data = GetWriteData();
                Google.Cloud.Firestore.DocumentReference docRef = db.Collection("UserData").Document(data.Email);
                docRef.SetAsync(data);
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }

        private UserData GetWriteData()
        {
            string email = tbCorreo.Text.Trim();
            string password = Security.Encrypt(pbContraseña.Password);
            string phone = tbTelefono.Text.Trim();
            string username = tbUsuario.Text.Trim();

            return new UserData()
            {
                Email = email,
                Password = password,
                Phone = phone,
                Username = username
            };
        }

        private bool CheckEmailAlreadyExist()
        {
            string email = tbCorreo.Text.Trim();
            var db = FirestoreHelper.Database;
            Google.Cloud.Firestore.DocumentReference docRef = db.Collection("UserData").Document(email);
            UserData data = docRef.GetSnapshotAsync().Result.ConvertTo<UserData>();

            if (data != null)
            {
                return true;
            }
            return false;
        }

        private bool CheckUserAlreadyExist()
        {
            string username = tbUsuario.Text.Trim();
            var db = FirestoreHelper.Database;
            var query = db.Collection("UserData").WhereEqualTo("Username", username).Limit(1);
            var snapshot = query.GetSnapshotAsync().Result;

            return snapshot.Documents.Count > 0;
        }

        private bool CheckPhoneAlreadyExist()
        {
            string phone = tbTelefono.Text.Trim();
            var db = FirestoreHelper.Database;
            var query = db.Collection("UserData").WhereEqualTo("Phone", phone).Limit(1);
            var snapshot = query.GetSnapshotAsync().Result;

            return snapshot.Documents.Count > 0;
        }

    }
}
