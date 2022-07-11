using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
namespace Extra.Attributes
{
    public partial class GetPropertyDrawer
    {
        private const float ButtonWidth = 0.2f, FieldWidth = 0.8f;

        private static Rect RightPartOf(Rect position)
        {
            position.x += position.width * (1 - ButtonWidth);
            position.width *= ButtonWidth;
            return position;
        }

        private static Rect LeftPartOf(Rect position)
        {
            position.width *= FieldWidth;
            return position;
        }
        
        private static GUIContent Formatted(GUIContent label, in GetAttribute attribute)
        {
            label.text = $"{label.text} [src: {attribute.GetterSource.ToString()}]";
            return label;
        }
        
        private static IEnumerable<string> CountedNames(Object[] input, bool includeAddNew = false)
        {
            var res = new List<string>(input.Length + 1);
            var names = input.Select(item => item.name).ToArray();

            var indexTrackerDictionary = names
                .GroupBy(item => item)
                .Where(item => item.Count() > 1)
                .ToDictionary(item => item.Key, _ => 0);

            foreach (var item in names)
            {
                if (!indexTrackerDictionary.TryGetValue(item, out var value)) // If not a duplicate...
                {
                    res.Add(item); // We don't care.
                    continue;
                }

                indexTrackerDictionary[item] = ++value; // We've now encountered this item {value} times.
                res.Add($"{item} [{value - 1}]");
            }

            if (includeAddNew) res.Add("(Add new)");
            return res;
        }
    }
}
#endif