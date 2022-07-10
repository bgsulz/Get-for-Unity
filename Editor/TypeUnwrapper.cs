#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Extra.Editor.Properties;

namespace Extra.Attributes
{
    internal static class TypeUnwrapper
    {
        public static bool IsArray(this Type type) => type.IsArray;
        public static bool IsList(this Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        public static bool IsWrapper(this Type type) => type.GetInterface(nameof(IPropertyWrapper)) != null;
        
        public static Type Unwrap(this Type type)
        {
            if (type.IsWrapper()) return type.WrappedType().Unwrap();
            if (type.IsArray()) return type.ArrayElementType().Unwrap();
            if (type.IsList()) return type.ListElementType().Unwrap();
            return type;
        }

        public static Type UnwrapElement(this Type type)
        {
            if (type.IsArray()) return type.ArrayElementType().UnwrapElement();
            if (type.IsList()) return type.ListElementType().UnwrapElement();
            return type;
        }

        private static Type ArrayElementType(this Type type) => type.IsArray ? type.GetElementType() : type;
        private static Type ListElementType(this Type type) => type.IsList() ? type.GetGenericArguments()[0] : type;
        private static Type WrappedType(this Type type) => type.IsWrapper() ? type.GetGenericArguments()[0] : type;
    }
}
#endif