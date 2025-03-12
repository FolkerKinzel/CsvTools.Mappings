using FolkerKinzel.CsvTools.Mappings.Intls;
using FolkerKinzel.Helpers.Polyfills;
using System.Collections;

namespace FolkerKinzel.CsvTools.Mappings;

/// <summary>Provides read-only forward access to the data of a CSV file.</summary>
/// <typeparam name="TResult">
/// Generic type parameter for the data type to which the contents of each row of the 
/// CSV will be converted.
/// </typeparam>
/// 
/// <remarks>
/// <para>
/// The class implements <see cref="IEnumerable{T}"/>. A 
/// <see cref="CsvReader{TResult}"/> instance can be iterated with <c>foreach</c> or
/// queried using Linq methods. Note that an instance can only be iterated once; if an 
/// attempt is made to iterate it twice, an <see cref="ObjectDisposedException"/> is 
/// thrown.
/// </para>
/// <note type="tip">
/// Use the methods of <see cref="CsvConverter"/> to create an instance!
/// </note>
/// </remarks>
public sealed class CsvReader<TResult> : IEnumerable<TResult>, IEnumerator<TResult>
{
    private readonly CsvReader _reader;
    private readonly bool _cloneMapping;
    private readonly CsvTo<TResult> _converter;
    private TResult? _current;
    private bool _disposed;

    /// <summary>
    /// Initializes a new <see cref="CsvReader{TResult}"/> instance.
    /// </summary>
    /// <param name="reader">A <see cref="CsvReader"/> instance.</param>
    /// <param name="mapping">The <see cref="CsvMapping"/> used to convert the CSV data.</param>
    /// <param name="conversion">
    /// <para>
    /// A function that converts the content of <paramref name="mapping"/>
    /// to an instance of <typeparamref name="TResult"/>. 
    /// </para>
    /// <para>
    /// The function is called for each row in the CSV data and gets the specified 
    /// <see cref="CsvMapping"/> as argument, filled with the current <see cref="CsvRecord"/> 
    /// instance. The <see cref="CsvMapping"/> is passed to the function as <c>dynamic</c> 
    /// argument: Inside the function the registered <see cref="DynamicProperty"/> instances
    /// can be used like regular .NET properties, but without IntelliSense ("late binding").
    /// </para>
    /// </param>
    /// <param name="cloneMapping"><c>true</c> to clone <paramref name="mapping"/> each time 
    /// before passing it to <paramref name="conversion"/>, or <c>false</c> to use always the 
    /// same <see cref="CsvMapping"/> instance. (Cloning is required if 
    /// <typeparamref name="TResult"/> is <see cref="CsvMapping"/> and the results need to be 
    /// cached.)</param>
    /// 
    /// <remarks>
    /// <note type="tip">
    /// It's recommended to use the methods of <see cref="CsvConverter"/> to create an instance.
    /// </note>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException">
    /// <paramref name="reader"/>, or <paramref name="mapping"/>, or <paramref name="conversion"/>
    /// is <c>null</c>.
    /// </exception>
    public CsvReader(CsvReader reader,
                     CsvMapping mapping,
                     Func<dynamic, TResult> conversion,
                     bool cloneMapping = true)
    {
        _ArgumentNullException.ThrowIfNull(reader, nameof(reader));

        _reader = reader;
        _cloneMapping = cloneMapping;
        _converter = new CsvToIntl<TResult>(mapping, conversion);
    }

    public CsvReader(CsvReader reader,
                     CsvTo<TResult> converter,
                     bool cloneMapping = true)
    {
        _ArgumentNullException.ThrowIfNull(reader, nameof(reader));
        _ArgumentNullException.ThrowIfNull(converter, nameof(converter));

        _reader = reader;
        _cloneMapping = cloneMapping;
        _converter = converter;
    }

    /// <inheritdoc/>
    TResult IEnumerator<TResult>.Current => _current!;

    /// <inheritdoc/>
    object? IEnumerator.Current => ((IEnumerator<TResult>)this).Current;

    /// <inheritdoc/>
    /// 
    /// <exception cref="ObjectDisposedException">The CSV file was already
    /// closed.</exception>
    /// <exception cref="IOException">I/O error.</exception>
    /// <exception cref="CsvFormatException">Invalid CSV file. The interpretation 
    /// depends on the <see cref="CsvOpts" /> value, specified in the constructor 
    /// of the <see cref="CsvReader"/>.
    /// </exception>
    /// <exception cref="FormatException">
    /// Parsing fails and the <see cref="TypeConverters.TypeConverter{T}.Throwing"/>
    /// property of that <see cref="TypeConverters.TypeConverter{T}"/> is <c>true</c>.
    /// </exception>
    bool IEnumerator.MoveNext() => TryRead(out _current);

    /// <summary>
    /// Throws a <see cref="NotSupportedException"/>.
    /// </summary>
    /// <exception cref="NotSupportedException">The method has been called.</exception>
    void IEnumerator.Reset() => throw new NotSupportedException();

    /// <inheritdoc/>
    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            _reader.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    /// <inheritdoc/>
    public IEnumerator<TResult> GetEnumerator() => this;

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>Tries to read the next <typeparamref name="TResult"/> from the 
    /// CSV file.
    /// </summary>
    /// <param name="result">After the method returns <c>true</c>, contains
    /// the next <typeparamref name="TResult"/> parsed from the CSV file, 
    /// otherwise <c>default</c>.</param>
    /// <returns><c>true</c> if a new <typeparamref name="TResult"/> instance 
    /// could be parsed from the CSV data, otherwise <c>false</c>.</returns>
    /// 
    /// <exception cref="ObjectDisposedException">The CSV file was already
    /// closed.</exception>
    /// <exception cref="IOException">I/O error.</exception>
    /// <exception cref="CsvFormatException">Invalid CSV file. The interpretation 
    /// depends on the <see cref="CsvOpts" /> value, specified in the constructor 
    /// of the <see cref="CsvReader"/>.
    /// </exception>
    /// <exception cref="FormatException">
    /// Parsing fails and the <see cref="TypeConverters.TypeConverter{T}.Throwing"/>
    /// property of that <see cref="TypeConverters.TypeConverter{T}"/> is <c>true</c>.
    /// </exception>
    public bool TryRead(out TResult result)
    {
        CsvRecord? record = _reader.Read();

        if (record is null)
        {
            Dispose();
            result = default!;
            return false;
        }

        CsvMapping clone = _cloneMapping ? (CsvMapping)_converter.Mapping.Clone() : _converter.Mapping;
        clone.Record = record;
        result = _converter.Convert(clone);
        return true;
    }
}
