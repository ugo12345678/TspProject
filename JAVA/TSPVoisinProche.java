import java.io.*;
import java.util.*;

class Ville {
    String nom;
    double x, y;

    public Ville(String nom, double x, double y) {
        this.nom = nom;
        this.x = x;
        this.y = y;
    }
}

class TSPVoisinProche {
    private List<Ville> villes;

    public TSPVoisinProche(List<Ville> villes) {
        this.villes = villes;
    }

    private double distance(Ville v1, Ville v2) {
        return Math.sqrt(Math.pow(v1.x - v2.x, 2) + Math.pow(v1.y - v2.y, 2));
    }

    public void solve() {
        int nombreVilles = villes.size();
        if (nombreVilles == 0) return;

        boolean[] visitees = new boolean[nombreVilles];
        List<Ville> chemin = new ArrayList<>();
        double distanceTotale = 0;

        int villeActuelle = 0;
        chemin.add(villes.get(villeActuelle));
        visitees[villeActuelle] = true;

        long startTime = System.nanoTime();

        for (int i = 1; i < nombreVilles; i++) {
            int villeSuivante = -1;
            double distanceMin = Double.MAX_VALUE;

            for (int j = 0; j < nombreVilles; j++) {
                if (!visitees[j]) {
                    double dist = distance(villes.get(villeActuelle), villes.get(j));
                    if (dist < distanceMin) {
                        distanceMin = dist;
                        villeSuivante = j;
                    }
                }
            }

            villeActuelle = villeSuivante;
            visitees[villeActuelle] = true;
            chemin.add(villes.get(villeActuelle));
            distanceTotale += distanceMin;
        }

        distanceTotale += distance(villes.get(villeActuelle), villes.get(0));
        chemin.add(villes.get(0));

        long endTime = System.nanoTime();
        double executionTime = (endTime - startTime) / 1e6; // en ms

        System.out.printf("Distance totale : %.6f\n", distanceTotale);
        System.out.printf("Temps d'exécution : %.4f ms\n", executionTime);
        System.out.print("Chemin suivi : ");
        for (Ville v : chemin) {
            System.out.print(v.nom + " -> ");
        }
        System.out.println();
    }

    public static List<Ville> lireFichierCSV(String nomFichier) throws IOException {
        List<Ville> villes = new ArrayList<>();
        BufferedReader br = new BufferedReader(new FileReader(nomFichier));
        String ligne;

        while ((ligne = br.readLine()) != null) {
            String[] data = ligne.split("\\|");
            if (data.length != 3) continue;

            String nom = data[0];
            double x = Double.parseDouble(data[1].replace(",", ".")); 
            double y = Double.parseDouble(data[2].replace(",", ".")); 

            villes.add(new Ville(nom, x, y));
        }
        br.close();
        return villes;
    }

    public static void main(String[] args) {
        try {
            List<Ville> villes = lireFichierCSV("../cities.csv");  
            TSPVoisinProche tsp = new TSPVoisinProche(villes);
            tsp.solve();
        } catch (IOException e) {
            System.out.println("Erreur de lecture du fichier : " + e.getMessage());
        }
    }
}
