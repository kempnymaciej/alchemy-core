using UnityEditor;
using UnityEngine;

namespace AlchemyBow.Core.Editor.Wizards
{
    /// <summary>
    /// Provides the base class for an editor wizard window. 
    /// </summary>
    public abstract class WizardWindow : EditorWindow
    {
        private readonly ValidationMessageList validationMessageList = new ValidationMessageList();

        private Vector2 scrollPosition;

        /// <summary>
        /// Determines whether the wizard content is valid.
        /// </summary>
        /// <returns>
        /// <c>true</c> if there are no validation errors; Otherwise, <c>false</c>.
        /// </returns>
        protected bool IsValid => !validationMessageList.HasErrors;

        /// <summary>
        /// Override this method to alternate Awake behaviour. By default, <c>Validate()</c> is called.
        /// </summary>
        protected virtual void Awake()
        {
            Validate();
        }

        /// <summary>
        /// Override this method to alternate OnProjectChange behaviour. By default, <c>Validate()</c> is called.
        /// </summary>
        protected virtual void OnProjectChange()
        {
            Validate();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUI.BeginChangeCheck();

            OnValidableGUI();

            if (EditorGUI.EndChangeCheck())
            {
                Validate();
            }

            EditorGUI.BeginDisabledGroup(!IsValid);
            if (GUILayout.Button("Create"))
            {
                Create();
            }
            EditorGUI.EndDisabledGroup();

            foreach (var validationError in validationMessageList.Errors)
            {
                EditorGUILayout.HelpBox(validationError, MessageType.Error);
            }

            foreach (var validationWarning in validationMessageList.Warnings)
            {
                EditorGUILayout.HelpBox(validationWarning, MessageType.Warning);
            }

            foreach (var validationInfo in validationMessageList.Infos)
            {
                EditorGUILayout.HelpBox(validationInfo, MessageType.Info);
            }

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Override this method to draw validable controls inside the wizard.
        /// </summary>
        protected abstract void OnValidableGUI();

        /// <summary>
        /// Override this method to validate the wizard content.
        /// </summary>
        /// <param name="validationMessageList">A list of validation messages to append results to.</param>
        protected abstract void Validate(ValidationMessageList validationMessageList);

        /// <summary>
        /// Starts the wizard content validation.
        /// </summary>
        protected void Validate()
        {
            validationMessageList.Clear();
            Validate(validationMessageList);
        }

        /// <summary>
        /// Override this method to determine what happens when the create button is clicked.
        /// </summary>
        protected abstract void Create();
    }
}
