#define EXTRA_GET

using UnityEngine;

namespace Extra.Attributes
{
    public enum GetterSource
    {
        Object,
        Children,
        Parent,
        Find,
        FindAssets
    }

    public class GetAttribute : PropertyAttribute
    {
        public GetterSource GetterSource { get; }

        public GetAttribute(GetterSource getterSource = GetterSource.Object) =>
            GetterSource = getterSource;
    }

    public class GetInChildrenAttribute : GetAttribute
    {
        public GetInChildrenAttribute() : base(GetterSource.Children) { }
    }

    public class GetInParentAttribute : GetAttribute
    {
        public GetInParentAttribute() : base(GetterSource.Parent) { }
    }

    public class FindAttribute : GetAttribute
    {
        public FindAttribute() : base(GetterSource.Find) { }
    }

    public class FindAssetsAttribute : GetAttribute
    {
        public FindAssetsAttribute() : base(GetterSource.FindAssets) { }
    }
}