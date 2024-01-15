using SDG.Unturned;
using System;

namespace DanielWillett.UITools.Util;

/// <summary>
/// Extensions for <see cref="IGlazier"/> implementations.
/// </summary>
public static class GlazierExtensions
{
    /// <summary>
    /// Create an <see cref="ISleekBox"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static SleekElementBuilder<ISleekBox> ConfigureBox(this IGlazier glazier)
    {
        ThreadUtil.assertIsGameThread();

        return glazier.CreateBox().Configure();
    }

    /// <summary>
    /// Create an <see cref="ISleekButton"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static SleekElementBuilder<ISleekButton> ConfigureButton(this IGlazier glazier)
    {
        ThreadUtil.assertIsGameThread();

        return glazier.CreateButton().Configure();
    }

    /// <summary>
    /// Create an <see cref="ISleekElement"/> frame and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static SleekElementBuilder<ISleekElement> ConfigureFrame(this IGlazier glazier)
    {
        ThreadUtil.assertIsGameThread();

        return glazier.CreateFrame().Configure();
    }

    /// <summary>
    /// Create an <see cref="ISleekConstraintFrame"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static SleekElementBuilder<ISleekConstraintFrame> ConfigureConstraintFrame(this IGlazier glazier)
    {
        ThreadUtil.assertIsGameThread();

        return glazier.CreateConstraintFrame().Configure();
    }

    /// <summary>
    /// Create an <see cref="ISleekImage"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static SleekElementBuilder<ISleekImage> ConfigureImage(this IGlazier glazier)
    {
        ThreadUtil.assertIsGameThread();

        return glazier.CreateImage().Configure();
    }

    /// <summary>
    /// Create an <see cref="ISleekSprite"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static SleekElementBuilder<ISleekSprite> ConfigureSprite(this IGlazier glazier)
    {
        ThreadUtil.assertIsGameThread();

        return glazier.CreateSprite().Configure();
    }

    /// <summary>
    /// Create an <see cref="ISleekLabel"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static SleekElementBuilder<ISleekLabel> ConfigureLabel(this IGlazier glazier)
    {
        ThreadUtil.assertIsGameThread();

        return glazier.CreateLabel().Configure();
    }

    /// <summary>
    /// Create an <see cref="ISleekScrollView"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static SleekElementBuilder<ISleekScrollView> ConfigureScrollView(this IGlazier glazier)
    {
        ThreadUtil.assertIsGameThread();

        return glazier.CreateScrollView().Configure();
    }

    /// <summary>
    /// Create an <see cref="ISleekSlider"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static SleekElementBuilder<ISleekSlider> ConfigureSlider(this IGlazier glazier)
    {
        ThreadUtil.assertIsGameThread();

        return glazier.CreateSlider().Configure();
    }

    /// <summary>
    /// Create an <see cref="ISleekField"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static SleekElementBuilder<ISleekField> ConfigureStringField(this IGlazier glazier)
    {
        ThreadUtil.assertIsGameThread();

        return glazier.CreateStringField().Configure();
    }

    /// <summary>
    /// Create an <see cref="ISleekToggle"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static SleekElementBuilder<ISleekToggle> ConfigureToggle(this IGlazier glazier)
    {
        ThreadUtil.assertIsGameThread();

        return glazier.CreateToggle().Configure();
    }

    /// <summary>
    /// Create an <see cref="ISleekUInt8Field"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static SleekElementBuilder<ISleekUInt8Field> ConfigureUInt8Field(this IGlazier glazier)
    {
        ThreadUtil.assertIsGameThread();

        return glazier.CreateUInt8Field().Configure();
    }

    /// <summary>
    /// Create an <see cref="ISleekUInt16Field"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static SleekElementBuilder<ISleekUInt16Field> ConfigureUInt16Field(this IGlazier glazier)
    {
        ThreadUtil.assertIsGameThread();

        return glazier.CreateUInt16Field().Configure();
    }

    /// <summary>
    /// Create an <see cref="ISleekUInt32Field"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static SleekElementBuilder<ISleekUInt32Field> ConfigureUInt32Field(this IGlazier glazier)
    {
        ThreadUtil.assertIsGameThread();

        return glazier.CreateUInt32Field().Configure();
    }

    /// <summary>
    /// Create an <see cref="ISleekInt32Field"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static SleekElementBuilder<ISleekInt32Field> ConfigureInt32Field(this IGlazier glazier)
    {
        ThreadUtil.assertIsGameThread();

        return glazier.CreateInt32Field().Configure();
    }

    /// <summary>
    /// Create an <see cref="ISleekFloat32Field"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static SleekElementBuilder<ISleekFloat32Field> ConfigureFloat32Field(this IGlazier glazier)
    {
        ThreadUtil.assertIsGameThread();

        return glazier.CreateFloat32Field().Configure();
    }

    /// <summary>
    /// Create an <see cref="ISleekFloat64Field"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static SleekElementBuilder<ISleekFloat64Field> ConfigureFloat64Field(this IGlazier glazier)
    {
        ThreadUtil.assertIsGameThread();

        return glazier.CreateFloat64Field().Configure();
    }

    /// <summary>
    /// Creates a new sleek type with the given <see cref="IGlazier"/> instance and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static SleekElementBuilder<TSleekWrapper> ConfigureWrapper<TSleekWrapper>(this IGlazier glazier) where TSleekWrapper : class, ISleekElement, new()
    {
        return glazier.CreateWrapper<TSleekWrapper>().Configure();
    }

    /// <summary>
    /// Creates a new sleek type with the given <see cref="IGlazier"/> instance.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static TSleekWrapper CreateWrapper<TSleekWrapper>(this IGlazier glazier) where TSleekWrapper : ISleekElement, new()
    {
        ThreadUtil.assertIsGameThread();

        if (Glazier.instance == glazier)
            return new TSleekWrapper();

        IGlazier oldInstance = Glazier.instance;
        Glazier.instance = glazier;

        TSleekWrapper wrapper = new TSleekWrapper();

        Glazier.instance = oldInstance;

        return wrapper;
    }

    /// <summary>
    /// Creates a new sleek type with the given <see cref="IGlazier"/> instance and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static SleekElementBuilder<TSleekWrapper> ConfigureWrapper<TSleekWrapper>(this IGlazier glazier, Func<TSleekWrapper> factory) where TSleekWrapper : class, ISleekElement
    {
        return glazier.CreateWrapper(factory).Configure();
    }

    /// <summary>
    /// Creates a new sleek type with the given <see cref="IGlazier"/> instance.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static TSleekWrapper CreateWrapper<TSleekWrapper>(this IGlazier glazier, Func<TSleekWrapper> factory) where TSleekWrapper : ISleekElement
    {
        ThreadUtil.assertIsGameThread();

        if (Glazier.instance == glazier)
            return factory();

        IGlazier oldInstance = Glazier.instance;
        Glazier.instance = glazier;

        TSleekWrapper wrapper = factory();

        Glazier.instance = oldInstance;

        return wrapper;
    }
}
