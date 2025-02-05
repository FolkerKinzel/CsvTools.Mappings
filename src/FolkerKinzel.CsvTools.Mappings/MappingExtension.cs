﻿using FolkerKinzel.CsvTools.Mappings.Converters;
using FolkerKinzel.CsvTools.Mappings.Intls;
using FolkerKinzel.CsvTools.Mappings.Resources;
using System.Data;
using System.Globalization;

namespace FolkerKinzel.CsvTools.Mappings;

/// <summary>
/// Extension methods for the <see cref="Mapping"/> class.
/// </summary>
public static class MappingExtension
{
    ///// <summary>
    ///// Resets all <see cref="DynamicProperty"/> instances in <paramref name="mapping"/>
    ///// to their <see cref="DynamicProperty.DefaultValue"/>s.
    ///// </summary>
    ///// <param name="mapping">The <see cref="Mapping"/> instance whose values are being reset.</param>
    ///// <returns><paramref name="mapping"/> to chain calls.</returns>
    //public static Mapping Clear(this Mapping mapping)
    //{
    //    _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));

    //    foreach (var prop in mapping)
    //    {
    //        prop.Value = prop.DefaultValue;
    //    }

    //    return mapping;
    //}

    /// <summary>
    /// Fills <paramref name="mapping"/> with the items of 
    /// a collection. 
    /// </summary>
    /// <param name="mapping">The <see cref="Mapping"/> to fill.</param>
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
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="mapping"/> or <paramref name="data"/>
    /// is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="data"/> has more items than the
    /// number of <see cref="DynamicProperty"/> instances in <paramref name="mapping"/>.</exception>
    /// <exception cref="InvalidOperationException"> The <see cref="Mapping.Record"/> property of
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
    public static void FillWith(this Mapping mapping,
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
    /// <param name="mapping">The <see cref="Mapping"/> to fill.</param>
    /// <param name="dataRow">The <see cref="DataRow"/> whose content is used to fill 
    /// <paramref name="mapping"/>.</param>
    /// 
    /// <remarks>
    /// <para>
    /// Each <see cref="DynamicProperty.PropertyName"/> in <paramref name="mapping"/>
    /// MUST have a corresponding <see cref="DataColumn.ColumnName"/> in <paramref name="dataRow"/>.
    /// The corresponding columns have to match in theír data type too.
    /// </para>
    /// <para>
    /// Since <see cref="Mapping"/> uses case-sensitiv property names and the column names in 
    /// <paramref name="dataRow"/> are case-insensitive, effort must be taken that the 
    /// <see cref="DynamicProperty.PropertyName"/>s in <paramref name="mapping"/> are unique, 
    /// even when treated case-insensitive.
    /// </para>
    /// <para>
    /// The <see cref="DynamicProperty"/> instances in <paramref name="mapping"/> don't need to match 
    /// all columns in <paramref name="dataRow"/> (neither in number nor in order).
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
    /// <exception cref="InvalidOperationException"> The <see cref="Mapping.Record"/> property of
    /// <paramref name="mapping"/> is <c>null</c>. Assign a 
    /// <see cref="CsvRecord"/> instance to <paramref name="mapping"/> before calling 
    /// this method.
    /// </exception>
    /// <exception cref="FormatException">
    /// One of the <see cref="TypeConverter{T}"/> instances uses an invalid format string.
    /// </exception>
    public static void FillWith(this Mapping mapping, DataRow dataRow)
    {
        _ArgumentNullException.ThrowIfNull(mapping, nameof(mapping));
        _ArgumentNullException.ThrowIfNull(dataRow, nameof(dataRow));
        FillWithIntl(mapping, dataRow);
    }

    /// <summary>
    /// Fills <paramref name="mapping"/> with the fields of 
    /// a <see cref="DataRow"/>.
    /// </summary>
    /// <param name="mapping">The <see cref="Mapping"/> to fill.</param>
    /// <param name="dataRow">The <see cref="DataRow"/> whose content is used to fill 
    /// <paramref name="mapping"/>.</param>
    /// 
    /// <remarks>
    /// <para>
    /// Each <see cref="DynamicProperty.PropertyName"/> in <paramref name="mapping"/>
    /// MUST have a corresponding <see cref="DataColumn.ColumnName"/> in <paramref name="dataRow"/>.
    /// The corresponding columns have to match in theír data type too.
    /// </para>
    /// <para>
    /// Since <see cref="Mapping"/> uses case-sensitiv property names and the column names in 
    /// <paramref name="dataRow"/> are case-insensitive, effort must be taken that the 
    /// <see cref="DynamicProperty.PropertyName"/>s in <paramref name="mapping"/> are unique, 
    /// even when treated case-insensitive.
    /// </para>
    /// <para>
    /// The <see cref="DynamicProperty"/> instances in <paramref name="mapping"/> don't need to match 
    /// all columns in <paramref name="dataRow"/> (neither in number nor in order).
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
    /// <exception cref="InvalidOperationException"> The <see cref="Mapping.Record"/> property of
    /// <paramref name="mapping"/> is <c>null</c>. Assign a 
    /// <see cref="CsvRecord"/> instance to <paramref name="mapping"/> before calling 
    /// this method.
    /// </exception>
    /// <exception cref="FormatException">
    /// One of the <see cref="TypeConverter{T}"/> instances uses an invalid format string.
    /// </exception>
    internal static void FillWithIntl(this Mapping mapping, DataRow dataRow)
    {
        if (dataRow.RowState == DataRowState.Deleted)
        {
            if (mapping.Record is null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Res.InstanceIsNull, nameof(Mapping.Record)));
            }

            mapping.Record.Clear();
            return;
        }

        int i = 0;

        for (; i < mapping.Count; i++)
        {
            mapping[i].Value = dataRow[mapping[i].PropertyName];
        }
    }
}