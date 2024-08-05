using CodeGenerator.Console.Enums;
using CodeGenerator.Console.Models;
using CodeGenerator.Console.Repositories;
using static CodeGenerator.Console.Utils.Fixtures.Delete;
using static CodeGenerator.Console.Utils.Fixtures.Generate;
using static CodeGenerator.Console.Utils.Fixtures.Get;

namespace CodeGenerator.Console;

public class Main
{
    public static (byte[] bytes, Guid guid) Execute(GenerateCodeRequest request)
    {
        (string solutionName, string contextName, bool isPKGuid, List<Model> models, bool? isGenerateZip, RequestTypeEnum? requestType) = request;

        CheckVariables(solutionName, contextName, models);
        Guid guid = Guid.NewGuid();
        string rootPath = GenerateDefaultDirectories(solutionName, isGenerateZip.GetValueOrDefault(), guid, requestType.GetValueOrDefault());

        foreach (var model in models)
        {
            List<string> props = GetEntityPropsSplitted(classDefinition: model.Props, rootPath);

            #region Entity
            List<Content> entityContent = EntityRepository.GenerateEntity(solutionName, rootPath, className: model.Name, props, isPKGuid);
            GenerateFiles(contents: entityContent, isGenerateZip.GetValueOrDefault());
            #endregion

            #region UseCase
            List<Content> useCaseContent = UseCaseRepository.GenerateUseCaseAndAllItsDependencies(solutionName, contextName, rootPath, useCaseName: model.Name, props, isPKGuid);
            GenerateFiles(contents: useCaseContent, isGenerateZip.GetValueOrDefault());
            #endregion

            #region Controller
            List<Content> controllerContent = ControllerRepository.GenerateController(solutionName, rootPath, className: model.Name, props, isPKGuid);
            GenerateFiles(contents: controllerContent, isGenerateZip.GetValueOrDefault());
            #endregion
        }

        #region Zip
        if (isGenerateZip.GetValueOrDefault())
        {
            string rootPathZipFile = GenerateZipFromFolder(solutionName, pathToZip: rootPath);
            byte[] bytes = GetArrayOfBytesFromPath(rootPathZipFile);

            if (request.RequestType == RequestTypeEnum.API)
            {
                DeleteFile(rootPathZipFile);
            }

            return (bytes, guid);
        }
        #endregion

        return (Array.Empty<byte>(), guid);
    }
    
    #region Misc
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
    #endregion
}