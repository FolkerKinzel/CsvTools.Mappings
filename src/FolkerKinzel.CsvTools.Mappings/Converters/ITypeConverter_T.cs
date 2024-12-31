namespace FolkerKinzel.CsvTools.Mappings.Converters;

/// <summary>
/// Interface for converting between CSV data and .NET data types.
/// </summary>
/// <typeparam name="T">The .NET data type the converter can convert.</typeparam>
public interface ITypeConverter<T>
{
    /// <summary>
    /// Gets a value indicating whether the converter throws a
    /// <see cref="FormatException"/> 
    /// when a parsing error occurs, or if it returns 
    /// <see cref="FallbackValue"/> value instead.
    /// </summary>
    /// <value><c>true</c> if the converter throws a 
    /// <see cref="FormatException"/> on parsing errors,
    /// <c>false</c> otherwise.</value>
    bool Throwing { get; }

    /// <summary>
    /// Gets the value to return when a parsing error occurs and
    /// the <see cref="Throwing"/> property is <c>false</c>.
    /// </summary>
    T? FallbackValue { get; }

    /// <summary>
    /// Gets a value indicating whether the converter accepts 
    /// <c>null</c> values.
    /// </summary>
    /// <value><c>true</c> if the converter accepts <c>null</c> values,
    /// otherwise <c>false</c>.</value>
    bool AcceptsNull { get; }

    /// <summary>
    /// The data type the converter converts.
    /// </summary>
    Type DataType { get; }
}
