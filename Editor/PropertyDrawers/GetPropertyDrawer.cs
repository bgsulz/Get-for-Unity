﻿#if UNITY_EDITOR
using System;
using System.Linq;
using Extra.Editor.Properties;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Extra.Attributes
{
    [CustomPropertyDrawer(typeof(GetFromAttribute), true)]
    public partial class GetPropertyDrawer : PropertyDrawer
    {
        private Object[] _candidates;
        private Object _inspectedObject;
        private NewableType _newableType;
        
        private SerializedProperty _objectProperty;
        private Type _fieldType, _elementType, _targetType;
        
        private ObjectSearchProvider _objectSearchProvider;
        private TypeSearchProvider _typeSearchProvider;
        
        private GetFromAttribute _attribute;
        private GetFromAttribute Attribute => _attribute ??= (GetFromAttribute) attribute;

        private bool IsFinder => Attribute.GetterSource.HasFlag(GetterSource.Find) || Attribute.GetterSource.HasFlag(GetterSource.FindAssets);
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _inspectedObject = property.serializedObject.targetObject;

            if (ShouldCheckForUpdate(position))
            {
                _fieldType = fieldInfo.FieldType;
                _elementType = _fieldType.UnwrapElement();
                _targetType = _fieldType.Unwrap();
                _newableType = CalculateNewableType();

                _candidates = ObjectRetrievalHelper.GetCandidates(Attribute.GetterSource, _inspectedObject, _targetType);
            }
            _objectProperty = _elementType.IsWrapper() ? property.FindPropertyRelative(IPropertyWrapper.PropertyName) : property;

            using (new EditorGUI.PropertyScope(position, label, _objectProperty))
            {
                switch (_candidates.Length)
                {
                    case <= 0: NoneFoundField(position, _objectProperty, label); break;
                    default: CandidatesField(position, _objectProperty, label, _candidates); break;
                }
            }
            
            // ------------------------------
            
            bool ShouldCheckForUpdate(Rect propertyRect)
            {
                if (_candidates == null) return true;
                if (Event.current.type != EventType.MouseDown) return false;
                return propertyRect.Contains(Event.current.mousePosition);
            }
            
            NewableType CalculateNewableType()
            {
                if (_inspectedObject && _inspectedObject is MonoBehaviour && !IsFinder && !_elementType.IsWrapper())
                    return NewableType.Component;
                if (Attribute.GetterSource.HasFlag(GetterSource.FindAssets) && typeof(ScriptableObject).IsAssignableFrom(_targetType))
                    return NewableType.Asset;
                return NewableType.None;
            }
        }

        private void CandidatesField(Rect position, SerializedProperty objectProperty, GUIContent label, Object[] candidates)
        {
            var leftPos = position;
            if (_newableType != 0)
            {
                leftPos = Shorten(leftPos, out var buttonPos);
                if (AddNewButton(buttonPos))
                    _typeSearchProvider.PopupAddNewOfType(objectProperty, _targetType, _newableType);
            }
            leftPos = Shorten(leftPos, out var rightPos);

            if (DropdownButton(rightPos))
                _objectSearchProvider.PopupSelectObject(objectProperty, candidates, true);

            if (Attribute.AutoFill && !Application.isPlaying && !objectProperty.objectReferenceValue)
                objectProperty.objectReferenceValue = candidates.FirstOrDefault();

            using (new EditorGUI.DisabledScope(Attribute.AutoFill))
                EditorGUI.PropertyField(leftPos, objectProperty, label);
        }

        private void NoneFoundField(Rect position, SerializedProperty objectProperty, GUIContent label)
        {
            var leftPos = position;
            
            leftPos = Shorten(leftPos, out var buttonPos);
            
            if (_newableType != NewableType.None)
            {
                if (AddNewButton(buttonPos))
                    _typeSearchProvider.PopupAddNewOfType(objectProperty, _targetType, _newableType);
            }
            else
            {
                objectProperty.objectReferenceValue = null;
                ErrorIconLabel(buttonPos);
            }

            using (new EditorGUI.DisabledScope(true)) 
                EditorGUI.PropertyField(leftPos, objectProperty, label);
        }
    }
}
#endif