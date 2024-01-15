using DanielWillett.UITools.API;
using SDG.Unturned;
using System;

namespace DanielWillett.UITools.Util;

/// <summary>
/// Extensions for <see cref="ISleekElement"/> and <see cref="SleekElementBuilder{TElement}"/>.
/// </summary>
public static class SleekExtensions
{
    /// <summary>
    /// Create a <see cref="SleekElementBuilder{TElement}"/> for the given element.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static SleekElementBuilder<TElement> Configure<TElement>(this TElement element) where TElement : class, ISleekElement
    {
        ThreadUtil.assertIsGameThread();

        return new SleekElementBuilder<TElement>(element);
    }

    /// <summary>
    /// Enumerate through the children of <paramref name="element"/>.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static SleekChildEnumerator AsEnumerable(this ISleekElement element)
    {
        ThreadUtil.assertIsGameThread();

        return new SleekChildEnumerator(element);
    }

    /// <summary>
    /// Enumerate through the children of <paramref name="element"/>.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static SleekChildEnumerator GetEnumerator(this ISleekElement element)
    {
        ThreadUtil.assertIsGameThread();

        return new SleekChildEnumerator(element);
    }
}