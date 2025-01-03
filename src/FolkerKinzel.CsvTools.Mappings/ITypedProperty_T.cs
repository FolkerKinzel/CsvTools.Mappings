namespace FolkerKinzel.CsvTools.Mappings;

/// <summary>
/// Interface for typed <see cref="MappingProperty"/> 
/// instances.
/// </summary>
/// <typeparam name="T">The type of the dynamic properties value.</typeparam>
public interface ITypedProperty<T>
{
    /// <summary>
    /// Allows to get and set the value of the referenced field in the CSV file
    /// without having to use a dynamic property.
    /// </summary>
    /// <remarks>
    /// This property supports high performance scenarios: boxing and unboxing of 
    /// value types can be omitted in this way.
    /// </remarks>
    /// <exception cref="InvalidOperationException"><see cref="Record"/> is <c>null</c>. Assign a <see cref="CsvRecord"/> instance
    /// to <see cref="CsvRecordMapping.Record"/> first before accessing this property.</exception>
    /// <exception cref="InvalidCastException">
    /// When setting the value,
    /// <paramref name="value"/> is <c>null</c> and 
    /// <see cref="ITypeConverter{T}.AllowsNull"/> is <c>false</c>.
    /// </exception>
    /// <exception cref="FormatException">When getting the value, parsing fails and <see cref="TypeConverter{T}.Throwing"/>
    /// is <c>true</c>.</exception>
    T Value { get; set; }

    // Value can't be T?. If it were, this property could never return
    // a non-nullable reference type.
}