using FolkerKinzel.CsvTools.Mappings.Converters.Interfaces;
using FolkerKinzel.CsvTools.Mappings.Intls;
using FolkerKinzel.CsvTools.Mappings.Resources;
using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.Converters;

/// <summary>
/// Abstract base class for type converters that provides conversions between
/// .NET data types and CSV data that is distributed across multiple columns of a CSV file.
/// </summary>
/// 
/// <typeparam name="T">The data <see cref="Type"/> that the <see cref="MultiColumnTypeConverter{T}"/> converts.</typeparam>
/// 
/// <remarks>
/// <para>
/// A ready-to-use implementation of this class can't be provided because their structure depends 
/// on the CSV file to be processed. Fortunately, writing a derived class is easy:
/// </para>
/// <note type="implement">
/// Pass a <see cref="Mappings.Mapping"/> instance that 
/// targets the required columns of the CSV file to the constructor, and override the abstract 
/// members. For overriding <see cref="ICloneable.Clone"/> using the copy constructor 
/// (<see cref="MultiColumnTypeConverter{T}(MultiColumnTypeConverter{T})"/>) is required!
/// </note>
/// </remarks>
/// 
/// <example>
/// <para>
/// Using <see cref="MultiColumnTypeConverter{T}"/> to create and parse a CSV file.
/// </para>
/// <para>
/// (For the sake of easier readability exception handling has been omitted.)
/// </para>
/// <img src="images\MultiColumnConverter.png"/>
/// <code language="cs" source="../Examples/MultiColumnConverterExample.cs"/>
/// </example>
public abstract class MultiColumnTypeConverter<T> : ITypeConverter<T>, ICloneable
{
    /// <summary>
    /// Constructor used by derived classes.
    /// </summary>
    /// <param name="mapping">
    /// <para>
    /// The <see cref="Mappings.Mapping"/> to use to access those columns 
    /// of the CSV file that are required for the <see cref="Type"/> conversion.
    /// </para>
    /// <note type="tip">
    /// It's easier to debug if a separate <see cref="Mappings.Mapping"/> is used here.
    /// </note>
    /// </param>
    /// <param name="throwing">Sets the value of the <see cref="Throwing"/> property.</param>
    /// <param name="defaultValue">
    /// The <see cref="DefaultValue"/> to return when a parsing error occurs and
    /// the <see cref="Throwing"/> property is <c>false</c>.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="mapping"/> is <c>null</c>.</exception>
    protected MultiColumnTypeConverter(Mapping mapping,
                                       T defaultValue,
                                       bool throwing)
    {
        Mapping = mapping ?? throw new ArgumentNullException(nameof(mapping));
        Throwing = throwing;
        DefaultValue = defaultValue;
    }

    /// <summary>
    /// Copy constructor used by derived classes to implement <see cref="ICloneable"/>.
    /// </summary>
    /// <param name="other">The <see cref="MultiColumnTypeConverter{T}"/> instance to clone.</param>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
    protected MultiColumnTypeConverter(MultiColumnTypeConverter<T> other)
    {
        _ArgumentNullException.ThrowIfNull(other, nameof(other));

        Mapping = (Mapping)other.Mapping.Clone();
        Throwing = other.Throwing;
        DefaultValue = other.DefaultValue;
    }

    /// <inheritdoc/>
    public abstract object Clone();

    /// <summary>
    /// The <see cref="Mappings.Mapping"/> to use to access those columns 
    /// of the CSV file that are required for the <see cref="Type"/> conversion.
    /// </summary>
    public Mapping Mapping { get; }

    /// <inheritdoc/>
    public abstract bool AcceptsNull { get; }

    /// <inheritdoc/>
    public bool Throwing { get; }

    /// <inheritdoc/>
    /// <remarks>
    /// <note type="implement">
    /// The value may be <c>null</c> for non-nullable reference types
    /// if two conditions are met at the same time:
    /// <list type="number">
    /// <item><see cref="Throwing"/> MUST be <c>true</c> in this case, and</item>
    /// <item><see cref="TryParse(out T?)"/> MUST
    /// return <c>false</c> if its result is <see cref="DefaultValue"/> 
    /// (respectively <c>null</c>).</item>
    /// </list>
    /// </note>
    /// </remarks>
    public T DefaultValue { get; }

