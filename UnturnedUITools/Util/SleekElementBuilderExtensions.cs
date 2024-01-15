using DanielWillett.UITools.API;
using JetBrains.Annotations;
using SDG.Unturned;
using System;
using UnityEngine;
using Action = System.Action;

namespace DanielWillett.UITools.Util;

/// <summary>
/// Extensions for <see cref="SleekElementBuilder{TElement}"/> to handle extended sleek element types.
/// </summary>
public static class SleekElementBuilderExtensions
{
    /// <summary>
    /// Sets the background color of an <see cref="ISleekBox"/>.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekBox> WithBackgroundColor(this SleekElementBuilder<ISleekBox> builder,
        [ValueProvider("SDG.Unturned.ESleekTint"), ValueProvider("UnityEngine.Color")] SleekColor backgroundColor)
    {
        builder.Element.BackgroundColor = backgroundColor;
        return builder;
    }


    /// <summary>
    /// Sets the background color of an <see cref="ISleekButton"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekTint.BACKGROUND"/> (from settings, default value <c>RGB01(0.9, 0.9, 0.9)</c>).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekButton> WithBackgroundColor(this SleekElementBuilder<ISleekButton> builder,
        [ValueProvider("SDG.Unturned.ESleekTint"), ValueProvider("UnityEngine.Color")] SleekColor backgroundColor)
    {
        builder.Element.BackgroundColor = backgroundColor;
        return builder;
    }

    /// <summary>
    /// Sets <see cref="ISleekButton.IsClickable"/> to <see langword="true"/> on an <see cref="ISleekButton"/>.
    /// <para>Interactability defines if the user can interact with the button (click it).</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (interactable).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekButton> Interactable(this SleekElementBuilder<ISleekButton> builder)
        => builder.WithIsInteractable(true);

    /// <summary>
    /// Sets <see cref="ISleekButton.IsClickable"/> to <see langword="false"/> on an <see cref="ISleekButton"/>.
    /// <para>Interactability defines if the user can interact with the button (click it).</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (interactable).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekButton> NotInteractable(this SleekElementBuilder<ISleekButton> builder)
        => builder.WithIsInteractable(false);

    /// <summary>
    /// Sets <see cref="ISleekButton.IsClickable"/> to <paramref name="isClickable"/> on an <see cref="ISleekButton"/>.
    /// <para>Interactability defines if the user can interact with the button (click it).</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (interactable).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekButton> WithIsInteractable(this SleekElementBuilder<ISleekButton> builder, bool isClickable)
    {
        if (builder.Element.IsClickable != isClickable)
            builder.Element.IsClickable = isClickable;
        return builder;
    }

    /// <summary>
    /// Sets <see cref="ISleekButton.IsRaycastTarget"/> to <see langword="true"/> on an <see cref="ISleekButton"/>.
    /// <para>Should the button view consume clicks?</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (raycast target).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekButton> RaycastTarget(this SleekElementBuilder<ISleekButton> builder)
        => builder.WithIsRaycastTarget(true);

    /// <summary>
    /// Sets <see cref="ISleekButton.IsRaycastTarget"/> to <see langword="false"/> on an <see cref="ISleekButton"/>.
    /// <para>Should the button view consume clicks?</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (raycast target).</remarks>v
    public static SleekElementBuilder<ISleekButton> RaycastIgnored(this SleekElementBuilder<ISleekButton> builder)
        => builder.WithIsRaycastTarget(false);

    /// <summary>
    /// Sets <see cref="ISleekButton.IsRaycastTarget"/> to <paramref name="isRaycastTarget"/> on an <see cref="ISleekButton"/>.
    /// <para>Should the button view consume clicks?</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (raycast target).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekButton> WithIsRaycastTarget(this SleekElementBuilder<ISleekButton> builder, bool isRaycastTarget)
    {
        if (builder.Element.IsRaycastTarget != isRaycastTarget)
            builder.Element.IsRaycastTarget = isRaycastTarget;
        return builder;
    }

    /// <summary>
    /// Adds a callback for when an <see cref="ISleekButton"/> is left clicked.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekButton> WhenLeftClicked(this SleekElementBuilder<ISleekButton> builder, ClickedButton callback)
    {
        builder.Element.OnClicked += callback;
        return builder;
    }

    /// <summary>
    /// Adds a callback for when an <see cref="ISleekButton"/> is right clicked.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekButton> WhenRightClicked(this SleekElementBuilder<ISleekButton> builder, ClickedButton callback)
    {
        builder.Element.OnRightClicked += callback;
        return builder;
    }

    /// <summary>
    /// Adds a callback for when an <see cref="ISleekButton"/> is left or right clicked.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekButton> WhenAnyClicked(this SleekElementBuilder<ISleekButton> builder, ClickedButton callback)
    {
        builder.Element.OnClicked += callback;
        builder.Element.OnRightClicked += callback;
        return builder;
    }


    /// <summary>
    /// Sets the constraint mode of an <see cref="ISleekConstraintFrame"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekConstraint.NONE"/>.</remarks>
    /// <exception cref="InvalidOperationException">Constraint was set more than once.</exception>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekConstraintFrame> WithConstraintMode(this SleekElementBuilder<ISleekConstraintFrame> builder, ESleekConstraint constraint)
    {
        if (builder.Element.Constraint == constraint)
            return builder;

        if (builder.Element.Constraint != ESleekConstraint.NONE)
            throw new InvalidOperationException("Constraint can not be set more than once.");

        if (builder.Element.Constraint != constraint)
            builder.Element.Constraint = constraint;
        return builder;
    }

    /// <summary>
    /// Sets the aspect ratio of an <see cref="ISleekConstraintFrame"/> in <see cref="ESleekConstraint.FitInParent"/> constraint mode.
    /// </summary>
    /// <param name="aspectRatio">Aspect ratio to scale the content to. Calculated by dividing the width by the height.</param>
    /// <remarks>Default value: 1.0f.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekConstraintFrame> WithAspectRatio(this SleekElementBuilder<ISleekConstraintFrame> builder, float aspectRatio)
    {
        if (builder.Element.AspectRatio != aspectRatio)
            builder.Element.AspectRatio = aspectRatio;
        return builder;
    }


    /// <summary>
    /// Sets the background color of an <see cref="ISleekField"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekTint.BACKGROUND"/> (from settings, default value <c>RGB01(0.9, 0.9, 0.9)</c>).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekField> WithBackgroundColor(this SleekElementBuilder<ISleekField> builder,
        [ValueProvider("SDG.Unturned.ESleekTint"), ValueProvider("UnityEngine.Color")] SleekColor backgroundColor)
    {
        builder.Element.BackgroundColor = backgroundColor;
        return builder;
    }

    /// <summary>
    /// Sets <see cref="ISleekField.IsPasswordField"/> to <see langword="true"/> on an <see cref="ISleekField"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (not password field).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekField> PasswordField(this SleekElementBuilder<ISleekField> builder)
        => builder.WithIsPasswordField(true);

    /// <summary>
    /// Sets <see cref="ISleekField.IsPasswordField"/> to <see langword="false"/> on an <see cref="ISleekField"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (not password field).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekField> NotPasswordField(this SleekElementBuilder<ISleekField> builder)
        => builder.WithIsPasswordField(false);

    /// <summary>
    /// Sets <see cref="ISleekField.IsPasswordField"/> to <paramref name="isPasswordField"/> on an <see cref="ISleekField"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (not password field).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekField> WithIsPasswordField(this SleekElementBuilder<ISleekField> builder, bool isPasswordField)
    {
        if (builder.Element.IsPasswordField != isPasswordField)
            builder.Element.IsPasswordField = isPasswordField;
        return builder;
    }


    /// <summary>
    /// Sets <see cref="ISleekField.IsMultiline"/> to <see langword="true"/> on an <see cref="ISleekField"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (singleline).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekField> Multiline(this SleekElementBuilder<ISleekField> builder)
        => builder.WithIsMultiline(true);

    /// <summary>
    /// Sets <see cref="ISleekField.IsMultiline"/> to <see langword="false"/> on an <see cref="ISleekField"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (singleline).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekField> Singleline(this SleekElementBuilder<ISleekField> builder)
        => builder.WithIsMultiline(false);

    /// <summary>
    /// Sets <see cref="ISleekField.IsMultiline"/> to <paramref name="isMultiline"/> on an <see cref="ISleekField"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="false"/>. (singleline)</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekField> WithIsMultiline(this SleekElementBuilder<ISleekField> builder, bool isMultiline)
    {
        if (builder.Element.IsMultiline != isMultiline)
            builder.Element.IsMultiline = isMultiline;
        return builder;
    }

    /// <summary>
    /// Sets the max length in characters of an <see cref="ISleekField"/>.
    /// </summary>
    /// <param name="maxLength">Maximum amount of characters in this field.</param>
    /// <remarks>Default value: 100.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekField> WithMaxLength(this SleekElementBuilder<ISleekField> builder, int maxLength)
    {
        if (builder.Element.MaxLength != maxLength)
            builder.Element.MaxLength = maxLength;
        return builder;
    }

    /// <summary>
    /// Sets the placeholder text of an <see cref="ISleekField"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="null"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekField> WithPlaceholder(this SleekElementBuilder<ISleekField> builder, string placeholderText)
    {
        if (!ReferenceEquals(builder.Element.PlaceholderText, placeholderText))
            builder.Element.PlaceholderText = placeholderText;
        return builder;
    }

    /// <summary>
    /// Adds a callback for when text is submitted in an <see cref="ISleekField"/>.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekField> WhenTextSubmitted(this SleekElementBuilder<ISleekField> builder, Entered callback)
    {
        builder.Element.OnTextSubmitted += callback;
        return builder;
    }

    /// <summary>
    /// Adds a callback for when the text in an <see cref="ISleekField"/> is modified.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekField> WhenTextTyped(this SleekElementBuilder<ISleekField> builder, Typed callback)
    {
        builder.Element.OnTextChanged += callback;
        return builder;
    }

    /// <summary>
    /// Adds a callback for when the focus is lost for an <see cref="ISleekField"/>.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekField> WhenFocusLeft(this SleekElementBuilder<ISleekField> builder, Escaped callback)
    {
        builder.Element.OnTextEscaped += callback;
        return builder;
    }


    /// <summary>
    /// Adds a callback for when a value is submitted in an <see cref="ISleekFloat32Field"/>.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekFloat32Field> WhenValueSubmitted(this SleekElementBuilder<ISleekFloat32Field> builder, TypedSingle callback)
    {
        builder.Element.OnValueSubmitted += callback;
        return builder;
    }

    /// <summary>
    /// Adds a callback for when the value in an <see cref="ISleekFloat32Field"/> is modified.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekFloat32Field> WhenValueTyped(this SleekElementBuilder<ISleekFloat32Field> builder, TypedSingle callback)
    {
        builder.Element.OnValueChanged += callback;
        return builder;
    }

    /// <summary>
    /// Sets the initial value (text) of an <see cref="ISleekFloat32Field"/>.
    /// </summary>
    /// <remarks>Default value: 0.0f.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekFloat32Field> WithInitialValue(this SleekElementBuilder<ISleekFloat32Field> builder, float value)
    {
        if (builder.Element.Value != value)
            builder.Element.Value = value;
        return builder;
    }


    /// <summary>
    /// Adds a callback for when the value in an <see cref="ISleekFloat64Field"/> is modified.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekFloat64Field> WhenValueTyped(this SleekElementBuilder<ISleekFloat64Field> builder, TypedDouble callback)
    {
        builder.Element.OnValueChanged += callback;
        return builder;
    }

    /// <summary>
    /// Sets the initial value (text) of an <see cref="ISleekFloat64Field"/>.
    /// </summary>
    /// <remarks>Default value: 0.0d.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekFloat64Field> WithInitialValue(this SleekElementBuilder<ISleekFloat64Field> builder, double value)
    {
        if (builder.Element.Value != value)
            builder.Element.Value = value;
        return builder;
    }


    /// <summary>
    /// Sets the texture of an <see cref="ISleekImage"/>.
    /// </summary>
    /// <param name="shouldDestroyTexture">Should <see cref="UnityEngine.Object.Destroy(UnityEngine.Object)" /> be called on the texture when the element is destroyed.</param>
    /// <remarks>Default value: <see langword="null"/>.</remarks>
    /// <seealso cref="WithTexture(SleekElementBuilder{ISleekImage},Bundle,string,bool)"/>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekImage> WithTexture(this SleekElementBuilder<ISleekImage> builder, Texture? texture, bool shouldDestroyTexture)
    {
        if (texture is Texture2D t2d)
            builder.Element.SetTextureAndShouldDestroy(t2d, shouldDestroyTexture);
        else
        {
            builder.Element.Texture = texture;
            builder.Element.ShouldDestroyTexture = shouldDestroyTexture;
        }
        return builder;
    }

    /// <summary>
    /// Loads the texture of an <see cref="ISleekImage"/>.
    /// </summary>
    /// <param name="shouldDestroyTexture">Should <see cref="UnityEngine.Object.Destroy(UnityEngine.Object)" /> be called on the texture when the element is destroyed.</param>
    /// <exception cref="ArgumentException">Unable to find a <see cref="Texture2D"/> at <paramref name="path"/> in <paramref name="bundle"/>.</exception>
    /// <remarks>Default value: <see langword="null"/>.</remarks>
    /// <seealso cref="WithTexture(SleekElementBuilder{ISleekImage},Texture,bool)"/>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekImage> WithTexture(this SleekElementBuilder<ISleekImage> builder, Bundle bundle, string path, bool shouldDestroyTexture)
    {
        Texture2D? texture = bundle.load<Texture2D>(path);
        if (texture == null)
            throw new ArgumentException("Unable to find a texture in the provided bundle.", nameof(path));

        builder.Element.SetTextureAndShouldDestroy(texture, shouldDestroyTexture);
        return builder;
    }

    /// <summary>
    /// Sets the texture of an <see cref="ISleekImage"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="null"/>.</remarks>
    /// <seealso cref="WithTexture(SleekElementBuilder{ISleekImage},Bundle,string)"/>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekImage> WithTexture(this SleekElementBuilder<ISleekImage> builder, Texture? texture)
    {
        if (texture is Texture2D t2d)
            builder.Element.UpdateTexture(t2d);
        else
            builder.Element.Texture = texture;
        return builder;
    }

    /// <summary>
    /// Loads the texture of an <see cref="ISleekImage"/>.
    /// </summary>
    /// <exception cref="ArgumentException">Unable to find a <see cref="Texture2D"/> at <paramref name="path"/> in <paramref name="bundle"/>.</exception>
    /// <remarks>Default value: <see langword="null"/>.</remarks>
    /// <seealso cref="WithTexture(SleekElementBuilder{ISleekImage},Texture)"/>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekImage> WithTexture(this SleekElementBuilder<ISleekImage> builder, Bundle bundle, string path)
    {
        Texture2D? texture = bundle.load<Texture2D>(path);
        if (texture == null)
            throw new ArgumentException("Unable to find a texture in the provided bundle.", nameof(path));

        builder.Element.UpdateTexture(texture);
        return builder;
    }

    /// <summary>
    /// Sets the rotation angle of an <see cref="ISleekImage"/> and sets <see cref="ISleekImage.CanRotate"/> to <see langword="true"/>.
    /// </summary>
    /// <param name="angleDeg">Angle to render the image at in degrees.</param>
    /// <remarks>Default value: 0.0f (can't rotate).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekImage> WithRotationAngle(this SleekElementBuilder<ISleekImage> builder, float angleDeg)
    {
        if (builder.Element.CanRotate && builder.Element.RotationAngle == angleDeg)
            return builder;

        builder.Element.RotationAngle = angleDeg;
        builder.Element.CanRotate = true;
        return builder;
    }

    /// <summary>
    /// Zeros the rotation angle of an <see cref="ISleekImage"/> and sets <see cref="ISleekImage.CanRotate"/> to <see langword="false"/>.
    /// </summary>
    /// <remarks>Default value: 0.0f (can't rotate).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekImage> WithNoRotation(this SleekElementBuilder<ISleekImage> builder)
    {
        if (builder.Element is { CanRotate: false, RotationAngle: 0f })
            return builder;

        builder.Element.RotationAngle = 0f;
        builder.Element.CanRotate = false;
        return builder;
    }

    /// <summary>
    /// Sets the tint of an <see cref="ISleekImage"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekTint.NONE"/> (<see cref="Color.white"/>).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekImage> WithTintColor(this SleekElementBuilder<ISleekImage> builder,
        [ValueProvider("SDG.Unturned.ESleekTint"), ValueProvider("UnityEngine.Color")] SleekColor tintColor)
    {
        builder.Element.TintColor = tintColor;
        return builder;
    }

    /// <summary>
    /// Sets <see cref="ISleekImage.ShouldDestroyTexture"/> to <see langword="true"/> on an <see cref="ISleekImage"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (keep texture alive).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekImage> DestroyTexture(this SleekElementBuilder<ISleekImage> builder)
        => builder.WithShouldDestroyTexture(true);

    /// <summary>
    /// Sets <see cref="ISleekImage.ShouldDestroyTexture"/> to <see langword="false"/> on an <see cref="ISleekImage"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (keep texture alive).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekImage> KeepTextureAlive(this SleekElementBuilder<ISleekImage> builder)
        => builder.WithShouldDestroyTexture(false);

    /// <summary>
    /// Sets <see cref="ISleekImage.ShouldDestroyTexture"/> to <paramref name="shouldDestroyTexture"/> on an <see cref="ISleekImage"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (keep texture alive).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekImage> WithShouldDestroyTexture(this SleekElementBuilder<ISleekImage> builder, bool shouldDestroyTexture)
    {
        if (builder.Element.ShouldDestroyTexture != shouldDestroyTexture)
            builder.Element.ShouldDestroyTexture = shouldDestroyTexture;
        return builder;
    }

    /// <summary>
    /// Adds a callback for when an <see cref="ISleekImage"/> is left clicked.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekImage> WhenLeftClicked(this SleekElementBuilder<ISleekImage> builder, Action callback)
    {
        builder.Element.OnClicked += callback;
        return builder;
    }

    /// <summary>
    /// Adds a callback for when an <see cref="ISleekImage"/> is right clicked.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekImage> WhenRightClicked(this SleekElementBuilder<ISleekImage> builder, Action callback)
    {
        builder.Element.OnRightClicked += callback;
        return builder;
    }

    /// <summary>
    /// Adds a callback for when an <see cref="ISleekImage"/> is left or right clicked.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekImage> WhenAnyClicked(this SleekElementBuilder<ISleekImage> builder, Action callback)
    {
        builder.Element.OnClicked += callback;
        builder.Element.OnRightClicked += callback;
        return builder;
    }


    /// <summary>
    /// Adds a callback for when the value in an <see cref="ISleekInt32Field"/> is modified.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekInt32Field> WhenValueTyped(this SleekElementBuilder<ISleekInt32Field> builder, TypedInt32 callback)
    {
        builder.Element.OnValueChanged += callback;
        return builder;
    }

    /// <summary>
    /// Sets the initial value (text) of an <see cref="ISleekInt32Field"/>.
    /// </summary>
    /// <remarks>Default value: 0.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekInt32Field> WithInitialValue(this SleekElementBuilder<ISleekInt32Field> builder, int value)
    {
        if (builder.Element.Value != value)
            builder.Element.Value = value;
        return builder;
    }


    /// <summary>
    /// Sets the text of an <see cref="ISleekLabel"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="null"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<TSleekLabel> WithText<TSleekLabel>(this SleekElementBuilder<TSleekLabel> builder, string text) where TSleekLabel : class, ISleekLabel
    {
        if (!ReferenceEquals(builder.Element.Text, text))
            builder.Element.Text = text;
        return builder;
    }

    /// <summary>
    /// Sets the font style of an <see cref="ISleekLabel"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="FontStyle.Normal"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<TSleekLabel> WithFontStyle<TSleekLabel>(this SleekElementBuilder<TSleekLabel> builder, FontStyle fontStyle) where TSleekLabel : class, ISleekLabel
    {
        if (builder.Element.FontStyle != fontStyle)
            builder.Element.FontStyle = fontStyle;
        return builder;
    }

    /// <summary>
    /// Adds bolding to an <see cref="ISleekLabel"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="FontStyle.Normal"/>. If the font style is already <see cref="FontStyle.Italic"/> it will be set to <see cref="FontStyle.BoldAndItalic"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<TSleekLabel> Bold<TSleekLabel>(this SleekElementBuilder<TSleekLabel> builder) where TSleekLabel : class, ISleekLabel
    {
        FontStyle fontStyle = builder.Element.FontStyle == FontStyle.Italic ? FontStyle.BoldAndItalic : FontStyle.Bold;

        if (builder.Element.FontStyle != fontStyle)
            builder.Element.FontStyle = fontStyle;
        return builder;
    }

    /// <summary>
    /// Adds italics to an <see cref="ISleekLabel"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="FontStyle.Normal"/>. If the font style is already <see cref="FontStyle.Bold"/> it will be set to <see cref="FontStyle.BoldAndItalic"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<TSleekLabel> Italic<TSleekLabel>(this SleekElementBuilder<TSleekLabel> builder) where TSleekLabel : class, ISleekLabel
    {
        FontStyle fontStyle = builder.Element.FontStyle == FontStyle.Bold ? FontStyle.BoldAndItalic : FontStyle.Italic;

        if (builder.Element.FontStyle != fontStyle)
            builder.Element.FontStyle = fontStyle;
        return builder;
    }

    /// <summary>
    /// Adds bold and italics to an <see cref="ISleekLabel"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="FontStyle.Normal"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<TSleekLabel> BoldAndItalic<TSleekLabel>(this SleekElementBuilder<TSleekLabel> builder) where TSleekLabel : class, ISleekLabel
    {
        if (builder.Element.FontStyle != FontStyle.BoldAndItalic)
            builder.Element.FontStyle = FontStyle.BoldAndItalic;
        return builder;
    }

    /// <summary>
    /// Sets the text anchor of an <see cref="ISleekLabel"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="TextAnchor.MiddleCenter"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<TSleekLabel> WithTextAnchor<TSleekLabel>(this SleekElementBuilder<TSleekLabel> builder, TextAnchor textAnchor) where TSleekLabel : class, ISleekLabel
    {
        if (builder.Element.TextAlignment != textAnchor)
            builder.Element.TextAlignment = textAnchor;
        return builder;
    }

    /// <summary>
    /// Sets the text anchor of an <see cref="ISleekLabel"/> in separate components.
    /// </summary>
    /// <remarks>Default values: <see cref="TextAlignment.Center"/>, <see cref="VerticalTextAlignment.Center"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<TSleekLabel> WithTextAnchor<TSleekLabel>(this SleekElementBuilder<TSleekLabel> builder, TextAlignment horizontalAlignment, VerticalTextAlignment verticalAlignment) where TSleekLabel : class, ISleekLabel
    {
        TextAnchor textAnchor = (TextAnchor)((int)horizontalAlignment + (int)verticalAlignment * 3);
        if (builder.Element.TextAlignment != textAnchor)
            builder.Element.TextAlignment = textAnchor;
        return builder;
    }

    /// <summary>
    /// Sets the horizontal text alignment of an <see cref="ISleekLabel"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="TextAlignment.Center"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<TSleekLabel> WithHorizontalTextAlignment<TSleekLabel>(this SleekElementBuilder<TSleekLabel> builder, TextAlignment textAlignment) where TSleekLabel : class, ISleekLabel
    {
        TextAnchor textAnchor = (TextAnchor)((int)builder.Element.TextAlignment / 3 + (int)textAlignment);
        if (builder.Element.TextAlignment != textAnchor)
            builder.Element.TextAlignment = textAnchor;
        return builder;
    }

    /// <summary>
    /// Sets the vertical text alignment of an <see cref="ISleekLabel"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="VerticalTextAlignment.Center"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<TSleekLabel> WithVerticalTextAlignment<TSleekLabel>(this SleekElementBuilder<TSleekLabel> builder, VerticalTextAlignment textAlignment) where TSleekLabel : class, ISleekLabel
    {
        TextAnchor textAnchor = (TextAnchor)((int)builder.Element.TextAlignment % 3 + (int)textAlignment * 3);
        if (builder.Element.TextAlignment != textAnchor)
            builder.Element.TextAlignment = textAnchor;
        return builder;
    }

    /// <summary>
    /// Sets the font size preset of an <see cref="ISleekLabel"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekFontSize.Default"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<TSleekLabel> WithFontSize<TSleekLabel>(this SleekElementBuilder<TSleekLabel> builder, ESleekFontSize fontSize) where TSleekLabel : class, ISleekLabel
    {
        if (builder.Element.FontSize != fontSize)
            builder.Element.FontSize = fontSize;
        return builder;
    }

    /// <summary>
    /// Sets the contrast context of an <see cref="ISleekLabel"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ETextContrastContext.Default"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<TSleekLabel> WithContrastContext<TSleekLabel>(this SleekElementBuilder<TSleekLabel> builder, ETextContrastContext contrastContext) where TSleekLabel : class, ISleekLabel
    {
        if (builder.Element.TextContrastContext != contrastContext)
            builder.Element.TextContrastContext = contrastContext;
        return builder;
    }

    /// <summary>
    /// Sets the text color of an <see cref="ISleekLabel"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekTint.FONT"/> (from settings, default value <c>RGB01(0.9, 0.9, 0.9)</c>).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekLabel> WithTextColor(this SleekElementBuilder<ISleekLabel> builder,
        [ValueProvider("SDG.Unturned.ESleekTint"), ValueProvider("UnityEngine.Color")] SleekColor textColor)
    {
        builder.Element.TextColor = textColor;
        return builder;
    }

    /// <summary>
    /// Sets the text color of an <see cref="ISleekButton"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekTint.FONT"/> (from settings, default value <c>RGB01(0.9, 0.9, 0.9)</c>).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekButton> WithTextColor(this SleekElementBuilder<ISleekButton> builder,
        [ValueProvider("SDG.Unturned.ESleekTint"), ValueProvider("UnityEngine.Color")] SleekColor textColor)
    {
        builder.Element.TextColor = textColor;
        return builder;
    }

    /// <summary>
    /// Sets the text color of an <see cref="ISleekBox"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekTint.FONT"/> (from settings, default value <c>RGB01(0.9, 0.9, 0.9)</c>).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekBox> WithTextColor(this SleekElementBuilder<ISleekBox> builder,
        [ValueProvider("SDG.Unturned.ESleekTint"), ValueProvider("UnityEngine.Color")] SleekColor textColor)
    {
        builder.Element.TextColor = textColor;
        return builder;
    }

    /// <summary>
    /// Sets the text color of an <see cref="ISleekField"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekTint.FONT"/> (from settings, default value <c>RGB01(0.9, 0.9, 0.9)</c>).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekField> WithTextColor(this SleekElementBuilder<ISleekField> builder,
        [ValueProvider("SDG.Unturned.ESleekTint"), ValueProvider("UnityEngine.Color")] SleekColor textColor)
    {
        builder.Element.TextColor = textColor;
        return builder;
    }


    /// <summary>
    /// Sets the text color of an <see cref="ISleekNumericField"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekTint.FONT"/> (from settings, default value <c>RGB01(0.9, 0.9, 0.9)</c>).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<TSleekNumericField> WithTextColor<TSleekNumericField>(this SleekElementBuilder<TSleekNumericField> builder,
        [ValueProvider("SDG.Unturned.ESleekTint"), ValueProvider("UnityEngine.Color")] SleekColor textColor) where TSleekNumericField : class, ISleekNumericField
    {
        builder.Element.TextColor = textColor;
        return builder;
    }

    /// <summary>
    /// Sets the background color of an <see cref="ISleekNumericField"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekTint.BACKGROUND"/> (from settings, default value <c>RGB01(0.9, 0.9, 0.9)</c>).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<TSleekNumericField> WithBackgroundColor<TSleekNumericField>(this SleekElementBuilder<TSleekNumericField> builder,
        [ValueProvider("SDG.Unturned.ESleekTint"), ValueProvider("UnityEngine.Color")] SleekColor backgroundColor) where TSleekNumericField : class, ISleekNumericField
    {
        builder.Element.BackgroundColor = backgroundColor;
        return builder;
    }


    /// <summary>
    /// Sets <see cref="ISleekScrollView.ScaleContentToWidth"/> to <see langword="true"/> for an <see cref="ISleekScrollView"/>.
    /// <para>Should <see cref="ISleekScrollView.ContentScaleFactor" /> be applied in the x direction?</para>
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (don't scale content).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekScrollView> ScaleContentToWidth(this SleekElementBuilder<ISleekScrollView> builder)
        => builder.WithShouldScaleContentToWidth(true);

    /// <summary>
    /// Sets <see cref="ISleekScrollView.ScaleContentToWidth"/> to <see langword="false"/> for an <see cref="ISleekScrollView"/>.
    /// <para>Should <see cref="ISleekScrollView.ContentScaleFactor" /> be applied in the x direction?</para>
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (don't scale content).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekScrollView> DoNotScaleContentToWidth(this SleekElementBuilder<ISleekScrollView> builder)
        => builder.WithShouldScaleContentToWidth(false);

    /// <summary>
    /// Sets <see cref="ISleekScrollView.ScaleContentToWidth"/> to <paramref name="shouldScaleContentToWidth"/> for an <see cref="ISleekScrollView"/>.
    /// <para>Should <see cref="ISleekScrollView.ContentScaleFactor" /> be applied in the x direction?</para>
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (don't scale content).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekScrollView> WithShouldScaleContentToWidth(this SleekElementBuilder<ISleekScrollView> builder, bool shouldScaleContentToWidth)
    {
        if (builder.Element.ScaleContentToWidth != shouldScaleContentToWidth)
            builder.Element.ScaleContentToWidth = shouldScaleContentToWidth;
        return builder;
    }

    /// <summary>
    /// Sets <see cref="ISleekScrollView.ScaleContentToHeight"/> to <see langword="true"/> for an <see cref="ISleekScrollView"/>.
    /// <para>Should <see cref="ISleekScrollView.ContentScaleFactor" /> be applied in the y direction?</para>
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (don't scale content).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekScrollView> ScaleContentToHeight(this SleekElementBuilder<ISleekScrollView> builder)
        => builder.WithShouldScaleContentToHeight(true);

    /// <summary>
    /// Sets <see cref="ISleekScrollView.ScaleContentToHeight"/> to <see langword="false"/> for an <see cref="ISleekScrollView"/>.
    /// <para>Should <see cref="ISleekScrollView.ContentScaleFactor" /> be applied in the y direction?</para>
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (don't scale content).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekScrollView> DoNotScaleContentToHeight(this SleekElementBuilder<ISleekScrollView> builder)
        => builder.WithShouldScaleContentToHeight(false);

    /// <summary>
    /// Sets <see cref="ISleekScrollView.ScaleContentToHeight"/> to <paramref name="verticalScrollbarVisibility"/> for an <see cref="ISleekScrollView"/>.
    /// <para>Should <see cref="ISleekScrollView.ContentScaleFactor" /> be applied in the y direction?</para>
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (don't scale content).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekScrollView> WithShouldScaleContentToHeight(this SleekElementBuilder<ISleekScrollView> builder, bool verticalScrollbarVisibility)
    {
        if (builder.Element.ScaleContentToHeight != verticalScrollbarVisibility)
            builder.Element.ScaleContentToHeight = verticalScrollbarVisibility;
        return builder;
    }

    /// <summary>
    /// Sets the initial scale factor of an <see cref="ISleekScrollView"/>.
    /// <para>How zoomed in the content is.</para>
    /// </summary>
    /// <remarks>Default value: 0.0f.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekScrollView> WithInitialScaleFactor(this SleekElementBuilder<ISleekScrollView> builder, float scaleFactor)
    {
        if (builder.Element.ContentScaleFactor != scaleFactor)
            builder.Element.ContentScaleFactor = scaleFactor;
        return builder;
    }

    /// <summary>
    /// Sets <see cref="ISleekScrollView.ReduceWidthWhenScrollbarVisible"/> to <see langword="true"/> for an <see cref="ISleekScrollView"/>.
    /// <para>The scrollbar being hidden will slightly increase the width of the scroll view.</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (reduce width).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekScrollView> ReduceWidthWhenScrollbarVisible(this SleekElementBuilder<ISleekScrollView> builder)
        => builder.WithShouldScaleContentToHeight(true);

    /// <summary>
    /// Sets <see cref="ISleekScrollView.ReduceWidthWhenScrollbarVisible"/> to <see langword="false"/> for an <see cref="ISleekScrollView"/>.
    /// <para>The scrollbar being hidden will not change the width of the scroll view.</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (reduce width).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekScrollView> KeepWidthWhenScrollbarVisible(this SleekElementBuilder<ISleekScrollView> builder)
        => builder.WithShouldScaleContentToHeight(false);

    /// <summary>
    /// Sets <see cref="ISleekScrollView.ReduceWidthWhenScrollbarVisible"/> to <paramref name="reduceWidthWhenScrollbarVisible"/> for an <see cref="ISleekScrollView"/>.
    /// <para>The scrollbar being hidden can slightly increase the width of the scroll view.</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (reduce width).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekScrollView> WithScrollbarVisibleWidthBehavior(this SleekElementBuilder<ISleekScrollView> builder, bool reduceWidthWhenScrollbarVisible)
    {
        if (builder.Element.ReduceWidthWhenScrollbarVisible != reduceWidthWhenScrollbarVisible)
            builder.Element.ReduceWidthWhenScrollbarVisible = reduceWidthWhenScrollbarVisible;
        return builder;
    }

    /// <summary>
    /// Sets <see cref="ISleekScrollView.ScaleContentToHeight"/> to <see langword="true"/> for an <see cref="ISleekScrollView"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekScrollbarVisibility.Default"/> (vertical scrollbar visible).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekScrollView> WithVerticalScrollbar(this SleekElementBuilder<ISleekScrollView> builder)
        => builder.WithVerticalScrollbarVisibility(ESleekScrollbarVisibility.Default);

    /// <summary>
    /// Sets <see cref="ISleekScrollView.ScaleContentToHeight"/> to <see langword="false"/> for an <see cref="ISleekScrollView"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekScrollbarVisibility.Default"/> (vertical scrollbar visible).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekScrollView> WithoutVerticalScrollbar(this SleekElementBuilder<ISleekScrollView> builder)
        => builder.WithVerticalScrollbarVisibility(ESleekScrollbarVisibility.Hidden);

    /// <summary>
    /// Sets <see cref="ISleekScrollView.VerticalScrollbarVisibility"/> to <paramref name="verticalScrollbarVisibility"/> for an <see cref="ISleekScrollView"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekScrollbarVisibility.Default"/> (vertical scrollbar visible).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekScrollView> WithVerticalScrollbarVisibility(this SleekElementBuilder<ISleekScrollView> builder, ESleekScrollbarVisibility verticalScrollbarVisibility)
    {
        builder.Element.VerticalScrollbarVisibility = verticalScrollbarVisibility;
        return builder;
    }

    /// <summary>
    /// Sets the content size offset of an <see cref="ISleekScrollView"/>.
    /// <para>Coordintes: (size X, size Y).</para>
    /// </summary>
    /// <remarks>Default value: (0.0f, 0.0f).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekScrollView> WithContentSizeOffset(this SleekElementBuilder<ISleekScrollView> builder, float contentSizeOffsetX, float contentSizeOffsetY)
        => builder.WithContentSizeOffset(new Vector2(contentSizeOffsetX, contentSizeOffsetY));

    /// <summary>
    /// Sets the content size offset of an <see cref="ISleekScrollView"/>.
    /// <para>Coordintes: (size X, size Y).</para>
    /// </summary>
    /// <remarks>Default value: (0.0f, 0.0f).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekScrollView> WithContentSizeOffset(this SleekElementBuilder<ISleekScrollView> builder, Vector2 contentSizeOffset)
    {
        if (builder.Element.ContentSizeOffset != contentSizeOffset)
            builder.Element.ContentSizeOffset = contentSizeOffset;
        return builder;
    }

    /// <summary>
    /// Sets the normalized (0-1) center/offset of an <see cref="ISleekScrollView"/>.
    /// <para>Coordintes: (Left to right, top to bottom).</para>
    /// </summary>
    /// <remarks>Default value: (0.0f, 0.0f).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekScrollView> WithInitialScrollOffset(this SleekElementBuilder<ISleekScrollView> builder, float contentSizeOffsetX, float contentSizeOffsetY)
        => builder.WithContentSizeOffset(new Vector2(contentSizeOffsetX, contentSizeOffsetY));

    /// <summary>
    /// Sets the normalized (0-1) center/offset of an <see cref="ISleekScrollView"/>.
    /// <para>Coordintes: (Left to right, top to bottom).</para>
    /// </summary>
    /// <remarks>Default value: (0.0f, 0.0f).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekScrollView> WithInitialScrollOffset(this SleekElementBuilder<ISleekScrollView> builder, Vector2 contentSizeOffset)
    {
        builder.Element.NormalizedStateCenter = contentSizeOffset;
        return builder;
    }

    /// <summary>
    /// Sets <see cref="ISleekScrollView.HandleScrollWheel"/> to <see langword="true"/> for an <see cref="ISleekScrollView"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (handle scrolling).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekScrollView> HandleScrolling(this SleekElementBuilder<ISleekScrollView> builder)
        => builder.ShouldHandleScrolling(true);

    /// <summary>
    /// Sets <see cref="ISleekScrollView.HandleScrollWheel"/> to <see langword="false"/> for an <see cref="ISleekScrollView"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (handle scrolling).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekScrollView> DoNotHandleScrolling(this SleekElementBuilder<ISleekScrollView> builder)
        => builder.ShouldHandleScrolling(false);

    /// <summary>
    /// Sets <see cref="ISleekScrollView.HandleScrollWheel"/> to <paramref name="handleScrolling"/> for an <see cref="ISleekScrollView"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (handle scrolling).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekScrollView> ShouldHandleScrolling(this SleekElementBuilder<ISleekScrollView> builder, bool handleScrolling)
    {
        builder.Element.HandleScrollWheel = handleScrolling;
        return builder;
    }

    /// <summary>
    /// Sets the background color of an <see cref="ISleekScrollView"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekTint.BACKGROUND"/> (from settings, default value <c>RGB01(0.9, 0.9, 0.9)</c>).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekScrollView> WithBackgroundColor(this SleekElementBuilder<ISleekScrollView> builder,
        [ValueProvider("SDG.Unturned.ESleekTint"), ValueProvider("UnityEngine.Color")] SleekColor backgroundColor)
    {
        builder.Element.BackgroundColor = backgroundColor;
        return builder;
    }

    /// <summary>
    /// Sets the foreground color of an <see cref="ISleekScrollView"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekTint.FOREGROUND"/> (from settings, default value <c>RGB01(0.9, 0.9, 0.9)</c>).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekScrollView> WithForegroundColor(this SleekElementBuilder<ISleekScrollView> builder,
        [ValueProvider("SDG.Unturned.ESleekTint"), ValueProvider("UnityEngine.Color")] SleekColor foregroundColor)
    {
        builder.Element.ForegroundColor = foregroundColor;
        return builder;
    }

    /// <summary>
    /// Sets <see cref="ISleekScrollView.ContentUseManualLayout"/> to <see langword="true"/> for an <see cref="ISleekScrollView"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (manual layout).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekScrollView> WithManualLayout(this SleekElementBuilder<ISleekScrollView> builder)
        => builder.WithShouldUseManualLayout(true);

    /// <summary>
    /// Sets <see cref="ISleekScrollView.ContentUseManualLayout"/> to <see langword="false"/> for an <see cref="ISleekScrollView"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (manual layout).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekScrollView> WithAutomaticLayout(this SleekElementBuilder<ISleekScrollView> builder)
        => builder.WithShouldUseManualLayout(false);

    /// <summary>
    /// Sets <see cref="ISleekScrollView.ContentUseManualLayout"/> to <paramref name="useManualLayout"/> for an <see cref="ISleekScrollView"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (manual layout).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekScrollView> WithShouldUseManualLayout(this SleekElementBuilder<ISleekScrollView> builder, bool useManualLayout)
    {
        builder.Element.ContentUseManualLayout = useManualLayout;
        return builder;
    }

    /// <summary>
    /// Sets <see cref="ISleekScrollView.AlignContentToBottom"/> to <see langword="true"/> for an <see cref="ISleekScrollView"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (top aligned).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekScrollView> WithBottomAlignedContent(this SleekElementBuilder<ISleekScrollView> builder)
        => builder.WithShouldAlignContentToBottom(true);

    /// <summary>
    /// Sets <see cref="ISleekScrollView.AlignContentToBottom"/> to <see langword="false"/> for an <see cref="ISleekScrollView"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (top aligned).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekScrollView> WithTopAlignedContent(this SleekElementBuilder<ISleekScrollView> builder)
        => builder.WithShouldAlignContentToBottom(false);

    /// <summary>
    /// Sets <see cref="ISleekScrollView.AlignContentToBottom"/> to <paramref name="alignContentToBottom"/> for an <see cref="ISleekScrollView"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (top aligned).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekScrollView> WithShouldAlignContentToBottom(this SleekElementBuilder<ISleekScrollView> builder, bool alignContentToBottom)
    {
        builder.Element.AlignContentToBottom = alignContentToBottom;
        return builder;
    }

    /// <summary>
    /// Sets <see cref="ISleekScrollView.IsRaycastTarget"/> to <see langword="true"/> on an <see cref="ISleekScrollView"/>.
    /// <para>Should the scroll view consume clicks?</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (raycast target).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekScrollView> RaycastTarget(this SleekElementBuilder<ISleekScrollView> builder)
        => builder.WithIsRaycastTarget(true);

    /// <summary>
    /// Sets <see cref="ISleekScrollView.IsRaycastTarget"/> to <see langword="false"/> on an <see cref="ISleekScrollView"/>.
    /// <para>Should the scroll view consume clicks?</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (raycast target).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekScrollView> RaycastIgnored(this SleekElementBuilder<ISleekScrollView> builder)
        => builder.WithIsRaycastTarget(false);

    /// <summary>
    /// Sets <see cref="ISleekScrollView.IsRaycastTarget"/> to <paramref name="isRaycastTarget"/> on an <see cref="ISleekScrollView"/>.
    /// <para>Should the scroll view consume clicks?</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (raycast target).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekScrollView> WithIsRaycastTarget(this SleekElementBuilder<ISleekScrollView> builder, bool isRaycastTarget)
    {
        if (builder.Element.IsRaycastTarget != isRaycastTarget)
            builder.Element.IsRaycastTarget = isRaycastTarget;
        return builder;
    }

    /// <summary>
    /// Sets <see cref="ISleekScrollView.NormalizedVerticalPosition"/> to 0.0f.
    /// </summary>
    /// <remarks>Default value: 0.0f (start at top).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekScrollView> StartAtTop(this SleekElementBuilder<ISleekScrollView> builder)
    {
        if (builder.Element.NormalizedVerticalPosition != 0f)
            builder.Element.ScrollToTop();
        return builder;
    }

    /// <summary>
    /// Sets <see cref="ISleekScrollView.NormalizedVerticalPosition"/> to 1.0f.
    /// </summary>
    /// <remarks>Default value: 0.0f (start at top).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekScrollView> StartAtBottom(this SleekElementBuilder<ISleekScrollView> builder)
    {
        if (builder.Element.NormalizedVerticalPosition != 1f)
            builder.Element.ScrollToBottom();
        return builder;
    }

    /// <summary>
    /// Adds a callback for when the normalized (0-1) center/offset in an <see cref="ISleekScrollView"/> is changed.
    /// <para>Top to bottom or left to right depending on orientation.</para>
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekScrollView> WhenScrollOffsetChanged(this SleekElementBuilder<ISleekScrollView> builder, Action<Vector2> callback)
    {
        builder.Element.OnNormalizedValueChanged += callback;
        return builder;
    }


    /// <summary>
    /// Sets the normalized (0-1) scroll progress of an <see cref="ISleekSlider"/>.
    /// <para>Top to bottom or left to right depending on orientation.</para>
    /// </summary>
    /// <remarks>Default value: 0.0f.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekSlider> WithInitialProgress(this SleekElementBuilder<ISleekSlider> builder, float progress)
    {
        progress = Mathf.Clamp01(progress);
        if (builder.Element.Value != progress)
            builder.Element.Value = progress;
        return builder;
    }

    /// <summary>
    /// Sets the orientation of an <see cref="ISleekSlider"/> to <see cref="ESleekOrientation.VERTICAL"/>.
    /// <para>Is the slider vertical or horizontal?</para>
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekOrientation.VERTICAL"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekSlider> WithVerticalOrientation(this SleekElementBuilder<ISleekSlider> builder)
        => builder.WithOrientation(ESleekOrientation.VERTICAL);

    /// <summary>
    /// Sets the orientation of an <see cref="ISleekSlider"/> to <see cref="ESleekOrientation.HORIZONTAL"/>.
    /// <para>Is the slider vertical or horizontal?</para>
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekOrientation.VERTICAL"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekSlider> WithHorizontalOrientation(this SleekElementBuilder<ISleekSlider> builder)
        => builder.WithOrientation(ESleekOrientation.HORIZONTAL);

    /// <summary>
    /// Sets the orientation of an <see cref="ISleekSlider"/>.
    /// <para>Is the slider vertical or horizontal?</para>
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekOrientation.VERTICAL"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekSlider> WithOrientation(this SleekElementBuilder<ISleekSlider> builder, ESleekOrientation orientation)
    {
        if (builder.Element.Orientation != orientation)
            builder.Element.Orientation = orientation;
        return builder;
    }

    /// <summary>
    /// Sets <see cref="ISleekSlider.IsInteractable"/> to <see langword="true"/> on an <see cref="ISleekSlider"/>.
    /// <para>Interactability defines if the user can interact with the slider (change it's value).</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (interactable).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekSlider> Interactable(this SleekElementBuilder<ISleekSlider> builder)
        => builder.WithIsInteractable(true);

    /// <summary>
    /// Sets <see cref="ISleekSlider.IsInteractable"/> to <see langword="false"/> on an <see cref="ISleekSlider"/>.
    /// <para>Interactability defines if the user can interact with the slider (change it's value).</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (interactable).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekSlider> NotInteractable(this SleekElementBuilder<ISleekSlider> builder)
        => builder.WithIsInteractable(false);

    /// <summary>
    /// Sets <see cref="ISleekSlider.IsInteractable"/> to <paramref name="isInteractable"/> on an <see cref="ISleekSlider"/>.
    /// <para>Interactability defines if the user can interact with the slider (change it's value).</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (interactable).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekSlider> WithIsInteractable(this SleekElementBuilder<ISleekSlider> builder, bool isInteractable)
    {
        if (builder.Element.IsInteractable != isInteractable)
            builder.Element.IsInteractable = isInteractable;
        return builder;
    }

    /// <summary>
    /// Sets the background color of an <see cref="ISleekSlider"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekTint.BACKGROUND"/> (from settings, default value <c>RGB01(0.9, 0.9, 0.9)</c>).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekSlider> WithBackgroundColor(this SleekElementBuilder<ISleekSlider> builder,
        [ValueProvider("SDG.Unturned.ESleekTint"), ValueProvider("UnityEngine.Color")] SleekColor backgroundColor)
    {
        builder.Element.BackgroundColor = backgroundColor;
        return builder;
    }

    /// <summary>
    /// Sets the foreground color of an <see cref="ISleekSlider"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekTint.FOREGROUND"/> (from settings, default value <c>RGB01(0.9, 0.9, 0.9)</c>).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekSlider> WithForegroundColor(this SleekElementBuilder<ISleekSlider> builder,
        [ValueProvider("SDG.Unturned.ESleekTint"), ValueProvider("UnityEngine.Color")] SleekColor foregroundColor)
    {
        builder.Element.ForegroundColor = foregroundColor;
        return builder;
    }

    /// <summary>
    /// Adds a callback for when the scroll progress in an <see cref="ISleekSlider"/> is changed.
    /// <para>Top to bottom or left to right depending on orientation.</para>
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekSlider> WhenProgressChanged(this SleekElementBuilder<ISleekSlider> builder, Dragged callback)
    {
        builder.Element.OnValueChanged += callback;
        return builder;
    }


    /// <summary>
    /// Sets the sprite of an <see cref="ISleekSprite"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="null"/>.</remarks>
    /// <seealso cref="WithSprite(SleekElementBuilder{ISleekSprite},Bundle,string)"/>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekSprite> WithSprite(this SleekElementBuilder<ISleekSprite> builder, Sprite? sprite)
    {
        if (builder.Element.Sprite != sprite)
            builder.Element.Sprite = sprite;
        return builder;
    }

    /// <summary>
    /// Loads the sprite of an <see cref="ISleekSprite"/>.
    /// </summary>
    /// <exception cref="ArgumentException">Unable to find a <see cref="Sprite"/> at <paramref name="path"/> in <paramref name="bundle"/>.</exception>
    /// <remarks>Default value: <see langword="null"/>.</remarks>
    /// <seealso cref="WithSprite(SleekElementBuilder{ISleekSprite},Sprite)"/>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekSprite> WithSprite(this SleekElementBuilder<ISleekSprite> builder, Bundle bundle, string path)
    {
        Sprite? sprite = bundle.load<Sprite>(path);
        if (sprite == null)
            throw new ArgumentException("Unable to find a sprite in the provided bundle.", nameof(path));

        builder.Element.Sprite = sprite;
        return builder;
    }

    /// <summary>
    /// Sets the tint of an <see cref="ISleekSprite"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekTint.NONE"/> (<see cref="Color.white"/>).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekSprite> WithTintColor(this SleekElementBuilder<ISleekSprite> builder,
        [ValueProvider("SDG.Unturned.ESleekTint"), ValueProvider("UnityEngine.Color")] SleekColor tintColor)
    {
        builder.Element.TintColor = tintColor;
        return builder;
    }

    /// <summary>
    /// Adds a callback for when an <see cref="ISleekSprite"/> is left clicked.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekSprite> WhenLeftClicked(this SleekElementBuilder<ISleekSprite> builder, Action callback)
    {
        builder.Element.OnClicked += callback;
        return builder;
    }

    /// <summary>
    /// Sets <see cref="ISleekSprite.IsRaycastTarget"/> to <see langword="true"/> on an <see cref="ISleekSprite"/>.
    /// <para>Should the sprite consume clicks?</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (raycast target).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekSprite> RaycastTarget(this SleekElementBuilder<ISleekSprite> builder)
        => builder.WithIsRaycastTarget(true);

    /// <summary>
    /// Sets <see cref="ISleekSprite.IsRaycastTarget"/> to <see langword="false"/> on an <see cref="ISleekSprite"/>.
    /// <para>Should the sprite consume clicks?</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (raycast target).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekSprite> RaycastIgnored(this SleekElementBuilder<ISleekSprite> builder)
        => builder.WithIsRaycastTarget(false);

    /// <summary>
    /// Sets <see cref="ISleekSprite.IsRaycastTarget"/> to <paramref name="isRaycastTarget"/> on an <see cref="ISleekSprite"/>.
    /// <para>Should the sprite consume clicks?</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (raycast target).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekSprite> WithIsRaycastTarget(this SleekElementBuilder<ISleekSprite> builder, bool isRaycastTarget)
    {
        if (builder.Element.IsRaycastTarget != isRaycastTarget)
            builder.Element.IsRaycastTarget = isRaycastTarget;
        return builder;
    }

    /// <summary>
    /// Sets the draw method of an <see cref="ISleekSprite"/> to <see cref="ESleekSpriteType.Sliced"/>.
    /// <para>Edges of a sprite are separated from the center, defined in Unity's sprite editor.</para>
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekSpriteType.Tiled"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekSprite> WithSlicedDrawMethod(this SleekElementBuilder<ISleekSprite> builder)
    {
        if (builder.Element.DrawMethod != ESleekSpriteType.Sliced)
            builder.Element.DrawMethod = ESleekSpriteType.Sliced;
        return builder;
    }

    /// <summary>
    /// Sets the draw method of an <see cref="ISleekSprite"/> to <see cref="ESleekSpriteType.Regular"/>.
    /// <para>Only draws one image with no manipulation.</para>
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekSpriteType.Tiled"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekSprite> WithSingularDrawMethod(this SleekElementBuilder<ISleekSprite> builder)
    {
        if (builder.Element.DrawMethod != ESleekSpriteType.Regular)
            builder.Element.DrawMethod = ESleekSpriteType.Regular;
        return builder;
    }

    /// <summary>
    /// Sets the draw method of an <see cref="ISleekSprite"/> to <see cref="ESleekSpriteType.Tiled"/>.
    /// <para>Images are repeated in the positive X and Y directions (or in all directions in some implementations).</para>
    /// </summary>
    /// <param name="tilingDimensions">Number of images to tile in each direction. Used for the UIToolkit <see cref="Glazier"/> set.</param>
    /// <remarks>Default value: <see cref="ESleekSpriteType.Tiled"/> (you still should pass tiling dimensions if you want more than a 1x1).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekSprite> WithTiledDrawMethod(this SleekElementBuilder<ISleekSprite> builder, Vector2Int tilingDimensions)
    {
        if (builder.Element.DrawMethod != ESleekSpriteType.Tiled)
            builder.Element.DrawMethod = ESleekSpriteType.Tiled;
        builder.Element.TileRepeatHintForUITK = tilingDimensions;
        return builder;
    }

    /// <summary>
    /// Sets the draw method of an <see cref="ISleekSprite"/> to <see cref="ESleekSpriteType.Regular"/>.
    /// <para>Defines how sprites are drawn. Use <see cref="WithTiledDrawMethod(SleekElementBuilder{ISleekSprite},Vector2Int)"/> to set the mode to <see cref="ESleekSpriteType.Tiled"/> (this method throws an exception in that case).</para>
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekSpriteType.Tiled"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    /// <exception cref="ArgumentException">Use <see cref="WithTiledDrawMethod(SleekElementBuilder{ISleekSprite},Vector2Int)"/> to set tiling mode.</exception>
    public static SleekElementBuilder<ISleekSprite> WithDrawMethod(this SleekElementBuilder<ISleekSprite> builder, ESleekSpriteType drawMethod)
    {
        if (drawMethod == ESleekSpriteType.Tiled)
            throw new ArgumentException("Use the overload that takes a Vector2Int for the tiled draw method.", nameof(drawMethod));

        if (builder.Element.DrawMethod != drawMethod)
            builder.Element.DrawMethod = drawMethod;
        return builder;
    }


    /// <summary>
    /// Sets the checked state of an <see cref="ISleekToggle"/> to checked.
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (not checked).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekToggle> InitiallyChecked(this SleekElementBuilder<ISleekToggle> builder)
        => builder.WithInitialState(true);

    /// <summary>
    /// Sets the checked state of an <see cref="ISleekToggle"/> to unchecked.
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (not checked).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekToggle> InitiallyUnchecked(this SleekElementBuilder<ISleekToggle> builder)
        => builder.WithInitialState(false);

    /// <summary>
    /// Sets the checked state of an <see cref="ISleekToggle"/>.
    /// </summary>
    /// <remarks>Default value: <see langword="false"/> (not checked).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekToggle> WithInitialState(this SleekElementBuilder<ISleekToggle> builder, bool isChecked)
    {
        if (builder.Element.Value != isChecked)
            builder.Element.Value = isChecked;
        return builder;
    }

    /// <summary>
    /// Sets <see cref="ISleekToggle.IsInteractable"/> to <see langword="true"/> on an <see cref="ISleekToggle"/>.
    /// <para>Interactability defines if the user can interact with the check box (change it's value).</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (interactable).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekToggle> Interactable(this SleekElementBuilder<ISleekToggle> builder)
        => builder.WithIsInteractable(true);

    /// <summary>
    /// Sets <see cref="ISleekToggle.IsInteractable"/> to <see langword="false"/> on an <see cref="ISleekToggle"/>.
    /// <para>Interactability defines if the user can interact with the check box (change it's value).</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (interactable).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekToggle> NotInteractable(this SleekElementBuilder<ISleekToggle> builder)
        => builder.WithIsInteractable(false);

    /// <summary>
    /// Sets <see cref="ISleekToggle.IsInteractable"/> to <paramref name="isInteractable"/> on an <see cref="ISleekToggle"/>.
    /// <para>Interactability defines if the user can interact with the check box (change it's value).</para>
    /// </summary>
    /// <remarks>Default value: <see langword="true"/> (interactable).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekToggle> WithIsInteractable(this SleekElementBuilder<ISleekToggle> builder, bool isInteractable)
    {
        if (builder.Element.IsInteractable != isInteractable)
            builder.Element.IsInteractable = isInteractable;
        return builder;
    }

    /// <summary>
    /// Sets the background color of an <see cref="ISleekToggle"/>.
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekTint.BACKGROUND"/> (from settings, default value <c>RGB01(0.9, 0.9, 0.9)</c>).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekToggle> WithBackgroundColor(this SleekElementBuilder<ISleekToggle> builder,
        [ValueProvider("SDG.Unturned.ESleekTint"), ValueProvider("UnityEngine.Color")] SleekColor backgroundColor)
    {
        builder.Element.BackgroundColor = backgroundColor;
        return builder;
    }

    /// <summary>
    /// Sets the foreground color of an <see cref="ISleekToggle"/>.
    /// <para>The foreground color is the check mark.</para>
    /// </summary>
    /// <remarks>Default value: <see cref="ESleekTint.FOREGROUND"/> (from settings, default value <c>RGB01(0.9, 0.9, 0.9)</c>).</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekToggle> WithForegroundColor(this SleekElementBuilder<ISleekToggle> builder,
        [ValueProvider("SDG.Unturned.ESleekTint"), ValueProvider("UnityEngine.Color")] SleekColor foregroundColor)
    {
        builder.Element.ForegroundColor = foregroundColor;
        return builder;
    }

    /// <summary>
    /// Adds a callback for when the checked state of an <see cref="ISleekToggle"/> is changed.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekToggle> WhenStateChanged(this SleekElementBuilder<ISleekToggle> builder, Toggled callback)
    {
        builder.Element.OnValueChanged += callback;
        return builder;
    }


    /// <summary>
    /// Adds a callback for when the value in an <see cref="ISleekUInt16Field"/> is modified.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekUInt16Field> WhenValueTyped(this SleekElementBuilder<ISleekUInt16Field> builder, TypedUInt16 callback)
    {
        builder.Element.OnValueChanged += callback;
        return builder;
    }

    /// <summary>
    /// Sets the initial value (text) of an <see cref="ISleekUInt16Field"/>.
    /// </summary>
    /// <remarks>Default value: 0.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekUInt16Field> WithInitialValue(this SleekElementBuilder<ISleekUInt16Field> builder, ushort value)
    {
        if (builder.Element.Value != value)
            builder.Element.Value = value;
        return builder;
    }

    /// <summary>
    /// Sets the minimum value of an <see cref="ISleekUInt16Field"/>.
    /// </summary>
    /// <remarks>Default value: 0.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekUInt16Field> WithMinimumValue(this SleekElementBuilder<ISleekUInt16Field> builder, ushort value)
    {
        if (builder.Element.MinValue != value)
            builder.Element.MinValue = value;
        return builder;
    }

    /// <summary>
    /// Sets the maximum value of an <see cref="ISleekUInt16Field"/>.
    /// </summary>
    /// <remarks>Default value: 65535.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekUInt16Field> WithMaximumValue(this SleekElementBuilder<ISleekUInt16Field> builder, ushort value)
    {
        if (builder.Element.MaxValue != value)
            builder.Element.MaxValue = value;
        return builder;
    }


    /// <summary>
    /// Adds a callback for when the value in an <see cref="ISleekUInt32Field"/> is modified.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekUInt32Field> WhenValueTyped(this SleekElementBuilder<ISleekUInt32Field> builder, TypedUInt32 callback)
    {
        builder.Element.OnValueChanged += callback;
        return builder;
    }

    /// <summary>
    /// Sets the initial value (text) of an <see cref="ISleekUInt32Field"/>.
    /// </summary>
    /// <remarks>Default value: 0.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekUInt32Field> WithInitialValue(this SleekElementBuilder<ISleekUInt32Field> builder, uint value)
    {
        if (builder.Element.Value != value)
            builder.Element.Value = value;
        return builder;
    }


    /// <summary>
    /// Adds a callback for when the value in an <see cref="ISleekUInt8Field"/> is modified.
    /// </summary>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekUInt8Field> WhenValueTyped(this SleekElementBuilder<ISleekUInt8Field> builder, TypedByte callback)
    {
        builder.Element.OnValueChanged += callback;
        return builder;
    }

    /// <summary>
    /// Sets the initial value (text) of an <see cref="ISleekUInt8Field"/>.
    /// </summary>
    /// <remarks>Default value: 0.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<ISleekUInt8Field> WithInitialValue(this SleekElementBuilder<ISleekUInt8Field> builder, byte value)
    {
        if (builder.Element.Value != value)
            builder.Element.Value = value;
        return builder;
    }


    /// <summary>
    /// Sets the tooltip text of an <see cref="ISleekWithTooltip"/>.
    /// <para>Text that will show when hovering over the element.</para>
    /// </summary>
    /// <remarks>Default value: <see langword="null"/>.</remarks>
    /// <exception cref="NotImplementedException">Not implemented in the current <see cref="Glazier"/> type.</exception>
    public static SleekElementBuilder<TSleekWithTooltip> WithTooltip<TSleekWithTooltip>(this SleekElementBuilder<TSleekWithTooltip> builder, string? tooltip) where TSleekWithTooltip : class, ISleekWithTooltip
    {
        if (tooltip is { Length: 0 })
            tooltip = null;

        if (!ReferenceEquals(builder.Element.TooltipText, tooltip))
            builder.Element.TooltipText = tooltip;
        return builder;
    }
}
