using AlchemyBow.Core.Editor.Utilities;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AlchemyBow.Core.Editor.Wizards
{
    /// <summary>
    /// Provides a wizard for <c>CoreController</c> scripts.
    /// </summary>
    public sealed class CoreControllerScriptWizard : WizardWindow
    {
        private static Type awakeProjectContextType;

        private Type projectContextType;
        private string folder;
        private bool customNamespace;
        private string namespaceName;
        private string className;

        /// <summary>
        /// Opens a new core controller script wizard window.
        /// </summary>
        /// <param name="projectContextType">A type of the core project context.</param>
        public static void CreateWindow(Type projectContextType)
        {
            awakeProjectContextType = projectContextType;

            var window = EditorWindow.CreateInstance<CoreControllerScriptWizard>();
            window.ShowUtility();
            window.titleContent = new GUIContent("Create Core Controller Script");
            window.minSize = new Vector2(380, 160);
        }

        protected override void Awake()
        {
            projectContextType = awakeProjectContextType;
            awakeProjectContextType = null;
            namespaceName = projectContextType.Namespace;
            className = "MyCoreController";
            folder = "Assets";

            base.Awake();
        }

        protected override void OnValidableGUI()
        {
            folder = CoreEditorUtility.FolderField("Folder", folder);
            className = EditorGUILayout.TextField("Class Name", className);
            customNamespace = GUILayout.Toggle(customNamespace, "Custom Namespace");
            if (customNamespace)
            {
                namespaceName = EditorGUILayout.TextField("Namespace Name", namespaceName);
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);
                namespaceName = projectContextType.Namespace ?? string.Empty;
                EditorGUILayout.TextField("Namespace Name", namespaceName);
                EditorGUI.EndDisabledGroup();
            }
        }

        protected override void Validate(ValidationMessageList validationMessageList)
        {
            WizardValidationUtility.ValidateFolder(folder, validationMessageList);
            WizardValidationUtility.ValidateClassName(className, validationMessageList);

            if (className == projectContextType.Name)
            {
                validationMessageList.AddError("The core controller name must be different then the core project context name.");
            }

            if (!string.IsNullOrEmpty(namespaceName) && className == namespaceName)
            {
                validationMessageList.AddError("The class name must be different then the namespace name.");
            }

            if (customNamespace)
            {
                WizardValidationUtility.ValidateNamespaceName(namespaceName, validationMessageList);
            }
        }

        protected override void Create()
        {
            string fileName = className + ".cs";

            if (!CoreEditorUtility.AssetsPathToSystemPath(folder, out string directoryPath))
            {
                Debug.LogError($"Failed to create '{className}'. '{folder}' is not a valid folder.");
                return;
            }

            if (!CoreEditorUtility.CheckIfCanCreateFilesInFolderAndLogErrors(folder, fileName))
            {
                Debug.LogError($"Failed to create '{className}'. '{fileName}' cannot be created at '{folder}'.");
                return;
            }

            string filePath = $"{directoryPath}/{fileName}";
            var scriptContent = BuildCoreControllerScript(namespaceName, className, projectContextType.Namespace, projectContextType.Name);

            File.WriteAllText(filePath, scriptContent);
            AssetDatabase.Refresh();

            if (CoreEditorUtility.SystemPathToAssetsPath(filePath, out string assetsPath))
            {
                var asset = AssetDatabase.LoadAssetAtPath(assetsPath, typeof(UnityEngine.Object));
                CoreEditorUtility.FocusProjectWindowAndPingObject(asset);
            }

            Debug.Log($"'{fileName}' was created at '{folder}'.");
            Close();
        }

        /// <summary>
        /// Builds a minimal content of the <c>CoreController</c> script.
        /// </summary>
        /// <param name="namespaceName">The controller namespace name.</param>
        /// <param name="className">The controller class name.</param>
        /// <param name="projectContextNamespaceName">The project context namespace name.</param>
        /// <param name="projectContextClassName">The project context class name.</param>
        /// <returns>A minimal content of the <c>CoreController</c> script.</returns>
        public static string BuildCoreControllerScript(string namespaceName, string className,
            string projectContextNamespaceName, string projectContextClassName)
        {
            bool hasNamespace = !string.IsNullOrEmpty(namespaceName);

            var builder = new IndentStringBuilder();
            builder.AppendLine("using AlchemyBow.Core;");
            builder.AppendLine("using System.Collections.Generic;");
            if (!string.IsNullOrEmpty(projectContextNamespaceName) && namespaceName != projectContextNamespaceName)
            {
                builder.AppendLine($"using {projectContextNamespaceName};");
            }

            builder.AppendLine();

            if (hasNamespace)
            {
                builder.AppendLine($"namespace {namespaceName}");
                builder.AppendLine("{");
                builder.IndentLevel++;
            }

            builder.AppendLine($"public class {className} : CoreController<{projectContextClassName}>");
            builder.AppendLine("{");

            builder.IndentLevel++;
            builder.AppendLine("protected override IEnumerable<ICoreLoadable> GetLoadables()");
            builder.AppendLine("{");
            builder.AppendLine("\tyield break;");
            builder.AppendLine("}");
            builder.IndentLevel--;

            builder.AppendLine("}");

            if (hasNamespace)
            {
                builder.IndentLevel--;
                builder.AppendLine("}");
            }

            return builder.ToString();
        }
    }
}
