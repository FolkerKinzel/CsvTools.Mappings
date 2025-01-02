using FolkerKinzel.CsvTools.Mappings.Converters;
using FolkerKinzel.CsvTools.Mappings.Intls;

namespace FolkerKinzel.CsvTools.Mappings;

/// <summary>
/// Extension methods for <see cref="MappingProperty"/>.
/// </summary>
public static class MappingPropertyExtension 
{
    /// <summary>
    /// Casts a <see cref="MappingProperty"/> to a <see cref="ITypedProperty{T}"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="ITypeConverter{T}.DataType"/> of the 
    /// <see cref="TypeConverter{T}"/> that <paramref name="property"/> has
    /// been initialized with.</typeparam>
    /// <param name="property">The <see cref="MappingProperty"/> to cast.</param>
    /// <returns>
    /// <paramref name="property"/> casted to <see cref="ITypedProperty{T}"/>.
    /// </returns>
    /// <remarks>
    /// <note type="caution">
    /// When using nullable reference types, take care to cast to the correct nullability:
    /// The compiler won't warn you!
    /// </note>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="property"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidCastException"><paramref name="property"/> is not of type 
    /// <see cref="ITypedProperty{T}"/>.</exception>"
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ITypedProperty<T> AsITypedProperty<T>(this MappingProperty property)
    {
        _ArgumentNullException.ThrowIfNull(property, nameof(property));
        return (ITypedProperty<T>)property;
    }
}
