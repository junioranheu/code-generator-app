using System.Text;
using CodeGenerator.Console.Models;
using static CodeGenerator.Console.Utils.Fixtures.Generate;
using static CodeGenerator.Console.Utils.Fixtures.Get;

namespace CodeGenerator.Console.Repositories;

public static class SolutionRepository
{
    public static void Generate(string solutionName, string rootPath, string contextName, List<Model> models)
    {
        string apiProject = $"{solutionName}.API";
        string applicationProject = $"{solutionName}.Application";
        string domainProject = $"{solutionName}.Domain";
        string infrastructureProject = $"{solutionName}.Infrastructure";

        GenerateFolder(solutionName, Path.Combine(rootPath, apiProject));
        GenerateFolder(solutionName, Path.Combine(rootPath, applicationProject));
        GenerateFolder(solutionName, Path.Combine(rootPath, domainProject));
        GenerateFolder(solutionName, Path.Combine(rootPath, infrastructureProject));

        Write(rootPath, $"{solutionName}.sln", GenerateSolutionFile(solutionName, apiProject, applicationProject, domainProject, infrastructureProject));
        Write(rootPath, Path.Combine(apiProject, $"{apiProject}.csproj"), GenerateApiProject(apiProject, applicationProject, infrastructureProject));
        Write(rootPath, Path.Combine(applicationProject, $"{applicationProject}.csproj"), GenerateApplicationProject(applicationProject, domainProject, infrastructureProject));
        Write(rootPath, Path.Combine(domainProject, $"{domainProject}.csproj"), GenerateLibraryProject(domainProject));
        Write(rootPath, Path.Combine(infrastructureProject, $"{infrastructureProject}.csproj"), GenerateInfrastructureProject(infrastructureProject, domainProject));

        Write(rootPath, Path.Combine(apiProject, "Program.cs"), GenerateApiProgram(solutionName, models));
        Write(rootPath, Path.Combine(apiProject, "appsettings.json"), "{\n  \"ConnectionStrings\": {\n    \"DefaultConnection\": \"Data Source=generated.db\"\n  },\n  \"Logging\": {\n    \"LogLevel\": {\n      \"Default\": \"Information\",\n      \"Microsoft.AspNetCore\": \"Warning\"\n    }\n  }\n}\n");
        Write(rootPath, Path.Combine(applicationProject, "UseCases", "Shared", "PaginationInput.cs"), GeneratePaginationInput(solutionName));
        Write(rootPath, Path.Combine(applicationProject, "UseCases", "Shared", "PagedQuery.cs"), GeneratePagedQuery(solutionName));
        Write(rootPath, Path.Combine(infrastructureProject, "Data", $"{contextName}.cs"), GenerateDbContext(solutionName, contextName, models));
        Write(rootPath, Path.Combine(infrastructureProject, "DependencyInjection.cs"), GenerateInfrastructureDependencyInjection(solutionName, infrastructureProject, contextName));
    }

    private static string GenerateApiProject(string apiProject, string applicationProject, string infrastructureProject) => $@"<Project Sdk=""Microsoft.NET.Sdk.Web"">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include=""..\\{applicationProject}\\{applicationProject}.csproj"" />
    <ProjectReference Include=""..\\{infrastructureProject}\\{infrastructureProject}.csproj"" />
    <PackageReference Include=""AutoMapper.Extensions.Microsoft.DependencyInjection"" Version=""12.0.1"" />
    <PackageReference Include=""Swashbuckle.AspNetCore"" Version=""6.6.2"" />
  </ItemGroup>
</Project>";

    private static string GenerateApplicationProject(string applicationProject, string domainProject, string infrastructureProject) => $@"<Project Sdk=""Microsoft.NET.Sdk""><PropertyGroup><TargetFramework>net10.0</TargetFramework><Nullable>enable</Nullable><ImplicitUsings>enable</ImplicitUsings></PropertyGroup><ItemGroup><ProjectReference Include=""..\\{domainProject}\\{domainProject}.csproj"" /><ProjectReference Include=""..\\{infrastructureProject}\\{infrastructureProject}.csproj"" /></ItemGroup></Project>";

    private static string GenerateLibraryProject(string projectName) => $@"<Project Sdk=""Microsoft.NET.Sdk""><PropertyGroup><TargetFramework>net10.0</TargetFramework><Nullable>enable</Nullable><ImplicitUsings>enable</ImplicitUsings></PropertyGroup></Project>";

    private static string GenerateInfrastructureProject(string infrastructureProject, string domainProject) => $@"<Project Sdk=""Microsoft.NET.Sdk""><PropertyGroup><TargetFramework>net10.0</TargetFramework><Nullable>enable</Nullable><ImplicitUsings>enable</ImplicitUsings></PropertyGroup><ItemGroup><ProjectReference Include=""..\\{domainProject}\\{domainProject}.csproj"" /><PackageReference Include=""Microsoft.EntityFrameworkCore.Sqlite"" Version=""10.0.0"" /><PackageReference Include=""Microsoft.EntityFrameworkCore.Design"" Version=""10.0.0""><PrivateAssets>all</PrivateAssets></PackageReference></ItemGroup></Project>";

