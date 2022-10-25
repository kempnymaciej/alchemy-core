using AlchemyBow.Core.Editor.Utilities;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AlchemyBow.Core.Editor.Wizards
{
    /// <summary>
    /// Provides a wizard for <c>CoreProjectContext</c> scripts.
    /// </summary>
    public sealed class CoreProjectContextScriptWizard : WizardWindow
    {
        private string folder;
        private string namespaceName;
        private string className;

        /// <summary>
        /// Opens a new the core project context script wizard window.
        /// </summary>
        public static void CreateWindow()
        {
            var window = EditorWindow.CreateInstance<CoreProjectContextScriptWizard>();
            window.ShowUtility();
            window.titleContent = new GUIContent("Create CoreProjectContext Script");
            window.minSize = new Vector2(380, 160);
        }

        protected override void Awake()
        {
            folder = "Assets";
            namespaceName = "";
            className = "MyCoreProjectContext";

            base.Awake();
        }

        protected override void OnValidableGUI()
        {
            folder = CoreEditorUtility.FolderField("Folder", folder);
            className = EditorGUILayout.TextField("Class Name", className);
            namespaceName = EditorGUILayout.TextField("Namespace Name", namespaceName);
        }

        protected override void Validate(ValidationMessageList validationMessageList)
        {
            WizardValidationUtility.ValidateFolder(folder, validationMessageList);

            if (!string.IsNullOrEmpty(className) && !string.IsNullOrEmpty(namespaceName) && className == namespaceName)
            {
                validationMessageList.AddError("Class and Namespace must have unique names.");
            }

            WizardValidationUtility.ValidateClassName(className, validationMessageList);
            WizardValidationUtility.ValidateNamespaceName(namespaceName, validationMessageList);
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
                Debug.LogError($"Failed to create '{className}'. It cannot be created at '{folder}'.");
                return;
            }

            string filePath = $"{directoryPath}/{fileName}";
            var scriptContent = BuildCoreProjectContextScript(namespaceName, className);

            File.WriteAllText(filePath, scriptContent);
            AssetDatabase.Refresh();

            if (CoreEditorUtility.SystemPathToAssetsPath(filePath, out string assetsPath))
            {
                var asset = AssetDatabase.LoadAssetAtPath(assetsPath, typeof(Object));
                CoreEditorUtility.FocusProjectWindowAndPingObject(asset);
            }

            Debug.Log($"'{fileName}' was created at '{folder}'.");
            Close();
        }

        /// <summary>
        /// Builds a minimal content of the <c>CoreProjectContext</c> script.
        /// </summary>
        /// <param name="namespaceName">The namespace name.</param>
        /// <param name="className">The class name.</param>
        /// <returns>A minimal content of the <c>CoreProjectContext</c> script.</returns>
        public static string BuildCoreProjectContextScript(string namespaceName, string className)
        {
            bool hasNamespace = !string.IsNullOrEmpty(namespaceName);

            var builder = new IndentStringBuilder();
            builder.AppendLine("using AlchemyBow.Core;");
            builder.AppendLine("using System.Collections.Generic;");
            builder.AppendLine();

            if (hasNamespace)
            {
                builder.AppendLine($"namespace {namespaceName}");
                builder.AppendLine("{");
                builder.IndentLevel++;
            }

            builder.AppendLine($"public class {className} : CoreProjectContext");
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
