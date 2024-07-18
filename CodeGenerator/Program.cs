using CodeGenerator.Enums;
using CodeGenerator.Models;
using CodeGenerator.Repositories;
using static CodeGenerator.Utils.Fixtures.Generate;
using static CodeGenerator.Utils.Fixtures.Get;

string INPUT_solutionName = "Anheu";
string path = GenerateDefaultDirectories(INPUT_solutionName);

#region Models
List<Model> INPUT_models =
[
    new() { Name = "User", Props = "UserId Guid Name string Age int City string aeaaaaaa bool" },
    new() { Name = "Log", Props = "LogId Guid Desc string Date DateTime" }
];

foreach (var model in INPUT_models)
{
    Content content = new()
    {
        Value = ModelRepository.GenerateModel(INPUT_solutionName, model.Name, ModelRepository.GenerateModelProps(model.Props)),
        Path = ContentPathEnum.Model
    };

    GenerateFile(INPUT_solutionName, path, fileName: model.Name, content, GetEnumDesc(ExtensionsEnum.cs));
}
#endregion

GetLog("Press any key to exit the program");
Console.ReadLine();