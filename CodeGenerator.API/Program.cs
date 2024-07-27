using CodeGenerator.Console.Models;
using Main = CodeGenerator.Console.Main;

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
    request.IsGenerateZip = true;
    byte[] bytes = Main.Execute(request);

    return Results.Ok("Code generated successfully");
});

app.Run();