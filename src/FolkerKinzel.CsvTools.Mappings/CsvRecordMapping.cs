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

namespace FolkerKinzel.CsvTools.Mappings;

/// <summary>
/// Mapping for <see cref="CsvRecord"/> instances.
/// </summary>
/// <remarks>
/// <para>
/// The class forms a wrapper around objects of the class <see cref="CsvRecord"/>, which enables you to index the data of the <see cref="CsvRecord"/>
/// object in a sequence of your choice, to access the data with properties dynamically implemented at runtime, and to perform type conversions. In 
/// order to be able to use the dynamically 
/// implemented properties of the <see cref="CsvRecordMapping"/> class directly ("late binding") and to assign the return values of the indexers without a cast, the 
/// instance of <see cref="CsvRecordMapping"/> must be assigned to a variable that is declared with the keyword <c>dynamic</c>.
/// </para>
/// <para>
/// After a <see cref="CsvRecordMapping"/> object has been initialized, the method <see cref="AddProperty(MappingProperty)"/> has to be called to 
/// register <see cref="MappingProperty"/> 
/// objects, which are dynamic properties of the <see cref="CsvRecordMapping"/> object and perform type conversions for the string data in the underlying 
/// <see cref="CsvRecord"/> object. The order in which the properties are registered determines the order, in which the return values ​​of these properties 
/// are returned if the <see cref="CsvRecordMapping"/> object is iterated in a <c>foreach</c> loop, or if it is accessed with the indexer <see cref="this[int]"/>. 
/// </para>
/// <para>
/// Because of the <see cref="ColumnNameProperty{T}"/> objects access the underlying <see cref="CsvRecord"/> object via the CSV column name, the number and 
/// order of the <see cref="ColumnNameProperty{T}"/> objects registered 
/// in the <see cref="CsvRecordMapping"/> object don't have to match the column-order of the underlying <see cref="CsvRecord"/> object. 
/// The same is for <see cref="IndexProperty{T}"/> objects: The number and order of the registered <see cref="IndexProperty{T}"/> objects
/// is independent of the columns of the underlying <see cref="CsvRecord"/> object because the <see cref="IndexProperty{T}.CsvIndex"/> 
/// is stored inside of the <see cref="IndexProperty{T}"/> instances."/>
/// </para>
/// <para>The order of the registered properties can be influenced at any time with the following methods:</para>
/// <list type="bullet">
/// <item><see cref="InsertProperty(int, MappingProperty)"/></item>
/// <item><see cref="ReplaceProperty(string, MappingProperty)"/></item>
/// <item><see cref="ReplacePropertyAt(int, MappingProperty)"/></item>
/// <item><see cref="RemoveProperty(string)"/></item>
/// <item><see cref="RemovePropertyAt(int)"/></item>
/// </list>
/// With <see cref="Contains(string)"/> you can check whether a <see cref="MappingProperty"/> instance with the specified 
/// <see cref="MappingProperty.PropertyName"/> is already registered. 
/// <para>
/// The indexers <see cref="this[int]"/>, and <see cref="this[string]"/> can also be accessed if the <see cref="CsvRecordMapping"/>
/// instance is assigned to a normal variable, but the the values returned by the indexers possibly have to be cast into the actual 
/// <see cref="Type"/> using the cast operator since they are of the type <see cref="object"/>.
/// </para>
/// <para>
/// If the <see cref="CsvRecordMapping"/> instance is assigned to a variable that has been declared with the keyword <c>dynamic</c>, 
/// the runtime takes over the necessary casts. In addition, it is then possible to address the registered <see cref="MappingProperty"/> 
/// objects like normal .NET properties by their name (<see cref="MappingProperty.PropertyName"/>). The disadvantage is that Visual Studio cannot 
/// offer any "IntelliSense" on dynamic variables.
/// </para>
/// For writing it is sufficient to assign the <see cref="CsvRecord"/> instance to the <see cref="CsvRecordMapping"/> object only once, because 
/// the same <see cref="CsvRecord"/> instance is always used. For reading the current <see cref="CsvRecord"/> instance has to be assigned with
/// every iteration. An exception is when a CSV file is read with the <see cref="CsvOpts.DisableCaching"/> flag: then the <see cref="CsvRecord"/>
/// object is the same for each iteration.
/// <para>
/// If <see cref="MultiColumnProperty{T}"/> objects are inserted into <see cref="CsvRecordMapping"/>, the <see cref="CsvRecordMapping"/> instances 
/// of their <see cref="MultiColumnTypeConverter{T}"/> will automatically get the current <see cref="CsvRecord"/> instance via the parent 
/// <see cref="CsvRecordMapping"/> they are inserted into.
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
public sealed class CsvRecordMapping : DynamicObject, IEnumerable<KeyValuePair<string, object?>>
{
    private class PropertyCollection : KeyedCollection<string, MappingProperty>
    {
        protected override string GetKeyForItem(MappingProperty item) => item.PropertyName;

