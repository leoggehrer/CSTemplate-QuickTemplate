//@BaseCode
//MdStart
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonBase.Extensions
{
    public static partial class EnumerableExtensions
    {
        public static IEnumerable<ST> ToEnumerable<T, ST>(this IEnumerable<T> source, Func<T, ST> expandSelector)
        {
            List<ST> expandResult = new();

            if (source != null && expandSelector != null)
            {
                foreach (var item in source)
                {
                    var subItem = expandSelector(item);

                    if (subItem != null)
                    {
                        expandResult.Add(subItem);
                    }
                }
            }
            return expandResult;
        }
        public static IEnumerable<ST> Flatten<T, ST>(this IEnumerable<T> source, Func<T, IEnumerable<ST>> expandSelector)
        {
            List<ST> expandResult = new();

            if (source != null && expandSelector != null)
            {
                foreach (var item in source)
                {
                    var subItems = expandSelector(item);

                    if (subItems != null)
                    {
                        expandResult.AddRange(subItems);
                    }
                }
            }
            return expandResult;
        }

        public static T SingleAction<T>(this T source, Action<T> action)
        {
            if (source != null && action != null)
            {
                action(source);
            }
            return source;
        }
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source != null && action != null)
            {
                foreach (var item in source)
                {
                    action(item);
                }
            }
            return source ?? Array.Empty<T>();
        }
        public static int NextValue<T>(this IEnumerable<T> source, Func<T, int> getValue)
        {
            int result = 0;

            if (source != null && getValue != null)
            {
                source.ForEach(i =>
                {
                    int value = getValue(i);

                    if (value > result)
                    {
                        result = value;
                    }
                });
            }
            return result + 1;
        }

        public static IEnumerable<T> Add<T>(this IEnumerable<T> source, T item)
        {
            var result = new List<T>(source)
            {
                item
            };
            return result;
        }
        public static IEnumerable<T> AddRange<T>(this IEnumerable<T> source, IEnumerable<T> items)
        {
            var result = new List<T>(source);

            result.AddRange(items);
            return result;
        }
        public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> source)
        {
            using var e = source.GetEnumerator();
            if (e.MoveNext())
            {
                for (var value = e.Current; e.MoveNext(); value = e.Current)
                {
                    yield return value;
                }
            }
        }

        ///<summary>Finds the index of the first item matching an expression in an enumerable.</summary>
        ///<param name="items">The enumerable to search.</param>
        ///<param name="predicate">The expression to test the items against.</param>
        ///<returns>The index of the first matching item, or -1 if no items match.</returns>
        public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            int retVal = 0;

            foreach (var item in items)
            {
                if (predicate(item))
                    return retVal;

                retVal++;
            }
            return -1;
        }
        ///<summary>Finds the index of the first occurrence of an item in an enumerable.</summary>
        ///<param name="items">The enumerable to search.</param>
        ///<param name="item">The item to find.</param>
        ///<returns>The index of the first matching item, or -1 if the item was not found.</returns>
        public static int IndexOf<T>(this IEnumerable<T> items, T item)
        {
            var result = -1;

            if (items != null)
            {
                result = items.FindIndex(i => EqualityComparer<T>.Default.Equals(item, i));
            }
            return result;
        }

        public static IEnumerable<T> TakeTo<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            var result = default(IEnumerable<T>);

            if (source != null)
            {
                var end = false;

                result = source.Where(e =>
                {
                    if (end == false && predicate != null)
                    {
                        end = predicate(e);
                    }
                    return end == false;
                });
            }
            return result ?? Array.Empty<T>();
        }
    }
}
//MdEnd