using CodeGenerator.Enums;

namespace CodeGenerator.Models;

public sealed class Content
{
    public string Value { get; set; } = string.Empty;

    public ContentPathEnum Path { get; set; }
}