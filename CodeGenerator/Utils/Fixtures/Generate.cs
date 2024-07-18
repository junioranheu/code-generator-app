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
        string rootPath = Path.Combine(desktopPath, GetFileName(solutionName));

        if (Directory.Exists(rootPath))
        {
            return rootPath;
        }

        Directory.CreateDirectory(rootPath);
        #endregion

        #region Create children folders;
        List<string> contentPathEnums = GetEnumDescriptionOfAllItemsAndAssignInListStr<ContentPathEnum>();
        GenerateFolderByPathList(solutionName, rootPath, paths: contentPathEnums);
        #endregion

        GetLog("The default folders have been generated successfully");

        return rootPath;
    }

    public static void GenerateFolderByPathList(string solutionName, string rootPath, List<string> paths)
    {
        foreach (var path in paths)
        {
            List<string> splitedPaths = [.. path.Split('/')];

            for (int i = 0; i < splitedPaths.Count; i++)
            {
                bool isMainFolder = (i == 0);
                string previousItemName = (isMainFolder ? string.Empty : $"{solutionName}.{splitedPaths[i - 1]}");
                string currentItemName = Path.Combine(previousItemName, (isMainFolder ? $"{solutionName}.{splitedPaths[i]}" : splitedPaths[i]));

                string finalPath = Path.Combine(rootPath, currentItemName);

                if (Directory.Exists(finalPath))
                {
                    continue;
                }

                Directory.CreateDirectory(finalPath);
            }
        }
    }

    public static void GenerateFolder(string solutionName, string folderPath)
    {
        if (Directory.Exists(folderPath))
        {
            return;
        }

        Directory.CreateDirectory(folderPath);
        GetLog($"Folder {GetStringAfterText(folderPath, $"{solutionName}.")} generated successfully");
    }

    public static void GenerateFile(string solutionName, string rootPath, string fileName, Content content, string extension)
    {
        string pathNormalized = Path.Combine(rootPath, $"{solutionName}.{GetEnumDesc(content.Path)}");
        string pathFinalFile = Path.Combine(pathNormalized, $"{fileName}{extension}");

        File.WriteAllText(pathFinalFile, content.Value.TrimEnd());

        GetLog($"File {fileName}{extension} generated successfully");
    }
}