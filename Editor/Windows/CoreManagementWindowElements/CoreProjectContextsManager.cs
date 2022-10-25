using AlchemyBow.Core.Editor.Utilities;
using AlchemyBow.Core.Editor.Wizards;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AlchemyBow.Core.Editor.Windows.CoreManagementWindowElements
{
    internal sealed class CoreProjectContextsManager
    {
        private readonly Type[] projectContextTypes;
        private readonly Object[] projectContextPrefabs;

        private CoreProjectContextsManager(Type[] projectContextTypes, Object[] projectContextPrefabs)
        {
            this.projectContextTypes = projectContextTypes;
            this.projectContextPrefabs = projectContextPrefabs;
        }

        public static CoreProjectContextsManager Create()
        {
            var projectContextTypes = CoreEditorUtility.FindAllCoreProjectContextTypes();
            var projectContextPrefabs = projectContextTypes.Select(t =>
                 CoreEditorUtility.LoadCoreProjectContextFromResources(t)
            ).ToArray();
            return new CoreProjectContextsManager(projectContextTypes, projectContextPrefabs);
        }

        public void OnGUI()
        {
            int numberOfProjectContexts = projectContextTypes.Length;

            if (numberOfProjectContexts == 0)
            {
                EditorGUILayout.HelpBox("No core project context class was detected.", MessageType.Info);
            }
            else
            {
                for (int i = 0; i < numberOfProjectContexts; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(projectContextTypes[i].Name);
                    if (GUILayout.Button("Select Script"))
                    {
                        if (!CoreEditorUtility.TryPingScript(projectContextTypes[i]))
                        {
                            Debug.LogWarning($"Failed to find the script - '{projectContextTypes[i].Name}'. It can happen if the file name doesn't match the class name.");
                        }
                    }
                    if (projectContextPrefabs[i] == null)
                    {
                        if (GUILayout.Button("Create Prefab"))
                        {
                            CoreProjectContextPrefabWizard.CreateWindow(projectContextTypes[i]);
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Select Prefab"))
                        {
                            CoreEditorUtility.FocusProjectWindowAndPingObject(projectContextPrefabs[i]);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            if (GUILayout.Button("New Core Project Context"))
            {
                CoreProjectContextScriptWizard.CreateWindow();
            }
        }
    } 
}
