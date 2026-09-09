using CodeGenerator.Console.Consts;
using CodeGenerator.Console.Models;
using System.Text;
using static CodeGenerator.Console.Utils.Fixtures.Generate;
using static CodeGenerator.Console.Utils.Fixtures.Get;

namespace CodeGenerator.Console.Repositories;

public static class SolutionRepository
{
    /// <summary>
    /// Gera a estrutura completa da solução e dos projetos (API, Application, Domain, Infrastructure)
    /// a partir do nome da solução, diretório raiz, nome do DbContext e modelos fornecidos.
    /// </summary>
    public static void Generate(string solutionName, string rootPath, string contextName, List<Model> models)
    {
        string apiProject = $"{solutionName}.API";
        string applicationProject = $"{solutionName}.Application";
        string domainProject = $"{solutionName}.Domain";
        string infrastructureProject = $"{solutionName}.Infrastructure";
        string apiName = $"{solutionName}.API";

        GenerateMiscFiles(solutionName, rootPath, apiName);

        GenerateFolder(solutionName, Path.Combine(rootPath, apiProject));
        GenerateFolder(solutionName, Path.Combine(rootPath, applicationProject));
        GenerateFolder(solutionName, Path.Combine(rootPath, domainProject));
        GenerateFolder(solutionName, Path.Combine(rootPath, infrastructureProject));

        Write(rootPath, $"{solutionName}.sln", GenerateSolutionFile(apiProject, applicationProject, domainProject, infrastructureProject));
        Write(rootPath, Path.Combine(apiProject, $"{apiProject}.csproj"), GenerateApiProject(applicationProject, infrastructureProject));
        Write(rootPath, Path.Combine(applicationProject, $"{applicationProject}.csproj"), GenerateApplicationProject(domainProject, infrastructureProject));
        Write(rootPath, Path.Combine(domainProject, $"{domainProject}.csproj"), GenerateLibraryProject());
        Write(rootPath, Path.Combine(infrastructureProject, $"{infrastructureProject}.csproj"), GenerateInfrastructureProject(domainProject));

        Write(rootPath, Path.Combine(apiProject, "Program.cs"), GenerateApiProgram(solutionName));
        Write(rootPath, Path.Combine(apiProject, "appsettings.json"), GenerateAppSettingsJson());
        Write(rootPath, Path.Combine(apiProject, "DependencyInjection.cs"), GenerateAPIDependencyInjection(solutionName));
        Write(rootPath, Path.Combine(apiProject, "DependencyAppConfiguration.cs"), GenerateAPIAppConfigurationDependencyInjection(solutionName));

        Write(rootPath, Path.Combine(applicationProject, "UseCases", "Shared", "PaginationInput.cs"), GeneratePaginationInput(solutionName));
        Write(rootPath, Path.Combine(applicationProject, "UseCases", "Shared", "PagedQuery.cs"), GeneratePagedQuery(solutionName));
        Write(rootPath, Path.Combine(applicationProject, "DependencyInjection.cs"), GenerateApplicationDependencyInjection(solutionName, models));

        Write(rootPath, Path.Combine(infrastructureProject, "Data", $"{contextName}.cs"), GenerateDbContext(solutionName, contextName, models));
        Write(rootPath, Path.Combine(infrastructureProject, "DependencyInjection.cs"), GenerateInfrastructureDependencyInjection(solutionName, contextName));
    }

    /// <summary>
    /// Gera o conteúdo do arquivo .csproj para o projeto API, incluindo referências a Application e Infrastructure.
    /// </summary>
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
        sb.AppendLine($"    <PackageReference Include=\"Mapster\" Version=\"{Misc.MapsterVersion}\" />");
        sb.AppendLine($"    <PackageReference Include=\"Swashbuckle.AspNetCore\" Version=\"{Misc.SwaggerVersion}\" />");
        sb.AppendLine("  </ItemGroup>");
        sb.AppendLine("</Project>");

