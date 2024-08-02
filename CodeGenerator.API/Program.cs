using CodeGenerator.Console.Models;
using Main = CodeGenerator.Console.Main;

#region Program
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    request.IsGenerateZip = true;
    (byte[] bytes, Guid guid) = Main.Execute(request);

    return Results.File(bytes, "application/zip", $"{request.SolutionName}{guid}.zip");
});

app.MapGet("/Teste", () =>
{
    return "aweonao";
});

app.Run();