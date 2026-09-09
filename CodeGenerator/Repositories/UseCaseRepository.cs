using System.Text;
using CodeGenerator.Console.Enums;
using CodeGenerator.Console.Models;
using static CodeGenerator.Console.Utils.Fixtures.Generate;
using static CodeGenerator.Console.Utils.Fixtures.Get;

namespace CodeGenerator.Console.Repositories;

public sealed class UseCaseRepository
{
    /// <summary>
    /// Criação dinâmica com base nos parâmetros;
    /// </summary>
    public static List<Content> GenerateUseCaseAndAllItsDependencies(string solutionName, string context, string rootPath, string useCaseName, List<string> props, bool isPKGuid)
    {
        List<string> contentPathEnums = GenerateFolders(solutionName, rootPath, useCaseName);
        List<Content> finalContent = GenerateContent(solutionName, context, rootPath, useCaseName, props, contentPathEnums, isPKGuid);
        GenerateDependencyInjection(finalContent, solutionName, rootPath, useCaseName, contentPathEnums);

        return finalContent;
    }

    /// <summary>
    /// Criação dos use cases de Auth (placeholder apenas, por limitação (ou preguiça?));
    /// </summary>
    public static List<Content> GenerateUseCaseForAuth(string solutionName, string rootPath)
    {
        string useCaseName = "Auth";

        List<string> contentPathEnums = ["CreateRefreshTokenJWT", "CreateTokenJWT", "GetMe", "GetRefreshTokenJWT", "Logout", "Shared"];
        string mainFolderPath = Path.Combine(rootPath, $"{solutionName}.{GetEnumDesc(ContentDirectoryEnum.UseCase)}", useCaseName);
        GenerateFolderByPathList(solutionName, mainFolderPath, paths: [.. contentPathEnums]);

        List<Content> finalContent = GenerateContentAuthPlaceholder(solutionName, rootPath, useCaseName, contentPathEnums);
        GenerateDependencyInjection(finalContent, solutionName, rootPath, useCaseName, contentPathEnums, getUseCaseNamePlural: false);

        return finalContent;
    }

    #region Main (extras)
    private static List<string> GenerateFolders(string solutionName, string rootPath, string useCaseName)
    {
        string mainFolderPath = Path.Combine(rootPath, $"{solutionName}.{GetEnumDesc(ContentDirectoryEnum.UseCase)}", GetStrPlural(useCaseName));
        GenerateFolder(solutionName, folderPath: mainFolderPath);

        List<string> contentPathEnums = GetEnumDescriptionOfAllItemsAndAssignInListStr<UseCaseEnum>();
        List<string> contentPathExtra = ["Shared"]; // Itens extras;
        GenerateFolderByPathList(solutionName, mainFolderPath, paths: [.. contentPathEnums, .. contentPathExtra]);

        return contentPathEnums;
    }

