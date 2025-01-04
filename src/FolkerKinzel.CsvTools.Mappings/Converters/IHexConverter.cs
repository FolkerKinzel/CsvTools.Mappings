namespace FolkerKinzel.CsvTools.Mappings.Converters;

/// <summary>
/// Generic interface that allows implementing instances of <see cref="TypeConverter{T}"/>
/// to change their behavior to enable hexadecimal conversion.
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
    TypeConverter<T> ToHexConverter();

    ///// <summary>
    ///// Gets a value that indicates whether the instance can convert
    ///// hexadecimal string values.
    ///// </summary>
    ///// <value><c>true</c> if the converter can convert hexadecimal values,
    ///// <c>false</c> if not.</value>
    //bool IsHexConverter { get; }
}