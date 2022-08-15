using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Extra.Editor.Properties
{
    [Serializable]
    public class InterfaceReference<T> : IPropertyWrapper where T : class
    {
        [SerializeField] private Object reference;

        private T _asInterface;
        public T AsInterface => _asInterface ??= reference as T;

        public static implicit operator T(InterfaceReference<T> self) => self.AsInterface;
    }
}