using CodeGenerator.Consts;
using CodeGenerator.Enums;
using System.Text;
using static CodeGenerator.Utils.Fixtures.Generate;
using static CodeGenerator.Utils.Fixtures.Get;

namespace CodeGenerator.Repositories;

public class UseCaseRepository
{
    public static (string content, string mainFolderPath) GenerateUseCase(string solutionName, string rootPath, string useCaseName)
    {
        string mainFolderPath = Path.Combine(rootPath, $"{solutionName}.{GetEnumDesc(ContentPathEnum.UseCase)}", useCaseName);
        GenerateFolder(solutionName, folderPath: mainFolderPath);

        StringBuilder classBuilder = new();

        classBuilder.AppendLine("using System.ComponentModel.DataAnnotations;");
        classBuilder.AppendLine();

        classBuilder.AppendLine($"namespace {solutionName}.Domain.Entities;");
        classBuilder.AppendLine();

        classBuilder.AppendLine($"public sealed class {useCaseName}");
        classBuilder.AppendLine("{");

        classBuilder.AppendLine($"{Misc.Tab}[Key]");

        //foreach (var prop in props)
        //{
        //    string[] parts = prop.Split(' ');

        //    if (parts.Length == 2)
        //    {
        //        string attrName = parts[0];
        //        string attrType = parts[1];

        //        classBuilder.AppendLine();
        //        classBuilder.AppendLine($"{Misc.Tab}public {attrType} {attrName} {{ get; set; }}");
        //    }
        //}

        classBuilder.AppendLine("}");

        return (classBuilder.ToString(), mainFolderPath);
    }
}