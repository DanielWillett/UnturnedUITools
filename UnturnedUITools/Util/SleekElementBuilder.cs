using DanielWillett.UITools.API;
using SDG.Unturned;
using System;
using JetBrains.Annotations;
using UnityEngine;

namespace DanielWillett.UITools.Util;

/// <summary>
/// Fluent API implementation for editing <see cref="ISleekElement"/>.
/// </summary>
/// <remarks>There is no generic constraint on this because not all sleek interfaces inherit <see cref="ISleekElement"/>.</remarks>
public readonly ref struct SleekElementBuilder<TElement> where TElement : class
{
    private readonly TElement _element;

    /// <summary>
    /// The element currently being edited.
    /// </summary>
    public TElement Element => _element;

    /// <summary>
    /// The element currently being edited.
    /// </summary>
    /// <remarks>This allows you to access the element as a <see cref="ISleekElement"/> since not all of the interfaces inherit <see cref="ISleekElement"/>.</remarks>
    public ISleekElement SleekElement => (ISleekElement)_element;

    internal SleekElementBuilder(ISleekElement element)
    {
        _element = element as TElement ?? throw new ArgumentException($"Must implement {typeof(TElement).Name}", nameof(element));
    }

    /// <summary>
    /// Fills the entire bounding area with the element.
    /// </summary>
    public SleekElementBuilder<TElement> Fill()
    {
        ISleekElement element = (ISleekElement)_element;

        element.PositionOffset_X = 0;
        element.PositionOffset_Y = 0;
        element.PositionScale_X = 0;
        element.PositionScale_Y = 0;
        element.SizeOffset_X = 0;
        element.SizeOffset_Y = 0;
        element.SizeScale_X = 1f;
        element.SizeScale_Y = 1f;
        return this;
    }

    /// <summary>
    /// Copies all basic transforms from <see cref="ISleekElement"/>.
    /// </summary>
    public SleekElementBuilder<TElement> WithOrigin(ISleekElement transform)
    {
        ISleekElement element = (ISleekElement)_element;

        element.PositionOffset_X = transform.PositionOffset_X;
        element.PositionOffset_Y = transform.PositionOffset_Y;
        element.PositionScale_X = transform.PositionScale_X;
        element.PositionScale_Y = transform.PositionScale_Y;
        element.SizeOffset_X = transform.SizeOffset_X;
        element.SizeOffset_Y = transform.SizeOffset_Y;
        element.SizeScale_X = transform.SizeScale_X;
        element.SizeScale_Y = transform.SizeScale_Y;
        return this;
    }

    /// <summary>
    /// Copies all basic transforms from <see cref="ISleekElement"/>.
    /// </summary>
    public SleekElementBuilder<TElement> WithPreset([ValueProvider("DanielWillett.UITools.Util.SleekTransformPresets")] in SleekTransformPreset preset)
    {
        preset.Apply((ISleekElement)_element);
        return this;
    }

    /// <summary>
    /// Moves the element to the top left corner of the screen and resets any offsets.
    /// </summary>
    public SleekElementBuilder<TElement> TopLeft()
    {
        ISleekElement element = (ISleekElement)_element;

        element.PositionOffset_X = 0;
        element.PositionOffset_Y = 0;
        element.PositionScale_X = 0;
        element.PositionScale_Y = 0;
        return this;
    }

    /// <summary>
    /// Moves the element to the top left corner of the screen and resets any offsets.
    /// </summary>
    public SleekElementBuilder<TElement> BottomLeft()
    {
        ISleekElement element = (ISleekElement)_element;

        element.PositionOffset_X = 0;
        element.PositionOffset_Y = -element.SizeOffset_Y;
        element.PositionScale_X = 0;
        element.PositionScale_Y = 1 - element.SizeScale_Y;
        return this;
    }

    /// <summary>
    /// Moves the element to the top left corner of the screen and resets any offsets.
    /// </summary>
    public SleekElementBuilder<TElement> TopRight()
    {
        ISleekElement element = (ISleekElement)_element;

        element.PositionOffset_X = -element.SizeOffset_X;
        element.PositionOffset_Y = 0;
        element.PositionScale_X = 1 - element.SizeScale_X;
        element.PositionScale_Y = 0;
        return this;
    }

    /// <summary>
    /// Moves the element to the top left corner of the screen and resets any offsets.
    /// </summary>
    public SleekElementBuilder<TElement> BottomRight()
    {
        ISleekElement element = (ISleekElement)_element;

        element.PositionOffset_X = -element.SizeOffset_X;
        element.PositionOffset_Y = -element.SizeOffset_Y;
        element.PositionScale_X = 1 - element.SizeScale_X;
        element.PositionScale_Y = 1 - element.SizeScale_Y;
        return this;
    }

    /// <summary>
    /// Moves the element to the center of the screen and resets any offsets.
    /// </summary>
    public SleekElementBuilder<TElement> Center()
    {
        ISleekElement element = (ISleekElement)_element;

        element.PositionOffset_X = -(element.SizeOffset_X / 2f);
        element.PositionOffset_Y = -(element.SizeOffset_Y / 2f);
        element.PositionScale_X = 0.5f;
        element.PositionScale_Y = 0.5f;
        return this;
    }

    /// <summary>
    /// Sets the raw values of the element's position offset without any manipulation
    /// </summary>
    /// <remarks>In pixels.</remarks>
    public SleekElementBuilder<TElement> WithRawOffsetPixels(Vector2 offset)
    {
        ISleekElement element = (ISleekElement)_element;

        element.PositionOffset_X = offset.x;
        element.PositionOffset_Y = offset.y;
        return this;
    }

    /// <summary>
    /// Sets the raw values of the element's position offset without any manipulation
    /// </summary>
    /// <remarks>In pixels.</remarks>
    public SleekElementBuilder<TElement> WithRawOffsetPixels(float offsetX, float offsetY)
    {
        ISleekElement element = (ISleekElement)_element;

        element.PositionOffset_X = offsetX;
        element.PositionOffset_Y = offsetY;
        return this;
    }

    /// <summary>
    /// Sets the raw values of the element's position offset without any manipulation
    /// </summary>
    /// <remarks>In pixels.</remarks>
    public SleekElementBuilder<TElement> AddRawOffsetPixels(Vector2 offset)
    {
        ISleekElement element = (ISleekElement)_element;

        element.PositionOffset_X += offset.x;
        element.PositionOffset_Y += offset.y;
        return this;
    }

    /// <summary>
    /// Sets the raw values of the element's position offset without any manipulation
    /// </summary>
    /// <remarks>In pixels.</remarks>
    public SleekElementBuilder<TElement> AddRawOffsetPixels(float offsetX, float offsetY)
    {
        ISleekElement element = (ISleekElement)_element;

        element.PositionOffset_X += offsetX;
        element.PositionOffset_Y += offsetY;
        return this;
    }

    /// <summary>
    /// Sets the raw values of the element's size without any manipulation
    /// </summary>
    /// <remarks>In pixels.</remarks>
    public SleekElementBuilder<TElement> WithRawSizePixels(Vector2 size)
    {
        ISleekElement element = (ISleekElement)_element;

        element.SizeOffset_X = size.x;
        element.SizeOffset_Y = size.y;
        return this;
    }

    /// <summary>
    /// Sets the raw values of the element's size without any manipulation
    /// </summary>
    /// <remarks>In pixels.</remarks>
    public SleekElementBuilder<TElement> WithRawSizePixels(float sizeX, float sizeY)
    {
        ISleekElement element = (ISleekElement)_element;

        element.SizeOffset_X = sizeX;
        element.SizeOffset_Y = sizeY;
        return this;
    }

    /// <summary>
    /// Sets the raw values of the element's position scale without any manipulation
    /// </summary>
    /// <remarks>Normalized value usually in [0, 1].</remarks>
    public SleekElementBuilder<TElement> WithRawOffsetScale(Vector2 scale)
    {
        ISleekElement element = (ISleekElement)_element;

        element.PositionScale_X = scale.x;
        element.PositionScale_Y = scale.y;
        return this;
    }

    /// <summary>
    /// Sets the raw values of the element's position scale without any manipulation
    /// </summary>
    /// <remarks>Normalized value usually in [0, 1].</remarks>
    public SleekElementBuilder<TElement> WithRawOffsetScale(float scaleX, float scaleY)
    {
        ISleekElement element = (ISleekElement)_element;

        element.PositionScale_X = scaleX;
        element.PositionScale_Y = scaleY;
        return this;
    }

    /// <summary>
    /// Sets the raw values of the element's position scale without any manipulation
    /// </summary>
    /// <remarks>Normalized value usually in [0, 1].</remarks>
    public SleekElementBuilder<TElement> AddRawOffsetScale(Vector2 scale)
    {
        ISleekElement element = (ISleekElement)_element;

        element.PositionScale_X += scale.x;
        element.PositionScale_Y += scale.y;
        return this;
    }

    /// <summary>
    /// Sets the raw values of the element's position scale without any manipulation
    /// </summary>
    /// <remarks>Normalized value usually in [0, 1].</remarks>
    public SleekElementBuilder<TElement> AddRawOffsetScale(float scaleX, float scaleY)
    {
        ISleekElement element = (ISleekElement)_element;

        element.PositionScale_X += scaleX;
        element.PositionScale_Y += scaleY;
        return this;
    }

    /// <summary>
    /// Sets the raw values of the element's position scale without any manipulation
    /// </summary>
    /// <remarks>Normalized value usually in [0, 1].</remarks>
    public SleekElementBuilder<TElement> WithRawSizeScale(Vector2 scale)
    {
        ISleekElement element = (ISleekElement)_element;

        element.SizeScale_X = scale.x;
        element.SizeScale_Y = scale.y;
        return this;
    }

    /// <summary>
    /// Sets the raw values of the element's position scale without any manipulation
    /// </summary>
    /// <remarks>Normalized value usually in [0, 1].</remarks>
    public SleekElementBuilder<TElement> WithRawSizeScale(float scaleX, float scaleY)
    {
        ISleekElement element = (ISleekElement)_element;

        element.SizeScale_X = scaleX;
        element.SizeScale_Y = scaleY;
        return this;
    }

    /// <summary>
    /// Changes the absolute position of the element. This is in pixels.
    /// </summary>
    /// <remarks>Default value: (0.0f, 0.0f).</remarks>
    /// <param name="offsetTowards">Where to offset towards (relative to parent's bounding box).</param>
    /// <exception cref="ArgumentOutOfRangeException">Anchor was not a valid value.</exception>
    public SleekElementBuilder<TElement> WithOffsetPixels(Vector2 offset, SleekOffsetAnchor offsetTowards = SleekOffsetAnchor.Inwards)
        => WithOffsetPixels(offset.x, offset.y, offsetTowards);

    /// <summary>
    /// Changes the absolute position of the element. This is in pixels.
    /// </summary>
    /// <remarks>Default value: (0.0f, 0.0f).</remarks>
    /// <param name="offsetTowards">Where to offset towards (relative to parent's bounding box).</param>
    /// <exception cref="ArgumentOutOfRangeException">Anchor was not a valid value.</exception>
    public SleekElementBuilder<TElement> WithOffsetPixels(float offsetX, float offsetY, SleekOffsetAnchor offsetTowards = SleekOffsetAnchor.Inwards)
    {
        ISleekElement element = (ISleekElement)_element;

        bool top = offsetTowards is SleekOffsetAnchor.TopCenter or SleekOffsetAnchor.TopRight or SleekOffsetAnchor.TopLeft or SleekOffsetAnchor.TopOutwardsCenter,
            bottom = offsetTowards is SleekOffsetAnchor.BottomCenter or SleekOffsetAnchor.BottomRight or SleekOffsetAnchor.BottomLeft or SleekOffsetAnchor.BottomOutwardsCenter,
            left = offsetTowards is SleekOffsetAnchor.LeftCenter or SleekOffsetAnchor.TopLeft or SleekOffsetAnchor.BottomLeft or SleekOffsetAnchor.LeftOutwardsCenter,
            right = offsetTowards is SleekOffsetAnchor.RightCenter or SleekOffsetAnchor.TopRight or SleekOffsetAnchor.BottomRight or SleekOffsetAnchor.RightOutwardsCenter;

        int outwardsScale = offsetTowards is SleekOffsetAnchor.Outwards or SleekOffsetAnchor.BottomOutwardsCenter
            or SleekOffsetAnchor.TopOutwardsCenter or SleekOffsetAnchor.LeftOutwardsCenter
            or SleekOffsetAnchor.RightOutwardsCenter ? -1 : 1;

        if (!top && !bottom && !left && !right)
        {
            if (offsetTowards != SleekOffsetAnchor.Inwards && outwardsScale != -1)
                throw new ArgumentOutOfRangeException(nameof(offsetTowards));

            int xNorm = element.PositionScale_X.RangeSign() * outwardsScale;
            int yNorm = element.PositionScale_Y.RangeSign() * outwardsScale;

            element.PositionOffset_X = xNorm * -offsetX;
            element.PositionOffset_Y = yNorm * offsetY;
        }
        else
        {
            if (top)
                element.PositionOffset_Y = -offsetY;
            else if (bottom)
                element.PositionOffset_Y = offsetY;
            else
            {
                int yNorm = element.PositionScale_Y.RangeSign() * outwardsScale;
                element.PositionOffset_Y = yNorm * offsetY;
            }

            if (right)
                element.PositionOffset_X = offsetX;
            else if (left)
                element.PositionOffset_X = -offsetX;
            else
            {
                int xNorm = element.PositionScale_X.RangeSign() * outwardsScale;
                element.PositionOffset_X = xNorm * -offsetX;
            }
        }

        element.PositionOffset_X -= element.SizeOffset_X / 2f;
        element.PositionOffset_Y -= element.SizeOffset_Y / 2f;

        return this;
    }

    /// <summary>
    /// Changes the absolute position of the element. This is a normalized value in [-1, 1] following the default XY plane layout.
    /// </summary>
    /// <remarks>Default value: (-1f, 1f) (top left).</remarks>
    public SleekElementBuilder<TElement> WithOffsetScale(Vector2 scale)
        => WithOffsetScale(scale.x, scale.y);

    /// <summary>
    /// Changes the absolute position of the element. This is a normalized value in [-1, 1] following the default XY plane layout.
    /// </summary>
    /// <remarks>Default value: (-1f, 1f) (top left).</remarks>
    public SleekElementBuilder<TElement> WithOffsetScale(float scaleX, float scaleY)
    {
        ISleekElement element = (ISleekElement)_element;

        element.PositionScale_X = scaleX.ToGameScale();
        element.PositionScale_Y = (-scaleY).ToGameScale();
        return this;
    }

    /// <summary>
    /// Changes the absolute size of the element. This is in pixels.
    /// </summary>
    /// <remarks>Default value: (0.0f, 0.0f).</remarks>
    /// <param name="anchor">Where to scale from (relative to the current scale anchor).</param>
    /// <exception cref="ArgumentOutOfRangeException">Anchor was not a valid value.</exception>
    public SleekElementBuilder<TElement> WithSizePixels(Vector3 size, SleekResizeAnchor anchor = SleekResizeAnchor.Center) => WithSizePixels(size.x, size.y, anchor);

    /// <summary>
    /// Changes the absolute size of the element. This is in pixels.
    /// </summary>
    /// <remarks>Default value: (0.0f, 0.0f).</remarks>
    /// <param name="anchor">Where to scale from (relative to the current scale anchor).</param>
    /// <exception cref="ArgumentOutOfRangeException">Anchor was not a valid value.</exception>
    public SleekElementBuilder<TElement> WithSizePixels(float sizeX, float sizeY, SleekResizeAnchor anchor = SleekResizeAnchor.Center)
    {
        ISleekElement element = (ISleekElement)_element;

        bool top    = anchor is SleekResizeAnchor.TopCenter or SleekResizeAnchor.TopRight or SleekResizeAnchor.TopLeft,
             bottom = anchor is SleekResizeAnchor.BottomCenter or SleekResizeAnchor.BottomRight or SleekResizeAnchor.BottomLeft,
             left   = anchor is SleekResizeAnchor.LeftCenter or SleekResizeAnchor.TopLeft or SleekResizeAnchor.BottomLeft,
             right  = anchor is SleekResizeAnchor.RightCenter or SleekResizeAnchor.TopRight or SleekResizeAnchor.BottomRight;

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
                element.PositionOffset_Y -= element.SizeOffset_Y;
            else if (!top)
            {
                float cy = element.PositionOffset_Y + element.SizeOffset_Y / 2f;
                element.PositionOffset_Y = cy - sizeY / 2f;
            }

            if (right)
                element.PositionOffset_X -= element.SizeOffset_X;
            else if (!left)
            {
                float cx = element.PositionOffset_X + element.SizeOffset_X / 2f;
                element.PositionOffset_X = cx - sizeX / 2f;
            }
        }

        element.SizeOffset_X = sizeX;
        element.SizeOffset_Y = sizeY;

        return this;
    }

    /// <summary>
    /// Changes the normalized size of the element. This is usually in [0, 1].
    /// </summary>
    /// <remarks>Default value: (0.0f, 0.0f).</remarks>
    /// <param name="anchor">Where to scale from (relative to the current scale anchor).</param>
    /// <exception cref="ArgumentOutOfRangeException">Anchor was not a valid value.</exception>
    public SleekElementBuilder<TElement> WithSizeScale(Vector3 scale, SleekResizeAnchor anchor = SleekResizeAnchor.Center) => WithSizeScale(scale.x, scale.y, anchor);

    /// <summary>
    /// Changes the normalized size of the element. This is usually in [0, 1].
    /// </summary>
    /// <remarks>Default value: (0.0f, 0.0f).</remarks>
    /// <param name="anchor">Where to scale from (relative to the current scale anchor).</param>
    /// <exception cref="ArgumentOutOfRangeException">Anchor was not a valid value.</exception>
    public SleekElementBuilder<TElement> WithSizeScale(float scaleX, float scaleY, SleekResizeAnchor anchor = SleekResizeAnchor.Center)
    {
        ISleekElement element = (ISleekElement)_element;

        bool top    = anchor is SleekResizeAnchor.TopCenter or SleekResizeAnchor.TopRight or SleekResizeAnchor.TopLeft,
             bottom = anchor is SleekResizeAnchor.BottomCenter or SleekResizeAnchor.BottomRight or SleekResizeAnchor.BottomLeft,
             left   = anchor is SleekResizeAnchor.LeftCenter or SleekResizeAnchor.TopLeft or SleekResizeAnchor.BottomLeft,
             right  = anchor is SleekResizeAnchor.RightCenter or SleekResizeAnchor.TopRight or SleekResizeAnchor.BottomRight;

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
                element.PositionScale_Y -= element.SizeScale_Y;
            else if (!top)
            {
                float cy = element.PositionScale_Y + element.SizeScale_Y / 2f;
                element.PositionScale_Y = cy - scaleY / 2f;
            }

            if (right)
                element.PositionScale_X -= element.SizeScale_X;
            else if (!left)
            {
                float cx = element.PositionScale_X + element.SizeScale_X / 2f;
                element.PositionScale_X = cx - scaleX / 2f;
            }
        }

        element.SizeScale_X = scaleX;
        element.SizeScale_Y = scaleY;

        return this;
    }

    /// <summary>
    /// Add a side label to the element.
    /// </summary>
    /// <param name="text">Text to display on the label.</param>
    /// <param name="side">Which side of the element to put the label on.</param>
    /// <param name="configureLabelAction">Optional configuration method.</param>
    public SleekElementBuilder<TElement> AddLabel(string text, ESleekSide side, Action<ISleekLabel>? configureLabelAction = null)
    {
        ISleekElement element = (ISleekElement)_element;

        if (element.SideLabel != null)
            element.RemoveChild(element.SideLabel);
        
        element.AddLabel(text, side);
        configureLabelAction?.Invoke(element.SideLabel!);
        return this;
    }

    /// <summary>
    /// Add a side label to the element.
    /// </summary>
    /// <param name="text">Text to display on the label.</param>
    /// <param name="textColor">Color of the label's text.</param>
    /// <param name="side">Which side of the element to put the label on.</param>
    /// <param name="configureLabelAction">Optional configuration method.</param>
    public SleekElementBuilder<TElement> AddLabel(string text, Color textColor, ESleekSide side, Action<ISleekLabel>? configureLabelAction = null)
    {
        ISleekElement element = (ISleekElement)_element;

        if (element.SideLabel != null)
            element.RemoveChild(element.SideLabel);

        element.AddLabel(text, textColor, side);
        configureLabelAction?.Invoke(element.SideLabel!);
        return this;
    }

    /// <summary>
    /// Further configure this element (without using the fluent API).
    /// </summary>
    public SleekElementBuilder<TElement> Configure(Action<TElement> configureAction)
    {
        configureAction?.Invoke(_element);
        return this;
    }

    /// <summary>
    /// Finalize the builder.
    /// </summary>
    /// <returns>The finished element.</returns>
    public TElement Build() => _element;

    /// <summary>
    /// Finalize the builder and add the element as a child of <paramref name="parent"/>.
    /// </summary>
    /// <remarks>The same as calling <see cref="ISleekElement.AddChild"/> on the parent after building.</remarks>
    /// <returns>The finished element.</returns>
    public TElement BuildAndParent(ISleekElement parent)
    {
        ISleekElement element = (ISleekElement)_element;

        if (parent.FindIndexOfChild(element) == -1)
            parent.AddChild(element);

        return _element;
    }
}
