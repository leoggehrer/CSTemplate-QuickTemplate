//@BaseCode
//MdStart

namespace CommonBase.Extensions
{
    public static class IEnumerableExtensions
    {
        public static int IndexOf<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            source.CheckArgument(nameof(source));
            predicate.CheckArgument(nameof(predicate));

            var idx = 0;
            var result = -1;
            var enumerator = source.GetEnumerator();

            while (result == -1 && enumerator.MoveNext())
            {
                if (predicate(enumerator.Current))
                {
                    result = idx;
                }
                idx++;
            }
            return result;
        }
    }
}
//MdEnd