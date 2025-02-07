using FolkerKinzel.CsvTools.Mappings.Converters;
using FolkerKinzel.CsvTools.Mappings.Converters.Interfaces;
using System.ComponentModel;
using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.Intls.Converters;

internal static class HexConverter
{
    internal const string HEX_FORMAT = "X";

    private static bool IsHexConverter<T>(IHexConverter<T> converter)
        => converter.Styles.HasFlag(NumberStyles.AllowHexSpecifier)
       && (converter.Styles & NumberStyles.HexNumber) == converter.Styles
       && StringComparer.OrdinalIgnoreCase.Equals(converter.Format, "X");


    internal static TypeConverter<T> CreateHexConverter<T, TInput>(TInput converter)
        where TInput: TypeConverter<T>, IHexConverter<T>, IAsHexConverter
    {
        if(IsHexConverter(converter))
        {
            return converter;
        }

        var clone =  (IAsHexConverter)converter.Clone();
        clone.AsHexConverter();
        return (TypeConverter<T>)clone;
    }

    internal static NumberStyles ToHexStyle(NumberStyles styles) => (styles & NumberStyles.HexNumber) | NumberStyles.AllowHexSpecifier;
}