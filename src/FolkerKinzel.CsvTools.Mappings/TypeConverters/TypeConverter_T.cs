using FolkerKinzel.CsvTools.Mappings.TypeConverters.Interfaces;
using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.TypeConverters;

/// <summary>
/// Abstract base class for type converters that convert 
/// between the content of a single field of CSV data and .NET data types.
/// </summary>
/// <typeparam name="T">The <see cref="Type"/> to convert.</typeparam>
/// <param name="throwing">Sets the value of the 
/// <see cref="Throwing"/> property.</param>
/// <param name="defaultValue">
/// Sets the value of the <see cref="DefaultValue"/> property.
/// </param>
/// 
/// <example>
/// <para>
/// Writing an own implementation of <see cref="TypeConverter{T}"/> is easy: 
/// </para>
/// <code language="cs" source="..\Examples\Int128Converter.cs"/>
/// </example>
public abstract class TypeConverter<T>(bool throwing,
                                       T defaultValue) : ITypeConverter<T>
{
    /// <inheritdoc/>
    public T DefaultValue { get; } = defaultValue;

    ///<inheritdoc/>
    public bool Throwing { get; } = throwing;

    /// <inheritdoc/>
    public abstract bool AcceptsNull { get; }

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
    /// 
    /// <remarks>
    /// <note type="implement">
    /// Implement this method in derived classes to determine the behavior of <see cref="Parse(ReadOnlySpan{char})"/>.
    /// <para>
    /// </para>
    /// <para>
    /// In any case the method MUST NOT throw an exception. Instead, it should return <c>false</c> 
    /// if parsing fails. In this case <paramref name="result"/> is treated as undefined.
    /// </para>
    /// </note>
    /// </remarks>
    public abstract bool TryParse(ReadOnlySpan<char> value, out T? result);

    /// <summary>
    /// Parses a read-only span of characters and returns 
    /// the corresponding .NET object.
    /// </summary>
    /// <param name="value">The span to parse.</param>
    /// <returns>An object of the desired type or <see cref="DefaultValue"/>.</returns>
    /// <remarks>
    /// <note type="implement">
    /// Override <see cref="TryParse(ReadOnlySpan{char}, out T?)"/> to define the behavior 
    /// of this method.
    /// </note>
    /// </remarks>
    /// <exception cref="FormatException">
    /// The parsing failed and <see cref="Throwing"/> is <c>true</c>.
    /// </exception>
    public T? Parse(ReadOnlySpan<char> value)
        => value.IsEmpty 
            ? DefaultValue 
            : TryParse(value, out T? result)
                ? result
                : Throwing
                    ? throw new FormatException(
                        string.Format(CultureInfo.CurrentCulture, "Cannot convert {0} to {1}.",
                        value.Length > 40 ? nameof(value) : $"\"{value.ToString()}\"",
                        typeof(T).FullName))
                    : DefaultValue;

    /// <summary>
    /// Converts <paramref name="value"/> to a <see cref="string"/> or <c>null</c>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>A <see cref="string"/> that represents <paramref name="value"/> or <c>null</c>.</returns>
    /// <exception cref="FormatException">The instance uses an invalid format string.</exception>
    public abstract string? ConvertToString(T value);
}
