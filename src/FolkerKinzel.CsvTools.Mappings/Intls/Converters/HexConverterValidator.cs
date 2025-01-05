using FolkerKinzel.CsvTools.Mappings.Converters.Interfaces;
using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.Intls.Converters;

internal static class HexConverterValidator
{

    internal static bool IsHexConverter<T>(IHexConverter<T> converter)
        => converter.Styles.HasFlag(NumberStyles.AllowHexSpecifier)
       && (converter.Styles & NumberStyles.HexNumber) == converter.Styles
       && StringComparer.OrdinalIgnoreCase.Equals(converter.Format, "X");
}