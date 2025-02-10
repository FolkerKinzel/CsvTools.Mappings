using FolkerKinzel.CsvTools.Mappings.Intls;

namespace FolkerKinzel.CsvTools.Mappings;

/// <summary>Writes data of any type as CSV (RFC 4180).</summary>
/// 
/// <typeparam name="TData">
/// Generic type parameter for the data type that the <see cref="CsvWriter{TData}"/> 
/// can write as CSV row.
/// </typeparam>
public sealed class CsvWriter<TData> : IDisposable
{
    private readonly CsvWriter _writer;
    private readonly CsvMapping _mapping;
    private readonly Action<TData, dynamic> _conversion;

    private bool _disposed;

    /// <summary>
    /// Initializes a new <see cref="CsvWriter{TResult}"/> instance.
    /// </summary>
    /// <param name="writer">A <see cref="CsvWriter"/> instance.</param>
    /// <param name="mapping">The <see cref="CsvMapping"/> used to convert 
    /// <typeparamref name="TData"/> to CSV.</param>
    /// <param name="conversion">
    /// <para>
    /// A method that fills the content of a <typeparamref name="TData"/> instance
    /// into the properties of <paramref name="mapping"/>. 
    /// </para>
    /// <para>
    /// <paramref name="conversion"/> is called with each call to <see cref="Write(TData)"/> and it
    /// gets the <typeparamref name="TData"/> instance and <paramref name="mapping"/> as
    /// arguments. The <see cref="CsvMapping"/>
    /// is passed to the method as <c>dynamic</c> argument: Inside the method the registered 
    /// <see cref="DynamicProperty"/> instances can be used like 
    /// regular .NET properties, but without IntelliSense ("late binding").
    /// </para>
    /// <para>
    /// With each call all <see cref="DynamicProperty"/> instances in
    /// <paramref name="mapping"/> have been reset to their <see cref="DynamicProperty.DefaultValue"/>.
    /// </para>
    /// </param>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="writer"/>, or <paramref name="mapping"/>, 
    /// or <paramref name="conversion"/> is <c>null</c>.</exception>
    public CsvWriter(CsvWriter writer,
                     CsvMapping mapping,
                     Action<TData, dynamic> conversion)
    {
        _ArgumentNullException.ThrowIfNull(writer, nameof(writer));
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));
        _ArgumentNullException.ThrowIfNull(conversion, nameof(conversion));

        _writer = writer;
        _mapping = mapping;
        _mapping.Record = _writer.Record;
        _conversion = conversion;
    }

    /// <summary>
    /// Writes <paramref name="data"/> as a new CSV row.
    /// </summary>
    /// <param name="data">The <typeparamref name="TData"/> instance to be written or <c>null</c>.</param>
    /// <exception cref="IOException">I/O error.</exception>
    /// <exception cref="ObjectDisposedException">The file was already closed.</exception>
    public void Write(TData? data)
    {
        if (data is not null)
        {
            _conversion(data, _mapping); 
        }
        
        _writer.WriteRecord();
    }

    /// <summary>
    /// Gets the field separator character.
    /// </summary>
    public char Delimiter => _writer.Delimiter;

    /// <inheritdoc/>
    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            _writer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
