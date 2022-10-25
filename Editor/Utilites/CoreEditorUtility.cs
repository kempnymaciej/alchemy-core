using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AlchemyBow.Core.Editor.Utilities
{
    /// <summary>
    /// Provides a set of various utility functions for the AlchemyBow.Core editor windows.
    /// </summary>
    public static class CoreEditorUtility
    {
        private static Regex validClassNameRegex;
        private static Regex validNamespaceNameRegex;

        /// <summary>
        /// Validates the class name according to the (English) identifier naming rules.
        /// </summary>
        /// <param name="className">The class name.</param>
        /// <returns><c>true</c> if valid; Otherwise, <c>false</c>.</returns>
        public static bool IsValidClassName(string className)
        {
            if (validClassNameRegex == null)
            {
                validClassNameRegex = new Regex(@"^[A-Za-z_][A-Za-z0-9_]*$");
            }
            return validClassNameRegex.IsMatch(className);
        }

        /// <summary>
        /// Validates the namespace name according to the (English) identifier naming rules.
        /// </summary>
        /// <param name="namespaceName">The namespace name.</param>
        /// <returns><c>true</c> if valid; Otherwise, <c>false</c>.</returns>
        public static bool IsValidNamespaceName(string namespaceName)
        {
            if (validNamespaceNameRegex == null)
            {
                validNamespaceNameRegex = new Regex(@"^[A-Za-z_][A-Za-z0-9_]*(?:\.[A-Za-z_][A-Za-z0-9_]*)*$");
            }
            return validNamespaceNameRegex.IsMatch(namespaceName);
        }

        /// <summary>
        /// Determines the name of the Unity project.
        /// </summary>
        /// <returns>The name of the Unity project.</returns>
        public static string GetProjectName()
        {
            string path = Application.dataPath;
            path = path.Substring(0, path.Length - "*Assets".Length);
            return Path.GetFileName(path);
        }

        /// <summary>
        /// Makes a field for selecting folders inside the 'Assets' folder.
        /// </summary>
        /// <param name="label">The label to display in front of the field.</param>
        /// <param name="currentFolder">The current folder.</param>
        /// <returns>The folder selected by user if modified and valid; Otherwise, the current folder.</returns>
        public static string FolderField(string label, string currentFolder)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField(new GUIContent(label, currentFolder), currentFolder);
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("..", GUILayout.MaxWidth(16)))
            {
                var directoryPath = EditorUtility.SaveFolderPanel("Select Folder", "Assets", "");
                if (SystemPathToAssetsPath(directoryPath, out string folder))
                {
                    currentFolder = folder;
                    AssetDatabase.Refresh();
                }
                else
                {
                    if (!string.IsNullOrEmpty(directoryPath))
                    {
                        Debug.LogError("You can only select folders under the 'Assets' folder.");
                    }
                }
            }

            EditorGUILayout.EndHorizontal();
            return currentFolder;
        }

        /// <summary>
        /// Checks if files with the specified names can be created in the specified folder (inside the 'Assets' folder) and logs errors to the console.
        /// </summary>
        /// <param name="folder">The folder (inside the 'Assets' folder).</param>
        /// <param name="fileNames">The file names.</param>
        /// <returns><c>true</c> if there are no errors; Otherwise, <c>false</c>.</returns>
        public static bool CheckIfCanCreateFilesInFolderAndLogErrors(string folder, params string[] fileNames)
        {
            if (!AssetDatabase.IsValidFolder(folder))
            {
                Debug.LogError($"'{folder}' is not a valid folder (in Assets).");
                return false;
            }

            if (!AssetsPathToSystemPath(folder, out string directoryPath))
            {
                Debug.LogError($"'{folder}' cannot be converted to a system path.");
                return false;
            }

            fileNames ??= Array.Empty<string>();
            var filePaths = fileNames.Select(fn => $"{directoryPath}/{fn}").ToArray();

            var uniquePaths = new HashSet<string>(filePaths);
            if (filePaths.Length != uniquePaths.Count)
            {
                string filePathList = "";
                foreach (var filePath in filePaths)
                {
                    filePathList += filePath + "\n";
                }
                Debug.LogError("File names must be unique.\n" + filePathList);
                return false;
            }

            var existingFilesPaths = new HashSet<string>(Directory.GetFiles(directoryPath).Select(dp => EnsureOnlyForwardSlashes(dp)));
            foreach (var filePath in filePaths)
            {
                if (existingFilesPaths.Contains(filePath))
                {
                    Debug.LogError($"A file '{filePath}' already exists.");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the resource path for the specified core project context type.
        /// </summary>
        /// <param name="coreProjectContextType">The core project context type.</param>
        /// <returns>The resource path for the specified core project context type.</returns>
        public static string GetCoreProjectContextResourcesPath(Type coreProjectContextType)
        {
            return $"Core/{coreProjectContextType.Name}";
        }

        /// <summary>
        /// Loads the prefab for the specified core project context type from resources.
        /// </summary>
        /// <param name="coreProjectContextType">The core project context type.</param>
        /// <returns>The prefab if it exists; Otherwise, <c>null</c>.</returns>
        public static GameObject LoadCoreProjectContextFromResources(Type coreProjectContextType)
        {
            return Resources.Load<GameObject>(GetCoreProjectContextResourcesPath(coreProjectContextType));
        }

        /// <summary>
        /// Attempts to ping a mono script for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if succeeded; Otherwise, <c>false</c>.</returns>
        public static bool TryPingScript(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            string className = type.Name;
            var guids = AssetDatabase.FindAssets($"t:MonoScript {className}", null);

            if (guids.Length > 0)
            {
                foreach (var guid in guids)
                {
                    var script = AssetDatabase.LoadAssetAtPath<MonoScript>(AssetDatabase.GUIDToAssetPath(guid));
                    if (script != null && script.GetClass() == type)
                    {
                        FocusProjectWindowAndPingObject(script);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Attempts to find or create the best match for '.../Resources/Core' in the specified folder.
        /// </summary>
        /// <param name="parentFolder">The parent folder for '.../Resources/Core'.</param>
        /// <returns>The folder if succeeded; Otherwise, <c>null</c>.</returns>
        public static string TryGetCoreFolderAt(string parentFolder)
        {
            if (!AssetDatabase.IsValidFolder(parentFolder))
            {
                Debug.LogError($"'{parentFolder}' is not a valid folder.");
                return null;
            }

            if (parentFolder.EndsWith("/Resources/Core"))
            {
                return parentFolder;
            }
            else if (parentFolder.EndsWith("/Resources"))
            {
                if (!AssetDatabase.GetSubFolders(parentFolder).Any(sf => sf.EndsWith("/Core")))
                {
                    AssetDatabase.CreateFolder(parentFolder, "Core");
                }
                return parentFolder + "/Core";
            }
            else
            {
                if (!AssetDatabase.GetSubFolders(parentFolder).Any(sf => sf.EndsWith("/Resources")))
                {
                    AssetDatabase.CreateFolder(parentFolder, "Resources");
                }
                return TryGetCoreFolderAt(parentFolder + "/Resources");
            }
        }

        /// <summary>
        /// Gets all 'Assets/.../Resources/Core' folders in the project.
        /// </summary>
        /// <returns>An array of 'Assets/.../Resources/Core' folders.</returns>
        public static string[] GetAllCoreFolders()
        {
            var coreFolders = new List<string>();
            foreach (var resourceFolder in GetAllResourcesFolders())
            {
                foreach (var folder in AssetDatabase.GetSubFolders(resourceFolder))
                {
                    if (folder.EndsWith("/Core"))
                    {
                        coreFolders.Add(folder);
                    }
                }
            }

            return coreFolders.ToArray();
        }

        /// <summary>
        ///  Gets all 'Assets/.../Resources' folders in the project.
        /// </summary>
        /// <returns>An array of 'Assets/.../Resources' folders.</returns>
        public static string[] GetAllResourcesFolders()
        {
            var resourceFolders = new List<string>();
            GetAllResourcesFoldersRecursive("Assets", resourceFolders);
            return resourceFolders.ToArray();
        }

        private static void GetAllResourcesFoldersRecursive(string parentFolder, List<string> folders)
        {
            if (parentFolder.EndsWith("/Resources"))
            {
                folders.Add(parentFolder);
                return;
            }

            foreach (var folder in AssetDatabase.GetSubFolders(parentFolder))
            {
                GetAllResourcesFoldersRecursive(folder, folders);
            }
        }

        /// <summary>
        /// Finds the types of all non-abstract <c>CoreProjectContext</c> subclasses.
        /// </summary>
        /// <returns>The types of all non-abstract <c>CoreProjectContext</c> subclasses.</returns>
        public static Type[] FindAllCoreProjectContextTypes()
        {
            var projectContextType = typeof(CoreProjectContext);
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a
                .GetTypes().Where(t => t.IsSubclassOf(projectContextType) && !t.IsAbstract)
            ).ToArray();
        }

        /// <summary>
        /// Finds the types of all non-abstract <c>CoreController</c> subclasses for the specified <c>CoreProjectContext</c> type.
        /// </summary>
        /// <param name="projectContextType">The project context type used to filter the result.</param>
        /// <returns>The types of all non-abstract <c>CoreController</c> subclasses for the specified <c>CoreProjectContext</c> type.</returns>
        public static Type[] FindAllCoreControllerTypes(Type projectContextType)
        {
            if (!projectContextType.IsSubclassOf(typeof(CoreProjectContext)))
            {
                return Array.Empty<Type>();
            }

            var controllerType = typeof(CoreController<>).MakeGenericType(new[] { projectContextType });
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a
                .GetTypes().Where(t => t.IsSubclassOf(controllerType) && !t.IsAbstract)
            ).ToArray();
        }

        /// <summary>
        /// Focuses the project window and pings the specified object.
        /// </summary>
        /// <param name="obj">The object to ping.</param>
        public static void FocusProjectWindowAndPingObject(Object obj)
        {
            EditorUtility.FocusProjectWindow();
            EditorGUIUtility.PingObject(obj);
        }

        /// <summary>
        /// Converts a project local path (starting with "Assets") to an absolute system path.
        /// </summary>
        /// <param name="assetsPath">A project local path (starting with "Assets").</param>
        /// <param name="systemPath">The result absolute system path.</param>
        /// <returns><c>true</c> if the conversion is successful; Otherwise, <c>false</c>.</returns>
        public static bool AssetsPathToSystemPath(string assetsPath, out string systemPath)
        {
            if (assetsPath != null && assetsPath.StartsWith("Assets"))
            {
                string assetsFolderPath = Application.dataPath;
                systemPath = assetsFolderPath + assetsPath.Substring("Assets".Length);
                return true;
            }

            systemPath = null;
            return false;
        }

        /// <summary>
        /// Converts an absolute system path to a project local path (starting with "Assets").
        /// </summary>
        /// <param name="systemPath">An absolute system path.</param>
        /// <param name="assetsPath">The result project local path (starting with "Assets").</param>
        /// <returns><c>true</c> if the conversion is successful; Otherwise, <c>false</c>.</returns>
        public static bool SystemPathToAssetsPath(string systemPath, out string assetsPath)
        {
            systemPath = EnsureOnlyForwardSlashes(systemPath);
            string assetsFolderPath = Application.dataPath;
            if (systemPath != null && systemPath.StartsWith(assetsFolderPath))
            {
                assetsPath = systemPath.Substring(assetsFolderPath.Length - "Assets".Length);
                return true;
            }

            assetsPath = null;
            return false;
        }


        /// <summary>
        /// Replaces all backslashes with forward slashes.
        /// </summary>
        /// <param name="value">The initial value.</param>
        /// <returns>A value with all backslashes replaced by forward slashes.</returns>
        public static string EnsureOnlyForwardSlashes(string value)
        {
            return value.Replace("\\", "/");
        }
    } 
}
