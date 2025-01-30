using FolkerKinzel.CsvTools.Mappings.Intls;
using System.Collections;

namespace FolkerKinzel.CsvTools.Mappings;

public sealed class CsvMappingReader<TResult> : IEnumerable<TResult>, IEnumerator<TResult>
{
    private readonly CsvReader _reader;
    private readonly Func<dynamic, TResult> _converter;
    private TResult? _current;

    public CsvMappingReader(CsvReader reader, Func<dynamic, TResult> converter)
    {
        _ArgumentNullException.ThrowIfNull(reader, nameof(reader));
        _reader = reader;
        _converter = converter;
    }

    /// <inheritdoc/>
    public TResult Current => _current!;

    /// <inheritdoc/>
    object? IEnumerator.Current => ((IEnumerator<TResult>)this).Current;

    /// <inheritdoc/>
    public bool MoveNext()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Throws a <see cref="NotSupportedException"/>.
    /// </summary>
    /// <exception cref="NotSupportedException">The method has been called.</exception>
    public void Reset() => throw new NotSupportedException();

    /// <inheritdoc/>
    public void Dispose()
    {
        _reader.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public IEnumerator<TResult> GetEnumerator() => this;
    
    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
}
