package main

import (
	"bufio"
	"log"
	"math"
	"os"
	"strconv"
	"strings"
)

// Ville représente une ville avec ses coordonnées x et y
type Ville struct {
	nom  string
	x, y float64
}

// distance calcule la distance euclidienne entre deux villes
func distance(v1, v2 Ville) float64 {
	dx, dy := v1.x-v2.x, v1.y-v2.y
	return math.Sqrt(dx*dx + dy*dy)
}

// plusProcheVoisin résout le problème du voyageur de commerce
func plusProcheVoisin(villes []Ville) ([]Ville, float64) {
	n := len(villes)
	if n == 0 {
		return nil, 0
	}

	visitees := make([]bool, n)
	tour := make([]Ville, 0, n)
	distanceTotale := 0.0

	// Commencer par la première ville
	villeActuelle := 0
	tour = append(tour, villes[villeActuelle])
	visitees[villeActuelle] = true

	for i := 1; i < n; i++ {
		villeSuivante, distanceMin := -1, math.MaxFloat64

		for j := 0; j < n; j++ {
			if !visitees[j] {
				dist := distance(villes[villeActuelle], villes[j])
				if dist < distanceMin {
					distanceMin, villeSuivante = dist, j
				}
			}
		}

		villeActuelle = villeSuivante
		visitees[villeActuelle] = true
		tour = append(tour, villes[villeActuelle])
		distanceTotale += distanceMin
	}

	distanceTotale += distance(villes[villeActuelle], villes[0])
	tour = append(tour, villes[0])

	return tour, distanceTotale
}

// lireVilles lit un fichier CSV et retourne une liste de villes
func lireVilles(nomFichier string) ([]Ville, error) {
	fichier, err := os.Open(nomFichier)
	if err != nil {
		return nil, err
	}
	defer fichier.Close()

	villes := make([]Ville, 0, 30000)
	reader := bufio.NewReader(fichier)

	for {
		ligne, err := reader.ReadString('\n')
		if err != nil {
			break
		}
		ligneFormated := strings.ReplaceAll(ligne, ",", ".")
		champs := strings.Split(strings.TrimSpace(ligneFormated), "|")
		if len(champs) != 3 {
			continue
		}
		x, err1 := strconv.ParseFloat(champs[1], 64)
		y, err2 := strconv.ParseFloat(champs[2], 64)
		if err1 != nil || err2 != nil {
			continue
		}
		villes = append(villes, Ville{nom: champs[0], x: x, y: y})
	}

	return villes, nil
}

func main() {
	villes, err := lireVilles("../../cities.csv")
	if err != nil {
		log.Fatalf("Erreur de lecture du fichier : %v", err)
	}

	plusProcheVoisin(villes)
}
