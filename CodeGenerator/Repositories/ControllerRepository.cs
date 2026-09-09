using CodeGenerator.Console.Enums;
using CodeGenerator.Console.Models;
using System.Text;
using static CodeGenerator.Console.Utils.Fixtures.Get;

namespace CodeGenerator.Console.Repositories;

public sealed class ControllerRepository
{
    public static List<Content> GenerateController(string solutionName, string rootPath, string className, List<string> props, bool isPKGuid)
    {
        ExtensionsEnum extension = ExtensionsEnum.CS;
        ContentDirectoryEnum contentDirectory = ContentDirectoryEnum.Controller;

        List<Content> content =
        [
            new(
                value: GenerateContent(solutionName, className, props, isPKGuid),
                contentDirectory,
                extension,
                solutionName,
                fileFinalPath: GetFinalFilePath(solutionName, rootPath, fileName: $"{GetStrCapitalizedFirstLetter(className)}Controller", contentDirectory, extension)
            )
        ];

        return content;
    }

    public static List<Content> GenerateControllerBaseAndAllDependencies(string solutionName, string rootPath)
    {
        List<Content> content =
        [
            new(
                value: GenerateUserRoleEnumContent(solutionName),
                contentDirectory: ContentDirectoryEnum.Enum,
                extension: ExtensionsEnum.CS,
                solutionName: solutionName,
                fileFinalPath: GetFinalFilePath(solutionName, rootPath, fileName: "UserRoleEnum", contentDirectory: ContentDirectoryEnum.Enum, extension: ExtensionsEnum.CS)
            ),
            new(
                value: GenerateBaseControllerContent(solutionName),
                contentDirectory: ContentDirectoryEnum.Controller,
                extension: ExtensionsEnum.CS,
                solutionName: solutionName,
                fileFinalPath: GetFinalFilePath(solutionName, rootPath, fileName: "BaseController", contentDirectory: ContentDirectoryEnum.Controller, extension: ExtensionsEnum.CS)
            )
        ];

        return content;
    }

