using CodeGenerator.Console.Enums;
using System.Globalization;

namespace CodeGenerator.Console.Utils.Fixtures;

public static class Format
{
    public static string FormatDateTime(DateTime dateTime, DateTimeFormat format, string? customFormat = null)
    {
        switch (format)
        {
            case DateTimeFormat.ShortDate:
                return dateTime.ToString("d", CultureInfo.InvariantCulture);
            case DateTimeFormat.LongDate:
                return dateTime.ToString("D", CultureInfo.InvariantCulture);
            case DateTimeFormat.FullDateTime:
                return dateTime.ToString("F", CultureInfo.InvariantCulture);
            case DateTimeFormat.RFC1123:
                return dateTime.ToString("R", CultureInfo.InvariantCulture);
            case DateTimeFormat.SortableDateTime:
                return dateTime.ToString("s", CultureInfo.InvariantCulture);
            case DateTimeFormat.UniversalSortableDateTime:
                return dateTime.ToString("u", CultureInfo.InvariantCulture);
            case DateTimeFormat.Custom:
                if (string.IsNullOrEmpty(customFormat))
                {
                    throw new ArgumentException("Custom format string cannot be null or empty when using Custom format.");
                }

                return dateTime.ToString(customFormat, CultureInfo.InvariantCulture);
            case DateTimeFormat.FileName:
                return dateTime.ToString("dd-MM-yyyy HH-mm");
            case DateTimeFormat.CompleteDateTime:
                return dateTime.ToString("dd/MM/yyyy HH:mm:ss");                
            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
    }
}