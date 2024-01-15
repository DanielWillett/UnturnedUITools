using SDG.Unturned;
using System;

namespace DanielWillett.UITools.Util;

/// <summary>
/// Extensions for <see cref="IGlazier"/> implementations.
/// </summary>
public static class GlazierExtensions
{
    private static SleekElementBuilder<ISleekBox> _box;
    private static SleekElementBuilder<ISleekButton> _button;
    private static SleekElementBuilder<ISleekElement> _frame;
    private static SleekElementBuilder<ISleekConstraintFrame> _constraintFrame;
    private static SleekElementBuilder<ISleekImage> _image;
    private static SleekElementBuilder<ISleekSprite> _sprite;
    private static SleekElementBuilder<ISleekLabel> _label;
    private static SleekElementBuilder<ISleekScrollView> _scrollView;
    private static SleekElementBuilder<ISleekSlider> _slider;
    private static SleekElementBuilder<ISleekField> _stringField;
    private static SleekElementBuilder<ISleekToggle> _toggle;
    private static SleekElementBuilder<ISleekUInt8Field> _uint8Field;
    private static SleekElementBuilder<ISleekUInt16Field> _uint16Field;
    private static SleekElementBuilder<ISleekUInt32Field> _uint32Field;
    private static SleekElementBuilder<ISleekInt32Field> _int32Field;
    private static SleekElementBuilder<ISleekFloat32Field> _float32Field;
    private static SleekElementBuilder<ISleekFloat64Field> _float64Field;

    /// <summary>
    /// Create an <see cref="ISleekBox"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static ref SleekElementBuilder<ISleekBox> ConfigureBox(this IGlazier glazier)
    {
        ThreadUtil.assertIsGameThread();

        _box = glazier.CreateBox().Configure();
        return ref _box;
    }

    /// <summary>
    /// Create an <see cref="ISleekButton"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static ref SleekElementBuilder<ISleekButton> ConfigureButton(this IGlazier glazier)
    {
        ThreadUtil.assertIsGameThread();

        _button = glazier.CreateButton().Configure();
        return ref _button;
    }

    /// <summary>
    /// Create an <see cref="ISleekElement"/> frame and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static ref SleekElementBuilder<ISleekElement> ConfigureFrame(this IGlazier glazier)
    {
        ThreadUtil.assertIsGameThread();

        _frame = glazier.CreateFrame().Configure();
        return ref _frame;
    }

    /// <summary>
    /// Create an <see cref="ISleekConstraintFrame"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static ref SleekElementBuilder<ISleekConstraintFrame> ConfigureConstraintFrame(this IGlazier glazier)
    {
        ThreadUtil.assertIsGameThread();

        _constraintFrame = glazier.CreateConstraintFrame().Configure();
        return ref _constraintFrame;
    }

    /// <summary>
    /// Create an <see cref="ISleekImage"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static ref SleekElementBuilder<ISleekImage> ConfigureImage(this IGlazier glazier)
    {
        ThreadUtil.assertIsGameThread();

        _image = glazier.CreateImage().Configure();
        return ref _image;
    }

    /// <summary>
    /// Create an <see cref="ISleekSprite"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static ref SleekElementBuilder<ISleekSprite> ConfigureSprite(this IGlazier glazier)
    {
        ThreadUtil.assertIsGameThread();

        _sprite = glazier.CreateSprite().Configure();
        return ref _sprite;
    }

    /// <summary>
    /// Create an <see cref="ISleekLabel"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static ref SleekElementBuilder<ISleekLabel> ConfigureLabel(this IGlazier glazier)
    {
        ThreadUtil.assertIsGameThread();

        _label = glazier.CreateLabel().Configure();
        return ref _label;
    }

    /// <summary>
    /// Create an <see cref="ISleekScrollView"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static ref SleekElementBuilder<ISleekScrollView> ConfigureScrollView(this IGlazier glazier)
    {
        ThreadUtil.assertIsGameThread();

        _scrollView = glazier.CreateScrollView().Configure();
        return ref _scrollView;
    }

    /// <summary>
    /// Create an <see cref="ISleekSlider"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static ref SleekElementBuilder<ISleekSlider> ConfigureSlider(this IGlazier glazier)
    {
        ThreadUtil.assertIsGameThread();

        _slider = glazier.CreateSlider().Configure();
        return ref _slider;
    }

