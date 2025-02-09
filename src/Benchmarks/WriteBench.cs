using BenchmarkDotNet.Attributes;

namespace Benchmarks;

[MemoryDiagnoser]
[BaselineColumn]
public class WriteBench
{
    public Calculation[]? Calculations {get; set;}

    [Params(200)]
    public int Count {get; set;}

    [GlobalSetup]
    public void Setup()
    {
        Calculations = Utility.CreateCalculations(Count);
    }

    [Benchmark(Baseline = true)]
    public string Default() => CalculationWriter.WriteDefault(Calculations!);

    [Benchmark]
    public string Performance() => CalculationWriter.WritePerformance(Calculations!);
}
