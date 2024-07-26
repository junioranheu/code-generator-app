using System.Text;
using CodeGenerator.Consts;
using CodeGenerator.Enums;
using CodeGenerator.Models;
using static CodeGenerator.Utils.Fixtures.Get;

namespace CodeGenerator.Repositories;

public sealed class EntityRepository
{
    public static List<Content> GenerateEntity(string solutionName, string rootPath, string className, List<string> props, bool isFKGuid)
    {
        ExtensionsEnum extension = ExtensionsEnum.CS;
        ContentDirectoryEnum contentDirectory = ContentDirectoryEnum.Entity;

        List<Content> content =
        [
            new (
                value: GenerateContent(solutionName, className, props, isFKGuid),
                contentDirectory,
                extension,
                solutionName,
                fileFinalPath: GetFinalFilePath(solutionName, rootPath, fileName:className, contentDirectory, extension)
            )
        ];

        return content;
    }

    public static string GenerateContent(string solutionName, string className, List<string> props, bool isFKGuid, bool isInput = false, bool isOutput = false)
    {
        StringBuilder content = new();
        string paramId = GetClassId(className, isFKGuid, isLowerCaseFirstLetter: false);
        bool isNormalEntity = (!isInput && !isOutput);

        if (isNormalEntity)
        {
            content.AppendLine("using System.ComponentModel.DataAnnotations;");
            content.AppendLine();
        }

        content.AppendLine($"namespace {solutionName}.Domain.Entities;");
        content.AppendLine();

        content.AppendLine($"public sealed class {className}{(isInput ? "Input" : string.Empty)}{(isOutput ? "Output" : string.Empty)}");
        content.AppendLine("{");

        if (isNormalEntity)
        {
            content.AppendLine("[Key]");
            content.AppendLine($"public {paramId} {{ get; set; }}");
        }

        GenerateCustomTextStringBuilderByProps(stringBuilder: content, props, $"{Misc.Tab}public REPLACE_VAR_TYPE REPLACE_VAR_NAME {{ get; set; }}");

        content.AppendLine("}");

        return GetIndentedCode(content.ToString());
    }
}