    /// <inheritdoc/>
    public Type DataType => typeof(T);
    
    /// <summary>
    /// Tries to convert the content of <see cref="Mapping"/> to <typeparamref name="T"/>.
    /// </summary>
    /// 
    /// <param name="result">
    /// After the method was successful, contains the <typeparamref name="T"/> value that is equivalent
    /// to the content of the converted <see cref="DynamicProperty"/> instances in
    /// <see cref="Mapping"/>,
    /// or the default value of <typeparamref name="T"/> if the parsing failed.
    /// </param>
    /// <returns><c>true</c> if the parsing was successfull, otherwise <c>false</c>.</returns>
    /// <remarks>
    /// <note type="implement">
    /// Implement this method in derived classes to determine the behavior of <see cref="Parse"/>.
    /// <para>
    /// </para>
    /// <para>
    /// In any case the method MUST NOT throw an exception. Instead, it should return <c>false</c> 
    /// if parsing fails. In this case <paramref name="result"/> is treated as undefined.
    /// </para>
    /// </note>
    /// </remarks>
    protected abstract bool TryParse(out T? result);

    /// <summary>
    /// Converts the content of <see cref="Mapping"/> to <typeparamref name="T"/>.
    /// </summary>
    /// <returns>An instance of <typeparamref name="T"/> or <see cref="DefaultValue"/>.</returns>
    /// <exception cref="FormatException">The conversion fails and <see cref="Throwing"/> is <c>true</c>.
    /// </exception>
    /// <remarks>
    /// <note type="implement">
    /// Override <see cref="TryParse(out T?)"/> to define the behavior of this method.
    /// </note>
    /// </remarks>
    public T? Parse()
        => TryParse(out T? result)
             ? result
             : Throwing
                 ? throw new FormatException(string.Format(CultureInfo.CurrentCulture, Res.CannotParseCsv, typeof(T).FullName))
                 : DefaultValue;

    /// <summary>
    /// Writes a <typeparamref name="T"/> value to several properties of <see cref="Mapping"/>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <remarks>
    /// <note type="implement">
    /// Implement this method in derived classes to determine the behavior of <see cref="ConvertToCsv(object?)"/>
    /// and <see cref="ConvertToCsv(T?)"/>. (Several <see cref="DynamicProperty"/> instances in <see cref="Mapping"/>
    /// have to be filled with data.)
    /// </note>
    /// </remarks>
    /// <exception cref="FormatException">One of the susequent converters uses an invalid format string.</exception>
    protected abstract void DoConvertToCsv(T? value);

    /// <summary>
    /// Writes <paramref name="value"/> to the selected fields of <see cref="DynamicProperty.Record"/> 
    /// using <see cref="Mapping"/>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// 
    /// <remarks>
    /// <note type="implement">
    /// Override <see cref="DoConvertToCsv(T?)"/> to define the behavior of this method.
    /// </note>
    /// </remarks>
    /// 
    /// <exception cref="FormatException">One of the subsequent converters uses an invalid format string.</exception>
    public void ConvertToCsv(T? value) => DoConvertToCsv(value);

    /// <summary>
    /// Writes <paramref name="value"/> to the selected fields of <see cref="DynamicProperty.Record"/> 
    /// using <see cref="Mapping"/>.
    /// </summary>
    /// <param name="value">The object to write to the selected fields of <see cref="DynamicProperty.Record"/>.</param>
    /// 
    /// <remarks>
    /// <note type="implement">
    /// Override <see cref="DoConvertToCsv(T?)"/> to define the behavior of this method.
    /// </note>
    /// </remarks>
    /// 
    /// <exception cref="InvalidCastException"><paramref name="value"/> has an incompatible data type.</exception>
    /// <exception cref="FormatException">One of the subsequent converters uses an invalid format string.</exception>
    public void ConvertToCsv(object? value)
    {
        if (value is null && !AcceptsNull)
        {
            throw new InvalidCastException(string.Format(CultureInfo.CurrentCulture, Res.CannotCastNull, typeof(T).FullName));
        }
        else
        {
            DoConvertToCsv((T?)value);
        }
    }
}
