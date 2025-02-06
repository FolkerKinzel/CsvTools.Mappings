using System.Data;

namespace FolkerKinzel.CsvTools.Mappings.Intls;

internal static class DataTableHelper
{
    /// <summary>
    /// Creates a <see cref="Dictionary{TKey, TValue}"/> that has the <see cref="DataColumn.Caption"/>
    /// properties of the <paramref name="dataTable"/> as keys and the corresponding 
    /// <see cref="DataColumn.ColumnName"/>s as values.
    /// </summary>
    /// <param name="dataTable">The <see cref="DataTable"/> to examine.</param>
    /// 
    /// <returns>A <see cref="Dictionary{TKey, TValue}"/> that has the <see cref="DataColumn.Caption"/>
    /// properties of the <paramref name="dataTable"/> as keys and the corresponding 
    /// <see cref="DataColumn.ColumnName"/>s as values.</returns>
    /// 
    /// <exception cref="ArgumentException">
    /// A value of <see cref="DataColumn.Caption"/> occurs twice in <paramref name="dataTable"/>. The 
    /// comparison is case-insensitive.
    /// </exception>
    internal static Dictionary<string, string> CreateCaptionDictionary(DataTable dataTable)
    {
        DataColumnCollection columns = dataTable.Columns;
        var captionDictionary = new Dictionary<string, string>(columns.Count, StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < columns.Count; i++)
        {
            DataColumn column = columns[i];
            captionDictionary.Add(column.Caption, column.ColumnName);
        }

        return captionDictionary;
    }
}