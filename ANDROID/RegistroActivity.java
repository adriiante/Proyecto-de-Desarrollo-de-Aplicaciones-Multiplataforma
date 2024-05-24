package com.inforace;

import androidx.appcompat.app.AppCompatActivity;
import android.content.Intent;
import android.os.Bundle;
import android.text.TextUtils;
import android.util.Log;
import android.util.Patterns;
import android.view.View;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import com.google.firebase.FirebaseApp;
import com.google.firebase.firestore.FirebaseFirestore;

import java.util.HashMap;
import java.util.Map;

public class RegistroActivity extends AppCompatActivity {

    private EditText editTextCorreo;
    private EditText editTextContraseña;
    private EditText editTextConfirmarContraseña;
    private EditText editTextTelefono;
    private EditText editTextNombreUsuario;
    private CheckBox checkBoxAceptoCondiciones;
    private CheckBox checkBoxRecibirNotificaciones;
    private Button buttonRegistrar;
    private TextView textViewLogin;

    private FirebaseFirestore db;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.registro_activity);

        FirebaseApp.initializeApp(this);
        db = FirebaseFirestore.getInstance();

        // Inicializar vistas
        editTextCorreo = findViewById(R.id.editTextCorreo);
        editTextContraseña = findViewById(R.id.editTextContraseña);
        editTextConfirmarContraseña = findViewById(R.id.editTextConfirmarContraseña);
        editTextTelefono = findViewById(R.id.editTextTelefono);
        editTextNombreUsuario = findViewById(R.id.editTextNombreUsuario);
        checkBoxAceptoCondiciones = findViewById(R.id.checkBoxAceptoCondiciones);
        checkBoxRecibirNotificaciones = findViewById(R.id.checkBoxRecibirNotificaciones);
        buttonRegistrar = findViewById(R.id.buttonRegistrar);
        textViewLogin = findViewById(R.id.textViewLogin);

        buttonRegistrar.setOnClickListener(v -> {
            if (validarFormulario()) {
                registrarUsuario();
            }
        });

        textViewLogin.setOnClickListener(v -> irAlLogin());
    }

    private void irAlLogin() {
        Intent intent = new Intent(RegistroActivity.this, LoginActivity.class);
        startActivity(intent);
    }

    private boolean validarFormulario() {
        // Validación de correo electrónico
        String correo = editTextCorreo.getText().toString().trim();
        if (TextUtils.isEmpty(correo) || !Patterns.EMAIL_ADDRESS.matcher(correo).matches()) {
            editTextCorreo.setError("Correo inválido");
            return false;
        }
        // Validación de contraseña
        String contraseña = editTextContraseña.getText().toString().trim();
        if (TextUtils.isEmpty(contraseña) || contraseña.length() < 8 || !contraseña.matches(".*[A-Z].*")) {
            editTextContraseña.setError("Contraseña inválida (al menos 8 caracteres con una mayúscula)");
            return false;
        }
        // Validación de confirmación de contraseña
        String confirmarContraseña = editTextConfirmarContraseña.getText().toString().trim();
        if (!confirmarContraseña.equals(contraseña)) {
            editTextConfirmarContraseña.setError("Las contraseñas no coinciden");
            return false;
        }
        // Validación de teléfono
        String telefono = editTextTelefono.getText().toString().trim();
        if (TextUtils.isEmpty(telefono) || telefono.length() != 9 || !telefono.matches("\\d{9}")) {
            editTextTelefono.setError("Teléfono inválido");
            return false;
        }
        // Validación de nombre de usuario
        String nombreUsuario = editTextNombreUsuario.getText().toString().trim();
        if (TextUtils.isEmpty(nombreUsuario)) {
            editTextNombreUsuario.setError("Nombre de usuario requerido");
            return false;
        }
        // Validación de aceptar condiciones
        if (!checkBoxAceptoCondiciones.isChecked()) {
            Toast.makeText(this, "Debes aceptar los términos y condiciones", Toast.LENGTH_SHORT).show();
            return false;
        }
        return true;
    }

    private void registrarUsuario() {
        String correo = editTextCorreo.getText().toString().trim();
        String telefono = editTextTelefono.getText().toString().trim();
        String nombreUsuario = editTextNombreUsuario.getText().toString().trim();
        String contraseña = editTextContraseña.getText().toString().
                trim();
        String contraseñaEncriptada = Security.encrypt(contraseña); // Encriptar la contraseña

        verificarUsuario(correo, telefono, nombreUsuario, contraseñaEncriptada); // Pasar la contraseña encriptada
    }

    private void verificarUsuario(String correo, String telefono, String nombreUsuario, String contraseña) {
        db.collection("UserData")
                .whereEqualTo("Email", correo)
                .get()
                .addOnSuccessListener(queryDocumentSnapshots -> {
                    if (!queryDocumentSnapshots.isEmpty()) {
                        Toast.makeText(RegistroActivity.this, "El correo ya está en uso", Toast.LENGTH_SHORT).show();
                    } else {
                        db.collection("UserData")
                                .whereEqualTo("Phone", telefono)
                                .get()
                                .addOnSuccessListener(queryDocumentSnapshots2 -> {
                                    if (!queryDocumentSnapshots2.isEmpty()) {
                                        Toast.makeText(RegistroActivity.this, "El teléfono ya está en uso", Toast.LENGTH_SHORT).show();
                                    } else {
                                        db.collection("UserData")
                                                .whereEqualTo("Username", nombreUsuario)
                                                .get()
                                                .addOnSuccessListener(queryDocumentSnapshots3 -> {
                                                    if (!queryDocumentSnapshots3.isEmpty()) {
                                                        Toast.makeText(RegistroActivity.this, "El nombre de usuario ya está en uso", Toast.LENGTH_SHORT).show();
                                                    } else {
                                                        // Proceder con el registro del usuario
                                                        registrarEnFirestore(correo, telefono, nombreUsuario, contraseña);
                                                    }
                                                })
                                                .addOnFailureListener(e -> {
                                                    Toast.makeText(RegistroActivity.this, "Error al verificar nombre de usuario", Toast.LENGTH_SHORT).show();
                                                    Log.e("RegistroActivity", "Error al verificar nombre de usuario", e);
                                                });
                                    }
                                })
                                .addOnFailureListener(e -> {
                                    Toast.makeText(RegistroActivity.this, "Error al verificar teléfono", Toast.LENGTH_SHORT).show();
                                    Log.e("RegistroActivity", "Error al verificar teléfono", e);
                                });
                    }
                })
                .addOnFailureListener(e -> {
                    Toast.makeText(RegistroActivity.this, "Error al verificar correo", Toast.LENGTH_SHORT).show();
                    Log.e("RegistroActivity", "Error al verificar correo", e);
                });
    }

    private void registrarEnFirestore(String correo, String telefono, String nombreUsuario, String contraseña) {
        Map<String, Object> user = new HashMap<>();
        user.put("Email", correo);
        user.put("Phone", telefono);
        user.put("Username", nombreUsuario);
        user.put("Password", contraseña); // Considera no almacenar contraseñas en texto plano

        db.collection("UserData")
                .document(correo)
                .set(user)
                .addOnSuccessListener(aVoid -> {
                    Toast.makeText(RegistroActivity.this, "Registro exitoso", Toast.LENGTH_SHORT).show();
                    irAlLogin(); // Navegar a la pantalla de inicio de sesión
                })
                .addOnFailureListener(e -> {
                    Toast.makeText(RegistroActivity.this, "Error al registrar usuario", Toast.LENGTH_SHORT).show();
                    Log.e("RegistroActivity", "Error al registrar usuario", e);
                });
    }
}
