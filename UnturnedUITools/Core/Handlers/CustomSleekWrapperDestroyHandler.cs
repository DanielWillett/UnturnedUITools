using DanielWillett.ReflectionTools;
using DanielWillett.ReflectionTools.Formatting;
using DanielWillett.UITools.API;
using DanielWillett.UITools.Util;
using HarmonyLib;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DanielWillett.UITools.Core.Handlers;
internal class CustomSleekWrapperDestroyHandler : ICustomOnDestroyUIHandler
{
    private const string Source = UIAccessor.Source + ".SLEEK WRAPPER HANDLER";
    private readonly List<MethodInfo> Implementations = new List<MethodInfo>(3);
    private static readonly Dictionary<Type, InstanceGetter<object, SleekWrapper>?> WrapperGetters = new Dictionary<Type, InstanceGetter<object, SleekWrapper>?>(1);

    public event Action<Type?, object?>? OnDestroyed;
    public bool HasBeenInitialized { get; set; }
    public bool HasOnDestroyBeenInitialized { get; set; }
    public void Patch(Harmony patcher)
    {
        MethodInfo? intlDestroy = typeof(ISleekElement).GetMethod(nameof(ISleekElement.InternalDestroy), BindingFlags.Public | BindingFlags.Instance);
        if (intlDestroy == null)
        {
            Accessor.Logger?.LogWarning(Source, $"Unable to find method: {Accessor.Formatter.Format(new MethodDefinition(nameof(ISleekElement.InternalDestroy))
                .DeclaredIn<ISleekElement>(isStatic: false)
                .WithNoParameters()
                .Returning(typeof(void))
            )}.");
            return;
        }

        Implementations.Clear();
        foreach (Type type in Accessor.GetTypesSafe(new Assembly[] { typeof(Provider).Assembly, typeof(ISleekElement).Assembly })
                     .Where(x => x is { IsAbstract: false, IsClass: true } &&
                                 x.Name.IndexOf("Proxy", StringComparison.Ordinal) != -1 &&
                                 typeof(ISleekElement).IsAssignableFrom(x)))
        {
            MethodInfo? impl = Accessor.GetImplementedMethod(type, intlDestroy);

            if (impl == null)
            {
                Accessor.Logger?.LogWarning(Source, $"Unable to find implemented method for {Accessor.Formatter.Format(intlDestroy)} in {Accessor.Formatter.Format(type)}.");
            }
            else
            {
                Implementations.Add(impl);
            }
        }

        for (int i = 0; i < Implementations.Count; ++i)
        {
            try
            {
                patcher.Patch(Implementations[i], prefix: new HarmonyMethod(Accessor.GetMethod(OnDestroyInvoker)!));
            }
            catch (Exception ex)
            {
                Accessor.Logger?.LogError(Source, ex, $"Unable to patch {Accessor.Formatter.Format(Implementations[i])}.");
            }
        }
    }
    public void Unpatch(Harmony patcher)
    {
        foreach (MethodInfo method in Implementations)
        {
            try
            {
                patcher.Unpatch(method, Accessor.GetMethod(OnDestroyInvoker)!);
            }
            catch (Exception ex)
            {
                Accessor.Logger?.LogError(Source, ex, $"Unable to unpatch {Accessor.Formatter.Format(method)}.");
            }
        }
    }
    private static void OnDestroyInvoker(object __instance)
    {
        Type type = __instance.GetType();
        if (!WrapperGetters.TryGetValue(type, out InstanceGetter<object, SleekWrapper>? getter))
        {
            getter = CreateGetter(type);
            WrapperGetters.Add(type, getter);
        }

        if (getter == null)
        {
            Accessor.Logger?.LogWarning(Source, $"Unable to get sleek wrapper from proxy implementation: {Accessor.Formatter.Format(type)}.");
            return;
        }

        __instance = getter(__instance);
        if (__instance == null)
        {
            Accessor.Logger?.LogWarning(Source, $"Sleek wrapper not available from proxy implementation {Accessor.Formatter.Format(type)}.");
            return;
        }

        type = __instance.GetType();

        if (!UIAccessor.TryGetUITypeInfo(type, out UITypeInfo typeInfo) || typeInfo.CustomOnDestroy is not CustomSleekWrapperDestroyHandler customSleekHandler)
            return;

        customSleekHandler.OnDestroyed?.Invoke(null, __instance);
    }
    private static InstanceGetter<object, SleekWrapper>? CreateGetter(Type type)
    {
        FieldInfo? field = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .FirstOrDefault(x => x.FieldType == typeof(SleekWrapper));

        if (field != null)
            return Accessor.GenerateInstanceGetter<SleekWrapper>(type, field.Name);

        PropertyInfo? property = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .FirstOrDefault(x => x.PropertyType == typeof(SleekWrapper));

        if (property != null)
            return Accessor.GenerateInstancePropertyGetter<SleekWrapper>(type, property.Name, allowUnsafeTypeBinding: true);

        Accessor.Logger?.LogWarning(Source, $"Failed to find property or field for SleekWrapper in proxy type {Accessor.Formatter.Format(type)}.");
        return null;
    }
}