using CodeGenerator.Models;
using CodeGenerator.Repositories;
using static CodeGenerator.Utils.Fixtures.Generate;
using static CodeGenerator.Utils.Fixtures.Get;

namespace CodeGenerator;

public class Main
{
    public static string Execute(string solutionName, string contextName, bool isFKGuid, List<Model> models, bool isGenerateZip)
    {
        string rootPath = GenerateDefaultDirectories(solutionName, isGenerateZip);

        foreach (var model in models)
        {
            List<string> props = GetEntityPropsSplitted(model.Props);

            #region Entity
            List<Content> entityContent = EntityRepository.GenerateEntity(solutionName, rootPath, className: model.Name, props, isFKGuid);
            GenerateFiles(contents: entityContent, isGenerateZip);
            #endregion

            #region UseCase
            List<Content> useCaseContent = UseCaseRepository.GenerateUseCaseAndAllItsDependencies(solutionName, contextName, rootPath, useCaseName: model.Name, props, isFKGuid);
            GenerateFiles(contents: useCaseContent, isGenerateZip);
            #endregion

            #region Controller
            List<Content> controllerContent = ControllerRepository.GenerateController(solutionName, rootPath, className: model.Name, props, isFKGuid);
            GenerateFiles(contents: controllerContent, isGenerateZip);
            #endregion
        }

        if (isGenerateZip)
        {
            GenerateZipFolder(solutionName, pathToZip: rootPath);
        }

        return rootPath;
    }
}