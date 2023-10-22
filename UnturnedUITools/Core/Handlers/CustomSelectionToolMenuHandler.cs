using DanielWillett.ReflectionTools;
using DanielWillett.UITools.API;
using DanielWillett.UITools.Util;
using HarmonyLib;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace DanielWillett.UITools.Core.Handlers;

internal class CustomNodeMenuHandler : CustomSelectionToolMenuHandler
{
    public CustomNodeMenuHandler() : base(UIAccessor.EditorEnvironmentNodesUIType, "EditorEnvironmentNodesUI") { }
}
internal class CustomVolumeMenuHandler : CustomSelectionToolMenuHandler
{
    public CustomVolumeMenuHandler() : base(UIAccessor.EditorVolumesUIType, "EditorVolumesUI") { }
}

internal abstract class CustomSelectionToolMenuHandler : ICustomOnCloseUIHandler, ICustomOnOpenUIHandler
{
    private const string Source = UIAccessor.Source + ".SELECTION TOOL HANDLER";
    
    public event Action<Type?, object?>? OnClosed;
    public event Action<Type?, object?>? OnOpened;
    public bool HasBeenInitialized { get; set; }
    public bool HasOnCloseBeenInitialized { get; set; }
    public bool HasOnOpenBeenInitialized { get; set; }
    public bool HasOnDestroyBeenInitialized { get; set; }
    public Type? Type { get; }
    public string TypeName { get; }
    protected CustomSelectionToolMenuHandler(Type? type, string backupName)
    {
        Type = type;
        TypeName = type?.Name ?? backupName;
    }
    
    public void Patch(Harmony patcher)
    {
        if (Type == null)
        {
            CommandWindow.LogWarning($"[{Source}] Unable to find type: {TypeName}.");
            return;
        }

        MethodInfo? onUpdateMethod = Type?.GetMethod("OnUpdate", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        if (onUpdateMethod == null)
        {
            CommandWindow.LogWarning($"[{Source}] Unable to find method: {TypeName}.OnUpdate.");
            return;
        }

        try
        {

            patcher.Patch(onUpdateMethod, transpiler: new HarmonyMethod(Accessor.GetMethod(TranspileUIOnUpdate)!));
        }
        catch (Exception ex)
        {
            CommandWindow.LogWarning($"[{Source}] Unable to patch: {TypeName}.OnUpdate.");
            CommandWindow.LogWarning(ex);
            return;
        }

        MethodInfo? closeMethod = Type!.GetMethod("Close", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        if (closeMethod == null)
        {
            CommandWindow.LogWarning($"[{Source}] Unable to find method: {TypeName}.Close.");
            return;
        }

        try
        {

            patcher.Patch(closeMethod, postfix: new HarmonyMethod(Accessor.GetMethod(OnClosedAndDestroyedInvoker)!));
        }
        catch (Exception ex)
        {
            CommandWindow.LogWarning($"[{Source}] Unable to patch: {TypeName}.Close.");
            CommandWindow.LogWarning(ex);
        }
    }

    public void Unpatch(Harmony patcher)
    {
        MethodInfo? onUpdateMethod = UIAccessor.EditorVolumesUIType?.GetMethod("OnUpdate", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        MethodInfo? closeMethod = UIAccessor.EditorVolumesUIType!.GetMethod("Close", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        if (onUpdateMethod != null)
            patcher.Unpatch(onUpdateMethod, Accessor.GetMethod(TranspileUIOnUpdate)!);

        if (closeMethod != null)
            patcher.Unpatch(closeMethod, Accessor.GetMethod(OnClosedAndDestroyedInvoker)!);
    }

    private static IEnumerable<CodeInstruction> TranspileUIOnUpdate(IEnumerable<CodeInstruction> instructions, MethodBase method)
    {
        MethodInfo addInvoker = Accessor.GetMethod(OnOpenedInvoker)!;
        MethodInfo removeInvoker = Accessor.GetMethod(OnClosedAndDestroyedInvoker)!;
        MethodInfo? addMethod = typeof(SleekWrapper).GetMethod(nameof(SleekWrapper.AddChild), BindingFlags.Instance | BindingFlags.Public);
        if (addMethod == null)
            CommandWindow.LogError($"[{Source}] Unable to find method: SleekWrapper.AddChild.");
        MethodInfo? removeMethod = typeof(SleekWrapper).GetMethod(nameof(SleekWrapper.RemoveChild), BindingFlags.Instance | BindingFlags.Public);
        if (removeMethod == null)
            CommandWindow.LogError($"[{Source}] Unable to find method: SleekWrapper.RemoveChild.");

        List<CodeInstruction> ins = new List<CodeInstruction>(instructions);

        bool add = false, remove = false;
        for (int i = 0; i < ins.Count; ++i)
        {
            if (!add && EmitUtilitiy.FollowPattern(ins, ref i,
                    x => x.opcode == OpCodes.Ldfld,
                    x => addMethod != null && x.Calls(addMethod)
                    ))
            {
                add = true;
                ins.Insert(i, ins[i - 3].CopyWithoutSpecial());
                ins.Insert(i + 1, ins[i - 2].CopyWithoutSpecial());
                ins.Insert(i + 2, new CodeInstruction(OpCodes.Call, addInvoker));
                i += 3;
            }

            if (!remove && EmitUtilitiy.MatchPattern(ins, i,
                    x => x.opcode == OpCodes.Ldfld,
                    x => removeMethod != null && x.Calls(removeMethod)
                ))
            {
                remove = true;
                ins.Insert(i + 1, new CodeInstruction(OpCodes.Dup));
                ins.Insert(i + 2, new CodeInstruction(OpCodes.Call, removeInvoker));
                i += 4;
            }
        }
        if (!add)
        {
            CommandWindow.LogError($"[{Source}] Failed to patch: {method.FullDescription()}, unable to insert add child invoker (on opened).");
        }
        if (!remove)
        {
            CommandWindow.LogError($"[{Source}] Failed to patch: {method.FullDescription()}, unable to insert remove child invoker (on closed).");
        }

        return ins;
    }
    private static void OnOpenedInvoker(object __instance)
    {
        Type type = __instance.GetType();
        CommandWindow.Log($"[{Source}] Started opening " + __instance.ToString());

        if (!UIAccessor.TryGetUITypeInfo(type, out UITypeInfo typeInfo) || typeInfo.CustomOnOpen is not CustomSelectionToolMenuHandler customHandler)
            return;

        customHandler.OnOpened?.Invoke(null, __instance);
        CommandWindow.Log($"[{Source}] Opened " + __instance.ToString() + " ( " + typeInfo.Type + ").");
    }
    private static void OnClosedAndDestroyedInvoker(object __instance)
    {
        Type type = __instance.GetType();
        CommandWindow.Log($"[{Source}] Started closing " + __instance.ToString());

        if (!UIAccessor.TryGetUITypeInfo(type, out UITypeInfo typeInfo) || typeInfo.CustomOnClose is not CustomSelectionToolMenuHandler customHandler)
            return;

        customHandler.OnClosed?.Invoke(null, __instance);
        CommandWindow.Log($"[{Source}] Closed " + __instance.ToString() + " ( " + typeInfo.Type + ").");
    }
}