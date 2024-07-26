using CodeGenerator.Consts;
using CodeGenerator.Enums;
using CodeGenerator.Models;
using System.Text;
using static CodeGenerator.Utils.Fixtures.Get;

namespace CodeGenerator.Repositories;

public class EntityRepository
{
    public static List<Content> GenerateEntity(string solutionName, string rootPath, string className, string props, bool isFKGuid = true)
    {
        ExtensionsEnum extension = ExtensionsEnum.CS;
        ContentDirectoryEnum contentDirectory = ContentDirectoryEnum.Entity;

        List<Content> content =
        [
            new (
                value: GenerateContent(solutionName, className, props: GenerateEntityProps(props), isFKGuid),
                contentDirectory,
                extension,
                solutionName,
                fileFinalPath: GetFinalFilePath(solutionName, rootPath, fileName:className, contentDirectory, extension)
            )
        ];

        return content;
    }

    private static string GenerateContent(string solutionName, string className, List<string> props, bool isFKGuid)
    {
        StringBuilder content = new();

        content.AppendLine("using System.ComponentModel.DataAnnotations;");
        content.AppendLine();

        content.AppendLine($"namespace {solutionName}.Domain.Entities;");
        content.AppendLine();

        content.AppendLine($"public sealed class {className}");
        content.AppendLine("{");

        content.AppendLine($"{Misc.Tab}[Key]");
        content.AppendLine($"{Misc.Tab}public {(isFKGuid ? "Guid" : "int")} {className}Id {{ get; set; }}");

        foreach (var prop in props)
        {
            string[] parts = prop.Split(' ');

            if (parts.Length == 2)
            {
                string attrName = parts[0];
                string attrType = parts[1];

                content.AppendLine();
                content.AppendLine($"{Misc.Tab}public {attrType} {attrName} {{ get; set; }}");
            }
        }

        content.AppendLine("}");

        return content.ToString();
    }

    private static List<string> GenerateEntityProps(string classDefinition)
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
}