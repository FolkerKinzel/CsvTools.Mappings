using System;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public abstract class CsvTypeConverter<T>(bool throwsOnParseErrors, T? fallbackValue = default) : ICsvTypeConverter
{
    public T? FallbackValue { get; } = fallbackValue;

    object? ICsvTypeConverter.FallbackValue => FallbackValue;

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

    object? ICsvTypeConverter.Parse(ReadOnlySpan<char> value) => Parse(value);
}
