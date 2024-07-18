using System.ComponentModel;

namespace CodeGenerator.Enums;

public enum ContentPathEnum
{
    [Description("API/Controllers")]
    Controller,

    [Description("Application/UseCases")]
    UseCases,

    [Description("Domain/Entities")]
    Model,

    [Description("Domain/Enums")]
    Enum,
}