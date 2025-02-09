using BenchmarkDotNet.Attributes;

namespace Benchmarks;

[MemoryDiagnoser]
[BaselineColumn]
public class ReadBench
{
    public string? CalculationCsv {get; set;}

    [Params(200)]
    public int Count {get; set;}

    [GlobalSetup]
    public void Setup()
    {
        CalculationCsv = Utility.CreateCalculationCsv(Count);
    }

    [Benchmark(Baseline = true)]
    public IList<Calculation> Default() => CalculationReader.ReadDefault(CalculationCsv!);

    [Benchmark]
    public IList<Calculation> Performance() => CalculationReader.ReadPerformance(CalculationCsv!);
}
