// Top-level build file where you can add configuration options common to all sub-projects/modules.
plugins {
    id("com.android.application") version "8.1.2" apply false
    id("com.google.gms.google-services") version "4.4.1" apply false
}
buildscript {
    repositories {
        // Otras repositorios aquí
        google() // Agrega este repositorio
    }
    // Otras configuraciones aquí
}
