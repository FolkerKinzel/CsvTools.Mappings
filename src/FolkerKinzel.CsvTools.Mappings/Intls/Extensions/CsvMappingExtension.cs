using FolkerKinzel.CsvTools.Mappings.TypeConverters;
using FolkerKinzel.CsvTools.Mappings.Intls;
using FolkerKinzel.CsvTools.Mappings.Resources;
using System.Data;
using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings.Intls.Extensions;

/// <summary>
/// Extension methods for the <see cref="CsvMapping"/> class.
/// </summary>
internal static class CsvMappingExtension
{
    /// <summary>
    /// Fills <paramref name="mapping"/> with the fields of 
    /// a <see cref="DataRow"/>.
    /// </summary>
    /// <param name="mapping">The <see cref="CsvMapping"/> to fill.</param>
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
    /// <exception cref="InvalidOperationException"> The <see cref="CsvMapping.Record"/> property of
    /// <paramref name="mapping"/> is <c>null</c>. Assign a 
    /// <see cref="CsvRecord"/> instance to <paramref name="mapping"/> before calling 
    /// this method.
    /// </exception>
    /// <exception cref="FormatException">
    /// One of the <see cref="TypeConverter{T}"/> instances uses an invalid format string.
    /// </exception>
    internal static void FillWith(this CsvMapping mapping, DataRow dataRow, Dictionary<string, string> captionDictionary)
    {
        if (dataRow.RowState == DataRowState.Deleted)
        {
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