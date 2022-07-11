#define EXTRA_GET

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Extra.Attributes
{
    [Flags]
    public enum GetterSource
    {
        Object = 1,
        Children = 2,
        Parent = 4,
        Find = 8,
        FindAssets = 16
    }

    public class GetAttribute : PropertyAttribute
    {
        public GetterSource GetterSource { get; }

        public Object[] GetCandidates(Object target, Type type)
        {
            var results = Enumerable.Empty<Object>();
            if (target is MonoBehaviour mb)
            {
                if (GetterSource.HasFlag(GetterSource.Object)) 
                    results = results.Concat(mb.GetComponents(type));
                if (GetterSource.HasFlag(GetterSource.Children))
                    results = results.Concat(mb.GetComponentsInChildren(type));
                if (GetterSource.HasFlag(GetterSource.Parent)) 
                    results = results.Concat(mb.GetComponentsInParent(type));
            }
            if (GetterSource.HasFlag(GetterSource.Find) && !type.IsInterface) 
                results = results.Concat(Object.FindObjectsOfType(type));
            if (GetterSource.HasFlag(GetterSource.FindAssets)) 
                results = results.Concat(FindAllAssetsOfType(type));

            return results.Distinct().ToArray();
        }

        public GetAttribute(GetterSource getterSource = GetterSource.Object) => GetterSource = getterSource;

        private static IEnumerable<Object> FindAllAssetsOfType(Type type) =>
            AssetDatabase.FindAssets($"t:{type.Name}")
                .Select(x => AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(x)))
                .Where(x => x != null);
    }
    
    public class GetInChildrenAttribute : GetAttribute
    {
        public GetInChildrenAttribute() : base(GetterSource.Children) { }
    }

    public class GetInParentAttribute : GetAttribute
    {
        public GetInParentAttribute() : base(GetterSource.Parent) { }
    }
    
    public class GetInChildrenAndParentAttribute : GetAttribute
    {
        public GetInChildrenAndParentAttribute() : base(GetterSource.Children | GetterSource.Parent) { }
    }

    public class FindAttribute : GetAttribute
    {
        public FindAttribute() : base(GetterSource.Find) { }
    }

    public class FindAssetsAttribute : GetAttribute
    {
        public FindAssetsAttribute() : base(GetterSource.FindAssets) { }
    }
    
    public class FindAllAttribute : GetAttribute
    {
        public FindAllAttribute() : base(GetterSource.Children | GetterSource.Parent | GetterSource.FindAssets) { }
    }
}