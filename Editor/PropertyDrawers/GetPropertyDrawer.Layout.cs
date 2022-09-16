using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace Extra.Attributes
{
    public partial class GetPropertyDrawer
    {
        private static readonly float ButtonWidth = EditorGUIUtility.singleLineHeight;
        
        private static GUIStyle _defaultIconButtonStyle;
        private static GUIStyle DefaultIconButtonStyle => _defaultIconButtonStyle ??= new GUIStyle(EditorStyles.iconButton)
        {
            padding = new RectOffset(0, 0, 0, 0),
            alignment = TextAnchor.MiddleCenter
        };

        private static Rect Shorten(Rect position, out Rect rest, float amount = -1)
        {
            if (amount < 0) amount = ButtonWidth;
            position.xMax -= amount;

            rest = position;
            rest.xMin = position.xMax;
            rest.width = amount;

            return position;
        }
        
        private static bool AddNewButton(Rect position) => IconButton(position, "d_Toolbar Plus More");
        private static bool DropdownButton(Rect position) => IconButton(position, "d_icon dropdown@2x", 12f);

        private static GUIContent _errorIconLabelContent;
        private static void ErrorIconLabel(Rect position) 
            => EditorGUI.LabelField(position, _errorIconLabelContent ??= new GUIContent(
                    EditorGUIUtility.IconContent("console.erroricon.inactive.sml@2x").image, 
                    "No search results found!"));

        private static bool IconButton(Rect position, string iconContent, float iconSize = 0, GUIStyle style = null)
        {
            EditorGUIUtility.SetIconSize(Vector2.one * iconSize);
            return GUI.Button(position, EditorGUIUtility.IconContent(iconContent), style ?? DefaultIconButtonStyle);
        }
    }
}
#endif