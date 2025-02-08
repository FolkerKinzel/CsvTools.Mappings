using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.TypeConverters.Interfaces;

/// <summary>
/// Generic interface that allows implementing instances of <see cref="TypeConverter{T}"/>
/// to create a modified version of themself that provides hexadecimal conversion.
/// </summary>
/// <typeparam name="T">Generic type parameter.</typeparam>
public interface IHexConverter<T> : ICloneable
{
    /// <summary>
    /// Initializes a new instance of <see cref="IHexConverter{T}"/> 
    /// that can convert hexadecimal string values.
    /// </summary>
    /// <returns>A new instance of <see cref="IHexConverter{T}"/> 
    /// that can convert hexadecimal string values.</returns>
    /// 
    /// <example>
    /// <para>
    /// Using <see cref="MultiColumnTypeConverter{T}"/> to create and parse a CSV file.
    /// </para>
    /// <para>
    /// (For the sake of easier readability exception handling has been omitted.)
    /// </para>
    /// <img src="images\MultiColumnConverter.png"/>
    /// <code language="cs" source="../Examples/MultiColumnConverterExample.cs"/>
    /// </example>
    TypeConverter<T> ToHexConverter();

    /// <summary>
    /// The format string to use.
    /// </summary>
    public string? Format { get; }

    /// <summary>
    /// Gets a combined value of the <see cref="NumberStyles"/> enum that 
    /// provides additional information for parsing.
    /// </summary>
    public NumberStyles Styles { get; }

}