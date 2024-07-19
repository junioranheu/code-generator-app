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
    List<Content> entityContent = EntityRepository.GenerateEntity(INPUT_solutionName, rootPath, className: model.Name, props: model.Props );
    GenerateFile(contents: entityContent);
    #endregion

    #region UseCase
    List<Content> useCaseContent = UseCaseRepository.GenerateUseCase(INPUT_solutionName, rootPath, useCaseName: model.Name);
    GenerateFile(contents: useCaseContent);
    #endregion
}

GetLog("Press any key to exit the program", type: LogEnum.Info);
Console.ReadKey();

Directory.Delete(rootPath, true); // XD