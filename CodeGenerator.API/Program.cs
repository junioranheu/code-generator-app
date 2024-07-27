using CodeGenerator.Console.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/GenerateCode", (GenerateCodeRequest request) =>
{
    var (solutionName, contextName, isPKGuid, models, isGenerateZip) = request;

    // Your logic here

    return Results.Ok("Code generated successfully.");
});

app.Run();