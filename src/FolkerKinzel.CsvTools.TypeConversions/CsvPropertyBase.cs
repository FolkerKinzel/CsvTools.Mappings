using System.Text.RegularExpressions;
using FolkerKinzel.CsvTools.TypeConversions.Converters.Intls;
using FolkerKinzel.CsvTools.TypeConversions.Resources;

namespace FolkerKinzel.CsvTools.TypeConversions;

/// <summary>
/// Abstrakte Basisklasse für Klassen, die eine Eigenschaft von <see cref="CsvRecordMapping"/> repräsentieren, die dynamisch zur Laufzeit
/// implementiert wird.
/// </summary>
/// <remarks>
/// <see cref="CsvPropertyBase"/> kapselt Informationen, die <see cref="CsvRecordMapping"/> benötigt,
/// um auf die Daten des ihm zugrundeliegenden <see cref="CsvRecord"/>-Objekts zuzugreifen.
/// </remarks>
public abstract partial class CsvPropertyBase
{
    /// <summary>
    /// Initialisiert ein neues <see cref="CsvPropertyBase"/>-Objekt.
    /// </summary>
    /// <param name="propertyName">Der Bezeichner unter dem die Eigenschaft angesprochen wird. Er muss den Regeln für C#-Bezeichner
    /// entsprechen. Es werden nur ASCII-Zeichen akzeptiert.</param>
    /// 
    /// <exception cref="ArgumentException"><paramref name="propertyName"/> entspricht nicht den Regeln für C#-Bezeichner (nur
    /// ASCII-Zeichen).</exception>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> ist <c>null</c>.
    /// </exception>
    protected CsvPropertyBase(string propertyName)
    {
        _ArgumentNullException.ThrowIfNull(propertyName, nameof(propertyName));

#if NET8_0_OR_GREATER
        if (!PropertyNameRegex().IsMatch(propertyName))
#else
        if (!PropertyNameRegex.IsMatch(propertyName))
#endif
        {
            throw new ArgumentException(Res.BadIdentifier, nameof(propertyName));
        }

        this.PropertyName = propertyName;
    }

    /// <summary>
    /// Bezeichner der Eigenschaft
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// Das <see cref="CsvRecord"/>-Objekt, über das der Zugriff auf die CSV-Datei erfolgt.
    /// </summary>
    protected internal abstract CsvRecord? Record { get; internal set; }

    /// <summary>
    /// Extrahiert Daten eines bestimmten Typs aus <see cref="CsvRecord"/>.
    /// </summary>
    /// <returns>Die extrahierten Daten.</returns>
    protected internal abstract object? GetValue();

    /// <summary>
    /// Speichert Daten eines bestimmten Typs in der CSV-Datei./>.
    /// </summary>
    /// <param name="value">Das zu speichernde Objekt.</param>
    /// <exception cref="InvalidCastException"><paramref name="value"/> entspricht nicht dem erwarteten Datentyp.</exception>
    protected internal abstract void SetValue(object? value);

#if NET8_0_OR_GREATER
    [GeneratedRegex("^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.CultureInvariant, 500)]
    private static partial Regex PropertyNameRegex();
#else
    private static Regex PropertyNameRegex { get; } = new("^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.Compiled | RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(500));
#endif
}
