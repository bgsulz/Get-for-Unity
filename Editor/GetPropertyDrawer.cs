#if UNITY_EDITOR

using System;
using System.Linq;
using Extra.Editor.Properties;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Extra.Attributes
{
    [CustomPropertyDrawer(typeof(GetAttributeBase), true)]
    public partial class GetPropertyDrawer : PropertyDrawer
    {
        private Object[] _candidates;
        private Object _target;
        private bool _ableToAddNew;
        
        private SerializedProperty _objectProperty;
        private Type _fieldType, _elementType, _targetType;
        
        private GetAttributeBase _attribute;
        private GetAttributeBase Attribute => _attribute ??= (GetAttributeBase) attribute;

        private bool IsFinder => Attribute.GetterSource is GetterSource.Find or GetterSource.FindAssets;
        private Component AddComponent => (_target as MonoBehaviour)?.gameObject.AddComponent(_targetType);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _target = property.serializedObject.targetObject;

            if (ShouldCheckForUpdate(position))
            {
                UpdateTypes();
                _candidates = Attribute.GetCandidates(_target, _targetType);
            }
            _objectProperty = _elementType.IsWrapper() ? property.FindPropertyRelative(IPropertyWrapper.PropertyName) : property;

            using (new EditorGUI.PropertyScope(position, label, _objectProperty))
            {
                if (_candidates!.Length <= 0)
                    NoneFoundField(position, _objectProperty, label);
                else
                    CandidatesField(position, _objectProperty, label, _candidates);
            }
            
            property.serializedObject.Update();
        }
        
        private bool ShouldCheckForUpdate(Rect position)
        {
            if (_candidates == null) return true;
            if (Event.current.type != EventType.MouseDown) return false;
            return position.Contains(Event.current.mousePosition);
        }

        private void UpdateTypes()
        {
            _fieldType = fieldInfo.FieldType;
            _elementType = _fieldType.UnwrapElement();
            _targetType = _fieldType.Unwrap();
            _ableToAddNew = _target && _target is MonoBehaviour && !IsFinder && !_elementType.IsWrapper();
        }

        private void CandidatesField(Rect position, SerializedProperty objectProperty, GUIContent label, Object[] candidates)
        {
            var leftPos = LeftPartOf(position);
            var rightPos = RightPartOf(position);

            var oldRef = objectProperty.objectReferenceValue;
            var index = Mathf.Max(Array.IndexOf(candidates, oldRef), 0);

            EditorGUI.BeginChangeCheck();

            var countedNames = CountedNames(candidates, _ableToAddNew).ToArray();
            var newIndex = EditorGUI.Popup(rightPos, index, countedNames);

            if (EditorGUI.EndChangeCheck() || !oldRef || !oldRef.GetType().IsAssignableFrom(_targetType))
            {
                objectProperty.objectReferenceValue = newIndex < candidates.Length ? candidates[newIndex] : AddComponent;

                if (!oldRef || oldRef != objectProperty.objectReferenceValue)
                {
                    Undo.RecordObject(_target, "Changed property value");
                    objectProperty.serializedObject.ApplyModifiedProperties();
                }
            }

            using (new EditorGUI.DisabledScope(true)) 
                EditorGUI.PropertyField(leftPos, objectProperty, Formatted(label, Attribute));
        }

        private void NoneFoundField(Rect position, SerializedProperty objectProperty, GUIContent label)
        {
            var leftPos = LeftPartOf(position);
            var rightPos = RightPartOf(position);

            objectProperty.objectReferenceValue = null;

            using (new EditorGUI.DisabledScope(true)) 
                EditorGUI.PropertyField(leftPos, objectProperty, Formatted(label, Attribute));

            if (_ableToAddNew)
            {
                if (GUI.Button(rightPos, "Add")) 
                    objectProperty.objectReferenceValue = AddComponent;
            }
            else
            {
                var wasColor = GUI.color;
                GUI.color = Color.red;
                EditorGUI.LabelField(rightPos, "None found.");
                GUI.color = wasColor;
            }
        }
    }
}
#endif