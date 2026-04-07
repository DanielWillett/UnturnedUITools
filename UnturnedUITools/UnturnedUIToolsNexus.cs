using DanielWillett.ReflectionTools;
using DanielWillett.UITools.API.Extensions;
using DanielWillett.UITools.Core.Extensions;
using DanielWillett.UITools.Util;
using SDG.Framework.Modules;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using Module = SDG.Framework.Modules.Module;
using Object = UnityEngine.Object;

namespace DanielWillett.UITools;

/// <summary>
/// Module class for the <see cref="UITools"/> module/library.
/// </summary>
/// <remarks>Call <see cref="Initialize()"/> to set up if not loaded as a module.</remarks>
public class UnturnedUIToolsNexus : IModuleNexus
{
    private static readonly object Sync = new object();

    private static IUIExtensionManager? _uiExtensionManager;
    private static bool _init;
    private static bool _isStandaloneCached;
    private static bool _isStandalone;
    private bool _subModInit;
    private static GameObject? _extensionManagerObject;

    /// <summary>
    /// Is <see cref="UITools"/> installed as a module instead of a library?
    /// </summary>
    public static bool IsStandalone
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        get
        {
            if (_isStandaloneCached)
                return _isStandalone;

            if (ModuleHook.modules == null)
                return false;

            Assembly thisAsm = Assembly.GetExecutingAssembly();

            _isStandalone = ModuleHook.modules.Any(x => x.assemblies != null && Array.IndexOf(x.assemblies, thisAsm) != -1);
            _isStandaloneCached = true;
            return _isStandalone;
        }
    }

#nullable disable

    /// <summary>
    /// Manages extensions, the default type is <see cref="Core.Extensions.UIExtensionManager"/>.
    /// </summary>
    /// <remarks>Call <see cref="Initialize()"/> to setup with default values.</remarks>
    /// <returns>The current extension manager, or <see langword="null"/> duing initialization if it hasn't been set up yet.</returns>
    /// <exception cref="InvalidOperationException">The <see cref="IUIExtensionManager"/> was never set up.</exception>
    public static IUIExtensionManager UIExtensionManager
    {
        get
        {
            if (_init && _uiExtensionManager == null)
                throw new InvalidOperationException("UI Extension Manager was never set up.");

            lock (Sync)
                return _uiExtensionManager;
        }
        set
        {
            ThreadUtil.assertIsGameThread();

            lock (Sync)
            {
                if (value is null)
                    throw new ArgumentNullException(nameof(value));

                IUIExtensionManager old = Interlocked.Exchange(ref _uiExtensionManager, value);
                if (ReferenceEquals(old, value))
                    return;

                if (old is IDisposable disposable)
                    disposable.Dispose();

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (old is Object obj && obj != null)
                {
                    if (obj is MonoBehaviour b && b.gameObject == _extensionManagerObject)
                    {
                        Object.Destroy(_extensionManagerObject);
                    }
                    else
                    {
                        Object.Destroy(obj);
                    }
                }

                if (_init)
                {
                    value.Initialize();

                    Register();
                }
            }
        }
    }

