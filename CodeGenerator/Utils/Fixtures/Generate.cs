using CodeGenerator.Enums;
using CodeGenerator.Models;
using static CodeGenerator.Utils.Fixtures.Get;

namespace CodeGenerator.Utils.Fixtures;

public static class Generate
{
    public static string GenerateDefaultDirectories(string solutionName)
    {
        #region Create main folder
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string folderPath = Path.Combine(desktopPath, GetFileName(solutionName));

        if (Directory.Exists(folderPath))
        {
            return folderPath;
        }

        Directory.CreateDirectory(folderPath);
        #endregion

        #region Create children folders;
        List<string> contentPathEnums = GetEnumDescriptionOfAllItemsAndAssignInListStr<ContentPathEnum>();

        foreach (var path in contentPathEnums)
        {
            List<string> paths = [.. path.Split('/')];

            for (int i = 0; i < paths.Count; i++)
            {
                bool isMainFolder = (i == 0);
                string previousItemName = (isMainFolder ? string.Empty : $"{solutionName}.{paths[i - 1]}");
                string currentItemName = Path.Combine(previousItemName, (isMainFolder ? $"{solutionName}.{paths[i]}" : paths[i]));

                string finalPath = Path.Combine(folderPath, currentItemName);

                if (Directory.Exists(finalPath))
                {
                    continue;
                }

                Directory.CreateDirectory(finalPath);
            }
        }
        #endregion

        GetLog("The default folders have been generated successfully");

        return folderPath;
    }

    public static void GenerateFile(string solutionName, string path, string fileName, Content content, string extension)
    {
        string pathNormalized = Path.Combine(path, $"{solutionName}.{GetEnumDesc(content.Path)}");
        string pathFinalFile = Path.Combine(pathNormalized, $"{fileName}{extension}");

        File.WriteAllText(pathFinalFile, content.Value.TrimEnd());

        GetLog($"File {fileName}{extension} generated successfully");
    }
}