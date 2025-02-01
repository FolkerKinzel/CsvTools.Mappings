using FolkerKinzel.CsvTools;
using FolkerKinzel.CsvTools.Mappings;
using FolkerKinzel.CsvTools.Mappings.Converters;
using System.Data;
using System.Globalization;

namespace Examples;

internal static class DataTableExample
{
    public static void DataTableWriteReadCsv(string filePath)
    {
        using var dataTable = new DataTable();

        dataTable.Columns.Add(new DataColumn("not_used", typeof(int)));
        dataTable.Columns.Add(new DataColumn("name"));
        dataTable.Columns.Add(new DataColumn("subject"));
        dataTable.Columns.Add(new DataColumn("day", typeof(DayOfWeek)));
        dataTable.Columns.Add(new DataColumn("begin", typeof(TimeOnly)));

        _ = dataTable.Rows.Add(
            [4711, "Susi Meyer", "Piano", DayOfWeek.Wednesday, new TimeOnly(14, 30, 0)]);
        _ = dataTable.Rows.Add(
            [0, "Carl Czerny", "Piano", DayOfWeek.Thursday, new TimeOnly(15, 15, 0)]);
        _ = dataTable.Rows.Add(
            [111, "Frederic Chopin", "Piano"]);

        // Store the stringConverter because you can reuse the same 
        // converter for more than one property in CsvRecordWrapper.
        TypeConverter<object> stringConverter
            = StringConverter.CreateNullable().ToDBNullConverter();

        // All properties of the Mapping have to have a corresponding column
        // in the DataTable (corresponding in the case-insensitive ColumnName
        // and the accepted data type). They dont't need to correspond in their
        // order and they don't need to match neither the columns of the CSV file
        // nor all DataColumns of the DataTable:
        Mapping mapping = Mapping
            .Create()
            .AddProperty("Name", stringConverter)
            .AddProperty("Subject", stringConverter)
            .AddProperty("Day", new EnumConverter<DayOfWeek>(format: "G").ToDBNullConverter())
            .AddProperty("Begin", ["begin", "*start"], new TimeOnlyConverter().ToDBNullConverter());

        // Write the CSV file:
        // (The column names provided when initalizing the CsvWriter determine
        // which DataColumns will be part of the CSV and their order in the CSV file.)
        string[] columns =
            ["Subject", "Lesson Start", "Name", "Day", "Reserved"];

        using (CsvWriter writer = Csv.OpenWrite(filePath, columns))
        {
            dataTable.WriteCsv(writer, mapping);
        }

        dataTable.Clear();

        // Refill the DataTable from the CSV-file:
        using CsvReader reader = Csv.OpenRead(filePath);
        dataTable.ReadCsv(reader, mapping);

        Console.WriteLine("Csv file:");
        Console.WriteLine();
        Console.WriteLine(File.ReadAllText(filePath));

        WriteConsole(dataTable);
    }

    private static void WriteConsole(DataTable dataTable)
    {
        Console.WriteLine();
        Console.WriteLine("Content of the refilled DataTable:");

        foreach (DataRow? dataRow in dataTable.Rows)
        {
            if (dataRow is null)
            {
                continue;
            }

            const int padding = 15;

            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                object o = dataRow[i];
                Console.Write(o switch
                {
                    null => "<null>".PadRight(padding),
                    DBNull dBNull => "<DBNull>".PadRight(padding),
                    string s when s.Length == 0 => "\"\"".PadRight(padding),
                    TimeOnly ts => ts.ToString(CultureInfo.InvariantCulture).PadRight(padding),
                    _ => o.ToString()?.PadRight(padding)
                });
                Console.Write(' ');
            }

            Console.WriteLine();
        }
    }

 /* 
Console output:

Csv file:

Subject,Lesson Start,Name,Day,Reserved
Piano,14:30:00,Susi Meyer,Wednesday,
Piano,15:15:00,Carl Czerny,Thursday,
Piano,,Frederic Chopin,,

Content of the refilled DataTable:
<DBNull>        Susi Meyer      Piano           3               14:30
<DBNull>        Carl Czerny     Piano           4               15:15
<DBNull>        Frederic Chopin Piano           <DBNull>        <DBNull>
*/
}
