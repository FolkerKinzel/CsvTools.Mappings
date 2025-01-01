using FolkerKinzel.CsvTools.Mappings.Intls;

namespace FolkerKinzel.CsvTools.Mappings;

internal static class TimeoutHelper
{
    /// <summary>
    /// Normalizes a timeout value for regular expressions.
    /// </summary>
    /// <param name="timeOut">The value to normalize.</param>
    /// <param name="paramName">The identifier of <paramref name="timeOut"/>.</param>
    /// <returns>The normalized value.</returns>
    /// <event cref="ArgumentOutOfRangeException">The value of <paramref name="timeOut"/> 
    /// is negative or zero and not <see cref="Timeout.Infinite"/>.</event>"
    internal static int NormalizeRegexTimeout(int timeOut, string paramName)
    {
        if (timeOut > CsvRecordMapping.MaxRegexTimeout)
        {
            return CsvRecordMapping.MaxRegexTimeout;
        }
        else if (timeOut != Timeout.Infinite)
        {
            _ArgumentOutOfRangeException.ThrowIfNegativeOrZero(timeOut, paramName);
        }

        return timeOut;
    }
}