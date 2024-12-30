namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

/// <summary>
/// Abstract base class for implementing type converters that convert 
/// between CSV data and .NET data types.
/// </summary>
/// <typeparam name="T">The <see cref="Type"/> to convert.</typeparam>
/// <param name="throwsOnParseErrors">Sets the value of the 
/// <see cref="Throwing"/> property.</param>
/// <param name="fallbackValue">
/// The <see cref="FallbackValue"/> to return when a parsing error occurs and
/// the <see cref="Throwing"/> property is <c>false</c>.
/// </param>
public abstract class CsvTypeConverter<T>(bool throwsOnParseErrors,
                                          T? fallbackValue = default) : ICsvTypeConverter
{
    /// <summary>
    /// Gets the value to return when a parsing error occurs and
    /// the <see cref="Throwing"/> property is <c>false</c>.
    /// </summary>
    public T? FallbackValue { get; } = fallbackValue;

    /// <inheritdoc/>
    object? ICsvTypeConverter.FallbackValue => FallbackValue;

    /// <summary>
    /// Gets a value indicating whether the converter throws an 
    /// <see cref="FormatException"/> 
    /// when a parsing error occurs, or if it returns 
    /// <see cref="FallbackValue"/> value instead.
    /// </summary>
    /// <value><c>true</c> if the converter throws a <see cref="FormatException"/>
    /// on parsing errors,
    /// <c>false</c> otherwise.</value>
    public bool Throwing { get; } = throwsOnParseErrors;

    public abstract bool TryParseValue(ReadOnlySpan<char> value, out T result);

    public abstract bool AcceptsNull { get; }

    protected abstract string? DoConvertToString(T value);

    public string? ConvertToString(object? value)
        => value is T t
            ? DoConvertToString(t)
            : value is null
                ? AcceptsNull ? null : throw new InvalidCastException(string.Format("Cannot cast null to {0}.", typeof(T)))
                : throw new InvalidCastException("Assignment of an incompliant Type.");


    public string? ConvertToString(T? value) => value is null ? null : DoConvertToString(value);

    protected virtual bool CsvHasValue(ReadOnlySpan<char> csvInput) => !csvInput.IsWhiteSpace();

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
        => !CsvHasValue(value)
            ? FallbackValue
            : TryParseValue(value, out T? result)
                ? result
                : Throwing
                    ? throw new FormatException(
                        string.Format("Cannot convert {0} into {1}.",
                        value.Length > 40 ? nameof(value) : $"\"{value.ToString()}\"",
                        typeof(T)))
                    : FallbackValue;

    /// <inheritdoc/>
    object? ICsvTypeConverter.Parse(ReadOnlySpan<char> value) => Parse(value);
}
