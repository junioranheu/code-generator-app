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
        SolutionRepository.Generate(solutionName, rootPath, contextName, models);

        // Criar entidades sistemáticas e ControllerBase com todas as dependências;
        CreateEntityAudit(solutionName, rootPath);
        CreateEntityRefreshToken(solutionName, rootPath);
        CreateControllerBaseAndAllDependencies(solutionName, rootPath);

        foreach (var model in models)
        {
            List<string> props = GetEntityPropsSplitted(classDefinition: model.Props, rootPath);

            #region Entity
            List<Content> entityContent = EntityRepository.GenerateEntity(solutionName, rootPath, className: model.Name, props, isPKGuid);
            GenerateFiles(contents: entityContent);
            #endregion

            #region UseCase
            List<Content> useCaseContent = UseCaseRepository.GenerateUseCaseAndAllItsDependencies(solutionName, contextName, rootPath, useCaseName: model.Name, props, isPKGuid);
            GenerateFiles(contents: useCaseContent);
            #endregion

            #region Controller
            List<Content> controllerContent = ControllerRepository.GenerateController(solutionName, rootPath, className: model.Name, props, isPKGuid);
            GenerateFiles(contents: controllerContent);
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

        if (solutionName == contextName)
        {
            throw new ArgumentException("The solution and context name can not be the same");
        }

        if (models.Count < 1)
        {
            throw new ArgumentException("Models can not be empty");
        }
    }

    private static void CreateEntityAudit(string solutionName, string rootPath)
    {
        string className = "Audit";

        List<string> props = [
            "CreatedDate DateTime?",
            "CreatedBy Guid?",
            "LastModificationDate DateTime?",
            "LastModificationBy Guid?",
            "Status bool"
        ];

        List<Content> entityContent = EntityRepository.GenerateEntity(solutionName, rootPath, className, props, isPKGuid: true, isSystematicEntity: true);
        GenerateFiles(contents: entityContent);
    }

    private static void CreateEntityRefreshToken(string solutionName, string rootPath)
    {
        string className = "RefreshToken";

        List<string> props = [
            "Token string?",
            "UserId Guid",
            "CreatedDate DateTime",
            "ExpiredDate DateTime?",
            "RevokedDate DateTime?"
        ];

        List<Content> entityContent = EntityRepository.GenerateEntity(solutionName, rootPath, className, props, isPKGuid: true, isSystematicEntity: true);
        GenerateFiles(contents: entityContent);
    }

    private static void CreateControllerBaseAndAllDependencies(string solutionName, string rootPath)
    {
        List<Content> baseControllerContent = ControllerRepository.GenerateControllerBaseAndAllDependencies(solutionName, rootPath);
        GenerateFiles(contents: baseControllerContent);
    }
    #endregion
}