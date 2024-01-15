using SDG.Unturned;
using System;

namespace DanielWillett.UITools.Util;

/// <summary>
/// Fluent API implementation for editing <see cref="ISleekElement"/>.
/// </summary>
/// <remarks>There is no generic constraint on this because not all sleek interfaces inherit <see cref="ISleekElement"/>.</remarks>
public readonly struct SleekElementBuilder<TElement> where TElement : class
{
    private readonly TElement _element;

    /// <summary>
    /// The element currently being edited.
    /// </summary>
    public TElement Element => _element ?? throw new InvalidOperationException("Can not cross configure method chains unless you use the overload with an 'out' variable.");

    /// <summary>
    /// The element currently being edited.
    /// </summary>
    /// <remarks>This allows you to access the element as a <see cref="ISleekElement"/> since not all of the interfaces inherit <see cref="ISleekElement"/>.</remarks>
    public ISleekElement SleekElement => (ISleekElement)_element;

    internal SleekElementBuilder(ISleekElement element)
    {
        _element = element as TElement ?? throw new ArgumentException($"Must implement {typeof(TElement).Name}", nameof(element));
    }
}
