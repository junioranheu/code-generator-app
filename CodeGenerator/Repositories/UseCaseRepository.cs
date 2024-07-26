using System.Text;
using CodeGenerator.Consts;
using CodeGenerator.Enums;
using CodeGenerator.Models;
using static CodeGenerator.Utils.Fixtures.Generate;
using static CodeGenerator.Utils.Fixtures.Get;

namespace CodeGenerator.Repositories;

public class UseCaseRepository
{
    #region Main
    public static List<Content> GenerateUseCase(string solutionName, string rootPath, string useCaseName, List<string> props)
    {
        List<string> contentPathEnums = GenerateFolders(solutionName, rootPath, useCaseName);
        List<Content> finalContent = GenerateContent(solutionName, rootPath, useCaseName, props, contentPathEnums);
        GenerateDependencyInjection(finalContent, solutionName, rootPath, useCaseName, props);

        return finalContent;
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
        List<Content> finalContent = [];

        foreach (string item in contentPathEnums)
        {
            ExtensionsEnum extension = ExtensionsEnum.CS;
            ContentDirectoryEnum contentDirectory = ContentDirectoryEnum.UseCase;
            string fileName = GetFileName(useCaseName, item, isInterface: false);
            string interfaceFileName = GetFileName(useCaseName, item, isInterface: true);

            (string content, string parameters) = CheckUseCaseEnumAndGenerateContent(item, solutionName, useCaseName, props);

            if (string.IsNullOrEmpty(content))
            {
                continue;
            }

            // Use Case;
            finalContent.Add(new(
                value: content,
                contentDirectory,
                extension,
                solutionName,
                fileFinalPath: GetFinalFilePath(solutionName, rootPath, fileName, contentDirectory, extension)
            ));

            // Interface;
            finalContent.Add(new(
                value: GenerateInterface(item, solutionName, useCaseName, props, parameters),
                contentDirectory,
                extension,
                solutionName,
                fileFinalPath: GetFinalFilePath(solutionName, rootPath, interfaceFileName, contentDirectory, extension)
            ));
        }

        return finalContent;
    }

    private static void GenerateDependencyInjection(List<Content> finalContent, string solutionName, string rootPath, string useCaseName, List<string> props)
    {
        ExtensionsEnum extension = ExtensionsEnum.CS;
        ContentDirectoryEnum contentDirectory = ContentDirectoryEnum.UseCase;

        finalContent.Add(new(
            value: GenerateDependencyInjection(solutionName, useCaseName, props),
            contentDirectory,
            extension,
            solutionName,
            fileFinalPath: GetFinalFilePath(solutionName, rootPath, fileName: $"{GetStrPlural(useCaseName)}/DependencyInjection", contentDirectory, extension)
        ));
    }

    #endregion

