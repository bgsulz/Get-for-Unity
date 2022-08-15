#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Extra.Attributes
{
    public static class ScriptableHelper
    {
        public static void CreateAssetAndSet(Type type, SerializedProperty property)
        {
            const string defaultPath = "Assets";
            
            var currentReference = property.objectReferenceValue;
            var rootPath = !currentReference ? defaultPath : Path.GetDirectoryName(AssetDatabase.GetAssetPath(currentReference)) ?? defaultPath;
            
            var savePath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(rootPath, $"New {type.Name}.asset"));
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance(type), savePath);
            property.objectReferenceValue = AssetDatabase.LoadAssetAtPath<ScriptableObject>(savePath);
            
            Undo.RecordObject(property.serializedObject.targetObject, $"Created new {type.Name}");
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        }

        public static IEnumerable<Type> GetDerivedTypes(Type baseType) =>
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(domainAssembly => domainAssembly.GetTypes())
                .Where(type => baseType.IsAssignableFrom(type) && !type.IsAbstract && !type.IsGenericType);
    }
}
#endif