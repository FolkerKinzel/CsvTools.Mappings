namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

/// <summary>
/// Generic interface that allows implementing instances of <see cref="CsvTypeConverter{T}"/>
/// to change their behavior to enable hexadecimal conversion.
/// </summary>
/// <typeparam name="T">Generic type parameter.</typeparam>
public interface IHexConverter<T>
{
    /// <summary>
    /// Changes the converter to parse and output hexadecimal values.
    /// </summary>
    /// <returns>The modified converter instance to be able to chain calls.</returns>
    CsvTypeConverter<T> AsHexConverter();
}