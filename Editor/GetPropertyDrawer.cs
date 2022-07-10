#if UNITY_EDITOR

using System;
using System.Linq;
using Extra.Editor.Properties;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Extra.Attributes
{
    [CustomPropertyDrawer(typeof(GetAttribute), true)]
    public partial class GetPropertyDrawer : PropertyDrawer
    {
        private bool _initialized;
        private Object[] _candidates;
        
        private GetAttribute _attribute;
        private GetAttribute Attribute => _attribute ??= (GetAttribute) attribute;

        private Type _fieldType;
        private Type FieldType => _fieldType ??= fieldInfo.FieldType;
        
        private Type _targetType;
        private Type TargetType => _targetType ??= FieldType.Unwrap();
        
        private MonoBehaviour _target;

        private bool IsFinder => Attribute.GetterSource is GetterSource.Find or GetterSource.FindAssets;
        private bool AbleToAddNew => !IsFinder && !FieldType.IsWrapper();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _target = property.serializedObject.targetObject as MonoBehaviour;

            if (ShouldCheckForUpdate())
                _candidates = CandidateObjects(property, TargetType, Attribute.GetterSource);

            var elementType = FieldType.UnwrapElement();
            var objectProperty = elementType.IsWrapper() ? property.FindPropertyRelative(IPropertyWrapper.PropertyName) : property;

            EditorGUI.BeginProperty(position, label, objectProperty);

            if (_candidates.Length <= 0)
                NoneFoundField(position, objectProperty, label);
            else
                CandidatesField(position, objectProperty, label, _candidates);

            EditorGUI.EndProperty();

            bool ShouldCheckForUpdate()
            {
                if (!_initialized) return true;
                if (_candidates == null) return true;
                if (Event.current.type != EventType.MouseDown) return false;
                return position.Contains(Event.current.mousePosition);
            }
        }

        private void CandidatesField(Rect position, SerializedProperty objectProperty, GUIContent label, Object[] candidates)
        {
            var leftPos = LeftPartOf(position);
            var rightPos = RightPartOf(position);

            var oldRef = objectProperty.objectReferenceValue;
            var index = Mathf.Max(Array.IndexOf(candidates, oldRef), 0);

            EditorGUI.BeginChangeCheck();

            var countedNames = CountedNames(candidates, AbleToAddNew).ToArray();
            var newIndex = EditorGUI.Popup(rightPos, index, countedNames);

            if (EditorGUI.EndChangeCheck() || !oldRef || !oldRef.GetType().IsAssignableFrom(TargetType))
            {
                objectProperty.objectReferenceValue = newIndex < candidates.Length ? candidates[newIndex] : AddedComponent();

                if (!oldRef || oldRef != objectProperty.objectReferenceValue)
                {
                    Undo.RecordObject(_target, "Changed property value");
                    objectProperty.serializedObject.ApplyModifiedProperties();
                }
            }

            using (new EditorGUI.DisabledScope(true)) 
                EditorGUI.ObjectField(leftPos, objectProperty, Formatted(label, Attribute));
        }

        private void NoneFoundField(Rect position, SerializedProperty objectProperty, GUIContent label)
        {
            var leftPos = LeftPartOf(position);
            var rightPos = RightPartOf(position);

            objectProperty.objectReferenceValue = null;

            using (new EditorGUI.DisabledScope(true)) 
                EditorGUI.ObjectField(leftPos, objectProperty, Formatted(label, Attribute));

            if (AbleToAddNew)
            {
                var button = GUI.Button(rightPos, "Add");
                if (button) objectProperty.objectReferenceValue = AddedComponent();
            }
            else
            {
                GUI.color = Color.red;
                GUI.Label(rightPos, "None found.");
            }
        }

        private Component AddedComponent() => _target!.gameObject.AddComponent(TargetType);

        private static Object[] CandidateObjects(SerializedProperty property, Type targetType, GetterSource getterSource)
        {
            var mb = property.serializedObject.targetObject;
            return mb
                ? getterSource switch
                {
                    // ReSharper disable scope CoVariantArrayConversion
                    GetterSource.Object => (mb as MonoBehaviour)?.GetComponents(targetType),
                    GetterSource.Children => (mb as MonoBehaviour)?.GetComponentsInChildren(targetType, true),
                    GetterSource.Parent => (mb as MonoBehaviour)?.GetComponentsInParent(targetType, true),
                    GetterSource.Find => Object.FindObjectsOfType(targetType, true),
                    GetterSource.FindAssets => GetAllAssetsOfType(targetType),
                    _ => null
                }
                : null;
        }

        private static Object[] GetAllAssetsOfType(Type type) =>
            AssetDatabase.FindAssets($"t:{type.Name}")
                .Select(x => AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(x)))
                .Where(x => x != null)
                .ToArray();
    }
}
#endif