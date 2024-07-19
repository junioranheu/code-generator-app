using CodeGenerator.Enums;
using CodeGenerator.Models;
using System.Text;
using static CodeGenerator.Utils.Fixtures.Generate;
using static CodeGenerator.Utils.Fixtures.Get;

namespace CodeGenerator.Repositories;

public class UseCaseRepository
{
    public static List<Content> GenerateUseCase(string solutionName, string rootPath, string useCaseName)
    {
        List<string> contentPathEnums = GenerateFolders(solutionName, rootPath, useCaseName);
        List<Content> content = GenerateContent(solutionName, rootPath, useCaseName, contentPathEnums);

        return content;
    }

    private static List<string> GenerateFolders(string solutionName, string rootPath, string useCaseName)
    {
        string mainFolderPath = Path.Combine(rootPath, $"{solutionName}.{GetEnumDesc(ContentDirectoryEnum.UseCase)}", GetStrPlural(useCaseName));
        GenerateFolder(solutionName, folderPath: mainFolderPath);

        List<string> contentPathEnums = GetEnumDescriptionOfAllItemsAndAssignInListStr<UseCaseEnum>();
        GenerateFolderByPathList(solutionName, mainFolderPath, paths: contentPathEnums);

        return contentPathEnums;
    }

    private static List<Content> GenerateContent(string solutionName, string rootPath, string useCaseName, List<string> contentPathEnums)
    {
        List<Content> content = [];

        foreach (var item in contentPathEnums)
        {
            ExtensionsEnum extension = ExtensionsEnum.CS;
            ContentDirectoryEnum contentDirectory = ContentDirectoryEnum.UseCase;
            string fileName = Path.Combine(GetStrPlural(useCaseName), item, $"{item}{useCaseName}{GetEnumDesc(extension)}");

            content.Add(new(
                value: CheckUseCaseEnumAndGenerateContent(item, solutionName, useCaseName),
                contentDirectory,
                extension,
                solutionName,
                fileFinalPath: GetFinalFilePath(solutionName, rootPath, fileName, contentDirectory: contentDirectory, extension)
            ));
        }

        return content;
    }

    private static string CheckUseCaseEnumAndGenerateContent(string useCaseType, string solutionName, string useCaseName)
    {
        if (useCaseType == GetEnumDesc(UseCaseEnum.Get))
        {
            return GenerateUseCase_Get(solutionName, useCaseName);
        }
        else if (useCaseType == GetEnumDesc(UseCaseEnum.GetAll))
        {
            return GenerateUseCase_GetAll(solutionName, useCaseName);
        }
        else if (useCaseType == GetEnumDesc(UseCaseEnum.Create))
        {
            return GenerateUseCase_Create(solutionName, useCaseName);
        }
        else if (useCaseType == GetEnumDesc(UseCaseEnum.Update))
        {
            return GenerateUseCase_Update(solutionName, useCaseName);
        }
        else if (useCaseType == GetEnumDesc(UseCaseEnum.Delete))
        {
            return GenerateUseCase_Delete(solutionName, useCaseName);
        }

        throw new NotImplementedException();
    }

    #region UseCases
    private static string GenerateUseCase_Get(string solutionName, string useCaseName)
    {
        StringBuilder classBuilder = new();
        classBuilder.AppendLine($"{nameof(GenerateUseCase_Get)} {useCaseName}");

        return classBuilder.ToString();
    }

    private static string GenerateUseCase_GetAll(string solutionName, string useCaseName)
    {
        StringBuilder classBuilder = new();
        classBuilder.AppendLine($"{nameof(GenerateUseCase_GetAll)} {useCaseName}");

        return classBuilder.ToString();
    }

    private static string GenerateUseCase_Create(string solutionName, string useCaseName)
    {
        StringBuilder classBuilder = new();
        classBuilder.AppendLine($"{nameof(GenerateUseCase_Create)} {useCaseName}");

        return classBuilder.ToString();
    }

    private static string GenerateUseCase_Update(string solutionName, string useCaseName)
    {
        StringBuilder classBuilder = new();
        classBuilder.AppendLine($"{nameof(GenerateUseCase_Update)} {useCaseName}");

        return classBuilder.ToString();
    }

    private static string GenerateUseCase_Delete(string solutionName, string useCaseName)
    {
        StringBuilder classBuilder = new();
        classBuilder.AppendLine($"{nameof(GenerateUseCase_Delete)} {useCaseName}");

        return classBuilder.ToString();
    }
    #endregion
}