using FolkerKinzel.CsvTools.Mappings.Converters;
using FolkerKinzel.CsvTools.Mappings.Intls;
using FolkerKinzel.CsvTools.Mappings.Intls.Extensions;
using FolkerKinzel.CsvTools.Mappings.Resources;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace FolkerKinzel.CsvTools.Mappings;

/// <summary>
/// Mapping for <see cref="CsvRecord"/> instances.
/// </summary>
/// <remarks>
/// <para>
/// The class allows you to index the data of the <see cref="CsvRecord"/>
/// object in a sequence of your choice, to access the data with .NET properties dynamically implemented at runtime ("late binding"), and to 
/// perform type conversions automatically. In order to be able to use the dynamic properties of the <see cref="CsvRecordMapping"/> class, the 
/// instance of <see cref="CsvRecordMapping"/> must be assigned to a variable that is declared with the keyword <c>dynamic</c>. Use <see cref="Create"/>
/// to create a new instance.
/// </para>
/// <para>
/// An instance of the <see cref="DynamicProperty"/> class represent a dynamic property of the <see cref="CsvRecordMapping"/> object.
/// The extension methods of the <see cref="CsvRecordMappingExtension"/> class can be used to create and add <see cref="DynamicProperty"/> instances. 
/// The order, in which the <see cref="DynamicProperty"/> instances are added to a <see cref="CsvRecordMapping"/> instance, determines their index in 
/// that <see cref="CsvRecordMapping"/> instance. These indexes may differ from the indexes of the columns of the CSV file that these 
/// <see cref="DynamicProperty"/> instances access.
/// </para>
/// <para>
/// With <see cref="Contains(string)"/> you can check whether a <see cref="DynamicProperty"/> instance with the specified 
/// <see cref="DynamicProperty.PropertyName"/> is already added.
/// </para>
/// <para>
/// In order to support high-performance scenarios, the <see cref="DynamicProperty"/> instances can be accessed accessed directly without
/// having to use dynamic .NET properties: Use the indexers <see cref="this[int]"/> or <see cref="this[string]"/> to get a 
/// <see cref="DynamicProperty"/> instance. Cast it with <see cref="MappingPropertyExtension.AsITypedProperty{T}(DynamicProperty)"/>
/// and access its data directly with <see cref="ITypedProperty{T}.Value"/>. With this approach boxing and unboxing of value types can 
/// be avoided.
/// </para>
/// <para>
/// For dynamic properties, the compiler does not monitor the nullability of reference types. With the approach just presented, the use of 
/// dynamic properties can be avoided. A critical point here is the correct type cast with the 
/// <see cref="MappingPropertyExtension.AsITypedProperty{T}(DynamicProperty)"/> method: Make sure that you choose the correct nullability of 
/// the reference types. The compiler does not check this.
/// </para>
/// <para>
/// For CSV writing it is sufficient to assign the <see cref="CsvRecord"/> instance to the <see cref="CsvRecordMapping"/> object only once, because 
/// the same <see cref="CsvRecord"/> instance is always used. For reading the current <see cref="CsvRecord"/> instance has to be assigned with
/// every iteration. An exception is when a CSV file is read with the <see cref="CsvOpts.DisableCaching"/> flag: then the <see cref="CsvRecord"/>
/// object is the same for each iteration.
/// </para>
/// </remarks>
/// <example>
/// <note type="note">In the following code examples - for easier readability - exception handling has been omitted.</note>
/// <para>
/// Saving the contents of a <see cref="DataTable"/> as a CSV file and importing data from a CSV file into a 
/// <see cref="DataTable"/>: </para>
/// <code language="cs" source="..\Examples\CsvToDataTable.cs"/>
/// <para>Deserializing any objects from CSV files:</para>
/// <code language="cs" source="..\Examples\DeserializingClassesFromCsv.cs"/>
/// </example>
public sealed class CsvRecordMapping : DynamicObject, IEnumerable<DynamicProperty>
{
    private class PropertyCollection : KeyedCollection<string, DynamicProperty>
    {
        protected override string GetKeyForItem(DynamicProperty item) => item.PropertyName;

        public PropertyCollection() : base(StringComparer.Ordinal) { }
    }

    private static int _regexTimeout = 10;
    private readonly PropertyCollection _dynProps = new();
    private CsvRecord? _record;

