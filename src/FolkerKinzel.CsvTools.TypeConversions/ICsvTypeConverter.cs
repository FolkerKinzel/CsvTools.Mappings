namespace FolkerKinzel.CsvTools.TypeConversions;

/// <summary>
/// Interface for converting between CSV data and .NET data types.
/// </summary>
public interface ICsvTypeConverter
{
    /// <summary>
    /// Parses a read-only span of characters and returns 
    /// the corresponding .NET object.
    /// </summary>
    /// <param name="value">The span to parse.</param>
    /// <returns>An object of the desired type or <see cref="FallbackValue"/>.</returns>
    /// <exception cref="FormatException">
    /// The parsing failed and <see cref="Throwing"/> is <c>true</c>.
    /// </exception>
    object? Parse(ReadOnlySpan<char> value);

    /// <summary>
    /// Converts <paramref name="value"/> to a <see cref="string"/> or <c>null</c>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>A <see cref="string"/> that represents <paramref name="value"/>.</returns>
    /// <exception cref="InvalidCastException">
    /// The <see cref="Type"/> of value is not compatible with the converter.
    /// </exception>
    string? ConvertToString(object? value);

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
    object? FallbackValue { get; }

    /// <summary>
    /// Gets a value indicating whether the converter accepts 
    /// <c>null</c> values.
    /// </summary>
    /// <value><c>true</c> if the converter accepts <c>null</c> values,
    /// otherwise <c>false</c>.</value>
    bool AcceptsNull { get; }
}
