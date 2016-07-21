using System;
using System.Collections;

namespace DeepComparison
{
    internal static class EnumerableExt
    {
        public static bool SequenceEqual(this IEnumerable xE, IEnumerable yE, Func<object, object, bool> compare)
        {
            var xEr = xE.GetEnumerator();
            var yEr = yE.GetEnumerator();
            while (xEr.MoveNext())
            {
                if (!yEr.MoveNext()) return false;
                if (!compare(xEr.Current, yEr.Current)) return false;
            }
            return !yEr.MoveNext();
        }
    }
}