using FolkerKinzel.CsvTools.Mappings.Intls.MappingProperties;
using System;

namespace FolkerKinzel.CsvTools.Mappings.Converters;

/// <summary>
/// Abstract base class for serializing and deserializing objects whose data is distributed 
/// across multiple columns of a CSV file.
/// </summary>
/// <typeparam name="T">The <see cref="Type"/> to convert.</typeparam>
/// 
/// <param name="mapping">The <see cref="CsvRecordMapping"/> to use to access those columns 
/// of the CSV file that are required for the <see cref="Type"/> conversion.</param>
/// <param name="throwing">Sets the value of the 
/// <see cref="Throwing"/> property.</param>
/// <param name="fallbackValue">
/// The <see cref="FallbackValue"/> to return when a parsing error occurs and
/// the <see cref="Throwing"/> property is <c>false</c>.
/// </param>
/// <remarks>
/// Instances derived from this class are required by <see cref="MultiColumnProperty{T}"/>.
/// </remarks>
/// <seealso cref="MultiColumnProperty{T}"/>
/// 
/// <exception cref="ArgumentNullException"><paramref name="mapping"/> is <c>null</c>.</exception>
public abstract class MultiColumnTypeConverter<T>(CsvRecordMapping mapping,
                                                     bool throwing,
                                                     T? fallbackValue = default) : ITypeConverter<T>
{
    /// <summary>
    /// The <see cref="CsvRecordMapping"/> to use to access those columns 
    /// of the CSV file that are required for the <see cref="Type"/> conversion.
    /// </summary>
    public CsvRecordMapping Mapping { get; } = mapping ?? throw new ArgumentNullException(nameof(mapping));

    /// <inheritdoc/>
    public abstract bool AcceptsNull { get; }

    /// <inheritdoc/>
    public bool Throwing { get; } = throwing;

    /// <inheritdoc/>
    public T? FallbackValue { get; } = fallbackValue;

    /// <inheritdoc/>
    public Type DataType => typeof(T);

    /// <summary>
    /// Returns a <see cref="bool"/> value indicating whether the 
    /// content of <see cref="Mapping"/> that has to be parsed represents a value or not.
    /// </summary>
    /// <returns><c>true</c> if <see cref="Mapping"/> contains a parseable value, 
    /// otherwise <c>false</c>.</returns>
    protected abstract bool CsvHasValue();

    /// <summary>
    /// Tries to convert several <see cref="MappingProperty"/> instances in
    /// <see cref="Mapping"/> to a <typeparamref name="T"/> value.
    /// </summary>
    /// 
    /// <param name="result">
    /// After the method was successful, contains the <typeparamref name="T"/> value that is equivalent
    /// to the content of the converted <see cref="MappingProperty"/> instances in
    /// <see cref="Mapping"/>,
    /// or the default value of <typeparamref name="T"/> if the parsing failed.
    /// </param>
    /// <returns><c>true</c> if the parsing was successfull, otherwise <c>false</c>.</returns>
    /// <remarks>
    /// <note type="implement">
    /// If <see cref="Throwing"/> is <c>false</c> the method has to catch any <see cref="FormatException"/>
    /// that a converter in <see cref="Mapping"/> might throw.
    /// </note>
    /// </remarks>
    protected abstract bool TryConvertMapping(out T result);

    /// <summary>
    /// Converts several <see cref="MappingProperty"/> instances in <see cref="Mapping"/> to a 
    /// <typeparamref name="T"/> value.
    /// </summary>
    /// <returns>An object of the desired type or <see cref="FallbackValue"/>.</returns>
    /// <exception cref="FormatException">The conversion fails and <see cref="Throwing"/> is <c>true</c>.
    /// </exception>
    public T? Convert()
        => !CsvHasValue()
                ? FallbackValue
                : TryConvertMapping(out T? result)
                    ? result
                    : Throwing
                        ? throw new FormatException(string.Format("Cannot convert the CSV data to {0}.", typeof(T)))
                        : FallbackValue;

    /// <summary>
    /// Writes a <typeparamref name="T"/> value to several properties of <see cref="Mapping"/>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <remarks>
    /// Implement this method in derived classes to determine the behavior of <see cref="ConvertToCsv(object?)"/>.
    /// </remarks>
    protected abstract void DoConvertToCsv(T? value);

    /// <summary>
    /// Writes <paramref name="value"/> to the selected fields of <see cref="MappingProperty.Record"/> 
    /// using <see cref="Mapping"/>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <exception cref="InvalidCastException"><paramref name="value"/> is <c>null</c> and <see cref="AcceptsNull"/>
    /// is <c>false</c>.</exception>
    public void ConvertToCsv(T? value)
    {
        if (value is null && !AcceptsNull)
        {
            throw new InvalidCastException(string.Format("Cannot cast null to {0}.", typeof(T)));
        }

        DoConvertToCsv(value);
    }

    /// <summary>
    /// Writes <paramref name="value"/> to the selected fields of <see cref="MappingProperty.Record"/> 
    /// using <see cref="Mapping"/>.
    /// </summary>
    /// <param name="value">The object to write to the selected fields of <see cref="MappingProperty.Record"/>.</param>
    /// <exception cref="InvalidCastException"><paramref name="value"/> has an incompatible data type.</exception>
    public void ConvertToCsv(object? value)
    {
        if (value is null && !AcceptsNull)
        {
            throw new InvalidCastException(string.Format("Cannot cast null to {0}.", typeof(T).FullName));
        }
        else
        {
            ConvertToCsv((T?)value);
        }
    }
}
