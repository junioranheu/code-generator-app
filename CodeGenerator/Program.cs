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
    Content modelContent = new()
    {
        Value = EntityRepository.GenerateEntity(INPUT_solutionName, className: model.Name, props: EntityRepository.GenerateEntityProps(model.Props)),
        Path = ContentPathEnum.Entity
    };

    GenerateFile(INPUT_solutionName, rootPath, fileName: model.Name, modelContent, GetEnumDesc(ExtensionsEnum.cs));
    #endregion

    #region UseCase
    Content useCaseContent = new()
    {
        Value = UseCaseRepository.GenerateUseCase(INPUT_solutionName, rootPath, useCaseName: GetStrPlural(model.Name)),
        Path = ContentPathEnum.UseCase
    };

    GenerateFile(INPUT_solutionName, rootPath, fileName: GetStrPlural(model.Name), useCaseContent, GetEnumDesc(ExtensionsEnum.cs));
    #endregion
}

GetLog("Press any key to exit the program");
Console.ReadKey();

Directory.Delete(rootPath, true); // XD