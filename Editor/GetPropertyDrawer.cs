using BGS.Attributes;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GetAttribute), true)]
public class GetPropertyDrawer : PropertyDrawer
{
    private GetAttribute GetAttribute => (GetAttribute)attribute;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GetComponent(property, GetAttribute.GetterSource);
        
        EditorGUI.BeginProperty(position, label, property);
        GUI.enabled = false;
        GUI.backgroundColor = property.objectReferenceValue == null ? Color.red : Color.white;
        EditorGUI.PropertyField(position, property, new GUIContent(label.text + " " + LabelText(GetAttribute.GetterSource)));
        EditorGUI.EndProperty();
    }

    private string LabelText(GetterSource getterSource) => getterSource switch
        {
            GetterSource.Object => "[Auto Get]", 
            GetterSource.Children => "[Auto Get Children]", 
            GetterSource.Parent => "[Auto Get Parent]", 
            GetterSource.Find => "[Auto Find]", 
            GetterSource.Add => "[Auto Get/Add]", 
            _ => "[Auto Get]"
        };

    private void GetComponent(SerializedProperty property, GetterSource getterSource)
    {
        var mb = property.serializedObject.targetObject as MonoBehaviour;
        
        var component = mb == null ? null : getterSource switch
        {
            GetterSource.Object => mb.GetComponent(fieldInfo.FieldType), 
            GetterSource.Children => mb.GetComponentInChildren(fieldInfo.FieldType), 
            GetterSource.Parent => mb.GetComponentInParent(fieldInfo.FieldType), 
            GetterSource.Find => Object.FindObjectOfType(fieldInfo.FieldType), 
            GetterSource.Add => GetOrAdd(mb, fieldInfo.FieldType), 
            _ => null
        };
        
        property.objectReferenceValue = component ? component : null;
    }

    private Component GetOrAdd(MonoBehaviour enactor, System.Type type)
    {
        var tryGet = enactor.GetComponent(type);
        return tryGet ? tryGet : enactor.gameObject.AddComponent(type);
    }
}