using FolkerKinzel.CsvTools;
using FolkerKinzel.CsvTools.Mappings;
// A namespace alias helps to avoid name conflicts
// with the converters from System.ComponentModel
using Conv = FolkerKinzel.CsvTools.Mappings.TypeConverters;
using System.Text;

namespace Examples;

internal sealed record Pupil(string? Name, string? Subject, DayOfWeek? LessonDay, TimeOnly? LessonBegin);

internal static class ObjectSerializationExample
{
    public static void CsvReadWritePupils(string filePath)
    {
        Pupil[] pupils = [
                            new("Susi", "Piano", DayOfWeek.Wednesday, new TimeOnly(14, 30)),
                            new("Carl Czerny", "Piano", DayOfWeek.Thursday, new TimeOnly(15, 15)),
                            new("Frederic Chopin", "Piano", null, null)
                         ];

        // A converter can be reused for more than one DynamicProperty:
        Conv::TypeConverter<string?> stringConverter = Conv::StringConverter.CreateNullable();

        // Initialize a CsvMapping that maps the data from the CSV-Columns and converts it to the right data type.
        // Aliases with wildcards can be used to match the column-headers of the CSV file. 
        CsvMapping mapping = CsvMappingBuilder
            .Create()
            .AddProperty("Name", ["*name"], stringConverter)
            .AddProperty("Subject", ["*subject", "*fach"], stringConverter)
            .AddProperty("LessonDay", ["*day", "*tag"], new Conv::EnumConverter<DayOfWeek>().ToNullableConverter())
            .AddProperty("LessonBegin", ["*begin?"], new Conv::TimeOnlyConverter().ToNullableConverter())
            .Build();

        // Create a CSV-File as UTF-16 LE
        pupils.SaveCsv(filePath,
                       mapping,
                       (pupil, dyn) => // dyn is mapping as a dynamic variable ("late binding")
                       {
                           dyn.Name = pupil.Name;
                           dyn.Subject = pupil.Subject;
                           dyn.LessonDay = pupil.LessonDay;
                           dyn.LessonBegin = pupil.LessonBegin;
                       },
                       columnNames: ["Unterrichtstag", "Unterrichtsbeginn", "Vollständiger Name", "Unterrichtsfach"],
                       textEncoding: Encoding.Unicode);
        
        // Reading analyzed will auto-detect the UTF-16 encoding:
        using CsvReader<Pupil> pupilsReader =
           CsvConverter.OpenReadAnalyzed<Pupil>(filePath,
                                                mapping,
                                                static dyn => new Pupil(dyn.Name,
                                                                        dyn.Subject,
                                                                        dyn.LessonDay,
                                                                        dyn.LessonBegin));
                                                
        pupils = [.. pupilsReader];

        // Write the results to the Console:
        foreach (Pupil pupil in pupils)
        {
            Console.WriteLine(pupil);
        }
    }
}

/*
Console output: 

Pupil { Name = Susi, Subject = Piano, LessonDay = Wednesday, LessonBegin = 14:30 }
Pupil { Name = Carl Czerny, Subject = Piano, LessonDay = Thursday, LessonBegin = 15:15 }
Pupil { Name = Frederic Chopin, Subject = Piano, LessonDay = , LessonBegin =  }
*/
