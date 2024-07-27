using System.Runtime.InteropServices;
using CodeGenerator.Enums;
using CodeGenerator.Models;
using static CodeGenerator.Utils.Fixtures.Get;

namespace CodeGenerator.Utils.Fixtures;

public static class Generate
{
    public static string GenerateDefaultDirectories(string solutionName, bool isGenerateZip)
    {
        #region Create main folder
        string rootPath = string.Empty;

        if (isGenerateZip)
        {
            string projectDirectory = GetProjectDirectory();
            rootPath = Path.Combine(projectDirectory, "Temp");
        }
        else
        {
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

            string folderName = GetMainFolderName(solutionName);
            rootPath = Path.Combine(desktopPath, folderName);

            if (Directory.Exists(rootPath))
            {
                Directory.Delete(rootPath, true);
            }

            Directory.CreateDirectory(rootPath);
            GetLog($"Main folder has been successfully generated: {folderName}");
        }  
        #endregion

        #region Create children folders;
        List<string> contentDirectoryEnums = GetEnumDescriptionOfAllItemsAndAssignInListStr<ContentDirectoryEnum>();
        GenerateFolderByPathList(solutionName, rootPath, paths: contentDirectoryEnums, isCheckMainFolder: true);
        #endregion

        return rootPath;
    }

    public static void GenerateFolderByPathList(string solutionName, string rootPath, List<string> paths, bool? isCheckMainFolder = false)
    {
        foreach (var path in paths)
        {
            List<string> splitedPaths = [.. path.Split('/')];

            for (int i = 0; i < splitedPaths.Count; i++)
            {
                bool isMainFolder = (i == 0);
                string previousItemName = (isCheckMainFolder.GetValueOrDefault() == false ? string.Empty : (isMainFolder ? string.Empty : $"{solutionName}.{splitedPaths[i - 1]}"));
                string currentItemName = Path.Combine(previousItemName, ((isMainFolder && isCheckMainFolder.GetValueOrDefault()) ? $"{solutionName}.{splitedPaths[i]}" : splitedPaths[i]));

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

    public static void GenerateFiles(List<Content> contents, bool isGenerateZip)
    {
        foreach (var content in contents)
        {
            File.WriteAllText(content.FileFinalPath, content.Value.TrimEnd());
            GetLog($"File {GetStringAfterText(content.FileFinalPath, $"{content.SolutionName}.")} has been successfully generated");
        }
    }
}