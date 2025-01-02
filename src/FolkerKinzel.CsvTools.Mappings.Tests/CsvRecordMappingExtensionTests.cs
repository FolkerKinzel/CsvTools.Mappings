using FolkerKinzel.CsvTools.Mappings.Converters;

namespace FolkerKinzel.CsvTools.Mappings.Tests;

[TestClass]
public class CsvRecordMappingExtensionTests
{
    [TestMethod]
    public void AddSingleColumnPropertyTest1()
    {
        TypeConverter<string?> conv = StringConverter.CreateNullable();

        dynamic mapping = CsvRecordMapping.Create().AddSingleColumnProperty("Hallo", conv);
        mapping.Record = new CsvRecord(1);
        //ITypedProperty<string?> typed = mapping[0].AsITypedProperty<string?>();
        //string val = typed.Value;

        string val = mapping.Hallo;
        mapping.Hallo = null;
    }
}
