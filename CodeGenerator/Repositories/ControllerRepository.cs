using System.Text;
using CodeGenerator.Enums;
using CodeGenerator.Models;
using static CodeGenerator.Utils.Fixtures.Get;

namespace CodeGenerator.Repositories;

public sealed class ControllerRepository
{
    public static List<Content> GenerateController(string solutionName, string rootPath, string className, List<string> props)
    {
        ExtensionsEnum extension = ExtensionsEnum.CS;
        ContentDirectoryEnum contentDirectory = ContentDirectoryEnum.Controller;

        List<Content> content =
        [
            new(
                value: GenerateContent(solutionName, className, props),
                contentDirectory,
                extension,
                solutionName,
                fileFinalPath: GetFinalFilePath(solutionName, rootPath, fileName: className, contentDirectory, extension)
            )
        ];

        return content;
    }

    private static string GenerateContent(string solutionName, string className, List<string> props)
    {
        StringBuilder content = new();
        string parameters = GenerateParametersStringByProps(props);
        string parameterNamesOnly = GenerateParametersStringByProps(props, getBothNameAndType: false);
        List<string> contentPathEnums = GetEnumDescriptionOfAllItemsAndAssignInListStr<UseCaseEnum>();

        content.AppendLine($@"using AutoMapper;");

        GenerateCustomTextStringBuilderByListOfStrings(content, contentPathEnums, $"using {solutionName}.Application.UseCases.{GetStrPlural(className)}.REPLACE_VAR;");

        content.AppendLine($@"using {solutionName}.Application.UseCases.Shared;
using Microsoft.AspNetCore.Mvc;

namespace {solutionName}.API.Controllers;

[ApiController]
[Route(""api/[controller]"")]
public class {className}Controller(
IMapper mapper,
IGetAll{className} getAll) : BaseController<{className}Controller>
{{
    private readonly IMapper _mapper = mapper;
    private readonly IGetAll{className} _getAll = getAll;

    [HttpGet(""GetAll"")]
    public async Task<ActionResult> GetAll([FromQuery] PaginationInput pagination, {parameters})
    {{
        var result = await _getAll.Execute(pagination, {parameterNamesOnly});
        return Ok(_mapper.Map<IEnumerable<{className}Output>>(result));
    }}
}}");

        return GetIndentedCode(content.ToString());
    }
}