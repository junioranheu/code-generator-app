using CodeGenerator;
using CodeGenerator.Enums;
using CodeGenerator.Models;
using static CodeGenerator.Utils.Fixtures.Prompt;

#region Input
string solutionName = PromptInput("Solution name?");
string contextName = PromptInput("Context name?");
bool isPKGuid = PromptInputForBool("Are the primary keys (PKs) Guids?");
bool isGenerateZip = PromptInputForBool("Do you want to generate a ZIP file?");
List<Model> models = PromptInputForModel();
#endregion

Main.Execute(solutionName, contextName, isPKGuid, models, isGenerateZip);

PromptLog("Press any key to exit the program", type: LogEnum.Info);
Console.ReadKey();