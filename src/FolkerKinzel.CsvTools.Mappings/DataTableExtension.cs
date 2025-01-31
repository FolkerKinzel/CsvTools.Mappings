using FolkerKinzel.CsvTools.Mappings.Intls;
using System.Data;

namespace FolkerKinzel.CsvTools.Mappings;

public static class DataTableExtension
{
    /// <summary>
    /// Adds CSV content as <see cref="DataRow"/>s to a <see cref="DataTable"/>.
    /// </summary>
    /// <param name="dataTable">The <see cref="DataTable"/> to which <see cref="DataRows"/>
    /// are added.</param>
    /// <param name="reader">The <see cref="CsvReader"/> to use.</param>
    /// <param name="mapping">The <see cref="Mapping"/> to be used.</param>
    /// 
    /// <remarks>
    /// <para>
    /// Each <see cref="DynamicProperty.PropertyName"/> in <paramref name="mapping"/>
    /// MUST have a corresponding <see cref="DataColumn.ColumnName"/> in <paramref name="dataTable"/>.
    /// The corresponding columns have to match in theír data type too.
    /// </para>
    /// <para>
    /// Since <see cref="Mapping"/> uses case-sensitiv property names and the column names in 
    /// <paramref name="dataTable"/> are case-insensitive, effort must be taken that the 
    /// <see cref="DynamicProperty.PropertyName"/>s in <paramref name="mapping"/> are unique, 
    /// even when treated case-insensitive.
    /// </para>
    /// <para>
    /// The <see cref="DynamicProperty"/> instances in <paramref name="mapping"/> don't need to match 
    /// neither all columns of the <see cref="DataTable"/> nor all columns of the CSV file (neither in
    /// number nor in order).
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="dataTable"/>, or <paramref name="writer"/>,
    /// or <paramref name="mapping"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// There is a <see cref="DynamicProperty"/> in <paramref name="mapping"/> whose 
    /// <see cref="DynamicProperty.PropertyName"/> finds no corresponding <see cref="DataColumn.ColumnName"/>
    /// in <see cref="dataTable"/>.
    /// </exception>
    /// <exception cref="FormatException">Parsing fails and <see cref="ITypeConverter{T}.Throwing"/> is <c>true</c>.</exception>
    /// <exception cref="IOException">I/O error.</exception>
    /// <exception cref="ObjectDisposedException">The file was already closed.</exception>
    public static void ReadCsv(this DataTable dataTable, CsvReader reader, Mapping mapping)
    {
        _ArgumentNullException.ThrowIfNull(dataTable, nameof(dataTable));
        _ArgumentNullException.ThrowIfNull(reader, nameof(reader));
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));

        foreach (CsvRecord record in reader)
        {
            mapping.Record = record;
            DataRow dataRow = dataTable.NewRow();
            dataRow.BeginEdit();
            dataTable.Rows.Add(dataRow);
            
            for (int i = 0; i < mapping.Count; i++)
            {
                DynamicProperty prop = mapping[i];
                dataRow[prop.PropertyName] = prop.Value;
            }
        }

        //dataTable.AcceptChanges();
    }

    /// <summary>
    /// Writes the content of a <see cref="DataTable"/> as CSV.
    /// </summary>
    /// <param name="dataTable">The <see cref="DataTable"/> whose content is written.</param>
    /// <param name="writer">The <see cref="CsvWriter"/> to be used.</param>
    /// <param name="mapping">The <see cref="Mapping"/> to be used.</param>
    /// 
    /// <remarks>
    /// <para>
    /// Each <see cref="DynamicProperty.PropertyName"/> in <paramref name="mapping"/>
    /// MUST have a corresponding <see cref="DataColumn.ColumnName"/> in <paramref name="dataTable"/>.
    /// The corresponding columns have to match in theír data type too.
    /// </para>
    /// <para>
    /// Since <see cref="Mapping"/> uses case-sensitiv property names and the column names in 
    /// <paramref name="dataTable"/> are case-insensitive, effort must be taken that the 
    /// <see cref="DynamicProperty.PropertyName"/>s in <paramref name="mapping"/> are unique, 
    /// even when treated case-insensitive.
    /// </para>
    /// <para>
    /// The <see cref="DynamicProperty"/> instances in <paramref name="mapping"/> don't need to match 
    /// neither all columns of the <see cref="DataTable"/> nor all columns of the CSV file (neither in
    /// number nor in order).
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="dataTable"/>, or <paramref name="writer"/>,
    /// or <paramref name="mapping"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// There is a <see cref="DynamicProperty"/> in <paramref name="mapping"/> whose 
    /// <see cref="DynamicProperty.PropertyName"/> finds no corresponding <see cref="DataColumn.ColumnName"/>
    /// in <see cref="dataTable"/>.
    /// </exception>
    /// <exception cref="InvalidCastException">
    /// A value in <paramref name="dataTable"/> does not match the expected data type in 
    /// <paramref name="mapping"/>.
    /// </exception>
    /// <exception cref="FormatException">
    /// One of the <see cref="TypeConverter{T}"/> instances uses an invalid format string.
    /// </exception>
    /// <exception cref="IOException">I/O error.</exception>
    /// <exception cref="ObjectDisposedException">The file was already closed.</exception>
    public static void WriteCsv(this DataTable dataTable, CsvWriter writer, Mapping mapping)
    {
        _ArgumentNullException.ThrowIfNull(dataTable, nameof(dataTable));
        _ArgumentNullException.ThrowIfNull(writer, nameof(writer));
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));

        mapping.Record = writer.Record;
        DataRowCollection rows = dataTable.Rows;

        for (int i = 0; i < rows.Count; i++)
        {
            mapping.FillWithIntl(rows[i]);
            writer.WriteRecord();
        }
    }
}