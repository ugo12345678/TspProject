package main

import (
	"bufio"
	"fmt"
	"log"
	"math"
	"os"
	"strconv"
	"strings"
	"time"
)

// Ville représente une ville avec ses coordonnées x et y
type Ville struct {
	nom string
	x   float64
	y   float64
}

// distance calcule la distance euclidienne entre deux villes
func distance(v1, v2 Ville) float64 {
	return math.Sqrt(math.Pow(v1.x-v2.x, 2) + math.Pow(v1.y-v2.y, 2))
}

// plusProcheVoisin résout le problème du voyageur de commerce en utilisant l'heuristique du plus proche voisin
func plusProcheVoisin(villes []Ville) ([]Ville, float64) {
	nombreVilles := len(villes)
	if nombreVilles == 0 {
		return nil, 0
	}

	visitees := make([]bool, nombreVilles)
	tour := make([]Ville, 0, nombreVilles)
	distanceTotale := 0.0

	// Commencer par la première ville
	villeActuelle := 0
	tour = append(tour, villes[villeActuelle])
	visitees[villeActuelle] = true

	for i := 1; i < nombreVilles; i++ {
		villeSuivante := -1
		distanceMin := math.MaxFloat64

		// Trouver la ville la plus proche non visitée
		for j := 0; j < nombreVilles; j++ {
			if !visitees[j] {
				dist := distance(villes[villeActuelle], villes[j])
				if dist < distanceMin {
					distanceMin = dist
					villeSuivante = j
				}
			}
		}

		// Se déplacer vers la ville la plus proche
		villeActuelle = villeSuivante
		visitees[villeActuelle] = true
		tour = append(tour, villes[villeActuelle])
		distanceTotale += distanceMin
	}

	// Retourner à la ville de départ
	distanceTotale += distance(villes[villeActuelle], villes[0])
	tour = append(tour, villes[0])

	return tour, distanceTotale
}

func lireVilles(nomFichier string) ([]Ville, error) {
	fichier, err := os.Open(nomFichier)
	if err != nil {
		log.Fatalf("Erreur lors de l'ouverture du fichier : %v", err)
	}
	defer fichier.Close()

	var villes []Ville
	scanner := bufio.NewScanner(fichier)

	// Lire le fichier ligne par ligne
	for scanner.Scan() {
		ligne := scanner.Text()
		ligneFormated := strings.ReplaceAll(ligne, ",", ".")
		champs := strings.Split(ligneFormated, "|")

		if len(champs) != 3 {
			// log.Printf("Ligne ignorée (nombre de champs incorrect) : %s", ligne)
			continue
		}

		nom := champs[0]
		x, err := strconv.ParseFloat(champs[1], 64)
		if err != nil {
			// log.Printf("Erreur de conversion pour la valeur X de la ville %s : %v", nom, err)
			continue
		}

		y, err := strconv.ParseFloat(champs[2], 64)
		if err != nil {
			// log.Printf("Erreur de conversion pour la valeur Y de la ville %s : %v", nom, err)
			continue
		}

		ville := Ville{nom: nom, x: x, y: y}
		villes = append(villes, ville)
	}

	return villes, nil
}

func main() {
	// Exemple de villes avec leurs coordonnées
	start := time.Now()
	villes, err := lireVilles("../../cities.csv")
	duration := time.Since(start)
	fmt.Println("Temps d'exécution :", duration)
	if err != nil {
		log.Fatalf("Erreur de lecture du fichier : %v", err)
	}

	plusProcheVoisin(villes)
}
