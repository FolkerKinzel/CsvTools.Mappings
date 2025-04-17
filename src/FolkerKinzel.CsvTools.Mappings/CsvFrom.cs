using FolkerKinzel.Helpers.Polyfills;

namespace FolkerKinzel.CsvTools.Mappings;

/// <summary>
/// Abstract base class for classes that convert a <typeparamref name="TSource"/>
/// instance to a CSV row.
/// </summary>
/// <typeparam name="TSource">The data type to convert.</typeparam>
public abstract class CsvFrom<TSource>
{
    /// <summary>
    /// Constructor used by derived classes.
    /// </summary>
    /// <param name="mapping">The <see cref="CsvMapping"/> to use for 
    /// writing CSV.</param>
    /// <exception cref="ArgumentNullException"><paramref name="mapping"/> is <c>null</c>.</exception>
    protected CsvFrom(CsvMapping mapping)
    {
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));

        Mapping = mapping;
    }

    /// <summary>
    /// The <see cref="CsvMapping"/> to use for writing CSV.
    /// </summary>
    public dynamic Mapping { get; }

    /// <summary>
    /// Fills the dynamic properties of <see cref="Mapping"/> with
    /// the corresponding values from <paramref name="data"/>.
    /// </summary>
    /// <param name="data">The <typeparamref name="TSource"/> instance.
    /// (If the method is called from <see cref="CsvWriter{TSource}"/>, 
    /// <paramref name="data"/> is never <c>null</c>.)</param>
    /// <remarks>
    /// This method is called by <see cref="CsvWriter{TSource}"/>. It should
    /// not be called from own code.
    /// </remarks>
    public abstract void FillMapping(TSource data);
}
