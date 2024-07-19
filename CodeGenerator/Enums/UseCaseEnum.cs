using System.ComponentModel;

namespace CodeGenerator.Enums;

public enum UseCaseEnum
{
    [Description("Get")]
    Get,

    [Description("GetAll")]
    GetAll,

    [Description("Create")]
    Create,

    [Description("Update")]
    Update,

    [Description("Delete")]
    Delete
}