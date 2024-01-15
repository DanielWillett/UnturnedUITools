using DanielWillett.UITools.Util;

namespace DanielWillett.UITools.API;

/// <summary>
/// Defines how <see cref="SleekElementBuilder{TElement}"/> positions elements.
/// </summary>
public enum SleekPositionScaleAnchor
{
    /// <summary>
    /// Top left of the parent's bounding box.
    /// </summary>
    TopLeft,

    /// <summary>
    /// Top center of the parent's bounding box.
    /// </summary>
    TopCenter,

    /// <summary>
    /// Top right of the parent's bounding box.
    /// </summary>
    TopRight,

    /// <summary>
    /// Center left of the parent's bounding box.
    /// </summary>
    LeftCenter,

    /// <summary>
    /// Center of the parent's bounding box.
    /// </summary>
    Center,

    /// <summary>
    /// Center right of the parent's bounding box.
    /// </summary>
    RightCenter,

    /// <summary>
    /// Bottom left of the parent's bounding box.
    /// </summary>
    BottomLeft,

    /// <summary>
    /// Bottom center of the parent's bounding box.
    /// </summary>
    BottomCenter,

    /// <summary>
    /// Bottom right of the parent's bounding box.
    /// </summary>
    BottomRight
}
