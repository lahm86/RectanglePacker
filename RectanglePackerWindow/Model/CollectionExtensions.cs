using System;
using System.Collections.Generic;

namespace RectanglePackerWindow.Model
{
    public static class CollectionExtensions
    {
        public static void Randomise<T>(this List<T> list, Random rand)
        {
            SortedDictionary<int, T> map = new SortedDictionary<int, T>();
            foreach (T item in list)
            {
                int r;
                do
                {
                    r = rand.Next();
                }
                while (map.ContainsKey(r));
                map.Add(r, item);
            }

            list.Clear();
            list.AddRange(map.Values);
        }
    }
}