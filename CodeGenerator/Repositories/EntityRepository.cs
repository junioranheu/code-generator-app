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
            content.AppendLine("using System.ComponentModel.DataAnnotations.Schema;");
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
            content.AppendLine();
        }

        GenerateCustomTextStringBuilderByProps(stringBuilder: content, props, $"{Misc.Tab}public REPLACE_VAR_TYPE REPLACE_VAR_NAME {{ get; set; }}");

        content.AppendLine("}");

        return GetIndentedCode(content.ToString());
    }
}