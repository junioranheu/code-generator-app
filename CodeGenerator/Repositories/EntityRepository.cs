using System.Text;
using CodeGenerator.Console.Consts;
using CodeGenerator.Console.Enums;
using CodeGenerator.Console.Models;
using static CodeGenerator.Console.Utils.Fixtures.Get;

namespace CodeGenerator.Console.Repositories;

public sealed class EntityRepository
{
    public static List<Content> GenerateEntity(string solutionName, string rootPath, string className, List<string> props, bool isPKGuid)
    {
        ExtensionsEnum extension = ExtensionsEnum.CS;
        ContentDirectoryEnum contentDirectory = ContentDirectoryEnum.Entity;

        List<Content> content =
        [
            new (
                value: GenerateContent(solutionName, className, props, isPKGuid),
                contentDirectory,
                extension,
                solutionName,
                fileFinalPath: GetFinalFilePath(solutionName, rootPath, fileName:className, contentDirectory, extension)
            )
        ];

        return content;
    }

    public static string GenerateContent(string solutionName, string className, List<string> props, bool isPKGuid, bool isInput = false, bool isOutput = false)
    {
        StringBuilder content = new();
        string paramId = GetClassId(className, isPKGuid, isLowerCaseFirstLetter: false);
        bool isNormalEntity = (!isInput && !isOutput);

        if (isNormalEntity)
        {
            content.AppendLine("using System.ComponentModel.DataAnnotations;");
            content.AppendLine();
        }

        if (!isNormalEntity)
        {
            content.AppendLine($"namespace {solutionName}.Application.UseCases.{GetStrPlural(className)}.Shared;");
            content.AppendLine();
        }

        content.AppendLine($"public sealed class {className}{(isInput ? "Input" : string.Empty)}{(isOutput ? "Output" : string.Empty)}");
        content.AppendLine("{");

        if (isNormalEntity)
        {
            content.AppendLine("[Key]");
            content.AppendLine($"public {paramId} {{ get; set; }}");
            content.AppendLine();

            if (!HasProperty(props, "Status"))
            {
                content.AppendLine("public bool Status { get; set; }");
                content.AppendLine();
            }
        }
         
        GenerateCustomTextStringBuilderByProps(stringBuilder: content, props, $"{Misc.Tab}public REPLACE_VAR_TYPE REPLACE_VAR_NAME {{ get; set; }}", isInputOrOutput: isInput || isOutput);

        content.AppendLine("}");

        return GetIndentedCode(content.ToString());
    }

    private static bool HasProperty(List<string> props, string propertyName)
    {
        return props.Any(prop =>
        {
            string[] parts = prop.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 0 && parts[0].Equals(propertyName, StringComparison.OrdinalIgnoreCase);
        });
    }
}