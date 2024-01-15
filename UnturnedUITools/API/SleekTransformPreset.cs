using SDG.Unturned;

namespace DanielWillett.UITools.API;

/// <summary>
/// Stores a size transform for a sleek element.
/// </summary>
public readonly struct SleekTransformPreset
{
    /// <summary>
    /// How much bigger the element is outside of its bounds in the x direction (in pixels). Use negative values to shrink the element.
    /// </summary>
    public float? SizeOffsetX { get; }

    /// <summary>
    /// How much bigger the element is outside of its bounds in the y direction (in pixels). Use negative values to shrink the element.
    /// </summary>
    public float? SizeOffsetY { get; }

    /// <summary>
    /// Width of the element (from 0-1, 0 being 0px and 1 being the size of the monitor or parent object).
    /// </summary>
    public float? SizeScaleX { get; }

    /// <summary>
    /// Height of the element (from 0-1, 0 being 0px and 1 being the size of the monitor or parent object).
    /// </summary>
    public float? SizeScaleY { get; }

    /// <summary>
    /// How offset the element is from its bounds in the x direction (in pixels). Positive values move the element right, negative values move it left.
    /// </summary>
    public float? PositionOffsetX { get; }

    /// <summary>
    /// How offset the element is from its bounds in the y direction (in pixels). Positive values move the element down, negative values move it up.
    /// </summary>
    public float? PositionOffsetY { get; }

    /// <summary>
    /// X Position of the top left corner of the element from the left side of the screen (from 0-1, 0 being 0px and 1 being the size of the monitor or parent object).
    /// </summary>
    public float? PositionScaleX { get; }

    /// <summary>
    /// X Position of the top left corner of the element from the top of the screen (from 0-1, 0 being 0px and 1 being the size of the monitor or parent object).
    /// </summary>
    public float? PositionScaleY { get; }

    /// <summary>
    /// Create a <see cref="SleekTransformPreset"/> from raw <see cref="float"/> values.
    /// </summary>
    public SleekTransformPreset(float? positionOffsetX, float? positionOffsetY, float? positionScaleX, float? positionScaleY, float? sizeOffsetX, float? sizeOffsetY, float? sizeScaleX, float? sizeScaleY)
    {
        PositionOffsetX = positionOffsetX;
        PositionOffsetY = positionOffsetY;
        PositionScaleX = positionScaleX;
        PositionScaleY = positionScaleY;
        SizeOffsetX = sizeOffsetX;
        SizeOffsetY = sizeOffsetY;
        SizeScaleX = sizeScaleX;
        SizeScaleY = sizeScaleY;
    }

    /// <summary>
    /// Create a <see cref="SleekTransformPreset"/> from an <see cref="ISleekElement"/>.
    /// </summary>
    public SleekTransformPreset(ISleekElement element)
    {
        PositionOffsetX = element.PositionOffset_X;
        PositionOffsetY = element.PositionOffset_Y;
        PositionScaleX = element.PositionScale_X;
        PositionScaleY = element.PositionScale_Y;
        SizeOffsetX = element.SizeOffset_X;
        SizeOffsetY = element.SizeOffset_Y;
        SizeScaleX = element.SizeScale_X;
        SizeScaleY = element.SizeScale_Y;
    }


    /// <summary>
    /// Create a preset from a size in pixels.
    /// </summary>
    public static SleekTransformPreset SizePixels(float xOffset, float yOffset) => new SleekTransformPreset(null, null, null, null, xOffset, yOffset, null, null);

    /// <summary>
    /// Create a preset from a size in scale.
    /// </summary>
    public static SleekTransformPreset SizeScale(float xScale, float yScale) => new SleekTransformPreset(null, null, null, null, null, null, xScale, yScale);

    /// <summary>
    /// Create a preset from a size.
    /// </summary>
    public static SleekTransformPreset Size(float xOffset, float yOffset, float xScale, float yScale) => new SleekTransformPreset(null, null, null, null, xOffset, yOffset, xScale, yScale);

    /// <summary>
    /// Create a preset from a position in pixels.
    /// </summary>
    public static SleekTransformPreset PositionPixels(float xOffset, float yOffset) => new SleekTransformPreset(xOffset, yOffset, null, null, null, null, null, null);

    /// <summary>
    /// Create a preset from a position in scale.
    /// </summary>
    public static SleekTransformPreset PositionScale(float xScale, float yScale) => new SleekTransformPreset(null, null, xScale, yScale, null, null, null, null);

    /// <summary>
    /// Create a preset from a position.
    /// </summary>
    public static SleekTransformPreset Position(float xOffset, float yOffset, float xScale, float yScale) => new SleekTransformPreset(xOffset, yOffset, xScale, yScale, null, null, null, null);

    /// <summary>
    /// Apply this preset to an element.
    /// </summary>
    public void Apply(ISleekElement element)
    {
        if (PositionOffsetX.HasValue)
            element.PositionOffset_X = PositionOffsetX.Value;
        if (PositionOffsetY.HasValue)
            element.PositionOffset_Y = PositionOffsetY.Value;
        if (PositionScaleX.HasValue)
            element.PositionScale_X = PositionScaleX.Value;
        if (PositionScaleY.HasValue)
            element.PositionScale_Y = PositionScaleY.Value;
        if (SizeOffsetX.HasValue)
            element.SizeOffset_X = SizeOffsetX.Value;
        if (SizeOffsetY.HasValue)
            element.SizeOffset_Y = SizeOffsetY.Value;
        if (SizeScaleX.HasValue)
            element.SizeScale_X = SizeScaleX.Value;
        if (SizeScaleY.HasValue)
            element.SizeScale_Y = SizeScaleY.Value;
    }
}
