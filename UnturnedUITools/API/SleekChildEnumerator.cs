using SDG.Unturned;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DanielWillett.UITools.API;

/// <summary>
/// Enumerates through the children of an <see cref="ISleekElement"/>.
/// </summary>
public struct SleekChildEnumerator : IEnumerator<ISleekElement>, IEnumerable<ISleekElement>
{
    private int _index;
    private readonly ISleekElement _parent;
    private ISleekElement _current;

    /// <summary>
    /// Owner of this enumerator.
    /// </summary>
    public ISleekElement Parent => _parent;

    /// <summary>
    /// Child index of the current element (or -1 if not started/reset).
    /// </summary>
    public int Index => _index;

    /// <summary>
    /// The child of <see cref="Parent"/> at index <see cref="Index"/>.
    /// </summary>
    public ISleekElement Current => _current;
    object IEnumerator.Current => _current;

    /// <summary>
    /// Enumerate through the children of <paramref name="parent"/>.
    /// </summary>
    public SleekChildEnumerator(ISleekElement parent)
    {
        _index = -1;
        _parent = parent;
        _current = null!;
    }

    /// <summary>
    /// Does nothing.
    /// </summary>
    public void Dispose() { }

    /// <inheritdoc />
    public bool MoveNext()
    {
        ++_index;
        try
        {
            _current = _parent.GetChildAtIndex(_index);
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            _current = null!;
            return false;
        }
    }

    /// <inheritdoc />
    public void Reset()
    {
        _index = -1;
        _current = null!;
    }

    /// <inheritdoc />
    public IEnumerator<ISleekElement> GetEnumerator() => _index == -1 ? this : new SleekChildEnumerator(_parent);
    IEnumerator IEnumerable.GetEnumerator() => _index == -1 ? this : new SleekChildEnumerator(_parent);
}
