using FolkerKinzel.CsvTools.Mappings.Converters;
using FolkerKinzel.CsvTools.Mappings.Converters.Interfaces;
using System.ComponentModel;
using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.Intls.Converters;

internal static class HexConverterValidator
{

    internal static bool IsHexConverter<T>(IHexConverter<T> converter)
        => converter.Styles.HasFlag(NumberStyles.AllowHexSpecifier)
       && (converter.Styles & NumberStyles.HexNumber) == converter.Styles
       && StringComparer.OrdinalIgnoreCase.Equals(converter.Format, "X");


    internal static TypeConverter<T> ICreateHexConverter<T, TInput>(TInput converter) where TInput: TypeConverter<T>, IHexConverter<T>, ICreateHexConverter
    {
        if(IsHexConverter(converter))
        {
            return converter;
        }

        var clone =  (ICreateHexConverter)converter.Clone();
        clone.Styles = (clone.Styles & NumberStyles.HexNumber) | NumberStyles.AllowHexSpecifier;
        clone.Format = "X";
        return (TypeConverter<T>)clone;
    }
}