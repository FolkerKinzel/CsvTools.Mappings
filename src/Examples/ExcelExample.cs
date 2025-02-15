using System.Globalization;
using FolkerKinzel.CsvTools;
using FolkerKinzel.CsvTools.Mappings;
using Conv = FolkerKinzel.CsvTools.Mappings.TypeConverters;

namespace Examples;

internal sealed record Customer(string Name, decimal Sales, DateOnly RecentPurchase);

internal static class ExcelExample
{
    internal static void ExportingCsvToExcel(string filePath)
    {
        // Which field separators and formatting Excel accepts and exports depends on the
        // "Regional Settings" in Excel. The default setting corresponds to the settings
        // of the user's computer (and thus of CultureInfo.CurrentCulture).

        // Using CultureInfo.CurrentCulture, the corresponding parameters can be determined
        // automatically.

        // The application is free to override the value of CurrentCulture for the current
        // Thread if users do not use the default setting in Excel.

        // This example shows the procedure for exporting CSV data to Excel. The procedure
        // for importing is equivalent.(The console output is from a computer with the locale
        // "de-DE".)

        Console.WriteLine("Current culture: {0}", CultureInfo.CurrentCulture);

        Customer[] customers = [ new("Susi", 4_711m, new DateOnly(2004, 3, 14)),
                                 new("Tom", 38_527.28m, new DateOnly(2006, 12, 24)),
                                 new("Rachel", 25.8m, new DateOnly(2011, 8, 27)) ];

        // Get the Excel arguments for CultureInfo.CurrentCulture:
        (char delimiter, IFormatProvider formatProvider) = Csv.GetExcelArguments();

        // Pass the formatProvider from the Excel arguments to all localizable converters.
        // (The same CsvMapping could be used for parsing too.)
        CsvMapping mapping = CsvMappingBuilder
            .Create()
            .AddProperty("Name", Conv::StringConverter.CreateNonNullable())
            .AddProperty("Sales", new Conv::DecimalConverter(formatProvider))
            .AddProperty("RecentPurchase", new Conv::DateOnlyConverter(formatProvider))
            .Build();

        static void Conversion(Customer customer, dynamic dyn)
        {
            dyn.Name = customer.Name;
            dyn.Sales = customer.Sales;
            dyn.RecentPurchase = customer.RecentPurchase;
        }

        // Don't forget to pass the delimiter from the Excel arguments:
        customers.SaveCsv(filePath, mapping, Conversion, delimiter);

        Console.WriteLine();
        Console.WriteLine(File.ReadAllText(filePath));
    }
}

/*
 Console output:

Current culture: de-DE

Name;Sales;RecentPurchase
Susi;4711;14.03.2004
Tom;38527,28;24.12.2006
Rachel;25,8;27.08.2011
*/
