namespace DanielWillett.UITools.API.Extensions;

/// <summary>
/// Contains a callback for as soon as the extension is ready to use.
/// </summary>
/// <remarks>This usually is only necessary if the extension is a unity object and will be called after Awake() but before Start(). For non Unity Objects, it's called right after the constructor.</remarks>
public interface IUIExtensionReadyListener
{
    /// <summary>
    /// Called right after the extension is ready to use.
    /// </summary>
    /// <remarks>This usually is only necessary if the extension is a unity object and will be called after Awake() but before Start(). For non Unity Objects, it's called right after the constructor.</remarks>
    /// <param name="uiinstance">The instance of the vanilla UI linked to this object, if it isn't static.</param>
    void OnReady(object? uiinstance);
}
