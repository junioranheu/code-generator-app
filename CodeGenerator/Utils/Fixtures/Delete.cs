using static CodeGenerator.Console.Utils.Fixtures.Get;
using static CodeGenerator.Console.Utils.Fixtures.Prompt;

namespace CodeGenerator.Console.Utils.Fixtures;

public static class Delete
{
    public static void DeleteFolder(string path)
    {
        if (!Directory.Exists(path))
        {
            throw new ArgumentException("Source directory does not exist", nameof(path));
        }

        Directory.Delete(path, true);
        PromptLog($"Folder {GetStringAfterText(path, GetSolutionName())} has been deleted");
    }

    public static void DeleteFile(string path)
    {
        if (!File.Exists(path))
        {
            throw new ArgumentException("File directory does not exist", nameof(path));
        }

        File.Delete(path);
        PromptLog($"Folder {GetStringAfterText(path, GetSolutionName())} has been deleted");
    }
}