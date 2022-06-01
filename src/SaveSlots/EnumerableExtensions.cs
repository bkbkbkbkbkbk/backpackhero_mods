using System;
using System.Collections;
using System.Collections.Generic;

namespace SaveSlots
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<object> ToIEnumerable(this IEnumerator enumerator)
        {
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> enumerable, Func<T> extraStep)
        {
            foreach (var i in enumerable)
                yield return i;
            yield return extraStep();
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> enumerable, Func<T> extraStep)
        {
            yield return extraStep();
            foreach (var i in enumerable)
                yield return i;
        }
    }


}
