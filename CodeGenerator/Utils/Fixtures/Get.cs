using System.ComponentModel;
using System.Reflection;
using System.Text;
using CodeGenerator.Console.Enums;
using TimeZoneConverter;
using static CodeGenerator.Console.Utils.Fixtures.Format;
using static CodeGenerator.Console.Utils.Fixtures.Delete;

namespace CodeGenerator.Console.Utils.Fixtures;

public static class Get
{
    public static DateTime GetDateTime()
    {
        TimeZoneInfo timeZone = TZConvert.GetTimeZoneInfo("E. South America Standard Time");
        return TimeZoneInfo.ConvertTime(DateTime.UtcNow, timeZone);
    }

    public static string GetMainFolderName(string solutionName, Guid guid)
    {
        return $"{solutionName} {FormatDateTime(GetDateTime(), DateTimeFormat.FileName)} {guid}";
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

    public static string GetStringBeforeText(string input, string searchText)
    {
        ReadOnlySpan<char> inputSpan = input.AsSpan();
        ReadOnlySpan<char> searchTextSpan = searchText.AsSpan();

        int index = inputSpan.IndexOf(searchTextSpan);

        if (index != -1)
        {
            return inputSpan[..index].ToString();
        }

        return input.Replace("\\", "/");
    }

    public static string GetFinalFilePath(string solutionName, string rootPath, string fileName, ContentDirectoryEnum contentDirectory, ExtensionsEnum extension)
    {
        string pathNormalized = Path.Combine(rootPath, $"{solutionName}.{GetEnumDesc(contentDirectory)}");
        string pathFinalFile = Path.Combine(pathNormalized, $"{fileName}{GetEnumDesc(extension)}");

        return pathFinalFile;
    }

    public static List<string> GetEntityPropsSplitted(string classDefinition, string rootPath)
    {
        List<string> props = [];
        string[] parts = classDefinition.Trim().Split(' ');

        if (parts.Length % 2 != 0)
        {
            DeleteFolder(rootPath);
            throw new ArgumentException("Properties do not match. The entities properties can not be odd!");
        }

        for (int i = 0; i < parts.Length; i += 2)
        {
            if (i + 1 < parts.Length)
            {
                string propName = GetStrCapitalizedFirstLetter(parts[i]);
                string propType = parts[i + 1];

                if (string.IsNullOrEmpty(propName) || string.IsNullOrEmpty(propType))
                {
                    throw new Exception();
                }

                props.Add($"{propName} {propType}");
            }
        }

        return props;
    }

    /// <summary>
    /// string[] props = { "Name string", "Age int", "Email string" };
    /// GenerateCustomTextStringBuilderByProps(stringBuilder, props, $"This is a attrName property named attrType.");
    /// </summary>
    public static void GenerateCustomTextStringBuilderByProps(StringBuilder stringBuilder, List<string> props, string customText, bool isInputOrOutput)
    {
        int max = props.Count;
        int i = 1;

        foreach (var prop in props)
        {
            string[] parts = prop.Split(' ');

            if (parts.Length == 2)
            {
                string attrName = parts[0];
                string attrType = parts[1];

                string formattedText = string.Empty;

                bool isCommonType = GetIsCommonTypeName(attrType);

                if (!isCommonType)
                {
                    stringBuilder.AppendLine($"[ForeignKey(nameof({attrName}))]");
                    stringBuilder.AppendLine($"public int {attrName}Id {{ get; set; }}");

                    formattedText = customText.Replace("REPLACE_VAR_NAME", GetStrPlural(attrName)).Replace("REPLACE_VAR_TYPE", $"{attrType}?");
                }
                else
                {
                    if (!isInputOrOutput)
                    {
                        formattedText = customText.Replace("REPLACE_VAR_NAME", attrName).Replace("REPLACE_VAR_TYPE", attrType);
                    }
                    else
                    {
                        formattedText = customText.Replace("REPLACE_VAR_NAME", attrName).Replace("REPLACE_VAR_TYPE", $"{attrType}?");
                    }
                }

                stringBuilder.AppendLine(formattedText);

                if (i < max)
                {
                    stringBuilder.AppendLine();
                }

                i++;
            }
            else
            {
                throw new ArgumentException("Properties do not match");
            }
        }
    }

    /// <summary>
    /// string[] props = { "Name string", "Age int", "Email string" };
    /// GenerateWhereQueriesByProps(stringBuilder, props);
    /// </summary>
    public static StringBuilder GenerateWhereQueriesByProps(StringBuilder stringBuilder, List<string> props, bool hasInputPrefix = false)
    {
        List<string> conditions = [];

        foreach (var prop in props)
        {
            string[] parts = prop.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 2)
            {
                string attrName = parts[0];
                string attrType = parts[1].TrimEnd('?');

                if (attrType.Equals("bool", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string valueName = hasInputPrefix ? $"input.{attrName}" : GetStringLowerCaseFirstLetter(attrName);

                string condition = IsStringType(attrType)
                    ? $"string.IsNullOrEmpty({valueName}) || x.{attrName} == {valueName}"
                    : IsNumericType(attrType)
                        ? $"{valueName} <= 0 || x.{attrName} == {valueName}"
                        : $"x.{attrName} == {valueName}";

                conditions.Add(condition);
            }
        }

        for (int i = 0; i < conditions.Count; i++)
        {
            string suffix = i < conditions.Count - 1 ? " &&" : string.Empty;
            stringBuilder.AppendLine($"{conditions[i]}{suffix}");
        }

        return stringBuilder;
    }

    private static bool IsStringType(string type)
    {
        return type.Equals("string", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsNumericType(string type)
    {
        return type is "byte" or "short" or "int" or "long" or "float" or "double" or "decimal";
    }

    /// <summary>
    /// string[] props = { "Name string", "Age int", "Email string" };
    /// string params = GenerateParametersStringByProps(props, customText);
    /// </summary>
    public static string GenerateParametersStringByProps(List<string> props, bool addQuestionMark = false, bool getBothNameAndType = true)
    {
        StringBuilder content = new();

        foreach (var prop in props)
        {
            string[] parts = prop.Split(' ');

            if (parts.Length == 2)
            {
                string attrName = parts[0];
                string attrType = parts[1];

                if (getBothNameAndType)
                {
                    content.Append($"{attrType}{(addQuestionMark ? "?" : string.Empty)} {GetStringLowerCaseFirstLetter(attrName)}, ");
                }
                else
                {
                    content.Append($"{GetStringLowerCaseFirstLetter(attrName)}, ");
                }
            }
        }

        string contentStr = content.ToString();

        if (contentStr.EndsWith(", "))
        {
            contentStr = contentStr[..contentStr.LastIndexOf(", ")];
        }

        return contentStr;
    }

    public static string GetStringLowerCaseFirstLetter(string input)
    {
        if (string.IsNullOrEmpty(input) || char.IsLower(input[0]))
        {
            return input;
        }

        return char.ToLower(input[0]) + input[1..];
    }

    private const string getIndentedCode_Bracket = "}";
    private const string getIndentedCode_Bracket2 = "{";
    public static string GetIndentedCode(string code, int spacesPerIndent = 4)
    {
        StringBuilder indentedCode = new();
        string[] lines = code.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        int indentLevel = 0;
        int continuationIndent = 0;
        string indentString = new(' ', spacesPerIndent);

        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();

            if (trimmedLine.StartsWith(getIndentedCode_Bracket))
            {
                indentLevel--;
            }

            bool closesContinuation = trimmedLine.StartsWith(')');

            if (closesContinuation)
            {
                continuationIndent = 1;
            }

            if (trimmedLine.StartsWith(getIndentedCode_Bracket))
            {
                continuationIndent = 0;
            }

            if (!string.IsNullOrWhiteSpace(trimmedLine))
            {
                int totalIndent = Math.Max(0, indentLevel + continuationIndent) * indentString.Length;
                indentedCode.AppendLine(new string(' ', totalIndent) + trimmedLine);
            }
            else
            {
                indentedCode.AppendLine();
            }

            if (trimmedLine.EndsWith('.'))
            {
                continuationIndent = Math.Max(continuationIndent, 1);
            }
            else if (trimmedLine.EndsWith("=>"))
            {
                continuationIndent++;
            }

            if (closesContinuation && !trimmedLine.EndsWith('.'))
            {
                continuationIndent = 0;
            }

            if (trimmedLine.EndsWith(getIndentedCode_Bracket2))
            {
                indentLevel++;
            }
        }

        return indentedCode.ToString();
    }

    /// <summary>
    /// string[] props = { "Name", "Age", "Email" };
    /// GenerateCustomTextStringBuilderByListOfStrings(stringBuilder, props, $"There you go: REPLACE_VAR.");
    /// </summary>
    public static void GenerateCustomTextStringBuilderByListOfStrings(StringBuilder stringBuilder, List<string> props, string customText)
    {
        foreach (var prop in props)
        {
            string formattedText = customText.Replace("REPLACE_VAR_CAPITALIZEDFIRSTLETTER", GetStrCapitalizedFirstLetter(prop)).Replace("REPLACE_VAR", prop);
            stringBuilder.AppendLine(formattedText);
        }
    }

    public static string GetClassId(string className, bool isPKGuid, bool isLowerCaseFirstLetter)
    {
        return $"{(isPKGuid ? "Guid" : "int")} {(isLowerCaseFirstLetter ? GetStringLowerCaseFirstLetter(className) : className)}Id";
    }

    public static string GetClassIdWithoutType(string className, bool isLowerCaseFirstLetter)
    {
        return $"{(isLowerCaseFirstLetter ? GetStringLowerCaseFirstLetter(className) : className)}Id";
    }

    public static string GetSolutionName()
    {
        string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        DirectoryInfo directoryInfo = new(currentDirectory);

        while (directoryInfo is not null)
        {
            var solutionFile = directoryInfo.GetFiles("*.sln").FirstOrDefault();

            if (solutionFile is not null)
            {
                return Path.GetFileNameWithoutExtension(solutionFile.Name);
            }

            directoryInfo = directoryInfo.Parent!;
        }

        return string.Empty;
    }

    public static string GetProjectDirectory(RequestTypeEnum requestType)
    {
        if (requestType == RequestTypeEnum.API)
        {
            return "Zips/";
        }

        return "Zips/";
    }

    public static bool GetIsCommonTypeName(string input)
    {
        HashSet<string> CommonTypeNames =
        [
            "string",
            "bool",
            "int",
            "double",
            "float",
            "DateTime",
            "char",
            "byte",
            "short",
            "long",
            "decimal",
            "uint",
            "ulong",
            "ushort",
            "sbyte",
            "Guid"
        ];

        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        string typeName = input.Trim();

        // Verifica primeiro o nome exato (ex.: "int");
        if (CommonTypeNames.Contains(typeName))
        {
            return true;
        }

        // Se terminar com '?', verificar somente o caso em que o tipo base é comum (ex.: "int?");
        if (typeName.EndsWith('?'))
        {
            string baseType = typeName[..^1].Trim();
            return CommonTypeNames.Contains(baseType);
        }

        return false;
    }

    public static byte[] GetArrayOfBytesFromPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentException("File path cannot be null or empty", nameof(path));
        }

        if (!File.Exists(path))
        {
            throw new FileNotFoundException("File not found", path);
        }

        return File.ReadAllBytes(path);
    }

    public static string FirstCharToUpper(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        return $"{char.ToUpper(input[0])}{input[1..]}";
    }

    /// <summary>
    /// Verifica se a lista de propriedades contém uma propriedade com o nome informado.
    /// Espera-se que cada item em <paramref name="props"/> esteja no formato "Name Type".
    /// A comparação é case-insensitive e analisa apenas o primeiro token (nome) de cada definição.
    /// </summary>
    /// <param name="props">Lista de propriedades (ex.: "Name string").</param>
    /// <param name="propertyName">Nome da propriedade a procurar.</param>
    /// <returns>True se existir uma propriedade com o nome informado; caso contrário, false.</returns>
    public static bool ContainsProperty(List<string> props, string propertyName)
    {
        return props.Any(prop =>
        {
            string[] parts = prop.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 0 && parts[0].Equals(propertyName, StringComparison.OrdinalIgnoreCase);
        });
    }
}