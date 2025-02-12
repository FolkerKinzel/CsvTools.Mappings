using System.Data;
using System.Globalization;

namespace Examples;

internal static class Utility
{

    internal static void WriteConsole(DataTable dataTable)
    {
        foreach (DataRow? dataRow in dataTable.Rows)
        {
            if (dataRow is null)
            {
                continue;
            }

            const int padding = 15;

            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                object o = dataRow[i];
                Console.Write(o switch
                {
                    null => "<null>".PadRight(padding),
                    DBNull dBNull => "<DBNull>".PadRight(padding),
                    string s when s.Length == 0 => "\"\"".PadRight(padding),
                    TimeOnly ts => ts.ToString(CultureInfo.InvariantCulture).PadRight(padding),
                    _ => o.ToString()?.PadRight(padding)
                });
                Console.Write(' ');
            }

            Console.WriteLine();
        }
    }
}