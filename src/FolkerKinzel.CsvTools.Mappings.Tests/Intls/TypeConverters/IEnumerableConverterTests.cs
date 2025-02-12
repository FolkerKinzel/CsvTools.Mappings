using FolkerKinzel.CsvTools.Mappings.TypeConverters;
using FolkerKinzel.CsvTools.Mappings.Intls.DynamicProperties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.CsvTools.Mappings.Intls.TypeConverters.Tests;

[TestClass()]
public class IEnumerableConverterTests
{
    [TestMethod()]
    public void IEnumerableConverterTest1()
    {
        IEnumerable<int> arr1 = [1, 2, 3];
        TypeConverter<IEnumerable<int>?> conv = new Int32Converter().ToIEnumerableConverter("::");

        CsvMapping mapping = CsvMappingBuilder.Create().Build();
        var prop = new IndexProperty<IEnumerable<int>?>("TestProp", 0, conv);
        mapping.AddProperty(prop);

        using var writer = new StringWriter();
        using var csvWriter = new CsvWriter(writer, 1);

        mapping.Record = csvWriter.Record;
        mapping[0].Value = arr1;

        csvWriter.WriteRecord();

        string csv = writer.ToString();

        using var reader = new StringReader(csv);
        using var csvReader = new CsvReader(reader, false);

        mapping.Record = csvReader.First();

        dynamic dynWrapper = mapping;

        IEnumerable<int> arr2 = dynWrapper.TestProp;

        CollectionAssert.AreEqual(arr1.ToArray(), arr2.ToArray());
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentNullException))]
    public void IEnumerableConverterTest2() => _ = new Int32Converter().ToIEnumerableConverter(null!);

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void IEnumerableConverterTest2b() => _ = ((Int32Converter?)null)!.ToIEnumerableConverter("::");

    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void IEnumerableConverterTest3() => _ = new Int32Converter().ToIEnumerableConverter("");

    [TestMethod]
    public void IEnumerableConverterTest4()
    {
        var list = new List<int?>
        {
            7,
            9,
            11
        };

        TypeConverter<IEnumerable<int?>?> conv = new Int32Converter().ToNullableConverter().ToIEnumerableConverter("::");
        Assert.IsTrue(conv.AcceptsNull);
        string? s = conv.ConvertToString(list);

        var result = (IEnumerable<int?>?)conv.Parse(s.AsSpan());
        Assert.IsNotNull(result);
        CollectionAssert.AreEqual(list, result!.ToList());
    }

    [TestMethod]
    public void NonNullableTest1()
    {
        TypeConverter<IEnumerable<int>?> conv = new Int32Converter().ToIEnumerableConverter("::", false);
        Assert.IsNotNull(conv);
        Assert.IsTrue(conv.AcceptsNull);
        Assert.IsNotNull(conv.DefaultValue);
    }
}