#nullable restore


    void IModuleNexus.initialize()
    {
        _subModInit = true;
        _isStandalone = true;
        _isStandaloneCached = true;
        ModuleHook.onModulesInitialized += OnModulesInitialized;
    }
    void IModuleNexus.shutdown()
    {
        if (_subModInit)
        {
            _subModInit = false;
            ModuleHook.onModulesInitialized -= OnModulesInitialized;
        }

        IUIExtensionManager? oldManager = Interlocked.Exchange(ref _uiExtensionManager, null!);
        if (oldManager is IDisposable disposable)
            disposable.Dispose();

        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (oldManager is Object obj && obj != null)
        {
            if (obj is MonoBehaviour b && b.gameObject == _extensionManagerObject)
            {
                Object.DestroyImmediate(_extensionManagerObject);
            }
            else
            {
                Object.DestroyImmediate(obj);
            }
        }
        
        if (Accessor.Logger is UnturnedReflectionToolsLogger)
        {
            Accessor.Logger = null;
        }
    }
    private void OnModulesInitialized()
    {
        if (_subModInit)
        {
            _subModInit = false;
            ModuleHook.onModulesInitialized -= OnModulesInitialized;
        }

        Initialize();
    }

    /// <inheritdoc cref="InitializeIfNotStandalone(bool)"/>
    public static bool InitializeIfNotStandalone()
    {
        return InitializeIfNotStandalone(_uiExtensionManager?.DebugLogging ?? false);
    }

    /// <summary>
    /// Call this manually after all modules have loaded if using as a library (or you're unsure if it's used as a library). Safe to call multiple times.
    /// <para>
    /// Checks to see if <see cref="UITools"/> is loaded as a module, otherwise runs <see cref="Initialize()"/>.
    /// </para>
    /// </summary>
    /// <param name="debugLogging">Whether or not to enable debug logging for the current <see cref="IUIExtensionManager"/> before intializing.</param>
    /// <remarks>If desired, <see cref="UIExtensionManager"/> and <see cref="UIAccessor.HarmonyId"/> should be set beforehand.</remarks>
    /// <returns><see langword="True"/> if <see cref="UITools"/> was not found as a module and <see cref="Initialize()"/> was ran, otherwise <see langword="false"/>.</returns>
    public static bool InitializeIfNotStandalone(bool debugLogging)
    {
        if (ModuleHook.modules == null)
            throw new InvalidOperationException("Modules have not been loaded yet.");
        
        if (!IsStandalone)
        {
            Initialize(debugLogging);
            return true;
        }

        return false;
    }

    /// <inheritdoc cref="Initialize(bool)"/>
    public static void Initialize()
    {
        Initialize(_uiExtensionManager?.DebugLogging ?? false);
    }

    /// <summary>
    /// Call this manually after all modules have loaded if using as a library. Safe to call multiple times.
    /// </summary>
    /// <param name="debugLogging">Whether or not to enable debug logging for the current <see cref="IUIExtensionManager"/> before intializing.</param>
    /// <remarks>If desired, <see cref="UIExtensionManager"/> and <see cref="UIAccessor.HarmonyId"/> should be set beforehand.</remarks>
    public static void Initialize(bool debugLogging)
    {
        ThreadUtil.assertIsGameThread();

        if (_init)
            return;

        IReflectionToolsLogger? logger = Accessor.Logger;
        if (logger is ConsoleReflectionToolsLogger)
        {
            Accessor.Logger = new UnturnedReflectionToolsLogger();
        }

        lock (Sync)
        {
            if (_init)
                return;

            UIAccessor.Init();

            _init = true;
            if (_uiExtensionManager == null)
            {
                if (_extensionManagerObject != null)
                {
                    Object.Destroy(_extensionManagerObject);
                }

                _extensionManagerObject = new GameObject(nameof(Core.Extensions.UIExtensionManager), typeof(UIExtensionManager));
                Object.DontDestroyOnLoad(_extensionManagerObject);
                _uiExtensionManager = _extensionManagerObject.GetComponent<UIExtensionManager>();
            }

            _uiExtensionManager.DebugLogging = debugLogging;
            _uiExtensionManager.Initialize();

            Register();
        }
    }

    private static void Register()
    {
        if (ModuleHook.modules != null)
        {
            HashSet<string> alreadyScanned = new HashSet<string>();
            foreach (Module module in ModuleHook.modules)
            {
                if (module.assemblies == null)
                    continue;
                foreach (Assembly assembly in module.assemblies)
                {
                    if (assembly.ReflectionOnly || !alreadyScanned.Add(assembly.GetName().FullName))
                        continue;

                    try
                    {
                        UIExtensionManager.RegisterFromModuleAssembly(assembly, module);
                    }
                    catch (Exception ex)
                    {
                        CommandWindow.LogError($"Failed to register UI extensions from {module.config.Name}, assembly: {assembly.FullName}.");
                        CommandWindow.LogError(ex);
                    }
                }
            }
        }

        CommandWindow.Log($"Registered {UIExtensionManager.Extensions.Count} UI extension type(s).");
    }
}