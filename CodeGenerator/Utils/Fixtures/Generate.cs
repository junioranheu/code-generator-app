using CodeGenerator.Enums;
using CodeGenerator.Models;
using System.Runtime.InteropServices;
using static CodeGenerator.Utils.Fixtures.Get;

namespace CodeGenerator.Utils.Fixtures;

public static class Generate
{
    public static string GenerateDefaultDirectories(string solutionName)
    {
        #region Create main folder
        string desktopPath;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            string homePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            desktopPath = Path.Combine(homePath, "Desktop");
        }
        else
        {
            throw new NotSupportedException("Unsupported operating system");
        }

        string rootPath = Path.Combine(desktopPath, GetFileName(solutionName));

        if (Directory.Exists(rootPath))
        {
            return rootPath;
        }

        Directory.CreateDirectory(rootPath);
        GetLog($"Main folder has been successfully generated");
        #endregion

        #region Create children folders;
        List<string> contentPathEnums = GetEnumDescriptionOfAllItemsAndAssignInListStr<ContentPathEnum>();
        GenerateFolderByPathList(solutionName, rootPath, paths: contentPathEnums);
        #endregion

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
                GetLog($"Folder {GetStringAfterText(finalPath, $"{solutionName}.")} has been successfully generated");
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
        GetLog($"Folder {GetStringAfterText(folderPath, $"{solutionName}.")} has been successfully generated");
    }

    public static void GenerateFile(string solutionName, string pathFinalFile, Content content)
    {
        File.WriteAllText(pathFinalFile, content.Value.TrimEnd());
        GetLog($"File {GetStringAfterText(pathFinalFile, $"{solutionName}.")} has been successfully generated");
    }
}