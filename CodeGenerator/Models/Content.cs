using CodeGenerator.Enums;
using System.ComponentModel.DataAnnotations;
using static CodeGenerator.Utils.Fixtures.Validate;

namespace CodeGenerator.Models;

public sealed class Content
{
    public Content(string value, ContentDirectoryEnum contentDirectory, ExtensionsEnum extension, string solutionName, string fileFinalPath)
    {
        ValidateEntityParams(
            GetType().Name,
            [value, contentDirectory, extension, solutionName, fileFinalPath],
            nameof(value), nameof(contentDirectory), nameof(extension), nameof(solutionName), nameof(fileFinalPath)
        );

        Value = value;
        ContentDirectory = contentDirectory;
        Extension = extension;
        SolutionName = solutionName;
        FileFinalPath = fileFinalPath;
    }

    [Required]
    public string Value { get; set; } = string.Empty;

    [Required]
    public ContentDirectoryEnum ContentDirectory { get; set; }

    [Required]
    public ExtensionsEnum Extension { get; set; }

    [Required]
    public string SolutionName { get; set; } = string.Empty;

    [Required]
    public string FileFinalPath { get; set; } = string.Empty;
}