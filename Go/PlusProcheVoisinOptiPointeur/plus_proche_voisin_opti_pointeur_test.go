package main

import (
	"testing"
)

// Benchmark de l'algorithme du plus proche voisin
func BenchmarkPlusProcheVoisin(b *testing.B) {
	// Charger les villes depuis le fichier
	villes, err := lireVilles("cities.csv")
	if err != nil {
		b.Fatalf("Erreur de lecture du fichier : %v", err)
	}

	// Répéter le benchmark b.N fois
	for i := 0; i < b.N; i++ {
		plusProcheVoisin(villes)
	}
}
