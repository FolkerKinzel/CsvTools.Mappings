using FolkerKinzel.CsvTools.Mappings.Intls;
using System.Data;

namespace FolkerKinzel.CsvTools.Mappings;

public static class DataTableExtension
{

    public static void ReadCsv(this DataTable dataTable, CsvReader reader, Mapping mapping)
    {
        _ArgumentNullException.ThrowIfNull(dataTable, nameof(dataTable));
        _ArgumentNullException.ThrowIfNull(reader, nameof(reader));
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));

        foreach (CsvRecord record in reader)
        {
            mapping.Record = record;
            DataRow dataRow = dataTable.NewRow();
            dataRow.BeginEdit();
            dataTable.Rows.Add(dataRow);

            Span<object?> rowSpan = dataRow.ItemArray;

            // It doesn't matter that the columns in the CSV file have a
            // different order than the columns of the DataTable:
            // CsvRecordWrapper reorders that for us.
            for (int i = 0; i < mapping.Count; i++)
            {
                dataRow[i] = mapping[i].Value;
            }
        }

        dataTable.AcceptChanges();
    }


    public static void WriteCsv(this DataTable dataTable, CsvWriter writer, Mapping mapping)
    {
        _ArgumentNullException.ThrowIfNull(dataTable, nameof(dataTable));
        _ArgumentNullException.ThrowIfNull(writer, nameof(writer));
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));

        mapping.Record = writer.Record;

        DataRowCollection rows = dataTable.Rows;

        for (int i = 0; i < rows.Count; i++)
        {
            mapping.FillWith(rows[i]);
            writer.WriteRecord();
        }
    }
}