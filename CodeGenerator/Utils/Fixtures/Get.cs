﻿using System.ComponentModel;
using System.Reflection;
using System.Text;
using CodeGenerator.Consts;
using CodeGenerator.Enums;
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

    public static string GetLog(string msg, LogEnum? type = LogEnum.Success)
    {
        ConsoleColor originalColor = ConsoleColor.Gray;

        Console.ForegroundColor = type switch
        {
            LogEnum.Success => ConsoleColor.Cyan,
            LogEnum.Fail => ConsoleColor.Red,
            LogEnum.Warning => ConsoleColor.Yellow,
            LogEnum.Info => originalColor,
            _ => originalColor,
        };

        string final = $"{FormatDateTime(GetDateTime(), DateTimeFormat.CompleteDateTime)} | {msg}";
        Console.WriteLine(final);
        Console.ForegroundColor = originalColor;

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

    public static string GetStrPlural(string singular, bool isEnglish = true)
    {
        if (string.IsNullOrEmpty(singular))
        {
            throw new ArgumentException($"The param {nameof(singular)} in {nameof(GetStrPlural)} can't be null");
        }

        return isEnglish ? PluralizeEnglish(singular) : PluralizePortuguese(singular);

        static string PluralizeEnglish(string singular)
        {
            if (singular.EndsWith('y') && singular.Length > 1 && !IsVowel(singular[^2]))
            {
                return string.Concat(singular.AsSpan(0, singular.Length - 1), "ies");
            }

            if (singular.EndsWith('s') || singular.EndsWith("sh") || singular.EndsWith("ch") || singular.EndsWith('x') || singular.EndsWith('z'))
            {
                return singular + "es";
            }

            if (singular.EndsWith('f'))
            {
                return string.Concat(singular.AsSpan(0, singular.Length - 1), "ves");
            }

            if (singular.EndsWith("fe"))
            {
                return string.Concat(singular.AsSpan(0, singular.Length - 2), "ves");
            }

            return singular + "s";
        }

        static bool IsVowel(char c)
        {
            return "aeiouAEIOU".Contains(c);
        }

        static string PluralizePortuguese(string singular)
        {
            string[] terminacoesEs = ["r", "z", "s", "l", "ão"];
            string[] terminacoesIs = ["m"];

            foreach (var terminação in terminacoesEs)
            {
                if (singular.EndsWith(terminação))
                {
                    return singular + "es";
                }
            }

            foreach (var terminação in terminacoesIs)
            {
                if (singular.EndsWith(terminação))
                {
                    return string.Concat(singular.AsSpan(0, singular.Length - 1), "ns");
                }
            }

            return singular + "s";
        }
    }

    public static string GetStringAfterText(string input, string searchText)
    {
        ReadOnlySpan<char> inputSpan = input.AsSpan();
        ReadOnlySpan<char> searchTextSpan = searchText.AsSpan();

        int index = inputSpan.IndexOf(searchTextSpan);

        if (index != -1)
        {
            return inputSpan[(index + searchTextSpan.Length)..].ToString();
        }

        return input.Replace("\\", "/");
    }

    public static string GetFinalFilePath(string solutionName, string rootPath, string fileName, ContentDirectoryEnum contentDirectory, ExtensionsEnum extension)
    {
        string pathNormalized = Path.Combine(rootPath, $"{solutionName}.{GetEnumDesc(contentDirectory)}");
        string pathFinalFile = Path.Combine(pathNormalized, $"{fileName}{GetEnumDesc(extension)}");

        return pathFinalFile;
    }

    public static List<string> GetEntityPropsSplitted(string classDefinition)
    {
        List<string> props = [];
        string[] parts = classDefinition.Split(' ');

        for (int i = 0; i < parts.Length; i += 2)
        {
            if (i + 1 < parts.Length)
            {
                string propName = GetStrCapitalizedFirstLetter(parts[i]);
                string propType = parts[i + 1];

                props.Add($"{propName} {propType}");
            }
        }

        return props;
    }

    /// <summary>
    /// string[] props = { "Name string", "Age int", "Email string" };
    /// GenerateCustomTextStringBuilderByProps(stringBuilder, props, $"This is a attrName property named attrType.");
    /// </summary>
    public static StringBuilder GenerateCustomTextStringBuilderByProps(StringBuilder stringBuilder, List<string> props, string customText, bool isLowAttrName = false) 
    {
        foreach (var prop in props)
        {
            string[] parts = prop.Split(' ');

            if (parts.Length == 2)
            {
                string attrName = parts[0];
                string attrType = parts[1];

                string formattedText = customText.Replace("attrName", attrName).Replace("attrType", attrType);
                stringBuilder.AppendLine(formattedText);
            }
        }

        return stringBuilder;
    }

    /// <summary>
    /// string[] props = { "Name string", "Age int", "Email string" };
    /// GenerateWhereQueriesByProps(stringBuilder, props);
    /// </summary>
    public static StringBuilder GenerateWhereQueriesByProps(StringBuilder stringBuilder, List<string> props, bool hasInputPrefix = false)
    {
        int i = 0;
        int max = props.Count;

        foreach (var prop in props)
        {
            i++;
            string[] parts = prop.Split(' ');

            if (parts.Length == 2)
            {
                string attrName = parts[0];
                string attrNameLowerCase = GetStringLowerFirstLetter(attrName);

                stringBuilder.AppendLine($"(string.IsNullOrEmpty({(hasInputPrefix ? "input." : string.Empty)}{attrNameLowerCase}) || x.{attrName} == {attrNameLowerCase}) {(i < max ? "&&" : string.Empty)}");
            }
        }

        return stringBuilder;
    }

    /// <summary>
    /// string[] props = { "Name string", "Age int", "Email string" };
    /// string params = GenerateParametersStringByProps(props, customText);
    /// </summary>
    public static string GenerateParametersStringByProps(List<string> props)
    {
        StringBuilder content = new();

        foreach (var prop in props)
        {
            string[] parts = prop.Split(' ');

            if (parts.Length == 2)
            {
                string attrName = parts[0];
                string attrType = parts[1];

                content.Append($"{attrType} {GetStringLowerFirstLetter(attrName)}, ");
            }
        }

        string contentStr = content.ToString();

        if (contentStr.EndsWith(", "))
        {
            contentStr = contentStr[..contentStr.LastIndexOf(", ")];
        }

        return contentStr;
    }

    public static string GetStringLowerFirstLetter(string input)
    {
        if (string.IsNullOrEmpty(input) || char.IsLower(input[0]))
        {
            return input;
        }

        return char.ToLower(input[0]) + input[1..];
    }

    private const string getIndentedCode_Bracket = "}";
    private const string getIndentedCode_Bracket2 = "{";
    private static readonly char[] getIndentedCode_Lines = new[] { '\r', '\n' };

    public static string GetIndentedCode(string code, int spacesPerIndent = 4)
    {
        StringBuilder indentedCode = new();
        string[] lines = code.Split(getIndentedCode_Lines, StringSplitOptions.None);
        int indentLevel = 0;
        string indentString = new(' ', spacesPerIndent);

        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();

            if (trimmedLine.StartsWith(getIndentedCode_Bracket))
            {
                indentLevel--;
            }

            if (!string.IsNullOrWhiteSpace(trimmedLine))
            {
                indentedCode.AppendLine(new string(' ', indentLevel * indentString.Length) + trimmedLine);
            }
            else
            {
                indentedCode.AppendLine();
            }

            if (trimmedLine.EndsWith(getIndentedCode_Bracket2))
            {
                indentLevel++;
            }
        }

        return indentedCode.ToString();
    }
}