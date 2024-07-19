using CodeGenerator.Enums;
using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Models;

public sealed class Content
{
    [Required]
    public string Value { get; set; } = string.Empty;

    [Required]
    public ContentPathEnum Path { get; set; }

    [Required]
    public ExtensionsEnum Extension { get; set; }
}