    /// <summary>
    /// Create an <see cref="ISleekField"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static ref SleekElementBuilder<ISleekField> ConfigureStringField(this IGlazier glazier)
    {
        ThreadUtil.assertIsGameThread();

        _stringField = glazier.CreateStringField().Configure();
        return ref _stringField;
    }

    /// <summary>
    /// Create an <see cref="ISleekToggle"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static ref SleekElementBuilder<ISleekToggle> ConfigureToggle(this IGlazier glazier)
    {
        ThreadUtil.assertIsGameThread();

        _toggle = glazier.CreateToggle().Configure();
        return ref _toggle;
    }

    /// <summary>
    /// Create an <see cref="ISleekUInt8Field"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static ref SleekElementBuilder<ISleekUInt8Field> ConfigureUInt8Field(this IGlazier glazier)
    {
        ThreadUtil.assertIsGameThread();

        _uint8Field = glazier.CreateUInt8Field().Configure();
        return ref _uint8Field;
    }

    /// <summary>
    /// Create an <see cref="ISleekUInt16Field"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static ref SleekElementBuilder<ISleekUInt16Field> ConfigureUInt16Field(this IGlazier glazier)
    {
        ThreadUtil.assertIsGameThread();

        _uint16Field = glazier.CreateUInt16Field().Configure();
        return ref _uint16Field;
    }

    /// <summary>
    /// Create an <see cref="ISleekUInt32Field"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static ref SleekElementBuilder<ISleekUInt32Field> ConfigureUInt32Field(this IGlazier glazier)
    {
        ThreadUtil.assertIsGameThread();

        _uint32Field = glazier.CreateUInt32Field().Configure();
        return ref _uint32Field;
    }

    /// <summary>
    /// Create an <see cref="ISleekInt32Field"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static ref SleekElementBuilder<ISleekInt32Field> ConfigureInt32Field(this IGlazier glazier)
    {
        ThreadUtil.assertIsGameThread();

        _int32Field = glazier.CreateInt32Field().Configure();
        return ref _int32Field;
    }

    /// <summary>
    /// Create an <see cref="ISleekFloat32Field"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static ref SleekElementBuilder<ISleekFloat32Field> ConfigureFloat32Field(this IGlazier glazier)
    {
        ThreadUtil.assertIsGameThread();

        _float32Field = glazier.CreateFloat32Field().Configure();
        return ref _float32Field;
    }

    /// <summary>
    /// Create an <see cref="ISleekFloat64Field"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static ref SleekElementBuilder<ISleekFloat64Field> ConfigureFloat64Field(this IGlazier glazier)
    {
        ThreadUtil.assertIsGameThread();

        _float64Field = glazier.CreateFloat64Field().Configure();
        return ref _float64Field;
    }

    /// <summary>
    /// Create an <see cref="ISleekBox"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static void ConfigureBox(this IGlazier glazier, out SleekElementBuilder<ISleekBox> builder)
    {
        ThreadUtil.assertIsGameThread();

        builder = glazier.CreateBox().Configure();
    }

    /// <summary>
    /// Create an <see cref="ISleekButton"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static void ConfigureButton(this IGlazier glazier, out SleekElementBuilder<ISleekButton> builder)
    {
        ThreadUtil.assertIsGameThread();

        builder = glazier.CreateButton().Configure();
    }

    /// <summary>
    /// Create an <see cref="ISleekElement"/> frame and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static void ConfigureFrame(this IGlazier glazier, out SleekElementBuilder<ISleekElement> builder)
    {
        ThreadUtil.assertIsGameThread();

        builder = glazier.CreateFrame().Configure();
    }

    /// <summary>
    /// Create an <see cref="ISleekConstraintFrame"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static void ConfigureConstraintFrame(this IGlazier glazier, out SleekElementBuilder<ISleekConstraintFrame> builder)
    {
        ThreadUtil.assertIsGameThread();

        builder = glazier.CreateConstraintFrame().Configure();
    }

    /// <summary>
    /// Create an <see cref="ISleekImage"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static void ConfigureImage(this IGlazier glazier, out SleekElementBuilder<ISleekImage> builder)
    {
        ThreadUtil.assertIsGameThread();

        builder = glazier.CreateImage().Configure();
    }

    /// <summary>
    /// Create an <see cref="ISleekSprite"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static void ConfigureSprite(this IGlazier glazier, out SleekElementBuilder<ISleekSprite> builder)
    {
        ThreadUtil.assertIsGameThread();

        builder = glazier.CreateSprite().Configure();
    }

