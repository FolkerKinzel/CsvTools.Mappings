using FolkerKinzel.CsvTools.Mappings.Converters;
using FolkerKinzel.CsvTools.Mappings.Intls.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Mappings.Tests;

[TestClass]
public class MultiColumnPropertyTests
{
    private sealed class SumConverter : MultiColumnTypeConverter<int?>
    {
        public override bool AcceptsNull => true;

        public SumConverter(Mapping mapping) : base(mapping, null, false) { }
        
        private SumConverter(SumConverter other) : base(other) { }

        public override object Clone() => new SumConverter(this);
        
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
    public void MulticolumnPropertyTest1()
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
    [ExpectedException(typeof(ArgumentNullException))]
    public void MultiColumnPropertyTest2()
        => _ = Mapping.Create().AddProperty("PropName", (MultiColumnTypeConverter<int>)null!);

    [TestMethod]
    public void SumConverterTest1()
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
        mapping["Sum"].Value = null;

        CollectionAssert.AreEqual(new int[] { 0, 1 }, mapping["Sum"].CsvColumnIndexes.ToArray());
        CollectionAssert.AreEqual(new string[] { "A", "B" }, mapping["Sum"].CsvColumnNames.ToArray());

        Assert.IsNull(mapping["A"].Value);
        Assert.IsNull(mapping["B"].Value);

        mapping["Sum"].Value = 42;

        Assert.AreEqual(44, mapping["A"].Value);
        Assert.AreEqual(-2, mapping["B"].Value);

        Assert.AreEqual(3, mapping.Count);
        Assert.IsInstanceOfType<Mapping>(mapping.Clear());
        Assert.AreEqual(0, mapping.Count);
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

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ValueTest1()
    {
        var mapping = Mapping.Create();
        var conv = new SumConverter(mapping);
        mapping.AddProperty("Prop", conv);
        object? o = mapping["Prop"].Value;
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ValueTest2()
    {
        var mapping = Mapping.Create();
        var conv = new SumConverter(mapping);
        mapping.AddProperty("Prop", conv);
        mapping["Prop"].Value = 7;
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ValueTest3()
    {
        var mapping = Mapping.Create();
        var conv = new SumConverter(mapping);
        mapping.AddProperty("Prop", conv);
        mapping["Prop"].AsITypedProperty<int?>().Value = 7;
    }


    [TestMethod]
    public void ParseMappings()
    {
        const string csv = """
            A,B,Sum
            2,3,5
            -1,4,3
            """;

        TypeConverter<int?> intConverter = new Int32Converter().ToNullableConverter();
        Mapping mapping = Mapping
            .Create()
            .AddProperty("A", intConverter)
            .AddProperty("B", intConverter);
            
        mapping.AddProperty("Sum", new SumConverter(mapping));

        dynamic[] result = CsvMapping.Parse(csv, mapping, dyn => dyn);
    }
}
