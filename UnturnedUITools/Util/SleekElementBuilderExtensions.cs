using DanielWillett.UITools.API;
using JetBrains.Annotations;
using SDG.Unturned;
using System;
using System.Reflection;
using UnityEngine;
using Action = System.Action;

namespace DanielWillett.UITools.Util;

/// <summary>
/// Extensions for <see cref="SleekElementBuilder{TElement}"/> to handle extended sleek element types.
/// </summary>
public static class SleekElementBuilderExtensions
{

    /// <summary>
    /// Finalize the builder.
    /// </summary>
    /// <returns>The finished element.</returns>
    public static TSleekElement Build<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder) where TSleekElement : class
    {
        TSleekElement element = builder.Element;
        return element;
    }

    /// <summary>
    /// Finalize the builder and add the element as a child of <paramref name="parent"/>.
    /// </summary>
    /// <remarks>The same as calling <see cref="ISleekElement.AddChild"/> on the parent after building.</remarks>
    /// <returns>The finished element.</returns>
    public static TSleekElement BuildAndParent<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder, ISleekElement parent) where TSleekElement : class
    {
        ISleekElement element = (ISleekElement)builder.Element;

        if (parent.FindIndexOfChild(element) == -1)
            parent.AddChild(element);

        UnturnedLog.info($"{element.GetType().Name}.");
        int c = 0;
        foreach (PropertyInfo property in element.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy))
        {
            if (property.GetMethod != null)
                UnturnedLog.info($" {++c:00}. {property.DeclaringType}.{property.Name} = {property.GetValue(element)}.");
        }

        return (TSleekElement)element;
    }

    /// <summary>
    /// Fills the entire bounding area with the element.
    /// </summary>
    public static ref readonly SleekElementBuilder<TSleekElement> Fill<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder) where TSleekElement : class
    {
        ISleekElement element = (ISleekElement)builder.Element;

        element.PositionOffset_X = 0;
        element.PositionOffset_Y = 0;
        element.PositionScale_X = 0;
        element.PositionScale_Y = 0;
        element.SizeOffset_X = 0;
        element.SizeOffset_Y = 0;
        element.SizeScale_X = 1f;
        element.SizeScale_Y = 1f;
        return ref builder;
    }

    /// <summary>
    /// Copies all basic transforms from <see cref="ISleekElement"/>.
    /// </summary>
    public static ref readonly SleekElementBuilder<TSleekElement> WithOrigin<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder, ISleekElement transform) where TSleekElement : class
    {
        ISleekElement element = (ISleekElement)builder.Element;

        element.PositionOffset_X = transform.PositionOffset_X;
        element.PositionOffset_Y = transform.PositionOffset_Y;
        element.PositionScale_X = transform.PositionScale_X;
        element.PositionScale_Y = transform.PositionScale_Y;
        element.SizeOffset_X = transform.SizeOffset_X;
        element.SizeOffset_Y = transform.SizeOffset_Y;
        element.SizeScale_X = transform.SizeScale_X;
        element.SizeScale_Y = transform.SizeScale_Y;
        return ref builder;
    }

    /// <summary>
    /// Copies all basic transforms from <see cref="ISleekElement"/>.
    /// </summary>
    public static ref readonly SleekElementBuilder<TSleekElement> WithPreset<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder, [ValueProvider("DanielWillett.UITools.Util.SleekTransformPresets")] in SleekTransformPreset preset) where TSleekElement : class
    {
        preset.Apply((ISleekElement)builder.Element);
        return ref builder;
    }

    /// <summary>
    /// Moves the element to the desired point on the screen and resets any offsets.
    /// </summary>
    public static ref readonly SleekElementBuilder<TSleekElement> Anchor<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder, SleekPositionScaleAnchor anchor) where TSleekElement : class
    {
        ISleekElement element = (ISleekElement)builder.Element;

        switch ((SleekPositionScaleAnchor)((int)anchor % 3))
        {
            case SleekPositionScaleAnchor.TopLeft:
                element.PositionOffset_X = 0;
                element.PositionScale_X = 0;
                break;
            case SleekPositionScaleAnchor.TopCenter:
                element.PositionOffset_X = -(element.SizeOffset_X / 2f);
                element.PositionScale_X = 0.5f - element.SizeScale_X / 2f;
                break;
            case SleekPositionScaleAnchor.TopRight:
                element.PositionOffset_X = -element.SizeOffset_X;
                element.PositionScale_X = 1 - element.SizeScale_X;
                break;
        }
        switch ((SleekPositionScaleAnchor)((int)anchor / 3 * 3))
        {
            case SleekPositionScaleAnchor.TopLeft:
                element.PositionOffset_Y = 0;
                element.PositionScale_Y = 0;
                break;
            case SleekPositionScaleAnchor.LeftCenter:
                element.PositionOffset_Y = -(element.SizeOffset_Y / 2f);
                element.PositionScale_Y = 0.5f - element.SizeScale_Y / 2f;
                break;
            case SleekPositionScaleAnchor.BottomLeft:
                element.PositionOffset_Y = -element.SizeOffset_Y;
                element.PositionScale_Y = 1 - element.SizeScale_Y;
                break;
        }
        return ref builder;
    }

    /// <summary>
    /// Moves the element to the top left corner of the screen and resets any offsets.
    /// </summary>
    public static ref readonly SleekElementBuilder<TSleekElement> TopLeft<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder) where TSleekElement : class
    {
        ISleekElement element = (ISleekElement)builder.Element;

        element.PositionOffset_X = 0;
        element.PositionOffset_Y = 0;
        element.PositionScale_X = 0;
        element.PositionScale_Y = 0;
        return ref builder;
    }

    /// <summary>
    /// Moves the element to the top center of the screen and resets any offsets.
    /// </summary>
    public static ref readonly SleekElementBuilder<TSleekElement> TopCenter<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder) where TSleekElement : class
    {
        ISleekElement element = (ISleekElement)builder.Element;

        element.PositionOffset_X = -(element.SizeOffset_X / 2f);
        element.PositionOffset_Y = 0;
        element.PositionScale_X = 0.5f - element.SizeScale_X / 2f;
        element.PositionScale_Y = 0;
        return ref builder;
    }

    /// <summary>
    /// Moves the element to the top right corner of the screen and resets any offsets.
    /// </summary>
    public static ref readonly SleekElementBuilder<TSleekElement> TopRight<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder) where TSleekElement : class
    {
        ISleekElement element = (ISleekElement)builder.Element;

        element.PositionOffset_X = -element.SizeOffset_X;
        element.PositionOffset_Y = 0;
        element.PositionScale_X = 1 - element.SizeScale_X;
        element.PositionScale_Y = 0;
        return ref builder;
    }

    /// <summary>
    /// Moves the element to the bottom left corner of the screen and resets any offsets.
    /// </summary>
    public static ref readonly SleekElementBuilder<TSleekElement> BottomLeft<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder) where TSleekElement : class
    {
        ISleekElement element = (ISleekElement)builder.Element;

        element.PositionOffset_X = 0;
        element.PositionOffset_Y = -element.SizeOffset_Y;
        element.PositionScale_X = 0;
        element.PositionScale_Y = 1 - element.SizeScale_Y;
        return ref builder;
    }

    /// <summary>
    /// Moves the element to the top left corner of the screen and resets any offsets.
    /// </summary>
    public static ref readonly SleekElementBuilder<TSleekElement> BottomCenter<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder) where TSleekElement : class
    {
        ISleekElement element = (ISleekElement)builder.Element;

        element.PositionOffset_X = -(element.SizeOffset_X / 2f);
        element.PositionOffset_Y = -element.SizeOffset_Y;
        element.PositionScale_X = 0.5f - element.SizeScale_X / 2f;
        element.PositionScale_Y = 1 - element.SizeScale_Y;
        return ref builder;
    }

    /// <summary>
    /// Moves the element to the top left corner of the screen and resets any offsets.
    /// </summary>
    public static ref readonly SleekElementBuilder<TSleekElement> BottomRight<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder) where TSleekElement : class
    {
        ISleekElement element = (ISleekElement)builder.Element;

        element.PositionOffset_X = -element.SizeOffset_X;
        element.PositionOffset_Y = -element.SizeOffset_Y;
        element.PositionScale_X = 1 - element.SizeScale_X;
        element.PositionScale_Y = 1 - element.SizeScale_Y;
        return ref builder;
    }

    /// <summary>
    /// Moves the element to the left center of the screen and resets any offsets.
    /// </summary>
    public static ref readonly SleekElementBuilder<TSleekElement> LeftCenter<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder) where TSleekElement : class
    {
        ISleekElement element = (ISleekElement)builder.Element;

        element.PositionOffset_X = 0;
        element.PositionOffset_Y = -(element.SizeOffset_Y / 2f);
        element.PositionScale_X = 0;
        element.PositionScale_Y = 0.5f - element.SizeScale_Y / 2f;
        return ref builder;
    }

    /// <summary>
    /// Moves the element to the center of the screen and resets any offsets.
    /// </summary>
    public static ref readonly SleekElementBuilder<TSleekElement> Center<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder) where TSleekElement : class
    {
        ISleekElement element = (ISleekElement)builder.Element;

        element.PositionOffset_X = -(element.SizeOffset_X / 2f);
        element.PositionOffset_Y = -(element.SizeOffset_Y / 2f);
        element.PositionScale_X = 0.5f - element.SizeScale_X / 2f;
        element.PositionScale_Y = 0.5f - element.SizeScale_Y / 2f;
        return ref builder;
    }

    /// <summary>
    /// Moves the element to the right center of the screen and resets any offsets.
    /// </summary>
    public static ref readonly SleekElementBuilder<TSleekElement> RightCenter<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder) where TSleekElement : class
    {
        ISleekElement element = (ISleekElement)builder.Element;

        element.PositionOffset_X = -element.SizeOffset_X;
        element.PositionOffset_Y = -(element.SizeOffset_Y / 2f);
        element.PositionScale_X = 1f;
        element.PositionScale_Y = 0.5f - element.SizeScale_Y / 2f;
        return ref builder;
    }

    /// <summary>
    /// Sets the raw values of the element's position offset without any manipulation
    /// </summary>
    /// <remarks>In pixels.</remarks>
    public static ref readonly SleekElementBuilder<TSleekElement> WithRawOffsetPixels<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder, Vector2 offset) where TSleekElement : class
    {
        ISleekElement element = (ISleekElement)builder.Element;

        element.PositionOffset_X = offset.x;
        element.PositionOffset_Y = offset.y;
        return ref builder;
    }

    /// <summary>
    /// Sets the raw values of the element's position offset without any manipulation
    /// </summary>
    /// <remarks>In pixels.</remarks>
    public static ref readonly SleekElementBuilder<TSleekElement> WithRawOffsetPixels<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder, float offsetX, float offsetY) where TSleekElement : class
    {
        ISleekElement element = (ISleekElement)builder.Element;

        element.PositionOffset_X = offsetX;
        element.PositionOffset_Y = offsetY;
        return ref builder;
    }

    /// <summary>
    /// Sets the raw values of the element's position offset without any manipulation
    /// </summary>
    /// <remarks>In pixels.</remarks>
    public static ref readonly SleekElementBuilder<TSleekElement> AddRawPositionPixels<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder, Vector2 offset) where TSleekElement : class
    {
        ISleekElement element = (ISleekElement)builder.Element;

        element.PositionOffset_X += offset.x;
        element.PositionOffset_Y += offset.y;
        return ref builder;
    }

    /// <summary>
    /// Sets the raw values of the element's position offset without any manipulation
    /// </summary>
    /// <remarks>In pixels.</remarks>
    public static ref readonly SleekElementBuilder<TSleekElement> AddRawPositionPixels<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder, float offsetX, float offsetY) where TSleekElement : class
    {
        ISleekElement element = (ISleekElement)builder.Element;

        element.PositionOffset_X += offsetX;
        element.PositionOffset_Y += offsetY;
        return ref builder;
    }

    /// <summary>
    /// Sets the raw values of the element's size without any manipulation
    /// </summary>
    /// <remarks>In pixels.</remarks>
    public static ref readonly SleekElementBuilder<TSleekElement> WithRawSizePixels<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder, Vector2 size) where TSleekElement : class
    {
        ISleekElement element = (ISleekElement)builder.Element;

        element.SizeOffset_X = size.x;
        element.SizeOffset_Y = size.y;
        return ref builder;
    }

    /// <summary>
    /// Sets the raw values of the element's size without any manipulation
    /// </summary>
    /// <remarks>In pixels.</remarks>
    public static ref readonly SleekElementBuilder<TSleekElement> WithRawSizePixels<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder, float sizeX, float sizeY) where TSleekElement : class
    {
        ISleekElement element = (ISleekElement)builder.Element;

        element.SizeOffset_X = sizeX;
        element.SizeOffset_Y = sizeY;
        return ref builder;
    }

    /// <summary>
    /// Sets the raw values of the element's position scale without any manipulation
    /// </summary>
    /// <remarks>Normalized value usually in [0, 1].</remarks>
    public static ref readonly SleekElementBuilder<TSleekElement> WithRawPositionScale<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder, Vector2 scale) where TSleekElement : class
    {
        ISleekElement element = (ISleekElement)builder.Element;

        element.PositionScale_X = scale.x;
        element.PositionScale_Y = scale.y;
        return ref builder;
    }

    /// <summary>
    /// Sets the raw values of the element's position scale without any manipulation
    /// </summary>
    /// <remarks>Normalized value usually in [0, 1].</remarks>
    public static ref readonly SleekElementBuilder<TSleekElement> WithRawPositionScale<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder, float scaleX, float scaleY) where TSleekElement : class
    {
        ISleekElement element = (ISleekElement)builder.Element;

        element.PositionScale_X = scaleX;
        element.PositionScale_Y = scaleY;
        return ref builder;
    }

    /// <summary>
    /// Sets the raw values of the element's position scale without any manipulation
    /// </summary>
    /// <remarks>Normalized value usually in [0, 1].</remarks>
    public static ref readonly SleekElementBuilder<TSleekElement> AddRawPositionScale<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder, Vector2 scale) where TSleekElement : class
    {
        ISleekElement element = (ISleekElement)builder.Element;

        element.PositionScale_X += scale.x;
        element.PositionScale_Y += scale.y;
        return ref builder;
    }

    /// <summary>
    /// Sets the raw values of the element's position scale without any manipulation
    /// </summary>
    /// <remarks>Normalized value usually in [0, 1].</remarks>
    public static ref readonly SleekElementBuilder<TSleekElement> AddRawPositionScale<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder, float scaleX, float scaleY) where TSleekElement : class
    {
        ISleekElement element = (ISleekElement)builder.Element;

        element.PositionScale_X += scaleX;
        element.PositionScale_Y += scaleY;
        return ref builder;
    }

    /// <summary>
    /// Sets the raw values of the element's position scale without any manipulation
    /// </summary>
    /// <remarks>Normalized value usually in [0, 1].</remarks>
    public static ref readonly SleekElementBuilder<TSleekElement> WithRawSizeScale<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder, Vector2 scale) where TSleekElement : class
    {
        ISleekElement element = (ISleekElement)builder.Element;

        element.SizeScale_X = scale.x;
        element.SizeScale_Y = scale.y;
        return ref builder;
    }

    /// <summary>
    /// Sets the raw values of the element's position scale without any manipulation
    /// </summary>
    /// <remarks>Normalized value usually in [0, 1].</remarks>
    public static ref readonly SleekElementBuilder<TSleekElement> WithRawSizeScale<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder, float scaleX, float scaleY) where TSleekElement : class
    {
        ISleekElement element = (ISleekElement)builder.Element;

        element.SizeScale_X = scaleX;
        element.SizeScale_Y = scaleY;
        return ref builder;
    }

    /// <summary>
    /// Changes the absolute center position of the element. This is in pixels.
    /// </summary>
    /// <remarks>Default value: (0.0f, 0.0f).</remarks>
    /// <param name="offsetTowards">Where to offset towards (relative to parent's bounding box).</param>
    /// <exception cref="ArgumentOutOfRangeException">Anchor was not a valid value.</exception>
    public static ref readonly SleekElementBuilder<TSleekElement> WithPositionPixels<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder, Vector2 offset, SleekPositionOffsetAnchor offsetTowards = SleekPositionOffsetAnchor.Inwards) where TSleekElement : class
        => ref builder.WithPositionPixels(offset.x, offset.y, offsetTowards);

    /// <summary>
    /// Changes the absolute center position of the element. This is in pixels.
    /// </summary>
    /// <remarks>Default value: (0.0f, 0.0f).</remarks>
    /// <param name="offsetTowards">Where to offset towards (relative to parent's bounding box).</param>
    /// <exception cref="ArgumentOutOfRangeException">Anchor was not a valid value.</exception>
    public static ref readonly SleekElementBuilder<TSleekElement> WithPositionPixels<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder, float offsetX, float offsetY, SleekPositionOffsetAnchor offsetTowards = SleekPositionOffsetAnchor.Inwards) where TSleekElement : class
    {
        ISleekElement element = (ISleekElement)builder.Element;

        bool top = offsetTowards is SleekPositionOffsetAnchor.TopCenter or SleekPositionOffsetAnchor.TopRight or SleekPositionOffsetAnchor.TopLeft or SleekPositionOffsetAnchor.TopOutwardsCenter,
            bottom = offsetTowards is SleekPositionOffsetAnchor.BottomCenter or SleekPositionOffsetAnchor.BottomRight or SleekPositionOffsetAnchor.BottomLeft or SleekPositionOffsetAnchor.BottomOutwardsCenter,
            left = offsetTowards is SleekPositionOffsetAnchor.LeftCenter or SleekPositionOffsetAnchor.TopLeft or SleekPositionOffsetAnchor.BottomLeft or SleekPositionOffsetAnchor.LeftOutwardsCenter,
            right = offsetTowards is SleekPositionOffsetAnchor.RightCenter or SleekPositionOffsetAnchor.TopRight or SleekPositionOffsetAnchor.BottomRight or SleekPositionOffsetAnchor.RightOutwardsCenter;

        int outwardsScale = offsetTowards is SleekPositionOffsetAnchor.Outwards or SleekPositionOffsetAnchor.BottomOutwardsCenter
            or SleekPositionOffsetAnchor.TopOutwardsCenter or SleekPositionOffsetAnchor.LeftOutwardsCenter
            or SleekPositionOffsetAnchor.RightOutwardsCenter ? -1 : 1;

        float x, y;
        if (!top && !bottom && !left && !right)
        {
            if (offsetTowards != SleekPositionOffsetAnchor.Inwards && outwardsScale != -1)
                throw new ArgumentOutOfRangeException(nameof(offsetTowards));

            int xNorm = (element.PositionScale_X + element.SizeScale_X / 2f).RangeSign() * outwardsScale;
            int yNorm = (element.PositionScale_Y + element.SizeScale_Y / 2f).RangeSign() * outwardsScale;

            x = xNorm * -offsetX;
            y = yNorm * -offsetY;
        }
        else
        {
            if (bottom)
                y = offsetY;
            else if (top)
                y = -offsetY;
            else
            {
                int yNorm = (element.PositionScale_Y + element.SizeScale_Y / 2f).RangeSign() * outwardsScale;
                y = yNorm * offsetY;
            }

            if (right)
                x = offsetX;
            else if (left)
                x = -offsetX;
            else
            {
                int xNorm = (element.PositionScale_X + element.SizeScale_X / 2f).RangeSign() * outwardsScale;
                x = xNorm * -offsetX;
            }
        }

        element.PositionOffset_X = x - element.SizeOffset_X / 2f;
        element.PositionOffset_Y = y - element.SizeOffset_Y / 2f;

        return ref builder;
    }

    /// <summary>
    /// Changes the absolute center position of the element. This is a normalized value in [0, 1].
    /// </summary>
    /// <remarks>Default value: (0.0f, 0.0f).</remarks>
    /// <param name="offsetTowards">Where to offset towards (relative to parent's bounding box).</param>
    /// <exception cref="ArgumentOutOfRangeException">Anchor was not a valid value.</exception>
    public static ref readonly SleekElementBuilder<TSleekElement> WithPositionScale<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder, Vector2 offset, SleekPositionOffsetAnchor offsetTowards = SleekPositionOffsetAnchor.Inwards) where TSleekElement : class
        => ref builder.WithPositionScale(offset.x, offset.y, offsetTowards);

    /// <summary>
    /// Changes the absolute center position of the element. This is a normalized value in [0, 1].
    /// </summary>
    /// <remarks>Default value: (0.0f, 0.0f).</remarks>
    /// <param name="offsetTowards">Where to offset towards (relative to parent's bounding box).</param>
    /// <exception cref="ArgumentOutOfRangeException">Anchor was not a valid value.</exception>
    public static ref readonly SleekElementBuilder<TSleekElement> WithPositionScale<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder, float offsetX, float offsetY, SleekPositionOffsetAnchor offsetTowards = SleekPositionOffsetAnchor.Inwards) where TSleekElement : class
    {
        ISleekElement element = (ISleekElement)builder.Element;

        bool top = offsetTowards is SleekPositionOffsetAnchor.TopCenter or SleekPositionOffsetAnchor.TopRight or SleekPositionOffsetAnchor.TopLeft or SleekPositionOffsetAnchor.TopOutwardsCenter,
            bottom = offsetTowards is SleekPositionOffsetAnchor.BottomCenter or SleekPositionOffsetAnchor.BottomRight or SleekPositionOffsetAnchor.BottomLeft or SleekPositionOffsetAnchor.BottomOutwardsCenter,
            left = offsetTowards is SleekPositionOffsetAnchor.LeftCenter or SleekPositionOffsetAnchor.TopLeft or SleekPositionOffsetAnchor.BottomLeft or SleekPositionOffsetAnchor.LeftOutwardsCenter,
            right = offsetTowards is SleekPositionOffsetAnchor.RightCenter or SleekPositionOffsetAnchor.TopRight or SleekPositionOffsetAnchor.BottomRight or SleekPositionOffsetAnchor.RightOutwardsCenter;

        int outwardsScale = offsetTowards is SleekPositionOffsetAnchor.Outwards or SleekPositionOffsetAnchor.BottomOutwardsCenter
            or SleekPositionOffsetAnchor.TopOutwardsCenter or SleekPositionOffsetAnchor.LeftOutwardsCenter
            or SleekPositionOffsetAnchor.RightOutwardsCenter ? -1 : 1;

        float x, y;
        if (!top && !bottom && !left && !right)
        {
            if (offsetTowards != SleekPositionOffsetAnchor.Inwards && outwardsScale != -1)
                throw new ArgumentOutOfRangeException(nameof(offsetTowards));

            int xNorm = (element.PositionScale_X + element.SizeScale_X / 2f).RangeSign() * outwardsScale;
            int yNorm = (element.PositionScale_Y + element.SizeScale_Y / 2f).RangeSign() * outwardsScale;

            x = xNorm * -offsetX + ((float)xNorm).ToGameScale();
            y = yNorm * -offsetY + ((float)yNorm).ToGameScale();
        }
        else
        {
            if (bottom)
                y = offsetY;
            else if (top)
                y = -offsetY;
            else
            {
                int yNorm = (element.PositionScale_Y + element.SizeScale_Y / 2f).RangeSign() * outwardsScale;
                y = yNorm * -offsetY + ((float)yNorm).ToGameScale();
            }

            if (right)
                x = offsetX;
            else if (left)
                x = -offsetX;
            else
            {
                int xNorm = (element.PositionScale_X + element.SizeScale_X / 2f).RangeSign() * outwardsScale;
                x = xNorm * -offsetX + ((float)xNorm).ToGameScale();
            }
        }

        element.PositionScale_X = x - element.SizeScale_X / 2f;
        element.PositionScale_Y = y - element.SizeScale_Y / 2f;

        return ref builder;
    }

    /// <summary>
    /// Changes the absolute position of the element. This is a normalized value in [-1, 1] following the default XY plane layout.
    /// </summary>
    /// <remarks>Default value: (-1f, 1f) (top left).</remarks>
    public static ref readonly SleekElementBuilder<TSleekElement> WithNormalizedPosition<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder, Vector2 normalizedPosition) where TSleekElement : class
        => ref builder.WithNormalizedPosition(normalizedPosition.x, normalizedPosition.y);

    /// <summary>
    /// Changes the absolute position of the element. This is a normalized value in [-1, 1] following the default XY plane layout.
    /// </summary>
    /// <remarks>Default value: (-1f, 1f) (top left).</remarks>
    public static ref readonly SleekElementBuilder<TSleekElement> WithNormalizedPosition<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder, float normalizedPositionX, float normalizedPositionY) where TSleekElement : class
    {
        ISleekElement element = (ISleekElement)builder.Element;

        element.PositionScale_X = normalizedPositionX.ToGameScale();
        element.PositionScale_Y = (-normalizedPositionY).ToGameScale();
        return ref builder;
    }

    /// <summary>
    /// Changes the absolute size of the element. This is in pixels.
    /// </summary>
    /// <remarks>Default value: (0.0f, 0.0f).</remarks>
    /// <param name="anchor">Where to scale from (relative to the current scale anchor).</param>
    /// <exception cref="ArgumentOutOfRangeException">Anchor was not a valid value.</exception>
    public static ref readonly SleekElementBuilder<TSleekElement> WithSizePixels<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder, Vector3 size, SleekResizeAnchor anchor = SleekResizeAnchor.Auto) where TSleekElement : class
        => ref builder.WithSizePixels(size.x, size.y, anchor);

    /// <summary>
    /// Changes the absolute size of the element. This is in pixels.
    /// </summary>
    /// <remarks>Default value: (0.0f, 0.0f).</remarks>
    /// <param name="anchor">Where to scale from (relative to the current scale anchor).</param>
    /// <exception cref="ArgumentOutOfRangeException">Anchor was not a valid value.</exception>
    public static ref readonly SleekElementBuilder<TSleekElement> WithSizePixels<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder, float sizeX, float sizeY, SleekResizeAnchor anchor = SleekResizeAnchor.Auto) where TSleekElement : class
    {
        ISleekElement element = (ISleekElement)builder.Element;

        if (anchor == SleekResizeAnchor.Auto)
            anchor = GetResizeAnchorFromAuto(element);

        bool top = anchor is SleekResizeAnchor.TopCenter or SleekResizeAnchor.TopRight or SleekResizeAnchor.TopLeft,
             bottom = anchor is SleekResizeAnchor.BottomCenter or SleekResizeAnchor.BottomRight or SleekResizeAnchor.BottomLeft,
             left = anchor is SleekResizeAnchor.LeftCenter or SleekResizeAnchor.TopLeft or SleekResizeAnchor.BottomLeft,
             right = anchor is SleekResizeAnchor.RightCenter or SleekResizeAnchor.TopRight or SleekResizeAnchor.BottomRight;

        if (!top && !bottom && !left && !right)
        {
            if (anchor != SleekResizeAnchor.Center)
                throw new ArgumentOutOfRangeException(nameof(anchor));

            float cx = element.PositionOffset_X + element.SizeOffset_X / 2f;
            float cy = element.PositionOffset_Y + element.SizeOffset_Y / 2f;

            element.PositionOffset_X = cx - sizeX / 2f;
            element.PositionOffset_Y = cy - sizeY / 2f;
        }
        else
        {
            if (bottom)
                element.PositionOffset_Y -= sizeY - element.SizeOffset_Y;
            else if (!top)
            {
                float cy = element.PositionOffset_Y + element.SizeOffset_Y / 2f;
                element.PositionOffset_Y = cy - sizeY / 2f;
            }

            if (right)
                element.PositionOffset_X -= sizeX - element.SizeOffset_X;
            else if (!left)
            {
                float cx = element.PositionOffset_X + element.SizeOffset_X / 2f;
                element.PositionOffset_X = cx - sizeX / 2f;
            }
        }

        element.SizeOffset_X = sizeX;
        element.SizeOffset_Y = sizeY;

        return ref builder;
    }

    /// <summary>
    /// Changes the normalized size of the element. This is usually in [0, 1].
    /// </summary>
    /// <remarks>Default value: (0.0f, 0.0f).</remarks>
    /// <param name="anchor">Where to scale from (relative to the current scale anchor).</param>
    /// <exception cref="ArgumentOutOfRangeException">Anchor was not a valid value.</exception>
    public static ref readonly SleekElementBuilder<TSleekElement> WithSizeScale<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder, Vector3 scale, SleekResizeAnchor anchor = SleekResizeAnchor.Auto) where TSleekElement : class
        => ref builder.WithSizeScale(scale.x, scale.y, anchor);

    /// <summary>
    /// Changes the normalized size of the element. This is usually in [0, 1].
    /// </summary>
    /// <remarks>Default value: (0.0f, 0.0f).</remarks>
    /// <param name="anchor">Where to scale from (relative to the current scale anchor).</param>
    /// <exception cref="ArgumentOutOfRangeException">Anchor was not a valid value.</exception>
    public static ref readonly SleekElementBuilder<TSleekElement> WithSizeScale<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder, float scaleX, float scaleY, SleekResizeAnchor anchor = SleekResizeAnchor.Auto) where TSleekElement : class
    {
        ISleekElement element = (ISleekElement)builder.Element;

        if (anchor == SleekResizeAnchor.Auto)
            anchor = GetResizeAnchorFromAuto(element);

        bool top = anchor is SleekResizeAnchor.TopCenter or SleekResizeAnchor.TopRight or SleekResizeAnchor.TopLeft,
             bottom = anchor is SleekResizeAnchor.BottomCenter or SleekResizeAnchor.BottomRight or SleekResizeAnchor.BottomLeft,
             left = anchor is SleekResizeAnchor.LeftCenter or SleekResizeAnchor.TopLeft or SleekResizeAnchor.BottomLeft,
             right = anchor is SleekResizeAnchor.RightCenter or SleekResizeAnchor.TopRight or SleekResizeAnchor.BottomRight;

        if (!top && !bottom && !left && !right)
        {
            if (anchor != SleekResizeAnchor.Center)
                throw new ArgumentOutOfRangeException(nameof(anchor));

            float cx = element.PositionScale_X + element.SizeScale_X / 2f;
            float cy = element.PositionScale_Y + element.SizeScale_Y / 2f;
            element.PositionScale_X = cx - scaleX / 2f;
            element.PositionScale_Y = cy - scaleY / 2f;
        }
        else
        {
            if (bottom)
                element.PositionScale_Y -= scaleY - element.SizeScale_Y;
            else if (!top)
            {
                float cy = element.PositionScale_Y + element.SizeScale_Y / 2f;
                element.PositionScale_Y = cy - scaleY / 2f;
            }

            if (right)
                element.PositionScale_X -= scaleX - element.SizeScale_X;
            else if (!left)
            {
                float cx = element.PositionScale_X + element.SizeScale_X / 2f;
                element.PositionScale_X = cx - scaleX / 2f;
            }
        }

        element.SizeScale_X = scaleX;
        element.SizeScale_Y = scaleY;

        return ref builder;
    }
    private static SleekResizeAnchor GetResizeAnchorFromAuto(ISleekElement element)
    {
        int xNorm = (element.PositionScale_X + element.SizeScale_X / 2f).RangeSign();
        int yNorm = (element.PositionScale_Y + element.SizeScale_Y / 2f).RangeSign();

        return (SleekResizeAnchor)(xNorm + 1 + (yNorm + 1) * 3);
    }

    /// <summary>
    /// Add a side label to the element.
    /// </summary>
    /// <param name="text">Text to display on the label.</param>
    /// <param name="side">Which side of the element to put the label on.</param>
    /// <param name="configureLabelAction">Optional configuration method.</param>
    public static ref readonly SleekElementBuilder<TSleekElement> AddLabel<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder, string text, ESleekSide side, Action<ISleekLabel>? configureLabelAction = null) where TSleekElement : class
    {
        ISleekElement element = (ISleekElement)builder.Element;

        if (element.SideLabel != null)
            element.RemoveChild(element.SideLabel);

        element.AddLabel(text, side);
        configureLabelAction?.Invoke(element.SideLabel!);
        return ref builder;
    }

    /// <summary>
    /// Add a side label to the element.
    /// </summary>
    /// <param name="text">Text to display on the label.</param>
    /// <param name="textColor">Color of the label's text.</param>
    /// <param name="side">Which side of the element to put the label on.</param>
    /// <param name="configureLabelAction">Optional configuration method.</param>
    public static ref readonly SleekElementBuilder<TSleekElement> AddLabel<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder, string text, Color textColor, ESleekSide side, Action<ISleekLabel>? configureLabelAction = null) where TSleekElement : class
    {
        ISleekElement element = (ISleekElement)builder.Element;

        if (element.SideLabel != null)
            element.RemoveChild(element.SideLabel);

        element.AddLabel(text, textColor, side);
        configureLabelAction?.Invoke(element.SideLabel!);
        return ref builder;
    }

    /// <summary>
    /// Further configure this element (without using the fluent API).
    /// </summary>
    public static ref readonly SleekElementBuilder<TSleekElement> Configure<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder, Action<TSleekElement> configureAction) where TSleekElement : class
    {
        configureAction?.Invoke(builder.Element);
        return ref builder;
    }

    /// <summary>
    /// Sets <see cref="ISleekElement.IsVisible"/> to <see langword="true"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (enabled).</remarks>
    public static ref readonly SleekElementBuilder<TSleekElement> Enable<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder) where TSleekElement : class
        => ref builder.WithIsVisible(true);

    /// <summary>
    /// Sets <see cref="ISleekElement.IsVisible"/> to <see langword="false"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (enabled).</remarks>
    public static ref readonly SleekElementBuilder<TSleekElement> Hide<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder) where TSleekElement : class
        => ref builder.WithIsVisible(false);

    /// <summary>
    /// Sets <see cref="ISleekElement.IsVisible"/> to <paramref name="isVisible"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (enabled).</remarks>
    public static ref readonly SleekElementBuilder<TSleekElement> WithIsVisible<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder, bool isVisible) where TSleekElement : class
    {
        builder.SleekElement.IsVisible = isVisible;
        return ref builder;
    }

    /// <summary>
    /// Sets <see cref="ISleekElement.ChildAutoLayoutPadding"/> to <paramref name="childAutoLayoutPadding"/>.
    /// </summary>
    /// <remarks>Default value: 0.</remarks>
    public static ref readonly SleekElementBuilder<TSleekElement> WithChildAutoLayoutPadding<TSleekElement>(this in SleekElementBuilder<TSleekElement> builder, float childAutoLayoutPadding) where TSleekElement : class
    {
        builder.SleekElement.ChildAutoLayoutPadding = childAutoLayoutPadding;
        return ref builder;
    }

    /// <summary>
    /// Sets the background color of an <see cref="ISleekBox"/>.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekBox> WithBackgroundColor(this in SleekElementBuilder<ISleekBox> builder,
        [ValueProvider("SDG.Unturned.ESleekTint"), ValueProvider("UnityEngine.Color")] SleekColor backgroundColor)
    {
        builder.Element.BackgroundColor = backgroundColor;
        return ref builder;
    }


    /// <summary>
    /// Sets the background color of an <see cref="ISleekButton"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekTint.BACKGROUND"/> (from settings, default value <c>RGB01(0.9, 0.9, 0.9)</c>).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekButton> WithBackgroundColor(this in SleekElementBuilder<ISleekButton> builder,
        [ValueProvider("SDG.Unturned.ESleekTint"), ValueProvider("UnityEngine.Color")] SleekColor backgroundColor)
    {
        builder.Element.BackgroundColor = backgroundColor;
        return ref builder;
    }

    /// <summary>
    /// Sets <see cref="ISleekButton.IsClickable"/> to <see langword="true"/> on an <see cref="ISleekButton"/>.
    /// <para>Interactability defines if the user can interact with the button (click it).</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (interactable).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekButton> Interactable(this in SleekElementBuilder<ISleekButton> builder)
        => ref builder.WithIsInteractable(true);

    /// <summary>
    /// Sets <see cref="ISleekButton.IsClickable"/> to <see langword="false"/> on an <see cref="ISleekButton"/>.
    /// <para>Interactability defines if the user can interact with the button (click it).</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (interactable).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekButton> NotInteractable(this in SleekElementBuilder<ISleekButton> builder)
        => ref builder.WithIsInteractable(false);

    /// <summary>
    /// Sets <see cref="ISleekButton.IsClickable"/> to <paramref name="isClickable"/> on an <see cref="ISleekButton"/>.
    /// <para>Interactability defines if the user can interact with the button (click it).</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (interactable).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekButton> WithIsInteractable(this in SleekElementBuilder<ISleekButton> builder, bool isClickable)
    {
        if (builder.Element.IsClickable != isClickable)
            builder.Element.IsClickable = isClickable;
        return ref builder;
    }

    /// <summary>
    /// Sets <see cref="ISleekButton.IsRaycastTarget"/> to <see langword="true"/> on an <see cref="ISleekButton"/>.
    /// <para>Should the button view consume clicks?</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (raycast target).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekButton> RaycastTarget(this in SleekElementBuilder<ISleekButton> builder)
        => ref builder.WithIsRaycastTarget(true);

    /// <summary>
    /// Sets <see cref="ISleekButton.IsRaycastTarget"/> to <see langword="false"/> on an <see cref="ISleekButton"/>.
    /// <para>Should the button view consume clicks?</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (raycast target).</remarks>v
    public static ref readonly SleekElementBuilder<ISleekButton> RaycastIgnored(this in SleekElementBuilder<ISleekButton> builder)
        => ref builder.WithIsRaycastTarget(false);

    /// <summary>
    /// Sets <see cref="ISleekButton.IsRaycastTarget"/> to <paramref name="isRaycastTarget"/> on an <see cref="ISleekButton"/>.
    /// <para>Should the button view consume clicks?</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (raycast target).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekButton> WithIsRaycastTarget(this in SleekElementBuilder<ISleekButton> builder, bool isRaycastTarget)
    {
        if (builder.Element.IsRaycastTarget != isRaycastTarget)
            builder.Element.IsRaycastTarget = isRaycastTarget;
        return ref builder;
    }

    /// <summary>
    /// Adds a callback for when an <see cref="ISleekButton"/> is left clicked.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekButton> WhenLeftClicked(this in SleekElementBuilder<ISleekButton> builder, ClickedButton callback)
    {
        builder.Element.OnClicked += callback;
        return ref builder;
    }

    /// <summary>
    /// Adds a callback for when an <see cref="ISleekButton"/> is right clicked.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekButton> WhenRightClicked(this in SleekElementBuilder<ISleekButton> builder, ClickedButton callback)
    {
        builder.Element.OnRightClicked += callback;
        return ref builder;
    }

    /// <summary>
    /// Adds a callback for when an <see cref="ISleekButton"/> is left or right clicked.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekButton> WhenAnyClicked(this in SleekElementBuilder<ISleekButton> builder, ClickedButton callback)
    {
        builder.Element.OnClicked += callback;
        builder.Element.OnRightClicked += callback;
        return ref builder;
    }


    /// <summary>
    /// Sets the constraint mode of an <see cref="ISleekConstraintFrame"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekConstraint.NONE"/>.</remarks>
    /// <exception cref="InvalidOperationException">Constraint was set more than once.</exception>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekConstraintFrame> WithConstraintMode(this in SleekElementBuilder<ISleekConstraintFrame> builder, ESleekConstraint constraint)
    {
        if (builder.Element.Constraint == constraint)
            return ref builder;

        if (builder.Element.Constraint != ESleekConstraint.NONE)
            throw new InvalidOperationException("Constraint can not be set more than once.");

        if (builder.Element.Constraint != constraint)
            builder.Element.Constraint = constraint;
        return ref builder;
    }

    /// <summary>
    /// Sets the aspect ratio of an <see cref="ISleekConstraintFrame"/> in <see cref="ESleekConstraint.FitInParent"/> constraint mode.
    /// </summary>
    /// <param name="aspectRatio">Aspect ratio to scale the content to. Calculated by dividing the width by the height.</param>
    /// <remarks>Default value: 1.0f.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekConstraintFrame> WithAspectRatio(this in SleekElementBuilder<ISleekConstraintFrame> builder, float aspectRatio)
    {
        if (builder.Element.AspectRatio != aspectRatio)
            builder.Element.AspectRatio = aspectRatio;
        return ref builder;
    }


    /// <summary>
    /// Sets the background color of an <see cref="ISleekField"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekTint.BACKGROUND"/> (from settings, default value <c>RGB01(0.9, 0.9, 0.9)</c>).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekField> WithBackgroundColor(this in SleekElementBuilder<ISleekField> builder,
        [ValueProvider("SDG.Unturned.ESleekTint"), ValueProvider("UnityEngine.Color")] SleekColor backgroundColor)
    {
        builder.Element.BackgroundColor = backgroundColor;
        return ref builder;
    }

    /// <summary>
    /// Sets <see cref="ISleekField.IsPasswordField"/> to <see langword="true"/> on an <see cref="ISleekField"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (not password field).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekField> PasswordField(this in SleekElementBuilder<ISleekField> builder)
        => ref builder.WithIsPasswordField(true);

    /// <summary>
    /// Sets <see cref="ISleekField.IsPasswordField"/> to <see langword="false"/> on an <see cref="ISleekField"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (not password field).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekField> NotPasswordField(this in SleekElementBuilder<ISleekField> builder)
        => ref builder.WithIsPasswordField(false);

    /// <summary>
    /// Sets <see cref="ISleekField.IsPasswordField"/> to <paramref name="isPasswordField"/> on an <see cref="ISleekField"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (not password field).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekField> WithIsPasswordField(this in SleekElementBuilder<ISleekField> builder, bool isPasswordField)
    {
        if (builder.Element.IsPasswordField != isPasswordField)
            builder.Element.IsPasswordField = isPasswordField;
        return ref builder;
    }


    /// <summary>
    /// Sets <see cref="ISleekField.IsMultiline"/> to <see langword="true"/> on an <see cref="ISleekField"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (singleline).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekField> Multiline(this in SleekElementBuilder<ISleekField> builder)
        => ref builder.WithIsMultiline(true);

    /// <summary>
    /// Sets <see cref="ISleekField.IsMultiline"/> to <see langword="false"/> on an <see cref="ISleekField"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (singleline).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekField> Singleline(this in SleekElementBuilder<ISleekField> builder)
        => ref builder.WithIsMultiline(false);

    /// <summary>
    /// Sets <see cref="ISleekField.IsMultiline"/> to <paramref name="isMultiline"/> on an <see cref="ISleekField"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="false"/>. (singleline)</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekField> WithIsMultiline(this in SleekElementBuilder<ISleekField> builder, bool isMultiline)
    {
        if (builder.Element.IsMultiline != isMultiline)
            builder.Element.IsMultiline = isMultiline;
        return ref builder;
    }

    /// <summary>
    /// Sets the max length in characters of an <see cref="ISleekField"/>.
    /// </summary>
    /// <param name="maxLength">Maximum amount of characters in this field.</param>
    /// <remarks>Default value: 100.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekField> WithMaxLength(this in SleekElementBuilder<ISleekField> builder, int maxLength)
    {
        if (builder.Element.MaxLength != maxLength)
            builder.Element.MaxLength = maxLength;
        return ref builder;
    }

    /// <summary>
    /// Sets the placeholder text of an <see cref="ISleekField"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="null"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekField> WithPlaceholder(this in SleekElementBuilder<ISleekField> builder, string placeholderText)
    {
        if (!ReferenceEquals(builder.Element.PlaceholderText, placeholderText))
            builder.Element.PlaceholderText = placeholderText;
        return ref builder;
    }

    /// <summary>
    /// Adds a callback for when text is submitted in an <see cref="ISleekField"/>.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekField> WhenTextSubmitted(this in SleekElementBuilder<ISleekField> builder, Entered callback)
    {
        builder.Element.OnTextSubmitted += callback;
        return ref builder;
    }

    /// <summary>
    /// Adds a callback for when the text in an <see cref="ISleekField"/> is modified.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekField> WhenTextTyped(this in SleekElementBuilder<ISleekField> builder, Typed callback)
    {
        builder.Element.OnTextChanged += callback;
        return ref builder;
    }

    /// <summary>
    /// Adds a callback for when the focus is lost for an <see cref="ISleekField"/>.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekField> WhenFocusLeft(this in SleekElementBuilder<ISleekField> builder, Escaped callback)
    {
        builder.Element.OnTextEscaped += callback;
        return ref builder;
    }


    /// <summary>
    /// Adds a callback for when a value is submitted in an <see cref="ISleekFloat32Field"/>.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekFloat32Field> WhenValueSubmitted(this in SleekElementBuilder<ISleekFloat32Field> builder, TypedSingle callback)
    {
        builder.Element.OnValueSubmitted += callback;
        return ref builder;
    }

    /// <summary>
    /// Adds a callback for when the value in an <see cref="ISleekFloat32Field"/> is modified.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekFloat32Field> WhenValueTyped(this in SleekElementBuilder<ISleekFloat32Field> builder, TypedSingle callback)
    {
        builder.Element.OnValueChanged += callback;
        return ref builder;
    }

    /// <summary>
    /// Sets the initial value (text) of an <see cref="ISleekFloat32Field"/>.
    /// </summary>
    /// <remarks>Default value: 0.0f.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekFloat32Field> WithInitialValue(this in SleekElementBuilder<ISleekFloat32Field> builder, float value)
    {
        if (builder.Element.Value != value)
            builder.Element.Value = value;
        return ref builder;
    }


    /// <summary>
    /// Adds a callback for when the value in an <see cref="ISleekFloat64Field"/> is modified.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekFloat64Field> WhenValueTyped(this in SleekElementBuilder<ISleekFloat64Field> builder, TypedDouble callback)
    {
        builder.Element.OnValueChanged += callback;
        return ref builder;
    }

    /// <summary>
    /// Sets the initial value (text) of an <see cref="ISleekFloat64Field"/>.
    /// </summary>
    /// <remarks>Default value: 0.0d.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekFloat64Field> WithInitialValue(this in SleekElementBuilder<ISleekFloat64Field> builder, double value)
    {
        if (builder.Element.Value != value)
            builder.Element.Value = value;
        return ref builder;
    }


    /// <summary>
    /// Sets the texture of an <see cref="ISleekImage"/>.
    /// </summary>
    /// <param name="shouldDestroyTexture">Should <see cref="UnityEngine.Object.Destroy(UnityEngine.Object)" /> be called on the texture when the element is destroyed.</param>
    /// <remarks>Default value: <see langword="null"/>.</remarks>
    /// <seealso cref="WithTexture(in SleekElementBuilder{ISleekImage},Bundle,string,bool)"/>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekImage> WithTexture(this in SleekElementBuilder<ISleekImage> builder, Texture? texture, bool shouldDestroyTexture)
    {
        if (texture is Texture2D t2d)
            builder.Element.SetTextureAndShouldDestroy(t2d, shouldDestroyTexture);
        else
        {
            builder.Element.Texture = texture;
            builder.Element.ShouldDestroyTexture = shouldDestroyTexture;
        }
        return ref builder;
    }

    /// <summary>
    /// Loads the texture of an <see cref="ISleekImage"/>.
    /// </summary>
    /// <param name="shouldDestroyTexture">Should <see cref="UnityEngine.Object.Destroy(UnityEngine.Object)" /> be called on the texture when the element is destroyed.</param>
    /// <exception cref="ArgumentException">Unable to find a <see cref="Texture2D"/> at <paramref name="path"/> in <paramref name="bundle"/>.</exception>
    /// <remarks>Default value: <see langword="null"/>.</remarks>
    /// <seealso cref="WithTexture(in SleekElementBuilder{ISleekImage},Texture,bool)"/>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekImage> WithTexture(this in SleekElementBuilder<ISleekImage> builder, Bundle bundle, string path, bool shouldDestroyTexture)
    {
        Texture2D? texture = bundle.load<Texture2D>(path);
        if (texture == null)
            throw new ArgumentException("Unable to find a texture in the provided bundle.", nameof(path));

        builder.Element.SetTextureAndShouldDestroy(texture, shouldDestroyTexture);
        return ref builder;
    }

    /// <summary>
    /// Sets the texture of an <see cref="ISleekImage"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="null"/>.</remarks>
    /// <seealso cref="WithTexture(in SleekElementBuilder{ISleekImage},Bundle,string)"/>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekImage> WithTexture(this in SleekElementBuilder<ISleekImage> builder, Texture? texture)
    {
        if (texture is Texture2D t2d)
            builder.Element.UpdateTexture(t2d);
        else
            builder.Element.Texture = texture;
        return ref builder;
    }

    /// <summary>
    /// Loads the texture of an <see cref="ISleekImage"/>.
    /// </summary>
    /// <exception cref="ArgumentException">Unable to find a <see cref="Texture2D"/> at <paramref name="path"/> in <paramref name="bundle"/>.</exception>
    /// <remarks>Default value: <see langword="null"/>.</remarks>
    /// <seealso cref="WithTexture(in SleekElementBuilder{ISleekImage},Texture)"/>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekImage> WithTexture(this in SleekElementBuilder<ISleekImage> builder, Bundle bundle, string path)
    {
        Texture2D? texture = bundle.load<Texture2D>(path);
        if (texture == null)
            throw new ArgumentException("Unable to find a texture in the provided bundle.", nameof(path));

        builder.Element.UpdateTexture(texture);
        return ref builder;
    }

    /// <summary>
    /// Sets the rotation angle of an <see cref="ISleekImage"/> and sets <see cref="ISleekImage.CanRotate"/> to <see langword="true"/>.
    /// </summary>
    /// <param name="angleDeg">Angle to render the image at in degrees.</param>
    /// <remarks>Default value: 0.0f (can't rotate).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekImage> WithRotationAngle(this in SleekElementBuilder<ISleekImage> builder, float angleDeg)
    {
        if (builder.Element.CanRotate && builder.Element.RotationAngle == angleDeg)
            return ref builder;

        builder.Element.RotationAngle = angleDeg;
        builder.Element.CanRotate = true;
        return ref builder;
    }

    /// <summary>
    /// Zeros the rotation angle of an <see cref="ISleekImage"/> and sets <see cref="ISleekImage.CanRotate"/> to <see langword="false"/>.
    /// </summary>
    /// <remarks>Default value: 0.0f (can't rotate).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekImage> WithNoRotation(this in SleekElementBuilder<ISleekImage> builder)
    {
        if (builder.Element is { CanRotate: false, RotationAngle: 0f })
            return ref builder;

        builder.Element.RotationAngle = 0f;
        builder.Element.CanRotate = false;
        return ref builder;
    }

    /// <summary>
    /// Sets the tint of an <see cref="ISleekImage"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekTint.NONE"/> (<see cref="Color.white"/>).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekImage> WithTintColor(this in SleekElementBuilder<ISleekImage> builder,
        [ValueProvider("SDG.Unturned.ESleekTint"), ValueProvider("UnityEngine.Color")] SleekColor tintColor)
    {
        builder.Element.TintColor = tintColor;
        return ref builder;
    }

    /// <summary>
    /// Sets <see cref="ISleekImage.ShouldDestroyTexture"/> to <see langword="true"/> on an <see cref="ISleekImage"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (keep texture alive).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekImage> DestroyTexture(this in SleekElementBuilder<ISleekImage> builder)
        => ref builder.WithShouldDestroyTexture(true);

    /// <summary>
    /// Sets <see cref="ISleekImage.ShouldDestroyTexture"/> to <see langword="false"/> on an <see cref="ISleekImage"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (keep texture alive).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekImage> KeepTextureAlive(this in SleekElementBuilder<ISleekImage> builder)
        => ref builder.WithShouldDestroyTexture(false);

    /// <summary>
    /// Sets <see cref="ISleekImage.ShouldDestroyTexture"/> to <paramref name="shouldDestroyTexture"/> on an <see cref="ISleekImage"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (keep texture alive).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekImage> WithShouldDestroyTexture(this in SleekElementBuilder<ISleekImage> builder, bool shouldDestroyTexture)
    {
        if (builder.Element.ShouldDestroyTexture != shouldDestroyTexture)
            builder.Element.ShouldDestroyTexture = shouldDestroyTexture;
        return ref builder;
    }

    /// <summary>
    /// Adds a callback for when an <see cref="ISleekImage"/> is left clicked.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekImage> WhenLeftClicked(this in SleekElementBuilder<ISleekImage> builder, Action callback)
    {
        builder.Element.OnClicked += callback;
        return ref builder;
    }

    /// <summary>
    /// Adds a callback for when an <see cref="ISleekImage"/> is right clicked.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekImage> WhenRightClicked(this in SleekElementBuilder<ISleekImage> builder, Action callback)
    {
        builder.Element.OnRightClicked += callback;
        return ref builder;
    }

    /// <summary>
    /// Adds a callback for when an <see cref="ISleekImage"/> is left or right clicked.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekImage> WhenAnyClicked(this in SleekElementBuilder<ISleekImage> builder, Action callback)
    {
        builder.Element.OnClicked += callback;
        builder.Element.OnRightClicked += callback;
        return ref builder;
    }


    /// <summary>
    /// Adds a callback for when the value in an <see cref="ISleekInt32Field"/> is modified.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekInt32Field> WhenValueTyped(this in SleekElementBuilder<ISleekInt32Field> builder, TypedInt32 callback)
    {
        builder.Element.OnValueChanged += callback;
        return ref builder;
    }

    /// <summary>
    /// Sets the initial value (text) of an <see cref="ISleekInt32Field"/>.
    /// </summary>
    /// <remarks>Default value: 0.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekInt32Field> WithInitialValue(this in SleekElementBuilder<ISleekInt32Field> builder, int value)
    {
        if (builder.Element.Value != value)
            builder.Element.Value = value;
        return ref builder;
    }


    /// <summary>
    /// Sets the text of an <see cref="ISleekLabel"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="null"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<TSleekLabel> WithText<TSleekLabel>(this in SleekElementBuilder<TSleekLabel> builder, string text) where TSleekLabel : class, ISleekLabel
    {
        if (!ReferenceEquals(builder.Element.Text, text))
            builder.Element.Text = text;
        return ref builder;
    }

    /// <summary>
    /// Sets the font style of an <see cref="ISleekLabel"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="FontStyle.Normal"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<TSleekLabel> WithFontStyle<TSleekLabel>(this in SleekElementBuilder<TSleekLabel> builder, FontStyle fontStyle) where TSleekLabel : class, ISleekLabel
    {
        if (builder.Element.FontStyle != fontStyle)
            builder.Element.FontStyle = fontStyle;
        return ref builder;
    }

    /// <summary>
    /// Adds bolding to an <see cref="ISleekLabel"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="FontStyle.Normal"/>. If the font style is already <see cref="FontStyle.Italic"/> it will be set to <see cref="FontStyle.BoldAndItalic"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<TSleekLabel> Bold<TSleekLabel>(this in SleekElementBuilder<TSleekLabel> builder) where TSleekLabel : class, ISleekLabel
    {
        FontStyle fontStyle = builder.Element.FontStyle == FontStyle.Italic ? FontStyle.BoldAndItalic : FontStyle.Bold;

        if (builder.Element.FontStyle != fontStyle)
            builder.Element.FontStyle = fontStyle;
        return ref builder;
    }

    /// <summary>
    /// Adds italics to an <see cref="ISleekLabel"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="FontStyle.Normal"/>. If the font style is already <see cref="FontStyle.Bold"/> it will be set to <see cref="FontStyle.BoldAndItalic"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<TSleekLabel> Italic<TSleekLabel>(this in SleekElementBuilder<TSleekLabel> builder) where TSleekLabel : class, ISleekLabel
    {
        FontStyle fontStyle = builder.Element.FontStyle == FontStyle.Bold ? FontStyle.BoldAndItalic : FontStyle.Italic;

        if (builder.Element.FontStyle != fontStyle)
            builder.Element.FontStyle = fontStyle;
        return ref builder;
    }

    /// <summary>
    /// Adds bold and italics to an <see cref="ISleekLabel"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="FontStyle.Normal"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<TSleekLabel> BoldAndItalic<TSleekLabel>(this in SleekElementBuilder<TSleekLabel> builder) where TSleekLabel : class, ISleekLabel
    {
        if (builder.Element.FontStyle != FontStyle.BoldAndItalic)
            builder.Element.FontStyle = FontStyle.BoldAndItalic;
        return ref builder;
    }

    /// <summary>
    /// Sets the text anchor of an <see cref="ISleekLabel"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="TextAnchor.MiddleCenter"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<TSleekLabel> WithTextAnchor<TSleekLabel>(this in SleekElementBuilder<TSleekLabel> builder, TextAnchor textAnchor) where TSleekLabel : class, ISleekLabel
    {
        if (builder.Element.TextAlignment != textAnchor)
            builder.Element.TextAlignment = textAnchor;
        return ref builder;
    }

    /// <summary>
    /// Sets the text anchor of an <see cref="ISleekLabel"/> in separate components.
    /// </summary>
    /// <remarks>Default values: <see cref="TextAlignment.Center"/>, <see cref="VerticalTextAlignment.Center"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<TSleekLabel> WithTextAnchor<TSleekLabel>(this in SleekElementBuilder<TSleekLabel> builder, TextAlignment horizontalAlignment, VerticalTextAlignment verticalAlignment) where TSleekLabel : class, ISleekLabel
    {
        TextAnchor textAnchor = (TextAnchor)((int)horizontalAlignment + (int)verticalAlignment * 3);
        if (builder.Element.TextAlignment != textAnchor)
            builder.Element.TextAlignment = textAnchor;
        return ref builder;
    }

    /// <summary>
    /// Sets the horizontal text alignment of an <see cref="ISleekLabel"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="TextAlignment.Center"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<TSleekLabel> WithHorizontalTextAlignment<TSleekLabel>(this in SleekElementBuilder<TSleekLabel> builder, TextAlignment textAlignment) where TSleekLabel : class, ISleekLabel
    {
        TextAnchor textAnchor = (TextAnchor)((int)builder.Element.TextAlignment / 3 + (int)textAlignment);
        if (builder.Element.TextAlignment != textAnchor)
            builder.Element.TextAlignment = textAnchor;
        return ref builder;
    }

    /// <summary>
    /// Sets the vertical text alignment of an <see cref="ISleekLabel"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="VerticalTextAlignment.Center"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<TSleekLabel> WithVerticalTextAlignment<TSleekLabel>(this in SleekElementBuilder<TSleekLabel> builder, VerticalTextAlignment textAlignment) where TSleekLabel : class, ISleekLabel
    {
        TextAnchor textAnchor = (TextAnchor)((int)builder.Element.TextAlignment % 3 + (int)textAlignment * 3);
        if (builder.Element.TextAlignment != textAnchor)
            builder.Element.TextAlignment = textAnchor;
        return ref builder;
    }

    /// <summary>
    /// Sets the font size preset of an <see cref="ISleekLabel"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekFontSize.Default"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<TSleekLabel> WithFontSize<TSleekLabel>(this in SleekElementBuilder<TSleekLabel> builder, ESleekFontSize fontSize) where TSleekLabel : class, ISleekLabel
    {
        if (builder.Element.FontSize != fontSize)
            builder.Element.FontSize = fontSize;
        return ref builder;
    }

    /// <summary>
    /// Sets the contrast context of an <see cref="ISleekLabel"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ETextContrastContext.Default"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<TSleekLabel> WithContrastContext<TSleekLabel>(this in SleekElementBuilder<TSleekLabel> builder, ETextContrastContext contrastContext) where TSleekLabel : class, ISleekLabel
    {
        if (builder.Element.TextContrastContext != contrastContext)
            builder.Element.TextContrastContext = contrastContext;
        return ref builder;
    }

    /// <summary>
    /// Sets the text color of an <see cref="ISleekLabel"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekTint.FONT"/> (from settings, default value <c>RGB01(0.9, 0.9, 0.9)</c>).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekLabel> WithTextColor(this in SleekElementBuilder<ISleekLabel> builder,
        [ValueProvider("SDG.Unturned.ESleekTint"), ValueProvider("UnityEngine.Color")] SleekColor textColor)
    {
        builder.Element.TextColor = textColor;
        return ref builder;
    }

    /// <summary>
    /// Sets the text color of an <see cref="ISleekButton"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekTint.FONT"/> (from settings, default value <c>RGB01(0.9, 0.9, 0.9)</c>).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekButton> WithTextColor(this in SleekElementBuilder<ISleekButton> builder,
        [ValueProvider("SDG.Unturned.ESleekTint"), ValueProvider("UnityEngine.Color")] SleekColor textColor)
    {
        builder.Element.TextColor = textColor;
        return ref builder;
    }

    /// <summary>
    /// Sets the text color of an <see cref="ISleekBox"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekTint.FONT"/> (from settings, default value <c>RGB01(0.9, 0.9, 0.9)</c>).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekBox> WithTextColor(this in SleekElementBuilder<ISleekBox> builder,
        [ValueProvider("SDG.Unturned.ESleekTint"), ValueProvider("UnityEngine.Color")] SleekColor textColor)
    {
        builder.Element.TextColor = textColor;
        return ref builder;
    }

    /// <summary>
    /// Sets the text color of an <see cref="ISleekField"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekTint.FONT"/> (from settings, default value <c>RGB01(0.9, 0.9, 0.9)</c>).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekField> WithTextColor(this in SleekElementBuilder<ISleekField> builder,
        [ValueProvider("SDG.Unturned.ESleekTint"), ValueProvider("UnityEngine.Color")] SleekColor textColor)
    {
        builder.Element.TextColor = textColor;
        return ref builder;
    }


    /// <summary>
    /// Sets the text color of an <see cref="ISleekNumericField"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekTint.FONT"/> (from settings, default value <c>RGB01(0.9, 0.9, 0.9)</c>).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<TSleekNumericField> WithTextColor<TSleekNumericField>(this in SleekElementBuilder<TSleekNumericField> builder,
        [ValueProvider("SDG.Unturned.ESleekTint"), ValueProvider("UnityEngine.Color")] SleekColor textColor) where TSleekNumericField : class, ISleekNumericField
    {
        builder.Element.TextColor = textColor;
        return ref builder;
    }

    /// <summary>
    /// Sets the background color of an <see cref="ISleekNumericField"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekTint.BACKGROUND"/> (from settings, default value <c>RGB01(0.9, 0.9, 0.9)</c>).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<TSleekNumericField> WithBackgroundColor<TSleekNumericField>(this in SleekElementBuilder<TSleekNumericField> builder,
        [ValueProvider("SDG.Unturned.ESleekTint"), ValueProvider("UnityEngine.Color")] SleekColor backgroundColor) where TSleekNumericField : class, ISleekNumericField
    {
        builder.Element.BackgroundColor = backgroundColor;
        return ref builder;
    }


    /// <summary>
    /// Sets <see cref="ISleekScrollView.ScaleContentToWidth"/> to <see langword="true"/> for an <see cref="ISleekScrollView"/>.
    /// <para>Should <see cref="ISleekScrollView.ContentScaleFactor" /> be applied in the x direction?</para>
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (don't scale content).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekScrollView> ScaleContentToWidth(this in SleekElementBuilder<ISleekScrollView> builder)
        => ref builder.WithShouldScaleContentToWidth(true);

    /// <summary>
    /// Sets <see cref="ISleekScrollView.ScaleContentToWidth"/> to <see langword="false"/> for an <see cref="ISleekScrollView"/>.
    /// <para>Should <see cref="ISleekScrollView.ContentScaleFactor" /> be applied in the x direction?</para>
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (don't scale content).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekScrollView> DoNotScaleContentToWidth(this in SleekElementBuilder<ISleekScrollView> builder)
        => ref builder.WithShouldScaleContentToWidth(false);

    /// <summary>
    /// Sets <see cref="ISleekScrollView.ScaleContentToWidth"/> to <paramref name="shouldScaleContentToWidth"/> for an <see cref="ISleekScrollView"/>.
    /// <para>Should <see cref="ISleekScrollView.ContentScaleFactor" /> be applied in the x direction?</para>
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (don't scale content).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekScrollView> WithShouldScaleContentToWidth(this in SleekElementBuilder<ISleekScrollView> builder, bool shouldScaleContentToWidth)
    {
        if (builder.Element.ScaleContentToWidth != shouldScaleContentToWidth)
            builder.Element.ScaleContentToWidth = shouldScaleContentToWidth;
        return ref builder;
    }

    /// <summary>
    /// Sets <see cref="ISleekScrollView.ScaleContentToHeight"/> to <see langword="true"/> for an <see cref="ISleekScrollView"/>.
    /// <para>Should <see cref="ISleekScrollView.ContentScaleFactor" /> be applied in the y direction?</para>
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (don't scale content).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekScrollView> ScaleContentToHeight(this in SleekElementBuilder<ISleekScrollView> builder)
        => ref builder.WithShouldScaleContentToHeight(true);

    /// <summary>
    /// Sets <see cref="ISleekScrollView.ScaleContentToHeight"/> to <see langword="false"/> for an <see cref="ISleekScrollView"/>.
    /// <para>Should <see cref="ISleekScrollView.ContentScaleFactor" /> be applied in the y direction?</para>
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (don't scale content).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekScrollView> DoNotScaleContentToHeight(this in SleekElementBuilder<ISleekScrollView> builder)
        => ref builder.WithShouldScaleContentToHeight(false);

    /// <summary>
    /// Sets <see cref="ISleekScrollView.ScaleContentToHeight"/> to <paramref name="verticalScrollbarVisibility"/> for an <see cref="ISleekScrollView"/>.
    /// <para>Should <see cref="ISleekScrollView.ContentScaleFactor" /> be applied in the y direction?</para>
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (don't scale content).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekScrollView> WithShouldScaleContentToHeight(this in SleekElementBuilder<ISleekScrollView> builder, bool verticalScrollbarVisibility)
    {
        if (builder.Element.ScaleContentToHeight != verticalScrollbarVisibility)
            builder.Element.ScaleContentToHeight = verticalScrollbarVisibility;
        return ref builder;
    }

    /// <summary>
    /// Sets the initial scale factor of an <see cref="ISleekScrollView"/>.
    /// <para>How zoomed in the content is.</para>
    /// </summary>
    /// <remarks>Default value: 0.0f.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekScrollView> WithInitialScaleFactor(this in SleekElementBuilder<ISleekScrollView> builder, float scaleFactor)
    {
        if (builder.Element.ContentScaleFactor != scaleFactor)
            builder.Element.ContentScaleFactor = scaleFactor;
        return ref builder;
    }

    /// <summary>
    /// Sets <see cref="ISleekScrollView.ReduceWidthWhenScrollbarVisible"/> to <see langword="true"/> for an <see cref="ISleekScrollView"/>.
    /// <para>The scrollbar being hidden will slightly increase the width of the scroll view.</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (reduce width).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekScrollView> ReduceWidthWhenScrollbarVisible(this in SleekElementBuilder<ISleekScrollView> builder)
        => ref builder.WithShouldScaleContentToHeight(true);

    /// <summary>
    /// Sets <see cref="ISleekScrollView.ReduceWidthWhenScrollbarVisible"/> to <see langword="false"/> for an <see cref="ISleekScrollView"/>.
    /// <para>The scrollbar being hidden will not change the width of the scroll view.</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (reduce width).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekScrollView> KeepWidthWhenScrollbarVisible(this in SleekElementBuilder<ISleekScrollView> builder)
        => ref builder.WithShouldScaleContentToHeight(false);

    /// <summary>
    /// Sets <see cref="ISleekScrollView.ReduceWidthWhenScrollbarVisible"/> to <paramref name="reduceWidthWhenScrollbarVisible"/> for an <see cref="ISleekScrollView"/>.
    /// <para>The scrollbar being hidden can slightly increase the width of the scroll view.</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (reduce width).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekScrollView> WithScrollbarVisibleWidthBehavior(this in SleekElementBuilder<ISleekScrollView> builder, bool reduceWidthWhenScrollbarVisible)
    {
        if (builder.Element.ReduceWidthWhenScrollbarVisible != reduceWidthWhenScrollbarVisible)
            builder.Element.ReduceWidthWhenScrollbarVisible = reduceWidthWhenScrollbarVisible;
        return ref builder;
    }

    /// <summary>
    /// Sets <see cref="ISleekScrollView.ScaleContentToHeight"/> to <see langword="true"/> for an <see cref="ISleekScrollView"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekScrollbarVisibility.Default"/> (vertical scrollbar visible).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekScrollView> WithVerticalScrollbar(this in SleekElementBuilder<ISleekScrollView> builder)
        => ref builder.WithVerticalScrollbarVisibility(ESleekScrollbarVisibility.Default);

    /// <summary>
    /// Sets <see cref="ISleekScrollView.ScaleContentToHeight"/> to <see langword="false"/> for an <see cref="ISleekScrollView"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekScrollbarVisibility.Default"/> (vertical scrollbar visible).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekScrollView> WithoutVerticalScrollbar(this in SleekElementBuilder<ISleekScrollView> builder)
        => ref builder.WithVerticalScrollbarVisibility(ESleekScrollbarVisibility.Hidden);

    /// <summary>
    /// Sets <see cref="ISleekScrollView.VerticalScrollbarVisibility"/> to <paramref name="verticalScrollbarVisibility"/> for an <see cref="ISleekScrollView"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekScrollbarVisibility.Default"/> (vertical scrollbar visible).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekScrollView> WithVerticalScrollbarVisibility(this in SleekElementBuilder<ISleekScrollView> builder, ESleekScrollbarVisibility verticalScrollbarVisibility)
    {
        builder.Element.VerticalScrollbarVisibility = verticalScrollbarVisibility;
        return ref builder;
    }

    /// <summary>
    /// Sets the content size offset of an <see cref="ISleekScrollView"/>.
    /// <para>Coordintes: (size X, size Y).</para>
    /// </summary>
    /// <remarks>Default value: (0.0f, 0.0f).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekScrollView> WithContentSizeOffset(this in SleekElementBuilder<ISleekScrollView> builder, float contentSizeOffsetX, float contentSizeOffsetY)
        => ref builder.WithContentSizeOffset(new Vector2(contentSizeOffsetX, contentSizeOffsetY));

    /// <summary>
    /// Sets the content size offset of an <see cref="ISleekScrollView"/>.
    /// <para>Coordintes: (size X, size Y).</para>
    /// </summary>
    /// <remarks>Default value: (0.0f, 0.0f).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekScrollView> WithContentSizeOffset(this in SleekElementBuilder<ISleekScrollView> builder, Vector2 contentSizeOffset)
    {
        if (builder.Element.ContentSizeOffset != contentSizeOffset)
            builder.Element.ContentSizeOffset = contentSizeOffset;
        return ref builder;
    }

    /// <summary>
    /// Sets the normalized (0-1) center/offset of an <see cref="ISleekScrollView"/>.
    /// <para>Coordintes: (Left to right, top to bottom).</para>
    /// </summary>
    /// <remarks>Default value: (0.0f, 0.0f).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekScrollView> WithInitialScrollOffset(this in SleekElementBuilder<ISleekScrollView> builder, float contentSizeOffsetX, float contentSizeOffsetY)
        => ref builder.WithContentSizeOffset(new Vector2(contentSizeOffsetX, contentSizeOffsetY));

    /// <summary>
    /// Sets the normalized (0-1) center/offset of an <see cref="ISleekScrollView"/>.
    /// <para>Coordintes: (Left to right, top to bottom).</para>
    /// </summary>
    /// <remarks>Default value: (0.0f, 0.0f).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekScrollView> WithInitialScrollOffset(this in SleekElementBuilder<ISleekScrollView> builder, Vector2 contentSizeOffset)
    {
        builder.Element.NormalizedStateCenter = contentSizeOffset;
        return ref builder;
    }

    /// <summary>
    /// Sets <see cref="ISleekScrollView.HandleScrollWheel"/> to <see langword="true"/> for an <see cref="ISleekScrollView"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (handle scrolling).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekScrollView> HandleScrolling(this in SleekElementBuilder<ISleekScrollView> builder)
        => ref builder.ShouldHandleScrolling(true);

    /// <summary>
    /// Sets <see cref="ISleekScrollView.HandleScrollWheel"/> to <see langword="false"/> for an <see cref="ISleekScrollView"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (handle scrolling).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekScrollView> DoNotHandleScrolling(this in SleekElementBuilder<ISleekScrollView> builder)
        => ref builder.ShouldHandleScrolling(false);

    /// <summary>
    /// Sets <see cref="ISleekScrollView.HandleScrollWheel"/> to <paramref name="handleScrolling"/> for an <see cref="ISleekScrollView"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (handle scrolling).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekScrollView> ShouldHandleScrolling(this in SleekElementBuilder<ISleekScrollView> builder, bool handleScrolling)
    {
        builder.Element.HandleScrollWheel = handleScrolling;
        return ref builder;
    }

    /// <summary>
    /// Sets the background color of an <see cref="ISleekScrollView"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekTint.BACKGROUND"/> (from settings, default value <c>RGB01(0.9, 0.9, 0.9)</c>).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekScrollView> WithBackgroundColor(this in SleekElementBuilder<ISleekScrollView> builder,
        [ValueProvider("SDG.Unturned.ESleekTint"), ValueProvider("UnityEngine.Color")] SleekColor backgroundColor)
    {
        builder.Element.BackgroundColor = backgroundColor;
        return ref builder;
    }

    /// <summary>
    /// Sets the foreground color of an <see cref="ISleekScrollView"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekTint.FOREGROUND"/> (from settings, default value <c>RGB01(0.9, 0.9, 0.9)</c>).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekScrollView> WithForegroundColor(this in SleekElementBuilder<ISleekScrollView> builder,
        [ValueProvider("SDG.Unturned.ESleekTint"), ValueProvider("UnityEngine.Color")] SleekColor foregroundColor)
    {
        builder.Element.ForegroundColor = foregroundColor;
        return ref builder;
    }

    /// <summary>
    /// Sets <see cref="ISleekScrollView.ContentUseManualLayout"/> to <see langword="true"/> for an <see cref="ISleekScrollView"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (manual layout).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekScrollView> WithManualLayout(this in SleekElementBuilder<ISleekScrollView> builder)
        => ref builder.WithShouldUseManualLayout(true);

    /// <summary>
    /// Sets <see cref="ISleekScrollView.ContentUseManualLayout"/> to <see langword="false"/> for an <see cref="ISleekScrollView"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (manual layout).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekScrollView> WithAutomaticLayout(this in SleekElementBuilder<ISleekScrollView> builder)
        => ref builder.WithShouldUseManualLayout(false);

    /// <summary>
    /// Sets <see cref="ISleekScrollView.ContentUseManualLayout"/> to <paramref name="useManualLayout"/> for an <see cref="ISleekScrollView"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (manual layout).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekScrollView> WithShouldUseManualLayout(this in SleekElementBuilder<ISleekScrollView> builder, bool useManualLayout)
    {
        builder.Element.ContentUseManualLayout = useManualLayout;
        return ref builder;
    }

    /// <summary>
    /// Sets <see cref="ISleekScrollView.AlignContentToBottom"/> to <see langword="true"/> for an <see cref="ISleekScrollView"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (top aligned).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekScrollView> WithBottomAlignedContent(this in SleekElementBuilder<ISleekScrollView> builder)
        => ref builder.WithShouldAlignContentToBottom(true);

    /// <summary>
    /// Sets <see cref="ISleekScrollView.AlignContentToBottom"/> to <see langword="false"/> for an <see cref="ISleekScrollView"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (top aligned).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekScrollView> WithTopAlignedContent(this in SleekElementBuilder<ISleekScrollView> builder)
        => ref builder.WithShouldAlignContentToBottom(false);

    /// <summary>
    /// Sets <see cref="ISleekScrollView.AlignContentToBottom"/> to <paramref name="alignContentToBottom"/> for an <see cref="ISleekScrollView"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (top aligned).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekScrollView> WithShouldAlignContentToBottom(this in SleekElementBuilder<ISleekScrollView> builder, bool alignContentToBottom)
    {
        builder.Element.AlignContentToBottom = alignContentToBottom;
        return ref builder;
    }

    /// <summary>
    /// Sets <see cref="ISleekScrollView.IsRaycastTarget"/> to <see langword="true"/> on an <see cref="ISleekScrollView"/>.
    /// <para>Should the scroll view consume clicks?</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (raycast target).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekScrollView> RaycastTarget(this in SleekElementBuilder<ISleekScrollView> builder)
        => ref builder.WithIsRaycastTarget(true);

    /// <summary>
    /// Sets <see cref="ISleekScrollView.IsRaycastTarget"/> to <see langword="false"/> on an <see cref="ISleekScrollView"/>.
    /// <para>Should the scroll view consume clicks?</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (raycast target).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekScrollView> RaycastIgnored(this in SleekElementBuilder<ISleekScrollView> builder)
        => ref builder.WithIsRaycastTarget(false);

    /// <summary>
    /// Sets <see cref="ISleekScrollView.IsRaycastTarget"/> to <paramref name="isRaycastTarget"/> on an <see cref="ISleekScrollView"/>.
    /// <para>Should the scroll view consume clicks?</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (raycast target).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekScrollView> WithIsRaycastTarget(this in SleekElementBuilder<ISleekScrollView> builder, bool isRaycastTarget)
    {
        if (builder.Element.IsRaycastTarget != isRaycastTarget)
            builder.Element.IsRaycastTarget = isRaycastTarget;
        return ref builder;
    }

    /// <summary>
    /// Sets <see cref="ISleekScrollView.NormalizedVerticalPosition"/> to 0.0f.
    /// </summary>
    /// <remarks>Default value: 0.0f (start at top).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekScrollView> StartAtTop(this in SleekElementBuilder<ISleekScrollView> builder)
    {
        if (builder.Element.NormalizedVerticalPosition != 0f)
            builder.Element.ScrollToTop();
        return ref builder;
    }

    /// <summary>
    /// Sets <see cref="ISleekScrollView.NormalizedVerticalPosition"/> to 1.0f.
    /// </summary>
    /// <remarks>Default value: 0.0f (start at top).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekScrollView> StartAtBottom(this in SleekElementBuilder<ISleekScrollView> builder)
    {
        if (builder.Element.NormalizedVerticalPosition != 1f)
            builder.Element.ScrollToBottom();
        return ref builder;
    }

    /// <summary>
    /// Adds a callback for when the normalized (0-1) center/offset in an <see cref="ISleekScrollView"/> is changed.
    /// <para>Top to bottom or left to right depending on orientation.</para>
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekScrollView> WhenScrollOffsetChanged(this in SleekElementBuilder<ISleekScrollView> builder, Action<Vector2> callback)
    {
        builder.Element.OnNormalizedValueChanged += callback;
        return ref builder;
    }


    /// <summary>
    /// Sets the normalized (0-1) scroll progress of an <see cref="ISleekSlider"/>.
    /// <para>Top to bottom or left to right depending on orientation.</para>
    /// </summary>
    /// <remarks>Default value: 0.0f.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekSlider> WithInitialProgress(this in SleekElementBuilder<ISleekSlider> builder, float progress)
    {
        progress = Mathf.Clamp01(progress);
        if (builder.Element.Value != progress)
            builder.Element.Value = progress;
        return ref builder;
    }

    /// <summary>
    /// Sets the orientation of an <see cref="ISleekSlider"/> to <see cref="ESleekOrientation.VERTICAL"/>.
    /// <para>Is the slider vertical or horizontal?</para>
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekOrientation.VERTICAL"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekSlider> WithVerticalOrientation(this in SleekElementBuilder<ISleekSlider> builder)
        => ref builder.WithOrientation(ESleekOrientation.VERTICAL);

    /// <summary>
    /// Sets the orientation of an <see cref="ISleekSlider"/> to <see cref="ESleekOrientation.HORIZONTAL"/>.
    /// <para>Is the slider vertical or horizontal?</para>
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekOrientation.VERTICAL"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekSlider> WithHorizontalOrientation(this in SleekElementBuilder<ISleekSlider> builder)
        => ref builder.WithOrientation(ESleekOrientation.HORIZONTAL);

    /// <summary>
    /// Sets the orientation of an <see cref="ISleekSlider"/>.
    /// <para>Is the slider vertical or horizontal?</para>
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekOrientation.VERTICAL"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekSlider> WithOrientation(this in SleekElementBuilder<ISleekSlider> builder, ESleekOrientation orientation)
    {
        if (builder.Element.Orientation != orientation)
            builder.Element.Orientation = orientation;
        return ref builder;
    }

    /// <summary>
    /// Sets <see cref="ISleekSlider.IsInteractable"/> to <see langword="true"/> on an <see cref="ISleekSlider"/>.
    /// <para>Interactability defines if the user can interact with the slider (change it's value).</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (interactable).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekSlider> Interactable(this in SleekElementBuilder<ISleekSlider> builder)
        => ref builder.WithIsInteractable(true);

    /// <summary>
    /// Sets <see cref="ISleekSlider.IsInteractable"/> to <see langword="false"/> on an <see cref="ISleekSlider"/>.
    /// <para>Interactability defines if the user can interact with the slider (change it's value).</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (interactable).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekSlider> NotInteractable(this in SleekElementBuilder<ISleekSlider> builder)
        => ref builder.WithIsInteractable(false);

    /// <summary>
    /// Sets <see cref="ISleekSlider.IsInteractable"/> to <paramref name="isInteractable"/> on an <see cref="ISleekSlider"/>.
    /// <para>Interactability defines if the user can interact with the slider (change it's value).</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (interactable).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekSlider> WithIsInteractable(this in SleekElementBuilder<ISleekSlider> builder, bool isInteractable)
    {
        if (builder.Element.IsInteractable != isInteractable)
            builder.Element.IsInteractable = isInteractable;
        return ref builder;
    }

    /// <summary>
    /// Sets the background color of an <see cref="ISleekSlider"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekTint.BACKGROUND"/> (from settings, default value <c>RGB01(0.9, 0.9, 0.9)</c>).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekSlider> WithBackgroundColor(this in SleekElementBuilder<ISleekSlider> builder,
        [ValueProvider("SDG.Unturned.ESleekTint"), ValueProvider("UnityEngine.Color")] SleekColor backgroundColor)
    {
        builder.Element.BackgroundColor = backgroundColor;
        return ref builder;
    }

    /// <summary>
    /// Sets the foreground color of an <see cref="ISleekSlider"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekTint.FOREGROUND"/> (from settings, default value <c>RGB01(0.9, 0.9, 0.9)</c>).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekSlider> WithForegroundColor(this in SleekElementBuilder<ISleekSlider> builder,
        [ValueProvider("SDG.Unturned.ESleekTint"), ValueProvider("UnityEngine.Color")] SleekColor foregroundColor)
    {
        builder.Element.ForegroundColor = foregroundColor;
        return ref builder;
    }

    /// <summary>
    /// Adds a callback for when the scroll progress in an <see cref="ISleekSlider"/> is changed.
    /// <para>Top to bottom or left to right depending on orientation.</para>
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekSlider> WhenProgressChanged(this in SleekElementBuilder<ISleekSlider> builder, Dragged callback)
    {
        builder.Element.OnValueChanged += callback;
        return ref builder;
    }


    /// <summary>
    /// Sets the sprite of an <see cref="ISleekSprite"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="null"/>.</remarks>
    /// <seealso cref="WithSprite(in SleekElementBuilder{ISleekSprite},Bundle,string)"/>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekSprite> WithSprite(this in SleekElementBuilder<ISleekSprite> builder, Sprite? sprite)
    {
        if (builder.Element.Sprite != sprite)
            builder.Element.Sprite = sprite;
        return ref builder;
    }

    /// <summary>
    /// Loads the sprite of an <see cref="ISleekSprite"/>.
    /// </summary>
    /// <exception cref="ArgumentException">Unable to find a <see cref="Sprite"/> at <paramref name="path"/> in <paramref name="bundle"/>.</exception>
    /// <remarks>Default value: <see langword="null"/>.</remarks>
    /// <seealso cref="WithSprite(in SleekElementBuilder{ISleekSprite},Sprite)"/>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekSprite> WithSprite(this in SleekElementBuilder<ISleekSprite> builder, Bundle bundle, string path)
    {
        Sprite? sprite = bundle.load<Sprite>(path);
        if (sprite == null)
            throw new ArgumentException("Unable to find a sprite in the provided bundle.", nameof(path));

        builder.Element.Sprite = sprite;
        return ref builder;
    }

    /// <summary>
    /// Sets the tint of an <see cref="ISleekSprite"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekTint.NONE"/> (<see cref="Color.white"/>).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekSprite> WithTintColor(this in SleekElementBuilder<ISleekSprite> builder,
        [ValueProvider("SDG.Unturned.ESleekTint"), ValueProvider("UnityEngine.Color")] SleekColor tintColor)
    {
        builder.Element.TintColor = tintColor;
        return ref builder;
    }

    /// <summary>
    /// Adds a callback for when an <see cref="ISleekSprite"/> is left clicked.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekSprite> WhenLeftClicked(this in SleekElementBuilder<ISleekSprite> builder, Action callback)
    {
        builder.Element.OnClicked += callback;
        return ref builder;
    }

    /// <summary>
    /// Sets <see cref="ISleekSprite.IsRaycastTarget"/> to <see langword="true"/> on an <see cref="ISleekSprite"/>.
    /// <para>Should the sprite consume clicks?</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (raycast target).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekSprite> RaycastTarget(this in SleekElementBuilder<ISleekSprite> builder)
        => ref builder.WithIsRaycastTarget(true);

    /// <summary>
    /// Sets <see cref="ISleekSprite.IsRaycastTarget"/> to <see langword="false"/> on an <see cref="ISleekSprite"/>.
    /// <para>Should the sprite consume clicks?</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (raycast target).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekSprite> RaycastIgnored(this in SleekElementBuilder<ISleekSprite> builder)
        => ref builder.WithIsRaycastTarget(false);

    /// <summary>
    /// Sets <see cref="ISleekSprite.IsRaycastTarget"/> to <paramref name="isRaycastTarget"/> on an <see cref="ISleekSprite"/>.
    /// <para>Should the sprite consume clicks?</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (raycast target).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekSprite> WithIsRaycastTarget(this in SleekElementBuilder<ISleekSprite> builder, bool isRaycastTarget)
    {
        if (builder.Element.IsRaycastTarget != isRaycastTarget)
            builder.Element.IsRaycastTarget = isRaycastTarget;
        return ref builder;
    }

    /// <summary>
    /// Sets the draw method of an <see cref="ISleekSprite"/> to <see cref="ESleekSpriteType.Sliced"/>.
    /// <para>Edges of a sprite are separated from the center, defined in Unity's sprite editor.</para>
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekSpriteType.Tiled"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekSprite> WithSlicedDrawMethod(this in SleekElementBuilder<ISleekSprite> builder)
    {
        if (builder.Element.DrawMethod != ESleekSpriteType.Sliced)
            builder.Element.DrawMethod = ESleekSpriteType.Sliced;
        return ref builder;
    }

    /// <summary>
    /// Sets the draw method of an <see cref="ISleekSprite"/> to <see cref="ESleekSpriteType.Regular"/>.
    /// <para>Only draws one image with no manipulation.</para>
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekSpriteType.Tiled"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekSprite> WithSingularDrawMethod(this in SleekElementBuilder<ISleekSprite> builder)
    {
        if (builder.Element.DrawMethod != ESleekSpriteType.Regular)
            builder.Element.DrawMethod = ESleekSpriteType.Regular;
        return ref builder;
    }

    /// <summary>
    /// Sets the draw method of an <see cref="ISleekSprite"/> to <see cref="ESleekSpriteType.Tiled"/>.
    /// <para>Images are repeated in the positive X and Y directions (or in all directions in some implementations).</para>
    /// </summary>
    /// <param name="tilingDimensions">Number of images to tile in each direction. Used for the UIToolkit <see cref="Glazier"/> set.</param>
    /// <remarks>Default value: <see cref="ESleekSpriteType.Tiled"/> (you still should pass tiling dimensions if you want more than a 1x1).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekSprite> WithTiledDrawMethod(this in SleekElementBuilder<ISleekSprite> builder, Vector2Int tilingDimensions)
    {
        if (builder.Element.DrawMethod != ESleekSpriteType.Tiled)
            builder.Element.DrawMethod = ESleekSpriteType.Tiled;
        builder.Element.TileRepeatHintForUITK = tilingDimensions;
        return ref builder;
    }

    /// <summary>
    /// Sets the draw method of an <see cref="ISleekSprite"/> to <see cref="ESleekSpriteType.Regular"/>.
    /// <para>Defines how sprites are drawn. Use <see cref="WithTiledDrawMethod(in SleekElementBuilder{ISleekSprite},Vector2Int)"/> to set the mode to <see cref="ESleekSpriteType.Tiled"/> (this method throws an exception in that case).</para>
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekSpriteType.Tiled"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    /// <exception cref="ArgumentException">Use <see cref="WithTiledDrawMethod(in SleekElementBuilder{ISleekSprite},Vector2Int)"/> to set tiling mode.</exception>
    public static ref readonly SleekElementBuilder<ISleekSprite> WithDrawMethod(this in SleekElementBuilder<ISleekSprite> builder, ESleekSpriteType drawMethod)
    {
        if (drawMethod == ESleekSpriteType.Tiled)
            throw new ArgumentException("Use the overload that takes a Vector2Int for the tiled draw method.", nameof(drawMethod));

        if (builder.Element.DrawMethod != drawMethod)
            builder.Element.DrawMethod = drawMethod;
        return ref builder;
    }


    /// <summary>
    /// Sets the checked state of an <see cref="ISleekToggle"/> to checked.
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (not checked).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekToggle> InitiallyChecked(this in SleekElementBuilder<ISleekToggle> builder)
        => ref builder.WithInitialState(true);

    /// <summary>
    /// Sets the checked state of an <see cref="ISleekToggle"/> to unchecked.
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (not checked).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekToggle> InitiallyUnchecked(this in SleekElementBuilder<ISleekToggle> builder)
        => ref builder.WithInitialState(false);

    /// <summary>
    /// Sets the checked state of an <see cref="ISleekToggle"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (not checked).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekToggle> WithInitialState(this in SleekElementBuilder<ISleekToggle> builder, bool isChecked)
    {
        if (builder.Element.Value != isChecked)
            builder.Element.Value = isChecked;
        return ref builder;
    }

    /// <summary>
    /// Sets <see cref="ISleekToggle.IsInteractable"/> to <see langword="true"/> on an <see cref="ISleekToggle"/>.
    /// <para>Interactability defines if the user can interact with the check box (change it's value).</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (interactable).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekToggle> Interactable(this in SleekElementBuilder<ISleekToggle> builder)
        => ref builder.WithIsInteractable(true);

    /// <summary>
    /// Sets <see cref="ISleekToggle.IsInteractable"/> to <see langword="false"/> on an <see cref="ISleekToggle"/>.
    /// <para>Interactability defines if the user can interact with the check box (change it's value).</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (interactable).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekToggle> NotInteractable(this in SleekElementBuilder<ISleekToggle> builder)
        => ref builder.WithIsInteractable(false);

    /// <summary>
    /// Sets <see cref="ISleekToggle.IsInteractable"/> to <paramref name="isInteractable"/> on an <see cref="ISleekToggle"/>.
    /// <para>Interactability defines if the user can interact with the check box (change it's value).</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (interactable).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekToggle> WithIsInteractable(this in SleekElementBuilder<ISleekToggle> builder, bool isInteractable)
    {
        if (builder.Element.IsInteractable != isInteractable)
            builder.Element.IsInteractable = isInteractable;
        return ref builder;
    }

    /// <summary>
    /// Sets the background color of an <see cref="ISleekToggle"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekTint.BACKGROUND"/> (from settings, default value <c>RGB01(0.9, 0.9, 0.9)</c>).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekToggle> WithBackgroundColor(this in SleekElementBuilder<ISleekToggle> builder,
        [ValueProvider("SDG.Unturned.ESleekTint"), ValueProvider("UnityEngine.Color")] SleekColor backgroundColor)
    {
        builder.Element.BackgroundColor = backgroundColor;
        return ref builder;
    }

    /// <summary>
    /// Sets the foreground color of an <see cref="ISleekToggle"/>.
    /// <para>The foreground color is the check mark.</para>
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekTint.FOREGROUND"/> (from settings, default value <c>RGB01(0.9, 0.9, 0.9)</c>).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekToggle> WithForegroundColor(this in SleekElementBuilder<ISleekToggle> builder,
        [ValueProvider("SDG.Unturned.ESleekTint"), ValueProvider("UnityEngine.Color")] SleekColor foregroundColor)
    {
        builder.Element.ForegroundColor = foregroundColor;
        return ref builder;
    }

    /// <summary>
    /// Adds a callback for when the checked state of an <see cref="ISleekToggle"/> is changed.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekToggle> WhenStateChanged(this in SleekElementBuilder<ISleekToggle> builder, Toggled callback)
    {
        builder.Element.OnValueChanged += callback;
        return ref builder;
    }


    /// <summary>
    /// Adds a callback for when the value in an <see cref="ISleekUInt16Field"/> is modified.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekUInt16Field> WhenValueTyped(this in SleekElementBuilder<ISleekUInt16Field> builder, TypedUInt16 callback)
    {
        builder.Element.OnValueChanged += callback;
        return ref builder;
    }

    /// <summary>
    /// Sets the initial value (text) of an <see cref="ISleekUInt16Field"/>.
    /// </summary>
    /// <remarks>Default value: 0.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekUInt16Field> WithInitialValue(this in SleekElementBuilder<ISleekUInt16Field> builder, ushort value)
    {
        if (builder.Element.Value != value)
            builder.Element.Value = value;
        return ref builder;
    }

    /// <summary>
    /// Sets the minimum value of an <see cref="ISleekUInt16Field"/>.
    /// </summary>
    /// <remarks>Default value: 0.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekUInt16Field> WithMinimumValue(this in SleekElementBuilder<ISleekUInt16Field> builder, ushort value)
    {
        if (builder.Element.MinValue != value)
            builder.Element.MinValue = value;
        return ref builder;
    }

    /// <summary>
    /// Sets the maximum value of an <see cref="ISleekUInt16Field"/>.
    /// </summary>
    /// <remarks>Default value: 65535.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekUInt16Field> WithMaximumValue(this in SleekElementBuilder<ISleekUInt16Field> builder, ushort value)
    {
        if (builder.Element.MaxValue != value)
            builder.Element.MaxValue = value;
        return ref builder;
    }


    /// <summary>
    /// Adds a callback for when the value in an <see cref="ISleekUInt32Field"/> is modified.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekUInt32Field> WhenValueTyped(this in SleekElementBuilder<ISleekUInt32Field> builder, TypedUInt32 callback)
    {
        builder.Element.OnValueChanged += callback;
        return ref builder;
    }

    /// <summary>
    /// Sets the initial value (text) of an <see cref="ISleekUInt32Field"/>.
    /// </summary>
    /// <remarks>Default value: 0.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekUInt32Field> WithInitialValue(this in SleekElementBuilder<ISleekUInt32Field> builder, uint value)
    {
        if (builder.Element.Value != value)
            builder.Element.Value = value;
        return ref builder;
    }


    /// <summary>
    /// Adds a callback for when the value in an <see cref="ISleekUInt8Field"/> is modified.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekUInt8Field> WhenValueTyped(this in SleekElementBuilder<ISleekUInt8Field> builder, TypedByte callback)
    {
        builder.Element.OnValueChanged += callback;
        return ref builder;
    }

    /// <summary>
    /// Sets the initial value (text) of an <see cref="ISleekUInt8Field"/>.
    /// </summary>
    /// <remarks>Default value: 0.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<ISleekUInt8Field> WithInitialValue(this in SleekElementBuilder<ISleekUInt8Field> builder, byte value)
    {
        if (builder.Element.Value != value)
            builder.Element.Value = value;
        return ref builder;
    }


    /// <summary>
    /// Sets the tooltip text of an <see cref="ISleekWithTooltip"/>.
    /// <para>Text that will show when hovering over the element.</para>
    /// </summary>
    /// <remarks>Default value: <see langword="null"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static ref readonly SleekElementBuilder<TSleekWithTooltip> WithTooltip<TSleekWithTooltip>(this in SleekElementBuilder<TSleekWithTooltip> builder, string? tooltip) where TSleekWithTooltip : class, ISleekWithTooltip
    {
        if (tooltip is { Length: 0 })
            tooltip = null;

        if (!ReferenceEquals(builder.Element.TooltipText, tooltip))
            builder.Element.TooltipText = tooltip;
        return ref builder;
    }
}
