using FolkerKinzel.CsvTools.Mappings.Intls;
using System;
using System.Collections;

namespace FolkerKinzel.CsvTools.Mappings;

/// <summary>Provides read-only forward access to the data of a CSV file.</summary>
/// 
/// <typeparam name="TData">
/// Generic type parameter for the data type that the <see cref="CsvWriter{TData}"/> 
/// can write as CSV row.
/// </typeparam>
public sealed class CsvWriter<TData> : IDisposable
{
    private readonly CsvWriter _writer;
    private readonly Mapping _mapping;
    private readonly Action<TData, dynamic> _conversion;
  
    private bool _disposed;

    /// <summary>
    /// Initializes a new <see cref="CsvWriter{TResult}"/> instance.
    /// </summary>
    /// <param name="writer">A <see cref="CsvWriter"/> instance.</param>
    /// <param name="mapping">The <see cref="Mapping"/> used to convert 
    /// <typeparamref name="TData"/> to CSV.</param>
    /// <param name="conversion">
    /// <para>
    /// A method that fills the content of a <typeparamref name="TData"/> instance
    /// into the properties of <paramref name="mapping"/>. 
    /// </para>
    /// <para>
    /// <paramref name="conversion"/> is called with each call to <see cref="Write(TData)"/> and it
    /// gets the <typeparamref name="TData"/> instance and <paramref name="mapping"/> as
    /// arguments. The <see cref="Mapping"/>
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
                       Mapping mapping,
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
    /// <param name="data">The <typeparamref name="TData"/> instance to be written. If <paramref name="data"/> 
    /// is <c>null</c>, nothing is written.</param>
    /// <exception cref="IOException">I/O error.</exception>
    /// <exception cref="ObjectDisposedException">The file was already closed.</exception>
    public void Write(TData? data)
    {
        if(data is null)
        {
            return;
        }

        _conversion(data, _mapping);
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
