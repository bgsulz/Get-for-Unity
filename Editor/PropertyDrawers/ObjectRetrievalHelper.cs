#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Extra.Attributes
{
    public static class ObjectRetrievalHelper
    {
        public static Object[] GetCandidates(GetterSource getterSource, Object target, Type type)
        {
            var results = Enumerable.Empty<Object>();
            if (target is MonoBehaviour mb)
            {
                if (getterSource.HasFlag(GetterSource.Object)) 
                    results = results.Concat(mb.GetComponents(type));
                if (getterSource.HasFlag(GetterSource.Children))
                    results = results.Concat(mb.GetComponentsInChildren(type));
                if (getterSource.HasFlag(GetterSource.Parent)) 
                    results = results.Concat(mb.GetComponentsInParent(type));
            }
            if (getterSource.HasFlag(GetterSource.Find) && !type.IsInterface) 
                results = results.Concat(Object.FindObjectsOfType(type));
            if (getterSource.HasFlag(GetterSource.FindAssets)) 
                results = results.Concat(FindAllAssetsOfType(type));

            return results.Distinct().ToArray();
        }
        
        public static IEnumerable<Object> FindAllAssetsOfType(Type type)
        {
            var plain = AssetDatabase.FindAssets($"t:{type.Name}")
                .Select(x => AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(x)))
                .Where(x => x != null);
            
            if (!typeof(Component).IsAssignableFrom(type)) return plain;
            
            var comps = AssetDatabase.FindAssets($"t:{nameof(GameObject)}")
                .Select(x => AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(x)))
                .Select(x => x.GetComponent(type))
                .Where(x => x);
            return plain.Concat(comps).Distinct();
        }
        
        public static IEnumerable<T> FindAllAssetsOfType<T>() where T : Object
        {
            var plain = AssetDatabase.FindAssets($"t:{typeof(T).Name}")
                .Select(x => AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(x)))
                .Where(x => x != null)
                .Cast<T>();
            
            if (!typeof(Component).IsAssignableFrom(typeof(T))) return plain;
            
            var comps = AssetDatabase.FindAssets($"t:{nameof(GameObject)}")
                .Select(x => AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(x)))
                .Select(x => x.GetComponent<T>())
                .Where(x => x);
            
            return plain.Concat(comps).Distinct();
        }
    }
}
#endif