﻿using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

/// <summary>
/// <see cref="CsvTypeConverter{T}"/> implementation for <see cref="Guid"/>.
/// </summary>
public sealed class GuidConverter : CsvTypeConverter<Guid>
{
    private readonly string? _format;

    /// <summary>
    /// Initializes a new <see cref="GuidConverter"/> instance.
    /// </summary>
    /// <param name="throwing">Sets the value of the 
    /// <see cref="CsvTypeConverter{T}.Throwing"/> property.</param>
    /// <remarks>
    /// Instances initialized with this constructor use the format string "D".
    /// This constructor is much faster than its overload.
    /// </remarks>
    public GuidConverter(bool throwing = true) : base(throwing) => _format = "D";

    /// <summary>
    /// Initializes a new <see cref="GuidConverter"/> instance and allows
    /// to specify a format string.
    /// </summary>
    /// <param name="format">A format string or <c>null</c> for "D".</param>
    /// <param name="throwing">Sets the value of the 
    /// <see cref="CsvTypeConverter{T}.Throwing"/> property.</param>
    /// <exception cref="ArgumentException">
    /// <paramref name="format"/> is not valid.
    /// </exception>
    public GuidConverter(
        string? format,
        bool throwing = true) : base(throwing, default)
    {
        _format = format;
        ExamineFormat(nameof(format));
    }

    /// <inheritdoc/>
    public override bool AcceptsNull => false;

    /// <inheritdoc/>
    public override string? ConvertToString(Guid value) => value.ToString(_format, CultureInfo.InvariantCulture);

    /// <inheritdoc/>
    public override bool TryParseValue(ReadOnlySpan<char> value, out Guid result)
#if NET462 || NETSTANDARD2_0
        => Guid.TryParse(value.ToString(), out result);
#else
        => Guid.TryParse(value, out result);
#endif

    private void ExamineFormat(string parameterName)
    {
        switch (_format)
        {
            case "N":
            case "D":
            case "B":
            case "P":
            case "X":
            case null:
            case "":
                return;
            default:
                throw new ArgumentException("Invalid format string.", parameterName);
        }
    }
}
