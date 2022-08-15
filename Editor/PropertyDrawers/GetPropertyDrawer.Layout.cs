using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace Extra.Attributes
{
    public partial class GetPropertyDrawer
    {
        private static readonly float ButtonWidth = EditorGUIUtility.singleLineHeight;

        private static Rect Shorten(Rect position, out Rect rest, float amount = -1)
        {
            if (amount < 0) amount = ButtonWidth;
            position.xMax -= amount;

            rest = position;
            rest.xMin = position.xMax;
            rest.width = amount;

            return position;
        }

        private static GUIStyle _addButtonStyle;

        private static bool AddNewButton(Rect position) => IconButton(position, "d_Toolbar Plus More");
        private static bool DropdownButton(Rect position) => IconButton(position, "d_icon dropdown@2x", 12f);

        private static bool IconButton(Rect position, string iconContent, float iconSize = 0)
        {
            EditorGUIUtility.SetIconSize(Vector2.one * iconSize);
            return GUI.Button(position, EditorGUIUtility.IconContent(iconContent),
                _addButtonStyle ??= new GUIStyle(GUI.skin.button)
                {
                    padding = new RectOffset(0, 0, 0, 0),
                });
        }
    }
}
#endif