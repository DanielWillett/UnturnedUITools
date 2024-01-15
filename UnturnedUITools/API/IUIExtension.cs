using DanielWillett.UITools.API.Extensions;

namespace DanielWillett.UITools.API;

/// <summary>
/// Optional base interface for UI extensions (defined with <see cref="UIExtensionAttribute"/>). Provides OnOpened and OnClosed methods.
/// </summary>
/// <remarks>Consider using <see cref="UIExtension"/> instead if possible.</remarks>
public interface IUIExtension
{
    /// <summary>
    /// Called when the vanilla UI is opened, before <see cref="OnOpened"/>.
    /// </summary>
    void OnOpened();

    /// <summary>
    /// Called when the vanilla UI is closed, after <see cref="OnClosed"/>.
    /// </summary>
    void OnClosed();
}
