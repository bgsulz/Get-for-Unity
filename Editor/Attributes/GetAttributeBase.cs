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

    public class GetAttributeBase : PropertyAttribute
    {
        public GetterSource GetterSource { get; }
        public bool AutoFill { get; }

        public GetAttributeBase(GetterSource getterSource, bool autoFill = true) 
            => (GetterSource, AutoFill) = (getterSource, autoFill);
    }
    
    public class GetAttribute : GetAttributeBase
    {
        public GetAttribute(bool autoFill = true) : base(GetterSource.Object, autoFill) { }
    }
    
    public class GetInChildrenAttribute : GetAttributeBase
    {
        public GetInChildrenAttribute(bool autoFill = true) : base(GetterSource.Children, autoFill) { }
    }

    public class GetInParentAttribute : GetAttributeBase
    {
        public GetInParentAttribute(bool autoFill = true) : base(GetterSource.Parent, autoFill) { }
    }
    
    public class GetInChildrenAndParentAttribute : GetAttributeBase
    {
        public GetInChildrenAndParentAttribute(bool autoFill = true) : base(GetterSource.Children | GetterSource.Parent, autoFill) { }
    }

    public class FindAttribute : GetAttributeBase
    {
        public FindAttribute(bool autoFill = true) : base(GetterSource.Find, autoFill) { }
    }

    public class FindAssetsAttribute : GetAttributeBase
    {
        public FindAssetsAttribute(bool autoFill = true) : base(GetterSource.FindAssets, autoFill) { }
    }
    
    public class FindAllAttribute : GetAttributeBase
    {
        public FindAllAttribute(bool autoFill = true) : base(GetterSource.Children | GetterSource.Parent | GetterSource.FindAssets, autoFill) { }
    }
}