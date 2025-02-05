using FolkerKinzel.CsvTools.Mappings.Converters;
using FolkerKinzel.CsvTools.Mappings.Converters.Interfaces;
using FolkerKinzel.CsvTools.Mappings.Intls;

namespace FolkerKinzel.CsvTools.Mappings;

/// <summary>
/// Extension methods for <see cref="DynamicProperty"/>.
/// </summary>
public static class DynamicPropertyExtension 
{
    /// <summary>
    /// Casts a <see cref="DynamicProperty"/> to a <see cref="ITypedProperty{T}"/>
    /// in order to have type safe access to its <see cref="ITypedProperty{T}.Value"/>
    /// without having to use dynamic .NET properties ("late binding").
    /// </summary>
    /// <typeparam name="T">The <see cref="ITypeConverter{T}.DataType"/> of the 
    /// <see cref="TypeConverter{T}"/> that <paramref name="property"/> has
    /// been initialized with.</typeparam>
    /// <param name="property">The <see cref="DynamicProperty"/> to cast.</param>
    /// <returns>
    /// <paramref name="property"/> casted as <see cref="ITypedProperty{T}"/>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method exists in order to support high-performance scenarios. It allows you 
    /// to process value types without boxing and unboxing.
    /// </para>
    /// <para>
    /// The method is just syntactic sugar around a simple cast. When passing a <c>null</c>
    /// reference as argument the compiler will give you a nullability warning but the return type 
    /// will be <c>null</c>.
    /// </para>
    /// <note type="caution">
    /// When using nullable reference types, take care to cast to the correct nullability:
    /// The compiler won't warn you when casting incorrectly!
    /// </note>
    /// </remarks>
    /// 
    /// <exception cref="InvalidCastException"><paramref name="property"/> is not of type 
    /// <see cref="ITypedProperty{T}"/>.</exception>"
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ITypedProperty<T> AsITypedProperty<T>(this DynamicProperty property) => (ITypedProperty<T>)property;
}
