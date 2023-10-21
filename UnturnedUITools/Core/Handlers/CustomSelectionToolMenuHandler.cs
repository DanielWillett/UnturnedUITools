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

    private static CustomNodeMenuHandler? _nodes;
    private static CustomVolumeMenuHandler? _volumes;
    public event Action<Type?, object?>? OnClosed;
    public event Action<Type?, object?>? OnOpened;
    private readonly bool _isNodes;
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
        if (this is CustomNodeMenuHandler nodeHandler)
        {
            _nodes = nodeHandler;
            _isNodes = true;
        }
        else if (this is CustomVolumeMenuHandler volumeHandler)
            _volumes = volumeHandler;
    }

    private MethodInfo GetTranspiler() => _isNodes
        ? new Func<IEnumerable<CodeInstruction>, MethodBase, IEnumerable<CodeInstruction>>(TranspileNodeUIOnUpdate).Method
        : new Func<IEnumerable<CodeInstruction>, MethodBase, IEnumerable<CodeInstruction>>(TranspileVolumeUIOnUpdate).Method;
    private MethodInfo GetOnClosedAndDestroyedInvoker() => _isNodes
        ? new Action<object>(OnNodeClosedAndDestroyedInvoker).Method
        : new Action<object>(OnVolumeClosedAndDestroyedInvoker).Method;
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

            patcher.Patch(onUpdateMethod, transpiler: new HarmonyMethod(GetTranspiler()));
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

            patcher.Patch(closeMethod, postfix: new HarmonyMethod(GetOnClosedAndDestroyedInvoker()));
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
            patcher.Unpatch(onUpdateMethod, GetTranspiler());

        if (closeMethod != null)
            patcher.Unpatch(closeMethod, GetOnClosedAndDestroyedInvoker());
    }

    private static IEnumerable<CodeInstruction> TranspileNodeUIOnUpdate(IEnumerable<CodeInstruction> instructions, MethodBase method)
    {
        MethodInfo addInvoker = new Action<object>(OnNodeOpenedInvoker).Method;
        MethodInfo removeInvoker = new Action<object>(OnNodeClosedAndDestroyedInvoker).Method;
        return TranspileUIOnUpdate(instructions, method, addInvoker, removeInvoker);
    }
    private static IEnumerable<CodeInstruction> TranspileVolumeUIOnUpdate(IEnumerable<CodeInstruction> instructions, MethodBase method)
    {
        MethodInfo addInvoker = new Action<object>(OnVolumeOpenedInvoker).Method;
        MethodInfo removeInvoker = new Action<object>(OnVolumeClosedAndDestroyedInvoker).Method;
        return TranspileUIOnUpdate(instructions, method, addInvoker, removeInvoker);
    }

    private static IEnumerable<CodeInstruction> TranspileUIOnUpdate(IEnumerable<CodeInstruction> instructions, MethodBase method, MethodInfo addInvoker, MethodInfo removeInvoker)
    {
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
                ins.Insert(i, ins[i - 2].CopyWithoutSpecial());
                ins.Insert(i + 1, new CodeInstruction(OpCodes.Call, addInvoker));
                i += 2;
            }

            if (!remove && EmitUtilitiy.MatchPattern(ins, i,
                    x => x.opcode == OpCodes.Ldfld,
                    x => removeMethod != null && x.Calls(removeMethod)
                ))
            {
                remove = true;
                ins.Insert(i, new CodeInstruction(OpCodes.Dup));
                ins.Insert(i + 1, new CodeInstruction(OpCodes.Call, removeInvoker));
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
    private static void OnNodeOpenedInvoker(object __instance)
    {
        _nodes?.OnOpened?.Invoke(null, __instance);
    }
    private static void OnNodeClosedAndDestroyedInvoker(object __instance)
    {
        _nodes?.OnClosed?.Invoke(null, __instance);
    }
    private static void OnVolumeOpenedInvoker(object __instance)
    {
        _volumes?.OnOpened?.Invoke(null, __instance);
    }
    private static void OnVolumeClosedAndDestroyedInvoker(object __instance)
    {
        _volumes?.OnClosed?.Invoke(null, __instance);
    }
}