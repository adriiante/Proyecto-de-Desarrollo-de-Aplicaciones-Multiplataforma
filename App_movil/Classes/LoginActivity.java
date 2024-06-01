package com.inforace;

import androidx.appcompat.app.AppCompatActivity;
import android.content.Intent;
import android.os.Bundle;
import android.text.TextUtils;
import android.util.Log;
import android.util.Patterns;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import com.google.firebase.FirebaseApp;
import com.google.firebase.firestore.DocumentSnapshot;
import com.google.firebase.firestore.FirebaseFirestore;

public class LoginActivity extends AppCompatActivity {

    private EditText editTextCorreo, editTextContraseña;
    private Button buttonLogin;
    private TextView textViewRegistro;
    private FirebaseFirestore db;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.login_activity);

        FirebaseApp.initializeApp(this);
        db = FirebaseFirestore.getInstance();

        // Inicializar vistas
        editTextCorreo = findViewById(R.id.editTextCorreo);
        editTextContraseña = findViewById(R.id.editTextContraseña);
        buttonLogin = findViewById(R.id.buttonIniciarSesion);
        textViewRegistro = findViewById(R.id.textViewCrearCuenta);

        buttonLogin.setOnClickListener(v -> {
            if (validarFormulario()) {
                iniciarSesion();
            }
        });

        textViewRegistro.setOnClickListener(v -> irAlRegistro());
    }

    private void irAlRegistro() {
        Intent intent = new Intent(LoginActivity.this, RegistroActivity.class);
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
        if (TextUtils.isEmpty(contraseña)) {
            editTextContraseña.setError("Contraseña requerida");
            return false;
        }

        return true;
    }

    private void iniciarSesion() {
        String correo = editTextCorreo.getText().toString().trim();
        String contraseña = editTextContraseña.getText().toString().trim();

        db.collection("UserData").document(correo).get()
                .addOnSuccessListener(documentSnapshot -> {
                    if (documentSnapshot.exists()) {
                        String storedPassword = documentSnapshot.getString("Password");
                        String contraseñaDesencriptada = Security.decrypt(storedPassword);
                        if (contraseñaDesencriptada != null && contraseñaDesencriptada.equals(contraseña)) {
                            Toast.makeText(LoginActivity.this, "Inicio de sesión exitoso", Toast.LENGTH_SHORT).show();
                            irAMain();
                        } else {
                            Toast.makeText(LoginActivity.this, "Contraseña incorrecta", Toast.LENGTH_SHORT).show();
                        }
                    } else {
                        Toast.makeText(LoginActivity.this, "Usuario no registrado", Toast.LENGTH_SHORT).show();
                    }
                })
                .addOnFailureListener(e -> {
                    Toast.makeText(LoginActivity.this, "Error al iniciar sesión", Toast.LENGTH_SHORT).show();
                    Log.e("LoginActivity", "Error al iniciar sesión", e);
                });
    }

    private void irAMain() {
        Intent intent = new Intent(LoginActivity.this, MainActivity.class);
        startActivity(intent);
        finish();
    }
}
