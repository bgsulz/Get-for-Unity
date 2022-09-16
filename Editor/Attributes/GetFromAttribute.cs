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

    public class GetFromAttribute : PropertyAttribute
    {
        public GetterSource GetterSource { get; }
        public bool AutoFill { get; }

        public GetFromAttribute(GetterSource getterSource, bool autoFill = true) 
            => (GetterSource, AutoFill) = (getterSource, autoFill);
    }
    
    public class GetAttribute : GetFromAttribute
    {
        public GetAttribute(bool autoFill = true) : base(GetterSource.Object, autoFill) { }
    }
    
    public class GetInChildrenAttribute : GetFromAttribute
    {
        public GetInChildrenAttribute(bool autoFill = true) : base(GetterSource.Children, autoFill) { }
    }

    public class GetInParentAttribute : GetFromAttribute
    {
        public GetInParentAttribute(bool autoFill = true) : base(GetterSource.Parent, autoFill) { }
    }
    
    public class GetInChildrenAndParentAttribute : GetFromAttribute
    {
        public GetInChildrenAndParentAttribute(bool autoFill = true) : base(GetterSource.Children | GetterSource.Parent, autoFill) { }
    }

    public class FindAttribute : GetFromAttribute
    {
        public FindAttribute(bool autoFill = true) : base(GetterSource.Find, autoFill) { }
    }

    public class FindAssetsAttribute : GetFromAttribute
    {
        public FindAssetsAttribute(bool autoFill = true) : base(GetterSource.FindAssets, autoFill) { }
    }
    
    public class FindAllAttribute : GetFromAttribute
    {
        public FindAllAttribute(bool autoFill = true) : base(GetterSource.Children | GetterSource.Parent | GetterSource.FindAssets, autoFill) { }
    }
}