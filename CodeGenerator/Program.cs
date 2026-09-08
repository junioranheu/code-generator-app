using CodeGenerator.Console;
using CodeGenerator.Console.Enums;
using CodeGenerator.Console.Models;
using Spectre.Console;
using static CodeGenerator.Console.Utils.Fixtures.Prompt;
using static CodeGenerator.Console.Utils.Fixtures.Get;

#region Input
Console.Title = AppDomain.CurrentDomain.FriendlyName;

GenerateCodeRequest request = new()
{
    SolutionName = GetStrCapitalizedFirstLetter(PromptInput("Solution name:")),
    ContextName = GetStrCapitalizedFirstLetter(PromptInput("Context name:")),
    IsPKGuid = PromptInputForBool("Are the primary keys (PKs) Guids?", defaultValue: true),
    IsGenerateZip = PromptInputForBool("Do you want to generate a ZIP file?", defaultValue: false),
    Models = PromptInputForModel(),
    RequestType = RequestTypeEnum.Console
};
#endregion

Main.Execute(request);

PromptLog("Press any key to exit the program", type: LogEnum.Info);
AnsiConsole.Console.Input.ReadKey(intercept: true);