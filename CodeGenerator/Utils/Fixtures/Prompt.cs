using CodeGenerator.Enums;
using CodeGenerator.Models;
using static CodeGenerator.Utils.Fixtures.Format;
using static CodeGenerator.Utils.Fixtures.Get;

namespace CodeGenerator.Utils.Fixtures;

public static class Prompt
{
    public static string PromptInput(string msg)
    {
        Console.WriteLine(msg);
        string? input = Console.ReadLine();

        if (string.IsNullOrEmpty(input))
        {
            throw new ArgumentException("User input can not be empty");
        }

        return input;
    }

    public static bool PromptInputForBool(string msg)
    {
        Console.WriteLine($"{msg} (Answer y or n)");
        string? input = Console.ReadLine();

        if (string.IsNullOrEmpty(input))
        {
            throw new ArgumentException("User input can not be empty");
        }

        return input.Equals("y", StringComparison.CurrentCultureIgnoreCase);
    }

    public static List<Model> PromptInputForModel()
    {
        List<Model> models = new();
        bool keepWhile = true;

        Console.WriteLine("Class name example: Person");
        Console.WriteLine("Class name example: Name string LastName string Age int Height double IsUnder18 bool\n");

        while (keepWhile)
        {
            string className = PromptInput("\nClass name:");
            string props = PromptInput("Props:");
            models.Add(new() { Name = className, Props = props });

            keepWhile = PromptInputForBool("Do you want to add one more item? (Answer y or n)");
        }

        return models;
    }

    public static string PromptLog(string msg, LogEnum? type = LogEnum.Success)
    {
        ConsoleColor originalColor = ConsoleColor.Gray;

        Console.ForegroundColor = type switch
        {
            LogEnum.Success => ConsoleColor.Cyan,
            LogEnum.Fail => ConsoleColor.Red,
            LogEnum.Warning => ConsoleColor.Yellow,
            LogEnum.Info => originalColor,
            _ => originalColor,
        };

        string final = $"{FormatDateTime(GetDateTime(), DateTimeFormat.CompleteDateTime)} | {msg}";
        Console.WriteLine(final);
        Console.ForegroundColor = originalColor;

        return final;
    }
}