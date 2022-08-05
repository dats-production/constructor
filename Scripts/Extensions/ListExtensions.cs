using System.Collections.Generic;

namespace Extensions
{
    public static class ListExtensions
    {
        public static T Pop<T>(this IList<T> list, bool last = true)
        {
            if (list.Count <= 0) return default;

            var index = last ? list.Count - 1 : 0;
            var value = list[index];
            list.RemoveAt(index);
            return value;
        }
    }
}