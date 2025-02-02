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

        var sumConverter = new SumConverter(subMapping);

        Mapping mappping = Mapping
            .Create()
            .AddProperty("Sum", sumConverter);

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

    [TestMethod]
    public void SumConverterTest1()
    {
        TypeConverter<int?> nullableIntConverter = new Int32Converter().ToNullableConverter();

        using var stringWriter = new StringWriter();
        using var writer = Csv.OpenWrite(stringWriter, ["A", "B"]);
        Mapping mapping = Mapping
            .Create()
            .AddProperty("A", nullableIntConverter)
            .AddProperty("B", nullableIntConverter);
            
        mapping.Record = writer.Record;

        var sumConverter = new SumConverter(mapping);
        Assert.AreEqual(typeof(int?), sumConverter.DataType);

        mapping.AddProperty("Sum", sumConverter);
        mapping["Sum"].Value = null;

        CollectionAssert.AreEqual(new int[] { 0, 1 }, mapping["Sum"].CsvColumnIndexes.ToArray());
        CollectionAssert.AreEqual(new string[] { "A", "B" }, mapping["Sum"].CsvColumnNames.ToArray());

        Assert.IsNull(mapping["A"].Value);
        Assert.IsNull(mapping["B"].Value);

        mapping["Sum"].Value = 42;

        Assert.AreEqual(44, mapping["A"].Value);
        Assert.AreEqual(-2, mapping["B"].Value);
    }

    [TestMethod]
    public void SumConverterTest2()
    {
        TypeConverter<int?> nullableIntConverter = new Int32Converter().ToNullableConverter();

        using var stringWriter = new StringWriter();
        using CsvWriter writer = Csv.OpenWrite(stringWriter, ["A", "B"]);
        Mapping mapping = Mapping
            .Create()
            .AddProperty("A", nullableIntConverter)
            .AddProperty("B", nullableIntConverter);

        mapping.Record = writer.Record;

        var sumConverter = new SumConverter(mapping);
        Assert.AreEqual(typeof(int?), sumConverter.DataType);

        mapping.AddProperty("Sum", sumConverter);
        Assert.IsNull(mapping["Sum"].DefaultValue);

        ITypedProperty<int?> sumProp = mapping["Sum"].AsITypedProperty<int?>();
        ITypedProperty<int?> aProp = mapping["A"].AsITypedProperty<int?>();
        ITypedProperty<int?> bProp = mapping["B"].AsITypedProperty<int?>();

        Assert.IsNull(sumProp.DefaultValue);
        Assert.AreEqual(typeof(int?), sumProp.Converter.DataType);

        sumProp.Value = null;

        CollectionAssert.AreEqual(new int[] { 0, 1 }, mapping["Sum"].CsvColumnIndexes.ToArray());
        CollectionAssert.AreEqual(new string[] { "A", "B" }, mapping["Sum"].CsvColumnNames.ToArray());

        Assert.IsNull(aProp.Value);
        Assert.IsNull(bProp.Value);

        sumProp.Value = 42;

        Assert.AreEqual(44, aProp.Value);
        Assert.AreEqual(-2, bProp.Value);
        Assert.AreEqual(42, sumProp.Value);

        var mapping2 = (Mapping)mapping.Clone();
        Assert.IsNotNull(mapping2);
        Assert.AreNotSame(mapping, mapping2);
        Assert.AreNotSame(mapping["Sum"], mapping2["Sum"]);

        ITypedProperty<int?> sumProp2 = mapping2["Sum"].AsITypedProperty<int?>();
        Assert.AreSame(sumProp.Converter, sumProp2.Converter);
        Assert.AreSame(sumProp.Record, sumProp2.Record);
    }
}
