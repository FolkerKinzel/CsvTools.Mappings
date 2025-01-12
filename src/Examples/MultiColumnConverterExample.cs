using FolkerKinzel.CsvTools;
using FolkerKinzel.CsvTools.Mappings;
using FolkerKinzel.CsvTools.Mappings.Converters;
using System.Diagnostics;
using System.Drawing;

namespace Examples;

internal static class MultiColumnConverterExample
{
    private class ColorConverter(Mapping mapping) 
        : MultiColumnTypeConverter<Color>(mapping, Color.Transparent, true)
    {
        public override bool AllowsNull => false;

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
            catch(FormatException)
            {
                result = FallbackValue;
                return false;
            }
        }
    }

    internal static void ParseDataFromSeveralColumns()
    {
        const string csv = """
            Name,A,R,G,B
            CornflowerBlue,FF,64,95,ED
            LawnGreen,FF,7C,FC,00
            Salmon,FF,FA,80,72
            """;

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

        DirectoryInfo htmlDirectory = Directory.CreateTempSubdirectory();
        string htmlPath = Path.Combine(htmlDirectory.FullName, "colors.htm");

        CreateHtmlFile(htmlPath, mapping, csv);
        Console.WriteLine(File.ReadAllText(htmlPath));
        _ = Process.Start(new ProcessStartInfo { FileName = htmlPath, UseShellExecute = true });

        Thread.Sleep(2000);
        htmlDirectory.Delete(true);
    }

    private static void CreateHtmlFile(string htmlPath, Mapping mapping, string csv)
    {
        var htmlFile = new FileInfo(htmlPath);
        using StreamWriter writer = htmlFile.AppendText();

        writer.WriteLine("""
            <html>
            <head>
            <title>Colors From CSV</title>
            </head>
            <body>
            <table style="font-size: 200%;">
            <thead>
            <tr><th>Name</th><th>Color</th></tr>
            </thead>
            <tbody>
            """);

        using var stringReader = new StringReader(csv);
        using CsvReader csvReader = Csv.OpenRead(stringReader);
        dynamic map = mapping;

        foreach (CsvRecord record in csvReader)
        {
            map.Record = record;
            writer.Write("<tr><td>");
            writer.Write(map.ColorName);
            writer.Write("</td>");
            writer.Write("<td style=\"background-color: #");
            writer.Write((map.Color.ToArgb() & 0xFFFFFF).ToString("x"));
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
