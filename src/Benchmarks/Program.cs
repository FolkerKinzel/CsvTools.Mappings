using System;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Benchmarks;

internal class Program
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnötige Zuweisung eines Werts.", Justification = "<Ausstehend>")]
    private static void Main()
    {
        //Summary summary = BenchmarkRunner.Run<DBNullBenchmarks>();
        //Summary summary = BenchmarkRunner.Run<AccessBenchmark>();
        Summary summary = BenchmarkRunner.Run<WriteBench>();
        Summary summary2 = BenchmarkRunner.Run<ReadBench>();

        Calculation[] calcs = Utility.CreateCalculations(10);
        //string csv = CalculationWriter.WriteCsvHelper(calcs);

        //string csv = Utility.CreateCalculationCsv(10);
        //IList<Calculation> calculations = CalculationReader.ReadCsvHelper(csv);
    }
}
