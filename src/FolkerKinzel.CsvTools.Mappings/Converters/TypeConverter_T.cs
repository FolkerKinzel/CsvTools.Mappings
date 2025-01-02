using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.Converters;

/// <summary>
/// Abstract base class for implementing type converters that convert 
/// between CSV data and .NET data types.
/// </summary>
/// <typeparam name="T">The <see cref="Type"/> to convert.</typeparam>
/// <param name="throwing">Sets the value of the 
/// <see cref="Throwing"/> property.</param>
/// <param name="fallbackValue">
/// The <see cref="FallbackValue"/> to return when a parsing error occurs and
/// the <see cref="Throwing"/> property is <c>false</c>. The value MUST NOT be 
/// <c>null</c> for value types (except <see cref="Nullable{T}"/>). It should 
/// not be <c>null</c> for non-nullable reference types, except <paramref name="throwing"/>
/// is <c>true</c>.
/// </param>
public abstract class TypeConverter<T>(bool throwing,
                                       T? fallbackValue) : ITypeConverter<T> 
{
    /// <inheritdoc/>
    public T? FallbackValue { get; } = fallbackValue;

    ///<inheritdoc/>
    public bool Throwing { get; } = throwing;

    /// <inheritdoc/>
    public abstract bool AllowsNull { get; }

    /// <inheritdoc/>
    public Type DataType => typeof(T);

    /// <summary>
    /// Tries to parse a read-only span of characters as a <typeparamref name="T"/> value.
    /// </summary>
    /// <param name="value">The read-only span of characters to parse.</param>
    /// <param name="result">
    /// After the method returns, contains the <typeparamref name="T"/> value that is equivalent 
    /// to the parsed <paramref name="value"/>, if the parsing succeeds, or the default value of 
    /// <typeparamref name="T"/> if the parsing failed.
    /// </param>
    /// <returns><c>true</c> if the parsing was successfull, otherwise <c>false</c>.</returns>
    /// <remarks>
    /// <note type="implement">
    /// In any case the method MUST NOT throw an exception. Instead, it should return <c>false</c> 
    /// if parsing fails. In this case <paramref name="result"/> is treated as undefined.
    /// </note>
    /// </remarks>
    public abstract bool TryParseValue(ReadOnlySpan<char> value, out T? result);

    /// <summary>
    /// Converts <paramref name="value"/> to a <see cref="string"/> or <c>null</c>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>A <see cref="string"/> that represents <paramref name="value"/> or <c>null</c>.</returns>
    public abstract string? ConvertToString(T value);

    ///// <summary>
    ///// Returns a <see cref="bool"/> value indicating whether the 
    ///// <paramref name="csvInput"/> represents a value or not.
    ///// </summary>
    ///// <param name="csvInput">The input to examine.</param>
    ///// <returns><c>true</c> if <paramref name="csvInput"/> represents a value, 
    ///// otherwise <c>false</c>.</returns>
    ///// <remarks>The method returns <c>false</c> if <paramref name="csvInput"/> is empty or 
    ///// contains only whitespace.
    ///// Override this method in derived classes to change this behavior.
    ///// </remarks>
    //protected virtual bool CsvHasValue(ReadOnlySpan<char> csvInput) => !csvInput.IsWhiteSpace();

    /// <summary>
    /// Parses a read-only span of characters and returns 
    /// the corresponding .NET object.
    /// </summary>
    /// <param name="value">The span to parse.</param>
    /// <returns>An object of the desired type or <see cref="FallbackValue"/>.</returns>
    /// <exception cref="FormatException">
    /// The parsing failed and <see cref="Throwing"/> is <c>true</c>.
    /// </exception>
    public T? Parse(ReadOnlySpan<char> value)
        => TryParseValue(value, out T? result)
             ? result
             : Throwing
                 ? throw new FormatException(
                     string.Format(CultureInfo.CurrentCulture, "Cannot convert {0} to {1}.",
                     value.Length > 40 ? nameof(value) : $"\"{value.ToString()}\"",
                     typeof(T).FullName))
                 : FallbackValue;
}
