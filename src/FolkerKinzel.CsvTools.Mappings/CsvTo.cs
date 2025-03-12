using FolkerKinzel.Helpers.Polyfills;

namespace FolkerKinzel.CsvTools.Mappings;

/// <summary>
/// Abstract base class for classes that convert a CSV row to a
/// <typeparamref name="TResult"/> instance.
/// </summary>
/// <typeparam name="TResult">The data type to initialize.</typeparam>
public abstract class CsvTo<TResult>
{
    /// <summary>
    /// Constructor used by derived classes.
    /// </summary>
    /// <param name="mapping">The <see cref="CsvMapping"/> to use for 
    /// reading CSV values.</param>
    /// <exception cref="ArgumentNullException"><paramref name="mapping"/> is <c>null</c>.</exception>
    protected CsvTo(CsvMapping mapping)
    {
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));

        Mapping = mapping;
    }

    /// <summary>
    /// The <see cref="CsvMapping"/> to use for reading CSV values.
    /// </summary>
    public CsvMapping Mapping { get; }

    /// <summary>
    /// Converts the values of the dynamic properties of <see cref="Mapping"/>
    /// to a <typeparamref name="TResult"/> instance.
    /// </summary>
    /// <param name="mapping">The <see cref="CsvMapping"/> instance of the 
    /// <see cref="Mapping"/> property, or a copy of this instance. When called
    /// from <see cref="CsvReader{TResult}"/>, the argument is never <c>null</c>.</param>
    /// <returns>The newly created <typeparamref name="TResult"/> instance.</returns>
    /// <remarks>
    /// This method is called by <see cref="CsvReader{TResult}"/>. It should
    /// not be called from own code.
    /// </remarks>
    public abstract TResult Convert(dynamic mapping);
}
