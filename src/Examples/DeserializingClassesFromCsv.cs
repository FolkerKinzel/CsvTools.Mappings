﻿using FolkerKinzel.CsvTools.Mappings;
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

internal static class DeserializingClassesFromCsv
{
    public static void TestDeserializingClassesFromCsv()
    {
        const string csvFileName = "Objects.csv";

        // Create a nonstandard CSV-File
        File.WriteAllText(csvFileName, """
                Unterrichtstag;Unterrichtsbeginn;Vollständiger Name;Unterrichtsfach;
                Wednesday;14:30;Susi;Piano
                Thursday;15:15;Carl Czerny;Piano;
                ;;Frederic Chopin
                """);

        // Reuse a converter for more than one property:
        TypeConverter<string?> stringConverter = StringConverter.CreateNullable();

        // Initialize a CsvRecordWrapper which retrieves the data from
        // the CSV-Columns and converts it to the right data type.
        // Aliases with wildcards can be used to match the column-headers
        // of the CSV file. 
        Mapping mapping = Mapping
            .Create()
            .AddProperty("Name", ["*name"], stringConverter)
            .AddProperty("Subject", ["*subject", "*fach"], stringConverter)
            .AddProperty("LessonDay", ["*day", "*tag"], new EnumConverter<DayOfWeek>().ToNullableConverter())
            .AddProperty("LessonBegin", ["*begin?"], new TimeSpanConverter().ToNullableConverter());

        Pupil[] pupils =
            mapping.ReadAnalyzed(csvFileName,
                                  static dyn => new Pupil
                                  {
                                      Name = dyn.Name,
                                      LessonBegin = dyn.LessonBegin,
                                      LessonDay = dyn.LessonDay,
                                      Subject = dyn.Subject
                                  }
                                );

        // Write the results to the Console:
        foreach (Pupil pupil in pupils)
        {
            Console.WriteLine(pupil);
            Console.WriteLine();
        }
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
*/