    #region UseCases
    private static (string content, string parameters) GenerateUseCase_Get(string solutionName, string useCaseName, List<string> props)
    {
        StringBuilder content = new();
        string parameters = GenerateParametersStringByProps(props);

        content.AppendLine($@"using {solutionName}.Domain.Entities;
using {solutionName}.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace {solutionName}.Application.UseCases.{useCaseName}.Get;

public sealed class Get{useCaseName}({Misc.Context} context) : IGet{useCaseName}
{{
private readonly {Misc.Context} _context = context;

public async Task<{useCaseName}?> Execute({parameters})
{{
var linq = await _context.{GetStrPlural(useCaseName)}.
Where(x =>
x.Status == true &&"
);

GenerateWhereQueriesByProps(content, props);

content.AppendLine($@").AsNoTracking().FirstOrDefaultAsync();

return linq;
}}
}}");

        return (GetIndentedCode(content.ToString()), parameters);
    }

    private static (string content, string parameters) GenerateUseCase_GetAll(string solutionName, string useCaseName, List<string> props)
    {
        StringBuilder content = new();
        string parameters = $"PaginationInput pagination, {useCaseName}Input input";

        content.AppendLine($@"using {solutionName}.Application.UseCases.{GetStrPlural(useCaseName)}.Shared;
using {solutionName}.Application.UseCases.Shared;
using {solutionName}.Domain.Entities;
using {solutionName}.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace {solutionName}.Application.UseCases.{useCaseName}.GetAll;

public sealed class GetAll{useCaseName}({Misc.Context} context) : IGetAll{useCaseName}
{{
private readonly {Misc.Context} _context = context;

public async Task<(IEnumerable<{useCaseName}> linq, int count)> Execute({parameters})
{{
var linq = await _context.{GetStrPlural(useCaseName)}.
OrderBy(x => x.xxx).
Where(x =>
x.Status == true &&"
);

        GenerateWhereQueriesByProps(content, props, hasInputPrefix: true);

        content.AppendLine($@").AsNoTracking();

return await PagedQuery.Execute(query, pagination);
}}
}}");

        return (GetIndentedCode(content.ToString()), parameters);
    }

    private static (string content, string parameters) GenerateUseCase_Create(string solutionName, string useCaseName)
    {
        StringBuilder content = new();
        content.AppendLine($"{nameof(GenerateUseCase_Create)} {useCaseName}");

        return (content.ToString(), "");
    }

    private static (string content, string parameters) GenerateUseCase_Update(string solutionName, string useCaseName)
    {
        StringBuilder content = new();
        content.AppendLine($"{nameof(GenerateUseCase_Update)} {useCaseName}");

        return (content.ToString(), "");
    }

    private static (string content, string parameters) GenerateUseCase_Delete(string solutionName, string useCaseName)
    {
        StringBuilder content = new();
        content.AppendLine($"{nameof(GenerateUseCase_Delete)} {useCaseName}");

        return (content.ToString(), "");
    }
    #endregion

    #region Etc

    private static string GetFileName(string useCaseName, string item, bool isInterface)
    {
        return Path.Combine(GetStrPlural(useCaseName), item, $"{(isInterface ? "I" : string.Empty)}{item}{useCaseName}");
    }

    private static (string content, string parameters) CheckUseCaseEnumAndGenerateContent(string useCaseType, string solutionName, string useCaseName, List<string> props)
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
            return GenerateUseCase_Create(solutionName, useCaseName);
        }
        else if (useCaseType == GetEnumDesc(UseCaseEnum.Update))
        {
            return GenerateUseCase_Update(solutionName, useCaseName);
        }
        else if (useCaseType == GetEnumDesc(UseCaseEnum.Delete))
        {
            return GenerateUseCase_Delete(solutionName, useCaseName);
        } else if (useCaseType == GetEnumDesc(UseCaseEnum.Shared))
        {
            return (string.Empty, string.Empty);
        }

        throw new NotImplementedException();
    }

    private static string GenerateInterface(string useCaseType, string solutionName, string useCaseName, List<string> props, string parameters)
    {
        StringBuilder content = new();

        content.AppendLine(@$"using {solutionName}.Domain.Entities;

namespace {solutionName}.Application.UseCases.{useCaseName}.{useCaseType};

public interface I{useCaseType}{useCaseName}
{{
    Task<{useCaseName}?> Execute({parameters});
}}");

        return GetIndentedCode(content.ToString());
    }

    private static string GenerateDependencyInjection(string solutionName, string useCaseName, List<string> props)
    {
        StringBuilder content = new();
        string usings = $"using {solutionName}.Application.UseCases.{GetStrPlural(useCaseName)}.AAAAAAAAAA;";

        content.AppendLine(@$"{usings}
using Microsoft.Extensions.DependencyInjection;

namespace {solutionName}.Application.UseCases.{useCaseName};

public static class DependencyInjection
{{
    public static IServiceCollection Add{GetStrPlural(useCaseName)}Application(this IServiceCollection services)
    {{
        services.AddScoped<IGetOcorrencia, GetOcorrencia>();
        services.AddScoped<IGetAllOcorrencia, GetAllOcorrencia>();

        return services;
    }}
}}");

        return GetIndentedCode(content.ToString());
    }
    #endregion
}