    #region extras
    private static string GenerateContent(string solutionName, string className, List<string> props, bool isPKGuid)
    {
        StringBuilder content = new();
        string parameters = GenerateParametersStringByProps(props);
        string parameterNamesOnly = GenerateParametersStringByProps(props, getBothNameAndType: false);
        string parametersWithQuestionMark = GenerateParametersStringByProps(props, addQuestionMark: true);
        List<string> contentPathEnums = GetEnumDescriptionOfAllItemsAndAssignInListStr<UseCaseEnum>();
        List<string> contentPathEnums_LowerCase = [.. contentPathEnums.Select(GetStringLowerCaseFirstLetter)];
        string paramId = GetClassId(className, isPKGuid, isLowerCaseFirstLetter: true);
        string paramIdWithoutType = GetClassIdWithoutType(className, isLowerCaseFirstLetter: true);
        string guidOrInt = isPKGuid ? "guid" : "int";

        GenerateCustomTextStringBuilderByListOfStrings(content, contentPathEnums, $"using {solutionName}.Application.UseCases.{GetStrPlural(className)}.REPLACE_VAR;", shouldIncludeSharedFolder: true);

        content.AppendLine($@"using {solutionName}.API.Filters;
using {solutionName}.Application.UseCases.Shared;
using {solutionName}.Domain.Consts;
using {solutionName}.Domain.Entities;
using {solutionName}.Domain.Enums;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace {solutionName}.API.Controllers;

[ApiController]
[Route(""api/[controller]"")]");

        content.Append($"public class {className}Controller("); // Intencionalmente Append apenas;

        GenerateCustomTextStringBuilderByListOfStrings(content, contentPathEnums_LowerCase, $"IREPLACE_VAR_CAPITALIZEDFIRSTLETTER{className} REPLACE_VAR,", shouldIncludeSharedFolder: false, removeLastCommaIfApplicable: true);

        content.AppendLine($") : BaseController<{className}Controller>");
        content.AppendLine("{");

        content.AppendLine("#region constructors");
        GenerateCustomTextStringBuilderByListOfStrings(content, contentPathEnums_LowerCase, $"private readonly IREPLACE_VAR_CAPITALIZEDFIRSTLETTER{className} _REPLACE_VAR = REPLACE_VAR;", shouldIncludeSharedFolder: false);
        content.AppendLine("#endregion");
        content.AppendLine();

        content.AppendLine($@"[AuthorizeFilter]
    [HttpGet]
    public async Task<ActionResult> Get([FromQuery] {className}Input input)
    {{
        {className} result = await _get.Execute(input) ?? throw new KeyNotFoundException(SystemConsts.Warnings.NotFoundData);
        {className}Output output = result.Adapt<{className}Output>();

        return Ok(output);
    }}

    [AuthorizeFilter]
    [HttpGet(nameof(GetAll))]
    public async Task<ActionResult> GetAll([FromQuery] PaginationInput pagination, [FromQuery] {className}Input input)
    {{
        (IEnumerable<{className}>? linq, int count) = await _getAll.Execute(pagination, input);
        IEnumerable<{className}Output> output = linq.Adapt<IEnumerable<{className}Output>>();

        return Ok(new {{ output, count }});
    }}

    [AuthorizeFilter]
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] {className}Input input)
    {{
        {className} item = input.Adapt<{className}>();
        await _create.Execute(item);

        return Ok(true);
    }}

    [AuthorizeFilter]
    [HttpPost(nameof(CreateRange))]
    public async Task<ActionResult> CreateRange([FromBody] List<{className}Input> input)
    {{
        List<{className}> list = input.Adapt<List<{className}>>();
        await _createRange.Execute(list);

        return Ok(true);
    }}

    [AuthorizeFilter([UserRoleEnum.Common, UserRoleEnum.Administrator])]
    [HttpPut]
    public async Task<ActionResult> Update([FromBody] {className}Input input)
    {{
        {className} item = input.Adapt<{className}>();
        await _update.Execute(item);

        return Ok(true);
    }}

    [AuthorizeFilter([UserRoleEnum.Administrator])]
    [HttpDelete(""{{{paramIdWithoutType}:{guidOrInt}}}"")]
    public async Task<ActionResult> Delete([FromRoute] {paramId})
    {{
        await _delete.Execute({GetStringLowerCaseFirstLetter(className)}Id);

        return Ok(true);
    }}
}}");

        return GetIndentedCode(content.ToString());
    }

    private static string GenerateUserRoleEnumContent(string solutionName)
    {
        StringBuilder content = new();

        content.AppendLine("using System.ComponentModel;");
        content.AppendLine();
        content.AppendLine($"namespace {solutionName}.Domain.Enums;");
        content.AppendLine();
        content.AppendLine("public enum UserRoleEnum");
        content.AppendLine("{");
        content.AppendLine("    [Description(\"Usuário do sistema\")]");
        content.AppendLine("    Common = 1,");
        content.AppendLine();
        content.AppendLine("    [Description(\"Suporte do sistema\")]");
        content.AppendLine("    Maintainer = 999,");
        content.AppendLine();
        content.AppendLine("    [Description(\"Administrador do sistema\")]");
        content.AppendLine("    Administrator = 1000");
        content.AppendLine("}");

        return GetIndentedCode(content.ToString());
    }

    private static string GenerateBaseControllerContent(string solutionName)
    {
        StringBuilder content = new();

        content.AppendLine($"using {solutionName}.Domain.Enums;");
        content.AppendLine("using Microsoft.AspNetCore.Mvc;");
        content.AppendLine("using System.ComponentModel;");
        content.AppendLine("using System.Reflection;");
        content.AppendLine("using System.Runtime.CompilerServices;");
        content.AppendLine("using System.Security.Claims;");
        content.AppendLine();
        content.AppendLine($"namespace {solutionName}.API.Controllers;");
        content.AppendLine();
        content.AppendLine("public abstract class BaseController<T> : Controller");
        content.AppendLine("{");
        content.AppendLine("    [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        content.AppendLine("    protected bool IsUserAuth()");
        content.AppendLine("    {");
        content.AppendLine("        if (User is null || User.Identity is null)");
        content.AppendLine("        {");
        content.AppendLine("            return false;");
        content.AppendLine("        }");
        content.AppendLine();
        content.AppendLine("        return User.Identity.IsAuthenticated;");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        content.AppendLine("    protected Guid GetUserIdAuth(bool throwExceptionIfNotAuth = false)");
        content.AppendLine("    {");
        content.AppendLine("        string? id = User?.FindFirstValue(ClaimTypes.NameIdentifier);");
        content.AppendLine();
        content.AppendLine("        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id.AsSpan(), out Guid guid))");
        content.AppendLine("        {");
        content.AppendLine("            if (throwExceptionIfNotAuth)");
        content.AppendLine("            {");
        content.AppendLine("                throw new UnauthorizedAccessException(\"Usuário não autenticado.\");");
        content.AppendLine("            }");
        content.AppendLine();
        content.AppendLine("            return Guid.Empty;");
        content.AppendLine("        }");
        content.AppendLine();
        content.AppendLine("        return guid;");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    protected string GetUserNameAuth()");
        content.AppendLine("    {");
        content.AppendLine("        if (!IsUserAuth())");
        content.AppendLine("        {");
        content.AppendLine("            return string.Empty;");
        content.AppendLine("        }");
        content.AppendLine();
        content.AppendLine("        string name = User.FindFirstValue(ClaimTypes.Name) ?? string.Empty;");
        content.AppendLine();
        content.AppendLine("        return name;");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    protected string GetUserEmailAuth()");
        content.AppendLine("    {");
        content.AppendLine("        if (!IsUserAuth())");
        content.AppendLine("        {");
        content.AppendLine("            return string.Empty;");
        content.AppendLine("        }");
        content.AppendLine();
        content.AppendLine("        string email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;");
        content.AppendLine();
        content.AppendLine("        return email;");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    protected (UserRoleEnum[] userRolesEnum, string[] userRolesStr) GetUserRolesAuth()");
        content.AppendLine("    {");
        content.AppendLine("        if (!IsUserAuth())");
        content.AppendLine("        {");
        content.AppendLine("            return (Array.Empty<UserRoleEnum>(), Array.Empty<string>());");
        content.AppendLine("        }");
        content.AppendLine();
        content.AppendLine("        List<UserRoleEnum> enums = [];");
        content.AppendLine("        List<string> names = [];");
        content.AppendLine();
        content.AppendLine("        foreach (var claim in User.FindAll(ClaimTypes.Role))");
        content.AppendLine("        {");
        content.AppendLine("            if (Enum.TryParse(claim.Value, true, out UserRoleEnum userRole))");
        content.AppendLine("            {");
        content.AppendLine("                enums.Add(userRole);");
        content.AppendLine("                names.Add(GetEnumDesc(userRole));");
        content.AppendLine("            }");
        content.AppendLine("        }");
        content.AppendLine();
        content.AppendLine("        return (enums.ToArray(), names.ToArray());");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    private static string GetEnumDesc(UserRoleEnum value)");
        content.AppendLine("    {");
        content.AppendLine("        MemberInfo? member = typeof(UserRoleEnum).GetMember(value.ToString()).FirstOrDefault();");
        content.AppendLine("        DescriptionAttribute? attribute = member?.GetCustomAttribute<DescriptionAttribute>();");
        content.AppendLine();
        content.AppendLine("        return attribute?.Description ?? value.ToString();");
        content.AppendLine("    }");
        content.AppendLine("}");

        return GetIndentedCode(content.ToString());
    }
    #endregion
}