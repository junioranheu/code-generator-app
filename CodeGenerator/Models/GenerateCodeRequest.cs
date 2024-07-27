namespace CodeGenerator.Console.Models;

public sealed class GenerateCodeRequest
{
    public required string SolutionName { get; set; } = string.Empty;
    public required string ContextName { get; set; } = string.Empty;
    public required bool IsPKGuid { get; set; }
    public required List<Model> Models { get; set; } 
    public required bool IsGenerateZip { get; set; }

    public void Deconstruct(out string solutionName, out string contextName, out bool isPKGuid, out List<Model> models, out bool isGenerateZip)
    {
        solutionName = SolutionName;
        contextName = ContextName;
        isPKGuid = IsPKGuid;
        models = Models;
        isGenerateZip = IsGenerateZip;
    }
}