using DanielWillett.UITools.API.Extensions;
using SDG.Unturned;
using System;

namespace DanielWillett.UITools.API;

/// <summary>
/// Implements a fullscreen container base for a UI.
/// </summary>
public abstract class ContainerUIExtension : UIExtension, IDisposable
{
    private bool _containerHasBeenParented;

    /// <summary>
    /// The parent of the container. Usually <see cref="EditorUI.window"/>, <see cref="PlayerUI.window"/>, <see cref="MenuUI.window"/>, or <see cref="LoadingUI.window"/>.
    /// </summary>
    protected abstract SleekWindow Parent { get; }
    protected virtual float SizeScaleX => 1f;
    protected virtual float SizeScaleY => 1f;
    protected virtual float PositionScaleX => 0f;
    protected virtual float PositionScaleY => 0f;
    protected virtual int SizeOffsetX => 0;
    protected virtual int SizeOffsetY => 0;
    protected virtual int PositionOffsetX => 0;
    protected virtual int PositionOffsetY => 0;

    /// <summary>
    /// Base container to add all elements to. Value may change.
    /// </summary>
    public SleekFullscreenBox Container { get; set; }
    protected ContainerUIExtension()
    {
        Container = new SleekFullscreenBox
        {
            SizeScale_X = 1f,
            SizeScale_Y = 1f,
            IsVisible = false
        };
        _containerHasBeenParented = false;
    }

    /// <summary>
    /// Add all your components in this method.
    /// </summary>
    protected abstract void OnShown();

    /// <summary>
    /// Remove all your components in this method by calling <see cref="SleekWrapper.RemoveChild"/> on <see cref="Container"/>.
    /// </summary>
    protected abstract void OnHidden();

    /// <summary>
    /// Unsubscribe from events, etc here. This is basically the <see cref="IDisposable.Dispose"/> method of your extension.
    /// </summary>
    protected abstract void OnDestroyed();
    protected sealed override void Opened()
    {
        if (Parent == null)
        {
            CommandWindow.LogError($"Parent null trying to add container: {this.GetType().Name}.");
            return;
        }
        if (!_containerHasBeenParented || Parent.FindIndexOfChild(Container) == -1)
        {
            if (_containerHasBeenParented)
            {
                try
                {
                    Container.InternalDestroy();
                }
                catch (Exception ex)
                {
                    if (UnturnedUIToolsNexus.UIExtensionManager.DebugLogging)
                    {
                        CommandWindow.Log("Error destroying container.");
                        CommandWindow.Log(ex);
                    }
                }

                Container = new SleekFullscreenBox
                {
                    SizeScale_X = 1f,
                    SizeScale_Y = 1f
                };
            }

            Parent.AddChild(Container);
            _containerHasBeenParented = true;
        }

        Container.SizeScale_X = SizeScaleX;
        Container.SizeScale_Y = SizeScaleY;
        Container.SizeOffset_X = SizeOffsetX;
        Container.SizeOffset_Y = SizeOffsetY;
        Container.PositionScale_X = PositionScaleX;
        Container.PositionScale_Y = PositionScaleY;
        Container.PositionOffset_X = PositionOffsetX;
        Container.PositionOffset_Y = PositionOffsetY;
        Container.IsVisible = true;
        OnShown();
    }
    protected sealed override void Closed()
    {
        OnHidden();
        Container.IsVisible = false;
    }
    public void Dispose()
    {
        OnDestroyed();
        if (Parent != null && Parent.FindIndexOfChild(Container) >= 0)
        {
            Parent.RemoveChild(Container);
            _containerHasBeenParented = true;
        }
        Container = null!;
    }
}