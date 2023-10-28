using System;
using System.Collections.Generic;
using System.Reflection;
using Module = SDG.Framework.Modules.Module;

namespace DanielWillett.UITools.API.Extensions;

/// <summary>
/// Keeps track of UI extensions.
/// </summary>
public interface IUIExtensionManager
{
    /// <summary>
    /// List of all registered UI Extensions (info, not instances themselves).
    /// </summary>
    IReadOnlyList<UIExtensionInfo> Extensions { get; }

    /// <summary>
    /// Enables extra debug logging to help diagnose problems.
    /// </summary>
    bool DebugLogging { get; set; }

    /// <summary>
    /// Gets the last created instance of <typeparamref name="T"/> (which should be a UI extension), or <see langword="null"/> if one isn't registered.
    /// </summary>
    T? GetInstance<T>() where T : class;

    /// <summary>
    /// Gets the last created instance of <typeparamref name="T"/> (which should be a UI extension) linked to the vanilla UI instance, or <see langword="null"/> if one isn't registered.
    /// </summary>
    T? GetInstance<T>(object vanillaUIInstance) where T : class;

    /// <summary>
    /// Manually register a UI extension.
    /// </summary>
    void RegisterExtension(Type extensionType, Type parentType, Module module);

    /// <summary>
    /// Register all UI extensions in an assembly and module.
    /// </summary>
    void RegisterFromModuleAssembly(Assembly assembly, Module module);

    /// <summary>
    /// Run any start-up requirements. This should not include any extension searching, as those will be registered with <see cref="RegisterFromModuleAssembly"/> and <see cref="RegisterExtension"/>.
    /// </summary>
    void Initialize();
}