    /// <summary>
    /// Create an <see cref="ISleekLabel"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static void ConfigureLabel(this IGlazier glazier, out SleekElementBuilder<ISleekLabel> builder)
    {
        ThreadUtil.assertIsGameThread();

        builder = glazier.CreateLabel().Configure();
    }

    /// <summary>
    /// Create an <see cref="ISleekScrollView"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static void ConfigureScrollView(this IGlazier glazier, out SleekElementBuilder<ISleekScrollView> builder)
    {
        ThreadUtil.assertIsGameThread();

        builder = glazier.CreateScrollView().Configure();
    }

    /// <summary>
    /// Create an <see cref="ISleekSlider"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static void ConfigureSlider(this IGlazier glazier, out SleekElementBuilder<ISleekSlider> builder)
    {
        ThreadUtil.assertIsGameThread();

        builder = glazier.CreateSlider().Configure();
    }

    /// <summary>
    /// Create an <see cref="ISleekField"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static void ConfigureStringField(this IGlazier glazier, out SleekElementBuilder<ISleekField> builder)
    {
        ThreadUtil.assertIsGameThread();

        builder = glazier.CreateStringField().Configure();
    }

    /// <summary>
    /// Create an <see cref="ISleekToggle"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static void ConfigureToggle(this IGlazier glazier, out SleekElementBuilder<ISleekToggle> builder)
    {
        ThreadUtil.assertIsGameThread();

        builder = glazier.CreateToggle().Configure();
    }

    /// <summary>
    /// Create an <see cref="ISleekUInt8Field"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static void ConfigureUInt8Field(this IGlazier glazier, out SleekElementBuilder<ISleekUInt8Field> builder)
    {
        ThreadUtil.assertIsGameThread();

        builder = glazier.CreateUInt8Field().Configure();
    }

    /// <summary>
    /// Create an <see cref="ISleekUInt16Field"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static void ConfigureUInt16Field(this IGlazier glazier, out SleekElementBuilder<ISleekUInt16Field> builder)
    {
        ThreadUtil.assertIsGameThread();

        builder = glazier.CreateUInt16Field().Configure();
    }

    /// <summary>
    /// Create an <see cref="ISleekUInt32Field"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static void ConfigureUInt32Field(this IGlazier glazier, out SleekElementBuilder<ISleekUInt32Field> builder)
    {
        ThreadUtil.assertIsGameThread();

        builder = glazier.CreateUInt32Field().Configure();
    }

    /// <summary>
    /// Create an <see cref="ISleekInt32Field"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static void ConfigureInt32Field(this IGlazier glazier, out SleekElementBuilder<ISleekInt32Field> builder)
    {
        ThreadUtil.assertIsGameThread();

        builder = glazier.CreateInt32Field().Configure();
    }

    /// <summary>
    /// Create an <see cref="ISleekFloat32Field"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static void ConfigureFloat32Field(this IGlazier glazier, out SleekElementBuilder<ISleekFloat32Field> builder)
    {
        ThreadUtil.assertIsGameThread();

        builder = glazier.CreateFloat32Field().Configure();
    }

    /// <summary>
    /// Create an <see cref="ISleekFloat64Field"/> and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static void ConfigureFloat64Field(this IGlazier glazier, out SleekElementBuilder<ISleekFloat64Field> builder)
    {
        ThreadUtil.assertIsGameThread();

        builder = glazier.CreateFloat64Field().Configure();
    }

    /// <summary>
    /// Creates a new sleek type with the given <see cref="IGlazier"/> instance and a builder for it.
    /// </summary>
    /// <exception cref="NotSupportedException">Not on main thread.</exception>
    public static void ConfigureWrapper<TSleekWrapper>(this IGlazier glazier, out SleekElementBuilder<TSleekWrapper> builder) where TSleekWrapper : class, ISleekElement, new()
    {
        builder = glazier.CreateWrapper<TSleekWrapper>().Configure();
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
    public static void ConfigureWrapper<TSleekWrapper>(this IGlazier glazier, Func<TSleekWrapper> factory, out SleekElementBuilder<TSleekWrapper> builder) where TSleekWrapper : class, ISleekElement
    {
        builder = glazier.CreateWrapper(factory).Configure();
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
