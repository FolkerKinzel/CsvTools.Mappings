using FolkerKinzel.CsvTools.Mappings.TypeConverters;
using FolkerKinzel.CsvTools.Mappings.Intls;
using FolkerKinzel.CsvTools.Mappings.Resources;
using System.Data;
using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings;

/// <summary>
/// Extension methods for the <see cref="CsvRecordMapping"/> class.
/// </summary>
public static class CsvRecordMappingExtension
{
    /// <summary>
    /// Fills <paramref name="mapping"/> with the items of 
    /// a collection. 
    /// </summary>
    /// <param name="mapping">The <see cref="CsvRecordMapping"/> to fill.</param>
    /// <param name="data">The collection whose content is used to fill 
    /// <paramref name="mapping"/>.</param>
    /// <param name="resetExcess">
    /// If <paramref name="data"/> has fewer items than <paramref name="mapping"/> has
    /// <see cref="DynamicProperty"/> instances and this parameter is <c>true</c>, the surplus 
    /// properties in record will be reset to their <see cref="DynamicProperty.DefaultValue"/>. 
    /// For performance reasons this parameter can be set to <c>false</c> when writing CSV because 
    /// <see cref="CsvWriter.WriteRecord"/> resets all fields in <paramref name="mapping"/>.
    /// </param>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="mapping"/> or <paramref name="data"/>
    /// is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="data"/> has more items than the
    /// number of <see cref="DynamicProperty"/> instances in <paramref name="mapping"/>.</exception>
    /// <exception cref="InvalidOperationException"> The <see cref="CsvRecordMapping.Record"/> property of
    /// <paramref name="mapping"/> is <c>null</c>. Assign a 
    /// <see cref="CsvRecord"/> instance to <paramref name="mapping"/> before calling 
    /// this method.
    /// </exception>
    /// <exception cref="InvalidCastException">
    /// An item in <paramref name="data"/> does not match the expected data type.
    /// </exception>
    /// <exception cref="FormatException">
    /// One of the <see cref="TypeConverter{T}"/> instances uses an invalid format string.
    /// </exception>
    public static void FillWith(this CsvRecordMapping mapping,
                                IEnumerable<object?> data,
                                bool resetExcess = true)
    {
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));
        _ArgumentNullException.ThrowIfNull(data, nameof(data));

        int i = 0;

        foreach (object? item in data)
        {
            if (i >= mapping.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(data));
            }

            mapping[i++].Value = item;
        }

        if (resetExcess)
        {
            for (; i < mapping.Count; i++)
            {
                DynamicProperty prop = mapping[i];
                prop.Value = prop.DefaultValue;
            }
        }
    }

    /// <summary>
    /// Fills <paramref name="mapping"/> with the fields of 
    /// a <see cref="DataRow"/>.
    /// </summary>
    /// <param name="mapping">The <see cref="CsvRecordMapping"/> to fill.</param>
    /// <param name="dataRow">The <see cref="DataRow"/> whose content is used to fill 
    /// <paramref name="mapping"/>.</param>
    /// 
    /// <remarks>
    /// <para>
    /// Each <see cref="DynamicProperty.PropertyName"/> of <paramref name="mapping"/>
    /// MUST have a corresponding <see cref="DataColumn"/> in <paramref name="dataRow"/>
    /// - corresponding in the <see cref="DataColumn.Caption"/> property (case-insensitive)
    /// and the accepted data type.
    /// </para>
    /// <para>
    /// Effort must be taken that the <see cref="DynamicProperty.PropertyName"/>s in 
    /// <paramref name="mapping"/> are unique, even when treated case-insensitive.
    /// </para>
    /// <para>
    /// The <see cref="DynamicProperty"/> instances in <paramref name="mapping"/> don't need to 
    /// match all columns of the <see cref="DataTable"/> or all columns of the CSV file (neither 
    /// in number nor in order).
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="mapping"/> or <paramref name="dataRow"/>
    /// is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// There is a <see cref="DynamicProperty"/> in <paramref name="mapping"/> whose 
    /// <see cref="DynamicProperty.PropertyName"/> finds no corresponding <see cref="DataColumn.ColumnName"/>
    /// in <paramref name="dataRow"/>.
    /// </exception>
    /// <exception cref="InvalidCastException">
    /// A value in <paramref name="dataRow"/> does not match the expected data type in 
    /// <paramref name="mapping"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException"> The <see cref="CsvRecordMapping.Record"/> property of
    /// <paramref name="mapping"/> is <c>null</c>. Assign a 
    /// <see cref="CsvRecord"/> instance to <paramref name="mapping"/> before calling 
    /// this method.
    /// </exception>
    /// <exception cref="FormatException">
    /// One of the <see cref="TypeConverter{T}"/> instances uses an invalid format string.
    /// </exception>
    public static void FillWith(this CsvRecordMapping mapping, DataRow dataRow)
    {
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));
        _ArgumentNullException.ThrowIfNull(dataRow, nameof(dataRow));

        mapping.FillWithIntl(dataRow, DataTableHelper.CreateCaptionDictionary(dataRow.Table));
    }

    /// <summary>
    /// Fills <paramref name="mapping"/> with the fields of 
    /// a <see cref="DataRow"/>.
    /// </summary>
    /// <param name="mapping">The <see cref="CsvRecordMapping"/> to fill.</param>
    /// <param name="dataRow">The <see cref="DataRow"/> whose content is used to fill 
    /// <paramref name="mapping"/>.</param>
    /// <param name="captionDictionary">
    /// A <see cref="Dictionary{TKey, TValue}"/> that has the <see cref="DataColumn.Caption"/>
    /// properties of the <paramref name="dataRow"/> as keys and the corresponding 
    /// <see cref="DataColumn.ColumnName"/>s as values.
    /// </param>
    /// 
    /// <remarks>
    /// <para>
    /// Each <see cref="DynamicProperty.PropertyName"/> of <paramref name="mapping"/>
    /// MUST have a corresponding <see cref="DataColumn"/> in <paramref name="dataRow"/>
    /// - corresponding in the <see cref="DataColumn.Caption"/> property (case-insensitive)
    /// and the accepted data type.
    /// </para>
    /// <para>
    /// Effort must be taken that the 
    /// <see cref="DynamicProperty.PropertyName"/>s in <paramref name="mapping"/> are unique, 
    /// even when treated case-insensitive.
    /// </para>
    /// <para>
    /// The <see cref="DynamicProperty"/> instances in <paramref name="mapping"/> don't need to match 
    /// all columns of the <see cref="DataTable"/> or all columns of the CSV file (neither in
    /// number nor in order).
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentException">
    /// There is a <see cref="DynamicProperty"/> in <paramref name="mapping"/> whose 
    /// <see cref="DynamicProperty.PropertyName"/> finds no corresponding <see cref="DataColumn.ColumnName"/>
    /// in <paramref name="dataRow"/>.
    /// </exception>
    /// <exception cref="InvalidCastException">
    /// A value in <paramref name="dataRow"/> does not match the expected data type in 
    /// <paramref name="mapping"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException"> The <see cref="CsvRecordMapping.Record"/> property of
    /// <paramref name="mapping"/> is <c>null</c>. Assign a 
    /// <see cref="CsvRecord"/> instance to <paramref name="mapping"/> before calling 
    /// this method.
    /// </exception>
    /// <exception cref="FormatException">
    /// One of the <see cref="TypeConverter{T}"/> instances uses an invalid format string.
    /// </exception>
    internal static void FillWithIntl(this CsvRecordMapping mapping, DataRow dataRow, Dictionary<string, string> captionDictionary)
    {
        if (dataRow.RowState == DataRowState.Deleted)
        {
            if (mapping.Record is null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Res.InstanceIsNull, nameof(CsvRecordMapping.Record)));
            }

            mapping.Record.Clear();
            return;
        }

        int i = 0;

        try
        {
            for (; i < mapping.Count; i++)
            {
                mapping[i].Value = dataRow[captionDictionary[mapping[i].PropertyName]];
            }
        } 
        catch (KeyNotFoundException e)
        {
            throw new ArgumentException(e.Message, nameof(mapping), e);
        }
    }
}