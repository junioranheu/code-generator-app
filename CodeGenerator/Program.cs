using CodeGenerator;
using CodeGenerator.Enums;
using CodeGenerator.Models;
using static CodeGenerator.Utils.Fixtures.Get;

#region Input
string INPUT_solutionName = "Anheu";
string INPUT_contextName = "AnheuContext";
bool INPUT_isFKGuid = true;
bool INPUT_isGenerateZip = true;

List<Model> INPUT_models =
[
    new() { Name = "Author", Props = "Name string Age int LastName string" },
    new() { Name = "Book", Props = "Name string Price double Type string Author Author" }
];
#endregion

Main.Execute(INPUT_solutionName, INPUT_contextName, INPUT_isFKGuid, INPUT_models, INPUT_isGenerateZip);

GetLog("Press any key to exit the program", type: LogEnum.Info);
Console.ReadKey();