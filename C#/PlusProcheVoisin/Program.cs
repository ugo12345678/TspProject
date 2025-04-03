using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

class City
{
    public string Name { get; set; }
    public double X { get; set; }
    public double Y { get; set; }

    public City(string name, double x, double y)
    {
        Name = name;
        X = x;
        Y = y;
    }
}

class Program
{
    static void Main()
    {
        int cityCount = 30_000; // Nombre de villes
        string filePath = "cities.csv"; // Fichier pour stocker les villes

        List<City> cities;
        if (File.Exists(filePath))
        {
            Console.WriteLine("Chargement des villes depuis le fichier...");
            cities = LoadCitiesFromFile(filePath);
        }
        else
        {
            Console.WriteLine("Génération des villes...");
            cities = GenerateRandomCities(cityCount);
            SaveCitiesToFile(cities, filePath);
        }

        Console.WriteLine($"Nombre de villes : {cities.Count}");
        
        Stopwatch stopwatch = Stopwatch.StartNew(); // Démarrer le chronomètre
        List<City> tour = NearestNeighbor(cities);
        stopwatch.Stop(); // Arrêter le chronomètre
        
        double totalDistance = CalculateTourDistance(tour);
        
        Console.WriteLine($"Distance totale du trajet : {totalDistance:F2} km");
        Console.WriteLine($"Temps d'exécution : {stopwatch.ElapsedMilliseconds} ms");
    }

    static List<City> GenerateRandomCities(int count)
    {
        Random rand = new Random();
        List<City> cities = new List<City>();

        for (int i = 0; i < count; i++)
        {
            cities.Add(new City($"City_{i}", rand.NextDouble() * 100, rand.NextDouble() * 100));
        }
        Console.WriteLine("Villes générées !");
        return cities;
    }

    static void SaveCitiesToFile(List<City> cities, string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (var city in cities)
            {
                writer.WriteLine($"{city.Name},{city.X},{city.Y}");
            }
        }
        Console.WriteLine("Villes sauvegardées !");
    }

    static List<City> LoadCitiesFromFile(string filePath)
    {
        List<City> cities = new List<City>();
        string[] lines = File.ReadAllLines(filePath);

        foreach (string line in lines)
        {
            var parts = line.Split(',');
            cities.Add(new City(parts[0], double.Parse(parts[1]), double.Parse(parts[2])));
        }
        Console.WriteLine("Villes chargées !");
        return cities;
    }

    static List<City> NearestNeighbor(List<City> cities)
    {
        List<City> tour = new List<City> { cities[0] }; // Départ de la première ville
        HashSet<City> remaining = new HashSet<City>(cities.Skip(1));

        int iteration = 0;
        while (remaining.Count > 0)
        {
            City lastVisited = tour.Last();
            City nearestCity = remaining.OrderBy(city => Distance(lastVisited, city)).First();

            tour.Add(nearestCity);
            remaining.Remove(nearestCity);

            iteration++;
            if (iteration % 1000 == 0 || remaining.Count == 0)
            {
                Console.WriteLine($"Villes restantes : {remaining.Count}");
            }
        }

        tour.Add(tour[0]); // Retour à la ville de départ
        return tour;
    }

    static double Distance(City a, City b)
    {
        return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
    }

    static double CalculateTourDistance(List<City> tour)
    {
        double total = 0;
        for (int i = 0; i < tour.Count - 1; i++)
        {
            total += Distance(tour[i], tour[i + 1]);
        }
        return total;
    }
}
