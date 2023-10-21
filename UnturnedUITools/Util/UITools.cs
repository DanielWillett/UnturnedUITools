using SDG.Unturned;

namespace DanielWillett.UITools.Util;

/// <summary>
/// Random utilities for working with Glazier elements.
/// </summary>
public static class UITools
{
    /// <summary>
    /// Copies all the basic transform values from one <see cref="ISleekElement"/> to another.
    /// </summary>
    public static void CopyTransformFrom(this ISleekElement element, ISleekElement other)
    {
        element.PositionOffset_X = other.PositionOffset_X;
        element.PositionOffset_Y = other.PositionOffset_Y;

        element.PositionScale_X = other.PositionScale_X;
        element.PositionScale_Y = other.PositionScale_Y;

        element.SizeOffset_X = other.SizeOffset_X;
        element.SizeOffset_Y = other.SizeOffset_Y;

        element.SizeScale_X = other.SizeScale_X;
        element.SizeScale_Y = other.SizeScale_Y;
    }
}
