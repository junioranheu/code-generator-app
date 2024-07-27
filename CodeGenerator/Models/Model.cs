using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Console.Models;

public sealed class Model
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Props { get; set; } = string.Empty;
}