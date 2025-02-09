namespace FolkerKinzel.CsvTools.Mappings.Tests;

internal static class Utility
{
    internal static CsvRecord CreateCsvRecord(int columnsCount = 0)
    {
        using var writer = new StringWriter();
        using var csvWriter = new CsvWriter(writer, columnsCount);

        return csvWriter.Record;
    }

    internal static CsvRecord CreateCsvRecord(IEnumerable<string?> columnNames, bool caseSensitive = false)
    {
        using var writer = new StringWriter();
        using var csvWriter = new CsvWriter(writer, columnNames, caseSensitive);

        return csvWriter.Record;
    }
}