        public PropertyCollection() : base(StringComparer.Ordinal) { }
    }

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
    public CsvRecordMapping() { }

    /// <summary>
    /// The <see cref="CsvRecord"/> instance whose data is accessed with dynamic properties.
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
    /// Gets the number of <see cref="MappingProperty"/> instances, which are registered in the 
    /// <see cref="CsvRecordMapping"/>.
    /// </summary>
    public int Count => _dynProps.Count;

    /// <summary>
    /// Returns the property names, which are currently registered in the <see cref="CsvRecordMapping"/>.
    /// </summary>
    public IEnumerable<string> PropertyNames => _dynProps.Select(x => x.PropertyName);

    ///// <summary>
    ///// Gibt eine Kopie der in <see cref="CsvRecord"/> gespeicherten Daten zurück.
    ///// </summary>
    ///// <exception cref="InvalidCastException">Der Rückgabewert einer indexierten <see cref="CsvPropertyBase"/> konnte nicht erfolgreich geparst werden und 
    ///// der <see cref="ICsvTypeConverter"/> dieser <see cref="CsvPropertyBase"/> war so konfiguriert, dass er in diesem Fall eine
    ///// Ausnahme wirft.</exception>
    ///// <exception cref="InvalidOperationException">Es wurde versucht, auf die Daten von <see cref="CsvRecordMapping"/> zuzugreifen, ohne dass diesem
    ///// ein <see cref="CsvRecord"/>-Objekt zugewiesen war.</exception>
    //public IList<object?> Values => this.Select(x => x.Value).ToArray();

