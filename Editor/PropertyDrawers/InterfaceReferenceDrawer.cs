# if UNITY_EDITOR
using System;
using Extra.Attributes;
using UnityEditor;
using UnityEngine;

namespace Extra.Editor.Properties
{
    [CustomPropertyDrawer(typeof(InterfaceReference<>))]
    public class InterfaceReferenceDrawer : PropertyDrawer
    {
        private Type _interfaceType;
        private PropertyDrawer _propertyDrawer;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _interfaceType ??= fieldInfo.FieldType.UnwrapElement().GetGenericArguments()[0];
            if (!_interfaceType.IsInterface)
            {
                EditorGUI.LabelField(position, $"{_interfaceType} is not an interface.");
                return;
            }

            var referenceProp = property.FindPropertyRelative("reference");

            var wasColor = GUI.color;
            if (!_interfaceType.IsInstanceOfType(referenceProp.objectReferenceValue)) GUI.color = Color.red;
            
            var changeCheck = new EditorGUI.ChangeCheckScope();
            using (changeCheck) EditorGUI.PropertyField(position, referenceProp, label);

            GUI.color = wasColor;
        }
    }
}
#endif