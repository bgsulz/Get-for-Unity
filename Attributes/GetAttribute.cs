using UnityEngine;

namespace BGS.Attributes
{
    public enum GetterSource
    {
        Object,
        Children,
        Parent,
        Find,
        Add
    }

    public class GetAttribute : PropertyAttribute
    {
        public GetterSource GetterSource;

        public GetAttribute(GetterSource getterSource = GetterSource.Object) =>
            GetterSource = getterSource;
    }

    public class GetInChildrenAttribute : GetAttribute
    {
        public GetInChildrenAttribute() => GetterSource = GetterSource.Children;
    }

    public class GetInParentAttribute : GetAttribute
    {
        public GetInParentAttribute() => GetterSource = GetterSource.Parent;
    }

    public class FindAttribute : GetAttribute
    {
        public FindAttribute() => GetterSource = GetterSource.Find;
    }

    public class GetOrAddAttribute : GetAttribute
    {
        public GetOrAddAttribute() => GetterSource = GetterSource.Add;
    }
}