using FolkerKinzel.CsvTools.TypeConversions.Converters.Intls;

namespace FolkerKinzel.CsvTools.TypeConversions;

/// <summary>
/// Abstract base class for serializing and deserializing objects whose data is distributed 
/// across multiple columns of a CSV file.
/// </summary>
/// <remarks>
/// Instances derived from this class are required by <see cref="CsvMultiColumnProperty"/>.
/// </remarks>
public abstract class CsvMultiColumnTypeConverter
{
    /// <summary>
    /// Initializes a new <see cref="CsvMultiColumnTypeConverter"/> instance.
    /// </summary>
    /// <param name="mapping">The <see cref="CsvRecordMapping"/> to use to access those columns 
    /// of the CSV file that are required for the <see cref="Type"/> conversion.</param>
    /// <exception cref="ArgumentNullException"><paramref name="mapping"/> is <c>null</c>.</exception>
    protected CsvMultiColumnTypeConverter(CsvRecordMapping mapping)
    {
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));
        this.Mapping = mapping;
    }

    /// <summary>
    /// The <see cref="CsvRecordMapping"/> to use to access those columns 
    /// of the CSV file that are required for the <see cref="Type"/> conversion.
    /// </summary>
    public CsvRecordMapping Mapping { get; }

    /// <summary>
    /// Liest die Daten aus <see cref="Mapping"/> und versucht, daraus eine Instanz des gewünschten Typs zu erzeugen.
    /// </summary>
    /// <returns>Eine Instanz des gewünschten Typs oder ein beliebiges FallbackValue (z.B. <c>null</c> oder <see cref="DBNull.Value"/>).</returns>
    public abstract object? Create();

    /// <summary>
    /// Schreibt <paramref name="value"/> mit Hilfe von <see cref="Mapping"/> in die CSV-Datei.
    /// </summary>
    /// <param name="value">Das in die CSV-Datei zu schreibende Objekt.</param>
    /// <exception cref="InvalidCastException"><paramref name="value"/> hat einen unerwarteten Datentyp.</exception>
    /// <remarks>
    /// <note type="inherit">
    /// Die Methode sollte eine 
    /// <see cref="InvalidCastException"/> werfen, wenn <paramref name="value"/> nicht <typeparamref name="T"/> oder einem
    /// anderen erwarteten Datentyp (z.B. <see cref="DBNull.Value"/>) entspricht.
    /// </note>
    /// </remarks>
    public abstract void ToCsv(object? value);

}
