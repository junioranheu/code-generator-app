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

        List<string> contentPathEnums = ["a", "b", "c"];
        GenerateFolderByPathList(solutionName, mainFolderPath, paths: contentPathEnums);

        StringBuilder classBuilder = new();

        classBuilder.AppendLine("using System.ComponentModel.DataAnnotations;");
        classBuilder.AppendLine();

        classBuilder.AppendLine($"namespace {solutionName}.Domain.Entities;");
        classBuilder.AppendLine();

        classBuilder.AppendLine($"public sealed class {useCaseName}");
        classBuilder.AppendLine("{");

        classBuilder.AppendLine($"{Misc.Tab}[Key]");

        classBuilder.AppendLine("}");

        return (classBuilder.ToString(), mainFolderPath);
    }
}