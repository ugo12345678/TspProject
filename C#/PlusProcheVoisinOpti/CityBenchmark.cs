using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.IO;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;

[MemoryDiagnoser]
[DisassemblyDiagnoser]  // Retirer le paramètre "printIL"
public class CityBenchmark
{
    public List<City> cities;

    [GlobalSetup]
    public void Setup()
    {
        int cityCount = 30_000;
        string filePath = "cities.csv";

        if (File.Exists(filePath))
        {
            cities = Program.LoadCitiesFromFile(filePath);
        }
        else
        {
            cities = Program.GenerateRandomCities(cityCount);
            Program.SaveCitiesToFile(cities, filePath);
        }
    }

    [Benchmark]
    public List<City> NearestNeighbor()
    {
        return Program.NearestNeighbor(cities);
    }
}