    private static string GenerateSolutionFile(string solutionName, params string[] projects)
    {
        StringBuilder solution = new();
        solution.AppendLine("Microsoft Visual Studio Solution File, Format Version 12.00");
        solution.AppendLine("# Visual Studio Version 17");
        solution.AppendLine("VisualStudioVersion = 17.0.31903.59");
        solution.AppendLine("MinimumVisualStudioVersion = 10.0.40219.1");

        Dictionary<string, string> projectGuids = projects.ToDictionary(x => x, _ => Guid.NewGuid().ToString("B").ToUpperInvariant());
        foreach (string project in projects)
        {
            string projectType = project.EndsWith(".API", StringComparison.Ordinal) ? "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}" : "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
            solution.AppendLine($"Project(\"{projectType}\") = \"{project}\", \"{project}\\{project}.csproj\", \"{projectGuids[project]}\"");
            solution.AppendLine("EndProject");
        }

        solution.AppendLine("Global");
        solution.AppendLine("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
        solution.AppendLine("\t\tDebug|Any CPU = Debug|Any CPU");
        solution.AppendLine("\t\tRelease|Any CPU = Release|Any CPU");
        solution.AppendLine("\tEndGlobalSection");
        solution.AppendLine("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");
        foreach (string guid in projectGuids.Values)
        {
            solution.AppendLine($"\t\t{guid}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
            solution.AppendLine($"\t\t{guid}.Debug|Any CPU.Build.0 = Debug|Any CPU");
            solution.AppendLine($"\t\t{guid}.Release|Any CPU.ActiveCfg = Release|Any CPU");
            solution.AppendLine($"\t\t{guid}.Release|Any CPU.Build.0 = Release|Any CPU");
        }
        solution.AppendLine("\tEndGlobalSection");
        solution.AppendLine("EndGlobal");
        return solution.ToString();
    }

    private static string GenerateApiProgram(string solutionName, List<Model> models)
    {
        StringBuilder useCaseUsings = new();
        StringBuilder registrations = new();
        foreach (Model model in models)
        {
            useCaseUsings.AppendLine($"using {solutionName}.Application.UseCases.{model.Name};");
        }
        foreach (Model model in models)
        {
            registrations.AppendLine($"builder.Services.Add{GetStrPlural(model.Name)}Application();");
        }

        return string.Join(Environment.NewLine, [
            $"using {solutionName}.Infrastructure;",
            useCaseUsings.ToString(),
            "",
            "var builder = WebApplication.CreateBuilder(args);",
            "builder.Services.AddControllers();",
            "builder.Services.AddEndpointsApiExplorer();",
            "builder.Services.AddSwaggerGen();",
            "builder.Services.AddInfrastructure(builder.Configuration);",
            registrations.ToString(),
            "var app = builder.Build();",
            "if (app.Environment.IsDevelopment())",
            "{",
            "    app.UseSwagger();",
            "    app.UseSwaggerUI();",
            "}",

            "app.UseHttpsRedirection();",
            "app.MapControllers();",
            "app.Run();"
        ]);
    }

    private static string GeneratePaginationInput(string solutionName) => string.Join(Environment.NewLine, [
        $"namespace {solutionName}.Application.UseCases.Shared;",
        "",
        "public sealed class PaginationInput",
        "{",
        "    public int Page { get; set; } = 1;",
        "    public int PageSize { get; set; } = 20;",
        "}"
    ]);

    private static string GeneratePagedQuery(string solutionName) => string.Join(Environment.NewLine, [
        "using Microsoft.EntityFrameworkCore;",
        "",
        $"namespace {solutionName}.Application.UseCases.Shared;",
        "",
        "public static class PagedQuery",
        "{",
        "    public static async Task<(IEnumerable<T> linq, int count)> Execute<T>(IQueryable<T> query, PaginationInput pagination)",
        "    {",
        "        int count = await query.CountAsync();",
        "        IEnumerable<T> linq = await query.Skip((pagination.Page - 1) * pagination.PageSize).Take(pagination.PageSize).ToListAsync();",
        "        return (linq, count);",
        "    }",
        "}"
    ]);

    private static string GenerateDbContext(string solutionName, string contextName, List<Model> models)
    {
        StringBuilder content = new();
        content.AppendLine($"using {solutionName}.Domain.Entities;");
        content.AppendLine("using Microsoft.EntityFrameworkCore;");
        content.AppendLine();
        content.AppendLine($"namespace {solutionName}.Infrastructure.Data;");
        content.AppendLine();
        content.AppendLine($"public sealed class {contextName}(DbContextOptions<{contextName}> options) : DbContext(options)");
        content.AppendLine("{");
        foreach (Model model in models)
        {
            content.AppendLine($"    public DbSet<{model.Name}> {GetStrPlural(model.Name)} => Set<{model.Name}>();");
        }
        content.AppendLine("}");
        return content.ToString();
    }

    private static string GenerateInfrastructureDependencyInjection(string solutionName, string infrastructureProject, string contextName) => string.Join(Environment.NewLine, [
        "using Microsoft.EntityFrameworkCore;",
        "using Microsoft.Extensions.Configuration;",
        "using Microsoft.Extensions.DependencyInjection;",
        $"using {solutionName}.Infrastructure.Data;",
        "",
        $"namespace {solutionName}.Infrastructure;",
        "",
        "public static class DependencyInjection",
        "{",
        "    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)",
        "    {",
        $"        services.AddDbContext<{contextName}>(options => options.UseSqlite(configuration.GetConnectionString(\"DefaultConnection\")));",
        "        return services;",
        "    }",
        "}"
    ]);

    private static void Write(string rootPath, string relativePath, string content)
    {
        string path = Path.Combine(rootPath, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content.TrimEnd());
    }
}