using CodeGenerator.Consts;
using System.Text;
using static CodeGenerator.Utils.Fixtures.Get;

namespace CodeGenerator.Repositories;

public class EntityRepository
{
    public static string GenerateEntity(string solutionName, string className, List<string> props, bool isFKGuid = true)
    {
        StringBuilder classBuilder = new();

        classBuilder.AppendLine("using System.ComponentModel.DataAnnotations;");
        classBuilder.AppendLine();

        classBuilder.AppendLine($"namespace {solutionName}.Domain.Entities;");
        classBuilder.AppendLine();

        classBuilder.AppendLine($"public sealed class {className}");
        classBuilder.AppendLine("{");

        classBuilder.AppendLine($"{Misc.Tab}[Key]");
        classBuilder.AppendLine($"{Misc.Tab}public {(isFKGuid ? "Guid" : "int")} {className}Id {{ get; set; }}");

        foreach (var prop in props)
        {
            string[] parts = prop.Split(' ');

            if (parts.Length == 2)
            {
                string attrName = parts[0];
                string attrType = parts[1];

                classBuilder.AppendLine();
                classBuilder.AppendLine($"{Misc.Tab}public {attrType} {attrName} {{ get; set; }}");
            }
        }

        classBuilder.AppendLine("}");

        return classBuilder.ToString();
    }

    public static List<string> GenerateEntityProps(string classDefinition)
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