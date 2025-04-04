using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using BenchmarkDotNet.Running;


public class City
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
    public static void Main()
    {
        int cityCount = 30_000; // Nombre de villes
        string filePath = "cities.csv"; // Fichier de stockage

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

        Stopwatch stopwatch = Stopwatch.StartNew(); // Démarrer le chrono
        List<City> tour = NearestNeighbor(cities);
        stopwatch.Stop(); // Arrêter le chrono

        double totalDistance = CalculateTourDistance(tour);

        Console.WriteLine($"Distance totale du trajet : {totalDistance:F2} km");
        Console.WriteLine($"Temps d'exécution : {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"Launch Benchmark");
        BenchmarkRunner.Run<CityBenchmark>();
    }

    public static List<City> GenerateRandomCities(int count)
    {
        Random rand = new Random();
        List<City> cities = new List<City>();

        for (int i = 0; i < count; i++)
        {
            cities.Add(new City($"City_{i}", rand.NextDouble() * 100, rand.NextDouble() * 100));
        }
        return cities;
    }

    public static void SaveCitiesToFile(List<City> cities, string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (var city in cities)
            {
                writer.WriteLine($"{city.Name},{city.X},{city.Y}");
            }
        }
    }

    public static List<City> LoadCitiesFromFile(string filePath)
    {
        List<City> cities = new List<City>();
        string[] lines = File.ReadAllLines(filePath);

        foreach (string line in lines)
        {
            var parts = line.Split(',');
            cities.Add(new City(parts[0], double.Parse(parts[1]), double.Parse(parts[2])));
        }
        return cities;
    }

    public static List<City> NearestNeighbor(List<City> cities)
    {
        int count = cities.Count;
        if (count == 0) return new List<City>();

        List<City> tour = new List<City>(count + 1);
        bool[] visited = new bool[count];

        int currentCityIndex = 0;
        tour.Add(cities[currentCityIndex]);
        visited[currentCityIndex] = true;

        for (int i = 1; i < count; i++)
        {
            int nearestCityIndex = -1;
            double minDistance = double.MaxValue;

            for (int j = 0; j < count; j++)
            {
                if (!visited[j])
                {
                    double dist = Distance(cities[currentCityIndex], cities[j]);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        nearestCityIndex = j;
                    }
                }
            }

            currentCityIndex = nearestCityIndex;
            visited[currentCityIndex] = true;
            tour.Add(cities[currentCityIndex]);
        }

        // Retour à la ville de départ
        tour.Add(tour[0]);
        return tour;
    }

    static double Distance(City a, City b)
    {
        double dx = a.X - b.X;
        double dy = a.Y - b.Y;
        return Math.Sqrt(dx * dx + dy * dy);
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
