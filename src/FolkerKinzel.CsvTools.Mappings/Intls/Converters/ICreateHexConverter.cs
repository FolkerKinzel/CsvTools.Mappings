using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.Intls.Converters;

internal interface ICreateHexConverter
{
    NumberStyles Styles { get; set; }

    string? Format { get; set; }

}