    private static List<Content> GenerateContent(string solutionName, string context, string rootPath, string useCaseName, List<string> props, List<string> contentPathEnums, bool isPKGuid)
    {
        List<Content> finalContent = [];
        ExtensionsEnum extension = ExtensionsEnum.CS;
        ContentDirectoryEnum contentDirectory = ContentDirectoryEnum.UseCase;

        foreach (string item in contentPathEnums)
        {
            string fileName = GetFileName(useCaseName, item, isInterface: false);
            string interfaceFileName = GetFileName(useCaseName, item, isInterface: true);

            (string content, string parameters) = CheckUseCaseEnumAndGenerateContent(item, solutionName, context, useCaseName, props, isPKGuid);

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
                value: GenerateInterface(item, solutionName, useCaseName, parameters),
                contentDirectory,
                extension,
                solutionName,
                fileFinalPath: GetFinalFilePath(solutionName, rootPath, fileName: interfaceFileName, contentDirectory, extension)
            ));
        }

        // Input;
        finalContent.Add(new(
            value: EntityRepository.GenerateContent(solutionName, className: useCaseName, props, isPKGuid, isInput: true),
            contentDirectory,
            extension,
            solutionName,
            fileFinalPath: GetFinalFilePath(solutionName, rootPath, fileName: $"{GetStrPlural(useCaseName)}/Shared/{useCaseName}Input", contentDirectory, extension)
        ));

        // Output;
        finalContent.Add(new(
            value: EntityRepository.GenerateContent(solutionName, className: useCaseName, props, isPKGuid, isOutput: true),
            contentDirectory,
            extension,
            solutionName,
            fileFinalPath: GetFinalFilePath(solutionName, rootPath, fileName: $"{GetStrPlural(useCaseName)}/Shared/{useCaseName}Output", contentDirectory, extension)
        ));

        return finalContent;
    }

    private static void GenerateDependencyInjection(List<Content> finalContent, string solutionName, string rootPath, string useCaseName, List<string> contentPathEnums, bool? getUseCaseNamePlural = true)
    {
        ExtensionsEnum extension = ExtensionsEnum.CS;
        ContentDirectoryEnum contentDirectory = ContentDirectoryEnum.UseCase;

        string useCaseNameNormalized = getUseCaseNamePlural.GetValueOrDefault() ? GetStrPlural(useCaseName) : useCaseName;

        finalContent.Add(new(
            value: GenerateDependencyInjection(solutionName, useCaseName, contentPathEnums),
            contentDirectory,
            extension,
            solutionName,
            fileFinalPath: GetFinalFilePath(solutionName, rootPath, fileName: $"{useCaseNameNormalized}/DependencyInjection", contentDirectory, extension)
        ));
    }
    #endregion

    #region UseCases
    private static (string content, string parameters) GenerateUseCase_Get(string solutionName, string context, string useCaseName, List<string> props)
    {
        StringBuilder content = new();
        string parameters = $"{useCaseName}Input input";

        content.AppendLine($@"using {solutionName}.Domain.Entities;
using {solutionName}.Application.UseCases.{GetStrPlural(useCaseName)}.Shared;
using {solutionName}.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace {solutionName}.Application.UseCases.{GetStrPlural(useCaseName)}.Get;

public sealed class Get{useCaseName}({context} context) : IGet{useCaseName}
{{
    private readonly {context} _context = context;

    public async Task<{useCaseName}?> Execute({parameters})
    {{
        var linq = await _context.{GetStrPlural(useCaseName)}.
        Where(x =>
        x.Status == true &&"
    );

        GenerateWhereQueriesByProps(content, props, hasInputPrefix: true);

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

        content.AppendLine($@"using {solutionName}.Application.UseCases.Shared;
using {solutionName}.Application.UseCases.{GetStrPlural(useCaseName)}.Shared;
using {solutionName}.Domain.Entities;
using {solutionName}.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace {solutionName}.Application.UseCases.{GetStrPlural(useCaseName)}.GetAll;

public sealed class GetAll{useCaseName}({context} context) : IGetAll{useCaseName}
{{
    private readonly {context} _context = context;

    public async Task<(IEnumerable<{useCaseName}> linq, int count)> Execute({parameters})
    {{
        var query = _context.{GetStrPlural(useCaseName)}.
        OrderBy(x => x.{FirstCharToUpper(GetClassIdWithoutType(useCaseName, isLowerCaseFirstLetter: false))}).
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

namespace {solutionName}.Application.UseCases.{GetStrPlural(useCaseName)}.Create;

public sealed class Create{useCaseName}({context} context) : ICreate{useCaseName}
{{
    private readonly {context} _context = context;

    public async Task<{useCaseName}?> Execute({parameters})
    {{
        await _context.AddAsync(input);
        await _context.SaveChangesAsync();

        return input;
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
using Microsoft.EntityFrameworkCore;

namespace {solutionName}.Application.UseCases.{GetStrPlural(useCaseName)}.CreateRange;

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
        var linq = await _context.{GetStrPlural(useCaseName)}.Where(x => x.Status == true).ToListAsync();

        return linq;
    }}

    private async Task UpdateStatus(List<{useCaseName}> linqPrevious)
    {{
        if (linqPrevious?.Count == 0)
        {{
            return;
        }}

        foreach (var l in linqPrevious!)
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

namespace {solutionName}.Application.UseCases.{GetStrPlural(useCaseName)}.Update;

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

    private static (string content, string parameters) GenerateUseCase_Delete(string solutionName, string context, string useCaseName, bool isPKGuid)
    {
        StringBuilder content = new();
        string parameters = GetClassId(useCaseName, isPKGuid, isLowerCaseFirstLetter: true);
        string id = GetClassIdWithoutType(useCaseName, isLowerCaseFirstLetter: true);

        content.AppendLine($@"using {solutionName}.Infrastructure.Data;

namespace {solutionName}.Application.UseCases.{GetStrPlural(useCaseName)}.Delete;

public sealed class Delete{useCaseName}({context} context) : IDelete{useCaseName}
{{
    private readonly {context} _context = context;

    public async Task Execute({parameters})
    {{
        var entity = await _context.{GetStrPlural(useCaseName)}.FindAsync({id});

        if (entity is null) {{
            return;
        }}

        _context.Remove(entity);
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

    private static (string content, string parameters) CheckUseCaseEnumAndGenerateContent(string useCaseType, string solutionName, string context, string useCaseName, List<string> props, bool isPKGuid)
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
            return GenerateUseCase_Delete(solutionName, context, useCaseName, isPKGuid);
        }

        throw new NotImplementedException();
    }

    private static string GenerateInterface(string useCaseType, string solutionName, string useCaseName, string parameters)
    {
        string useCaseNamespace = $"{solutionName}.Application.UseCases.{GetStrPlural(useCaseName)}.{useCaseType}";
        string interfaceName = $"I{useCaseType}{useCaseName}";
        string executeMethod = GetInterfaceExecuteMethod(useCaseType, useCaseName, parameters);

        StringBuilder content = new();

        if (useCaseType == GetEnumDesc(UseCaseEnum.Get))
        {
            content.AppendLine($"using {solutionName}.Application.UseCases.{GetStrPlural(useCaseName)}.Shared;");
        }

        if (useCaseType == GetEnumDesc(UseCaseEnum.GetAll))
        {
            content.AppendLine($"using {solutionName}.Application.UseCases.Shared;");
            content.AppendLine($"using {solutionName}.Application.UseCases.{GetStrPlural(useCaseName)}.Shared;");
        }

        if (useCaseType != GetEnumDesc(UseCaseEnum.Delete))
        {
            content.AppendLine($"using {solutionName}.Domain.Entities;");
        }

        content.AppendLine();
        content.AppendLine($"namespace {useCaseNamespace};");
        content.AppendLine();
        content.AppendLine($"public interface {interfaceName}");
        content.AppendLine("{");
        content.AppendLine($"    {executeMethod}");
        content.AppendLine("}");

        return GetIndentedCode(content.ToString());
    }

    private static string GetInterfaceExecuteMethod(string useCaseType, string useCaseName, string parameters)
    {
        if (useCaseType == GetEnumDesc(UseCaseEnum.Delete) || useCaseType == GetEnumDesc(UseCaseEnum.CreateRange) || useCaseType == GetEnumDesc(UseCaseEnum.Update))
        {
            return $"Task Execute({parameters});";
        }

        string returnType = useCaseType == GetEnumDesc(UseCaseEnum.GetAll) ? $"(IEnumerable<{useCaseName}> linq, int count)" : $"{useCaseName}?";

        return $"Task<{returnType}> Execute({parameters});";
    }

    private static string GenerateDependencyInjection(string solutionName, string useCaseName, List<string> contentPathEnums)
    {
        StringBuilder content = new();

        GenerateCustomTextStringBuilderByListOfStrings(content, contentPathEnums, $"using {solutionName}.Application.UseCases.{GetStrPlural(useCaseName)}.REPLACE_VAR;", shouldIncludeSharedFolder: false);

        content.AppendLine(@$"using Microsoft.Extensions.DependencyInjection;

namespace {solutionName}.Application.UseCases.{GetStrPlural(useCaseName)};

public static class DependencyInjection
{{
    public static IServiceCollection Add{GetStrPlural(useCaseName)}Application(this IServiceCollection services)
    {{");

        GenerateCustomTextStringBuilderByListOfStrings(content, contentPathEnums, $"services.AddScoped<IREPLACE_VAR{useCaseName}, REPLACE_VAR{useCaseName}>();", shouldIncludeSharedFolder: false);

        content.AppendLine($@"
        return services;
    }}
}}");

        return GetIndentedCode(content.ToString());
    }
    #endregion

    #region UseCases (Auth - placeholder apenas)
    private static List<Content> GenerateContentAuthPlaceholder(string solutionName, string rootPath, string useCaseName, List<string> contentPathEnums)
    {
        List<Content> content = [];
        ExtensionsEnum extension = ExtensionsEnum.CS;
        ContentDirectoryEnum contentDirectory = ContentDirectoryEnum.UseCase;

        foreach (string item in contentPathEnums)
        {
            if (item == "Shared")
            {
                continue;
            }

            // Classe;
            string classFileName = Path.Combine(useCaseName, item, $"{item}{useCaseName}");
            StringBuilder classContent = new();

            classContent.AppendLine($"namespace {solutionName}.Application.UseCases.{useCaseName}.{item};");
            classContent.AppendLine();
            classContent.AppendLine($"public sealed class {item}{useCaseName} : I{item}{useCaseName}");
            classContent.AppendLine("{");
            classContent.AppendLine("    // TODO: implementar métodos específicos do use case;");
            classContent.AppendLine("}");

            if (item == "CreateRefreshTokenJWT")
            {
                // Adicionar método falso RefreshToken que retorna tupla (string, CookieOptions);
                classContent.Clear();
                classContent.AppendLine("using Microsoft.AspNetCore.Http;");
                classContent.AppendLine();
                classContent.AppendLine($"namespace {solutionName}.Application.UseCases.{useCaseName}.{item};");
                classContent.AppendLine();
                classContent.AppendLine($"public sealed class {item}{useCaseName} : I{item}{useCaseName}");
                classContent.AppendLine("{");
                classContent.AppendLine("    public async Task<(string newJwtToken, CookieOptions cookieOptions)> RefreshToken(Guid userIdAuth)");
                classContent.AppendLine("    {");
                classContent.AppendLine("        // Implementação fake para scaffold — retorna token simulado e cookieOptions básicos;");
                classContent.AppendLine("        await Task.CompletedTask;");
                classContent.AppendLine("        CookieOptions cookieOptions = new() { HttpOnly = true, Secure = true };");
                classContent.AppendLine();
                classContent.AppendLine("        return (Guid.NewGuid().ToString(), cookieOptions);");
                classContent.AppendLine("    }");
                classContent.AppendLine("}");
            }

            content.Add(new(
                value: GetIndentedCode(classContent.ToString()),
                contentDirectory,
                extension,
                solutionName,
                fileFinalPath: GetFinalFilePath(solutionName, rootPath, fileName: classFileName, contentDirectory, extension)
            ));

            // Interface (vazia - o usuário fornecerá as assinaturas após);
            string interfaceFileName = Path.Combine(useCaseName, item, $"I{item}{useCaseName}");
            StringBuilder interfaceContent = new();

            if (item == "CreateRefreshTokenJWT")
            {
                interfaceContent.AppendLine("using Microsoft.AspNetCore.Http;");
                interfaceContent.AppendLine();
                interfaceContent.AppendLine($"namespace {solutionName}.Application.UseCases.{useCaseName}.{item};");
                interfaceContent.AppendLine();
                interfaceContent.AppendLine($"public interface I{item}{useCaseName}");
                interfaceContent.AppendLine("{");
                interfaceContent.AppendLine("    Task<(string newJwtToken, CookieOptions cookieOptions)> RefreshToken(Guid userIdAuth);");
                interfaceContent.AppendLine("}");
            }
            else
            {
                interfaceContent.AppendLine($"namespace {solutionName}.Application.UseCases.{useCaseName}.{item};");
                interfaceContent.AppendLine();
                interfaceContent.AppendLine($"public interface I{item}{useCaseName}");
                interfaceContent.AppendLine("{");
                interfaceContent.AppendLine("    // TODO: definir assinaturas dos métodos");
                interfaceContent.AppendLine("}");
            }

            content.Add(new(
                value: GetIndentedCode(interfaceContent.ToString()),
                contentDirectory,
                extension,
                solutionName,
                fileFinalPath: GetFinalFilePath(solutionName, rootPath, fileName: interfaceFileName, contentDirectory, extension)
            ));
        }

        return content;
    }
    #endregion
}