using CodeGenerator.Enums;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using TimeZoneConverter;
using static CodeGenerator.Utils.Fixtures.Format;

namespace CodeGenerator.Utils.Fixtures;

public static class Get
{
    public static DateTime GetDateTime()
    {
        TimeZoneInfo timeZone = TZConvert.GetTimeZoneInfo("E. South America Standard Time");
        return TimeZoneInfo.ConvertTime(DateTime.UtcNow, timeZone);
    }

    public static string GetFileName(string solutionName)
    {
        return $"{solutionName} {FormatDateTime(GetDateTime(), DateTimeFormat.FileName)}";
    }

    public static string GetStrCapitalizedFirstLetter(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        StringBuilder sb = new(input);
        sb[0] = char.ToUpper(sb[0]);

        return sb.ToString();
    }

    public static string GetEnumDesc(Enum enumVal)
    {
        MemberInfo[] memInfo = enumVal.GetType().GetMember(enumVal.ToString());
        DescriptionAttribute? attribute = CustomAttributeExtensions.GetCustomAttribute<DescriptionAttribute>(memInfo[0]);

        return attribute?.Description ?? string.Empty;
    }

    public static string GetLog(string msg)
    {
        string final = $"{FormatDateTime(GetDateTime(), DateTimeFormat.CompleteDateTime)} | {msg}";
        Console.WriteLine(final);

        return final;
    }

    /// <summary>
    /// Retrieves descriptions from the specified enum type without hierarchical breakdown.
    /// </summary>
    /// <typeparam name="T">The enum type from which to retrieve descriptions. Must be an enum.</typeparam>
    /// <returns>A list of descriptions as strings.</returns>
    public static List<string> GetEnumDescriptionOfAllItemsAndAssignInListStr<T>() where T : Enum
    {
        var type = typeof(T);
        var descriptions = new HashSet<string>();
        var enumValues = Enum.GetValues(type).Cast<T>();

        foreach (var value in enumValues)
        {
            var field = type.GetField(value.ToString());
            var descriptionAttribute = field?.GetCustomAttribute<DescriptionAttribute>();

            if (descriptionAttribute != null)
            {
                descriptions.Add(descriptionAttribute.Description);
            }
        }

        return [.. descriptions.OrderBy(d => d)];
    }
}