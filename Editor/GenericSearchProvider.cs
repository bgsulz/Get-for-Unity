#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Extra.Attributes
{
    public abstract class GenericSearchProvider<T> : ScriptableObject, ISearchWindowProvider
    {
        private T[] _listItems;
        private Action<T> _selected;

        protected abstract string Name { get; }
        protected virtual char GroupSeparator => '/';
        protected virtual char ItemSeparator => '.';
        
        protected abstract string StringRepresentation(T value);
        protected virtual Texture IconRepresentation(T value) => null;

        public void Construct(T[] listItems, Action<T> selected, bool forceInitialize = false)
        {
            _listItems = listItems;
            if (forceInitialize || _selected == null) _selected = selected;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var entries = new List<SearchTreeEntry> { new SearchTreeGroupEntry(new GUIContent(Name)) };

            var stringToOriginal = new Dictionary<string, T>();
            foreach (var item in _listItems)
            {
                var key = StringRepresentation(item);
                var index = 0;
                while (stringToOriginal.ContainsKey(key))
                {
                    index++;
                    key = $"{key} ({index})";
                }
                stringToOriginal.Add(key, item);
            }
            
            var stringItems = stringToOriginal.Keys.OrderBy(x => x);

            var knownGroups = new List<string>();
            foreach (var item in stringItems)
            {
                var splitName = item.Split(ItemSeparator);

                entries.AddRange(GroupEntries(splitName, ref knownGroups));

                var original = stringToOriginal[item];
                entries.Add(new SearchTreeEntry(new GUIContent(splitName.Last(), IconRepresentation(original)))
                {
                    level = splitName.Length,
                    userData = original
                });
            }
        
            return entries;
        }

        private List<SearchTreeGroupEntry> GroupEntries(string[] splitName, ref List<string> knownGroups)
        {
            var groups = new List<SearchTreeGroupEntry>();

            var groupName = string.Empty;
            for (var i = 0; i < splitName.Length - 1; i++)
            {
                groupName += splitName[i];
                if (!knownGroups.Contains(groupName))
                {
                    var groupContent = new GUIContent(groupName, EditorGUIUtility.IconContent("d_Folder Icon").image);
                    groups.Add(new SearchTreeGroupEntry(groupContent, i + 1));
                    knownGroups.Add(groupName);
                }
                groupName += GroupSeparator;
            }

            return groups;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            if (searchTreeEntry.userData is not T asT) return false;
            _selected?.Invoke(asT);
            return true;
        }
    }
}
#endif