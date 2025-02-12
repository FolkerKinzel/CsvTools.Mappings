namespace FolkerKinzel.CsvTools.Mappings.Intls;

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
    internal static int ValidateRegexTimeout(int timeOut, string paramName)
        => timeOut is < 1 and not Timeout.Infinite 
                ? throw new ArgumentOutOfRangeException(paramName) 
                : timeOut;
}