using CodeGenerator.Console.Enums;
using CodeGenerator.Console.Models;
using Spectre.Console;
using System.Globalization;
using System.Text;
using static CodeGenerator.Console.Utils.Fixtures.Format;
using static CodeGenerator.Console.Utils.Fixtures.Get;

namespace CodeGenerator.Console.Utils.Fixtures;

public static class Prompt
{
    public static string PromptInput(string msg)
    {
        string? input = AnsiConsole.Ask<string>(Markup.Escape(msg));

        if (string.IsNullOrEmpty(input))
        {
            throw new ArgumentException("User input can not be empty");
        }

        string output = NormalizeInput(input);

        return output;
    }

    public static bool PromptInputForBool(string msg, bool defaultValue)
    {
        return AnsiConsole.Confirm(Markup.Escape(msg), defaultValue);
    }

    public static List<Model> PromptInputForModel()
    {
        List<Model> models = [];
        bool keepWhile = true;

        AnsiConsole.MarkupLine("\nClass name example: [cyan]Person[/]");
        AnsiConsole.MarkupLine("Class properties example: [cyan]Name string LastName string? Age int Height double IsUnder18 bool Country Country[/]\n");

        while (keepWhile)
        {
            string className = PromptInput("Class name:");
            string props = PromptInput("Props:");
            models.Add(new() { Name = className, Props = props });

            keepWhile = PromptInputForBool("\nDo you want to add one more item? (Answer y or n)", false);
        }

        return models;
    }

    public static string PromptLog(string msg, LogEnum? type = LogEnum.Success)
    {
        string style = type switch
        {
            LogEnum.Success => "cyan",
            LogEnum.Fail => "red",
            LogEnum.Warning => "yellow",
            _ => "grey",
        };

        string final = $"{FormatDateTime(GetDateTime(), DateTimeFormat.CompleteDateTime)} | {msg}";
        AnsiConsole.MarkupLine($"[{style}]{Markup.Escape(final)}[/]");

        return final;
    }

    private static string NormalizeInput(string input)
    {
        string normalized = input.Normalize(NormalizationForm.FormD);

        StringBuilder builder = new();

        foreach (char c in normalized)
        {
            UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(c);

            if (category != UnicodeCategory.NonSpacingMark && (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)))
            {
                builder.Append(c);
            }
        }

        return builder.ToString().Normalize(NormalizationForm.FormC).Trim();
    }
}