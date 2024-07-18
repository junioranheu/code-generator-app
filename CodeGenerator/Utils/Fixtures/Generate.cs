using CodeGenerator.Consts;
using CodeGenerator.Enums;
using System.Text;
using static CodeGenerator.Utils.Fixtures.Format;
using static CodeGenerator.Utils.Fixtures.Get;

namespace CodeGenerator.Utils.Fixtures;

public static class Generate
{
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

    public static string GenerateModel(string className, List<string> props)
    {
        StringBuilder classBuilder = new();

        classBuilder.AppendLine("using System;");
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

    public static void GenerateFile(string fileName, string content, string extension)
    {
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string folderPath = Path.Combine(desktopPath, GetFileName());

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string filePath = Path.Combine(folderPath, $"{fileName}{extension}");
        File.WriteAllText(filePath, content.TrimEnd());

        Console.WriteLine($"{FormatDateTime(GetDateTime(), DateTimeFormat.CompleteDateTime)} | File {fileName}{extension} generated successfully!");
    }
}