﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.CsvTools.TypeConversions;
using FolkerKinzel.CsvTools;
using BenchmarkDotNet.Attributes;
using System.IO;
using FolkerKinzel.CsvTools.TypeConversions.Converters;

namespace Benchmarks;

[MemoryDiagnoser]
public class AccessBenchmark
{
    private readonly string _csv;
    private readonly CsvRecordMapping _indexWrapper;
    private readonly CsvRecordMapping _nameWrapper;

    public AccessBenchmark()
    {
        _csv = Properties.Resources.Test1;
        ICsvTypeConverter conv = new StringConverter();
        _indexWrapper = new CsvRecordMapping();
        _indexWrapper.AddProperty(new CsvIndexProperty("Column0", 0, conv));
        _indexWrapper.AddProperty(new CsvIndexProperty("Column1", 1, conv));
        _indexWrapper.AddProperty(new CsvIndexProperty("Column2", 2, conv));

        _nameWrapper = new CsvRecordMapping();
        _nameWrapper.AddProperty(new CsvColumnNameProperty("Column0", new string[] { "Column0" }, conv));
        _nameWrapper.AddProperty(new CsvColumnNameProperty("Column1", new string[] { "Column1" }, conv));
        _nameWrapper.AddProperty(new CsvColumnNameProperty("Column2", new string[] { "Column2" }, conv));
    }

    [Benchmark]
    public int AccessIndexBench()
    {
        int letters = 0;

        var reader = new CsvEnumerator(new StringReader(_csv));
        foreach (CsvRecord row in reader)
        {
            _indexWrapper.Record = row;

            for (int i = 0; i < _indexWrapper.Count; i++)
            {
                letters += ((string?)_indexWrapper[i])!.Length;
            }
        }

        return letters;
    }

    [Benchmark]
    public int AccessNameBench()
    {
        int letters = 0;

        var reader = new CsvEnumerator(new StringReader(_csv));
        foreach (CsvRecord row in reader)
        {
            _nameWrapper.Record = row;

            for (int i = 0; i < _nameWrapper.Count; i++)
            {
                letters += ((string?)_nameWrapper[i])!.Length;
            }
        }

        return letters;
    }

}
