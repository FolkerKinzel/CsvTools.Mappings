﻿using FolkerKinzel.CsvTools.Mappings.Resources;
using System.Data;
using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.TypeConverters;

/// <summary>
/// Generic <see cref="TypeConverter{T}"/> implementation for <c>enum</c> types.
/// </summary>
/// <typeparam name="TEnum">The .NET enum data type the <see cref="EnumConverter{TEnum}"/>
/// can convert.</typeparam>
/// 
/// 
/// <threadsafety static="true" instance="true"/>
public sealed class EnumConverter<TEnum> : TypeConverter<TEnum> where TEnum : struct, Enum
{
    /// <summary>
    /// Initializes a new <see cref="EnumConverter{TEnum}"/> instance.
    /// </summary>
    /// <param name="format">
    /// <para>
    /// A format string that is used for the <see cref="string"/> output 
    /// of enum values. The accepted values 
    /// are "G", "g", "D", "d", "F", "f", <c>null</c> and <see cref="string.Empty"/>.
    /// </para>
    /// <note type="caution">
    /// The format strings "X" and "x" are not supported here!
    /// </note>
    /// </param>
    /// <param name="ignoreCase">A value that indicates whether the parser takes casing 
    /// into account.
    /// (<c>false</c> for case-sensitive parsing, otherwise, <c>true</c>.)</param>
    /// <param name="throwing">Sets the value of the 
    /// <see cref="TypeConverter{T}.Throwing"/> property.</param>
    /// <param name="defaultValue">Sets the value of the 
    /// <see cref="TypeConverter{T}.DefaultValue"/> property.</param>
    /// 
    /// <example>
    /// <note type="note">In the following code examples - for easier readability - exception 
    /// handling has been omitted.</note>
    /// <para>
    /// Saving the contents of a <see cref="DataTable"/> as a CSV file and importing data from 
    /// a CSV file into a <see cref="DataTable"/>: </para>
    /// <code language="cs" source="..\Examples\DataTableExample.cs"/>
    /// </example>
    /// 
    /// <exception cref="ArgumentException">The value of <paramref name="format"/> is 
    /// invalid.</exception>"
    public EnumConverter(
#if !(NET462 || NETSTANDARD2_0 || NETSTANDARD2_1)
        [StringSyntax(StringSyntaxAttribute.EnumFormat)]
#endif
        string? format = "D",
        bool ignoreCase = true,
        bool throwing = true,
        TEnum defaultValue = default)
        : base(throwing, defaultValue)
    {
        EnumConverter<TEnum>.ValidateFormat(format);
        IgnoreCase = ignoreCase;
        Format = format;
    }

    /// <summary>
    /// Gets a value that indicates whether the parser takes casing into account.
    /// </summary>
    /// <value><c>false</c> for case-sensitive parsing, otherwise <c>true</c>.</value>
    public bool IgnoreCase { get; }

    /// <summary>
    /// Gets the format string to use.
    /// </summary>
    public string? Format { get; }

    /// <inheritdoc/>
    public override bool AcceptsNull => false;

    /// <inheritdoc/>
    public override string? ConvertToString(TEnum value) => value.ToString(Format);

    /// <inheritdoc/>
    public override bool TryParse(ReadOnlySpan<char> value, out TEnum result)
    {
#if NET462 || NETSTANDARD2_0 || NETSTANDARD2_1
        return Enum.TryParse<TEnum>(value.ToString(), IgnoreCase, out result);
#else
        return Enum.TryParse<TEnum>(value, IgnoreCase, out result);
#endif
    }

    private static void ValidateFormat(string? format)
    {
        switch (format)
        {
            case "G":
            case "g":
            case "D":
            case "d":
            case "F":
            case "f":
            case null:
            case "":
                break;
            //case "X":
            //case "x":
            //break;
            default:
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, 
                                                          Res.FormatStringNotSupported, 
                                                          format),
                                            nameof(format));
        }
    }
}
