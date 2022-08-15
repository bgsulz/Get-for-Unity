using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
namespace Extra.Attributes
{
    public static class PopupHelper
    {
        private static void Open<T, TProvider>(ref TProvider provider, T[] listItems, Action<T> selected) 
            where TProvider : GenericSearchProvider<T>
        {
            if (!provider) provider = ScriptableObject.CreateInstance<TProvider>();

            provider.Construct(listItems, selected);
            provider.Open();
        }

        public static void PopupSelectObject(this ObjectSearchProvider provider, SerializedProperty property, Object[] candidates)
        {
            Open(ref provider, candidates, x =>
            {
                property.objectReferenceValue = x;
                RefreshProperty(property);
            });
        }
        
        public static void PopupAddNewOfType(this TypeSearchProvider provider, SerializedProperty property,
            Type targetType, NewableType newableType)
        {
            PopupAddNewOfType(targetType, provider, x =>
            {
                AddNewAndSet(newableType, x, property);
                RefreshProperty(property);
            });
            
            static void AddNewAndSet(NewableType newableType, Type targetType, SerializedProperty property)
            {
                switch (newableType)
                {
                    case NewableType.Component:
                        var mono = property.serializedObject.targetObject as MonoBehaviour;
                        property.objectReferenceValue = mono!.gameObject.AddComponent(targetType);
                        break;
                    case NewableType.Asset:
                        ScriptableHelper.CreateAssetAndSet(targetType, property);
                        break;
                }
            }
        }
        
        private static void PopupAddNewOfType(Type targetType, TypeSearchProvider provider, Action<Type> onSelected)
        {
            var derivedTypes = ScriptableHelper.GetDerivedTypes(targetType).OrderBy(x => x.Name).ToArray();

            switch (derivedTypes.Length)
            {
                case <= 0:
                    Debug.LogWarning($"Unable to find valid type deriving from {targetType.Name}.");
                    break;
                case 1:
                    onSelected(derivedTypes.First());
                    break;
                default:
                    Open(ref provider, derivedTypes.ToArray(), onSelected);
                    break;
            }
        }

        private static void RefreshProperty(SerializedProperty property)
        {
            Undo.RecordObject(property.serializedObject.targetObject, "Changed property value");
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        }

        public enum NewableType
        {
            Component = 1,
            Asset = 2
        }
    }
}
#endif