#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Extra.Attributes
{
    [CustomPropertyDrawer(typeof(GetAttribute), true)]
    public class GetPropertyDrawer : PropertyDrawer
    {
        private GetAttribute _attribute;
        private GetAttribute Attribute => _attribute ??= (GetAttribute) attribute;
        private MonoBehaviour _target;

        private const float ButtonWidth = 0.2f, FieldWidth = 0.8f;

        private static readonly GetterSource[] Finders = { GetterSource.Find, GetterSource.FindPrefab };
        private bool IsFinder => Finders.Contains(Attribute.GetterSource);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _target = property.serializedObject.targetObject as MonoBehaviour;

            var candidates = CandidateObjects(property, Attribute.GetterSource, fieldInfo);

            EditorGUI.BeginProperty(position, label, property);

            if (candidates.Length <= 0)
                NoneFoundField(position, property, label);
            else
                CandidatesField(position, property, label, candidates);

            EditorGUI.EndProperty();
        }

        private void CandidatesField(Rect position, SerializedProperty property, GUIContent label, Object[] candidates)
        {
            var leftPos = LeftPos(position);
            var rightPos = RightPos(position);

            var index = Mathf.Max(Array.IndexOf(candidates, property.objectReferenceValue), 0);

            EditorGUI.BeginChangeCheck();

            var countedNames = CountedNames(candidates, includeAddNew: !IsFinder).ToArray();
            var newIndex = EditorGUI.Popup(rightPos, index, countedNames);

            var oldRef = property.objectReferenceValue;

            if (EditorGUI.EndChangeCheck() || !oldRef || !UnwrapElementType(fieldInfo.FieldType).IsInstanceOfType(oldRef))
            {
                property.objectReferenceValue = newIndex < candidates.Length ? candidates[newIndex] : AddedComponent();

                if (!oldRef || oldRef != property.objectReferenceValue)
                {
                    Undo.RecordObject(_target, "Changed property value");
                    property.serializedObject.ApplyModifiedProperties();
                }
            }

            GUI.enabled = false;
            EditorGUI.ObjectField(leftPos, property, Formatted(label, Attribute));
        }

        private void NoneFoundField(Rect position, SerializedProperty property, GUIContent label)
        {
            var leftPos = LeftPos(position);
            var rightPos = RightPos(position);

            property.objectReferenceValue = null;

            GUI.enabled = false;
            EditorGUI.ObjectField(leftPos, property, Formatted(label, Attribute));
            GUI.enabled = true;

            if (IsFinder)
            {
                GUI.color = Color.red;
                GUI.Label(rightPos, "None found.");
            }
            else
            {
                var button = GUI.Button(rightPos, "Add");
                if (button) property.objectReferenceValue = AddedComponent();
            }
        }

        private Component AddedComponent() => _target!.gameObject.AddComponent(UnwrapElementType(fieldInfo.FieldType));

        private static Rect RightPos(Rect position)
        {
            position.x += position.width * (1 - ButtonWidth);
            position.width *= ButtonWidth;
            return position;
        }

        private static Rect LeftPos(Rect position)
        {
            position.width *= FieldWidth;
            return position;
        }

        private static GUIContent Formatted(GUIContent label, in GetAttribute attribute)
        {
            label.text = $"{label.text} [src: {attribute.GetterSource.ToString()}]";
            return label;
        }

        private static Object[] CandidateObjects(SerializedProperty property, in GetterSource getterSource, in FieldInfo fieldInfo)
        {
            var mb = property.serializedObject.targetObject as MonoBehaviour;
            var fieldType = UnwrapElementType(fieldInfo.FieldType);

            return mb
                ? getterSource switch
                {
                    GetterSource.Object => mb.GetComponents(fieldType) as Object[],
                    GetterSource.Children => mb.GetComponentsInChildren(fieldType, true) as Object[],
                    GetterSource.Parent => mb.GetComponentsInParent(fieldType, true) as Object[],
                    GetterSource.Find => Object.FindObjectsOfType(fieldType, true),
                    GetterSource.FindPrefab => Resources.FindObjectsOfTypeAll(fieldType),
                    _ => null
                }
                : null;
        }

        private static IEnumerable<string> CountedNames(Object[] input, bool includeAddNew = false)
        {
            var res = new List<string>(input.Length + 1);
            var names = input.Select(item => item.name).ToArray();

            var indexTrackerDictionary = names
                .GroupBy(item => item)
                .Where(item => item.Count() > 1)
                .ToDictionary(item => item.Key, _ => 0);

            foreach (var item in names)
            {
                if (!indexTrackerDictionary.TryGetValue(item, out var value)) // If not a duplicate...
                {
                    res.Add(item); // We don't care.
                    continue;
                }

                indexTrackerDictionary[item] = ++value; // We've now encountered this item {value} times.
                res.Add($"{item} [{value - 1}]");
            }

            if (includeAddNew) res.Add("(Add new)");
            return res;
        }

        private static Type UnwrapElementType(Type type) => type.IsArray ? UnwrapElementType(type.GetElementType()) : type;
    }
}
#endif