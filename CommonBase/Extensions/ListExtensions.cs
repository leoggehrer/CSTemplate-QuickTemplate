//@BaseCode
//MdStart
using System.Collections.Generic;

namespace CommonBase.Extensions
{
    public static partial class ListExtensions
    {
        public static IEnumerable<T> Eject<T>(this List<T> source)
        {
            var result = source.ToArray();

            source.Clear();
            return result;
        }
        public static int AddRangeAndCount<T>(this List<T> source, IEnumerable<T> items)
        {
            var result = 0;

            foreach (var item in items)
            {
                source.Add(item);
                result++;
            }
            return result;
        }
    }
}
//MdEnd