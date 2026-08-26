using System;
using Limbo.Umbraco.Emply.Exceptions;
using Skybrud.Essentials.Time.Xml;

namespace Limbo.Umbraco.Emply;

/// <summary>
/// Static class with various utility methods used throughout the package.
/// </summary>
public class EmplyUtils {

    /// <summary>
    /// Parses a string value into a <see cref="TimeSpan" />.
    /// </summary>
    /// <param name="value">The value to parse.</param>
    /// <returns>The parsed <see cref="TimeSpan" />.</returns>
    /// <remarks>
    /// Valid formats include:
    /// <list type="bullet">
    /// <item><description>Numeric minutes: <c>5</c>, <c>30</c>, <c>120</c></description></item>
    /// <item><description><see cref="TimeSpan" /> formats: <c>00:05:00</c>, <c>01:30:00</c>, <c>2.00:00:00</c></description></item>
    /// <item><description>XML Schema / ISO 8601 durations: <c>PT5M</c>, <c>PT30M</c>, <c>P1DT2H</c></description></item>
    /// </list>
    /// </remarks>
    /// <exception cref="EmplyException">Thrown when <paramref name="value" /> is not a valid interval.</exception>
    public static TimeSpan ParseTimeSpan(string value) {

        if (string.IsNullOrEmpty(value)) throw new EmplyException("Interval value cannot be null or empty.");

        // If configured value is a numeric value, we assume it's in minutes
        if (int.TryParse(value, out int minutes)) {
            return TimeSpan.FromMinutes(minutes);
        }

        // Try to parse as TimeSpan
        if (TimeSpan.TryParse(value, out TimeSpan timeSpan)) {
            return timeSpan;
        }

        // Try to parse the XML schema duration (also ISO 8601 duration)
        try {
            return XmlSchemaUtils.ParseDuration(value);
        } catch {
            // ignore
        }

        throw new EmplyException($"Invalid interval specified: {value}");

    }

}