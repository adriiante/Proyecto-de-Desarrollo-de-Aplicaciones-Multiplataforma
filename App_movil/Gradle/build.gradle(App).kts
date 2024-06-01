plugins {
    id("com.android.application")
    id("com.google.gms.google-services") // Agrega el plugin de servicios de Google
}

android {
    namespace = "com.inforace"
    compileSdk = 34

    defaultConfig {
        applicationId = "com.inforace"
        minSdk = 24
        targetSdk = 33
        versionCode = 1
        versionName = "1.0"

        testInstrumentationRunner = "androidx.test.runner.AndroidJUnitRunner"
    }

    buildTypes {
        release {
            isMinifyEnabled = false
            proguardFiles(getDefaultProguardFile("proguard-android-optimize.txt"), "proguard-rules.pro")
        }
    }
    compileOptions {
        sourceCompatibility = JavaVersion.VERSION_1_8
        targetCompatibility = JavaVersion.VERSION_1_8
    }
}

dependencies {
    // Importa el BoM de Firebase
    implementation(platform("com.google.firebase:firebase-bom:33.0.0"))

    // Agrega las dependencias de los productos de Firebase que deseas utilizar
    // Cuando uses el BoM, no especifiques las versiones en las dependencias de Firebase
    implementation("com.google.firebase:firebase-analytics")

    implementation("androidx.appcompat:appcompat:1.6.1")
    implementation("com.google.android.material:material:1.4.0") // Actualizado a la última versión
    implementation("androidx.constraintlayout:constraintlayout:2.1.3")
    implementation("com.google.firebase:firebase-firestore:25.0.0")
    implementation("com.google.firebase:firebase-auth:23.0.0")
    testImplementation("junit:junit:4.13.2")
    implementation ("com.google.firebase:firebase-auth:21.0.0")
    androidTestImplementation("androidx.test.ext:junit:1.1.3") // Actualizado a la última versión
    androidTestImplementation("androidx.test.espresso:espresso-core:3.4.0") // Actualizado a la última versión
}
