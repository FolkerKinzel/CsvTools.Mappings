
namespace FolkerKinzel.CsvTools.Mappings;

public interface ITypedProperty<T>
{
    ///// <summary>
    ///// The data type of the dynamic property.
    ///// </summary>
    //Type DataType { get; }

    /// <summary>
    /// Allows to get and set the value of the referenced field in the CSV file
    /// without having to use a dynamic property.
    /// </summary>
    /// <remarks>
    /// This property supports high performance scenarios: boxing and unboxing of 
    /// value types can be omitted in this way.
    /// </remarks>
    T? Value { get; set; }
}