using System.ComponentModel;

namespace CodeGenerator.Console.Enums;

public enum ExtensionsEnum
{
    [Description(".cs")]
    CS,

    [Description(".css")]
    CSS,

    [Description(".scss")]
    SCSS,

    [Description(".js")]
    JS,

    [Description(".ts")]
    TS
}