    /// <summary>
    /// Initializes a new <see cref="CsvRecordMapping"/> instance. 
    /// </summary>
    /// <remarks>
    /// <note type="important">
    /// Before accessing the dynamic properties a <see cref="CsvRecord"/> object 
    /// has to be assigned to <see cref="Record"/>.
    /// </note>
    /// </remarks>
    private CsvRecordMapping() { }

    /// <summary>
    /// Creates a new <see cref="CsvRecordMapping"/> instance. 
    /// </summary>
    /// <remarks>
    /// <note type="important">
    /// Before accessing the dynamic properties a <see cref="CsvRecord"/> object 
    /// has to be assigned to <see cref="Record"/>.
    /// </note>
    /// </remarks>
    public static CsvRecordMapping Create() => [];

    /// <summary>
    /// Maximum time (in milliseconds) that can be used to resolve a <see cref="Regex"/>.
    /// </summary>
    public const int MaxRegexTimeout = 100;

    /// <summary>
    /// Maximum time (in milliseconds) that can be used to resolve a CSV column 
    /// name alias. 
    /// </summary>
    /// <value>If the value is greater than <see cref="MaxRegexTimeout"/>, 
    /// <see cref="MaxRegexTimeout"/> is used instead. The default value is 10.
    /// Set this value to <see cref="Timeout.Infinite"/> to disable the timeout.</value>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Value is less than 1 and not <see cref="Timeout.Infinite"/>.
    /// </exception>
    public static int RegexTimeout
    {
        get => _regexTimeout;
        set => _regexTimeout = TimeoutHelper.NormalizeRegexTimeout(value, nameof(value));
    }

    /// <summary>
    /// Gets or sets the <see cref="CsvRecord"/> instance whose data is accessed with dynamic properties.
    /// </summary>
    public CsvRecord? Record
    {
        get => _record;

        set
        {
            if (!object.ReferenceEquals(_record, value))
            {
                _record = value;

                for (int i = 0; i < _dynProps.Count; i++)
                {
                    _dynProps[i].Record = value;
                }
            }
        }
    }

    /// <summary>
    /// Gets the number of <see cref="DynamicProperty"/> instances in the 
    /// <see cref="CsvRecordMapping"/>.
    /// </summary>
    public int Count => _dynProps.Count;

    /// <summary>
    /// Returns the property names, which are currently registered in the <see cref="CsvRecordMapping"/>.
    /// </summary>
    public IEnumerable<string> PropertyNames => _dynProps.Select(x => x.PropertyName);

    /// <summary>
    /// Gets the <see cref="DynamicProperty"/> with the specified <paramref name="index"/>. 
    /// </summary>
    /// <param name="index">Zero-based index of the <see cref="DynamicProperty"/> instance.</param>
    /// 
    /// <remarks>
    /// The <paramref name="index"/> corresponds to the order in which the <see cref="DynamicProperty"/> 
    /// instances had been added to the <see cref="CsvRecordMapping"/>.
    /// </remarks>
    /// 
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="index"/> is less than zero or greater than or equal to <see cref="Count"/>.
    /// </exception>
    public DynamicProperty this[int index] => _dynProps[index];

    /// <summary>
    /// Gets the <see cref="DynamicProperty"/> with the specified <see cref="DynamicProperty.PropertyName"/>.
    /// </summary>
    /// <param name="propertyName"> <see cref="DynamicProperty.PropertyName"/> of the<see cref="DynamicProperty"/> instance.
    /// The comparison is case-sensitive.</param>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// There was no <see cref="DynamicProperty"/> found whose 
    /// <see cref="DynamicProperty.PropertyName"/> property matches <paramref name="propertyName"/>. Check this beforehand with
    /// <see cref="CsvRecordMapping.Contains(string?)"/>.
    /// </exception>
    public DynamicProperty this[string propertyName] => _dynProps[propertyName];

    /// <summary>
    /// Registers a <see cref="DynamicProperty"/> at the end of the list of registered properties.
    /// </summary>
    /// <param name="property">The <see cref="DynamicProperty"/> to be added.</param>
    /// <exception cref="ArgumentException">
    /// A <see cref="DynamicProperty"/> with the same <see cref="DynamicProperty.PropertyName"/> has already been added.
    /// Check this beforehand with <see cref="CsvRecordMapping.Contains(string)"/>!
    /// </exception>
    internal void Add(DynamicProperty property)
    {    
        this._dynProps.Add(property);
        property.Record = Record;
    }

