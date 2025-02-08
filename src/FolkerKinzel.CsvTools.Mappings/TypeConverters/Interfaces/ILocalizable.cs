namespace FolkerKinzel.CsvTools.Mappings.TypeConverters.Interfaces;

/// <summary>
/// Interface for classes that allow culture-specific cconfiguration.
/// </summary>
public interface ILocalizable
{
    /// <summary>
    /// Gets the <see cref="IFormatProvider"/> instance that provides 
    /// culture-specific formatting information.
    /// </summary>
    public IFormatProvider FormatProvider { get; }
}
