
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
/// UI Handler to trigger when the UI is initialized.
/// </summary>
public interface ICustomOnInitializeUIHandler : ICustomUIHandler
{
    /// <summary>
    /// Invoked when the UI is initialized.
    /// </summary>
    event Action<Type?, object?> OnInitialize;

    /// <summary>
    /// Checks if the <see cref="ICustomOnInitializeUIHandler"/> handler has been patched.
    /// </summary>
    bool HasOnInitializeBeenInitialized { get; internal set; }
}
