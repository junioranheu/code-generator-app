using CodeGenerator.Consts;
using System.Text;
using static CodeGenerator.Utils.Fixtures.Get;

namespace CodeGenerator.Repositories;

public class ModelRepository
{
    public static string GenerateModel(string solutionName, string className, List<string> props)
    {
        StringBuilder classBuilder = new();

        classBuilder.AppendLine("using System;");
        classBuilder.AppendLine();

        classBuilder.AppendLine($"namespace {solutionName}.Domain.Entities;");
        classBuilder.AppendLine();

        classBuilder.AppendLine($"public class {className}");
        classBuilder.AppendLine("{");

        foreach (var prop in props)
        {
            string[] parts = prop.Split(' ');

            if (parts.Length == 2)
            {
                string attrName = parts[0];
                string attrType = parts[1];
                classBuilder.AppendLine($"{Misc.Tab}public {attrType} {attrName} {{ get; set; }}");
            }
        }

        classBuilder.AppendLine("}");

        return classBuilder.ToString();
    }

    public static List<string> GenerateModelProps(string classDefinition)
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