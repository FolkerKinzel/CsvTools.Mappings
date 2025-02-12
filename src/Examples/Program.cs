using System.Diagnostics.CodeAnalysis;

namespace Examples;

internal sealed class Program
{
    private static void Main()
    {
        //DataTableExample.DataTableWriteReadCsv("DataTable.csv");
        ObjectSerializationExample.CsvReadWritePupils("Objects.csv");
        //MultiColumnConverterExample.ParseDataFromSeveralCsvColumns();
    }
}
