using AlchemyBow.Core.Editor.Utilities;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AlchemyBow.Core.Editor.Wizards
{
    /// <summary>
    /// Provides a quick start wizard for AlchemyBow.Core.
    /// </summary>
    public sealed class CoreQuickStartWizard : WizardWindow
    {
        private const string CreationDataSessionStateKey = "AlchemyBow_Core_QuickStartCreationData";

        private string folder;
        private string namespaceName;
        private string projectContextClassName;
        private string controllerClassName;

        private bool createProjectContextPrefab;
        private bool createControllerInstance;

        /// <summary>
        /// Opens a new core quick start wizard window.
        /// </summary>
        [MenuItem("Window/AlchemyBow/Core/Quick Start Core", priority = 0)]
        public static void CreateWindow()
        {
            var window = EditorWindow.CreateInstance<CoreQuickStartWizard>();
            window.ShowUtility();
            window.titleContent = new GUIContent("Quick Start Core");
            window.minSize = new Vector2(380, 200);
        }

        protected override void Awake()
        {
            folder = "Assets";
            createProjectContextPrefab = true;
            createControllerInstance = true;

            string projectName = CoreEditorUtility.GetProjectName().Trim();
            if (CoreEditorUtility.IsValidClassName(projectName))
            {
                namespaceName = projectName;
                projectContextClassName = $"{projectName}CoreProjectContext";
                controllerClassName = $"{projectName}CoreController";
            }
            else
            {
                namespaceName = "";
                projectContextClassName = $"MyCoreProjectContext";
                controllerClassName = $"MyCoreController";
            }

            base.Awake();
        }

        protected override void OnValidableGUI()
        {
            folder = CoreEditorUtility.FolderField("Folder", folder);
            namespaceName = EditorGUILayout.TextField("Namespace Name", namespaceName);

            EditorGUILayout.LabelField("CoreProjectContext");
            using (new EditorGUI.IndentLevelScope())
            {
                projectContextClassName = EditorGUILayout.TextField("Class Name", projectContextClassName);
                createProjectContextPrefab = EditorGUILayout.Toggle("Create Prefab", createProjectContextPrefab); 
                if (!string.IsNullOrWhiteSpace(projectContextClassName) && createProjectContextPrefab)
                {
                    EditorGUILayout.HelpBox($"'.../Resources/Core/{projectContextClassName}.prefab' will be created at '{folder}/...'. You can move it anywhere.", MessageType.Info);
                }
            }

            EditorGUILayout.LabelField("CoreController");
            using (new EditorGUI.IndentLevelScope())
            {
                controllerClassName = EditorGUILayout.TextField("Class Name", controllerClassName);
                createControllerInstance = EditorGUILayout.Toggle("Create Instance", createControllerInstance);
                if (!string.IsNullOrWhiteSpace(controllerClassName) && createControllerInstance)
                {
                    EditorGUILayout.HelpBox($"An instance of '{controllerClassName}' will be created on the active scene.", MessageType.Info);
                }
            }
        }

        protected override void Validate(ValidationMessageList validationMessageList)
        {
            if (!string.IsNullOrEmpty(controllerClassName) && !string.IsNullOrEmpty(projectContextClassName))
            {
                if (namespaceName == controllerClassName || namespaceName == projectContextClassName || controllerClassName == projectContextClassName)
                {
                    validationMessageList.AddError("CoreProjectContext, CoreController and Namespace must have unique names.");
                }
            }

            WizardValidationUtility.ValidateFolder(folder, validationMessageList);
            WizardValidationUtility.ValidateClassName(projectContextClassName, validationMessageList);
            WizardValidationUtility.ValidateClassName(controllerClassName, validationMessageList);
            WizardValidationUtility.ValidateNamespaceName(namespaceName, validationMessageList);
        }

        protected override void Create()
        {
            if (!CoreEditorUtility.AssetsPathToSystemPath(folder, out string directoryPath))
            {
                Debug.LogError($"Quick start failed. '{folder}' is not a valid folder.");
                return;
            }

            if (!CoreEditorUtility.CheckIfCanCreateFilesInFolderAndLogErrors(folder, $"{controllerClassName}.cs", $"{projectContextClassName}.cs"))
            {
                Debug.LogError($"Quick start failed. '{controllerClassName}.cs' or/and '{projectContextClassName}.cs' cannot be created at '{folder}'.");
                return;
            }

            var projectContextContent = CoreProjectContextScriptWizard.BuildCoreProjectContextScript(namespaceName, projectContextClassName);
            var controllerContent = CoreControllerScriptWizard.BuildCoreControllerScript(namespaceName, controllerClassName, namespaceName, projectContextClassName);

            File.WriteAllText($"{directoryPath}/{projectContextClassName}.cs", projectContextContent);
            File.WriteAllText($"{directoryPath}/{controllerClassName}.cs", controllerContent);
            AssetDatabase.Refresh();

            CoreEditorUtility.FocusProjectWindowAndPingObject(AssetDatabase.LoadAssetAtPath<Object>($"{folder}/{projectContextClassName}.cs"));

            Debug.Log($"'{controllerClassName}.cs' and '{projectContextClassName}.cs' were created at '{folder}'.");

            if (createControllerInstance || createProjectContextPrefab)
            {
                var creationRequest = new CreationRequest(folder);
                if (createProjectContextPrefab)
                {
                    creationRequest.SetProjectContextData(projectContextClassName);
                }
                if (createControllerInstance)
                {
                    creationRequest.SetControllerData(controllerClassName);
                }
                ScheduleCreation(creationRequest);
            }

            Close();
        }

        private static void ScheduleCreation(CreationRequest creationRequest)
        {
            SessionState.SetString(CreationDataSessionStateKey, JsonUtility.ToJson(creationRequest));
        }

        [InitializeOnLoadMethod]
        private static void OnProjectLoadedInEditor()
        {
            var creationRequestJson = SessionState.GetString(CreationDataSessionStateKey, null);
            if (creationRequestJson == null)
            {
                return;
            }

            SessionState.EraseString(CreationDataSessionStateKey);

            var creationRequest = new CreationRequest();
            EditorJsonUtility.FromJsonOverwrite(creationRequestJson, creationRequest);
            
            EditorApplication.delayCall += creationRequest.Create;
        }

        [System.Serializable]
        private sealed class CreationRequest
        {
            [SerializeField]
            private string folder;
            [SerializeField]
            private string projectContextClassName;
            [SerializeField]
            private string controllerClassName;

            public CreationRequest()
            {
                
            }

            public CreationRequest(string folder)
            {
                this.folder = folder;
            }

            public void SetProjectContextData(string projectContextClassName)
            {
                this.projectContextClassName = projectContextClassName;
            }

            public void SetControllerData(string controllerClassName)
            {
                this.controllerClassName = controllerClassName;
            }

            public void Create()
            {
                if (!string.IsNullOrEmpty(folder) && !string.IsNullOrEmpty(projectContextClassName))
                {
                    CreateProjectContextPrefab();
                }

                if (!string.IsNullOrEmpty(folder) && !string.IsNullOrEmpty(controllerClassName))
                {
                    CreateControllerInstance();
                }
            }

            private void CreateProjectContextPrefab()
            {
                var projectContextScript = AssetDatabase.LoadAssetAtPath<MonoScript>($"{folder}/{projectContextClassName}.cs");
                if (projectContextScript == null)
                {
                    Debug.LogError($"Failed to create '{projectContextClassName}.prefab'. Could not load '{folder}/{projectContextClassName}.cs'.");
                    return;
                }

                var projectContextType = projectContextScript.GetClass();
                if (projectContextScript == null)
                {
                    Debug.LogError($"Failed to create '{projectContextClassName}.prefab'. Could not load the type of '{folder}/{projectContextClassName}.cs'.");
                    return;
                }

                var coreFolder = CoreEditorUtility.TryGetCoreFolderAt(folder);
                if (coreFolder == null)
                {
                    Debug.LogError($"Failed to create '{projectContextClassName}.prefab'. Could not get the core folder from '{folder}'.");
                    return;
                }

                if (!CoreEditorUtility.CheckIfCanCreateFilesInFolderAndLogErrors(coreFolder, $"{projectContextClassName}.prefab"))
                {
                    Debug.LogError($"Failed to create '{projectContextClassName}.prefab'. It cannot be created at '{coreFolder}'.");
                    return;
                }

                var go = new GameObject(projectContextClassName);
                go.AddComponent(projectContextType);
                PrefabUtility.SaveAsPrefabAsset(go, $"{coreFolder}/{projectContextClassName}.prefab", out bool success);
                GameObject.DestroyImmediate(go);

                if (success)
                {
                    Debug.Log($"'{projectContextClassName}.prefab' was created at '{coreFolder}'.");
                }
                else
                {
                    Debug.LogError($"Failed to create '{projectContextClassName}.prefab' at '{folder}'. Could not save a prefab.");
                }
            }

            private void CreateControllerInstance()
            {
                var controllerScript = AssetDatabase.LoadAssetAtPath<MonoScript>($"{folder}/{controllerClassName}.cs");
                if (controllerScript == null)
                {
                    Debug.LogError($"Failed to instantiate '{controllerClassName}'. Could not load '{folder}/{controllerClassName}.cs'.");
                    return;
                }

                var controllerType = controllerScript.GetClass();
                if (controllerType == null)
                {
                    Debug.LogError($"Failed to instantiate '{controllerClassName}'. Could not load the type of '{folder}/{controllerClassName}.cs'.");
                    return;
                }

                GameObject go = new GameObject(controllerType.Name);
                go.AddComponent(controllerType);
                Undo.RegisterCreatedObjectUndo(go, $"Create {controllerType.Name}");

                Debug.Log($"{go.name} was created on the active scene. (You can undo this operation.)", go);
            }
        }
    }
}
