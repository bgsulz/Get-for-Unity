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
        private static void Open<T, TProvider>(ref TProvider provider, T[] listItems, Action<T> selected, bool flatten = false) 
            where TProvider : GenericSearchProvider<T>
        {
            if (!provider) provider = ScriptableObject.CreateInstance<TProvider>();

            provider.Construct(listItems, selected, flatten);
            provider.Open();
        }

        public static void PopupSelectObject(this ObjectSearchProvider provider, SerializedProperty property, Object[] candidates, bool flatten = false)
        {
            Open(ref provider, candidates, x =>
            {
                property.objectReferenceValue = x;
                RefreshProperty(property);
            }, flatten);
        }
        
        public static void PopupAddNewOfType(this TypeSearchProvider provider, SerializedProperty property,
            Type targetType, NewableType newableType, bool flatten = false)
        {
            PopupAddNewOfType(targetType, provider, x =>
            {
                AddNewAndSet(newableType, x, property);
                RefreshProperty(property);
            }, flatten);
            
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
        
        private static void PopupAddNewOfType(Type targetType, TypeSearchProvider provider, Action<Type> onSelected, bool flatten = false)
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
                    Open(ref provider, derivedTypes.ToArray(), onSelected, flatten);
                    break;
            }
        }

        private static void RefreshProperty(SerializedProperty property)
        {
            Undo.RecordObject(property.serializedObject.targetObject, "Changed property value");
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        }
    }
}
#endif