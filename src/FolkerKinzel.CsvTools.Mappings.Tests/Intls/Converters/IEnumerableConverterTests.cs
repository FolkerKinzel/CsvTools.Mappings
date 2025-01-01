using FolkerKinzel.CsvTools.Mappings.Converters;
using FolkerKinzel.CsvTools.Mappings.Intls.MappingProperties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.CsvTools.Mappings.Intls.Converters.Tests;

[TestClass()]
public class IEnumerableConverterTests
{
    [TestMethod]
    public void MyTestMethod()
    {
        var list = new List<int?>
        {
            7,
            9,
            11
        };

        TypeConverter<IEnumerable<int?>?> conv = new Int32Converter().ToNullableConverter().ToIEnumerableConverter("::");
        string? s = conv.ConvertToString(list);

        var result = (IEnumerable<int?>?)conv.Parse(s.AsSpan());
        Assert.IsNotNull(result);
        CollectionAssert.AreEqual(list, result!.ToList());
    }

    [TestMethod()]
    public void IEnumerableTConverterTest()
    {
        IEnumerable<int> arr1 = [1, 2, 3];
        TypeConverter<IEnumerable<int>?> conv = new Int32Converter().ToIEnumerableConverter("::");

        var wrapper = CsvRecordMapping.Create();
        var prop = new IndexProperty<IEnumerable<int>?>("TestProp", 0, conv);
        wrapper.Add(prop);

        using var writer = new StringWriter();
        using var csvWriter = new CsvWriter(writer, 1);

        wrapper.Record = csvWriter.Record;
        wrapper[0].Value = arr1;

        csvWriter.WriteRecord();

        string csv = writer.ToString();

        using var reader = new StringReader(csv);
        using var csvReader = new CsvEnumerator(reader, false);

        wrapper.Record = csvReader.First();

        dynamic dynWrapper = wrapper;

        IEnumerable<int> arr2 = dynWrapper.TestProp;

        CollectionAssert.AreEqual(arr1.ToArray(), arr2.ToArray());
    }

    //[TestMethod()]
    //public void ParseTest()
    //{
    //    Assert.Fail();
    //}

    //[TestMethod()]
    //public void ConvertToStringTest()
    //{
    //    Assert.Fail();
    //}
}