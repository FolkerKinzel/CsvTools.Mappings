namespace FolkerKinzel.CsvTools.Mappings.Converters.Interfaces;

/// <summary>
/// Interface for classes that provide conversions between CSV data and .NET data types.
/// </summary>
/// <typeparam name="T">The .NET data type the converter can convert.</typeparam>
public interface ITypeConverter<T>
{
    /// <summary>
    /// Gets a value indicating whether the converter throws a
    /// <see cref="FormatException"/> 
    /// when a parsing error occurs, or if it returns 
    /// <see cref="DefaultValue"/> value instead.
    /// </summary>
    /// <value><c>true</c> if the converter throws a 
    /// <see cref="FormatException"/> on parsing errors,
    /// <c>false</c> to return <see cref="DefaultValue"/>
    /// in this case.</value>
    bool Throwing { get; }

    /// <summary>
    /// Gets the value to return if parsing fails and
    /// the <see cref="Throwing"/> property is <c>false</c>.
    /// </summary>
    /// <remarks>
    /// <note type="implement">
    /// In any case, the <see cref="ITypeConverter{T}"/> MUST
    /// accept this value as input.
    /// </note>
    /// </remarks>
    T DefaultValue { get; }

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
    /// The behavior is equivalent to the behavior of the AllowNullAttribute:
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
