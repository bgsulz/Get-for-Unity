#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Extra.Attributes
{
    public class ObjectSearchProvider : GenericSearchProvider<Object>
    {
        protected override string StringRepresentation(Object value)
        {
            return value switch
            {
                Component component => GetComponentPath(component),
                GameObject gameObject => GetGameObjectPath(gameObject.transform),
                ScriptableObject scriptable => GetScriptablePath(scriptable),
                _ => value.name
            };
        }

        protected override Texture IconRepresentation(Object value) => EditorGUIUtility.ObjectContent(value, value.GetType()).image;

        protected override string Name => "Candidate Objects";
        
        private static string GetGameObjectPath(Transform transform)
        {
            var path = transform.name;
            
            while (transform.parent != null)
            {
                transform = transform.parent;
                path = $"{transform.name}.{path}";
            }

            return path;
        }
        
        private static string GetComponentPath(Component component)
        {
            var transform = component.transform;
            var path = GetGameObjectPath(transform);
            
            return $"{path} [{component.GetType().Name}]";
        }
        
        private static string GetScriptablePath(Object scriptable) =>
            $"{Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(scriptable)).Replace("/", ".")} [{scriptable.GetType().Name}]";
    }
}
#endif