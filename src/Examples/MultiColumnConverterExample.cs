using FolkerKinzel.CsvTools;
using FolkerKinzel.CsvTools.Mappings;
using FolkerKinzel.CsvTools.Mappings.Converters;
using System.Diagnostics;
using System.Drawing;

namespace Examples;

internal static class MultiColumnConverterExample
{
    private sealed class ColorConverter(Mapping mapping)
        : MultiColumnTypeConverter<Color>(mapping, Color.Transparent, true)
    {
        public override bool AcceptsNull => false;

        protected override void DoConvertToCsv(Color value)
        {
            dynamic mapping = Mapping;

            mapping.A = value.A;
            mapping.R = value.R;
            mapping.G = value.G;
            mapping.B = value.B;
        }

        protected override bool TryParse(out Color result)
        {
            dynamic mapping = Mapping;

            try
            {
                result = Color.FromArgb(mapping.A, mapping.R, mapping.G, mapping.B);
                return true;
            }
            catch (FormatException)
            {
                result = DefaultValue;
                return false;
            }
        }
    }

    internal static void ParseDataFromSeveralCsvColumns()
    {
        TypeConverter<byte> byteConverter = new ByteConverter().ToHexConverter();
        Mapping colorMapping = Mapping.Create()
                                      .AddProperty("A", byteConverter)
                                      .AddProperty("R", byteConverter)
                                      .AddProperty("G", byteConverter)
                                      .AddProperty("B", byteConverter);
        var colorConverter = new ColorConverter(colorMapping);

        Mapping mapping = Mapping
            .Create()
            .AddProperty("ColorName", ["Name"], StringConverter.CreateNullable())
            .AddProperty("Color", colorConverter);

        DirectoryInfo tmpDirectory = Directory.CreateTempSubdirectory();
        string csvPath = Path.Combine(tmpDirectory.FullName, "Colors.csv");

        CreateCsvFile(csvPath, mapping);
        Console.WriteLine(File.ReadAllText(csvPath));

        ShowCsvContentInBrowser(mapping, csvPath);

        Thread.Sleep(5000);
        tmpDirectory.Delete(true);
    }

    private static void CreateCsvFile(string csvPath, dynamic mapping)
    {
        using CsvWriter writer = Csv.OpenWrite(csvPath, ["Name", "A", "R", "G", "B"]);
        mapping.Record = writer.Record;

        mapping.ColorName = nameof(Color.CornflowerBlue);
        mapping.Color = Color.CornflowerBlue;
        writer.WriteRecord();

        mapping.ColorName = nameof(Color.LawnGreen);
        mapping.Color = Color.LawnGreen;
        writer.WriteRecord();

        mapping.ColorName = nameof(Color.Salmon);
        mapping.Color = Color.Salmon;
        writer.WriteRecord();
    }

    private static void ShowCsvContentInBrowser(Mapping mapping, string csvPath)
    {
        string htmlPath = Path.Combine(Path.GetDirectoryName(csvPath) ?? "", "colors.htm");
        CreateHtmlFile(htmlPath, csvPath, mapping);
        _ = Process.Start(new ProcessStartInfo { FileName = htmlPath, UseShellExecute = true });
    }

    private static void CreateHtmlFile(string htmlPath, string csvPath, dynamic mapping)
    {
        var htmlFile = new FileInfo(htmlPath);
        using StreamWriter writer = htmlFile.AppendText();

        writer.WriteLine("""
            <html>
            <head>
            <title>Colors From CSV</title>
            <style>
            table { font-size: 200%; }
            th, td { padding: 30px; }
            </style>
            </head>
            <body>
            <table>
            <thead>
            <tr><th>Name</th><th>Color</th></tr>
            </thead>
            <tbody>
            """);

        using CsvReader csvReader = Csv.OpenRead(csvPath);

        foreach (CsvRecord record in csvReader)
        {
            mapping.Record = record;
            writer.Write("<tr><td>");
            writer.Write(mapping.ColorName);
            writer.Write("</td>");
            writer.Write("<td style=\"background-color: #");
            writer.Write((mapping.Color.ToArgb() & 0xFFFFFF).ToString("x"));
            writer.Write(";\" />");
            writer.WriteLine("</td></tr>");
        }

        writer.WriteLine("""
            </tbody>
            </table>
            </body>
            </html>
            """);
    }
}

/*
 Console Output:

Name,A,R,G,B
CornflowerBlue,FF,64,95,ED
LawnGreen,FF,7C,FC,0
Salmon,FF,FA,80,72

*/
