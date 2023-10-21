
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
/// UI Handler to trigger when the UI is destroyed.
/// </summary>
public interface ICustomOnDestroyUIHandler : ICustomUIHandler
{
    /// <summary>
    /// Invoked when the UI is destroyed.
    /// </summary>
    event Action<Type?, object?> OnDestroyed;

    /// <summary>
    /// Checks if the <see cref="ICustomOnDestroyUIHandler"/> handler has been patched.
    /// </summary>
    bool HasOnDestroyBeenInitialized { get; internal set; }
}
