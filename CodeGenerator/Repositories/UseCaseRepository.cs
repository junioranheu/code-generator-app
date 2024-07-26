using System.Text;
using CodeGenerator.Enums;
using CodeGenerator.Models;
using static CodeGenerator.Utils.Fixtures.Generate;
using static CodeGenerator.Utils.Fixtures.Get;

namespace CodeGenerator.Repositories;

public sealed class UseCaseRepository
{
    #region Main
    public static List<Content> GenerateUseCaseAndAllItsDependencies(string solutionName, string context, string rootPath, string useCaseName, List<string> props, bool isFKGuid)
    {
        List<string> contentPathEnums = GenerateFolders(solutionName, rootPath, useCaseName);
        List<Content> finalContent = GenerateContent(solutionName, context, rootPath, useCaseName, props, contentPathEnums, isFKGuid);
        GenerateDependencyInjection(finalContent, solutionName, rootPath, useCaseName, contentPathEnums);

        return finalContent;
    }

    private static List<string> GenerateFolders(string solutionName, string rootPath, string useCaseName)
    {
        string mainFolderPath = Path.Combine(rootPath, $"{solutionName}.{GetEnumDesc(ContentDirectoryEnum.UseCase)}", GetStrPlural(useCaseName));
        GenerateFolder(solutionName, folderPath: mainFolderPath);

        List<string> contentPathEnums = GetEnumDescriptionOfAllItemsAndAssignInListStr<UseCaseEnum>();
        List<string> contentPathExtra = ["Shared"]; // Extra items if needed;
        GenerateFolderByPathList(solutionName, mainFolderPath, paths: contentPathEnums.Concat(contentPathExtra).ToList());

        return contentPathEnums;
    }