    /// <summary>
    /// Allows access to the properties registered in <see cref="CsvRecordMapping"/> via a zero-based index. The index corresponds 
    /// to the order in which the <see cref="MappingProperty"/> objects are registered in the <see cref="CsvRecordMapping"/>.
    /// </summary>
    /// <param name="index">Zero-based index of the registered <see cref="MappingProperty"/> instance.</param>
    /// <returns>Return value of the <see cref="MappingProperty"/> registered at <paramref name="index"/>.</returns>
    /// 
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="index"/> index is less than zero or greater than or equal to <see cref="Count"/>.</exception>
    /// 
    /// <exception cref="InvalidCastException">
    /// <para>
    /// An object was assigned to an <paramref name="index"/> whose data type does not match the data type of the property that is 
    /// registered to this <paramref name="index"/>, 
    /// </para>
    /// <para>- or -</para>
    /// <para>
    /// the return value of the indexed <see cref="MappingProperty"/> could not be parsed successfully and the 
    /// type converter of this <see cref="MappingProperty"/> was configured to throw an <see cref="InvalidCastException"/>
    /// in this case.
    /// </para>
    /// </exception>
    /// 
    /// <exception cref="InvalidOperationException">No <see cref="CsvRecord"/> instance was assigned to <see cref="Record"/>.</exception>
    public object? this[int index]
    {
        get
        {
            return Record is null
                ? throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Res.CsvRecordIsNull, nameof(Record)))
                : _dynProps[index].GetValue();
        }

        set
        {
            if (Record is null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Res.CsvRecordIsNull, nameof(Record)));
            }

            _dynProps[index].SetValue(value);
        }
    }

    /// <summary>
    /// Allows access to the properties registered in <see cref="CsvRecordMapping"/> via the value of their 
    /// <see cref="MappingProperty.PropertyName"/> property.
    /// </summary>
    /// <param name="propertyName"> Name of the registered property. (Corresponds to <see cref="MappingProperty.PropertyName"/>.)
    /// The comparison is case-sensitive.</param>
    /// <returns>
    /// Return value of the registered <see cref="MappingProperty"/> whose <see cref="MappingProperty.PropertyName"/> property 
    /// matches <paramref name="propertyName"/>. The comparison is case-sensitive.
    /// </returns>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// There was no <see cref="MappingProperty"/> found among the properties registered at <see cref="CsvRecordMapping"/> whose 
    /// <see cref="MappingProperty.PropertyName"/> property matches <paramref name="propertyName"/>.
    /// </exception>
    /// 
    /// <exception cref="InvalidCastException">
    /// <para>
    /// An object was assigned to a <paramref name="propertyName"/> whose data type does not match the data type of the 
    /// property that is registered to this <paramref name="propertyName"/>, 
    /// </para>
    /// <para>- or -</para>
    /// <para>
    /// the return value of the accessed <see cref="MappingProperty"/> could not be parsed successfully and the 
    /// type converter of this <see cref="MappingProperty"/> was configured to throw an <see cref="InvalidCastException"/>
    /// in this case.
    /// </para>
    /// </exception>
    /// 
    /// <exception cref="InvalidOperationException">No <see cref="CsvRecord"/> instance was assigned to <see cref="Record"/>.</exception>
    public object? this[string propertyName]
    {
        get
        {
            return Record is null
                ? throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Res.CsvRecordIsNull, nameof(Record)))
                : propertyName is null
                    ? throw new ArgumentNullException(nameof(propertyName))
                    : this._dynProps.TryGetValue(propertyName, out MappingProperty? prop)
                        ? prop.GetValue()
                        : throw new ArgumentException(string.Format(Res.PropertyNotFound, nameof(propertyName)), nameof(propertyName));
        }

        set
        {
            if (Record is null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Res.CsvRecordIsNull, nameof(Record)));
            }

            _ArgumentNullException.ThrowIfNull(propertyName, nameof(propertyName));

            if (this._dynProps.TryGetValue(propertyName, out MappingProperty? prop))
            {
                prop.SetValue(value);
            }
            else
            {
                throw new ArgumentException(string.Format(Res.PropertyNotFound, nameof(propertyName)), nameof(propertyName));
            }
        }
    }

    /// <summary>
    /// Registers a <see cref="MappingProperty"/> at the end of the list of registered properties.
    /// </summary>
    /// <param name="property">The <see cref="MappingProperty"/> to be added.</param>
    /// <exception cref="ArgumentNullException"><paramref name="property"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// There is already a <see cref="MappingProperty"/> with the same <see cref="MappingProperty.PropertyName"/> registered. 
    /// Check this beforehand with <see cref="Contains(string?)"/>!
    /// </exception>
    public void AddProperty(MappingProperty property)
    {
        _ArgumentNullException.ThrowIfNull(property, nameof(property));
        
        this._dynProps.Add(property);
        property.Record = Record;
    }

    /// <summary>
    /// Removes the <see cref="MappingProperty"/> with the specified <see cref="MappingProperty.PropertyName"/>
    /// from the list of registered properties.
    /// </summary>
    /// <param name="propertyName">
    /// The <see cref="MappingProperty.PropertyName"/> of the <see cref="MappingProperty"/> to remove.
    /// </param>
    /// <returns>
    /// <c>true</c> if the searched <see cref="MappingProperty"/> was among the registered properties and could 
    /// be removed.
    /// </returns>
    public bool RemoveProperty(string? propertyName)
        => propertyName is not null && _dynProps.Remove(propertyName);

    /// <summary>
    /// Removes the <see cref="MappingProperty"/> on the specified <paramref name="index"/> from the list of 
    /// registered properties.
    /// </summary>
    /// <param name="index">
    /// The zero-based index at which the <see cref="MappingProperty"/> should be removed.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException"> <paramref name="index"/> index is less than zero or greater than or 
    /// equal to <see cref="Count"/>.</exception>
    public void RemovePropertyAt(int index)
    => _dynProps.RemoveAt(index);

    /// <summary>
    /// Inserts <paramref name="property"/> at <paramref name="index"/> in the list of the registered properties.
    /// </summary>
    /// <param name="index">
    /// The zero-based index at which to insert <paramref name="property"/>.</param>
    /// <param name="property">The <see cref="MappingProperty"/> to insert.</param>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="property"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> index is less than zero or greater than or equal
    /// to <see cref="Count"/>.</exception>
    /// 
    /// <exception cref="ArgumentException">
    /// There is already a <see cref="MappingProperty"/> with the same <see cref="MappingProperty.PropertyName"/> registered. 
    /// Check this beforehand with <see cref="Contains(string?)"/>!
    /// </exception>
    public void InsertProperty(int index, MappingProperty property)
    {
        _ArgumentNullException.ThrowIfNull(property, nameof(property));
        
        _dynProps.Insert(index, property);
        property.Record = Record;
    }

    /// <summary>
    /// Replaces the <see cref="MappingProperty"/> at the specified index of the list of registered properties 
    /// with <paramref name="property"/>.
    /// </summary>
    /// <param name="index">
    /// The zero-based index at which the registered <see cref="MappingProperty"/> instance is replaced with 
    /// <paramref name="property"/>.</param>
    /// <param name="property">A <see cref="MappingProperty"/> instance.</param>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="property"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> index is less than zero or 
    /// greater than or equal to <see cref="Count"/>.</exception>
    /// <exception cref="ArgumentException">
    /// There is already a <see cref="MappingProperty"/> with the same <see cref="MappingProperty.PropertyName"/> 
    /// registered. 
    /// Check this beforehand with <see cref="Contains(string?)"/>!
    /// </exception>
    public void ReplacePropertyAt(int index, MappingProperty property)
    {
        _ArgumentNullException.ThrowIfNull(property, nameof(property));
        _dynProps[index] = property;
    }

    /// <summary>
    /// Replaces the registered <see cref="MappingProperty"/> instance whose <see cref="MappingProperty.PropertyName"/>
    /// property equals <paramref name="propertyName"/> with <paramref name="property"/>.
    /// </summary>
    /// <param name="propertyName">Identifier of the registered <see cref="MappingProperty"/> instance to be replaced. 
    /// (See <see cref="MappingProperty.PropertyName"/>.)</param>
    /// <param name="property">The <see cref="MappingProperty"/> instance object to be used for replacement.</param>
    ///
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/>, or <paramref name="property"/>
    /// is <c>null</c>.</exception>
    /// 
    /// <exception cref="ArgumentException">
    /// <para>
    /// There is no registered <see cref="MappingProperty"/> whose <see cref="MappingProperty.PropertyName"/> property 
    /// matches <paramref name="propertyName"/>,
    /// </para>
    /// <para>- or -</para>
    /// <para>
    /// there is already a registered <see cref="MappingProperty"/> instance whose <see cref="MappingProperty.PropertyName"/> 
    /// property is identical with which of <paramref name="property"/>.
    /// Check this beforehand with <see cref="Contains(string?)"/>!
    /// </para>
    /// </exception>
    public void ReplaceProperty(string propertyName, MappingProperty property)
    {
        _ArgumentNullException.ThrowIfNull(propertyName, nameof(propertyName));
        _ArgumentNullException.ThrowIfNull(property, nameof(property));
        
        try
        {
            int index = _dynProps.IndexOf(_dynProps[propertyName]);
            _dynProps[index] = property;
        }
        catch (KeyNotFoundException e)
        {
            throw new ArgumentException(string.Format(Res.PropertyNotFound, nameof(propertyName)), nameof(propertyName), e);
        }
    }

    /// <summary>
    /// Examines whether a <see cref="MappingProperty"/> instance is already registered in the <see cref="CsvRecordMapping"/>
    /// under the name that is specified with <paramref name="propertyName"/>.
    /// </summary>
    /// <param name="propertyName">The <see cref="MappingProperty.PropertyName"/> of the <see cref="MappingProperty"/> instance
    /// to be searched for.</param>
    /// <returns><c>true</c>, wenn ein <see cref="MappingProperty"/>-Objekt unter dem mit <paramref name="propertyName"/>
    /// angegebenen Namen registriert ist.</returns>
    public bool Contains(string? propertyName) => propertyName is not null && _dynProps.Contains(propertyName);

    /// <summary>
    /// Gibt den Index der Eigenschaft zurück, die den Eigenschaftsnamen (<see cref="MappingProperty.PropertyName"/>) der 
    /// <paramref name="propertyName"/> entspricht oder -1, wenn eine solche Eigenschaft nicht in <see cref="CsvRecordMapping"/> registriert ist.
    /// </summary>
    /// <param name="propertyName">Der Eigenschaftsname der zu suchenden <see cref="MappingProperty"/>.</param>
    /// <returns>Der Index der Eigenschaft, die <paramref name="propertyName"/> als Eigenschaftsnamen (<see cref="MappingProperty.PropertyName"/>) hat
    /// oder -1, wenn eine solche Eigenschaft nicht in <see cref="CsvRecordMapping"/> registriert ist.</returns>
    public int IndexOf(string? propertyName) => propertyName is null ? -1 : _dynProps.Contains(propertyName) ? _dynProps.IndexOf(_dynProps[propertyName]) : -1;

    /// <summary>
    /// Wird automatisch aufgerufen, wenn einer dynamisch implementierten Eigenschaft ein Wert
    /// zugewiesen wird. (Nicht zur direkten Verwendung in eigenem Code bestimmt.)
    /// </summary>
    /// <param name="binder">Informationen über die aufrufende dynamische Eigenschaft.</param>
    /// <param name="value">Das Objekt, das der dynamisch implementierten Eigenschaft zugewiesen wird.</param>
    /// <returns><c>true</c>, wenn auf eine Eigenschaft zugegriffen wurde, die bereits zuvor als <see cref="MappingProperty"/>
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
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        _ArgumentNullException.ThrowIfNull(binder, nameof(binder));
        
        if (Record is null)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Res.CsvRecordIsNull, nameof(Record)));
        }

        if (this._dynProps.TryGetValue(binder.Name, out MappingProperty? prop))
        {
            prop.SetValue(value);
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
    /// <returns><c>true</c>, wenn auf eine Eigenschaft zugegriffen wurde, die zuvor als <see cref="MappingProperty"/>
    /// im <see cref="CsvRecordMapping"/>-Objekt registriert wurde.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="binder"/> ist <c>null</c>. (Das kann nur passieren,
    /// wenn die Methode direkt aus eigenem Code aufgerufen wird.)</exception>
    /// 
    /// <exception cref="FormatException">
    /// The return value of the accessed <see cref="MappingProperty"/> could not be parsed successfully and the 
    /// type converter of this <see cref="MappingProperty"/> was configured to throw an <see cref="FormatException"/>
    /// in this case.
    /// </exception>
    /// 
    /// <exception cref="InvalidOperationException">No <see cref="CsvRecord"/> instance was assigned to <see cref="Record"/>.</exception>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        _ArgumentNullException.ThrowIfNull(binder, nameof(binder));

        if (Record is null)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Res.CsvRecordIsNull, nameof(Record)));
        }

        if (this._dynProps.TryGetValue(binder.Name, out MappingProperty? prop))
        {
            result = prop.GetValue();
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

    /// <summary>
    /// Returns an <see cref="IEnumerator{T}">IEnumerator&lt;KeyValuePair&lt;string, object?&gt;&gt;</see> with which the return values of 
    /// the dynamically implemented properties are iterated. The order corresponds to the order in which the <see cref="MappingProperty"/> 
    /// instances are registered in the <see cref="CsvRecordMapping"/>.
    /// </summary>
    /// 
    /// <returns>An <see cref="IEnumerator{T}">IEnumerator&lt;KeyValuePair&lt;string, object?&gt;&gt;</see>.</returns>
    /// 
    /// <exception cref="FormatException">
    /// The return value of an accessed <see cref="MappingProperty"/> could not be parsed successfully and the 
    /// type converter of this <see cref="MappingProperty"/> was configured to throw an <see cref="FormatException"/>
    /// in this case.
    /// </exception>
    /// 
    /// <exception cref="InvalidOperationException">No <see cref="CsvRecord"/> instance was assigned to <see cref="Record"/>.</exception>
    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
    {
        if (Record is null)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Res.CsvRecordIsNull, nameof(Record)));
        }

        foreach (MappingProperty? prop in this._dynProps)
        {
            yield return new KeyValuePair<string, object?>(prop.PropertyName, prop.GetValue());
        }
    }

    /// <summary>
    /// Returns an <see cref="IEnumerator"/> with which the return values of 
    /// the dynamically implemented properties are iterated. The order corresponds to the order in which the <see cref="MappingProperty"/> 
    /// instances are registered in the <see cref="CsvRecordMapping"/>.
    /// </summary>
    /// <returns>An <see cref="IEnumerator"/>.</returns>
    /// 
    /// <exception cref="FormatException">
    /// The return value of an accessed <see cref="MappingProperty"/> could not be parsed successfully and the 
    /// type converter of this <see cref="MappingProperty"/> was configured to throw an <see cref="FormatException"/>
    /// in this case.
    /// </exception>
    /// 
    /// <exception cref="InvalidOperationException">No <see cref="CsvRecord"/> instance was assigned to <see cref="Record"/>.</exception>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

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
