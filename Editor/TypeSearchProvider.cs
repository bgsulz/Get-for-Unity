#if UNITY_EDITOR
using System;

namespace Extra.Attributes
{
    public class TypeSearchProvider : GenericSearchProvider<Type>
    {
        protected override string StringRepresentation(Type value) => value.FullName;

        protected override string Name => "Types";
    }
}
#endif