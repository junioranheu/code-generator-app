using System.Text;
using CodeGenerator.Consts;
using CodeGenerator.Enums;
using CodeGenerator.Models;
using static CodeGenerator.Utils.Fixtures.Get;

namespace CodeGenerator.Repositories;

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
                fileFinalPath: GetFinalFilePath(solutionName, rootPath, fileName: className, contentDirectory, extension)
            )
        ];

        return content;
    }

    private static string GenerateContent(string solutionName, string className, List<string> props, bool isPKGuid)
    {
        StringBuilder content = new();
        string parameters = GenerateParametersStringByProps(props);
        string parameterNamesOnly = GenerateParametersStringByProps(props, getBothNameAndType: false);
        string parametersWithQuestionMark = GenerateParametersStringByProps(props, addQuestionMark: true);
        List<string> contentPathEnums = GetEnumDescriptionOfAllItemsAndAssignInListStr<UseCaseEnum>();
        List<string> contentPathEnums_LowerCase = contentPathEnums.Select(x => GetStringLowerCaseFirstLetter(x)).ToList();
        string paramId = GetClassId(className, isPKGuid, isLowerCaseFirstLetter: true); 

        GenerateCustomTextStringBuilderByListOfStrings(content, contentPathEnums, $"using {solutionName}.Application.UseCases.{GetStrPlural(className)}.REPLACE_VAR;");

        content.AppendLine($@"using {solutionName}.Application.UseCases.Shared;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace {solutionName}.API.Controllers;

[ApiController]
[Route(""api/[controller]"")]
public class {className}Controller(");

        GenerateCustomTextStringBuilderByListOfStrings(content, contentPathEnums_LowerCase, $"IREPLACE_VAR_CAPITALIZEDFIRSTLETTER{className} REPLACE_VAR,");

        content.AppendLine($@"IMapper mapper) : BaseController<{className}Controller>
{{
    private readonly IMapper _mapper = mapper;");

        GenerateCustomTextStringBuilderByListOfStrings(content, contentPathEnums_LowerCase, $"private readonly IREPLACE_VAR_CAPITALIZEDFIRSTLETTER{className} _REPLACE_VAR = REPLACE_VAR;");
        content.AppendLine();

        content.AppendLine($@"[AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult> Get({parametersWithQuestionMark})
    {{
        var result = await _get.Execute({parameterNamesOnly}) ?? throw new InvalidOperationException(""{Misc.WarningEmpty}"");
        return Ok(_mapper.Map<{className}Output>(result));
    }}

    [AllowAnonymous]
    [HttpGet(""GetAll"")]
    public async Task<ActionResult> GetAll([FromQuery] PaginationInput pagination, {parameters})
    {{
        var result = await _getAll.Execute(pagination, {parameterNamesOnly});
        return Ok(_mapper.Map<IEnumerable<{className}Output>>(result));
    }}

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult> Create({className}Input input)
    {{
        var item = _mapper.Map<{className}>(input);
        await _create.Execute(item);

        return NoContent();
    }}

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult> CreateRange(List<{className}Input> input)
    {{
        var list = _mapper.Map<List<{className}>>(input);
        await _createRange.Execute(list);

        return NoContent();
    }}

    [AllowAnonymous]
    [HttpPut]
    public async Task<ActionResult> Update({className}Input input)
    {{
        var item = _mapper.Map<{className}>(input);
        await _update.Execute(item);

        return NoContent();
    }}

    [AllowAnonymous]
    [HttpDelete]
    public async Task<ActionResult> Update({paramId})
    {{
        await _delete.Execute({GetStringLowerCaseFirstLetter(className)}Id);
        return NoContent();
    }}
}}");

        return GetIndentedCode(content.ToString());
    }
}