    /// <summary>
    /// Removes all <see cref="DynamicProperty"/> instances from the <see cref="CsvRecordMapping"/>.
    /// </summary>
    internal void Clear() => _dynProps.Clear(); // Must be internal to make the extension method usable!

    ///// <summary>
    ///// Removes the <see cref="MappingProperty"/> with the specified <see cref="MappingProperty.PropertyName"/>
    ///// from the list of registered properties.
    ///// </summary>
    ///// <param name="propertyName">
    ///// The <see cref="MappingProperty.PropertyName"/> of the <see cref="MappingProperty"/> to remove.
    ///// </param>
    ///// <returns>
    ///// <c>true</c> if the searched <see cref="MappingProperty"/> was among the registered properties and could 
    ///// be removed.
    ///// </returns>
    ///// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <c>null</c>.</exception>
    //public bool RemoveProperty(string propertyName) => _dynProps.Remove(propertyName);

    ///// <summary>
    ///// Removes the <see cref="MappingProperty"/> on the specified <paramref name="index"/> from the list of 
    ///// registered properties.
    ///// </summary>
    ///// <param name="index">
    ///// The zero-based index at which the <see cref="MappingProperty"/> should be removed.
    ///// </param>
    ///// <exception cref="ArgumentOutOfRangeException"> <paramref name="index"/> index is less than zero or greater than or 
    ///// equal to <see cref="Count"/>.</exception>
    //public void RemovePropertyAt(int index) => _dynProps.RemoveAt(index);

    ///// <summary>
    ///// Inserts <paramref name="property"/> at <paramref name="index"/> in the list of the registered properties.
    ///// </summary>
    ///// <param name="index">
    ///// The zero-based index at which to insert <paramref name="property"/>.</param>
    ///// <param name="property">The <see cref="MappingProperty"/> to insert.</param>
    ///// 
    ///// <exception cref="ArgumentNullException"><paramref name="property"/> is <c>null</c>.</exception>
    ///// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> index is less than zero or greater than or equal
    ///// to <see cref="Count"/>.</exception>
    ///// 
    ///// <exception cref="ArgumentException">
    ///// There is already a <see cref="MappingProperty"/> with the same <see cref="MappingProperty.PropertyName"/> registered. 
    ///// Check this beforehand with <see cref="Contains(string?)"/>!
    ///// </exception>
    //public void InsertProperty(int index, MappingProperty property)
    //{
    //    _ArgumentNullException.ThrowIfNull(property, nameof(property));

    //    _dynProps.Insert(index, property);
    //    property.Record = Record;
    //}

    ///// <summary>
    ///// Replaces the <see cref="MappingProperty"/> at the specified index of the list of registered properties 
    ///// with <paramref name="property"/>.
    ///// </summary>
    ///// <param name="index">
    ///// The zero-based index at which the registered <see cref="MappingProperty"/> instance is replaced with 
    ///// <paramref name="property"/>.</param>
    ///// <param name="property">A <see cref="MappingProperty"/> instance.</param>
    ///// 
    ///// <exception cref="ArgumentNullException"><paramref name="property"/> is <c>null</c>.</exception>
    ///// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> index is less than zero or 
    ///// greater than or equal to <see cref="Count"/>.</exception>
    ///// <exception cref="ArgumentException">
    ///// There is already a <see cref="MappingProperty"/> with the same <see cref="MappingProperty.PropertyName"/> 
    ///// registered. 
    ///// Check this beforehand with <see cref="Contains(string?)"/>!
    ///// </exception>
    //public void ReplacePropertyAt(int index, MappingProperty property)
    //{
    //    _ArgumentNullException.ThrowIfNull(property, nameof(property));
    //    _dynProps[index] = property;
    //}

