using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

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

public class TSPForm : Form
{
    private List<City> cities;
    private List<City> tour = new List<City>();
    private Thread algoThread;
    private object drawLock = new object();
    private int step = 0;

    public TSPForm()
    {
        this.Text = "TSP - Nearest Neighbor Visualization";
        this.Width = 800;
        this.Height = 800;
        this.DoubleBuffered = true;

        string filePath = "cities.csv";
        if (File.Exists(filePath))
        {
            cities = LoadCitiesFromFile(filePath);
        }
        else
        {
            cities = GenerateRandomCities(200); // Pas 30 000, sinon lag...
            SaveCitiesToFile(cities, filePath);
        }

        algoThread = new Thread(() => RunAlgorithm());
        algoThread.Start();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        if (cities == null || cities.Count == 0) return;

        Graphics g = e.Graphics;
        lock (drawLock)
        {
            foreach (var city in cities)
            {
                g.FillEllipse(Brushes.Blue, TransformX(city.X) - 3, TransformY(city.Y) - 3, 6, 6);
            }

            for (int i = 0; i < step - 1 && i < tour.Count - 1; i++)
            {
                var a = tour[i];
                var b = tour[i + 1];
                g.DrawLine(Pens.Red, TransformX(a.X), TransformY(a.Y), TransformX(b.X), TransformY(b.Y));
            }
        }
    }

    private int TransformX(double x) => (int)(x * 7) + 50;
    private int TransformY(double y) => (int)(y * 7) + 50;

    private void RunAlgorithm()
    {
        bool[] visited = new bool[cities.Count];
        int current = 0;

        lock (drawLock)
        {
            tour.Add(cities[current]);
            visited[current] = true;
        }

        for (int i = 1; i < cities.Count; i++)
        {
            int nearest = -1;
            double minDist = double.MaxValue;

            for (int j = 0; j < cities.Count; j++)
            {
                if (!visited[j])
                {
                    double dist = Distance(cities[current], cities[j]);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        nearest = j;
                    }
                }
            }

            current = nearest;
            lock (drawLock)
            {
                tour.Add(cities[current]);
                visited[current] = true;
                step++;
            }

            this.Invoke(new Action(() => this.Invalidate()));
            Thread.Sleep(10); // Pour l’effet visuel
        }

        lock (drawLock)
        {
            tour.Add(tour[0]); // Retour au départ
            step++;
        }

        this.Invoke(new Action(() => this.Invalidate()));
    }

    private double Distance(City a, City b)
    {
        double dx = a.X - b.X;
        double dy = a.Y - b.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    private List<City> GenerateRandomCities(int count)
    {
        Random rand = new Random();
        List<City> cities = new List<City>();

        for (int i = 0; i < count; i++)
        {
            cities.Add(new City($"City_{i}", rand.NextDouble() * 100, rand.NextDouble() * 100));
        }
        return cities;
    }

    private void SaveCitiesToFile(List<City> cities, string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (var city in cities)
            {
                writer.WriteLine($"{city.Name},{city.X},{city.Y}");
            }
        }
    }

    private List<City> LoadCitiesFromFile(string filePath)
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
}

static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.Run(new TSPForm());
    }
}
