using System.ComponentModel;

namespace CodeGenerator.Enums;
public enum ExtensionsEnum
{
    [Description(".cs")]
    cs,

    [Description(".css")]
    css,

    [Description(".scss")]
    scss,

    [Description(".js")]
    js,

    [Description(".ts")]
    ts
}