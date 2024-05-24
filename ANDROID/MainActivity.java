package com.inforace;

import androidx.appcompat.app.AppCompatActivity;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.PopupMenu;
import android.widget.Toast;

import com.google.firebase.auth.FirebaseAuth;

public class MainActivity extends AppCompatActivity {

    private FirebaseAuth mAuth;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        mAuth = FirebaseAuth.getInstance();

        // Botón Formula
        Button botonF = findViewById(R.id.boton_f);
        botonF.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                showPopupMenu(v, R.menu.menu_f);
            }
        });

        // Botón Sport Cars
        Button botonSP = findViewById(R.id.boton_SP);
        botonSP.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                showPopupMenu(v, R.menu.menu_sp);
            }
        });

        // Botón Motos
        Button botonMotos = findViewById(R.id.boton_motos);
        botonMotos.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                showPopupMenu(v, R.menu.menu_m);
            }
        });

        // Botón Rally
        Button botonRally = findViewById(R.id.boton_rally);
        botonRally.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                showPopupMenu(v, R.menu.menu_ra);
            }
        });

        // Botón Resistencia
        Button botonResistencia = findViewById(R.id.boton_resistencia);
        botonResistencia.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                showPopupMenu(v, R.menu.menu_re);
            }
        });

        // Botón Karting
        Button botonKarting = findViewById(R.id.boton_karting);
        botonKarting.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                showPopupMenu(v, R.menu.menu_k);
            }
        });

        // ImageView para el menú de registro
        ImageView imageView4 = findViewById(R.id.imageView4);
        imageView4.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                showMenuReg(v);
            }
        });
    }

    private void showPopupMenu(View view, int menuResId) {
        PopupMenu popupMenu = new PopupMenu(this, view);
        popupMenu.getMenuInflater().inflate(menuResId, popupMenu.getMenu());
        popupMenu.setOnMenuItemClickListener(new PopupMenu.OnMenuItemClickListener() {
            @Override
            public boolean onMenuItemClick(MenuItem item) {
                Log.d("MainActivity", "Item seleccionado en menu: " + item.getTitle());
                try {
                    if (item.getItemId() == R.id.formula_1) {
                        startActivity(new Intent(MainActivity.this, F1Activity.class));
                        return true;
                    } else if (item.getItemId() == R.id.menu_c) {
                        startActivity(new Intent(MainActivity.this, CActivity.class));
                        return true;
                    } else if (item.getItemId() == R.id.menu_d) {
                        startActivity(new Intent(MainActivity.this, DActivity.class));
                        return true;
                    } else if (item.getItemId() == R.id.menu_k) {
                        startActivity(new Intent(MainActivity.this, KActivity.class));
                        return true;
                    } else if (item.getItemId() == R.id.menu_l) {
                        startActivity(new Intent(MainActivity.this, LActivity.class));
                        return true;
                    } else if (item.getItemId() == R.id.menu_m) {
                        startActivity(new Intent(MainActivity.this, MActivity.class));
                        return true;
                    } else if (item.getItemId() == R.id.menu_r) {
                        startActivity(new Intent(MainActivity.this, RActivity.class));
                        return true;
                    } else if (item.getItemId() == R.id.menu_sp) {
                        startActivity(new Intent(MainActivity.this, SPActivity.class));
                        return true;
                    } else if (item.getItemId() == R.id.menu_re) {
                        startActivity(new Intent(MainActivity.this, ResistenciaActivity.class));
                        return true;
                    } else {
                        return false;
                    }
                } catch (Exception e) {
                    e.printStackTrace();
                    Toast.makeText(MainActivity.this, "Error al abrir la actividad: " + e.getMessage(), Toast.LENGTH_SHORT).show();
                    return false;
                }
            }
        });
        popupMenu.show();
    }

    private void showMenuReg(View view) {
        PopupMenu popupMenu = new PopupMenu(this, view);
        popupMenu.getMenuInflater().inflate(R.menu.menu_reg, popupMenu.getMenu());
        popupMenu.setOnMenuItemClickListener(new PopupMenu.OnMenuItemClickListener() {
            @Override
            public boolean onMenuItemClick(MenuItem item) {
                Log.d("MainActivity", "Item seleccionado en menu_reg: " + item.getTitle());
                try {
                    if (item.getItemId() == R.id.registro) {
                        mAuth.signOut();
                        startActivity(new Intent(MainActivity.this, LoginActivity.class));
                        finish();
                        return true;
                    } else {
                        return false;
                    }
                } catch (Exception e) {
                    e.printStackTrace();
                    Toast.makeText(MainActivity.this, "Error al abrir la actividad: " + e.getMessage(), Toast.LENGTH_SHORT).show();
                    return false;
                }
            }
        });
        popupMenu.show();
    }
}
