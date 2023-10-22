using DanielWillett.UITools;
using DanielWillett.UITools.Core.Extensions;
using DanielWillett.UITools.Util;
using HarmonyLib;
using SDG.Framework.Modules;
using SDG.Unturned;

namespace ExampleModule;

public class Nexus : IModuleNexus
{
    public static Harmony Patcher { get; } = new Harmony("DanielWillett.ExampleModule");
    void IModuleNexus.initialize()
    {
        CommandWindow.Log("ExampleModule started.");

        // Optionally override the UI Extension manager in your initialize method to enable debug logging.

        UnturnedUIToolsNexus.UIExtensionManager = new UIExtensionManager
        {
            DebugLogging = true
        };


        // Enables a auto-clearing harmony debug log in Unturned/Logs. Path can be changed with UIAccessor.HarmonyLogPath.

        UIAccessor.ManageHarmonyDebugLog = true;


        // Unless you add it as a module instead of a library, you must explicitly initialize it.
        // This can be safely ran in more than one module as long as it's ran after all modules have loaded.
        ModuleHook.onModulesInitialized += OnModulesLoaded;
    }
    private void OnModulesLoaded()
    {
        UnturnedUIToolsNexus.Initialize();
    }

    void IModuleNexus.shutdown()
    {
        CommandWindow.Log("ExampleModule shutdown.");
        ModuleHook.onModulesInitialized -= OnModulesLoaded;
    }
}