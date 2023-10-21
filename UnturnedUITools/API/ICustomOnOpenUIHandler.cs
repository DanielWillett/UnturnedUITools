
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
/// UI Handler to trigger when the UI is opened.
/// </summary>
public interface ICustomOnOpenUIHandler : ICustomUIHandler
{
    /// <summary>
    /// Invoked when the UI is opened.
    /// </summary>
    event Action<Type?, object?> OnOpened;

    /// <summary>
    /// Checks if the <see cref="ICustomOnOpenUIHandler"/> handler has been patched.
    /// </summary>
    bool HasOnOpenBeenInitialized { get; internal set; }
}
