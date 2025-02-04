using FolkerKinzel.CsvTools;
using FolkerKinzel.CsvTools.Mappings;
using FolkerKinzel.CsvTools.Mappings.Converters;

namespace Examples;

internal sealed class Pupil
{
    public string? Name { get; set; }
    public string? Subject { get; set; }
    public DayOfWeek? LessonDay { get; set; }
    public TimeSpan? LessonBegin { get; set; }

    public override string ToString()
    {
        const string NULL = "<null>";
        string lessonDay = LessonDay.HasValue ? $"{nameof(DayOfWeek)}.{LessonDay}" : NULL;
        string lessonBegin = LessonBegin.HasValue ? LessonBegin.Value.ToString() : NULL;

        return $"""
            Name:        {Name ?? NULL}
            Subject:     {Subject ?? NULL}
            LessonDay:   {lessonDay}
            LessonBegin: {lessonBegin}
            """;
    }
}

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
                """);

        // Reuse a converter for more than one property:
        TypeConverter<string?> stringConverter = StringConverter.CreateNullable();

        // Initialize a Mapping that retrieves the data from
        // the CSV-Columns and converts it to the right data type.
        // Aliases with wildcards can be used to match the column-headers
        // of the CSV file. 
        Mapping mapping = MappingBuilder
            .Create()
            .AddProperty("Name", ["*name"], stringConverter)
            .AddProperty("Subject", ["*subject", "*fach"], stringConverter)
            .AddProperty("LessonDay", ["*day", "*tag"], new EnumConverter<DayOfWeek>().ToNullableConverter())
            .AddProperty("LessonBegin", ["*begin?"], new TimeSpanConverter().ToNullableConverter())
            .Build();

        using CsvReader<Pupil> pupilsReader =
           CsvMapping.OpenReadAnalyzed<Pupil>(filePath,
                                              mapping,
                                              // dyn is mapping as a dynamic variable ("late binding")
                                              static dyn => new Pupil
                                              {
                                                  Name = dyn.Name,
                                                  LessonBegin = dyn.LessonBegin,
                                                  LessonDay = dyn.LessonDay,
                                                  Subject = dyn.Subject
                                              });

        Pupil[] pupils = [.. pupilsReader];

        // Write the results to the Console:
        foreach (Pupil pupil in pupils)
        {
            Console.WriteLine(pupil);
            Console.WriteLine();
        }

        // Pass the column names of the newly created CSV file:
        using (CsvWriter csvWriter = Csv.OpenWrite(filePath, ["Name", "Subject", "Weekday", "Begin"]))
        using (CsvWriter<Pupil> pupilsWriter =
            CsvMapping.OpenWrite<Pupil>(csvWriter,
                                        mapping,
                                        static (pupil, dyn) =>
                                        {
                                            dyn.Name = pupil.Name;
                                            dyn.LessonBegin = pupil.LessonBegin;
                                            dyn.LessonDay = pupil.LessonDay;
                                            dyn.Subject = pupil.Subject;
                                        }))
        {
            foreach (Pupil pupil in pupils)
            {
                pupilsWriter.Write(pupil);
            }
        }

        Console.WriteLine(File.ReadAllText(filePath));
    }
}

/*
Console output: 

Name:        Susi
Subject:     Piano
LessonDay:   DayOfWeek.Wednesday
LessonBegin: 14:30:00

Name:        Carl Czerny
Subject:     Piano
LessonDay:   DayOfWeek.Thursday
LessonBegin: 15:15:00

Name:        Frederic Chopin
Subject:     <null>
LessonDay:   <null>
LessonBegin: <null>

Name,Subject,Weekday,Begin
Susi,Piano,3,
Carl Czerny,Piano,4,
Frederic Chopin,,,
*/
