using UnityEditor;
using UnityEngine;

namespace AlchemyBow.Core.Editor.Utilities
{
    internal static class CoreEditorStyles
    {
        private const float ToggleAdditionalSize = 5;
        private static GUIStyle leftToggleInternal;
        private static GUIStyle rightToggleInternal;

        public static GUIStyle LeftToggle {
            get {
                if (leftToggleInternal == null)
                {
                    leftToggleInternal = new GUIStyle(EditorStyles.miniButtonLeft);
                    leftToggleInternal.fixedHeight += ToggleAdditionalSize;
                }
                return leftToggleInternal;
            }
        }

        public static GUIStyle RightToggle {
            get {
                if (rightToggleInternal == null)
                {
                    rightToggleInternal = new GUIStyle(EditorStyles.miniButtonRight);
                    rightToggleInternal.fixedHeight += ToggleAdditionalSize;
                }
                return rightToggleInternal;
            }
        }
    } 
}
