﻿using System.Diagnostics;
using FolkerKinzel.CsvTools.TypeConversions.Converters;

namespace FolkerKinzel.CsvTools.TypeConversions;

/// <summary>
/// Abstrakte Basisklasse für Klassen, die eine Eigenschaft von <see cref="CsvRecordMapping"/> repräsentieren, die dynamisch zur Laufzeit
/// implementiert wird, und die ihre Daten aus einer einzelnen Spalte der CSV-Datei bezieht.
/// </summary>
public abstract class CsvSingleColumnProperty : CsvPropertyBase
{
    /// <summary>
    /// Initialisiert ein neues <see cref="CsvSingleColumnProperty"/>-Objekt.
    /// </summary>
    /// <param name="propertyName">Der Bezeichner unter dem die Eigenschaft angesprochen wird. Er muss den Regeln für C#-Bezeichner
    /// entsprechen. Es werden nur ASCII-Zeichen akzeptiert.</param>
    /// <param name="converter">Der <see cref="ICsvTypeConverter"/>, der die Typkonvertierung übernimmt.</param>
    /// 
    /// <exception cref="ArgumentException"><paramref name="propertyName"/> entspricht nicht den Regeln für C#-Bezeichner (nur
    /// ASCII-Zeichen).</exception>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> oder 
    /// <paramref name="converter"/> ist <c>null</c>.</exception>
    protected CsvSingleColumnProperty(string propertyName, ICsvTypeConverter converter) : base(propertyName)
    {
        if (converter is null)
        {
            throw new ArgumentNullException(nameof(converter));
        }

        this.Converter = converter;
    }

    /// <summary>
    /// Ein <see cref="ICsvTypeConverter"/>-Objekt, das die Typkonvertierung durchführt.
    /// </summary>
    public ICsvTypeConverter Converter { get; }

    /// <summary>
    /// Das <see cref="CsvRecord"/>-Objekt, über das der Zugriff auf die CSV-Datei erfolgt.
    /// </summary>
    protected internal override CsvRecord? Record { get; internal set; }

    /// <summary>
    /// Der Index der Spalte der CSV-Datei, auf die <see cref="CsvColumnNameProperty"/> tatsächlich zugreift oder <c>null</c>,
    /// wenn <see cref="CsvColumnNameProperty"/> kein Ziel in der CSV-Datei findet. Die Eigenschaft wird beim
    /// ersten Lese- oder Schreibzugriff aktualisiert.
    /// </summary>
    public int? ReferredCsvIndex { get; protected set; }

    /// <summary>
    /// Wird bei jedem Schreib- oder Lesevorgang aufgerufen, um zu überprüfen, ob <see cref="ReferredCsvIndex"/> noch aktuell ist.
    /// </summary>
    protected abstract void UpdateReferredCsvIndex();

    /// <inheritdoc/>
    protected internal override object? GetValue()
    {
        Debug.Assert(Record != null);
        UpdateReferredCsvIndex();

        try
        {
            return ReferredCsvIndex.HasValue ? Converter.Parse(Record.Values[ReferredCsvIndex.Value].Span) : Converter.FallbackValue;
        }
        catch (Exception e)
        {
            throw new InvalidCastException(e.Message, e);
        }
    }

    /// <inheritdoc/>
    protected internal override void SetValue(object? value)
    {
        Debug.Assert(Record != null);
        UpdateReferredCsvIndex();

        if (ReferredCsvIndex.HasValue)
        {
            Record.Values[ReferredCsvIndex.Value]
                = Converter.ConvertToString(value).AsMemory();
        }
    }
}
