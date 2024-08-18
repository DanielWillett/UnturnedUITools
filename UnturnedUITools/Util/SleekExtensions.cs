using DanielWillett.UITools.API;
using SDG.Unturned;
using System;

namespace DanielWillett.UITools.Util;

/// <summary>
/// Extensions for <see cref="ISleekElement"/> and <see cref="SleekElementBuilder{TElement}"/>.
/// </summary>
public static class SleekExtensions
{
    /// <summary>
    /// Create a <see cref="SleekElementBuilder{TElement}"/> for the given element.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static SleekElementBuilder<TElement> Configure<TElement>(this TElement element) where TElement : class, ISleekElement
    {
        ThreadUtil.assertIsGameThread();

        return new SleekElementBuilder<TElement>(element);
    }

    /// <summary>
    /// Enumerate through the children of <paramref name="element"/>.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static SleekChildEnumerator AsEnumerable(this ISleekElement element)
    {
        ThreadUtil.assertIsGameThread();

        return new SleekChildEnumerator(element);
    }

    /// <summary>
    /// Enumerate through the children of <paramref name="element"/>.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static SleekChildEnumerator GetEnumerator(this ISleekElement element)
    {
        ThreadUtil.assertIsGameThread();

        return new SleekChildEnumerator(element);
    }

    /// <summary>
    /// Sets the 'IsClickable' or 'IsInteractable' values for one of the following types: <see cref="ISleekButton"/>, <see cref="ISleekSlider"/>,
    /// <see cref="ISleekToggle"/>, <see cref="ISleekField"/>, <see cref="ISleekNumericField"/>,
    /// <see cref="SleekButtonIcon"/>, <see cref="SleekButtonIconConfirm"/>, <see cref="SleekButtonState"/>.
    /// </summary>
    /// <param name="isClickable">Can the user interact with the element (change it's value or click it)?</param>
    /// <returns><see langword="true"/> unless the input is not a valid type.</returns>
    public static bool SetIsClickable(this ISleekElement element, bool isClickable)
    {
        switch (element)
        {
            case ISleekButton btn1:
                btn1.IsClickable = isClickable;
                return true;

            case ISleekSlider slider:
                slider.IsInteractable = isClickable;
                return true;

            case ISleekToggle toggle:
                toggle.IsInteractable = isClickable;
                return true;

            case ISleekField field:
                field.IsClickable = isClickable;
                return true;

            case ISleekNumericField field:
                field.IsClickable = isClickable;
                return true;

            case SleekButtonIcon btn2:
                btn2.isClickable = isClickable;
                return true;

            case SleekButtonIconConfirm btn3:
                btn3.isClickable = isClickable;
                return true;

            case SleekButtonState btn4:
                btn4.isInteractable = isClickable;
                return true;

            default:
                return false;
        }
    }
}