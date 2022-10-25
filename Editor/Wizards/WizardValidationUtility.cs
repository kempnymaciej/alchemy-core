using AlchemyBow.Core.Editor.Utilities;
using UnityEditor;

namespace AlchemyBow.Core.Editor.Wizards
{
    /// <summary>
    /// Provides a set of validation functions for the AlchemyBow.Core editor wizards.
    /// </summary>
    public static class WizardValidationUtility
    {
        /// <summary>
        /// Validates the specified folder and adds the results to the list.
        /// </summary>
        /// <param name="folder">The specified folder.</param>
        /// <param name="validationMessageList">The message list.</param>
        public static void ValidateFolder(string folder, ValidationMessageList validationMessageList)
        {
            if (string.IsNullOrWhiteSpace(folder) || !AssetDatabase.IsValidFolder(folder))
            {
                validationMessageList.AddError($"'{folder}' is not a valid assets folder.");
            }
        }

        /// <summary>
        /// Validates the specified class name and adds the results to the list.
        /// </summary>
        /// <param name="className">The class name.</param>
        /// <param name="validationMessageList">The message list.</param>
        public static void ValidateClassName(string className, ValidationMessageList validationMessageList)
        {
            if (string.IsNullOrEmpty(className))
            {
                validationMessageList.AddError($"Enter the class name.");
            }
            else if (!CoreEditorUtility.IsValidClassName(className))
            {
                validationMessageList.AddWarning($"'{className}' may be an invalid class name.");
            }
        }

        /// <summary>
        /// Validates the specified namespace name and adds the results to the list.
        /// </summary>
        /// <param name="namespaceName">The namespace name.</param>
        /// <param name="validationMessageList">The message list.</param>
        public static void ValidateNamespaceName(string namespaceName, ValidationMessageList validationMessageList)
        {
            if (string.IsNullOrEmpty(namespaceName))
            {
                validationMessageList.AddInfo($"Consider entering the namespace name.");
            }
            else if (!CoreEditorUtility.IsValidNamespaceName(namespaceName))
            {
                validationMessageList.AddWarning($"'{namespaceName}' may be an invalid namespace name.");
            }
        }
    } 
}
