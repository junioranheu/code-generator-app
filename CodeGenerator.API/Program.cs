using System.IO.Compression;
using CodeGenerator.Console.Enums;
using CodeGenerator.Console.Models;
using Microsoft.AspNetCore.ResponseCompression;
using Main = CodeGenerator.Console.Main;

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed((host) => true).AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddResponseCompression(x =>
{
    x.EnableForHttps = true;
    x.Providers.Add<BrotliCompressionProvider>();
    x.Providers.Add<GzipCompressionProvider>();
});

builder.Services.Configure<BrotliCompressionProviderOptions>(x =>
{
    x.Level = CompressionLevel.Optimal;
});

builder.Services.Configure<GzipCompressionProviderOptions>(x =>
{
    x.Level = CompressionLevel.Optimal;
});
#endregion

#region App
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
#endregion

app.MapPost("/GenerateCode", (GenerateCodeRequest request) =>
{
    try
    {
        request.IsGenerateZip = true;
        request.RequestType = RequestTypeEnum.API;
        (byte[] bytes, Guid guid) = Main.Execute(request);

        return Results.File(bytes, "application/zip", $"{request.SolutionName}{guid}.zip");
    }
    catch (Exception ex)
    {
        throw new Exception(ex.Message);
    }
});

app.MapGet("/Teste", () =>
{
    return DateTime.Now;
});

app.Run();