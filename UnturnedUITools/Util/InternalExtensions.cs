using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace DanielWillett.UITools.Util;
internal static class InternalExtensions
{
    /// <summary>
    /// Returns the first matching value only if there are no other matching values.
    /// </summary>
    /// <remarks>Doesn't throw an error when there are no matches (unlike the normal linq version).</remarks>
    [Pure]
    public static T? SingleOrDefaultSafe<T>(this IEnumerable<T> enumerable, Predicate<T> predicate)
    {
        /* Why tf does SingleOrDefault throw an exception... */
        bool found = false;
        T? rtn = default;
        if (enumerable is T[] array)
        {
            for (int i = 0; i < array.Length; ++i)
            {
                if (!predicate(array[i]))
                    continue;
                if (found)
                    return default;

                rtn = array[i];
                found = true;
            }
        }
        else if (enumerable is List<T> list)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (!predicate(list[i]))
                    continue;
                if (found)
                    return default;

                rtn = list[i];
                found = true;
            }
        }
        else
        {
            foreach (T value in enumerable)
            {
                if (!predicate(value))
                    continue;
                if (found)
                    return default;

                rtn = value;
                found = true;
            }
        }

        return rtn;
    }
}
