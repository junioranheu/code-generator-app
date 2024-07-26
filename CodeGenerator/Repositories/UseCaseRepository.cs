using System.Text;
using CodeGenerator.Enums;
using CodeGenerator.Models;
using static CodeGenerator.Utils.Fixtures.Generate;
using static CodeGenerator.Utils.Fixtures.Get;

namespace CodeGenerator.Repositories;

public class UseCaseRepository
{
    public static List<Content> GenerateUseCase(string solutionName, string rootPath, string useCaseName, List<string> props)
    {
        List<string> contentPathEnums = GenerateFolders(solutionName, rootPath, useCaseName);
        List<Content> content = GenerateContent(solutionName, rootPath, useCaseName, props, contentPathEnums);

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

    private static List<Content> GenerateContent(string solutionName, string rootPath, string useCaseName, List<string> props, List<string> contentPathEnums)
    {
        List<Content> content = [];

        foreach (string item in contentPathEnums)
        {
            ExtensionsEnum extension = ExtensionsEnum.CS;
            ContentDirectoryEnum contentDirectory = ContentDirectoryEnum.UseCase;
            string fileName = GetFileName(useCaseName, item, isInterface: false);
            string interfaceFileName = GetFileName(useCaseName, item, isInterface: true);

            // Use Case;
            content.Add(new(
                value: CheckUseCaseEnumAndGenerateContent(item, solutionName, useCaseName, props),
                contentDirectory,
                extension,
                solutionName,
                fileFinalPath: GetFinalFilePath(solutionName, rootPath, fileName, contentDirectory: contentDirectory, extension)
            ));

            // Interface;
            content.Add(new(
                value: GenerateInterface(item, solutionName, useCaseName, props),
                contentDirectory,
                extension,
                solutionName,
                fileFinalPath: GetFinalFilePath(solutionName, rootPath, interfaceFileName, contentDirectory: contentDirectory, extension)
            ));
        }

        return content;
    }

    private static string GetFileName(string useCaseName, string item, bool isInterface)
    {
        return Path.Combine(GetStrPlural(useCaseName), item, $"{(isInterface ? "I" : string.Empty)}{item}{useCaseName}");
    }

    private static string CheckUseCaseEnumAndGenerateContent(string useCaseType, string solutionName, string useCaseName, List<string> props)
    {
        if (useCaseType == GetEnumDesc(UseCaseEnum.Get))
        {
            return GenerateUseCase_Get(solutionName, useCaseName, props);
        }
        else if (useCaseType == GetEnumDesc(UseCaseEnum.GetAll))
        {
            return GenerateUseCase_GetAll(solutionName, useCaseName, props);
        }
        else if (useCaseType == GetEnumDesc(UseCaseEnum.Create))
        {
            return GenerateUseCase_Create(solutionName, useCaseName, props);
        }
        else if (useCaseType == GetEnumDesc(UseCaseEnum.Update))
        {
            return GenerateUseCase_Update(solutionName, useCaseName, props);
        }
        else if (useCaseType == GetEnumDesc(UseCaseEnum.Delete))
        {
            return GenerateUseCase_Delete(solutionName, useCaseName, props);
        }

        throw new NotImplementedException();
    }

    #region UseCases
    private static string GenerateUseCase_Get(string solutionName, string useCaseName, List<string> props)
    {
        StringBuilder content = new();

        content.AppendLine($@"
            using {solutionName}.Domain.Entities;
            using {solutionName}.Infrastructure.Data;
            using Microsoft.EntityFrameworkCore;

            namespace {solutionName}.Application.UseCases.{useCaseName}.Get;

            public sealed class Get{useCaseName}(CONTEXT context) : IGet{useCaseName}
            {{
                private readonly CONTEXT _context = context;

                public async Task<{useCaseName}?> Execute(int? id, string? xxx)
                {{
                    var linq = await _context.Ocorrencias.
                               Where(x =>
                                  x.Status == true &&
                                  (id == null || x.Id == id) &&
                                  (string.IsNullOrEmpty(xxx) || x.XXX == xxx)
                               ).
                               AsNoTracking().FirstOrDefaultAsync();

                    return linq;
                }}
            }}"
        );

        return content.ToString();
    }

    private static string GenerateUseCase_GetAll(string solutionName, string useCaseName, List<string> props)
    {
        StringBuilder content = new();
        content.AppendLine($"{nameof(GenerateUseCase_GetAll)} {useCaseName}");

        return content.ToString();
    }

    private static string GenerateUseCase_Create(string solutionName, string useCaseName, List<string> props)
    {
        StringBuilder content = new();
        content.AppendLine($"{nameof(GenerateUseCase_Create)} {useCaseName}");

        return content.ToString();
    }

    private static string GenerateUseCase_Update(string solutionName, string useCaseName, List<string> props)
    {
        StringBuilder content = new();
        content.AppendLine($"{nameof(GenerateUseCase_Update)} {useCaseName}");

        return content.ToString();
    }

    private static string GenerateUseCase_Delete(string solutionName, string useCaseName, List<string> props)
    {
        StringBuilder content = new();
        content.AppendLine($"{nameof(GenerateUseCase_Delete)} {useCaseName}");

        return content.ToString();
    }

    private static string GenerateInterface(string useCaseType, string solutionName, string useCaseName, List<string> props)
    {
        StringBuilder content = new();

        content.AppendLine(@$"
            using {solutionName}.Backend.Domain.Entities;

            namespace {solutionName}.Application.UseCases.{useCaseName}.{useCaseType};

            public interface I{useCaseType}{useCaseName}
            {{
                Task<{useCaseName}?> Execute(int? id, string? xxx);
            }}
        ");

        return content.ToString();
    }
    #endregion
}