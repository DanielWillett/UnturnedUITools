using DanielWillett.UITools.Util;

namespace DanielWillett.UITools.API;

/// <summary>
/// Defines how <see cref="SleekElementBuilder{TElement}"/> offsets elements.
/// </summary>
public enum SleekPositionOffsetAnchor
{
    /// <summary>
    /// Positive values offset towards the top left of the parent's bounding box.
    /// </summary>
    TopLeft,

    /// <summary>
    /// Positive values offset towards the top center of the parent's bounding box. Sets offset to zero when the element is centered in the X direction.
    /// </summary>
    TopCenter,

    /// <summary>
    /// Positive values offset towards the top right of the parent's bounding box.
    /// </summary>
    TopRight,

    /// <summary>
    /// Positive values offset towards the left center of the parent's bounding box. Sets offset to zero when the element is centered in the Y direction.
    /// </summary>
    LeftCenter,

    /// <summary>
    /// Positive values offset towards the center of the parent's bounding box. Sets offset to zero when the element is centered.
    /// </summary>
    Center,

    /// <summary>
    /// Positive values offset towards the center of the parent's bounding box. Sets offset to zero when the element is centered.
    /// </summary>
    Inwards = Center,

    /// <summary>
    /// Positive values offset towards the right center of the parent's bounding box. Sets offset to zero when the element is centered in the Y direction.
    /// </summary>
    RightCenter,

    /// <summary>
    /// Positive values offset towards the bottom left of the parent's bounding box.
    /// </summary>
    BottomLeft,

    /// <summary>
    /// Positive values offset towards the bottom center of the parent's bounding box. Sets offset to zero when the element is centered in the X direction.
    /// </summary>
    BottomCenter,

    /// <summary>
    /// Positive values offset towards the bottom right of the parent's bounding box.
    /// </summary>
    BottomRight,

    /// <summary>
    /// Positive values offset away from center of the parent's bounding box. Sets offset to zero when the element is centered.
    /// </summary>
    Outwards,

    /// <summary>
    /// Positive values offset towards the top but away from the center of the parent's bounding box. Sets offset to zero when the element is centered in the X direction.
    /// </summary>
    TopOutwardsCenter,

    /// <summary>
    /// Positive values offset towards the left but away from the center of the parent's bounding box. Sets offset to zero when the element is centered in the Y direction.
    /// </summary>
    LeftOutwardsCenter,

    /// <summary>
    /// Positive values offset towards the right but away from the center of the parent's bounding box. Sets offset to zero when the element is centered in the Y direction.
    /// </summary>
    RightOutwardsCenter,

    /// <summary>
    /// Positive values offset towards the bottom but away from the center of the parent's bounding box. Sets offset to zero when the element is centered in the X direction.
    /// </summary>
    BottomOutwardsCenter,
}
