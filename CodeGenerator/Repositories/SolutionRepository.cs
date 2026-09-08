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
        Write(rootPath, Path.Combine(apiProject, "DependencyAppConfiguration.cs"), GenerateAPIAppConfigurationDependencyInjection(solutionName, apiName));

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
        sb.AppendLine("    <RestoreSources>https://api.nuget.org/v3/index.json;$(RestoreSources)</RestoreSources>");
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
        sb.AppendLine($"    <PackageReference Include=\"System.IdentityModel.Tokens.Jwt\" Version=\"{Misc.IdentityModelVersion}\" />");
        sb.AppendLine($"    <PackageReference Include=\"Microsoft.AspNetCore.Authentication.JwtBearer\" Version=\"{Misc.JwtBearerVersion}\" />");
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

    private static string GenerateAPIAppConfigurationDependencyInjection(string solutionName, string apiName)
    {
        StringBuilder content = new();

        content.AppendLine("using Microsoft.AspNetCore.Mvc.Controllers;");
        content.AppendLine("using Swashbuckle.AspNetCore.SwaggerUI;");
        content.AppendLine();
        content.AppendLine($"namespace {solutionName}.API;");
        content.AppendLine();
        content.AppendLine("public static class DependencyAppConfiguration");
        content.AppendLine("{");
        content.AppendLine("    public static async Task<WebApplication> UseAppConfiguration(this WebApplication app, WebApplicationBuilder builder)");
        content.AppendLine("    {");
        content.AppendLine("        AddMiddleware(app);");
        content.AppendLine("        AddSwagger(app);");
        content.AppendLine("        AddHttpsRedirection(app);");
        content.AppendLine("        AddCors(app, builder);");
        content.AppendLine("        AddCompression(app);");
        content.AppendLine("        AddAuth(app);");
        content.AppendLine("        AddCaching(app);");
        content.AppendLine("        AddDeveloperExceptionPage(app);");
        content.AppendLine("        await HandleDbInitialize(app);");
        content.AppendLine();
        content.AppendLine("        return app;");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    private static void AddMiddleware(WebApplication app)");
        content.AppendLine("    {");
        content.AppendLine("\t\t// TO DO;");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    private static void AddSwagger(WebApplication app)");
        content.AppendLine("    {");
        content.AppendLine("        if (app.Environment.IsDevelopment())");
        content.AppendLine("        {");
        content.AppendLine("            app.UseSwagger();");
        content.AppendLine();
        content.AppendLine("            app.UseSwaggerUI(c =>");
        content.AppendLine("            {");
        content.AppendLine($"                c.SwaggerEndpoint(\"/swagger/v1/swagger.json\", \"{apiName}\");");
        content.AppendLine("                c.DocExpansion(DocExpansion.None);");
        content.AppendLine();
        content.AppendLine("                if (OperatingSystem.IsMacOS() || OperatingSystem.IsWindows())");
        content.AppendLine("                {");
        content.AppendLine("                    c.RoutePrefix = string.Empty;");
        content.AppendLine("                }");
        content.AppendLine("            });");
        content.AppendLine("        }");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    private static void AddHttpsRedirection(WebApplication app)");
        content.AppendLine("    {");
        content.AppendLine("        if (app.Environment.IsProduction())");
        content.AppendLine("        {");
        content.AppendLine("            app.UseHttpsRedirection();");
        content.AppendLine("        }");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    private static void AddCors(WebApplication app, WebApplicationBuilder builder)");
        content.AppendLine("    {");
        content.AppendLine("        app.UseCors(builder.Configuration[\"CORSSettings:Cors\"] ?? string.Empty);");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    private static void AddCompression(WebApplication app)");
        content.AppendLine("    {");
        content.AppendLine("        /// <summary>");
        content.AppendLine("        /// O trecho \"app.UseWhen\" abaixo é necessário quando a API tem uma resposta IAsyncEnumerable/Yield;");
        content.AppendLine("        /// O \"UseResponseCompression\" conflita com esse tipo de requisição, portanto é obrigatória a verificação abaixo;");
        content.AppendLine("        /// Caso não existam requisições desse tipo na API, é apenas necessário o trecho \"app.UseResponseCompression()\";");
        content.AppendLine("        /// </summary>");
        content.AppendLine("        app.UseWhen(context => !IsStreamingRequest(context), x =>");
        content.AppendLine("        {");
        content.AppendLine("            x.UseResponseCompression();");
        content.AppendLine("        });");
        content.AppendLine();
        content.AppendLine("        static bool IsStreamingRequest(HttpContext context)");
        content.AppendLine("        {");
        content.AppendLine("            Endpoint? endpoint = context.GetEndpoint();");
        content.AppendLine();
        content.AppendLine("            if (endpoint is RouteEndpoint routeEndpoint)");
        content.AppendLine("            {");
        content.AppendLine("                ControllerActionDescriptor? action = routeEndpoint.Metadata.GetMetadata<ControllerActionDescriptor>();");
        content.AppendLine();
        content.AppendLine("                if (action is not null)");
        content.AppendLine("                {");
        content.AppendLine("                    Type? tipo = action.MethodInfo.ReturnType;");
        content.AppendLine();
        content.AppendLine("                    if (tipo.IsGenericType && tipo.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>))");
        content.AppendLine("                    {");
        content.AppendLine("                        return true;");
        content.AppendLine("                    }");
        content.AppendLine();
        content.AppendLine("                    return false;");
        content.AppendLine("                }");
        content.AppendLine("            }");
        content.AppendLine();
        content.AppendLine("            return false;");
        content.AppendLine("        }");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    private static void AddAuth(WebApplication app)");
        content.AppendLine("    {");
        content.AppendLine("        app.UseAuthentication();");
        content.AppendLine("        app.UseAuthorization();");
        content.AppendLine("        app.MapControllers();");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    private static void AddCaching(WebApplication app)");
        content.AppendLine("    {");
        content.AppendLine("        app.UseResponseCaching();");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    private static void AddDeveloperExceptionPage(WebApplication app)");
        content.AppendLine("    {");
        content.AppendLine("        if (app.Environment.IsDevelopment())");
        content.AppendLine("        {");
        content.AppendLine("            app.UseDeveloperExceptionPage();");
        content.AppendLine("        }");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    private static async Task HandleDbInitialize(WebApplication app)");
        content.AppendLine("    {");
        content.AppendLine("\t\t// TO DO;");
        content.AppendLine("    }");
        content.AppendLine("}");

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

        content.AppendLine("using Microsoft.AspNetCore.Authentication.JwtBearer;");
        content.AppendLine("using Microsoft.AspNetCore.Builder;");
        content.AppendLine("using Microsoft.AspNetCore.Hosting;");
        content.AppendLine("using Microsoft.AspNetCore.Http;");
        content.AppendLine("using Microsoft.EntityFrameworkCore;");
        content.AppendLine("using Microsoft.Extensions.Configuration;");
        content.AppendLine("using Microsoft.Extensions.DependencyInjection;");
        content.AppendLine("using Microsoft.Extensions.Hosting;");
        content.AppendLine("using Microsoft.IdentityModel.Tokens;");
        content.AppendLine($"using {solutionName}.Infrastructure.Auth.Models;");
        content.AppendLine($"using {solutionName}.Infrastructure.Auth.Token;");
        content.AppendLine($"using {solutionName}.Infrastructure.Data;");
        content.AppendLine($"using {solutionName}.Infrastructure.Factory;");
        content.AppendLine($"using {solutionName}.Infrastructure.Factory.DataBase;");
        content.AppendLine($"using {solutionName}.Infrastructure.Interceptors;");
        content.AppendLine($"using {solutionName}.Infrastructure.Services.Env;");
        content.AppendLine("using System.Text;");
        content.AppendLine("using System.Text.Json;");
        content.AppendLine();
        content.AppendLine($"namespace {solutionName}.Infrastructure;");
        content.AppendLine();
        content.AppendLine("public static class DependencyInjection");
        content.AppendLine("{");
        content.AppendLine("    public static IServiceCollection AddDependencyInjectionInfrastructure(this IServiceCollection services, WebApplicationBuilder builder)");
        content.AppendLine("    {");
        content.AppendLine("        AddServices(services, builder);");
        content.AppendLine("        AddAuth(services, builder);");
        content.AppendLine("        AddFactory(services, builder);");
        content.AppendLine("        AddContext(services, builder);");
        content.AppendLine("        AddJobs(services);");
        content.AppendLine();
        content.AppendLine("        return services;");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    private static void AddServices(IServiceCollection services, WebApplicationBuilder builder)");
        content.AppendLine("    {");
        content.AppendLine("        // JWT;");
        content.AppendLine("        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();");
        content.AppendLine("        services.Configure<JwtSettings>(builder.Configuration.GetSection(\"JwtSettings\"));");
        content.AppendLine();
        content.AppendLine("        // Env;");
        content.AppendLine("        services.AddSingleton<IEnvService>(x =>");
        content.AppendLine("        {");
        content.AppendLine("            IWebHostEnvironment env = builder.Environment;");
        content.AppendLine("            IConfiguration config = builder.Configuration;");
        content.AppendLine();
        content.AppendLine("            return new EnvService(env, config);");
        content.AppendLine("        });");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    private static readonly string[] OnAuthenticationFailed = [\"Sua sessão expirou. Por favor, realize o login novamente para continuar.\"]; ");
        content.AppendLine("    private static readonly string[] onChallengeError = [\"Acesso negado. É necessário estar autenticado para acessar este recurso.\"]; ");
        content.AppendLine();
        content.AppendLine("    private static void AddAuth(IServiceCollection services, WebApplicationBuilder builder)");
        content.AppendLine("    {");
        content.AppendLine("        services.AddAuthentication(x =>");
        content.AppendLine("        {");
        content.AppendLine("            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;");
        content.AppendLine("            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;");
        content.AppendLine("        })");
        content.AppendLine("             .AddJwtBearer(x =>");
        content.AppendLine("             {");
        content.AppendLine("                 x.RequireHttpsMetadata = !builder.Environment.IsDevelopment();");
        content.AppendLine("                 x.SaveToken = true;");
        content.AppendLine("                 x.IncludeErrorDetails = true;");
        content.AppendLine("                 x.TokenValidationParameters = new TokenValidationParameters");
        content.AppendLine("                 {");
        content.AppendLine("                     ValidateIssuerSigningKey = true,");
        content.AppendLine("                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration[\"JwtSettings:Secret\"] ?? string.Empty)),");
        content.AppendLine("                     ValidateIssuer = true,");
        content.AppendLine("                     ValidIssuer = builder.Configuration[\"JwtSettings:Issuer\"],");
        content.AppendLine("                     ValidateAudience = true,");
        content.AppendLine("                     ValidAudience = builder.Configuration[\"JwtSettings:Audience\"],");
        content.AppendLine("                     ValidateLifetime = true,");
        content.AppendLine("                     ClockSkew = TimeSpan.Zero");
        content.AppendLine("                 }; ");
        content.AppendLine();
        content.AppendLine("                 x.Events = new JwtBearerEvents");
        content.AppendLine("                 {");
        content.AppendLine("                     OnMessageReceived = context =>");
        content.AppendLine("                     {");
        content.AppendLine("                         if (context.HttpContext.Items.TryGetValue(SystemConsts.Cookies.Refresh, out object? refreshed) && refreshed is string refreshedToken && !string.IsNullOrEmpty(refreshedToken))");
        content.AppendLine("                         {");
        content.AppendLine("                             context.Token = refreshedToken;");
        content.AppendLine("                             return Task.CompletedTask;");
        content.AppendLine("                         }");
        content.AppendLine();
        content.AppendLine("                         if (context.Request.Cookies.ContainsKey(SystemConsts.Cookies.Auth))");
        content.AppendLine("                         {");
        content.AppendLine("                             context.Token = context.Request.Cookies[SystemConsts.Cookies.Auth];");
        content.AppendLine("                             return Task.CompletedTask;");
        content.AppendLine("                         }");
        content.AppendLine();
        content.AppendLine("                         return Task.CompletedTask;");
        content.AppendLine("                     },");
        content.AppendLine();
        content.AppendLine("                     OnAuthenticationFailed = context =>");
        content.AppendLine("                     {");
        content.AppendLine("                         context.NoResult();");
        content.AppendLine();
        content.AppendLine("                         if (!context.Response.HasStarted)");
        content.AppendLine("                         {");
        content.AppendLine("                             context.Response.StatusCode = StatusCodes.Status419AuthenticationTimeout;");
        content.AppendLine("                         }");
        content.AppendLine();
        content.AppendLine("                         return Task.CompletedTask;");
        content.AppendLine("                     },");
        content.AppendLine();
        content.AppendLine("                     OnChallenge = context =>");
        content.AppendLine("                     {");
        content.AppendLine("                         int statusCodeJar = context.Response.StatusCode;");
        content.AppendLine("                         context.HandleResponse();");
        content.AppendLine();
        content.AppendLine("                         int statusCode = statusCodeJar == StatusCodes.Status419AuthenticationTimeout ? StatusCodes.Status419AuthenticationTimeout : StatusCodes.Status401Unauthorized;");
        content.AppendLine("                         string[] message = statusCodeJar == StatusCodes.Status419AuthenticationTimeout ? OnAuthenticationFailed : onChallengeError;");
        content.AppendLine();
        content.AppendLine("                         context.Response.StatusCode = statusCode;");
        content.AppendLine("                         context.Response.ContentType = \"application/json\";");
        content.AppendLine();
        content.AppendLine("                         string result = JsonSerializer.Serialize(new");
        content.AppendLine("                         {");
        content.AppendLine("                             Code = statusCode,");
        content.AppendLine("                             Date = GetDateDetails(),");
        content.AppendLine("                             context.HttpContext.Request.Path,");
        content.AppendLine("                             Messages = message,");
        content.AppendLine("                             HasError = true");
        content.AppendLine("                         });");
        content.AppendLine();
        content.AppendLine("                         return context.Response.WriteAsync(result);");
        content.AppendLine("                     }");
        content.AppendLine("                 }; ");
        content.AppendLine("             });");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    private static void AddFactory(IServiceCollection services, WebApplicationBuilder builder)");
        content.AppendLine("    {");
        content.AppendLine("        services.AddSingleton<IDataBaseConnection, DataBaseConnection>();");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    private static void AddContext(IServiceCollection services, WebApplicationBuilder builder)");
        content.AppendLine("    {");
        content.AppendLine("        string con = new DataBaseConnection(builder.Configuration).GetConnectionString();");
        content.AppendLine();
        content.AppendLine($"        services.AddDbContextPool<{contextName}>((serviceProvider, options) =>");
        content.AppendLine("        {");
        content.AppendLine("            // Npgsql;");
        content.AppendLine("            options.UseNpgsql(con).AddInterceptors(slowQueryDebugInterceptor, changeLogInterceptor);");
        content.AppendLine("        });");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    private static void AddJobs(IServiceCollection services)");
        content.AppendLine("    {");
        content.AppendLine(" \t\t// TO DO;");
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