using System.ComponentModel;

namespace CodeGenerator.Console.Enums;

public enum ContentDirectoryEnum
{
    [Description("API/Controllers")]
    Controller,

    [Description("Application/UseCases")]
    UseCase,

    [Description("Domain/Entities")]
    Entity,

    [Description("Domain/Enums")]
    Enum,
}