    ///// <summary>
    ///// Replaces the registered <see cref="MappingProperty"/> instance whose <see cref="MappingProperty.PropertyName"/>
    ///// property equals <paramref name="propertyName"/> with <paramref name="property"/>.
    ///// </summary>
    ///// <param name="propertyName">Identifier of the registered <see cref="MappingProperty"/> instance to be replaced. 
    ///// (See <see cref="MappingProperty.PropertyName"/>.)</param>
    ///// <param name="property">The <see cref="MappingProperty"/> instance object to be used for replacement.</param>
    /////
    ///// <exception cref="ArgumentNullException"><paramref name="propertyName"/>, or <paramref name="property"/>
    ///// is <c>null</c>.</exception>
    ///// 
    ///// <exception cref="ArgumentException">
    ///// <para>
    ///// There is no registered <see cref="MappingProperty"/> whose <see cref="MappingProperty.PropertyName"/> property 
    ///// matches <paramref name="propertyName"/>,
    ///// </para>
    ///// <para>- or -</para>
    ///// <para>
    ///// there is already a registered <see cref="MappingProperty"/> instance whose <see cref="MappingProperty.PropertyName"/> 
    ///// property is identical with which of <paramref name="property"/>.
    ///// Check this beforehand with <see cref="Contains(string?)"/>!
    ///// </para>
    ///// </exception>
    //public void ReplaceProperty(string propertyName, MappingProperty property)
    //{
    //    _ArgumentNullException.ThrowIfNull(propertyName, nameof(propertyName));
    //    _ArgumentNullException.ThrowIfNull(property, nameof(property));

    //    try
    //    {
    //        int index = _dynProps.IndexOf(_dynProps[propertyName]);
    //        _dynProps[index] = property;
    //    }
    //    catch (KeyNotFoundException e)
    //    {
    //        throw new ArgumentException(string.Format(Res.PropertyNotFound, nameof(propertyName)), nameof(propertyName), e);
    //    }
    //}

    /// <summary>
    /// Examines whether a <see cref="DynamicProperty"/> instance is already registered in the <see cref="CsvRecordMapping"/>
    /// under the name that is specified with <paramref name="propertyName"/>.
    /// </summary>
    /// <param name="propertyName">The <see cref="DynamicProperty.PropertyName"/> of the <see cref="DynamicProperty"/> instance
    /// to be searched for.</param>
    /// <returns><c>true</c>, wenn ein <see cref="DynamicProperty"/>-Objekt unter dem mit <paramref name="propertyName"/>
    /// angegebenen Namen registriert ist.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <c>null</c>.</exception>
    public bool Contains(string propertyName) => _dynProps.Contains(propertyName);
    
    ///// <summary>
    ///// Gets the index of the <see cref="MappingProperty"/> whose <see cref="MappingProperty.PropertyName"/> equals
    ///// <paramref name="propertyName"/>, or -1 if no such property has been found.
    ///// </summary>
    ///// <param name="propertyName">Der Eigenschaftsname der zu suchenden <see cref="MappingProperty"/>.</param>
    ///// <returns>The index of the <see cref="MappingProperty"/> whose <see cref="MappingProperty.PropertyName"/> equals
    ///// <paramref name="propertyName"/>, or -1 if no such property has been found.</returns>
    ///// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <c>null</c>.</exception>
    //public int IndexOf(string propertyName) => _dynProps.Contains(propertyName) ? _dynProps.IndexOf(_dynProps[propertyName]) : -1;

    /// <inheritdoc/>
    public IEnumerator<DynamicProperty> GetEnumerator() => ((IEnumerable<DynamicProperty>)_dynProps).GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => _dynProps.GetEnumerator();

    /// <summary>
    /// Wird automatisch aufgerufen, wenn einer dynamisch implementierten Eigenschaft ein Wert
    /// zugewiesen wird. (Nicht zur direkten Verwendung in eigenem Code bestimmt.)
    /// </summary>
    /// <param name="binder">Informationen über die aufrufende dynamische Eigenschaft.</param>
    /// <param name="value">Das Objekt, das der dynamisch implementierten Eigenschaft zugewiesen wird.</param>
    /// <returns><c>true</c>, wenn auf eine Eigenschaft zugegriffen wurde, die bereits zuvor als <see cref="DynamicProperty"/>
    /// im <see cref="CsvRecordMapping"/>-Objekt registriert wurde.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="binder"/> ist <c>null</c>. (Das kann nur passieren,
    /// wenn die Methode direkt aus eigenem Code aufgerufen wird.)</exception>
    /// 
    /// 
    /// <exception cref="InvalidCastException">
    /// The data type of <paramref name="value"/> does not match the data type of the 
    /// registered property.
    /// </exception>
    /// 
    /// <exception cref="Exception">Es wurde versucht, auf eine nicht registrierte Property zuzugreifen.</exception>
    /// 
    /// <exception cref="InvalidOperationException">No <see cref="CsvRecord"/> instance was assigned to <see cref="Record"/>.</exception>
    /// <exception cref="InvalidCastException">
    /// <para>
    /// <paramref name="value"/> is <c>null</c> and 
    /// <see cref="ITypeConverter{T}.AllowsNull"/> is <c>false</c>,
    /// </para>
    /// <para>- or -</para>
    /// <para>
    /// <paramref name="value"/> does not match the expected data type.
    /// </para>
    /// </exception>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        _ArgumentNullException.ThrowIfNull(binder, nameof(binder));
        
