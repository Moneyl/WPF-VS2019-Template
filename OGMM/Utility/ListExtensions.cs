using System;
using System.Collections.Generic;
using System.Text;

namespace OGMM.Utility
{
    public static class ListExtensions
    {
        public static void AddIfUnique<T>(this List<T> list, T item)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
            }
        }
    }
}
