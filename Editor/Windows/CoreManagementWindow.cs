using AlchemyBow.Core.Editor.Utilities;
using AlchemyBow.Core.Editor.Windows.CoreManagementWindowElements;
using UnityEditor;
using UnityEngine;

namespace AlchemyBow.Core.Editor.Windows
{
    /// <summary>
    /// Provides a window to visually manage the main elements of AlchemyBow.Core.
    /// </summary>
    public sealed class CoreManagementWindow : EditorWindow
    {
        private CoreProjectContextsManager projectContextsManager;
        private CoreControllersManager controllersManager;

        private Vector2 scrollPosition;
        private bool showControllers;

        /// <summary>
        /// Gets or creates the core management window.
        /// </summary>
        [MenuItem("Window/AlchemyBow/Core/Core Management")]
        public static void CreateWindow()
        {
            var window = EditorWindow.GetWindow<CoreManagementWindow>("Core Management");
            window.minSize = new Vector2(380, 160);
        }

        private void OnEnable()
        {
            projectContextsManager = CoreProjectContextsManager.Create();
            controllersManager = CoreControllersManager.Create();
        }

        private void OnProjectChange()
        {
            projectContextsManager = CoreProjectContextsManager.Create();
            controllersManager = CoreControllersManager.Create();
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUILayout.BeginHorizontal();

            if(!showControllers != GUILayout.Toggle(!showControllers, "Core Project Contexts", CoreEditorStyles.LeftToggle))
            {
                showControllers = !showControllers;
            }
            if (showControllers != GUILayout.Toggle(showControllers, "Core Controllers", CoreEditorStyles.RightToggle))
            {
                showControllers = !showControllers;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(6);

            if (showControllers)
            {
                controllersManager.OnGUI();
            }
            else
            {
                projectContextsManager.OnGUI();
            }

            EditorGUILayout.EndScrollView();
        }
    } 
}
