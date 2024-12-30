﻿namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public interface IHexConverter<T>
{
    /// <summary>
    /// Changes the converter to parse and output hexadecimal values.
    /// </summary>
    /// <returns>The modified converter instance to be able to chain calls.</returns>
    CsvTypeConverter<T> AsHexConverter();
}