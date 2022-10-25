using AlchemyBow.Core.Editor.Utilities;
using AlchemyBow.Core.Editor.Wizards;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AlchemyBow.Core.Editor.Windows.CoreManagementWindowElements
{
    internal sealed class CoreControllersManager
    {
        private readonly Type[] projectContextTypes;
        private readonly string[] projectContextNames;
        private int selectedProjectContextIndex;

        private Type[] controllerTypes;

        private CoreControllersManager(Type[] projectContextTypes, string[] projectContextNames, Type[] controllerTypes)
        {
            this.projectContextTypes = projectContextTypes;
            this.projectContextNames = projectContextNames;
            this.controllerTypes = controllerTypes;
        }

        public static CoreControllersManager Create()
        {
            var projectContextTypes = CoreEditorUtility.FindAllCoreProjectContextTypes();
            var projectContextNames = projectContextTypes.Select(t => t.Name).ToArray();
            Type[] controllerTypes;
            if (projectContextTypes.Length > 0)
            {
                controllerTypes = CoreEditorUtility.FindAllCoreControllerTypes(projectContextTypes[0]);
            }
            else
            {
                controllerTypes = Array.Empty<Type>();
            }
            return new CoreControllersManager(projectContextTypes, projectContextNames, controllerTypes);
        }

        public void OnGUI()
        {
            int numberOfProjectContexts = projectContextTypes.Length;
            if (numberOfProjectContexts == 0)
            {
                EditorGUILayout.HelpBox("You need at least one core project context.", MessageType.Info);
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                selectedProjectContextIndex = EditorGUILayout.Popup(selectedProjectContextIndex, projectContextNames);
                if (EditorGUI.EndChangeCheck())
                {
                    controllerTypes = CoreEditorUtility.FindAllCoreControllerTypes(projectContextTypes[selectedProjectContextIndex]);
                }

                EditorGUILayout.Space(1);

                int numberOfControllers = controllerTypes.Length;
                for (int i = 0; i < numberOfControllers; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(controllerTypes[i].Name);
                    if (GUILayout.Button("Select Script"))
                    {
                        if (!CoreEditorUtility.TryPingScript(controllerTypes[i]))
                        {
                            Debug.LogWarning($"Failed to find the script - '{controllerTypes[i].Name}'. It can happen if the file name doesn't match the class name.");
                        }
                    }
                    if (GUILayout.Button("Create Instance"))
                    {
                        CreateControllerInstance(controllerTypes[i]);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                if (numberOfProjectContexts > 0)
                {
                    if (GUILayout.Button("New Core Controller"))
                    {
                        CoreControllerScriptWizard.CreateWindow(projectContextTypes[selectedProjectContextIndex]);
                    }
                }
            }
        }

        private void CreateControllerInstance(Type type)
        {
            GameObject go = new GameObject(type.Name);
            go.AddComponent(type);
            Undo.RegisterCreatedObjectUndo(go, $"Create {type.Name}");

            EditorGUIUtility.PingObject(go);
            Debug.Log($"{go.name} was created on the active scene. (You can undo this operation.)", go);
        }
    } 
}
