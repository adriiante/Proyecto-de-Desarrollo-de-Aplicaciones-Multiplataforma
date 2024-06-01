package com.inforace;

import androidx.appcompat.app.AppCompatActivity;
import android.content.Intent;
import android.net.Uri;
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

        // Botón Camiones
        Button botonCamiones = findViewById(R.id.boton_camiones);
        botonCamiones.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                showPopupMenu(v, R.menu.menu_c);
            }
        });

        // Botón Drifting
        Button botonDrifting = findViewById(R.id.boton_drifting);
        botonDrifting.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                showPopupMenu(v, R.menu.menu_d);
            }
        });

        // Botón About Us (boton_f5)
        Button botonF2 = findViewById(R.id.boton_f2);
        botonF2.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                // Navegar a AboutUsActivity
                startActivity(new Intent(MainActivity.this, AyudaActivity.class));
            }
        });

        // Botón Linktree (boton_f4)
        Button botonF4 = findViewById(R.id.boton_f4);
        botonF4.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                // Abrir enlace en navegador
                Intent browserIntent = new Intent(Intent.ACTION_VIEW, Uri.parse("https://linktr.ee/inforace"));
                startActivity(browserIntent);
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
                    } else if (item.getItemId() == R.id.formula_2) {
                        startActivity(new Intent(MainActivity.this, F2Activity.class));
                        return true;
                    } else if (item.getItemId() == R.id.formula_3) {
                        startActivity(new Intent(MainActivity.this, F3Activity.class));
                        return true;
                    } else if (item.getItemId() == R.id.formula_e) {
                        startActivity(new Intent(MainActivity.this, FEActivity.class));
                        return true;
                    } else if (item.getItemId() == R.id.camion_etrc) {
                        startActivity(new Intent(MainActivity.this, CActivity.class));
                        return true;
                    } else if (item.getItemId() == R.id.drifting_formula_drift) {
                        startActivity(new Intent(MainActivity.this, DActivity.class));
                        return true;
                    } else if (item.getItemId() == R.id.drifting_d1_gp) {
                        startActivity(new Intent(MainActivity.this, D1GPActivity.class));
                        return true;
                    } else if (item.getItemId() == R.id.karting_kwc) {
                        startActivity(new Intent(MainActivity.this, KActivity.class));
                        return true;
                    } else if (item.getItemId() == R.id.moto_motogp) {
                        startActivity(new Intent(MainActivity.this, MotoGPActivity.class));
                        return true;
                    } else if (item.getItemId() == R.id.moto_moto2) {
                        startActivity(new Intent(MainActivity.this, Moto2Activity.class));
                        return true;
                    } else if (item.getItemId() == R.id.moto_moto3) {
                        startActivity(new Intent(MainActivity.this, Moto3Activity.class));
                        return true;
                    } else if (item.getItemId() == R.id.rally_wrc) {
                        startActivity(new Intent(MainActivity.this, WRCActivity.class));
                        return true;
                    } else if (item.getItemId() == R.id.rally_x) {
                        startActivity(new Intent(MainActivity.this, RXActivity.class));
                        return true;
                    } else if (item.getItemId() == R.id.wec) {
                        startActivity(new Intent(MainActivity.this, WECActivity.class));
                        return true;
                    } else if (item.getItemId() == R.id.imsa) {
                        startActivity(new Intent(MainActivity.this, IMSAActivity.class));
                        return true;
                    } else if (item.getItemId() == R.id.sportcar_gt3) {
                        startActivity(new Intent(MainActivity.this, GT3Activity.class));
                        return true;
                    } else if (item.getItemId() == R.id.sportcar_nascar) {
                        startActivity(new Intent(MainActivity.this, NASCARActivity.class));
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
