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
    public static string PromptInput(string msg, bool allowSpaces, bool allowQuestionMark)
    {
        string? input = AnsiConsole.Ask<string>(Markup.Escape(msg));

        if (string.IsNullOrEmpty(input))
        {
            throw new ArgumentException("User input can not be empty");
        }

        string output = NormalizeInput(input, allowSpaces, allowQuestionMark);

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

        AnsiConsole.MarkupLine("\nClass name example: [cyan]User[/]");
        AnsiConsole.MarkupLine("Class properties example: [cyan]Name string Password string Height double? IsUnder18 bool Country Country[/]\n");

        while (keepWhile)
        {
            string className = PromptInput("Class name:", allowSpaces: false, allowQuestionMark: false);

            // Ler as propriedades até que correspondam ao padrão esperado (pares: nome tipo);
            string propsInput;

            while (true)
            {
                propsInput = PromptInput("Props:", allowSpaces: true, allowQuestionMark: true);

                // Normalizar os separadores e dividir a string em partes;
                string normalized = propsInput.Replace(',', ' ').Replace(';', ' ').Replace('\n', ' ').Replace('\r', ' ').Replace('\t', ' ');
                string[] parts = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 0)
                {
                    AnsiConsole.MarkupLine("[red]You must provide at least one property pair.[/]");
                    continue;
                }

                if (parts.Length % 2 != 0)
                {
                    AnsiConsole.MarkupLine("[yellow]Properties must be provided as pairs (Name type). Please try again.[/]");
                    continue;
                }

                // Construir as propriedades formatadas: capitalizar cada nome de propriedade (cada índice par) e manter os tipos como estão;
                StringBuilder sb = new();

                for (int i = 0; i < parts.Length; i += 2)
                {
                    string propName = GetStrCapitalizedFirstLetter(parts[i]);
                    string propType = parts[i + 1];

                    sb.Append(propName).Append(' ').Append(propType).Append(' ');
                }

                propsInput = sb.ToString().Trim();
                break;
            }

            models.Add(new() { Name = GetStrCapitalizedFirstLetter(className), Props = propsInput });

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

    private static string NormalizeInput(string input, bool allowSpaces, bool allowQuestionMark)
    {
        string normalized = input.Normalize(NormalizationForm.FormD);

        StringBuilder builder = new();

        foreach (char c in normalized)
        {
            UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(c);

            if (category == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            if (char.IsLetterOrDigit(c) || (allowSpaces && char.IsWhiteSpace(c)) || (allowQuestionMark && c == '?'))
            {
                builder.Append(c);
            }
        }

        return builder.ToString().Normalize(NormalizationForm.FormC).Trim();
    }
}