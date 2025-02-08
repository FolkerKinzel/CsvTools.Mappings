using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.CsvTools.Mappings;
using FolkerKinzel.CsvTools;
using BenchmarkDotNet.Attributes;
using System.IO;
using FolkerKinzel.CsvTools.Mappings.TypeConverters;

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
        TypeConverter<string> conv = StringConverter.CreateNonNullable();
        _indexWrapper = CsvRecordMappingBuilder
            .Create()
            .AddProperty("Column0", 0, conv)
            .AddProperty("Column1", 1, conv)
            .AddProperty("Column2", 2, conv)
            .Build();

        _nameWrapper = CsvRecordMappingBuilder
            .Create()
            .AddProperty("Column0", conv)
            .AddProperty("Column1", conv)
            .AddProperty("Column2", conv)
            .Build();
    }

    [Benchmark]
    public int AccessIndexBench()
    {
        int letters = 0;

        var reader = new CsvReader(new StringReader(_csv));

        foreach (CsvRecord row in reader)
        {
            _indexWrapper.Record = row;

            for (int i = 0; i < _indexWrapper.Count; i++)
            {
                letters += _indexWrapper[i].AsITypedProperty<string>().Value.Length;
            }
        }

        return letters;
    }

    [Benchmark]
    public int AccessNameBench()
    {
        int letters = 0;

        var reader = new CsvReader(new StringReader(_csv));

        foreach (CsvRecord row in reader)
        {
            _nameWrapper.Record = row;

            for (int i = 0; i < _nameWrapper.Count; i++)
            {
                letters += _nameWrapper[i].AsITypedProperty<string>().Value.Length;
            }
        }

        return letters;
    }

}
