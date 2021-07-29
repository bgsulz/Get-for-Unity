using UnityEngine;

namespace Extra.Attributes
{
    public enum GetterSource
    {
        Object,
        Children,
        Parent,
        Find
    }

    public class GetAttribute : PropertyAttribute
    {
        public GetterSource GetterSource;

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
}
