using DanielWillett.ReflectionTools;
using DanielWillett.UITools.Core.Extensions;
using HarmonyLib;
using SDG.Unturned;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace DanielWillett.UITools.API.Extensions.Members;

/// <summary>
/// Stores information about existing members (defined with <see cref="ExistingMemberAttribute"/>) on an extension.
/// </summary>
public class UIExistingMemberInfo
{
    /// <summary>
    /// Member to get or set in the extension.
    /// </summary>
    public MemberInfo Member { get; }

    /// <summary>
    /// Member to reference in the parent type.
    /// </summary>
    public MemberInfo Existing { get; }

    /// <summary>
    /// If the member in the parent type is static.
    /// </summary>
    public bool ExistingIsStatic { get; }

    /// <summary>
    /// If the member is set when the extension is created, instead of patching the extension member to get the value in realtime (customized by setting <see cref="ExistingMemberAttribute.InitializeMode"/>).
    /// </summary>
    public bool IsInitialized { get; }
    internal UIExistingMemberInfo(MemberInfo member, MemberInfo existing, bool existingIsStatic, bool isInitialized)
    {
        Member = member;
        Existing = existing;
        ExistingIsStatic = existingIsStatic;
        IsInitialized = isInitialized;
    }

    /// <summary>
    /// Emits instructions to get the value (expects the vanilla UI instance is on the stack if it's not a static member) and set the value of the member in the extension.
    /// </summary>
    /// <param name="il">Instruction emitter for a dynamic method.</param>
    /// <param name="onlyRead">Only get the value instead of also setting it to the extension's member.</param>
    public void EmitApply(ILGenerator il, bool onlyRead = false)
    {
        switch (Existing)
        {
            case FieldInfo field:
                il.Emit(ExistingIsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, field);
                break;
            case MethodInfo method:
                il.Emit(method.GetCall(), method);
                break;
            case PropertyInfo property:
                MethodInfo getter = property.GetGetMethod(true);
                if (getter == null)
                    goto default;
                il.Emit(getter.GetCall(), getter);
                break;
            default:
                CommandWindow.LogWarning($"[{UIExtensionManager.Source}] Invalid accessor for existing member: {Member.DeclaringType?.Name ?? "<unknown-type>"}.{Existing.Name}.");
                il.Emit(OpCodes.Ldnull);
                break;
        }
        if (!onlyRead)
        {
            switch (Member)
            {
                case FieldInfo field:
                    il.Emit(field.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, field);
                    break;
                case PropertyInfo property:
                    MethodInfo setter = property.GetSetMethod(true);
                    if (setter == null)
                        goto default;
                    il.Emit(setter.GetCall(), setter);
                    break;
                case MethodInfo method:
                    il.Emit(method.GetCall(), method);
                    break;
                default:
                    CommandWindow.LogWarning($"[{UIExtensionManager.Source}] Invalid accessor for implementing member: {Member.DeclaringType?.Name ?? "<unknown-type>"}.{Member.Name}.");
                    break;
            }
        }
    }

    /// <summary>
    /// Emits the <see cref="CodeInstruction"/>s to get the value (expects the vanilla UI instance is on the stack if it's not a static member) and set the value of the member in the extension.
    /// </summary>
    /// <param name="onlyRead">Only get the value instead of also setting it to the extension's member.</param>
    public IEnumerable<CodeInstruction> EnumerateApply(bool onlyRead = false)
    {
        switch (Existing)
        {
            case FieldInfo field:
                yield return new CodeInstruction(ExistingIsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, field);
                break;
            case MethodInfo method:
                yield return new CodeInstruction(method.GetCall(), method);
                break;
            case PropertyInfo property:
                MethodInfo getter = property.GetGetMethod(true);
                if (getter == null)
                    goto default;
                yield return new CodeInstruction(getter.GetCall(), getter);
                break;
            default:
                CommandWindow.LogWarning($"[{UIExtensionManager.Source}] Invalid accessor for existing member: {Member.DeclaringType?.Name ?? "<unknown-type>"}.{Existing.Name}.");
                yield return new CodeInstruction(OpCodes.Ldnull);
                break;
        }
        if (!onlyRead)
        {
            switch (Member)
            {
                case FieldInfo field:
                    yield return new CodeInstruction(field.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, field);
                    break;
                case PropertyInfo property:
                    MethodInfo setter = property.GetSetMethod(true);
                    if (setter == null)
                        goto default;
                    yield return new CodeInstruction(setter.GetCall(), setter);
                    break;
                case MethodInfo method:
                    yield return new CodeInstruction(method.GetCall(), method);
                    break;
                default:
                    CommandWindow.LogWarning($"[{UIExtensionManager.Source}] Invalid accessor for implementing member: {Member.DeclaringType?.Name ?? "<unknown-type>"}.{Member.Name}.");
                    break;
            }
        }
    }
}