        return sb.ToString();
    }

    /// <summary>
    /// Gera arquivos "misc" essenciais (constantes e settings) usados pelos projetos gerados,
    /// como Domain/Consts/SystemConsts.cs e Infrastructure/Auth/Models/JwtSettings.cs.
    /// </summary>
    private static void GenerateMiscFiles(string solutionName, string rootPath, string apiName)
    {
        #region SystemConsts em Domain/Consts;
        StringBuilder systemConsts = new();

        systemConsts.AppendLine($"namespace {solutionName}.Domain.Consts;");
        systemConsts.AppendLine();
        systemConsts.AppendLine("public static class SystemConsts");
        systemConsts.AppendLine("{");
        systemConsts.AppendLine("    public static class App");
        systemConsts.AppendLine("    {");
        systemConsts.AppendLine($"        public const string NameApi = \"{apiName}\";");
        systemConsts.AppendLine($"        public const string NameApp = \"{solutionName}\";");
        systemConsts.AppendLine("        public const string Email = \"xxx@gmail.com\";");
        systemConsts.AppendLine("        public const string Author = \"@junioranheu\";");
        systemConsts.AppendLine("        public const string Slogan = \"xxx\";");
        systemConsts.AppendLine("        public const string MainColor = \"#f9fff6\";");
        systemConsts.AppendLine("    }");
        systemConsts.AppendLine();
        systemConsts.AppendLine("    public static class Time");
        systemConsts.AppendLine("    {");
        systemConsts.AppendLine("        public const int OneSecond = 1;");
        systemConsts.AppendLine("        public const int OneMinute = 60;");
        systemConsts.AppendLine("        public const int TenMinutes = 600;");
        systemConsts.AppendLine("        public const int OneHour = 3600;");
        systemConsts.AppendLine("        public const int HalfDay = 43200;");
        systemConsts.AppendLine("        public const int OneDay = 86400;");
        systemConsts.AppendLine("        public const int OneWeek = 604800;");
        systemConsts.AppendLine("        public const int OneMonth = 2629800;");
        systemConsts.AppendLine("        public const int OneYear = 31536000;");
        systemConsts.AppendLine("    }");
        systemConsts.AppendLine();
        systemConsts.AppendLine("    public static class Cookies");
        systemConsts.AppendLine("    {");
        systemConsts.AppendLine("        public const string Auth = \"COOKIE_AUTH_BACK\";");
        systemConsts.AppendLine("        public const string Refresh = \"auth_refreshedToken\";");
        systemConsts.AppendLine("    }");
        systemConsts.AppendLine();
        systemConsts.AppendLine("    public static class Cache");
        systemConsts.AppendLine("    {");
        systemConsts.AppendLine("        public const string CacheKey_FiltersExample = \"CacheKey_FiltersExemple_exampleId_\";");
        systemConsts.AppendLine("    }");
        systemConsts.AppendLine();
        systemConsts.AppendLine("    public static class Warnings");
        systemConsts.AppendLine("    {");
        systemConsts.AppendLine("        public const string NotAuthSimpleUser = \"Usuário não autenticado.\";");
        systemConsts.AppendLine("        public const string NeedToVerifyUser = \"A sua conta ainda não foi verificada ou está desativada. Verifique-a e tente novamente.\";");
        systemConsts.AppendLine("        public const string VerifyTokenInvalid = \"Código de verificação inválido ou inexistente.\";");
        systemConsts.AppendLine("        public const string NotFoundData = \"A informação não foi encontrada na base de dados.\";");
        systemConsts.AppendLine("        public const string AlreadyAuth = \"Você já está autenticado no sistema, portanto não pode prosseguir com esta requisição.\";");
        systemConsts.AppendLine("    }");
        systemConsts.AppendLine("}");

        Write(rootPath, Path.Combine($"{solutionName}.Domain", "Consts", "SystemConsts.cs"), systemConsts.ToString());
        #endregion

        #region JwtSettings em Infrastructure/Auth/Models;
        StringBuilder jwt = new();

        jwt.AppendLine($"namespace {solutionName}.Infrastructure.Auth.Models;");
        jwt.AppendLine();
        jwt.AppendLine("public sealed class JwtSettings");
        jwt.AppendLine("{");
        jwt.AppendLine("    public int TokenExpiryMinutes { get; init; }");
        jwt.AppendLine("    public int RefreshTokenExpiryMinutes { get; init; }");
        jwt.AppendLine("    public string? Issuer { get; init; } = null;");
        jwt.AppendLine("    public string? Audience { get; init; } = null;");
        jwt.AppendLine("}");

        Write(rootPath, Path.Combine($"{solutionName}.Infrastructure", "Auth", "Models", "JwtSettings.cs"), jwt.ToString());
        #endregion

        #region Interface IDataBaseConnection em Infrastructure/Factory/Database
        StringBuilder dbConnInterface = new();

        dbConnInterface.AppendLine("using Npgsql;");
        dbConnInterface.AppendLine();
        dbConnInterface.AppendLine($"namespace {solutionName}.Infrastructure.Factory.DataBase;");
        dbConnInterface.AppendLine();
        dbConnInterface.AppendLine("public interface IDataBaseConnection");
        dbConnInterface.AppendLine("{");
        dbConnInterface.AppendLine("    string GetConnectionString();");
        dbConnInterface.AppendLine("    NpgsqlConnection GetConnection();");
        dbConnInterface.AppendLine("    string GetConnectionTypeName();");
        dbConnInterface.AppendLine("}");

        Write(rootPath, Path.Combine($"{solutionName}.Infrastructure", "Factory", "DataBase", "IDataBaseConnection.cs"), dbConnInterface.ToString());
        #endregion

        #region DataBaseConnection em Infrastructure/Factory/DataBase
        StringBuilder dbConn = new();

        dbConn.AppendLine("using Microsoft.Extensions.Configuration;");
        dbConn.AppendLine("using Npgsql;");
        dbConn.AppendLine();
        dbConn.AppendLine($"namespace {solutionName}.Infrastructure.Factory.DataBase;");
        dbConn.AppendLine();
        dbConn.AppendLine($"public class DataBaseConnection(IConfiguration configuration) : IDataBaseConnection");
        dbConn.AppendLine("{");
        dbConn.AppendLine("    private readonly IConfiguration _configuration = configuration;");
        dbConn.AppendLine();
        dbConn.AppendLine("    public string GetConnectionString()");
        dbConn.AppendLine("    {");
        dbConn.AppendLine("        string connectionStringName = _configuration[\"SystemSettings:ConnectionStringName\"] ?? string.Empty;");
        dbConn.AppendLine("        string connectionString = _configuration.GetConnectionString(connectionStringName) ?? string.Empty;");
        dbConn.AppendLine();
        dbConn.AppendLine("        if (string.IsNullOrEmpty(connectionString))");
        dbConn.AppendLine("        {");
        dbConn.AppendLine("            throw new InvalidOperationException(\"A connection string está nula.\");");
        dbConn.AppendLine("        }");
        dbConn.AppendLine();
        dbConn.AppendLine("        return connectionString;");
        dbConn.AppendLine("    }");
        dbConn.AppendLine();
        dbConn.AppendLine("    public NpgsqlConnection GetConnection()");
        dbConn.AppendLine("    {");
        dbConn.AppendLine("        return new NpgsqlConnection(GetConnectionString());");
        dbConn.AppendLine("    }");
        dbConn.AppendLine();
        dbConn.AppendLine("    public string GetConnectionTypeName()");
        dbConn.AppendLine("    {");
        dbConn.AppendLine("        return nameof(NpgsqlConnection);");
        dbConn.AppendLine("    }");
        dbConn.AppendLine("}");

        Write(rootPath, Path.Combine($"{solutionName}.Infrastructure", "Factory", "DataBase", "DataBaseConnection.cs"), dbConn.ToString());
        #endregion

        #region Interface IJwtTokenGenerator em Infrastructure/Auth/Token
        StringBuilder jwtGenInterface = new();

        jwtGenInterface.AppendLine("using Microsoft.AspNetCore.Http;");
        jwtGenInterface.AppendLine($"using {solutionName}.Domain.Entities;");
        jwtGenInterface.AppendLine($"using {solutionName}.Domain.Enums;");
        jwtGenInterface.AppendLine("using System.IdentityModel.Tokens.Jwt;");
        jwtGenInterface.AppendLine();
        jwtGenInterface.AppendLine($"namespace {solutionName}.Infrastructure.Auth.Token;");
        jwtGenInterface.AppendLine();
        jwtGenInterface.AppendLine("public interface IJwtTokenGenerator");
        jwtGenInterface.AppendLine("{");
        jwtGenInterface.AppendLine("    (string token, RefreshToken refreshToken, CookieOptions cookieOptions) GenerateToken(Guid userIdAuth, string name, string email, UserRoleEnum? role);");
        jwtGenInterface.AppendLine("    (bool isTokenExpiringSoonOrHasAlreadyExpired, double differenceInSeconds, DateTime validTo) IsTokenExpiringSoonOrHasAlreadyExpired(JwtSecurityToken token, int thresholdInMinutes = 0);");
        jwtGenInterface.AppendLine("    CookieOptions GetCookieOptions();");
        jwtGenInterface.AppendLine("}");

        Write(rootPath, Path.Combine($"{solutionName}.Infrastructure", "Auth", "Token", "IJwtTokenGenerator.cs"), jwtGenInterface.ToString());
        #endregion

        #region JwtTokenGenerator em Infrastructure/Auth/Token
        StringBuilder jwtGen = new();

        jwtGen.AppendLine("using Microsoft.AspNetCore.Http;");
        jwtGen.AppendLine("using Microsoft.Extensions.Configuration;");
        jwtGen.AppendLine("using Microsoft.Extensions.Options;");
        jwtGen.AppendLine("using Microsoft.IdentityModel.Tokens;");
        jwtGen.AppendLine($"using {solutionName}.Domain.Entities;");
        jwtGen.AppendLine($"using {solutionName}.Domain.Enums;");
        jwtGen.AppendLine($"using {solutionName}.Infrastructure.Auth.Models;");
        jwtGen.AppendLine("using System.IdentityModel.Tokens.Jwt;");
        jwtGen.AppendLine("using System.Security.Claims;");
        jwtGen.AppendLine("using System.Text;");
        jwtGen.AppendLine($"using static {solutionName}.Infrastructure.Utils.Get;");
        jwtGen.AppendLine();
        jwtGen.AppendLine($"namespace {solutionName}.Infrastructure.Auth.Token;");
        jwtGen.AppendLine();
        jwtGen.AppendLine($"public sealed class JwtTokenGenerator(IOptions<JwtSettings> jwtOptions, IConfiguration config) : IJwtTokenGenerator");
        jwtGen.AppendLine("{");
        jwtGen.AppendLine("    private readonly JwtSettings _jwtSettings = jwtOptions.Value;");
        jwtGen.AppendLine("    private readonly string _secret = config[\"JwtSettings:Secret\"] ?? throw new InvalidOperationException(\"JWT Secret não foi configurada no servidor!\");");
        jwtGen.AppendLine();
        jwtGen.AppendLine("    public (string token, RefreshToken refreshToken, CookieOptions cookieOptions) GenerateToken(Guid userIdAuth, string name, string email, UserRoleEnum? role)");
        jwtGen.AppendLine("    {");
        jwtGen.AppendLine("        JwtSecurityTokenHandler tokenHandler = new();");
        jwtGen.AppendLine();
        jwtGen.AppendLine("        SigningCredentials signingCredentials = new(");
        jwtGen.AppendLine("            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret ?? string.Empty)),");
        jwtGen.AppendLine("            algorithm: SecurityAlgorithms.HmacSha256Signature");
        jwtGen.AppendLine("        );");
        jwtGen.AppendLine();
        jwtGen.AppendLine("        List<Claim> claimList =");
        jwtGen.AppendLine("        [");
        jwtGen.AppendLine("            new Claim(ClaimTypes.NameIdentifier, userIdAuth.ToString()),");
        jwtGen.AppendLine("            new Claim(ClaimTypes.Name, name),");
        jwtGen.AppendLine("            new Claim(ClaimTypes.Email, email)");
        jwtGen.AppendLine("        ]; ");
        jwtGen.AppendLine();
        jwtGen.AppendLine("        if (role is not null && role.HasValue)");
        jwtGen.AppendLine("        {");
        jwtGen.AppendLine("            Claim roleClaim = new(ClaimTypes.Role, role.Value.ToString());");
        jwtGen.AppendLine("            claimList.Add(roleClaim);");
        jwtGen.AppendLine();
        jwtGen.AppendLine("            bool alreadyHasCommon = role == UserRoleEnum.Common;");
        jwtGen.AppendLine();
        jwtGen.AppendLine("            if (!alreadyHasCommon)");
        jwtGen.AppendLine("            {");
        jwtGen.AppendLine("                Claim roleComum = new(ClaimTypes.Role, UserRoleEnum.Common.ToString());");
        jwtGen.AppendLine("                claimList.Add(roleComum);");
        jwtGen.AppendLine("            }");
        jwtGen.AppendLine("        }");
        jwtGen.AppendLine();
        jwtGen.AppendLine("        ClaimsIdentity claims = new(claimList);");
        jwtGen.AppendLine();
        jwtGen.AppendLine("        DateTime date = GetDate();");
        jwtGen.AppendLine();
        jwtGen.AppendLine("        SecurityTokenDescriptor tokenDescriptor = new()");
        jwtGen.AppendLine("        {");
        jwtGen.AppendLine("            Issuer = _jwtSettings.Issuer,");
        jwtGen.AppendLine("            IssuedAt = date,");
        jwtGen.AppendLine("            Audience = _jwtSettings.Audience,");
        jwtGen.AppendLine("            NotBefore = date,");
        jwtGen.AppendLine("            Expires = date.AddMinutes(_jwtSettings.TokenExpiryMinutes),");
        jwtGen.AppendLine("            Subject = claims,");
        jwtGen.AppendLine("            SigningCredentials = signingCredentials");
        jwtGen.AppendLine("        }; ");
        jwtGen.AppendLine();
        jwtGen.AppendLine("        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);");
        jwtGen.AppendLine("        string jwt = tokenHandler.WriteToken(token);");
        jwtGen.AppendLine("        RefreshToken refreshToken = GenerateRefreshToken(userIdAuth);");
        jwtGen.AppendLine();
        jwtGen.AppendLine("        CookieOptions cookieOptions = GetCookieOptions();");
        jwtGen.AppendLine();
        jwtGen.AppendLine("        return (jwt, refreshToken, cookieOptions);");
        jwtGen.AppendLine("    }");
        jwtGen.AppendLine();
        jwtGen.AppendLine("    #region extras");
        jwtGen.AppendLine("    private RefreshToken GenerateRefreshToken(Guid userIdAuth)");
        jwtGen.AppendLine("    {");
        jwtGen.AppendLine("        string token = GenerateSafeToken32Bytes(urlSafe: false);");
        jwtGen.AppendLine();
        jwtGen.AppendLine("        RefreshToken refreshToken = new()");
        jwtGen.AppendLine("        {");
        jwtGen.AppendLine("            Token = token,");
        jwtGen.AppendLine("            UserId = userIdAuth,");
        jwtGen.AppendLine("            CreatedDate = GetDate(),");
        jwtGen.AppendLine("            ExpiredDate = GetDate().AddMinutes(_jwtSettings.RefreshTokenExpiryMinutes),");
        jwtGen.AppendLine("            RevokedDate = null");
        jwtGen.AppendLine("        }; ");
        jwtGen.AppendLine();
        jwtGen.AppendLine("        return refreshToken;");
        jwtGen.AppendLine("    }");
        jwtGen.AppendLine();
        jwtGen.AppendLine("    public CookieOptions GetCookieOptions()");
        jwtGen.AppendLine("    {");
        jwtGen.AppendLine("        return new CookieOptions");
        jwtGen.AppendLine("        {");
        jwtGen.AppendLine("            HttpOnly = true,");
        jwtGen.AppendLine("            Secure = true,");
        jwtGen.AppendLine("            SameSite = SameSiteMode.None, ");
        jwtGen.AppendLine("            Expires = GetDate().AddMinutes(_jwtSettings.RefreshTokenExpiryMinutes),");
        jwtGen.AppendLine("            Path = \"/\"");
        jwtGen.AppendLine("        }; ");
        jwtGen.AppendLine("    }");
        jwtGen.AppendLine();
        jwtGen.AppendLine("    public (bool isTokenExpiringSoonOrHasAlreadyExpired, double differenceInSeconds, DateTime validTo) IsTokenExpiringSoonOrHasAlreadyExpired(JwtSecurityToken token, int thresholdInMinutes = 0)");
        jwtGen.AppendLine("    {");
        jwtGen.AppendLine("        DateTime date = GetDate();");
        jwtGen.AppendLine("        DateTime dateThreshold = date.AddMinutes(thresholdInMinutes);");
        jwtGen.AppendLine();
        jwtGen.AppendLine("        double differenceInSeconds = (token.ValidTo - dateThreshold).TotalSeconds;");
        jwtGen.AppendLine("        bool isTokenExpiringSoonOrHasAlreadyExpired = differenceInSeconds <= 0;");
        jwtGen.AppendLine();
        jwtGen.AppendLine("        return (isTokenExpiringSoonOrHasAlreadyExpired, differenceInSeconds, token.ValidTo);");
        jwtGen.AppendLine("    }");
        jwtGen.AppendLine("    #endregion");
        jwtGen.AppendLine("}");

        Write(rootPath, Path.Combine($"{solutionName}.Infrastructure", "Auth", "Token", "JwtTokenGenerator.cs"), jwtGen.ToString());
        #endregion

        #region Helpers em Infrastructure/Utils/Get.cs
        StringBuilder utilsGet = new();

        utilsGet.AppendLine("using System.Security.Cryptography;");
        utilsGet.AppendLine();
        utilsGet.AppendLine($"namespace {solutionName}.Infrastructure.Utils;");
        utilsGet.AppendLine();
        utilsGet.AppendLine("public static class Get");
        utilsGet.AppendLine("{");
        utilsGet.AppendLine("    public static DateTime GetDate() => DateTime.UtcNow;");
        utilsGet.AppendLine();
        utilsGet.AppendLine("    public static string GenerateSafeToken32Bytes(bool urlSafe)");
        utilsGet.AppendLine("    {");
        utilsGet.AppendLine("        byte[] random = new byte[32];");
        utilsGet.AppendLine("        using var rng = RandomNumberGenerator.Create();");
        utilsGet.AppendLine("        rng.GetBytes(random);");
        utilsGet.AppendLine();
        utilsGet.AppendLine("        string token = Convert.ToBase64String(random);");
        utilsGet.AppendLine();
        utilsGet.AppendLine("        if (urlSafe)");
        utilsGet.AppendLine("        {");
        utilsGet.AppendLine("            token = token.Replace('+', '-').Replace('/', '_').TrimEnd('=');");
        utilsGet.AppendLine("        }");
        utilsGet.AppendLine();
        utilsGet.AppendLine("        return token;");
        utilsGet.AppendLine("    }");
        utilsGet.AppendLine("}");

        Write(rootPath, Path.Combine($"{solutionName}.Infrastructure", "Utils", "Get.cs"), utilsGet.ToString());
        #endregion
    }

    /// <summary>
    /// Gera o conteúdo do arquivo .csproj para o projeto Application com referências necessárias.
    /// </summary>
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

    /// <summary>
    /// Gera um .csproj simples para projetos de biblioteca (Domain).
    /// </summary>
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

    /// <summary>
    /// Gera o .csproj do projeto Infrastructure com referências a Entity Framework, autenticação e framework ASP.NET.
    /// </summary>
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
        sb.AppendLine($"    <PackageReference Include=\"Npgsql.EntityFrameworkCore.PostgreSQL\" Version=\"{Misc.EntityFrameworkVersion}\" />");
        sb.AppendLine($"    <PackageReference Include=\"Microsoft.EntityFrameworkCore.Design\" Version=\"{Misc.EntityFrameworkVersion}\">\n      <PrivateAssets>all</PrivateAssets></PackageReference>");
        sb.AppendLine($"    <PackageReference Include=\"System.IdentityModel.Tokens.Jwt\" Version=\"{Misc.IdentityModelVersion}\" />");
        sb.AppendLine($"    <PackageReference Include=\"Microsoft.AspNetCore.Authentication.JwtBearer\" Version=\"{Misc.JwtBearerVersion}\" />");
        sb.AppendLine($"    <FrameworkReference Include=\"Microsoft.AspNetCore.App\" />");
        sb.AppendLine("  </ItemGroup>");
        sb.AppendLine("</Project>");

        return sb.ToString();
    }

    /// <summary>
    /// Gera o conteúdo do arquivo de solução (.sln) incluindo configurações e guids para os projetos fornecidos.
    /// </summary>
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

    /// <summary>
    /// Gera o arquivo Program.cs para a API com a configuração inicial do WebApplication,
    /// registrando os módulos de Dependency Injection e invocando a configuração do app.
    /// </summary>
    private static string GenerateApiProgram(string solutionName)
    {
        StringBuilder content = new();

        content.AppendLine($"using {solutionName}.API;");
        content.AppendLine($"using {solutionName}.Domain.Consts;");
        content.AppendLine($"using {solutionName}.Application;");
        content.AppendLine($"using {solutionName}.Infrastructure;");
        content.AppendLine();
        content.AppendLine($"Console.Title = SystemConsts.App.NameApi;");
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

    /// <summary>
    /// Gera um appsettings.json mínimo com connection string e exemplos de URLs/ CORS.
    /// </summary>
    private static string GenerateAppSettingsJson()
    {
        StringBuilder sb = new();

        sb.AppendLine("{");
        sb.AppendLine("  \"SystemSettings\": {");
        sb.AppendLine("    \"ConnectionStringName\": \"DefaultConnection\"");
        sb.AppendLine("  },");
        sb.AppendLine("  \"ConnectionStrings\": {");
        sb.AppendLine("    \"DefaultConnection\": \"Host=192.xxx.x.xxx;Port=5432;Database=xxx;Username=xxx;Password=xxx;\"");
        sb.AppendLine("  },");
        sb.AppendLine("  \"JwtSettings\": {");
        sb.AppendLine("     \"TokenExpiryMinutes\": \"30\",");
        sb.AppendLine("     \"RefreshTokenExpiryMinutes\": \"20160\",");
        sb.AppendLine("     \"Issuer\": \"xxx\",");
        sb.AppendLine("     \"Audience\": \"xxx\"");
        sb.AppendLine("  },");
        sb.AppendLine("  \"Logging\": {");
        sb.AppendLine("    \"LogLevel\": {");
        sb.AppendLine("      \"Default\": \"Information\",");
        sb.AppendLine("      \"Microsoft.AspNetCore\": \"Warning\"");
        sb.AppendLine("    }");
        sb.AppendLine("  },");
        sb.AppendLine("  \"CORSSettings\": {");
        sb.AppendLine("     \"Cors\": \"xxx\"");
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

    /// <summary>
    /// Gera a classe PaginationInput usada por use-cases para representar parâmetros de paginação.
    /// </summary>
    private static string GeneratePaginationInput(string solutionName) => string.Join(Environment.NewLine, [
        $"namespace {solutionName}.Application.UseCases.Shared;",
        "",
        "public sealed class PaginationInput",
        "{",
        "    public int Page { get; set; } = 1;",
        "    public int PageSize { get; set; } = 20;",
        "}"
    ]);

    /// <summary>
    /// Gera a classe utilitária PagedQuery que executa consultas EF Core com paginação.
    /// </summary>
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
        "",
        "        return (linq, count);",
        "    }",
        "}"
    ]);

    /// <summary>
    /// Gera a implementação do DbContext com DbSet para cada entidade informada,
    /// além de regras de OnModelCreating e lógica de auditoria/SaveChanges.
    /// </summary>
    private static string GenerateDbContext(string solutionName, string contextName, List<Model> models)
    {
        StringBuilder content = new();

        content.AppendLine("using System.Security.Claims;");
        content.AppendLine("using Microsoft.AspNetCore.Http;");
        content.AppendLine("using Microsoft.EntityFrameworkCore;");
        content.AppendLine("using Microsoft.EntityFrameworkCore.Storage.ValueConversion;");
        content.AppendLine($"using {solutionName}.Domain.Entities;");
        content.AppendLine($"using static {solutionName}.Infrastructure.Utils.Get;");
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

        // Props sistêmaticas que devem existir no DbContext;
        content.AppendLine($"    public DbSet<RefreshToken> {GetStrPlural("RefreshToken")} {{ get; set; }}");

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
        content.AppendLine("                            audit.CreatedDate = GetDate();");
        content.AppendLine("                            audit.CreatedBy = UserIdAuth;");
        content.AppendLine("                            audit.Status = true;");
        content.AppendLine("                        }");
        content.AppendLine();
        content.AppendLine("                        break;");
        content.AppendLine();
        content.AppendLine("                    case EntityState.Modified:");
        content.AppendLine("                        audit.LastModificationDate = GetDate();");
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

    /// <summary>
    /// Gera o arquivo DependencyInjection.cs do projeto API contendo registros de serviços
    /// relacionados à API (controllers, swagger, cors, compressão etc.).
    /// </summary>
    private static string GenerateAPIDependencyInjection(string solutionName)
    {
        StringBuilder content = new();

        content.AppendLine($"using {solutionName}.Domain.Consts;");
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
        content.AppendLine($"            c.SwaggerDoc(\"v1\", new() {{ Title = SystemConsts.App.NameApi, Version = \"v1\" }});");
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

    /// <summary>
    /// Gera a classe responsável por configurar o pipeline da API (UseAppConfiguration),
    /// incluindo middlewares, Swagger, CORS, compressão e inicialização de banco.
    /// </summary>
    private static string GenerateAPIAppConfigurationDependencyInjection(string solutionName)
    {
        StringBuilder content = new();

        content.AppendLine($"using {solutionName}.Domain.Consts;");
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
        content.AppendLine($"                c.SwaggerEndpoint(\"/swagger/v1/swagger.json\", SystemConsts.App.NameApi);");
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

    /// <summary>
    /// Gera o arquivo DependencyInjection.cs do projeto Application para registrar use-cases
    /// e serviços da camada de aplicação (atualmente inicia vazio para customização posterior).
    /// </summary>
    private static string GenerateApplicationDependencyInjection(string solutionName, List<Model> models)
    {
        StringBuilder content = new();

        content.AppendLine("using Microsoft.AspNetCore.Builder;");
        content.AppendLine("using Microsoft.Extensions.DependencyInjection;");
        content.AppendLine("using Microsoft.Extensions.Logging;");

        // Usings dinâmicos para cada use-case;
        foreach (Model model in models)
        {
            content.AppendLine($"using {solutionName}.Application.UseCases.{GetStrPlural(model.Name)};");
        }

        content.AppendLine();
        content.AppendLine($"namespace {solutionName}.Application;");
        content.AppendLine();
        content.AppendLine("public static class DependencyInjection");
        content.AppendLine("{");
        content.AppendLine("    public static IServiceCollection AddDependencyInjectionApplication(this IServiceCollection services, WebApplicationBuilder builder)");
        content.AppendLine("    {");
        content.AppendLine("        AddLogger(builder);");
        content.AppendLine("        AddUseCases(services);");
        content.AppendLine();
        content.AppendLine("        return services;");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    private static void AddLogger(WebApplicationBuilder builder)");
        content.AppendLine("    {");
        content.AppendLine("        builder.Logging.ClearProviders();");
        content.AppendLine("        builder.Logging.AddConsole();");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    private static void AddUseCases(IServiceCollection services)");
        content.AppendLine("    {");

        // Registros dinâmicos para cada model;
        foreach (Model model in models)
        {
            content.AppendLine($"        services.Add{GetStrPlural(model.Name)}Application();");
        }

        content.AppendLine("    }");
        content.AppendLine("}");

        return content.ToString();
    }

    /// <summary>
    /// Gera o arquivo DependencyInjection.cs do projeto Infrastructure com registros de serviços
    /// como autenticação, factories, contexto de dados e demais infraestruturas necessárias.
    /// </summary>
    private static string GenerateInfrastructureDependencyInjection(string solutionName, string contextName)
    {
        StringBuilder content = new();

        content.AppendLine($"using {solutionName}.Domain.Consts;");
        content.AppendLine($"using {solutionName}.Infrastructure.Auth.Models;");
        content.AppendLine($"using {solutionName}.Infrastructure.Auth.Token;");
        content.AppendLine($"using {solutionName}.Infrastructure.Data;");
        content.AppendLine($"using {solutionName}.Infrastructure.Factory.DataBase;");
        content.AppendLine("using Microsoft.AspNetCore.Authentication.JwtBearer;");
        content.AppendLine("using Microsoft.AspNetCore.Builder;");
        content.AppendLine("using Microsoft.AspNetCore.Hosting;");
        content.AppendLine("using Microsoft.AspNetCore.Http;");
        content.AppendLine("using Microsoft.EntityFrameworkCore;");
        content.AppendLine("using Microsoft.Extensions.DependencyInjection;");
        content.AppendLine("using Microsoft.Extensions.Hosting;");
        content.AppendLine("using Microsoft.IdentityModel.Tokens;");
        content.AppendLine("using System.Text;");
        content.AppendLine("using System.Text.Json;");
        content.AppendLine($"using static {solutionName}.Infrastructure.Utils.Get;");
        content.AppendLine();
        content.AppendLine($"namespace {solutionName}.Infrastructure;");
        content.AppendLine();
        content.AppendLine("public static class DependencyInjection");
        content.AppendLine("{");
        content.AppendLine("    public static IServiceCollection AddDependencyInjectionInfrastructure(this IServiceCollection services, WebApplicationBuilder builder)");
        content.AppendLine("    {");
        content.AppendLine("        AddServices(services, builder);");
        content.AppendLine("        AddAuth(services, builder);");
        content.AppendLine("        AddFactory(services);");
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
        content.AppendLine("                             Date = $\"{GetDate():dd/MM/yyyy} às {GetDate():HH:mm:ss}\",");
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
        content.AppendLine("    private static void AddFactory(IServiceCollection services)");
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
        content.AppendLine("            options.UseNpgsql(con);");
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

    /// <summary>
    /// Escreve o conteúdo no arquivo alvo, criando diretórios intermediários se necessário.
    /// </summary>
    private static void Write(string rootPath, string relativePath, string content)
    {
        string path = Path.Combine(rootPath, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content.TrimEnd());
    }
}