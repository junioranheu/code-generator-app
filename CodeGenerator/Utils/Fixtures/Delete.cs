using static CodeGenerator.Utils.Fixtures.Get;

namespace CodeGenerator.Utils.Fixtures;

public static class Delete
{
    public static void DeleteFolder(string path)
    {
        if (!Directory.Exists(path))
        {
            throw new ArgumentException("Source directory does not exist", nameof(path));
        }

        Directory.Delete(path, true);
        GetLog($"Folder {GetStringAfterText(path, GetSolutionName())} has been deleted");
    }

    public static void DeleteFile(string path)
    {
        if (!File.Exists(path))
        {
            throw new ArgumentException("File directory does not exist", nameof(path));
        }

        File.Delete(path);
        GetLog($"Folder {GetStringAfterText(path, GetSolutionName())} has been deleted");
    }
}