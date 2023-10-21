using DanielWillett.UITools.API;
using DanielWillett.UITools.Util;
using HarmonyLib;
using SDG.Unturned;
using System;
using System.Reflection;

namespace DanielWillett.UITools.Core.Handlers;
internal class CustomSleekWrapperDestroyHandler : ICustomOnDestroyUIHandler
{
    private const string Source = UIAccessor.Source + ".SLEEK WRAPPER HANDLER";

    private static CustomSleekWrapperDestroyHandler? _instance;

    public event Action<Type?, object?>? OnDestroyed;
    public bool HasBeenInitialized { get; set; }
    public bool HasOnDestroyBeenInitialized { get; set; }
    public CustomSleekWrapperDestroyHandler()
    {
        _instance = this;
    }
    public void Patch(Harmony patcher)
    {
        MethodInfo? method = typeof(SleekWrapper).GetMethod(nameof(SleekWrapper.InternalDestroy), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (method == null)
        {
            CommandWindow.LogWarning($"[{Source}] Unable to find method: SleekWrapper.InternalDestroy.");
            return;
        }

        try
        {

            patcher.Patch(method, prefix: new HarmonyMethod(new Action<object>(OnDestroyInvoker).Method));
        }
        catch (Exception ex)
        {
            CommandWindow.LogWarning($"[{Source}] Unable to patch: SleekWrapper.InternalDestroy.");
            CommandWindow.LogWarning(ex);
        }
    }
    public void Unpatch(Harmony patcher)
    {
        MethodInfo? method = typeof(SleekWrapper).GetMethod(nameof(SleekWrapper.InternalDestroy), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (method != null)
            patcher.Unpatch(method, new Action<object>(OnDestroyInvoker).Method);
    }

    private static void OnDestroyInvoker(object __instance)
    {
        Type type = __instance.GetType();
        if (UIAccessor.TryGetUITypeInfo(type, out UITypeInfo typeInfo) && typeInfo.CustomOnDestroy is not CustomSleekWrapperDestroyHandler)
            return;
        _instance?.OnDestroyed?.Invoke(null, __instance);
    }
}