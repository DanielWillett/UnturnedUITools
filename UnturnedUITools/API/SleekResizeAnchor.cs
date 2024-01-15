using DanielWillett.UITools.Util;

namespace DanielWillett.UITools.API;

/// <summary>
/// Defines how <see cref="SleekElementBuilder{TElement}"/> resizes elements.
/// </summary>
public enum SleekResizeAnchor
{
    /// <summary>
    /// Scale out from the top left of the object.
    /// </summary>
    TopLeft,

    /// <summary>
    /// Scale out from the upper center of the object.
    /// </summary>
    TopCenter,

    /// <summary>
    /// Scale out from the top right of the object.
    /// </summary>
    TopRight,

    /// <summary>
    /// Scale out from the left center of the object.
    /// </summary>
    LeftCenter,

    /// <summary>
    /// Scale out from the center of the object.
    /// </summary>
    Center,

    /// <summary>
    /// Scale out from the right center of the object.
    /// </summary>
    RightCenter,

    /// <summary>
    /// Scale out from the bottom left of the object.
    /// </summary>
    BottomLeft,

    /// <summary>
    /// Scale out from the lower center of the object.
    /// </summary>
    BottomCenter,

    /// <summary>
    /// Scale out from the bottom right of the object.
    /// </summary>
    BottomRight,

    /// <summary>
    /// Scale from the corner closest to which corner the object's in within it's parent's bounding box.
    /// </summary>
    /// <remarks>This scales the object towards the center of its parent's bounding box.</remarks>
    Auto
}
