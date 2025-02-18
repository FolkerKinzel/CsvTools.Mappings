using BenchmarkDotNet.Attributes;

namespace Benchmarks;

[MemoryDiagnoser]
[BaselineColumn]
public class ReadBench
{
    public string? CalculationCsv {get; set;}

    [Params(50, 100, 200)]
    public int Count {get; set;}

    [GlobalSetup]
    public void Setup()
    {
        CalculationCsv = Utility.CreateCalculationCsv(Count);
    }

    [Benchmark(Baseline = true)]
    public IList<Calculation> Performance() => CalculationReader.ReadPerformance(CalculationCsv!);

    [Benchmark]
    public IList<Calculation> Default() => CalculationReader.ReadDefault(CalculationCsv!);

    [Benchmark]
    public IList<Calculation> CsvHelper() => CalculationReader.ReadCsvHelper(CalculationCsv!);
}
