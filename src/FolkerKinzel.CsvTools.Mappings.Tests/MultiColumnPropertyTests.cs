using FolkerKinzel.CsvTools.Mappings.Converters;
using FolkerKinzel.CsvTools.Mappings.Intls.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Mappings.Tests;

[TestClass]
public class MultiColumnPropertyTests
{
    private sealed class SumConverter(Mapping mapping)
        : MultiColumnTypeConverter<int?>(mapping, null, false)
    {
        public override bool AllowsNull => true;

        protected override void DoConvertToCsv(int? value)
        {
            if (!value.HasValue)
            {
                return;
            }

            dynamic dyn = Mapping;
            dyn.A = value.Value + 2;
            dyn.B = - 2;
        }

        protected override bool TryParse(out int? result)
        {
            dynamic dyn = Mapping;

            int? a = dyn.A;
            int? b = dyn.B;

            if(a.HasValue && b.HasValue)
            {
                result = a.Value + b.Value;
                return true;
            }

            result = DefaultValue;
            return false;
        }
    }

    [TestMethod]
    public void MulticolumnTest1()
    {
        TypeConverter<int?> nullableIntConverter = new Int32Converter().ToNullableConverter();

        Mapping subMapping = Mapping
            .Create()
            .AddProperty("A", nullableIntConverter)
            .AddProperty("B", nullableIntConverter);

        Mapping mappping = Mapping
            .Create()
            .AddProperty("Sum", new SumConverter(subMapping));

        using var stringWriter = new StringWriter();
        using CsvWriter csvWriter = Csv.OpenWrite(stringWriter, ["A", "B"]);
        mappping.Record = csvWriter.Record;

        int?[] sums = [3, 0, -7, null];

        foreach (int? sum in sums.AsSpan())
        {
            dynamic dyn = mappping;
            dyn.Sum = sum;
            csvWriter.WriteRecord();
        }

        csvWriter.Dispose();

        string csv = stringWriter.ToString();

        int?[] results = CsvMapping.Parse<int?>(csv, mappping, dyn => dyn.Sum);
        CollectionAssert.AreEqual(sums, results);
    }
}
