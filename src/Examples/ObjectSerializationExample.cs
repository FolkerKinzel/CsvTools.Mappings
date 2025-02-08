using FolkerKinzel.CsvTools;
using FolkerKinzel.CsvTools.Mappings;
// A namespace alias helps to avoid potential name conflicts
// with the converters from System.ComponentModel
using Conv = FolkerKinzel.CsvTools.Mappings.TypeConverters;
using System.Text;

namespace Examples;

internal sealed record Pupil(string? Name, string? Subject, DayOfWeek? LessonDay, TimeSpan? LessonBegin);

internal static class ObjectSerializationExample
{
    public static void CsvReadWritePupils(string filePath)
    {
        // Create a nonstandard CSV-File
        File.WriteAllText(filePath, """
                Unterrichtstag;Unterrichtsbeginn;Vollständiger Name;Unterrichtsfach;
                Wednesday;14:30;Susi;Piano
                Thursday;15:15;Carl Czerny;Piano;
                ;;Frederic Chopin
                """, Encoding.Unicode);

        // Reuse a converter for more than one property:
        Conv::TypeConverter<string?> stringConverter = Conv::StringConverter.CreateNullable();

        // Initialize a Mapping that retrieves the data from
        // the CSV-Columns and converts it to the right data type.
        // Aliases with wildcards can be used to match the column-headers
        // of the CSV file. 
        CsvRecordMapping mapping = CsvRecordMappingBuilder
            .Create()
            .AddProperty("Name", ["*name"], stringConverter)
            .AddProperty("Subject", ["*subject", "*fach"], stringConverter)
            .AddProperty("LessonDay", ["*day", "*tag"], new Conv::EnumConverter<DayOfWeek>().ToNullableConverter())
            .AddProperty("LessonBegin", ["*begin?"], new Conv::TimeSpanConverter().ToNullableConverter())
            .Build();

        using CsvReader<Pupil> pupilsReader =
           CsvConverter.OpenReadAnalyzed<Pupil>(filePath,
                                              mapping,
                                              // dyn is mapping as a dynamic variable ("late binding")
                                              static dyn => new Pupil(dyn.Name,
                                                                      dyn.Subject,
                                                                      dyn.LessonDay,
                                                                      dyn.LessonBegin));

        Pupil[] pupils = [.. pupilsReader];

        // Write the results to the Console:
        foreach (Pupil pupil in pupils)
        {
            Console.WriteLine(pupil);
        }

        static void PupilToCsv(Pupil pupil, dynamic dyn)
        {
            dyn.Name = pupil.Name;
            dyn.LessonBegin = pupil.LessonBegin;
            dyn.LessonDay = pupil.LessonDay;
            dyn.Subject = pupil.Subject;
        }

        // Pass the column names of the newly created CSV file:
        pupils.SaveCsv(filePath, ["Name", "Subject", "Weekday", "Begin"], mapping, PupilToCsv);
        
        Console.WriteLine();
        Console.WriteLine(File.ReadAllText(filePath));
    }
}

/*
Console output: 

Pupil { Name = Susi, Subject = Piano, LessonDay = Wednesday, LessonBegin = 14:30:00 }
Pupil { Name = Carl Czerny, Subject = Piano, LessonDay = Thursday, LessonBegin = 15:15:00 }
Pupil { Name = Frederic Chopin, Subject = , LessonDay = , LessonBegin =  }

Name,Subject,Weekday,Begin
Susi,Piano,3,
Carl Czerny,Piano,4,
Frederic Chopin,,,
*/
