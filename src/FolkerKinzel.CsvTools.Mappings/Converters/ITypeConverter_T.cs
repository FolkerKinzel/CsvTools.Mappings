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
    /// Gets a value indicating whether the converter allows 
    /// <c>null</c> values as input.
    /// </summary>
    /// <value><c>true</c> if the converter allows <c>null</c> values as input,
    /// otherwise <c>false</c>.</value>
    /// <remarks>
    /// <note type="implement">This value should be <c>true</c> for all reference types
    /// and <c>false</c> for all value types, except <see cref="Nullable{T}"/>.</note>
    /// <remarks>
    /// <para>
    /// The behavior is equivalent to the behavior of the <see cref="AllowNullAttribute"/>:
    /// Even if the converters <see cref="DataType"/> doesn't allow <c>null</c> values, 
    /// <c>null</c> will be accepted as input if the <see cref="AllowsNull"/> property 
    /// is <c>true</c>.
    /// </para>
    /// </remarks>
    /// </remarks>
    bool AllowsNull { get; }

    /// <summary>
    /// The data type the converter converts.
    /// </summary>
    Type DataType { get; }
}
