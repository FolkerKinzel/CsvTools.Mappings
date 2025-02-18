using BenchmarkDotNet.Attributes;

namespace Benchmarks;

[MemoryDiagnoser]
[BaselineColumn]
public class WriteBench
{
    public Calculation[]? Calculations {get; set;}

    [Params(50, 100, 200)]
    public int Count {get; set;}

    [GlobalSetup]
    public void Setup()
    {
        Calculations = Utility.CreateCalculations(Count);
    }

    [Benchmark(Baseline = true)]
    public string Performance() => CalculationWriter.WritePerformance(Calculations!);

    [Benchmark]
    public string Default() => CalculationWriter.WriteDefault(Calculations!);

    [Benchmark]
    public string CsvHelper() => CalculationWriter.WriteCsvHelper(Calculations!);
}
