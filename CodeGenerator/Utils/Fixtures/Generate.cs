using System.Text;
using static CodeGenerator.Utils.Fixtures.Get;

namespace CodeGenerator.Utils.Fixtures;

public static class Generate
{
    public static List<string> GenerateAttributes(string classDefinition)
    {
        List<string> attributes = [];
        string[] parts = classDefinition.Split(' ');

        for (int i = 0; i < parts.Length; i += 2)
        {
            if (i + 1 < parts.Length)
            {
                string propName = GetStrCapitalizedFirstLetter(parts[i]);
                string propType = parts[i + 1];

                attributes.Add($"{propName} {propType}");
            }
        }

        return attributes;
    }

    public static string GenerateModel(string className, List<string> attributes)
    {
        StringBuilder classBuilder = new();

        classBuilder.AppendLine("using System;");
        classBuilder.AppendLine();
        classBuilder.AppendLine($"public class {className}");
        classBuilder.AppendLine("{");

        foreach (var attribute in attributes)
        {
            string[] parts = attribute.Split(' ');

            if (parts.Length == 2)
            {
                string attrName = parts[0];
                string attrType = parts[1];
                classBuilder.AppendLine($"    public {attrType} {attrName} {{ get; set; }}");
            }
        }

        classBuilder.AppendLine("}");

        return classBuilder.ToString();
    }

    public static void GenerateFile(string fileName, string content, string extension)
    {
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string folderPath = Path.Combine(desktopPath, GetFileName());

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string filePath = Path.Combine(folderPath, $"{fileName}{extension}");
        File.WriteAllText(filePath, content);
    }
}