    private static List<Content> GenerateContent(string solutionName, string context, string rootPath, string useCaseName, List<string> props, List<string> contentPathEnums, bool isFKGuid)
    {
        List<Content> finalContent = [];

        foreach (string item in contentPathEnums)
        {
            ExtensionsEnum extension = ExtensionsEnum.CS;
            ContentDirectoryEnum contentDirectory = ContentDirectoryEnum.UseCase;
            string fileName = GetFileName(useCaseName, item, isInterface: false);
            string interfaceFileName = GetFileName(useCaseName, item, isInterface: true);

            (string content, string parameters) = CheckUseCaseEnumAndGenerateContent(item, solutionName, context, useCaseName, props, isFKGuid);

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

    private static void GenerateDependencyInjection(List<Content> finalContent, string solutionName, string rootPath, string useCaseName, List<string> contentPathEnums)
    {
        ExtensionsEnum extension = ExtensionsEnum.CS;
        ContentDirectoryEnum contentDirectory = ContentDirectoryEnum.UseCase;

        finalContent.Add(new(
            value: GenerateDependencyInjection(solutionName, useCaseName, contentPathEnums),
            contentDirectory,
            extension,
            solutionName,
            fileFinalPath: GetFinalFilePath(solutionName, rootPath, fileName: $"{GetStrPlural(useCaseName)}/DependencyInjection", contentDirectory, extension)
        ));
    }
    #endregion

    #region UseCases
    private static (string content, string parameters) GenerateUseCase_Get(string solutionName, string context, string useCaseName, List<string> props)
    {
        StringBuilder content = new();
        string parameters = GenerateParametersStringByProps(props);

        content.AppendLine($@"using {solutionName}.Domain.Entities;
using {solutionName}.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace {solutionName}.Application.UseCases.{useCaseName}.Get;

public sealed class Get{useCaseName}({context} context) : IGet{useCaseName}
{{
    private readonly {context} _context = context;

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

    private static (string content, string parameters) GenerateUseCase_GetAll(string solutionName, string context, string useCaseName, List<string> props)
    {
        StringBuilder content = new();
        string parameters = $"PaginationInput pagination, {useCaseName}Input input";

        content.AppendLine($@"using {solutionName}.Application.UseCases.{GetStrPlural(useCaseName)}.Shared;
using {solutionName}.Application.UseCases.Shared;
using {solutionName}.Domain.Entities;
using {solutionName}.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace {solutionName}.Application.UseCases.{useCaseName}.GetAll;

public sealed class GetAll{useCaseName}({context} context) : IGetAll{useCaseName}
{{
    private readonly {context} _context = context;

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

    private static (string content, string parameters) GenerateUseCase_Create(string solutionName, string context, string useCaseName)
    {
        StringBuilder content = new();
        string parameters = $"{useCaseName} input";

        content.AppendLine($@"using {solutionName}.Domain.Entities;
using {solutionName}.Infrastructure.Data;

namespace {solutionName}.Application.UseCases.{useCaseName}.Create;

public sealed class Create{useCaseName}({context} context) : ICreate{useCaseName}
{{
    private readonly {context} _context = context;

    public async Task Execute({parameters})
    {{
        await _context.AddAsync(input);
        await _context.SaveChangesAsync();
    }}
}}");

        return (GetIndentedCode(content.ToString()), parameters);
    }

    private static (string content, string parameters) GenerateUseCase_CreateRange(string solutionName, string context, string useCaseName)
    {
        StringBuilder content = new();
        string parameters = $"List<{useCaseName}> input";

        content.AppendLine($@"using {solutionName}.Domain.Entities;
using {solutionName}.Infrastructure.Data;

namespace {solutionName}.Application.UseCases.{useCaseName}.CreateRange;

public sealed class CreateRange{useCaseName}({context} context) : ICreateRange{useCaseName}
{{
    private readonly {context} _context = context;

    public async Task Execute({parameters})
    {{
        if (input?.Count < 1 || input is null)
        {{
            return;
        }}

        var linqPrevious = await GetAllPrevious();

        await _context.AddRangeAsync(input);
        await _context.SaveChangesAsync();

        await UpdateStatus(linqPrevious);
    }}

    private async Task<List<{useCaseName}>> GetAllPrevious()
    {{
        var linq = await _context.{GetStrPlural(useCaseName)}.
                   Where(x => x.Status == true).
                   AsNoTracking().ToListAsync();

        return linq;
    }}

    private async Task UpdateStatus(List<{useCaseName}> linqPrevious)
    {{
        if (linqPrevious?.Count == 0)
        {{
            return;
        }}

        foreach (var l in linqPrevious)
        {{
            l.Status = false;
        }}

        _context.UpdateRange(linqPrevious);
        await _context.SaveChangesAsync();
    }}
}}");

        return (GetIndentedCode(content.ToString()), parameters);
    }

    private static (string content, string parameters) GenerateUseCase_Update(string solutionName, string context, string useCaseName)
    {
        StringBuilder content = new();
        string parameters = $"{useCaseName} input";

        content.AppendLine($@"using {solutionName}.Domain.Entities;
using {solutionName}.Infrastructure.Data;

namespace {solutionName}.Application.UseCases.{useCaseName}.Update;

public sealed class Update{useCaseName}({context} context) : IUpdate{useCaseName}
{{
    private readonly {context} _context = context;

    public async Task Execute({parameters})
    {{
        var entity = await _context.{GetStrPlural(useCaseName)}.FindAsync(input.{useCaseName}Id);

        if (entity is null) {{
            return;
        }}

        _context.Update(input);
        await _context.SaveChangesAsync();
    }}
}}");

        return (GetIndentedCode(content.ToString()), parameters);
    }

    private static (string content, string parameters) GenerateUseCase_Delete(string solutionName, string context, string useCaseName, bool isFKGuid)
    {
        StringBuilder content = new();
        string parameters = GetClassId(useCaseName, isFKGuid);

        content.AppendLine($@"using {solutionName}.Domain.Entities;
using {solutionName}.Infrastructure.Data;

namespace {solutionName}.Application.UseCases.{useCaseName}.Delete;

public sealed class Delete{useCaseName}({context} context) : IDelete{useCaseName}
{{
    private readonly {context} _context = context;

    public async Task Execute({parameters})
    {{
        var entity = await _context.{GetStrPlural(useCaseName)}.FindAsync({useCaseName}Id);

        if (entity is null) {{
            return;
        }}

        _context.Remove(input);
        await _context.SaveChangesAsync();
    }}
}}");

        return (GetIndentedCode(content.ToString()), parameters);
    }
    #endregion

    #region Etc

    private static string GetFileName(string useCaseName, string item, bool isInterface)
    {
        return Path.Combine(GetStrPlural(useCaseName), item, $"{(isInterface ? "I" : string.Empty)}{item}{useCaseName}");
    }

    private static (string content, string parameters) CheckUseCaseEnumAndGenerateContent(string useCaseType, string solutionName, string context, string useCaseName, List<string> props, bool isFKGuid)
    {
        if (useCaseType == GetEnumDesc(UseCaseEnum.Get))
        {
            return GenerateUseCase_Get(solutionName, context, useCaseName, props);
        }
        else if (useCaseType == GetEnumDesc(UseCaseEnum.GetAll))
        {
            return GenerateUseCase_GetAll(solutionName, context, useCaseName, props);
        }
        else if (useCaseType == GetEnumDesc(UseCaseEnum.Create))
        {
            return GenerateUseCase_Create(solutionName, context, useCaseName);
        }
        else if (useCaseType == GetEnumDesc(UseCaseEnum.CreateRange))
        {
            return GenerateUseCase_CreateRange(solutionName, context, useCaseName);
        }
        else if (useCaseType == GetEnumDesc(UseCaseEnum.Update))
        {
            return GenerateUseCase_Update(solutionName, context, useCaseName);
        }
        else if (useCaseType == GetEnumDesc(UseCaseEnum.Delete))
        {
            return GenerateUseCase_Delete(solutionName, context, useCaseName, isFKGuid);
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

    private static string GenerateDependencyInjection(string solutionName, string useCaseName, List<string> contentPathEnums)
    {
        StringBuilder content = new();

        GenerateCustomTextStringBuilderByListOfStrings(content, contentPathEnums, $"using {solutionName}.Application.UseCases.{GetStrPlural(useCaseName)}.REPLACE_VAR;");

        content.AppendLine(@$"using Microsoft.Extensions.DependencyInjection;

namespace {solutionName}.Application.UseCases.{useCaseName};

public static class DependencyInjection
{{
    public static IServiceCollection Add{GetStrPlural(useCaseName)}Application(this IServiceCollection services)
    {{");

        GenerateCustomTextStringBuilderByListOfStrings(content, contentPathEnums, $"services.AddScoped<IREPLACE_VAR{useCaseName}, REPLACE_VAR{useCaseName}>();");

        content.AppendLine($@"
        return services;
    }}
}}");

        return GetIndentedCode(content.ToString());
    }
    #endregion
}