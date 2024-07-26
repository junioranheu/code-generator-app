using CodeGenerator.Enums;
using CodeGenerator.Models;
using CodeGenerator.Repositories;
using static CodeGenerator.Utils.Fixtures.Generate;
using static CodeGenerator.Utils.Fixtures.Get;

#region Input
string INPUT_solutionName = "Anheu";
string INPUT_contextName = "AnheuContext";
bool INPUT_isFKGuid = true;

List<Model> INPUT_models =
[
    new() { Name = "User", Props = "UserId Guid Name string Age int City string aeaaaaaa bool" },
    new() { Name = "Log", Props = "Desc string Date DateTime" }
];
#endregion

string rootPath = GenerateDefaultDirectories(INPUT_solutionName);

foreach (var model in INPUT_models)
{
    List<string> props = GetEntityPropsSplitted(model.Props);

    #region Entity
    List<Content> entityContent = EntityRepository.GenerateEntity(INPUT_solutionName, rootPath, className: model.Name, props, INPUT_isFKGuid);
    GenerateFiles(contents: entityContent);
    #endregion

    #region UseCase
    List<Content> useCaseContent = UseCaseRepository.GenerateUseCaseAndAllItsDependencies(INPUT_solutionName, INPUT_contextName, rootPath, useCaseName: model.Name, props, INPUT_isFKGuid);
    GenerateFiles(contents: useCaseContent);
    #endregion

    #region Controller
    List<Content> controllerContent = ControllerRepository.GenerateController(INPUT_solutionName, rootPath, className: model.Name, props, INPUT_isFKGuid);
    GenerateFiles(contents: controllerContent);
    #endregion
}

GetLog("Press any key to exit the program", type: LogEnum.Info);
Console.ReadKey();

Directory.Delete(rootPath, true); // XD