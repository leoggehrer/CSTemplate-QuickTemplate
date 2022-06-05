//@BaseCode
//MdStart
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace CommonBase.Extensions
{
    public static partial class ConcurrentBagExtensions
    {
        public static void AddSafe<T>(this ConcurrentBag<T> source, T otherElement)
            where T : class
        {
            otherElement.CheckArgument(nameof(otherElement));

            source.Add(otherElement);
        }
        public static IEnumerable<T> AddRangeSafe<T>(this ConcurrentBag<T> source, IEnumerable<T> otherSource)
        {
            foreach (var item in otherSource)
            {
                source.Add(item);
            }
            return source;
        }
    }
}
//MdEnd