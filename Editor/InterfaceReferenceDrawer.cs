# if UNITY_EDITOR
using System;
using System.Linq;
using Extra.Attributes;
using UnityEditor;
using UnityEngine;

namespace Extra.Editor.Properties
{
    [CustomPropertyDrawer(typeof(InterfaceReference<>))]
    public class InterfaceReferenceDrawer : PropertyDrawer
    {
        private Type _interfaceType;
        private bool _implementsInterface;

        private PropertyDrawer _propertyDrawer;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _interfaceType ??= fieldInfo.FieldType.GetGenericArguments()[0];
            if (!_interfaceType.IsInterface)
            {
                EditorGUI.LabelField(position, $"{_interfaceType} is not an interface.");
                return;
            }

            var referenceProp = property.FindPropertyRelative("reference");

            var oldColor = GUI.color;
            if (!_implementsInterface) GUI.color = Color.red;

            var changeCheck = new EditorGUI.ChangeCheckScope();
            using (changeCheck) EditorGUI.PropertyField(position, referenceProp, label);

            GUI.color = oldColor;

            if (!changeCheck.changed) return;

            var reference = referenceProp.objectReferenceValue;
            if (!reference) return;

            var type = reference.GetType();
            _implementsInterface = _interfaceType.IsAssignableFrom(type);
        }
    }
}
#endif