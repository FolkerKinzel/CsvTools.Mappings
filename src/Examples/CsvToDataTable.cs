using System;
using System.Data;
using System.Globalization;
using System.IO;
using FolkerKinzel.CsvTools;
using FolkerKinzel.CsvTools.Mappings;
using FolkerKinzel.CsvTools.Mappings.Converters;

namespace Examples;

internal static class CsvToDataTable
{
    private const string PUPILS_NAME = "Name";
    private const string SUBJECT = "Subject";
    private const string LESSON_DAY = "Day";
    private const string LESSON_BEGIN = "Begin";
    private const string FILE_NAME = "DataTable.csv";

    public static void TestCsvToDataTable()
    {
        using DataTable dataTable = InitDataTable();

        Mapping mapping = InitMapping();

        // Write the CSV file:
        // (We can sort the columns of the CSV file differently than those 
        // of the DataTable - CsvRecordWrapper will reorder that.)
        string[] columns =
            [SUBJECT, LESSON_BEGIN, PUPILS_NAME, LESSON_DAY];

        using (CsvWriter writer = Csv.OpenWrite(FILE_NAME, columns))
        {
            // (CsvWriter reuses the same record.)
            mapping.Record = writer.Record;

            foreach (DataRow dataRow in dataTable.Rows)
            {
                // The properties of the CsvRecordWrapper match the columns
                // of the DataTable in data type and order (but not the 
                // columns of the CSV file).
                mapping.FillWith(dataRow);
                writer.WriteRecord();
            }
        }

        dataTable.Clear();

        // Refill the DataTable from the CSV-file:
        using CsvReader reader = Csv.OpenRead(FILE_NAME);

        foreach (CsvRecord record in reader)
        {
            mapping.Record = record;
            DataRow dataRow = dataTable.NewRow();
            dataTable.Rows.Add(dataRow);

            // It doesn't matter that the columns in the CSV file have a
            // different order than the columns of the DataTable:
            // CsvRecordWrapper reorders that for us.
            for (int i = 0; i < mapping.Count; i++)
            {
                dataRow[i] = mapping[i].Value;
            }

        }

        WriteConsole(dataTable);

        // Console output:
        // Csv file:
        //
        // Subject,Begin,Name,Day
        // Piano,14:30:00,Susi Meyer, Wednesday
        // Piano,15:15:00,Carl Czerny, Thursday
        // ,, Frederic Chopin,
        //
        //
        // Content of the refilled DataTable:
        // Susi Meyer      Piano           3               14:30:00
        // Carl Czerny     Piano           4               15:15:00
        // Frederic Chopin <DBNull>        <DBNull>       <DBNull>
    }


    private static Mapping InitMapping()
    {
        // Store the stringConverter because you can reuse the same 
        // converter for more than one property in CsvRecordWrapper.
        TypeConverter<object> stringConverter
            = StringConverter.CreateNonNullable().ToDBNullConverter();

        return Mapping
            .Create()
            .AddProperty(PUPILS_NAME, stringConverter)
            .AddProperty(SUBJECT, stringConverter)
            .AddProperty(LESSON_DAY, new EnumConverter<DayOfWeek>(format: "G").ToDBNullConverter())
            .AddProperty(LESSON_BEGIN, new TimeSpanConverter().ToDBNullConverter());
    }


    private static DataTable InitDataTable()
    {
        var dataTable = new DataTable();

        dataTable.Columns.Add(new DataColumn(PUPILS_NAME));
        dataTable.Columns.Add(new DataColumn(SUBJECT));
        dataTable.Columns.Add(new DataColumn(LESSON_DAY, typeof(DayOfWeek)));
        dataTable.Columns.Add(new DataColumn(LESSON_BEGIN, typeof(TimeSpan)));

        _ = dataTable.Rows.Add(
            ["Susi Meyer", "Piano", DayOfWeek.Wednesday, new TimeSpan(14, 30, 0)]);
        _ = dataTable.Rows.Add(
            ["Carl Czerny", "Piano", DayOfWeek.Thursday, new TimeSpan(15, 15, 0)]);
        _ = dataTable.Rows.Add(
            ["Frederic Chopin"]);

        return dataTable;
    }


    private static void WriteConsole(DataTable dataTable)
    {
        Console.WriteLine("Csv file:");
        Console.WriteLine();
        Console.WriteLine(File.ReadAllText(FILE_NAME));

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
                    TimeSpan ts => ts.ToString("g", CultureInfo.InvariantCulture).PadRight(padding),
                    _ => o.ToString()?.PadRight(padding)
                });
                Console.Write(' ');
            }

            Console.WriteLine();
        }
    }
}
