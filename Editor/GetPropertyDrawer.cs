// ReSharper disable file CoVariantArrayConversion

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
        #region Helpers

        private GetAttribute GetAttribute => (GetAttribute) attribute;

        #endregion

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var listObjects = property.CandidateObjects(GetAttribute.GetterSource, fieldInfo);

            if (listObjects.Length <= 0)
            {
                GUI.enabled = false;
                EditorGUI.LabelField(position, label.GetterFormat(GetAttribute));
                
                var messagePosition = position.MessageRect();
                
                GUI.enabled = true;
                if (GetAttribute.GetterSource == GetterSource.Find) 
                    GUI.Label(messagePosition, "None found.");
                else if (GUI.Button(messagePosition, "None found. Add?")) 
                    property.SetRef(property.Target()?.gameObject.AddComponent(fieldInfo.FieldType));
                
                return;
            }

            var index = Mathf.Clamp(Array.IndexOf(listObjects, property.Ref()), 0, int.MaxValue);

            EditorGUI.BeginChangeCheck();

            var newIndex = EditorGUI.Popup(position.RightSide(), index, listObjects.ToFormattedNames().ToArray());

            if (EditorGUI.EndChangeCheck())
            {
                var oldRef = property.Ref();
                property.SetRef(listObjects[newIndex]);
                
                if (oldRef != property.Ref())
                {
                    Undo.RecordObject(property.serializedObject.targetObject, "Changed property value");
                    property.serializedObject.ApplyModifiedProperties();
                }
            }

            GUI.enabled = false;
            EditorGUI.ObjectField(position.LeftSide(), property, new GUIContent(label.GetterFormat(GetAttribute)));
        }
    }

    internal static class GetPropertyDrawerExtensions
    {
        internal static MonoBehaviour Target(this SerializedProperty property) 
            => property.serializedObject.targetObject as MonoBehaviour;
        
        internal static Object Ref(this SerializedProperty property)
            => property.objectReferenceValue;

        internal static Object SetRef(this SerializedProperty property, in Object value) 
            => property.objectReferenceValue = value;

        internal static string GetterFormat(this GUIContent label, in GetAttribute attribute)
            => $"{label.text} [src: {attribute.GetterSource.ToString()}]";
        
        internal static Rect LeftSide(this Rect position)
        {
            var res = position;
            res.width *= 0.8f;
            res.x = position.x;
            return res;
        }

        internal static Rect RightSide(this Rect position)
        {
            var res = position;
            res.width *= 0.2f;
            res.x = position.x + position.width * 0.8f;
            return res;
        }
        
        internal static Rect MessageRect(this Rect position)
        {
            var res = position;
            res.width = position.width * 0.6f;
            res.x = position.x + position.width * 0.4f;
            return res;
        }
        
        internal static IEnumerable<string> ToFormattedNames(this IEnumerable<Object> input)
        {
            var names = input.Select(item => item.name).ToArray();

            var indexTrackerDictionary = names
                .GroupBy(item => item)
                .Where(item => item.Count() > 1)
                .ToDictionary(item => item.Key, item => 0);

            foreach (var item in names)
            {
                if (!indexTrackerDictionary.TryGetValue(item, out var value)) // If not a duplicate...
                {
                    yield return item;
                    continue;
                }
                
                indexTrackerDictionary[item] = ++value; // We've now encountered this item {value} times.
                yield return $"{item} [{value - 1}]";
            }
        }

        internal static Object[] CandidateObjects(this SerializedProperty property, in GetterSource getterSource, in FieldInfo fieldInfo)
        {
            var mb = Target(property);
            
            return mb
                ? getterSource switch
                {
                    GetterSource.Object => mb.GetComponents(fieldInfo.FieldType),
                    GetterSource.Children => mb.GetComponentsInChildren(fieldInfo.FieldType, true),
                    GetterSource.Parent => mb.GetComponentsInParent(fieldInfo.FieldType, true),
                    GetterSource.Find => Object.FindObjectsOfType(fieldInfo.FieldType, true),
                    _ => null
                }
                : null;
        }
    }
}
