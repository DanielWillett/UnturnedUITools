using DanielWillett.UITools.API.Extensions;
using DanielWillett.UITools.Core.Extensions;
using DanielWillett.UITools.Util;
using SDG.Framework.Modules;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Reflection;
using Module = SDG.Framework.Modules.Module;

namespace DanielWillett.UITools;

/// <summary>
/// Module class for the <see cref="UITools"/> module/library.
/// </summary>
/// <remarks>Call <see cref="Initialize"/> to set up if not loaded as a module.</remarks>
public class UnturnedUIToolsNexus : IModuleNexus
{
    private static IUIExtensionManager? _uiExtensionManager;
    private static bool _init;
    private bool _subModInit;

#nullable disable

    /// <summary>
    /// Must be set in <see cref="IModuleNexus.initialize"/>.
    /// </summary>
    /// <remarks>Call <see cref="Initialize"/> to setup with default values.</remarks>
    public static IUIExtensionManager UIExtensionManager
    {
        get
        {
            if (_init && _uiExtensionManager == null)
                throw new InvalidOperationException("UI Extension Manager was never set up.");

            return _uiExtensionManager;
        }
        set
        {
            if (_init)
                throw new InvalidOperationException("UI Extension Manager has already been set up.");

            _uiExtensionManager = value;
        }
    }

#nullable restore


    void IModuleNexus.initialize()
    {
        _subModInit = true;
        ModuleHook.onModulesInitialized += OnModulesInitialized;
    }
    void IModuleNexus.shutdown()
    {
        if (_subModInit)
        {
            _subModInit = false;
            ModuleHook.onModulesInitialized -= OnModulesInitialized;
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

    /// <summary>
    /// Call this manually after all modules have loaded if using as a library. Safe to call if it's already been called.
    /// </summary>
    /// <remarks>If desired, <see cref="UIExtensionManager"/> and <see cref="UIAccessor.HarmonyId"/> should be set beforehand.</remarks>
    public static void Initialize()
    {
        ThreadUtil.assertIsGameThread();

        if (_init)
            return;

        UIAccessor.Init();

        _init = true;
        _uiExtensionManager ??= new UIExtensionManager();

        HashSet<string> alreadyScanned = new HashSet<string>();
        foreach (Module module in ModuleHook.modules)
        {
            foreach (Assembly assembly in module.assemblies)
            {
                if (!alreadyScanned.Add(assembly.GetName().FullName))
                    continue;

                try
                {
                    _uiExtensionManager.RegisterFromModuleAssembly(assembly, module);
                }
                catch (Exception ex)
                {
                    CommandWindow.LogError($"Failed to register UI extensions from {module.config.Name}, assembly: {assembly.FullName}.");
                    CommandWindow.LogError(ex);
                }
            }
        }

        CommandWindow.Log($"Registered {UIExtensionManager.Extensions.Count} UI extension type(s).");
    }
}