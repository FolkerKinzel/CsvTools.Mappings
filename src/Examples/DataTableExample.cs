using FolkerKinzel.CsvTools;
using FolkerKinzel.CsvTools.Mappings;
// A namespace alias helps to avoid name conflicts
// with the converters from System.ComponentModel
using Conv = FolkerKinzel.CsvTools.Mappings.TypeConverters;
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
        dataTable.Columns.Add(new DataColumn("lesson start", typeof(TimeOnly)));

        // The DataColumn.Caption property allows you to override the DataColumn.ColumnName property
        // when the ColumnName does not meet C# identifier requirements. The values ​​of the
        // DataColumn.Caption properties must be unique for CSV serialization (case-insensitive,
        // like DataColumn.ColumnName).
        dataTable.Columns["lesson start"]!.Caption = "begin";

        _ = dataTable.Rows.Add(
            [4711, "Susi Meyer", "Piano", DayOfWeek.Wednesday, new TimeOnly(14, 30, 0)]);
        _ = dataTable.Rows.Add(
            [0, "Carl Czerny", "Piano", DayOfWeek.Thursday, new TimeOnly(15, 15, 0)]);
        _ = dataTable.Rows.Add(
            [111, "Frederic Chopin", "Piano"]);

        // Store the stringConverter because you can reuse the same 
        // converter for more than one property in CsvRecordWrapper.
        Conv::TypeConverter<object> stringConverter
            = Conv::StringConverter.CreateNullable().ToDBNullConverter();

        // Each dynamic property name of the Mapping has to have a corresponding column in
        // the DataTable - corresponding in the DataColumn.Caption property (case-insensitive)
        // and the accepted data type. Mapping properties and DataColumns don't need to
        // correspond in their number and order and they don't need to match the columns of
        // the CSV file:
        CsvMapping mapping = CsvMappingBuilder
            .Create()
            .AddProperty("Name", stringConverter)
            .AddProperty("Subject", stringConverter)
            .AddProperty("Day", new Conv::EnumConverter<DayOfWeek>(format: "G")
                                .ToNullableConverter()
                                .ToDBNullConverter())
            .AddProperty("Begin", ["begin", "*start"], new Conv::TimeOnlyConverter()
                                                       .ToNullableConverter()
                                                       .ToDBNullConverter())
            .Build();

        // Write the CSV file:
        // (The column names determine which DataColumns will be part of the CSV
        // and their order in the CSV file.)
        dataTable.WriteCsv(filePath, 
                           ["Subject", "Lesson Start", "Name", "Day", "Reserved"],
                           mapping);

        dataTable.Clear();

        // Refill the DataTable from the CSV-file:
        dataTable.ReadCsv(filePath, mapping);

        Console.WriteLine("Csv file:");
        Console.WriteLine();
        Console.WriteLine(File.ReadAllText(filePath));
        Console.WriteLine();
        Console.WriteLine("Content of the refilled DataTable:");
        Utility.WriteConsole(dataTable);
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
