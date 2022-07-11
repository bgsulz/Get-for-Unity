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

    public abstract class GetAttributeBase : PropertyAttribute
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

        protected GetAttributeBase(GetterSource getterSource) => GetterSource = getterSource;

        protected static IEnumerable<Object> FindAllAssetsOfType(Type type) =>
            AssetDatabase.FindAssets($"t:{type.Name}")
                .Select(x => AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(x)))
                .Where(x => x != null);
    }
    
    public class GetAttribute : GetAttributeBase
    {
        public GetAttribute() : base(GetterSource.Object) { }
    }

    public class GetInChildrenAttribute : GetAttributeBase
    {
        public GetInChildrenAttribute() : base(GetterSource.Children) { }
    }

    public class GetInParentAttribute : GetAttributeBase
    {
        public GetInParentAttribute() : base(GetterSource.Parent) { }
    }
    
    public class GetInChildrenAndParentAttribute : GetAttributeBase
    {
        public GetInChildrenAndParentAttribute() : base(GetterSource.Children | GetterSource.Parent) { }
    }

    public class FindAttribute : GetAttributeBase
    {
        public FindAttribute() : base(GetterSource.Find) { }
    }

    public class FindAssetsAttribute : GetAttributeBase
    {
        public FindAssetsAttribute() : base(GetterSource.FindAssets) { }
    }
    
    public class FindAllAttribute : GetAttributeBase
    {
        public FindAllAttribute() : base(GetterSource.Children | GetterSource.Parent | GetterSource.FindAssets) { }
    }
}