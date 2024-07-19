using CodeGenerator.Enums;
using CodeGenerator.Models;
using CodeGenerator.Repositories;
using static CodeGenerator.Utils.Fixtures.Generate;
using static CodeGenerator.Utils.Fixtures.Get;

string INPUT_solutionName = "Anheu";
string rootPath = GenerateDefaultDirectories(INPUT_solutionName);

List<Model> INPUT_models =
[
    new() { Name = "User", Props = "UserId Guid Name string Age int City string aeaaaaaa bool" },
    new() { Name = "Log", Props = "Desc string Date DateTime" }
];

foreach (var model in INPUT_models)
{
    #region Entity
    Content entityContent = new()
    {
        Value = EntityRepository.GenerateEntity(INPUT_solutionName, className: model.Name, props: EntityRepository.GenerateEntityProps(model.Props)),
        Path = ContentPathEnum.Entity,
        Extension = ExtensionsEnum.cs
    };

    string entityFinalFilePath = GetFinalFilePath(INPUT_solutionName, rootPath, fileName: model.Name, content: entityContent);
    GenerateFile(solutionName: INPUT_solutionName, pathFinalFile: entityFinalFilePath, content: entityContent);
    #endregion

    #region UseCase
    (string useCaseContent_, string mainFolderPath) = UseCaseRepository.GenerateUseCase(INPUT_solutionName, rootPath, useCaseName: GetStrPlural(model.Name));

    Content useCaseContent = new()
    {
        Value = useCaseContent_,
        Path = ContentPathEnum.UseCase,
        Extension = ExtensionsEnum.cs
    };

    GenerateFile(solutionName: INPUT_solutionName, pathFinalFile: mainFolderPath, content: entityContent);
    #endregion
}

GetLog("Press any key to exit the program", type: LogEnum.Info);
Console.ReadKey();

Directory.Delete(rootPath, true); // XD