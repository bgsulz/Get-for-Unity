using UnityEditor.Experimental.GraphView;
using UnityEngine;

#if UNITY_EDITOR
namespace Extra.Attributes
{
    public static class SearchWindowGUI
    {
        public static bool Open<T>(this T provider) 
            where T : ScriptableObject, ISearchWindowProvider
        {
            return SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), provider);
        }
    }
}
#endif