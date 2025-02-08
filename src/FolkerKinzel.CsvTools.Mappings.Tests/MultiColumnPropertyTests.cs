using FolkerKinzel.CsvTools.Mappings.TypeConverters;
using FolkerKinzel.CsvTools.Mappings.Intls.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Mappings.Tests;

[TestClass]
public class MultiColumnPropertyTests
{
    private sealed class SumConverter : MultiColumnTypeConverter<int?>
    {
        public override bool AcceptsNull => true;

        public SumConverter(CsvRecordMappingBuilder mapping) : base(mapping, null, false) { }
        
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

        CsvRecordMappingBuilder subMapping = CsvRecordMappingBuilder
            .Create()
            .AddProperty("A", nullableIntConverter)
            .AddProperty("B", nullableIntConverter);

        var sumConverter = new SumConverter(subMapping);

        CsvRecordMapping mappping = CsvRecordMappingBuilder
            .Create()
            .AddProperty("Sum", sumConverter)
            .Build();

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

        int?[] results = CsvConverter.Parse<int?>(csv, mappping, dyn => dyn.Sum);
        CollectionAssert.AreEqual(sums, results);
    }

    [TestMethod]
    public void SumConverterTest1()
    {
        TypeConverter<int?> nullableIntConverter = new Int32Converter().ToNullableConverter();

        using var stringWriter = new StringWriter();
        using CsvWriter writer = Csv.OpenWrite(stringWriter, ["A", "B"]);
        CsvRecordMappingBuilder mappingBuilder = CsvRecordMappingBuilder
            .Create()
            .AddProperty("A", nullableIntConverter)
            .AddProperty("B", nullableIntConverter);

        var sumConverter = new SumConverter(mappingBuilder);
        Assert.AreEqual(typeof(int?), sumConverter.DataType);

        CsvRecordMapping mapping = CsvRecordMappingBuilder.Create().AddProperty("Sum", sumConverter).Build();
        mapping.Record = writer.Record;
        mapping["Sum"].Value = null;

        CollectionAssert.AreEqual(new int[] { 0, 1 }, mapping["Sum"].CsvColumnIndexes.ToArray());
        CollectionAssert.AreEqual(new string[] { "A", "B" }, mapping["Sum"].CsvColumnNames.ToArray());

        Assert.IsNull(sumConverter.Mapping["A"].Value);
        Assert.IsNull(sumConverter.Mapping["B"].Value);

        mapping["Sum"].Value = 42;

        Assert.AreEqual(44, sumConverter.Mapping["A"].Value);
        Assert.AreEqual(-2, sumConverter.Mapping["B"].Value);

        //Assert.AreEqual(3, mapping.Count);
        //Assert.AreEqual(0, mapping.Count);
    }

    [TestMethod]
    public void SumConverterTest2()
    {
        TypeConverter<int?> nullableIntConverter = new Int32Converter().ToNullableConverter();

        using var stringWriter = new StringWriter();
        using CsvWriter writer = Csv.OpenWrite(stringWriter, ["A", "B"]);
        CsvRecordMappingBuilder mappingBuilder = CsvRecordMappingBuilder
            .Create()
            .AddProperty("A", nullableIntConverter)
            .AddProperty("B", nullableIntConverter);

        var sumConverter = new SumConverter(mappingBuilder);
        Assert.AreEqual(typeof(int?), sumConverter.DataType);

        CsvRecordMapping mapping = CsvRecordMappingBuilder.Create().AddProperty("Sum", sumConverter).Build();
        mapping.Record = writer.Record;
        Assert.IsNull(mapping["Sum"].DefaultValue);

        ITypedProperty<int?> sumProp = mapping["Sum"].AsITypedProperty<int?>();
        ITypedProperty<int?> aProp = sumConverter.Mapping["A"].AsITypedProperty<int?>();
        ITypedProperty<int?> bProp = sumConverter.Mapping["B"].AsITypedProperty<int?>();

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

        var mapping2 = (CsvRecordMapping)mapping.Clone();
        Assert.IsNotNull(mapping2);
        Assert.AreNotSame(mapping, mapping2);
        Assert.AreNotSame(mapping["Sum"], mapping2["Sum"]);

        ITypedProperty<int?> sumProp2 = mapping2["Sum"].AsITypedProperty<int?>();
        Assert.AreNotSame(sumProp.Converter, sumProp2.Converter);
        Assert.AreSame(sumProp.Record, sumProp2.Record);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ValueTest1()
    {
        var conv = new SumConverter(CsvRecordMappingBuilder.Create());
        CsvRecordMapping mapping = CsvRecordMappingBuilder.Create().AddProperty("Prop", conv).Build();
        _ = mapping["Prop"].Value;
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ValueTest2()
    {
        var conv = new SumConverter(CsvRecordMappingBuilder.Create());
        CsvRecordMapping mapping = CsvRecordMappingBuilder.Create().AddProperty("Prop", conv).Build();
        mapping["Prop"].Value = 7;
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ValueTest3()
    {
        var conv = new SumConverter(CsvRecordMappingBuilder.Create());
        CsvRecordMapping mapping = CsvRecordMappingBuilder.Create().AddProperty("Prop", conv).Build();
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
        CsvRecordMappingBuilder mappingBuilder = CsvRecordMappingBuilder
            .Create()
            .AddProperty("A", intConverter)
            .AddProperty("B", intConverter);
            
        CsvRecordMapping mapping = CsvRecordMappingBuilder.Create().AddProperty("Sum", new SumConverter(mappingBuilder)).Build();

        dynamic[] result = CsvConverter.Parse(csv, mapping, dyn => dyn);
    }
}
