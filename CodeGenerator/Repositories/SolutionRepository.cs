using CodeGenerator.Console.Consts;
using CodeGenerator.Console.Models;
using System.Text;
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
        string apiName = $"{solutionName}.API";

        GenerateFolder(solutionName, Path.Combine(rootPath, apiProject));
        GenerateFolder(solutionName, Path.Combine(rootPath, applicationProject));
        GenerateFolder(solutionName, Path.Combine(rootPath, domainProject));
        GenerateFolder(solutionName, Path.Combine(rootPath, infrastructureProject));

        Write(rootPath, $"{solutionName}.sln", GenerateSolutionFile(apiProject, applicationProject, domainProject, infrastructureProject));
        Write(rootPath, Path.Combine(apiProject, $"{apiProject}.csproj"), GenerateApiProject(applicationProject, infrastructureProject));
        Write(rootPath, Path.Combine(applicationProject, $"{applicationProject}.csproj"), GenerateApplicationProject(domainProject, infrastructureProject));
        Write(rootPath, Path.Combine(domainProject, $"{domainProject}.csproj"), GenerateLibraryProject());
        Write(rootPath, Path.Combine(infrastructureProject, $"{infrastructureProject}.csproj"), GenerateInfrastructureProject(domainProject));

        Write(rootPath, Path.Combine(apiProject, "Program.cs"), GenerateApiProgram(solutionName, apiName));
        Write(rootPath, Path.Combine(apiProject, "appsettings.json"), GenerateAppSettingsJson());
        Write(rootPath, Path.Combine(apiProject, "DependencyInjection.cs"), GenerateAPIDependencyInjection(solutionName, apiName));
        Write(rootPath, Path.Combine(apiProject, "DependencyAppConfiguration.cs"), GenerateAPIAppConfigurationDependencyInjection(solutionName, contextName));

        Write(rootPath, Path.Combine(applicationProject, "UseCases", "Shared", "PaginationInput.cs"), GeneratePaginationInput(solutionName));
        Write(rootPath, Path.Combine(applicationProject, "UseCases", "Shared", "PagedQuery.cs"), GeneratePagedQuery(solutionName));
        Write(rootPath, Path.Combine(applicationProject, "DependencyInjection.cs"), GenerateApplicationDependencyInjection(solutionName, contextName));

        Write(rootPath, Path.Combine(infrastructureProject, "Data", $"{contextName}.cs"), GenerateDbContext(solutionName, contextName, models));
        Write(rootPath, Path.Combine(infrastructureProject, "DependencyInjection.cs"), GenerateInfrastructureDependencyInjection(solutionName, contextName));
    }

    private static string GenerateApiProject(string applicationProject, string infrastructureProject)
    {
        StringBuilder sb = new();

        sb.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk.Web\"> ");
        sb.AppendLine("  <PropertyGroup>");
        sb.AppendLine($"    <TargetFramework>{Misc.TargetFramework}</TargetFramework>");
        sb.AppendLine("    <Nullable>enable</Nullable>");
        sb.AppendLine("    <ImplicitUsings>enable</ImplicitUsings>");
        sb.AppendLine("  </PropertyGroup>");
        sb.AppendLine("  <ItemGroup>");
        sb.AppendLine($"    <ProjectReference Include=\"..\\{applicationProject}\\{applicationProject}.csproj\" />");
        sb.AppendLine($"    <ProjectReference Include=\"..\\{infrastructureProject}\\{infrastructureProject}.csproj\" />");
        sb.AppendLine($"    <PackageReference Include=\"AutoMapper.Extensions.Microsoft.DependencyInjection\" Version=\"{Misc.AutoMapperVersion}\" />");
        sb.AppendLine($"    <PackageReference Include=\"Swashbuckle.AspNetCore\" Version=\"{Misc.SwaggerVersion}\" />");
        sb.AppendLine("  </ItemGroup>");
        sb.AppendLine("</Project>");

        return sb.ToString();
    }

    private static string GenerateApplicationProject(string domainProject, string infrastructureProject)
    {
        StringBuilder sb = new();

        sb.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\"> ");
        sb.AppendLine("  <PropertyGroup>");
        sb.AppendLine($"    <TargetFramework>{Misc.TargetFramework}</TargetFramework>");
        sb.AppendLine("    <Nullable>enable</Nullable>");
        sb.AppendLine("    <ImplicitUsings>enable</ImplicitUsings>");
        sb.AppendLine("  </PropertyGroup>");
        sb.AppendLine("  <ItemGroup>");
        sb.AppendLine($"    <ProjectReference Include=\"..\\{domainProject}\\{domainProject}.csproj\" />");
        sb.AppendLine($"    <ProjectReference Include=\"..\\{infrastructureProject}\\{infrastructureProject}.csproj\" />");
        sb.AppendLine("  </ItemGroup>");
        sb.AppendLine("</Project>");

        return sb.ToString();
    }

    private static string GenerateLibraryProject()
    {
        StringBuilder sb = new();

        sb.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\"> ");
        sb.AppendLine("  <PropertyGroup>");
        sb.AppendLine($"    <TargetFramework>{Misc.TargetFramework}</TargetFramework>");
        sb.AppendLine("    <Nullable>enable</Nullable>");
        sb.AppendLine("    <ImplicitUsings>enable</ImplicitUsings>");
        sb.AppendLine("  </PropertyGroup>");
        sb.AppendLine("</Project>");

        return sb.ToString();
    }

    private static string GenerateInfrastructureProject(string domainProject)
    {
        StringBuilder sb = new();

        sb.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\"> ");
        sb.AppendLine("  <PropertyGroup>");
        sb.AppendLine($"    <TargetFramework>{Misc.TargetFramework}</TargetFramework>");
        sb.AppendLine("    <Nullable>enable</Nullable>");
        sb.AppendLine("    <ImplicitUsings>enable</ImplicitUsings>");
        sb.AppendLine("  </PropertyGroup>");
        sb.AppendLine("  <ItemGroup>");
        sb.AppendLine($"    <ProjectReference Include=\"..\\{domainProject}\\{domainProject}.csproj\" />");
        sb.AppendLine($"    <PackageReference Include=\"Microsoft.EntityFrameworkCore.Sqlite\" Version=\"{Misc.EntityFrameworkVersion}\" />");
        sb.AppendLine($"    <PackageReference Include=\"Microsoft.EntityFrameworkCore.Design\" Version=\"{Misc.EntityFrameworkVersion}\">\n      <PrivateAssets>all</PrivateAssets></PackageReference>");
        sb.AppendLine($"    <FrameworkReference Include=\"Microsoft.AspNetCore.App\" />");
        sb.AppendLine("  </ItemGroup>");
        sb.AppendLine("</Project>");

        return sb.ToString();
    }

    private static string GenerateSolutionFile(params string[] projects)
    {
        StringBuilder solution = new();
        solution.AppendLine($"Microsoft Visual Studio Solution File, Format Version {Misc.SolutionFormatVersion}");
        solution.AppendLine("# Visual Studio Version 17");
        solution.AppendLine($"VisualStudioVersion = {Misc.VisualStudioVersion}");
        solution.AppendLine($"MinimumVisualStudioVersion = {Misc.MinimumVisualStudioVersion}");

        Dictionary<string, string> projectGuids = projects.ToDictionary(x => x, _ => Guid.NewGuid().ToString("B").ToUpperInvariant());

        foreach (string project in projects)
        {
            solution.AppendLine($"Project(\"{Misc.CSharpProjectTypeGuid}\") = \"{project}\", \"{project}\\{project}.csproj\", \"{projectGuids[project]}\"");
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

    private static string GenerateApiProgram(string solutionName, string apiName)
    {
        StringBuilder content = new();

        content.AppendLine($"using {solutionName}.Infrastructure;");
        content.AppendLine($"using {solutionName}.Application;");
        content.AppendLine($"using {solutionName}.Domain;");
        content.AppendLine();
        content.AppendLine($"Console.Title = \"{apiName}\";");
        content.AppendLine();
        content.AppendLine("WebApplicationBuilder builder = WebApplication.CreateBuilder(args);");
        content.AppendLine("{");
        content.AppendLine("    builder.Services.AddDependencyInjectionAPI(builder);");
        content.AppendLine("    builder.Services.AddDependencyInjectionApplication(builder);");
        content.AppendLine("    builder.Services.AddDependencyInjectionInfrastructure(builder);");
        content.AppendLine("}");
        content.AppendLine();
        content.AppendLine("WebApplication app = builder.Build();");
        content.AppendLine("{");
        content.AppendLine("    await app.UseAppConfiguration(builder);");
        content.AppendLine("    app.Run();");
        content.AppendLine("}");

        return content.ToString();
    }

    private static string GenerateAppSettingsJson()
    {
        StringBuilder sb = new();

        sb.AppendLine("{");
        sb.AppendLine("  \"ConnectionStrings\": {");
        sb.AppendLine("    \"DefaultConnection\": \"Data Source=generated.db\"");
        sb.AppendLine("  },");
        sb.AppendLine("  \"Logging\": {");
        sb.AppendLine("    \"LogLevel\": {");
        sb.AppendLine("      \"Default\": \"Information\",");
        sb.AppendLine("      \"Microsoft.AspNetCore\": \"Warning\"");
        sb.AppendLine("    }");
        sb.AppendLine("  },");
        sb.AppendLine("  \"CORSSettings\": {");
        sb.AppendLine("     \"Cors\": \"xxx\",");
        sb.AppendLine("  },");
        sb.AppendLine("  \"URLs\": {");
        sb.AppendLine("    \"Development\": {");
        sb.AppendLine("      \"Frontend\": \"xxx\"");
        sb.AppendLine("    },");
        sb.AppendLine("    \"Production\": {");
        sb.AppendLine("      \"Frontend\": \"xxx\"");
        sb.AppendLine("    }");
        sb.AppendLine("  }");
        sb.AppendLine("} // made by @junioranheu");


        return sb.ToString();
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

        content.AppendLine("using System.Security.Claims;");
        content.AppendLine("using Microsoft.AspNetCore.Http;");
        content.AppendLine("using Microsoft.EntityFrameworkCore;");
        content.AppendLine("using Microsoft.EntityFrameworkCore.Storage.ValueConversion;");
        content.AppendLine($"using {solutionName}.Domain.Entities;");
        content.AppendLine();
        content.AppendLine($"namespace {solutionName}.Infrastructure.Data;");
        content.AppendLine();
        content.AppendLine($"public class {contextName}(DbContextOptions<{contextName}> options, IHttpContextAccessor httpContextAccessor) : DbContext(options)");
        content.AppendLine("{");

        // DbSet props;
        foreach (Model model in models)
        {
            content.AppendLine($"    public DbSet<{model.Name}> {GetStrPlural(model.Name)} {{ get; set; }}");
        }

        content.AppendLine();
        content.AppendLine("    #region extras");
        content.AppendLine("    protected override void OnModelCreating(ModelBuilder modelBuilder)");
        content.AppendLine("    {");
        content.AppendLine("        #region delete_behavior");
        content.AppendLine("        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))");
        content.AppendLine("        {");
        content.AppendLine("            relationship.DeleteBehavior = DeleteBehavior.Cascade;");
        content.AppendLine("        }");
        content.AppendLine("        #endregion");
        content.AppendLine();
        content.AppendLine("        #region postgreSQL_datetime_normalize_utc");
        content.AppendLine("        var utcConverter = new ValueConverter<DateTime, DateTime>(");
        content.AppendLine("            x => x.Kind == DateTimeKind.Utc ? x : x.ToUniversalTime(),");
        content.AppendLine("            x => DateTime.SpecifyKind(x, DateTimeKind.Utc)");
        content.AppendLine("        );");
        content.AppendLine();
        content.AppendLine("        foreach (var entityType in modelBuilder.Model.GetEntityTypes())");
        content.AppendLine("        {");
        content.AppendLine("            foreach (var property in entityType.GetProperties())");
        content.AppendLine("            {");
        content.AppendLine("                if (property.ClrType == typeof(DateTime))");
        content.AppendLine("                {");
        content.AppendLine("                    property.SetValueConverter(utcConverter);");
        content.AppendLine("                }");
        content.AppendLine("                else if (property.ClrType == typeof(DateTime?))");
        content.AppendLine("                {");
        content.AppendLine("                    property.SetValueConverter(");
        content.AppendLine("                        new ValueConverter<DateTime?, DateTime?>(");
        content.AppendLine("                            x => x.HasValue ? (x.Value.Kind == DateTimeKind.Utc ? x.Value : x.Value.ToUniversalTime()) : x, ");
        content.AppendLine("                            x => x.HasValue ? DateTime.SpecifyKind(x.Value, DateTimeKind.Utc) : x");
        content.AppendLine("                        )");
        content.AppendLine("                    );");
        content.AppendLine("                }");
        content.AppendLine("            }");
        content.AppendLine("        }");
        content.AppendLine("        #endregion");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    private Guid UserIdAuth");
        content.AppendLine("    {");
        content.AppendLine("        get");
        content.AppendLine("        {");
        content.AppendLine("            ClaimsPrincipal? user = httpContextAccessor.HttpContext?.User;");
        content.AppendLine();
        content.AppendLine("            if (user?.Identity?.IsAuthenticated ?? false)");
        content.AppendLine("            {");
        content.AppendLine("                string? userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;");
        content.AppendLine();
        content.AppendLine("                if (Guid.TryParse(userIdClaim, out Guid userIdAuth))");
        content.AppendLine("                {");
        content.AppendLine("                    return userIdAuth;");
        content.AppendLine("                }");
        content.AppendLine("            }");
        content.AppendLine();
        content.AppendLine("            return Guid.Empty;");
        content.AppendLine("        }");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    private void ApplyAuditLogRules()");
        content.AppendLine("    {");
        content.AppendLine("        foreach (var entry in ChangeTracker.Entries<Audit>())");
        content.AppendLine("        {");
        content.AppendLine("            if (entry.Entity is Audit audit)");
        content.AppendLine("            {");
        content.AppendLine("                switch (entry.State)");
        content.AppendLine("                {");
        content.AppendLine("                    case EntityState.Added:");
        content.AppendLine("                        if (audit.CreatedDate is null)");
        content.AppendLine("                        {");
        content.AppendLine("                            audit.CreatedDate = DateTime.UtcNow;");
        content.AppendLine("                            audit.CreatedBy = UserIdAuth;");
        content.AppendLine("                            audit.Status = true;");
        content.AppendLine("                        }");
        content.AppendLine();
        content.AppendLine("                        break;");
        content.AppendLine();
        content.AppendLine("                    case EntityState.Modified:");
        content.AppendLine("                        audit.LastModificationDate = DateTime.UtcNow;");
        content.AppendLine("                        audit.LastModificationBy = UserIdAuth;");
        content.AppendLine();
        content.AppendLine("                        break;");
        content.AppendLine("                }");
        content.AppendLine("            }");
        content.AppendLine("        }");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)");
        content.AppendLine("    {");
        content.AppendLine("        ApplyAuditLogRules();");
        content.AppendLine("        return base.SaveChangesAsync(cancellationToken);");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    public override int SaveChanges()");
        content.AppendLine("    {");
        content.AppendLine("        ApplyAuditLogRules();");
        content.AppendLine("        return base.SaveChanges();");
        content.AppendLine("    }");
        content.AppendLine("    #endregion");
        content.AppendLine("}");
        content.AppendLine();

        return content.ToString();
    }

    private static string GenerateAPIDependencyInjection(string solutionName, string apiName)
    {
        StringBuilder content = new();

        content.AppendLine("using Microsoft.AspNetCore.ResponseCompression;");
        content.AppendLine("using System.IO.Compression;");
        content.AppendLine("using System.Text.Json.Serialization;");
        content.AppendLine();
        content.AppendLine($"namespace {solutionName}.API;");
        content.AppendLine();
        content.AppendLine("public static class DependencyInjection");
        content.AppendLine("{");
        content.AppendLine("    public static IServiceCollection AddDependencyInjectionAPI(this IServiceCollection services, WebApplicationBuilder builder)");
        content.AppendLine("    {");
        content.AppendLine("        IWebHostEnvironment env = builder.Environment;");
        content.AppendLine();
        content.AppendLine("        AddSwagger(services);");
        content.AppendLine("        AddCors(services, builder);");
        content.AppendLine("        AddCompression(services);");
        content.AppendLine("        AddControllers(services, env);");
        content.AppendLine("        AddCaching(services);");
        content.AppendLine("        AddHttpContextAccessor(services);");
        content.AppendLine();
        content.AppendLine("        return services;");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    private static void AddSwagger(IServiceCollection services)");
        content.AppendLine("    {");
        content.AppendLine("        services.AddSwaggerGen(c =>");
        content.AppendLine("        {");
        content.AppendLine($"            c.SwaggerDoc(\"v1\", new() {{ Title = \"{apiName}\", Version = \"v1\" }});");
        content.AppendLine("        });");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    private static void AddCors(IServiceCollection services, WebApplicationBuilder builder)");
        content.AppendLine("    {");
        content.AppendLine("        string[] frontendUrls =");
        content.AppendLine("        [");
        content.AppendLine("            builder.Configuration[\"Urls:Development:Frontend\"] ?? string.Empty,");
        content.AppendLine("            builder.Configuration[\"Urls:Production:Frontend\"] ?? string.Empty");
        content.AppendLine("        ];");
        content.AppendLine();
        content.AppendLine("        if (frontendUrls == null || frontendUrls.Any(x => string.IsNullOrEmpty(x)))");
        content.AppendLine("        {");
        content.AppendLine("            throw new InvalidOperationException(\"Critical internal error: one or more Frontend URLs are not configured in appsettings.json.\");");
        content.AppendLine("        }");
        content.AppendLine();
        content.AppendLine("        services.AddCors(x =>");
        content.AppendLine("            x.AddPolicy(name: builder.Configuration[\"CORSSettings:Cors\"] ?? string.Empty, policyBuilder =>");
        content.AppendLine("            {");
        content.AppendLine("                policyBuilder.WithOrigins(frontendUrls).AllowAnyHeader().AllowAnyMethod().AllowCredentials();");
        content.AppendLine("            })");
        content.AppendLine("        );");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    private static void AddCompression(IServiceCollection services)");
        content.AppendLine("    {");
        content.AppendLine("        services.AddResponseCompression(options =>");
        content.AppendLine("        {");
        content.AppendLine("            options.EnableForHttps = true;");
        content.AppendLine("            options.Providers.Add<BrotliCompressionProvider>();");
        content.AppendLine("            options.Providers.Add<GzipCompressionProvider>();");
        content.AppendLine("        });");
        content.AppendLine();
        content.AppendLine("        services.Configure<BrotliCompressionProviderOptions>(x => x.Level = CompressionLevel.Optimal);");
        content.AppendLine("        services.Configure<GzipCompressionProviderOptions>(x => x.Level = CompressionLevel.Optimal);");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    private static void AddControllers(IServiceCollection services, IWebHostEnvironment env)");
        content.AppendLine("    {");
        content.AppendLine("        services.AddControllers(options =>");
        content.AppendLine("        {");
        content.AppendLine("            //");
        content.AppendLine("        })");
        content.AppendLine("        .AddJsonOptions(x =>");
        content.AppendLine("        {");
        content.AppendLine("            x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;");
        content.AppendLine("            x.JsonSerializerOptions.WriteIndented = env.IsDevelopment();");
        content.AppendLine("        });");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    private static void AddCaching(IServiceCollection services)");
        content.AppendLine("    {");
        content.AppendLine("        services.AddMemoryCache();");
        content.AppendLine("        services.AddResponseCaching();");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    private static void AddHttpContextAccessor(IServiceCollection services)");
        content.AppendLine("    {");
        content.AppendLine("        services.AddHttpContextAccessor();");
        content.AppendLine("    }");
        content.AppendLine("}");

        return content.ToString();
    }

    private static string GenerateAPIAppConfigurationDependencyInjection(string solutionName, string contextName)
    {
        StringBuilder content = new();

        return content.ToString();
    }

    private static string GenerateApplicationDependencyInjection(string solutionName, string contextName)
    {
        StringBuilder content = new();

        return content.ToString();
    }

    private static string GenerateInfrastructureDependencyInjection(string solutionName, string contextName)
    {
        StringBuilder content = new();

        content.AppendLine("using Microsoft.EntityFrameworkCore;");
        content.AppendLine("using Microsoft.Extensions.Configuration;");
        content.AppendLine("using Microsoft.Extensions.DependencyInjection;");
        content.AppendLine($"using {solutionName}.Infrastructure.Data;");
        content.AppendLine();
        content.AppendLine($"namespace {solutionName}.Infrastructure;");
        content.AppendLine();
        content.AppendLine("public static class DependencyInjection");
        content.AppendLine("{");
        content.AppendLine("    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)");
        content.AppendLine("    {");
        content.AppendLine($"       services.AddDbContext<{contextName}>(options => options.UseSqlite(configuration.GetConnectionString(\"DefaultConnection\"))); ");
        content.AppendLine("        return services;");
        content.AppendLine("    }");
        content.AppendLine("}");

        return content.ToString();
    }

    private static void Write(string rootPath, string relativePath, string content)
    {
        string path = Path.Combine(rootPath, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content.TrimEnd());
    }
}