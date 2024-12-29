using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using FolkerKinzel.CsvTools.TypeConversions.Converters.Intls;
using FolkerKinzel.CsvTools.TypeConversions.Intls.Extensions;
using FolkerKinzel.CsvTools.TypeConversions.Resources;

namespace FolkerKinzel.CsvTools.TypeConversions;

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
/// After a <see cref="CsvRecordMapping"/> object has been initialized, the method <see cref="AddProperty(CsvPropertyBase)"/> has to be called to 
/// register <see cref="CsvPropertyBase"/> 
/// objects, which are dynamic properties of the <see cref="CsvRecordMapping"/> object and perform type conversions for the string data in the underlying 
/// <see cref="CsvRecord"/> object. The order in which the properties are registered determines the order, in which the return values ​​of these properties 
/// are returned if the <see cref="CsvRecordMapping"/> object is iterated in a <c>foreach</c> loop, or if it is accessed with the indexer <see cref="this[int]"/>. 
/// </para>
/// <para>
/// Because of the <see cref="CsvColumnNameProperty"/> objects access the underlying <see cref="CsvRecord"/> object via the CSV column name, the number and 
/// order of the <see cref="CsvColumnNameProperty"/> objects registered 
/// in the <see cref="CsvRecordMapping"/> object don't have to match the column-order of the underlying <see cref="CsvRecord"/> object. 
/// The same is for <see cref="CsvColumnIndexProperty"/> objects: The number and order of the registered <see cref="CsvColumnIndexProperty"/> objects
/// is independent of the columns of the underlying <see cref="CsvRecord"/> object because the <see cref="CsvColumnIndexProperty.DesiredCsvColumnIndex"/> 
/// is stored inside of the <see cref="CsvColumnIndexProperty"/> instances."/>
/// </para>
/// <para>The order of the registered properties can be influenced at any time with the following methods:</para>
/// <list type="bullet">
/// <item><see cref="InsertProperty(int, CsvPropertyBase)"/></item>
/// <item><see cref="ReplaceProperty(string, CsvPropertyBase)"/></item>
/// <item><see cref="ReplacePropertyAt(int, CsvPropertyBase)"/></item>
/// <item><see cref="RemoveProperty(string)"/></item>
/// <item><see cref="RemovePropertyAt(int)"/></item>
/// </list>
/// With <see cref="Contains(string)"/> you can check whether a <see cref="CsvPropertyBase"/> instance with the specified 
/// <see cref="CsvPropertyBase.PropertyName"/> is already registered. 
/// <para>
/// The indexers <see cref="this[int]"/>, and <see cref="this[string]"/> can also be accessed if the <see cref="CsvRecordMapping"/>
/// instance is assigned to a normal variable, but the the values returned by the indexers possibly have to be cast into the actual 
/// <see cref="Type"/> using the cast operator since they are of the type <see cref="object"/>.
/// </para>
/// <para>
/// If the <see cref="CsvRecordMapping"/> instance is assigned to a variable that has been declared with the keyword <c>dynamic</c>, 
/// the runtime takes over the necessary casts. In addition, it is then possible to address the registered <see cref="CsvPropertyBase"/> 
/// objects like normal .NET properties by their name (<see cref="CsvPropertyBase.PropertyName"/>). The disadvantage is that Visual Studio cannot 
/// offer any "IntelliSense" on dynamic variables.
/// </para>
/// Normally it is sufficient to assign the <see cref="CsvRecord"/> instance to the <see cref="CsvRecordMapping"/> object only once, because 
/// the same <see cref="CsvRecord"/> instance is always used for normal reading and writing. An exception is when a CSV file is read with the 
/// <see cref="CsvOpts.EnableCaching"/> option: then the current <see cref="CsvRecord"/> object must be assigned for each iteration.
/// <para>
/// If <see cref="CsvMultiColumnProperty"/> objects are inserted into <see cref="CsvRecordMapping"/>, the <see cref="CsvRecordMapping"/> instances 
/// of their <see cref="CsvMultiColumnTypeConverter"/> will automatically get the current <see cref="CsvRecord"/> instance via the parent 
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
    private class PropertyCollection : KeyedCollection<string, CsvPropertyBase>
    {
        protected override string GetKeyForItem(CsvPropertyBase item) => item.PropertyName;

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
    /// Gets the number of <see cref="CsvPropertyBase"/> instances, which are registered in the 
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
    /// to the order in which the <see cref="CsvPropertyBase"/> objects are registered in the <see cref="CsvRecordMapping"/>.
    /// </summary>
    /// <param name="index">Zero-based index of the registered <see cref="CsvPropertyBase"/> instance.</param>
    /// <returns>Return value of the <see cref="CsvPropertyBase"/> registered at <paramref name="index"/>.</returns>
    /// 
    /// <exception cref="InvalidOperationException">No <see cref="CsvRecord"/> instance was assigned to <see cref="Record"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <para></para>
    /// <paramref name="index"/> ist kleiner als 0 oder größer oder gleich <see cref="Count"/>.</exception>
    /// <exception cref="InvalidCastException">
    /// <para>
    /// Einem Index wurde ein Objekt zugewiesen, dessen Datentyp nicht dem Datentyp der an
    /// diesem Index registrierten Property entspricht
    /// </para>
    /// <para>- oder -</para>
    /// <para>
    /// der Rückgabewert der indexierten <see cref="CsvPropertyBase"/> konnte nicht erfolgreich geparst werden und 
    /// der <see cref="ICsvTypeConverter"/> dieser <see cref="CsvPropertyBase"/> war so konfiguriert, dass er in diesem Fall eine
    /// Ausnahme wirft.
    /// </para></exception>
    /// <exception cref="InvalidOperationException">Es wurde versucht, auf die Daten von <see cref="CsvRecordMapping"/> zuzugreifen, ohne dass diesem
    /// ein <see cref="CsvRecord"/>-Objekt zugewiesen war.</exception>
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
    /// Ermöglicht den Zugriff auf die im <see cref="CsvRecordMapping"/> registrierten Eigenschaften
    /// über den Wert der Eigenschaft <see cref="CsvPropertyBase.PropertyName"/>.
    /// </summary>
    /// <param name="propertyName">Name der registrierten Eigenschaft. (Entspricht <see cref="CsvPropertyBase.PropertyName"/>.) Der
    /// Vergleich erfolgt case-sensitiv.</param>
    /// <returns>Rückgabewert der registrierten <see cref="CsvPropertyBase"/>, deren Eigenschaft <see cref="CsvPropertyBase.PropertyName"/>&#160;<paramref name="propertyName"/>
    /// entspricht. Der Vergleich ist case-sensitiv.</returns>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> ist <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Es wurde unter den bei <see cref="CsvRecordMapping"/> registrierten <see cref="CsvPropertyBase"/>-Eigenschaften kein
    /// <see cref="CsvPropertyBase"/>-Objekt gefunden, dessen Eigenschaft <see cref="CsvPropertyBase.PropertyName"/>&#160;<paramref name="propertyName"/> entspricht.</exception>
    /// <exception cref="InvalidCastException">
    /// <para>
    /// Einem Index wurde ein Objekt zugewiesen, dessen Datentyp nicht dem Datentyp der an
    /// diesem Index registrierten Property entspricht
    /// </para>
    /// <para>- oder -</para>
    /// <para>
    /// der Rückgabewert der indexierten <see cref="CsvPropertyBase"/> konnte nicht erfolgreich geparst werden und 
    /// der <see cref="ICsvTypeConverter"/> dieser <see cref="CsvPropertyBase"/> war so konfiguriert, dass er in diesem Fall eine
    /// Ausnahme wirft.
    /// </para></exception>
    /// <exception cref="InvalidOperationException">No <see cref="CsvRecord"/> instance was assigned to <see cref="Record"/>.</exception>
    public object? this[string propertyName]
    {
        get
        {
            return Record is null
                ? throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Res.CsvRecordIsNull, nameof(Record)))
                : propertyName is null
                    ? throw new ArgumentNullException(nameof(propertyName))
                    : this._dynProps.TryGetValue(propertyName, out CsvPropertyBase? prop)
                        ? prop.GetValue()
                        : throw new ArgumentException(string.Format(Res.PropertyNotFound, nameof(propertyName)), nameof(propertyName));
        }

        set
        {
            if (Record is null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Res.CsvRecordIsNull, nameof(Record)));
            }

            if (propertyName is null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }


            if (this._dynProps.TryGetValue(propertyName, out CsvPropertyBase? prop))
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
    /// Registriert eine <see cref="CsvPropertyBase"/> am Ende der Auflistung der registrierten Eigenschaften.
    /// </summary>
    /// <param name="property">Die anzufügende <see cref="CsvColumnNameProperty"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="property"/> ist <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Es ist bereits eine <see cref="CsvPropertyBase"/> mit demselben
    /// <see cref="CsvPropertyBase.PropertyName"/> enthalten. Prüfen Sie das vorher mit <see cref="Contains(string)"/>!</exception>
    public void AddProperty(CsvPropertyBase property)
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        this._dynProps.Add(property);
        property.Record = Record;
    }

    /// <summary>
    /// Entfernt die <see cref="CsvPropertyBase"/> mit dem angegebenen <see cref="CsvPropertyBase.PropertyName"/>
    /// aus der Auflistung der registrierten Eigenschaften.
    /// </summary>
    /// <param name="propertyName">Der <see cref="CsvPropertyBase.PropertyName"/> der zu entfernenden
    /// <see cref="CsvPropertyBase"/>.</param>
    /// <returns><c>true</c>, wenn die gesuchte <see cref="CsvPropertyBase"/> in der Auflistung enthalten war
    /// und entfernt werden konnte.</returns>
    public bool RemoveProperty(string? propertyName)
        => propertyName is not null && _dynProps.Remove(propertyName);

    /// <summary>
    /// Entfernt die <see cref="CsvPropertyBase"/> am angegebenen <paramref name="index"/> aus der
    /// Auflistung der registrierten Eigenschaften.
    /// </summary>
    /// <param name="index">Der nullbasierte Index, an dem die <see cref="CsvPropertyBase"/> entfernt werden soll.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> ist kleiner als 0 oder 
    /// größer oder gleich <see cref="Count"/>.</exception>
    public void RemovePropertyAt(int index)
    => _dynProps.RemoveAt(index);

    /// <summary>
    /// Fügt <paramref name="property"/> am <paramref name="index"/> der registrierten Eigenschaften ein.
    /// </summary>
    /// <param name="index">Der nullbasierte Index, an dem <paramref name="property"/> eingefügt werden soll.</param>
    /// <param name="property">Die einzufügende <see cref="CsvColumnNameProperty"/>.</param>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="property"/> ist <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> ist kleiner als 0 oder 
    /// größer als <see cref="Count"/>.</exception>
    /// <exception cref="ArgumentException">Es ist bereits eine <see cref="CsvPropertyBase"/> mit demselben
    /// <see cref="CsvPropertyBase.PropertyName"/> enthalten. Prüfen Sie das vorher mit <see cref="Contains(string)"/>!</exception>
    public void InsertProperty(int index, CsvPropertyBase property)
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        _dynProps.Insert(index, property);
        property.Record = Record;
    }

    /// <summary>
    /// Ersetzt die <see cref="CsvPropertyBase"/> am angegebenen Index der Auflistung der registrierten Eigenschaften
    /// durch <paramref name="property"/>.
    /// </summary>
    /// <param name="index">Der nullbasierte Index, an dem das in der Auflistung vorhandene
    /// <see cref="CsvPropertyBase"/>-Objekt durch <paramref name="property"/> ersetzt wird.</param>
    /// <param name="property">Ein <see cref="CsvPropertyBase"/>-Objekt.</param>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="property"/> ist <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> ist kleiner als 0 oder 
    /// größer oder gleich <see cref="Count"/>.</exception>
    /// <exception cref="ArgumentException">Es ist bereits eine <see cref="CsvPropertyBase"/> mit demselben
    /// <see cref="CsvPropertyBase.PropertyName"/> enthalten. Prüfen Sie das vorher mit <see cref="Contains(string)"/>!</exception>
    public void ReplacePropertyAt(int index, CsvPropertyBase property)
    {
        _ArgumentNullException.ThrowIfNull(property, nameof(property));
        _dynProps[index] = property;
    }

    /// <summary>
    /// Ersetzt die registrierte Eigenschaft, deren <see cref="CsvPropertyBase.PropertyName"/> gleich <paramref name="propertyName"/>
    /// ist durch <paramref name="property"/>.
    /// </summary>
    ///  <param name="propertyName">Der <see cref="CsvPropertyBase.PropertyName"/> der zu ersetzenden
    /// <see cref="CsvPropertyBase"/>.</param>
    /// <param name="property"><see cref="CsvPropertyBase"/>-Objekt, mit dem ersetzt werden soll.</param>
    ///
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> oder <paramref name="property"/>
    /// ist <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Es ist keine Eigenschaft unter der Bezeichnung <paramref name="propertyName"/>
    /// registriert - oder - in der Auflistung der registrierten Eigenschaften befindet sich bereits ein 
    /// <see cref="CsvPropertyBase"/>-Objekt dessen <see cref="CsvPropertyBase.PropertyName"/>-Eigenschaft identisch
    /// mit der von <paramref name="property"/> ist. Prüfen Sie das vorher mit <see cref="Contains(string)"/>!</exception>
    public void ReplaceProperty(string propertyName, CsvPropertyBase property)
    {
        if (propertyName is null)
        {
            throw new ArgumentNullException(nameof(propertyName));
        }

        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

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
    /// Untersucht, ob ein <see cref="CsvPropertyBase"/>-Objekt im <see cref="CsvRecordMapping"/> unter dem mit
    /// <paramref name="propertyName"/> angegebenen Namen registriert ist.
    /// </summary>
    /// <param name="propertyName">Der <see cref="CsvPropertyBase.PropertyName"/> der zu suchenden
    /// <see cref="CsvPropertyBase"/>.</param>
    /// <returns><c>true</c>, wenn ein <see cref="CsvColumnNameProperty"/>-Objekt unter dem mit <paramref name="propertyName"/>
    /// angegebenen Namen registriert ist.</returns>
    public bool Contains(string? propertyName) => propertyName is not null && _dynProps.Contains(propertyName);

    /// <summary>
    /// Gibt den Index der Eigenschaft zurück, die den Eigenschaftsnamen (<see cref="CsvPropertyBase.PropertyName"/>) der 
    /// <paramref name="propertyName"/> entspricht oder -1, wenn eine solche Eigenschaft nicht in <see cref="CsvRecordMapping"/> registriert ist.
    /// </summary>
    /// <param name="propertyName">Der Eigenschaftsname der zu suchenden <see cref="CsvColumnNameProperty"/>.</param>
    /// <returns>Der Index der Eigenschaft, die <paramref name="propertyName"/> als Eigenschaftsnamen (<see cref="CsvPropertyBase.PropertyName"/>) hat
    /// oder -1, wenn eine solche Eigenschaft nicht in <see cref="CsvRecordMapping"/> registriert ist.</returns>
    public int IndexOf(string? propertyName) => propertyName is null ? -1 : _dynProps.Contains(propertyName) ? _dynProps.IndexOf(_dynProps[propertyName]) : -1;

    /// <summary>
    /// Wird automatisch aufgerufen, wenn einer dynamisch implementierten Eigenschaft ein Wert
    /// zugewiesen wird. (Nicht zur direkten Verwendung in eigenem Code bestimmt.)
    /// </summary>
    /// <param name="binder">Informationen über die aufrufende dynamische Eigenschaft.</param>
    /// <param name="value">Das Objekt, das der dynamisch implementierten Eigenschaft zugewiesen wird.</param>
    /// <returns><c>true</c>, wenn auf eine Eigenschaft zugegriffen wurde, die zuvor als <see cref="CsvColumnNameProperty"/>
    /// im <see cref="CsvRecordMapping"/>-Objekt registriert wurde.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="binder"/> ist <c>null</c>. (Das kann nur passieren,
    /// wenn die Methode direkt aus eigenem Code aufgerufen wird.)</exception>
    /// <exception cref="InvalidCastException"><paramref name="value"/> ist nicht vom Datentyp der registrierten
    /// Property.</exception>
    /// <exception cref="Exception">Es wurde versucht, auf eine nicht registrierte Property zuzugreifen.</exception>
    /// 
    /// <exception cref="InvalidOperationException">No <see cref="CsvRecord"/> instance was assigned to <see cref="Record"/>.</exception>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        if (binder is null)
        {
            throw new ArgumentNullException(nameof(binder));
        }

        if (Record is null)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Res.CsvRecordIsNull, nameof(Record)));
        }

        if (this._dynProps.TryGetValue(binder.Name, out CsvPropertyBase? prop))
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
    /// <returns><c>true</c>, wenn auf eine Eigenschaft zugegriffen wurde, die zuvor als <see cref="CsvPropertyBase"/>
    /// im <see cref="CsvRecordMapping"/>-Objekt registriert wurde.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="binder"/> ist <c>null</c>. (Das kann nur passieren,
    /// wenn die Methode direkt aus eigenem Code aufgerufen wird.)</exception>
    /// <exception cref="Exception">Der Rückgabewert der indexierten <see cref="CsvPropertyBase"/> konnte nicht erfolgreich geparst werden und 
    /// der <see cref="ICsvTypeConverter"/> dieser <see cref="CsvPropertyBase"/> war so konfiguriert, dass er in diesem Fall eine
    /// Ausnahme wirft.</exception>
    /// 
    /// <exception cref="InvalidOperationException">No <see cref="CsvRecord"/> instance was assigned to <see cref="Record"/>.</exception>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        if (binder is null)
        {
            throw new ArgumentNullException(nameof(binder));
        }

        if (Record is null)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Res.CsvRecordIsNull, nameof(Record)));
        }

        if (this._dynProps.TryGetValue(binder.Name, out CsvPropertyBase? prop))
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
    /// Gibt einen <see cref="IEnumerator{T}">IEnumerator&lt;KeyValuePair&lt;string, object?&gt;&gt;</see> zurück, mit dem die Rückgabewerte der 
    /// dynamisch implementierten Eigenschaften durchlaufen werden. Die Reihenfolge
    /// entspricht der Reihenfolge, in der die
    /// <see cref="CsvPropertyBase"/>-Objekte im <see cref="CsvRecordMapping"/> registriert sind.
    /// </summary>
    /// <returns>Ein <see cref="IEnumerator{T}">IEnumerator&lt;KeyValuePair&lt;string, object?&gt;&gt;</see>.</returns>
    /// <exception cref="InvalidCastException">Der Rückgabewert einer indexierten <see cref="CsvPropertyBase"/> konnte nicht erfolgreich geparst werden und 
    /// der <see cref="ICsvTypeConverter"/> dieser <see cref="CsvPropertyBase"/> war so konfiguriert, dass er in diesem Fall eine
    /// Ausnahme wirft.</exception>
    /// 
    /// <exception cref="InvalidOperationException">No <see cref="CsvRecord"/> instance was assigned to <see cref="Record"/>.</exception>
    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
    {
        if (Record is null)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Res.CsvRecordIsNull, nameof(Record)));
        }


        foreach (CsvPropertyBase? prop in this._dynProps)
        {
            yield return new KeyValuePair<string, object?>(prop.PropertyName, prop.GetValue());
        }
    }

    /// <summary>
    /// Gibt einen <see cref="IEnumerator"/> zurück, mit dem die Rückgabewerte der 
    /// dynamisch implementierten Eigenschaften durchlaufen werden können. Die Reihenfolge
    /// entspricht der Reihenfolge, in der die
    /// <see cref="CsvPropertyBase"/>-Objekte im <see cref="CsvRecordMapping"/> registriert sind.
    /// </summary>
    /// <returns>Ein <see cref="IEnumerator"/>.</returns>
    /// <exception cref="InvalidCastException">Der Rückgabewert einer indexierten <see cref="CsvPropertyBase"/> konnte nicht erfolgreich geparst werden und 
    /// der <see cref="ICsvTypeConverter"/> dieser <see cref="CsvPropertyBase"/> war so konfiguriert, dass er in diesem Fall eine
    /// Ausnahme wirft.</exception>
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
