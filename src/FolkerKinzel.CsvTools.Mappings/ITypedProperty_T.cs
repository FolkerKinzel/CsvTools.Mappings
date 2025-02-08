using FolkerKinzel.CsvTools.Mappings.TypeConverters;
using FolkerKinzel.CsvTools.Mappings.TypeConverters.Interfaces;

namespace FolkerKinzel.CsvTools.Mappings;

/// <summary>
/// Interface for typed <see cref="DynamicProperty"/> 
/// instances.
/// </summary>
/// <typeparam name="T">The type of the dynamic properties value.</typeparam>
public interface ITypedProperty<T> : IDynamicProperty
{
    /// <summary>
    /// Allows to get and set the value of the referenced field in the CSV file
    /// without having to use a dynamic property.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This property supports high performance scenarios: boxing and unboxing of 
    /// value types can be omitted in this way.
    /// </para>
    /// <para>
    /// Use the extension method <see cref="DynamicPropertyExtension.AsITypedProperty{T}(DynamicProperty)"/>
    /// to cast a <see cref="DynamicProperty"/> instance to an <see cref="ITypedProperty{T}"/>
    /// instance.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException"><see cref="DynamicProperty.Record"/>
    /// is <c>null</c>. Assign a <see cref="CsvRecord"/> instance to 
    /// <see cref="CsvRecordMapping.Record"/> before accessing this property.</exception>
    /// <exception cref="InvalidCastException">
    /// When setting the value,
    /// <paramref name="value"/> is <c>null</c> and 
    /// <see cref="ITypeConverter{T}.AcceptsNull"/> is <c>false</c>.
    /// </exception>
    /// <exception cref="FormatException">
    /// <para>
    /// When getting the value, parsing fails and <see cref="TypeConverter{T}.Throwing"/>
    /// is <c>true</c>.
    /// </para>
    /// <para>- or -</para>
    /// <para>
    /// When setting the value, the converter uses an invalid format string.
    /// </para>
    /// </exception>
    T Value { get; set; }

    // Value can't be T? here: If it were, this property could never return
    // a non-nullable reference type.

    /// <summary>
    /// Gets the value that the <see cref="ITypedProperty{T}"/> returns if parsing 
    /// fails.
    /// </summary>
    T? DefaultValue { get; }

    /// <summary>
    /// An object implementing <see cref="ITypeConverter{T}"/> that performs the type conversion.
    /// </summary>
    ITypeConverter<T> Converter { get; }
}