using CodeGenerator.Models;
using CodeGenerator.Repositories;
using static CodeGenerator.Utils.Fixtures.Delete;
using static CodeGenerator.Utils.Fixtures.Generate;
using static CodeGenerator.Utils.Fixtures.Get;

namespace CodeGenerator;

public class Main
{
    public static byte[] Execute(string solutionName, string contextName, bool isPKGuid, List<Model> models, bool isGenerateZip)
    {
        CheckVariables(solutionName, contextName, models);
        string rootPath = GenerateDefaultDirectories(solutionName, isGenerateZip);

        foreach (var model in models)
        {
            List<string> props = GetEntityPropsSplitted(classDefinition: model.Props, rootPath);

            #region Entity
            List<Content> entityContent = EntityRepository.GenerateEntity(solutionName, rootPath, className: model.Name, props, isPKGuid);
            GenerateFiles(contents: entityContent, isGenerateZip);
            #endregion

            #region UseCase
            List<Content> useCaseContent = UseCaseRepository.GenerateUseCaseAndAllItsDependencies(solutionName, contextName, rootPath, useCaseName: model.Name, props, isPKGuid);
            GenerateFiles(contents: useCaseContent, isGenerateZip);
            #endregion

            #region Controller
            List<Content> controllerContent = ControllerRepository.GenerateController(solutionName, rootPath, className: model.Name, props, isPKGuid);
            GenerateFiles(contents: controllerContent, isGenerateZip);
            #endregion
        }

        #region zip
        if (isGenerateZip)
        {
            string rootPathZipFile = GenerateZipFolder(solutionName, pathToZip: rootPath);
            byte[] bytes = GetArrayOfBytesFromPath(rootPathZipFile);

            DeleteFile(rootPathZipFile);

            return bytes;
        }
        #endregion

        return Array.Empty<byte>();
    }

    private static void CheckVariables(string solutionName, string contextName, List<Model> models)
    {
        if (string.IsNullOrEmpty(solutionName))
        {
            throw new ArgumentException("The solution name can not be null or empty");
        }

        if (string.IsNullOrEmpty(contextName))
        {
            throw new ArgumentException("The context name can not be null or empty");
        }

        if (models.Count < 1)
        {
            throw new ArgumentException("Models can not be empty");
        }
    }
}