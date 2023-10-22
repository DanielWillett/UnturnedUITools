
/* Unmerged change from project 'UnturnedUITools (net461)'
Before:
using System;
After:
using System;
using DanielWillett;
using DanielWillett.UITools;
using DanielWillett.UITools.API;
using DanielWillett.UITools.API;
using DanielWillett.UITools.API.Extensions;
*/
using System;

namespace DanielWillett.UITools.API;

/// <summary>
/// UI Handler to trigger when the UI is closed.
/// </summary>
public interface ICustomOnCloseUIHandler : ICustomUIHandler
{
    /// <summary>
    /// Invoked when the UI is closed.
    /// </summary>
    event Action<Type?, object?> OnClosed;

    /// <summary>
    /// Checks if <see cref="OnClosed"/> has been subscribed to.
    /// </summary>
    bool HasOnCloseBeenInitialized { get; internal set; }
}