        if (this._dynProps.TryGetValue(binder.Name, out DynamicProperty? prop))
        {
            prop.Value = value;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Wird automatisch aufgerufen, wenn der Wert einer dynamisch implementierten Eigenschaft
    /// einer Variablen zugewiesen wird. (Nicht zur direkten Verwendung in eigenem Code bestimmt.)
    /// </summary>
    /// <param name="binder">Informationen über die aufrufende dynamische Eigenschaft.</param>
    /// <param name="result">Das Objekt, das den Rückgabewert der dynamisch implementierten Eigenschaft darstellt.</param>
    /// <returns><c>true</c>, wenn auf eine Eigenschaft zugegriffen wurde, die zuvor als <see cref="DynamicProperty"/>
    /// im <see cref="CsvRecordMapping"/>-Objekt registriert wurde.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="binder"/> ist <c>null</c>. (Das kann nur passieren,
    /// wenn die Methode direkt aus eigenem Code aufgerufen wird.)</exception>
    /// 
    /// <exception cref="FormatException">
    /// The return value of the accessed <see cref="DynamicProperty"/> could not be parsed successfully and the 
    /// type converter of this <see cref="DynamicProperty"/> was configured to throw an <see cref="FormatException"/>
    /// in this case.
    /// </exception>
    /// 
    /// <exception cref="InvalidOperationException">No <see cref="CsvRecord"/> instance was assigned to <see cref="Record"/>.</exception>
    /// <exception cref="FormatException">Parsing fails and <see cref="TypeConverter{T}.Throwing"/> is <c>true</c>.</exception>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        _ArgumentNullException.ThrowIfNull(binder, nameof(binder));

        if (this._dynProps.TryGetValue(binder.Name, out DynamicProperty? prop))
        {
            result = prop.Value;
            return true;
        }

        result = null;
        return false;
    }

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override IEnumerable<string> GetDynamicMemberNames() => base.GetDynamicMemberNames();

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override DynamicMetaObject GetMetaObject(Expression parameter) => base.GetMetaObject(parameter);

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object? result) => base.TryBinaryOperation(binder, arg, out result);

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool TryConvert(ConvertBinder binder, out object? result) => base.TryConvert(binder, out result);

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool TryCreateInstance(CreateInstanceBinder binder, object?[]? args, [NotNullWhen(true)] out object? result) => base.TryCreateInstance(binder, args, out result);

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes) => base.TryDeleteIndex(binder, indexes);

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool TryDeleteMember(DeleteMemberBinder binder) => base.TryDeleteMember(binder);

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object? result) => base.TryGetIndex(binder, indexes, out result);

    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool TryInvoke(InvokeBinder binder, object?[]? args, out object? result) => base.TryInvoke(binder, args, out result);

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result) => base.TryInvokeMember(binder, args, out result);

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object? value) => base.TrySetIndex(binder, indexes, value);

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool TryUnaryOperation(UnaryOperationBinder binder, out object? result) => base.TryUnaryOperation(binder, out result);


    /// <inheritdoc/>
    public override string ToString()
    {
        if (Record is null || Count == 0)
        {
            return base.ToString() ?? string.Empty;
        }

        var sb = new StringBuilder();

        foreach (string propName in this.PropertyNames)
        {
            object? value;

            string valString;

            try
            {
                value = this[propName];
                valString = value is null ? "<null>" : value is DBNull ? "<DBNull>" : value.ToString() ?? string.Empty;
            }
            catch
            {
                valString = "<Exception>";
            }

            _ = sb.Append(propName).Append(": ").Append(valString).Append(", ");
        }

        if (sb.Length >= 2)
        {
            sb.Length -= 2;
        }

        return sb.ToString();
    }
}
