#define EXTRA_GET

using System;
using UnityEngine;

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

        public GetAttribute(GetterSource getterSource = GetterSource.Object) => GetterSource = getterSource;
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