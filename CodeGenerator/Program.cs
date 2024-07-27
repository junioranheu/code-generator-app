using CodeGenerator;
using CodeGenerator.Enums;
using CodeGenerator.Models;
using static CodeGenerator.Utils.Fixtures.Prompt;

#region Input
string solutionName = PromptInput("Solution name?");
string contextName = PromptInput("Context name?");
bool isFKGuid = PromptInputForBool("Are the PKs Guids?");
bool isGenerateZip = PromptInputForBool("Do you want to generate a zip file?");

List<Model> INPUT_models =
[
    new() { Name = "Author", Props = "Name string Age int LastName string" },
    new() { Name = "Book", Props = "Name string Price double Type string Author Author" }
];
#endregion

Main.Execute(solutionName, contextName, isFKGuid, INPUT_models, isGenerateZip);

PromptLog("Press any key to exit the program", type: LogEnum.Info);
Console.ReadKey();