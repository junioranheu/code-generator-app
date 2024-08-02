using CodeGenerator.Console;
using CodeGenerator.Console.Enums;
using CodeGenerator.Console.Models;
using static CodeGenerator.Console.Utils.Fixtures.Prompt;
using static CodeGenerator.Console.Utils.Fixtures.Get;

#region Input
GenerateCodeRequest request = new()
{
    SolutionName = GetStrCapitalizedFirstLetter(PromptInput("Solution name?")),
    ContextName = GetStrCapitalizedFirstLetter(PromptInput("Context name?")),
    IsPKGuid = PromptInputForBool("Are the primary keys (PKs) Guids?"),
    IsGenerateZip = PromptInputForBool("Do you want to generate a ZIP file?"),
    Models = PromptInputForModel()
};
#endregion

Main.Execute(request);

PromptLog("Press any key to exit the program", type: LogEnum.Info);
Console.ReadKey();