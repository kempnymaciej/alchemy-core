using AlchemyBow.Core.Editor.Utilities;
using System;
using UnityEditor;
using UnityEngine;

namespace AlchemyBow.Core.Editor.Wizards
{
    /// <summary>
    /// Provides a wizard for <c>CoreProjectContext</c> prefabs.
    /// </summary>
    public sealed class CoreProjectContextPrefabWizard : WizardWindow
    {
        private static Type awakeProjectContextType;

        private Type projectContextType;
        private bool createNewCoreFolder;
        private string newCoreFolder = "Assets";

        private int selectedCoreFolderIndex;
        private string[] coreFolders;

        /// <summary>
        /// Opens a new core project context prefab wizard window.
        /// </summary>
        /// <param name="projectContextType">A type of the core project context.</param>
        public static void CreateWindow(Type projectContextType)
        {
            awakeProjectContextType = projectContextType;

            var window = EditorWindow.CreateInstance<CoreProjectContextPrefabWizard>();
            window.ShowUtility();
            window.titleContent = new GUIContent("Create CoreProjectContext Pefab");
            window.minSize = new Vector2(380, 160);
        }

        protected override void Awake()
        {
            projectContextType = awakeProjectContextType;
            awakeProjectContextType = null;
            newCoreFolder = "Assets";
            coreFolders = CoreEditorUtility.GetAllCoreFolders();
            base.Awake();
        }

        protected override void OnProjectChange()
        {
            if (projectContextType == null)
            {
                Close();
            }
            else
            {
                coreFolders = CoreEditorUtility.GetAllCoreFolders();
                if(selectedCoreFolderIndex >= coreFolders.Length)
                {
                    selectedCoreFolderIndex = 0;
                }
                Validate();
            }
        }

        protected override void OnValidableGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("ProjectContext", projectContextType.Name);
            EditorGUI.EndDisabledGroup();

            int numberOfCoreFolders = coreFolders.Length;
            if (numberOfCoreFolders == 0)
            {
                createNewCoreFolder = true;
                newCoreFolder = CoreEditorUtility.FolderField("Folder", newCoreFolder);
            }
            else
            {
                createNewCoreFolder = GUILayout.Toggle(createNewCoreFolder, "Create New Core Folder");
                if (createNewCoreFolder)
                {
                    newCoreFolder = CoreEditorUtility.FolderField("Folder", newCoreFolder);
                }
                else
                {
                    if (numberOfCoreFolders == 1)
                    {
                        selectedCoreFolderIndex = 0;
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.TextField("Folder", coreFolders[selectedCoreFolderIndex]);
                        EditorGUI.EndDisabledGroup();
                    }
                    else
                    {
                        selectedCoreFolderIndex = EditorGUILayout.Popup("Folder", selectedCoreFolderIndex, coreFolders);
                    }
                }
            }
        }

        protected override void Validate(ValidationMessageList validationMessageList)
        {
            if (createNewCoreFolder)
            {
                WizardValidationUtility.ValidateFolder(newCoreFolder, validationMessageList);
            }
        }

        protected override void Create()
        {
            string fileName = $"{projectContextType.Name}.prefab";

            string folder;
            if (createNewCoreFolder)
            {
                folder = CoreEditorUtility.TryGetCoreFolderAt(newCoreFolder);
                if (folder == null)
                {
                    Debug.LogError($"Failed to create '{fileName}'. Could not get the core folder from '{newCoreFolder}'.");
                    return;
                }
            }
            else
            {
                folder = coreFolders[selectedCoreFolderIndex];
            }

            if (!CoreEditorUtility.CheckIfCanCreateFilesInFolderAndLogErrors(folder, fileName))
            {
                Debug.LogError($"Failed to create '{fileName}'. It cannot be created at '{folder}'.");
                return;
            }

            var go = new GameObject(projectContextType.Name);
            go.AddComponent(projectContextType);
            var prefab = PrefabUtility.SaveAsPrefabAsset(go, $"{folder}/{fileName}", out bool success);

            GameObject.DestroyImmediate(go);

            if (success)
            {
                Debug.Log($"'{fileName}' was created at '{folder}'.");
                CoreEditorUtility.FocusProjectWindowAndPingObject(prefab);
                Close();
            }
            else
            {
                Debug.LogError($"Failed to create '{fileName}' at '{folder}'. Could not save a prefab.");
            }
        }
    }
}
