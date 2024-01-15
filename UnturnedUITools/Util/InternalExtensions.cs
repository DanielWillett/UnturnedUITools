using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace DanielWillett.UITools.Util;
internal static class InternalExtensions
{
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

    /*
     * Game scale
     * (0, 0)         (1, 0)
     * 0--                 ]
     * |                   ]
     * [                   ]
     * (0, 1)         (1, 1)
     *
     * Range scale
     * (-1, -1)      (1, -1)
     * [         |         ]
     * [       --0--       ]
     * [         |         ]
     * (-1, 1)        (1, 1)
     */

    [Pure]
    public static float ToRangeScale(this float gameScale) => gameScale * 2f - 1f;

    [Pure]
    public static float ToGameScale(this float rangeScale) => (rangeScale + 1f) / 2f;

    [Pure]
    public static int RangeSign(this float gameScale)
    {
        if (gameScale == 0.5f)
            return 0;
        if (gameScale < 0.5f)
            return -1;
        return 1;
    }
}
