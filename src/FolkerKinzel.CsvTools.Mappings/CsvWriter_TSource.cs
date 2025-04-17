using FolkerKinzel.CsvTools.Mappings.Intls;
using FolkerKinzel.Helpers.Polyfills;

namespace FolkerKinzel.CsvTools.Mappings;

/// <summary>Writes data of any type as CSV (RFC 4180).</summary>
/// 
/// <typeparam name="TSource">
/// Generic type parameter for the data type that the 
/// <see cref="CsvWriter{TSource}"/> can write as CSV row.
/// </typeparam>
public sealed class CsvWriter<TSource> : IDisposable
{
    private readonly CsvWriter _writer;
    private readonly CsvFrom<TSource> _converter;
    private bool _disposed;

    /// <summary>
    /// Initializes a new <see cref="CsvWriter{TResult}"/> instance.
    /// </summary>
    /// <param name="writer">A <see cref="CsvWriter"/> instance.</param>
    /// <param name="mapping">The <see cref="CsvMapping"/> used to 
    /// convert <typeparamref name="TSource"/> to CSV.</param>
    /// <param name="conversion">
    /// <para>
    /// A method that fills the content of a <typeparamref name="TSource"/>
    /// instance into the properties of <paramref name="mapping"/>. 
    /// </para>
    /// <para>
    /// <paramref name="conversion"/> is called with each call to <see cref="Write(TSource)"/>
    /// and it gets the <typeparamref name="TSource"/> instance and <paramref name="mapping"/>
    /// as arguments. The <see cref="CsvMapping"/> is passed to the method as <c>dynamic</c>
    /// argument: Inside the method the registered <see cref="DynamicProperty"/> instances 
    /// can be used like regular .NET properties, but without IntelliSense ("late binding").
    /// </para>
    /// <para>
    /// With each call all <see cref="DynamicProperty"/> instances in
    /// <paramref name="mapping"/> have been reset to their <see cref="DynamicProperty.DefaultValue"/>.
    /// </para>
    /// </param>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="writer"/>, or 
    /// <paramref name="mapping"/>, or <paramref name="conversion"/> is <c>null</c>.</exception>
    public CsvWriter(CsvWriter writer,
                     CsvMapping mapping,
                     Action<TSource, dynamic> conversion)
    {
        _ArgumentNullException.ThrowIfNull(writer, nameof(writer));

        _writer = writer;
        _converter = new CsvFromIntl<TSource>(mapping, conversion);
        _converter.Mapping.Record = _writer.Record;
    }

    /// <summary>
    /// Initializes a new <see cref="CsvWriter{TResult}"/> instance.
    /// </summary>
    /// <param name="writer">A <see cref="CsvWriter"/> instance.</param>
    /// 
    /// <param name="converter">
    /// An object that converts a <typeparamref name="TSource"/> instance to a 
    /// CSV row.
    /// </param>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="writer"/>, or 
    /// <paramref name="converter"/> is <c>null</c>.</exception>
    public CsvWriter(CsvWriter writer,
                     CsvFrom<TSource> converter)
    {
        _ArgumentNullException.ThrowIfNull(writer, nameof(writer));
        _ArgumentNullException.ThrowIfNull(converter, nameof(converter));

        _writer = writer;
        _converter = converter;
        _converter.Mapping.Record = _writer.Record;
    }

    /// <summary>
    /// Writes <paramref name="data"/> as a new CSV row.
    /// </summary>
    /// <param name="data">The <typeparamref name="TSource"/> instance to be written or 
    /// <c>null</c>.</param>
    /// <exception cref="IOException">I/O error.</exception>
    /// <exception cref="ObjectDisposedException">The file was already closed.</exception>
    public void Write(TSource? data)
    {
        if (data is not null)
        {
            _converter.FillMapping(data); 
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
