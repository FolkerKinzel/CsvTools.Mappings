﻿namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

/// <summary>
/// Generic <see cref="CsvTypeConverter{T}"/> implementation for <c>enum</c> types.
/// </summary>
public sealed class EnumConverter<TEnum> : CsvTypeConverter<TEnum> where TEnum : struct, Enum
{
    private const string DEFAULT_FORMAT = "D";

    /// <summary>
    /// Initializes a new <see cref="EnumConverter{TEnum}"/> instance.
    /// </summary>
    /// <param name="throwing">Sets the value of the 
    /// <see cref="CsvTypeConverter{T}.Throwing"/> property.</param>
    /// <param name="fallbackValue">
    /// The <see cref="CsvTypeConverter{T}.FallbackValue"/> to return when a parsing error occurs and
    /// the <see cref="CsvTypeConverter{T}.Throwing"/> property is <c>false</c>.
    /// </param>
    /// <param name="ignoreCase">A value that indicates whether the parser takes casing into account.
    /// (<c>false</c> for case-sensitive parsing, otherwise <c>true</c>.)</param>
    /// <remarks>This constructor initializes a <see cref="EnumConverter{TEnum}"/> instance that uses 
    /// the format string "D". This constructor is much faster than its overload.</remarks>
    public EnumConverter(
        bool throwing = true,
        TEnum fallbackValue = default,
        bool ignoreCase = true)
        : base(throwing, fallbackValue) => IgnoreCase = ignoreCase;

    /// <summary>
    /// Initializes a new <see cref="EnumConverter{TEnum}"/> instance and allows to specify a
    /// format string.
    /// </summary>
    /// <param name="format">The format string.</param>
    /// <param name="throwing">Sets the value of the 
    /// <see cref="CsvTypeConverter{T}.Throwing"/> property.</param>
    /// <param name="fallbackValue">
    /// The <see cref="CsvTypeConverter{T}.FallbackValue"/> to return when a parsing error occurs and
    /// the <see cref="CsvTypeConverter{T}.Throwing"/> property is <c>false</c>.
    /// </param>
    /// <param name="ignoreCase">A value that indicates whether the parser takes casing into account.
    /// (<c>false</c> for case-sensitive parsing, otherwise <c>true</c>.)</param>
    public EnumConverter(
        string? format,
        bool throwing = true,
        TEnum fallbackValue = default,
        bool ignoreCase = true)
        : base(throwing, fallbackValue)
    {
        ValidateFormat(format);
        IgnoreCase = ignoreCase;
        Format = format;
    }

    private static void ValidateFormat(string? format)
    {
        switch (format)
        {
            case "G":
            case "g":
            case "D":
            case "d":
            case "X":
            case "x":
            case "F":
            case "f":
            case null:
            case "":
                return;
            default:
                throw new ArgumentException("Invalid format string.", nameof(format));
        }
    }

    /// <summary>
    /// Gets a value that indicates whether the parser takes casing into account.
    /// </summary>
    /// <value><c>false</c> for case-sensitive parsing, otherwise <c>true</c>.</value>
    public bool IgnoreCase { get; }

    /// <summary>
    /// Gets the format string to use.
    /// </summary>
    public string? Format { get; } = DEFAULT_FORMAT;

    /// <inheritdoc/>
    public override bool AcceptsNull => false;

    /// <inheritdoc/>
    public override string? ConvertToString(TEnum value) => value.ToString(Format);

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, out TEnum result)
#if NET462 || NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0
        => Enum.TryParse<TEnum>(value.ToString(), IgnoreCase, out result);
#else
        => Enum.TryParse<TEnum>(value, IgnoreCase, out result);
#endif
}
