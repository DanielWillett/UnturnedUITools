using DanielWillett.ReflectionTools;
using DanielWillett.UITools.API;
using DanielWillett.UITools.API.Extensions;
using DanielWillett.UITools.API.Extensions.Members;
using DanielWillett.UITools.Util;
using HarmonyLib;
using JetBrains.Annotations;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using UnityEngine;
using Module = SDG.Framework.Modules.Module;

namespace DanielWillett.UITools.Core.Extensions;

/// <summary>
/// Default implementation of <see cref="IUIExtensionManager"/>, manages UI extensions.
/// </summary>
public class UIExtensionManager : MonoBehaviour, IUIExtensionManager, IDisposable
{
    internal const string Source = "UI EXT MNGR";
    private readonly List<UIExtensionInfo> _extensions = new List<UIExtensionInfo>(8);
    private readonly List<IUnpatchableExtension> _pendingUnpatches = new List<IUnpatchableExtension>();

    private bool _isAnActualObject;

    /// <summary>
    /// Stores all extensions in a mutable dictionary.
    /// </summary>
    protected readonly Dictionary<Type, UIExtensionInfo> ExtensionsDictIntl = new Dictionary<Type, UIExtensionInfo>(8);

    /// <summary>
    /// Stores all vanilla UI types in a mutable dictionary.
    /// </summary>
    protected readonly Dictionary<Type, UIExtensionParentTypeInfo> ParentTypeInfoIntl = new Dictionary<Type, UIExtensionParentTypeInfo>(8);

    /// <summary>
    /// Stores all extension patches in a mutable dictionary.
    /// </summary>
    protected readonly Dictionary<MethodBase, UIExtensionPatch> Patches = new Dictionary<MethodBase, UIExtensionPatch>(64);

    /// <summary>
    /// Stores all extension member patches in a mutable dictionary.
    /// </summary>
    protected readonly Dictionary<MethodBase, UIExtensionExistingMemberPatchInfo> PatchInfo = new Dictionary<MethodBase, UIExtensionExistingMemberPatchInfo>(64);

    /// <summary>
    /// Invoked when an extension is destroyed.
    /// </summary>
    protected Action<object>? OnRemoved;

    /// <summary>
    /// Invoked when an extension is initialzied.
    /// </summary>
    protected Action<object>? OnAdd;

    /// <summary>
    /// List of all registered UI Extensions (info, not instances themselves).
    /// </summary>
    public IReadOnlyList<UIExtensionInfo> Extensions { get; }

    /// <inheritdoc />
    public bool DebugLogging { get; set; }

    /// <summary>
    /// Dictionary of all parent types. Parent types references the vanilla UI type backend. (i.e. <see cref="PlayerDashboardCraftingUI"/>)
    /// </summary>
    public IReadOnlyDictionary<Type, UIExtensionParentTypeInfo> ParentTypeInfo { get; }

    /// <summary>
    /// Create a new default implementation of <see cref="IUIExtensionManager"/>.
    /// </summary>
    public UIExtensionManager()
    {
        ParentTypeInfo = new ReadOnlyDictionary<Type, UIExtensionParentTypeInfo>(ParentTypeInfoIntl);
        Extensions = _extensions.AsReadOnly();
        _isAnActualObject = false;
    }

    [UsedImplicitly]
    private void Awake()
    {
        _isAnActualObject = true;
    }

    /// <summary>
    /// Run any start-up requirements. This should not include any extension searching, as those will be registered with <see cref="RegisterFromModuleAssembly"/> and <see cref="RegisterExtension"/>.
    /// </summary>
    protected virtual void Initialize()
    {
        ThreadUtil.assertIsGameThread();
    }

    /// <summary>
    /// Clean up any patches.
    /// </summary>
    protected virtual void Dispose()
    {
        ThreadUtil.assertIsGameThread();

        if (DebugLogging)
            LogDebug("Cleaning up extensions...");

        for (int i = _extensions.Count - 1; i >= 0; i--)
        {
            UIExtensionInfo extension = _extensions[i];

            if (!ParentTypeInfoIntl.TryGetValue(extension.ParentType, out UIExtensionParentTypeInfo? parentTypeInfo))
                continue;

            bool close = parentTypeInfo is { ParentTypeInfo.CloseOnDestroy: true };
            for (int j = parentTypeInfo.InstancesIntl.Count - 1; j >= 0; j--)
            {
                UIExtensionInstanceInfo instanceInfo = parentTypeInfo.InstancesIntl[j];
                if (close && instanceInfo.VanillaInstance.IsOpen)
                {
                    if (instanceInfo.Instance is IUIExtension ext)
                    {
                        try
                        {
                            ext.OnClosed();
                        }
                        catch (Exception ex)
                        {
                            LogError($"Error invoking OnClosed from {instanceInfo.Instance.GetType().Name} while destroying.", extension.Module, extension.Assembly);
                            CommandWindow.LogError(ex);
                        }
                    }
                }
                if (instanceInfo.Instance is IDisposable disposable)
                {
                    try
                    {
                        disposable.Dispose();
                        if (DebugLogging)
                            LogDebug($"* Disposed: {extension.ImplementationType.Name}.", extension.Module, extension.Assembly);
                    }
                    catch (Exception ex)
                    {
                        LogError($"Error disposing UI extension: {extension.ImplementationType.Name}.", extension.Module, extension.Assembly);
                        CommandWindow.LogError(ex);
                    }
                }

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (_isAnActualObject && instanceInfo.Instance is UnityEngine.Object obj && obj != null)
                    DestroyImmediate(obj);

                if (instanceInfo.Extension.InstantiationsIntl.Count == 1 && instanceInfo.Extension.InstantiationsIntl[0] == instanceInfo.Instance)
                {
                    instanceInfo.Extension.InstantiationsIntl.RemoveAt(0);
                    if (instanceInfo.Instance is IUnpatchableExtension unpatchable)
                    {
                        Type instanceType = unpatchable.GetType();
                        if (!_pendingUnpatches.Exists(x => x.GetType() == instanceType))
                            _pendingUnpatches.Add(unpatchable);
                    }
                }
                else
                    instanceInfo.Extension.InstantiationsIntl.Remove(instanceInfo.Instance);
                parentTypeInfo.InstancesIntl.RemoveAt(i);
                OnRemoved?.Invoke(instanceInfo.Instance);
            }
        }

        _extensions.Clear();
        ExtensionsDictIntl.Clear();

        if (DebugLogging)
            LogDebug("Unpatching visibility listeners...");
        /* Unpatch open, close, initialize, and destroy patches */
        foreach (UIExtensionPatch patch in Patches.Values.Reverse())
        {
            try
            {
                UIAccessor.Patcher.Unpatch(patch.Original, patch.Patch);
            }
            catch (Exception ex)
            {
                LogError($"Unable to unpatch {patch.Original.FullDescription()}.");
                CommandWindow.LogError(ex);
            }
        }
        Patches.Clear();

        if (DebugLogging)
            LogDebug("Unpatching IUnpatchableExtension extensions...");
        /* Unpatch extensions implementing IUnpatchableExtension */
        for (int i = 0; i < _pendingUnpatches.Count; ++i)
        {
            try
            {
                _pendingUnpatches[i].Unpatch();
            }
            catch (Exception ex)
            {
                LogError($"Failed to unpatch extension {_pendingUnpatches[i].GetType().Name}.");
                CommandWindow.LogError(ex);
            }
        }
        _pendingUnpatches.Clear();

        if (DebugLogging)
            LogDebug("Unpatching existing member implementations...");
        /* Unpatch existing member getters */
        foreach (KeyValuePair<MethodBase, UIExtensionExistingMemberPatchInfo> existingMember in PatchInfo.Reverse())
        {
            try
            {
                UIAccessor.Patcher.Unpatch(existingMember.Key, existingMember.Value.Transpiler);
            }
            catch (Exception ex)
            {
                LogError($"Unable to unpatch existing member {existingMember.Key.FullDescription()}.");
                CommandWindow.LogError(ex);
            }
        }
        PatchInfo.Clear();

        ParentTypeInfoIntl.Clear();

        if (DebugLogging)
            LogDebug("Unpatching custom UI handlers...");
        /* Unpatch custom handlers */
        foreach (UITypeInfo typeInfo in UIAccessor.TypeInfo.Values)
        {
            if (typeInfo.CustomOnOpen != null)
            {
                if (typeInfo.CustomOnOpen.HasBeenInitialized)
                {
                    typeInfo.CustomOnOpen.HasBeenInitialized = false;
                    try
                    {
                        typeInfo.CustomOnOpen.Unpatch(UIAccessor.Patcher);
                    }
                    catch (Exception ex)
                    {
                        LogError($"Failed to unpatch CustomOnOpen {typeInfo.CustomOnOpen.GetType().Name} for {typeInfo.Type.Name}.");
                        CommandWindow.LogError(ex);
                    }
                }
                if (typeInfo.CustomOnOpen.HasOnOpenBeenInitialized)
                {
                    typeInfo.CustomOnOpen.HasOnOpenBeenInitialized = false;
                    typeInfo.CustomOnOpen.OnOpened -= OnOpened;
                }
            }
            if (typeInfo.CustomOnClose != null)
            {
                if (typeInfo.CustomOnClose.HasBeenInitialized)
                {
                    typeInfo.CustomOnClose.HasBeenInitialized = false;
                    try
                    {
                        typeInfo.CustomOnClose.Unpatch(UIAccessor.Patcher);
                    }
                    catch (Exception ex)
                    {
                        LogError($"Failed to unpatch CustomOnClose {typeInfo.CustomOnClose.GetType().Name} for {typeInfo.Type.Name}.");
                        CommandWindow.LogError(ex);
                    }
                }
                if (typeInfo.CustomOnClose.HasOnCloseBeenInitialized)
                {
                    typeInfo.CustomOnClose.HasOnCloseBeenInitialized = false;
                    typeInfo.CustomOnClose.OnClosed -= OnClosed;
                }
            }
            if (typeInfo.CustomOnDestroy != null)
            {
                if (typeInfo.CustomOnDestroy.HasBeenInitialized)
                {
                    typeInfo.CustomOnDestroy.HasBeenInitialized = false;
                    try
                    {
                        typeInfo.CustomOnDestroy.Unpatch(UIAccessor.Patcher);
                    }
                    catch (Exception ex)
                    {
                        LogError($"Failed to unpatch CustomOnDestroy {typeInfo.CustomOnDestroy.GetType().Name} for {typeInfo.Type.Name}.");
                        CommandWindow.LogError(ex);
                    }
                }
                if (typeInfo.CustomOnDestroy.HasOnDestroyBeenInitialized)
                {
                    typeInfo.CustomOnDestroy.HasOnDestroyBeenInitialized = false;
                    typeInfo.CustomOnDestroy.OnDestroyed -= OnDestroy;
                }
            }
            if (typeInfo.CustomOnInitialize != null)
            {
                if (typeInfo.CustomOnInitialize.HasBeenInitialized)
                {
                    typeInfo.CustomOnInitialize.HasBeenInitialized = false;
                    try
                    {
                        typeInfo.CustomOnInitialize.Unpatch(UIAccessor.Patcher);
                    }
                    catch (Exception ex)
                    {
                        LogError($"Failed to unpatch CustomOnInitialize {typeInfo.CustomOnInitialize.GetType().Name} for {typeInfo.Type.Name}.");
                        CommandWindow.LogError(ex);
                    }
                }
                if (typeInfo.CustomOnInitialize.HasOnInitializeBeenInitialized)
                {
                    typeInfo.CustomOnInitialize.HasOnInitializeBeenInitialized = false;
                    typeInfo.CustomOnInitialize.OnInitialized -= OnInitialized;
                }
            }
        }
    }

    /// <inheritdoc />
    void IUIExtensionManager.Initialize() => Initialize();

    /// <inheritdoc />
    void IDisposable.Dispose() => Dispose();

    /// <inheritdoc />
    public virtual T? GetInstance<T>() where T : class => InstanceCache<T>.Instance;

    /// <inheritdoc />
    public virtual T? GetInstance<T>(object vanillaUIInstance) where T : class
    {
        Type extType = typeof(T);
        if (!ExtensionsDictIntl.TryGetValue(extType, out UIExtensionInfo extension))
            return null;

        ParentTypeInfoIntl.TryGetValue(extension.ParentType, out UIExtensionParentTypeInfo parentTypeInfo);
        UIExtensionInstanceInfo? extInstance = parentTypeInfo?.InstancesIntl.FindLast(x => x.Instance is T && ReferenceEquals(x.VanillaInstance.Instance, vanillaUIInstance));
        return extInstance?.Instance as T;
    }
    
    /// <summary>
    /// Logs a message in debug format.
    /// </summary>
    protected virtual void LogDebug(string message, Module? module = null, Assembly? assembly = null)
    {
        message = "[DBG] " + message;
        if (module == null)
            CommandWindow.Log(assembly == null ? "[" + Source + "] " + message : ("[" + Source + " | " + assembly.GetName().Name.ToUpperInvariant() + "] " + message));
        else
            CommandWindow.Log("[" + Source + " | " + module.config.Name + " v" + module.config.Version + "] " + message);
    }

    /// <summary>
    /// Logs a message in info format.
    /// </summary>
    protected virtual void LogInfo(string message, Module? module = null, Assembly? assembly = null)
    {
        if (module == null)
            CommandWindow.Log(assembly == null ? "[" + Source + "] " + message : ("[" + Source + " | " + assembly.GetName().Name.ToUpperInvariant() + "] " + message));
        else
            CommandWindow.Log("[" + Source + " | " + module.config.Name + " v" + module.config.Version + "] " + message);
    }

    /// <summary>
    /// Logs a message in warning format.
    /// </summary>
    protected virtual void LogWarning(string message, Module? module = null, Assembly? assembly = null)
    {
        if (module == null)
            CommandWindow.LogWarning(assembly == null ? "[" + Source + "] " + message : ("[" + Source + " | " + assembly.GetName().Name.ToUpperInvariant() + "] " + message));
        else
            CommandWindow.LogWarning("[" + Source + " | " + module.config.Name + " v" + module.config.Version + "] " + message);
    }

    /// <summary>
    /// Logs a message in error format.
    /// </summary>
    protected virtual void LogError(string message, Module? module = null, Assembly? assembly = null)
    {
        if (module == null)
            CommandWindow.LogError(assembly == null ? "[" + Source + "] " + message : ("[" + Source + " | " + assembly.GetName().Name.ToUpperInvariant() + "] " + message));
        else
            CommandWindow.LogError("[" + Source + " | " + module.config.Name + " v" + module.config.Version + "] " + message);
    }

    /// <inheritdoc />
    public virtual void RegisterFromModuleAssembly(Assembly assembly, Module module)
    {
        ThreadUtil.assertIsGameThread();

        List<Type> types = Accessor.GetTypesSafe(assembly, true);
        foreach (Type type in types)
        {
            if (Attribute.GetCustomAttribute(type, typeof(UIExtensionAttribute)) is not UIExtensionAttribute attribute)
                continue;

            if (attribute.ParentType == null)
            {
                LogError($"Error initializing UI extension: {type.Name}. Unknown parent type in [UIExtension] attribute: \"{attribute.SearchedParentType ?? "<unknown>"}\".", module, type.Assembly);
                continue;
            }

            PriorityAttribute? priority = Attribute.GetCustomAttribute(type, typeof(PriorityAttribute)) as PriorityAttribute;

            UIExtensionInfo info = new UIExtensionInfo(type, attribute.ParentType, priority?.Priority ?? 0, module)
            {
                SuppressUIExtensionParentWarning = attribute.SuppressUIExtensionParentWarning
            };

            try
            {
                InitializeExtension(info);
            }
            catch (Exception ex)
            {
                LogError($"Error initializing UI extension: {type.Name}.", module, type.Assembly);
                CommandWindow.LogError(ex);
                continue;
            }
            
            if (DebugLogging)
                LogDebug($"Registered UI extension: {type.Name}.", module, type.Assembly);
        }
    }

    /// <summary>
    /// Called when a UI is detected to have opened.
    /// </summary>
    /// <param name="type">Type of the UI that was opened. Won't be <see langword="null"/> unless <paramref name="instance"/> has a value.</param>
    /// <param name="instance">Instance of a non-static UI that was opened. Won't be <see langword="null"/> unless <paramref name="type"/> has a value.</param>
    protected virtual void OnOpened(Type? type, object? instance)
    {
        if (instance != null)
            type = instance.GetType();

        if (DebugLogging)
            LogDebug($"Opened: {type!.Name}.");

        if (!ParentTypeInfoIntl.TryGetValue(type!, out UIExtensionParentTypeInfo parentTypeInfo))
        {
            LogWarning($"Unable to find parent type info while opening {type!.Name}.");
            return;
        }

        bool found = false;
        for (int i = 0; i < parentTypeInfo.VanillaInstancesIntl.Count; ++i)
        {
            UIExtensionVanillaInstanceInfo instanceInfo = parentTypeInfo.VanillaInstancesIntl[i];
            if (ReferenceEquals(instanceInfo.Instance, instance) || parentTypeInfo.VanillaInstancesIntl.Count == 1 && parentTypeInfo.ParentTypeInfo.IsInstanceUI)
            {
                if (instanceInfo.IsOpen)
                {
                    if (DebugLogging)
                        LogDebug($"Already open: {type!.Name}.");
                    return;
                }

                found = true;
                instanceInfo.IsOpen = true;
                break;
            }
        }

        if (!found)
        {
            LogWarning($"Unable to find vanilla instance info while opening {type!.Name}.");
            return;
        }

        if (parentTypeInfo.InstancesIntl.Count == 0)
        {
            if (DebugLogging)
                LogDebug($"No instances attached to UI type {type!.Name} to open.");
            return;
        }

        bool anyClosed = false;
        for (int i = parentTypeInfo.InstancesIntl.Count - 1; i >= 0; i--)
        {
            UIExtensionInstanceInfo instanceInfo = parentTypeInfo.InstancesIntl[i];

            if (!ReferenceEquals(instanceInfo.VanillaInstance.Instance, instance) && parentTypeInfo.ParentTypeInfo is { IsInstanceUI: false, IsStaticUI: false })
                continue;

            if (DebugLogging)
                LogDebug($"* Opening instance of: {instanceInfo.Extension.ImplementationType.Name}.", instanceInfo.Extension.Module, instanceInfo.Extension.Assembly);

            if (instanceInfo.Instance is IUIExtension ext)
            {
                try
                {
                    ext.OnOpened();
                }
                catch (Exception ex)
                {
                    LogError($"Error invoking OnOpened from {instanceInfo.Instance.GetType().Name}.", instanceInfo.Extension.Module, instanceInfo.Extension.Assembly);
                    CommandWindow.LogError(ex);
                }
            }

            if (DebugLogging)
                LogDebug($"* Opened: {instanceInfo.Extension.ImplementationType.Name}.", instanceInfo.Extension.Module, instanceInfo.Extension.Assembly);

            anyClosed = true;
        }

        if (!anyClosed && DebugLogging)
        {
            LogDebug($"No instances attached to UI type {type!.Name} with the provided instance to open.");
        }
    }

    /// <summary>
    /// Called when a UI is detected to have closed.
    /// </summary>
    /// <param name="type">Type of the UI that was opened. Won't be <see langword="null"/> unless <paramref name="instance"/> has a value.</param>
    /// <param name="instance">Instance of a non-static UI that was opened. Won't be <see langword="null"/> unless <paramref name="type"/> has a value.</param>
    protected virtual void OnClosed(Type? type, object? instance)
    {
        if (instance != null)
            type = instance.GetType();
        if (DebugLogging)
            LogDebug($"Closed: {type!.Name}.");
        if (!ParentTypeInfoIntl.TryGetValue(type!, out UIExtensionParentTypeInfo parentTypeInfo))
        {
            LogWarning($"Unable to find parent type info while closing {type!.Name}.");
            return;
        }

        bool found = false;
        for (int i = 0; i < parentTypeInfo.VanillaInstancesIntl.Count; ++i)
        {
            UIExtensionVanillaInstanceInfo instanceInfo = parentTypeInfo.VanillaInstancesIntl[i];
            if (ReferenceEquals(instanceInfo.Instance, instance) || parentTypeInfo.VanillaInstancesIntl.Count == 1 && parentTypeInfo.ParentTypeInfo.IsInstanceUI)
            {
                if (!instanceInfo.IsOpen)
                {
                    if (DebugLogging)
                        LogDebug($"Already closed: {type!.Name}.");
                    return;
                }

                found = true;
                instanceInfo.IsOpen = false;
                break;
            }
        }

        if (!found)
        {
            LogWarning($"Unable to find vanilla instance info while closing {type!.Name}.");
            return;
        }

        if (parentTypeInfo.InstancesIntl.Count == 0)
        {
            if (DebugLogging)
                LogDebug($"No instances attached to UI type {type!.Name} to close.");
            return;
        }

        bool anyClosed = false;
        for (int i = parentTypeInfo.InstancesIntl.Count - 1; i >= 0; i--)
        {
            UIExtensionInstanceInfo instanceInfo = parentTypeInfo.InstancesIntl[i];

            if (!ReferenceEquals(instanceInfo.VanillaInstance.Instance, instance) && parentTypeInfo.ParentTypeInfo is { IsInstanceUI: false, IsStaticUI: false })
                continue;

            if (DebugLogging)
                LogDebug($"* Closing instance of: {instanceInfo.Extension.ImplementationType.Name}.", instanceInfo.Extension.Module, instanceInfo.Extension.Assembly);

            if (instanceInfo.Instance is IUIExtension ext)
            {
                try
                {
                    ext.OnClosed();
                }
                catch (Exception ex)
                {
                    LogError($"Error invoking OnClosed from {instanceInfo.Instance.GetType().Name}.", instanceInfo.Extension.Module, instanceInfo.Extension.Assembly);
                    CommandWindow.LogError(ex);
                }
            }

            if (DebugLogging)
                LogDebug($"* Closed: {instanceInfo.Extension.ImplementationType.Name}.", instanceInfo.Extension.Module, instanceInfo.Extension.Assembly);

            anyClosed = true;
        }

        if (!anyClosed && DebugLogging)
        {
            LogDebug($"No instances attached to UI type {type!.Name} with the provided instance to close.");
        }
    }

    /// <summary>
    /// Called when a UI is detected to have been initialized.
    /// </summary>
    /// <param name="type">Type of the UI that was opened. Won't be <see langword="null"/> unless <paramref name="instance"/> has a value.</param>
    /// <param name="instance">Instance of a non-static UI that was opened. Won't be <see langword="null"/> unless <paramref name="type"/> has a value.</param>
    protected virtual void OnInitialized(Type? type, object? instance)
    {
        if (instance != null)
            type = instance.GetType();
        if (DebugLogging)
            LogDebug($"Initialized: {type!.Name}.");


        for (int i = 0; i < _extensions.Count; i++)
        {
            UIExtensionInfo info = _extensions[i];
            if (info.ParentType != type) continue;
            object? ext = CreateExtension(info, instance);
            if (ext == null) continue;
            if (DebugLogging)
                LogDebug($"* Initialized: {info.ImplementationType.Name}.", info.Module, info.Assembly);
            if ((info.TypeInfo.OpenOnInitialize || info.TypeInfo.DefaultOpenState) && ext is IUIExtension ext2)
            {
                try
                {
                    ext2.OnOpened();
                }
                catch (Exception ex)
                {
                    LogError($"Error invoking OnOpened from {ext.GetType().Name}.", info.Module, info.Assembly);
                    CommandWindow.LogError(ex);
                }
                if (DebugLogging)
                    LogDebug($"  * Opened: {info.ImplementationType.Name}.", info.Module, info.Assembly);
            }
        }
    }

    /// <summary>
    /// Called when a UI is detected to have been destroyed.
    /// </summary>
    /// <param name="type">Type of the UI that was opened. Won't be <see langword="null"/> unless <paramref name="instance"/> has a value.</param>
    /// <param name="instance">Instance of a non-static UI that was opened. Won't be <see langword="null"/> unless <paramref name="type"/> has a value.</param>
    protected virtual void OnDestroy(Type? type, object? instance)
    {
        if (instance != null)
            type = instance.GetType();

        bool logged = false;

        foreach (UITypeInfo otherTypeInfo in UIAccessor.TypeInfo.Values)
        {
            if (otherTypeInfo.DestroyWhenParentDestroys && otherTypeInfo.Type != type && otherTypeInfo.Parent == type)
            {
                if (!logged)
                {
                    logged = true;
                    if (DebugLogging)
                        LogDebug($"Destroyed: {type!.Name}.");
                }
                if (DebugLogging)
                    LogDebug($"* Destroying child: {otherTypeInfo.Type.Name}.");
                OnDestroy(otherTypeInfo.Type, null);
            }
        }

        if (!ParentTypeInfoIntl.TryGetValue(type!, out UIExtensionParentTypeInfo parentTypeInfo))
        {
            // not extended
            return;
        }

        if (!logged && DebugLogging)
        {
            LogDebug($"Destroyed: {type!.Name}.");
        }

        bool close = parentTypeInfo.ParentTypeInfo.CloseOnDestroy;
        if (close)
        {
            bool found = false;
            for (int i = 0; i < parentTypeInfo.VanillaInstancesIntl.Count; ++i)
            {
                UIExtensionVanillaInstanceInfo instanceInfo = parentTypeInfo.VanillaInstancesIntl[i];
                if (ReferenceEquals(instanceInfo.Instance, instance) ||
                    parentTypeInfo.VanillaInstancesIntl.Count == 1 && parentTypeInfo.ParentTypeInfo.IsInstanceUI)
                {
                    if (!instanceInfo.IsOpen)
                    {
                        if (DebugLogging)
                            LogDebug($"Already closed: {type!.Name}.");
                        close = false;
                    }
                    else instanceInfo.IsOpen = false;

                    found = true;
                    parentTypeInfo.VanillaInstancesIntl.RemoveAt(i);
                    break;
                }
            }

            if (!found)
            {
                LogWarning($"Unable to find vanilla instance info while closing (destroying) {type!.Name}.");
                close = false;
            }
        }

        if (parentTypeInfo.InstancesIntl.Count == 0)
        {
            if (DebugLogging)
                LogDebug($"No instances attached to UI type {type!.Name} to destroy.");
            return;
        }

        bool anyDestroyed = false;
        for (int i = parentTypeInfo.InstancesIntl.Count - 1; i >= 0; i--)
        {
            UIExtensionInstanceInfo instanceInfo = parentTypeInfo.InstancesIntl[i];

            if (!ReferenceEquals(instanceInfo.VanillaInstance.Instance, instance) && parentTypeInfo.ParentTypeInfo is { IsInstanceUI: false, IsStaticUI: false })
                continue;

            if (DebugLogging)
                LogDebug($"* Destroying instance of: {instanceInfo.Extension.ImplementationType.Name}.", instanceInfo.Extension.Module, instanceInfo.Extension.Assembly);

            if (close && instanceInfo.Instance is IUIExtension ext)
            {
                try
                {
                    ext.OnClosed();
                }
                catch (Exception ex)
                {
                    LogError($"Error invoking OnClosed from {instanceInfo.Instance.GetType().Name} while destroying.", instanceInfo.Extension.Module, instanceInfo.Extension.Assembly);
                    CommandWindow.LogError(ex);
                }

                if (DebugLogging)
                    LogDebug($"  * Closed: {instanceInfo.Extension.ImplementationType.Name}.", instanceInfo.Extension.Module, instanceInfo.Extension.Assembly);
            }
            if (instanceInfo.Instance is IDisposable disposable)
            {
                try
                {
                    disposable.Dispose();
                }
                catch (Exception ex)
                {
                    LogError($"Error disposing UI extension: {instanceInfo.Extension.ImplementationType.Name}.", instanceInfo.Extension.Module, instanceInfo.Extension.Assembly);
                    CommandWindow.LogError(ex);
                }

                if (DebugLogging)
                    LogDebug($"  * Disposed: {instanceInfo.Extension.ImplementationType.Name}.", instanceInfo.Extension.Module, instanceInfo.Extension.Assembly);
            }

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (_isAnActualObject && instanceInfo.Instance is UnityEngine.Object obj && obj != null)
            {
                Destroy(obj);

                if (DebugLogging)
                    LogDebug($"  * Destroyed: {instanceInfo.Extension.ImplementationType.Name}.", instanceInfo.Extension.Module, instanceInfo.Extension.Assembly);
            }

            if (instanceInfo.Extension.InstantiationsIntl.Count == 1 && instanceInfo.Extension.InstantiationsIntl[0] == instanceInfo.Instance)
            {
                instanceInfo.Extension.InstantiationsIntl.RemoveAt(0);
                if (instanceInfo.Instance is IUnpatchableExtension unpatchable)
                {
                    Type instanceType = unpatchable.GetType();
                    if (!_pendingUnpatches.Exists(x => x.GetType() == instanceType))
                        _pendingUnpatches.Add(unpatchable);
                }
            }
            else
                instanceInfo.Extension.InstantiationsIntl.Remove(instanceInfo.Instance);
            parentTypeInfo.InstancesIntl.RemoveAt(i);
            OnRemoved?.Invoke(instanceInfo.Instance);
            anyDestroyed = true;
        }

        if (!anyDestroyed && DebugLogging)
        {
            LogDebug($"No instances attached to UI type {type!.Name} with the provided instance to destroy.");
        }
    }

    /// <inheritdoc />
    public virtual void RegisterExtension(Type extensionType, Type parentType, Module module)
    {
        ThreadUtil.assertIsGameThread();

        UIExtensionAttribute? attribute = (UIExtensionAttribute?)Attribute.GetCustomAttribute(extensionType, typeof(UIExtensionAttribute));

        PriorityAttribute? priority = Attribute.GetCustomAttribute(extensionType, typeof(PriorityAttribute)) as PriorityAttribute;

        UIExtensionInfo info = new UIExtensionInfo(extensionType, parentType, priority?.Priority ?? 0, module)
        {
            SuppressUIExtensionParentWarning = attribute == null || attribute.SuppressUIExtensionParentWarning
        };

        try
        {
            InitializeExtension(info);
        }
        catch (Exception ex)
        {
            throw new Exception($"Unable to initialize UI extension: {extensionType.FullName}.", ex);
        }

        if (DebugLogging)
            LogDebug($"Registered UI extension: {extensionType.Name}.", module, extensionType.Assembly);
    }

    private void AddToList(UIExtensionInfo info)
    {
        lock (this)
        {
            int priority = info.Priority;
            int index = _extensions.FindIndex(x => x.Priority <= priority);
            if (index == -1)
                _extensions.Add(info);
            else
                _extensions.Insert(index, info);

            ExtensionsDictIntl[info.ImplementationType] = info;
        }
    }

    /// <summary>
    /// Main initialize method for a <see cref="UIExtensionInfo"/> (representing an extension class).
    /// </summary>
    /// <remarks>Should call <see cref="TryInitializeMember"/>, <see cref="InitializeExtensionPatches"/>, and <see cref="InitializeParentPatches"/>.</remarks>
    /// <exception cref="AggregateException">Something went wrong initializing the extension.</exception>
    protected virtual void InitializeExtension(UIExtensionInfo info)
    {
        if (!info.SuppressUIExtensionParentWarning && !typeof(IUIExtension).IsAssignableFrom(info.ImplementationType))
        {
            LogWarning($"It's recommended to derive UI extensions from the {nameof(UIExtension)} class or the {nameof(IUIExtension)} interface (unlike {info.ImplementationType.Name}).", info.Module, info.Assembly);
            LogInfo($"Alternatively set SuppressUIExtensionParentWarning to True in a {nameof(UIExtensionAttribute)} on the extension class.", info.Module, info.Assembly);
        }

        if (!UIAccessor.TryGetUITypeInfo(info.ParentType, out UITypeInfo? typeInfo))
        {
            LogWarning($"No type info for parent UI type: {info.ParentType.Name}, {info.ImplementationType.Name} UI extension may not behave as expected. Any warnings below:", info.Module, info.Assembly);
        }

        if (typeInfo == null)
        {
            typeInfo = new UITypeInfo(info.ParentType);
            LogInfo($"Created UI type info for {info.ParentType.Name}: {typeInfo.OpenMethods.Length} open method(s), " +
                    $"{typeInfo.CloseMethods.Length} close method(s), {typeInfo.InitializeMethods.Length} initialize method(s), " +
                    $"{typeInfo.DestroyMethods.Length} destroy method(s).", info.Module, info.Assembly);
        }

        info.TypeInfo = typeInfo;

        bool staticUI = !info.ParentType.GetIsStatic() && typeInfo.IsStaticUI;

        List<Exception>? exceptions = null;

        ConstructorInfo[] ctors = info.ImplementationType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        ConstructorInfo? constructor = staticUI ? null : ctors.FirstOrDefault(x => x.GetParameters().Length == 1 && x.GetParameters()[0].ParameterType.IsAssignableFrom(info.ParentType));
        if (constructor == null)
        {
            constructor = ctors.FirstOrDefault(x => x.GetParameters().Length == 0);
            if (constructor == null)
                (exceptions ??= new List<Exception>()).Add(new InvalidOperationException("Type " + info.ImplementationType.Name + " does not have a parameterless constructor or an instance input constructor."));
        }

        foreach (MemberInfo member in info.ImplementationType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                     .Concat<MemberInfo>(info.ImplementationType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy))
                     .Concat(info.ImplementationType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                         .Where(x => !x.IsSpecialName))
                     .OrderByDescending(Accessor.GetPriority)
                     .ThenBy(x => x.Name))
        {
            if (member.IsIgnored())
                continue;
            try
            {
                TryInitializeMember(info, member);
            }
            catch (Exception ex)
            {
                (exceptions ??= new List<Exception>()).Add(ex);
            }
        }

        if (exceptions != null)
            throw new AggregateException($"Failed to initialze UI extension: {info.ImplementationType.Name}.", exceptions);

        try
        {
            MethodInfo getTypeFromHandle = Accessor.GetMethod(Type.GetTypeFromHandle)!;
            MethodInfo getUninitObject = Accessor.GetMethod(FormatterServices.GetUninitializedObject)!;

            bool isUnityObject = typeof(Component).IsAssignableFrom(info.ImplementationType);

            MethodInfo? getGameObjectProperty = !isUnityObject ? null : typeof(Component).GetProperty(nameof(transform), BindingFlags.Instance | BindingFlags.Public)?.GetMethod;
            MethodInfo? addComponentMethod = !isUnityObject ? null : typeof(GameObject).GetMethod(nameof(GameObject.AddComponent),
                BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.Any, [ typeof(Type) ], null);
            MethodInfo? equalsMethod = !isUnityObject ? null : typeof(UnityEngine.Object).GetMethod(nameof(object.Equals),
                BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.Any,
                new Type[] { typeof(object) }, null);
            if (isUnityObject)
            {
                if (!_isAnActualObject)
                {
                    LogWarning($"Unity component {info.ImplementationType.Name} will not be instantiated properly because the UIExtensionManager was not initialized as a component.");
                    isUnityObject = false;
                }
                if (getGameObjectProperty == null)
                {
                    LogWarning($"Unknown property: get UnityEngine.Component.gameObject. Unity component {info.ImplementationType.Name} will not be instantiated properly.");
                    isUnityObject = false;
                }
                if (addComponentMethod == null)
                {
                    LogWarning($"Unknown method: UnityEngine.GameObject.AddComponent. Unity component {info.ImplementationType.Name} will not be instantiated properly.");
                    isUnityObject = false;
                }
                if (equalsMethod == null)
                {
                    LogWarning($"Unknown method: UnityEngine.Object.Equals. Unity component {info.ImplementationType.Name} will not be instantiated properly.");
                    isUnityObject = false;
                }
            }

            DynamicMethod dynMethod = new DynamicMethod("<DS_UIEXT>_CreateExtensionImpl",
                MethodAttributes.Static | MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName, CallingConventions.Standard, typeof(object),
                [ typeof(IUIExtensionManager), typeof(object) ], info.ImplementationType, true);
            dynMethod.DefineParameter(0, ParameterAttributes.None, "extensionManager");
            dynMethod.DefineParameter(1, ParameterAttributes.None, "uiInstance");
            ILGenerator il = dynMethod.GetILGenerator();

            il.DeclareLocal(info.ImplementationType); // 0
            il.DeclareLocal(info.ParentType); // 1

            Label useProvidedLocal1 = il.DefineLabel();
            Label setLocal1 = il.DefineLabel();
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Brtrue_S, useProvidedLocal1);
            if (!staticUI)
            {
                try
                {
                    UIAccessor.LoadUIToILGenerator(il, info.ParentType);
                }
                catch (InvalidOperationException)
                {
                    il.Emit(OpCodes.Ldnull);
                }
            }
            else
                il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Br, setLocal1);
            il.MarkLabel(useProvidedLocal1);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Castclass, info.ParentType);
            il.MarkLabel(setLocal1);
            il.Emit(OpCodes.Stloc_1);
            Label? skipLbl = null;
            Label? fallbackLbl = null;
            if (isUnityObject)
            {
                skipLbl = il.DefineLabel();
                fallbackLbl = il.DefineLabel();
                Label isGameObjectLbl = il.DefineLabel();

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Isinst, typeof(Component));
                il.Emit(OpCodes.Dup);

                il.Emit(OpCodes.Brtrue, isGameObjectLbl);
                il.Emit(OpCodes.Pop);
                il.Emit(OpCodes.Br, fallbackLbl.Value);

                il.MarkLabel(isGameObjectLbl);
                isGameObjectLbl = il.DefineLabel();

                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Ldnull);
                il.Emit(OpCodes.Call, equalsMethod!);
                il.Emit(OpCodes.Brfalse, isGameObjectLbl);
                il.Emit(OpCodes.Pop);
                il.Emit(OpCodes.Br, fallbackLbl.Value);

                il.MarkLabel(isGameObjectLbl);
                il.Emit(OpCodes.Call, getGameObjectProperty!);
                il.Emit(OpCodes.Ldtoken, info.ImplementationType);
                il.Emit(getTypeFromHandle.GetCallRuntime(), getTypeFromHandle);
                il.Emit(OpCodes.Call, addComponentMethod!);
                il.Emit(OpCodes.Br, skipLbl.Value);
            }

            if (fallbackLbl.HasValue)
                il.MarkLabel(fallbackLbl.Value);

            il.Emit(OpCodes.Ldtoken, info.ImplementationType);
            il.Emit(getTypeFromHandle.GetCallRuntime(), getTypeFromHandle);
            il.Emit(getUninitObject.GetCallRuntime(), getUninitObject);
            il.Emit(OpCodes.Castclass, info.ImplementationType);

            if (skipLbl.HasValue)
                il.MarkLabel(skipLbl.Value);

            il.Emit(OpCodes.Stloc_0);

            if (typeof(UIExtension).IsAssignableFrom(info.ImplementationType))
            {
                MethodInfo? setter = typeof(UIExtension).GetProperty(nameof(UIExtension.Instance), BindingFlags.Public | BindingFlags.Instance)?.GetSetMethod(true);
                if (setter != null)
                {
                    il.Emit(OpCodes.Ldloc_0);
                    il.Emit(OpCodes.Ldloc_1);
                    il.Emit(OpCodes.Call, setter);
                }
                setter = typeof(UIExtension).GetProperty(nameof(UIExtension.Manager), BindingFlags.Public | BindingFlags.Instance)?.GetSetMethod(true);
                if (setter != null)
                {
                    il.Emit(OpCodes.Ldloc_0);
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Call, setter);
                }
            }

            foreach (UIExistingMemberInfo member in info.ExistingMembers)
            {
                if (!member.IsInitialized) continue;
                if (!member.Member.GetIsStatic())
                    il.Emit(OpCodes.Ldloc_0);

                if (!member.ExistingIsStatic)
                    il.Emit(OpCodes.Ldloc_1);

                member.EmitApply(il);
            }

            if (!isUnityObject)
            {
                il.Emit(OpCodes.Ldloc_0);
                if (constructor!.GetParameters().Length == 1)
                    il.Emit(OpCodes.Ldloc_1);
                il.Emit(OpCodes.Call, constructor);
            }

            if (typeof(IUIExtensionReadyListener).IsAssignableFrom(info.ImplementationType))
            {
                MethodInfo? onReadyMethod = typeof(IUIExtensionReadyListener).GetMethod(nameof(IUIExtensionReadyListener.OnReady), BindingFlags.Public | BindingFlags.Instance);
                if (onReadyMethod != null)
                    onReadyMethod = Accessor.GetImplementedMethod(info.ImplementationType, onReadyMethod);

                if (onReadyMethod == null)
                {
                    LogWarning($"Unable to find implemented OnReady method of IUIExtensionReadyListener for {info.ImplementationType.Name}.");
                }
                else
                {
                    il.Emit(OpCodes.Ldloc_0);
                    il.Emit(OpCodes.Ldloc_1);
                    il.Emit(OpCodes.Callvirt, onReadyMethod);
                }
            }

            if (!AddInstanceMethod.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, typeof(UIExtensionManager));
            }
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(GetType() == typeof(UIExtensionManager) ? OpCodes.Call : OpCodes.Callvirt, AddInstanceMethod);

            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);

            info.CreateCallback = (CreateUIExtension)dynMethod.CreateDelegate(typeof(CreateUIExtension));
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to create instantiation method for UI extension: {info.ImplementationType.Name}.", ex);
        }

        try
        {
            InitializeParentPatches(info);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to patch for UI parent: {info.ParentType.Name} (while patching {info.ImplementationType.Name}).", ex);
        }

        AddToList(info);

        try
        {
            InitializeExtensionPatches(info);
        }
        catch (Exception ex)
        {
            try
            {
                if (ExtensionsDictIntl.TryGetValue(info.ImplementationType, out UIExtensionInfo extInfo))
                {
                    for (int i = 0; i < extInfo.InstantiationsIntl.Count; ++i)
                    {
                        object instance = extInfo.InstantiationsIntl[i];
                        if (instance is IDisposable disposable)
                        {
                            try
                            {
                                disposable.Dispose();
                            }
                            catch (Exception ex2)
                            {
                                LogWarning($"Error disposing UI extension: {extInfo.ImplementationType.Name}.", extInfo.Module, extInfo.Assembly);
                                CommandWindow.LogWarning(ex2);
                            }
                        }

                        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                        if (instance is UnityEngine.Object obj && obj != null)
                        {
                            UnityEngine.Object.Destroy(obj);

                            if (DebugLogging)
                                LogDebug($"  * Destroyed: {extInfo.ImplementationType.Name}.", extInfo.Module, extInfo.Assembly);
                        }

                        _extensions.Remove(extInfo);

                        if (ParentTypeInfo.TryGetValue(extInfo.ParentType, out UIExtensionParentTypeInfo parentInfo))
                            parentInfo.InstancesIntl.RemoveAll(x => x.Instance == instance);
                    }

                    if (DebugLogging)
                        LogDebug($"Deregistered UI extension: {info.ImplementationType.Name}.", extInfo.Module, extInfo.Assembly);
                }
            }
            catch (Exception)
            {
                // ignored
            }

            throw new Exception($"Failed to patch for UI extension: {info.ImplementationType.Name}.", ex);
        }
    }
    private static readonly MethodInfo AddInstanceMethod = typeof(UIExtensionManager).GetMethod(nameof(AddInstance), BindingFlags.NonPublic | BindingFlags.Instance)!;

    /// <summary>
    /// Register an extension instance in the list.
    /// </summary>
    [UsedImplicitly]
    protected virtual void AddInstance(object? uiInstance, object instance)
    {
        UIExtensionInfo? extInfo = null;
        Type? type = instance.GetType();
        type.ForEachBaseType((type, _) => !ExtensionsDictIntl.TryGetValue(type, out extInfo));
        type = extInfo?.ParentType;
        if (uiInstance != null && type != uiInstance.GetType() && type != null)
        {
            LogWarning($"Extension type does not match parent type: {type.Name} vs {instance.GetType().Name}.");
            return;
        }

        UIExtensionParentTypeInfo? parentInfo = null;
        type?.ForEachBaseType((type, _) => !ParentTypeInfoIntl.TryGetValue(type, out parentInfo));

        if (extInfo == null)
        {
            LogWarning($"Failed to find extension info for extension: {instance.GetType().Name}.");
            return;
        }
        if (parentInfo == null)
        {
            LogWarning($"Failed to find parent info for extension: {instance.GetType().Name}.", extInfo?.Module, extInfo?.Assembly);
            return;
        }

        UIExtensionVanillaInstanceInfo? info = null;
        List<UIExtensionVanillaInstanceInfo> vanillaInstances = parentInfo.VanillaInstancesIntl;

        if (vanillaInstances.Count != 0 && (parentInfo.ParentTypeInfo.IsInstanceUI || parentInfo.ParentTypeInfo.IsStaticUI) && (vanillaInstances.Count != 1 || !ReferenceEquals(vanillaInstances[0].Instance, instance)))
        {
            vanillaInstances.Clear();
            info = new UIExtensionVanillaInstanceInfo(parentInfo.ParentTypeInfo.IsStaticUI ? null : uiInstance, parentInfo.ParentTypeInfo.OpenOnInitialize || parentInfo.ParentTypeInfo.DefaultOpenState);
            vanillaInstances.Add(info);
            if (DebugLogging)
                LogDebug($"Replaced vanilla instance info: {parentInfo.ParentType.Name}.");
        }
        else
        {
            for (int i = 0; i < vanillaInstances.Count; ++i)
            {
                if (ReferenceEquals(vanillaInstances[i].Instance, uiInstance))
                {
                    info = vanillaInstances[i];
                    if (DebugLogging)
                        LogDebug($"Found vanilla instance info: {parentInfo.ParentType.Name}.");
                    break;
                }
            }

            if (info == null)
            {
                info = new UIExtensionVanillaInstanceInfo(parentInfo.ParentTypeInfo.IsStaticUI ? null : uiInstance, parentInfo.ParentTypeInfo.OpenOnInitialize || parentInfo.ParentTypeInfo.DefaultOpenState);
                vanillaInstances.Add(info);
                if (DebugLogging)
                    LogDebug($"Added vanilla instance info: {parentInfo.ParentType.Name}.");
            }
        }


        parentInfo.InstancesIntl.Add(new UIExtensionInstanceInfo(instance, info, extInfo));
        extInfo.InstantiationsIntl.Add(instance);
        if (instance is IUnpatchableExtension unpatchable)
        {
            Type instanceType = unpatchable.GetType();
            _pendingUnpatches.RemoveAll(x => x.GetType() == instanceType);
        }
    }

    /// <summary>
    /// Create the extension object. Calls <see cref="UIExtensionInfo.CreateCallback"/>.
    /// </summary>
    protected virtual object? CreateExtension(UIExtensionInfo info, object? uiInstance)
    {
        try
        {
            object? instance;
            lock (this)
            {
                instance = info.CreateCallback(this, uiInstance);

                if (instance == null)
                {
                    LogWarning($"Failed to create extension of type {info.ImplementationType.Name}.", info.Module, info.Assembly);
                    return null;
                }

                if (DebugLogging)
                    LogDebug($"Created {info.ImplementationType.Name} for {info.ParentType.Name}.", info.Module, info.Assembly);
            }

            OnAdd?.Invoke(instance);

            return instance;
        }
        catch (Exception ex)
        {
            LogError($"Error initializing {info.ImplementationType.Name}.", info.Module, info.Assembly);
            CommandWindow.LogError(ex);
            return null;
        }
    }

    /// <summary>
    /// Initialize existing members in the extension.
    /// </summary>
    protected virtual void TryInitializeMember(UIExtensionInfo info, MemberInfo member)
    {
        if (Attribute.GetCustomAttribute(member, typeof(ExistingMemberAttribute)) is not ExistingMemberAttribute existingMemberAttribute)
            return;

        ExistingMemberFailureBehavior failureMode = existingMemberAttribute.FailureBehavior;

        FieldInfo? field = member as FieldInfo;
        PropertyInfo? property = member as PropertyInfo;
        MethodInfo? method = member as MethodInfo;

        if (field == null && property == null && method == null)
            return;

        bool isStatic = member.GetIsStatic();
        if (isStatic)
            throw new Exception($"UI extensions should not have static existing members, such as \"{member.Name}\".");

        Type owningType = existingMemberAttribute.OwningType ?? info.ParentType;

        FieldInfo? existingField = owningType.GetField(existingMemberAttribute.MemberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
        PropertyInfo? existingProperty = owningType.GetProperty(existingMemberAttribute.MemberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
        MethodInfo? existingMethod = owningType.GetMethod(existingMemberAttribute.MemberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy,
            null, CallingConventions.Any, Array.Empty<Type>(), null);

        if (existingField == null &&
            (existingProperty == null ||
             existingProperty.GetSetMethod(true) == null && existingProperty.GetGetMethod(true) == null ||
             existingProperty.GetIndexParameters().Length > 0) &&
            (existingMethod == null || existingMethod.ReturnType == typeof(void) || existingMethod.GetParameters().Length > 0))
        {
            string msg = $"Unable to match \"{owningType.Name}.{existingMemberAttribute.MemberName}\" to a field, get-able property, or no-argument, non-void-returning method.";
            if (failureMode != ExistingMemberFailureBehavior.Ignore)
                throw new MemberAccessException(msg);

            LogInfo(msg, info.Module, info.Assembly);
            return;
        }
        MemberInfo existingMember = ((MemberInfo?)existingField ?? existingProperty) ?? existingMethod!;

        Type existingMemberType = existingMember.GetMemberType()!;
        Type memberType = member.GetMemberType()!;

        if (method != null)
        {
            ParameterInfo[] parameters = method.GetParameters();
            if (method.ReturnType == typeof(void))
            {
                // setter
                if (parameters.Length == 0 || !parameters[0].ParameterType.IsAssignableFrom(existingMemberType))
                {
                    string msg = $"Unable to assign existing method parameter type: \"{existingMemberType.FullName}\" to expected member type: \"{parameters[0].ParameterType.FullName}\" for existing member \"{member.Name}\".";
                    if (failureMode != ExistingMemberFailureBehavior.Ignore)
                        throw new Exception(msg);

                    LogInfo(msg, info.Module, info.Assembly);
                    return;
                }
            }
            else
            {
                // getter
                if (parameters.Length != 0 || !memberType.IsAssignableFrom(existingMemberType))
                {
                    string msg = $"Unable to assign existing method parameter type: \"{existingMemberType.FullName}\" to expected member type: \"{memberType.FullName}\" for existing member \"{member.Name}\".";
                    if (failureMode != ExistingMemberFailureBehavior.Ignore)
                        throw new Exception(msg);

                    LogInfo(msg, info.Module, info.Assembly);
                    return;
                }
            }
        }
        else if (!memberType.IsAssignableFrom(existingMemberType))
        {
            string msg = $"Unable to assign existing member type: \"{existingMemberType.FullName}\" to expected member type: \"{memberType.FullName}\" for existing member \"{member.Name}\".";
            if (failureMode != ExistingMemberFailureBehavior.Ignore)
                throw new Exception(msg);

            LogInfo(msg, info.Module, info.Assembly);
            return;
        }

        bool existingIsStatic = existingMember.GetIsStatic();

        if (!existingIsStatic && info.TypeInfo.IsStaticUI)
        {
            string msg = $"Requested instance variable ({existingMember.Name}) from static UI: {info.ParentType.Name} for existing member \"{member.Name}\".";
            if (failureMode != ExistingMemberFailureBehavior.Ignore)
                throw new InvalidOperationException(msg);

            LogInfo(msg, info.Module, info.Assembly);
            return;
        }

        bool initialized;
        if (existingMemberAttribute.InitializeMode is ExistingMemberInitializeMode.InitializeOnConstruct or ExistingMemberInitializeMode.PatchGetter)
        {
            initialized = existingMemberAttribute.InitializeMode == ExistingMemberInitializeMode.InitializeOnConstruct;
            if (!initialized && field != null)
            {
                LogWarning($"Fields can not be non-initialized (as indicated for {field.Name}).", info.Module, info.Assembly);
                initialized = true;
            }
            if (initialized && property != null && property.GetSetMethod(true) == null)
            {
                LogWarning($"Properties without a setter can not be initialized (as indicated for {property.Name}).", info.Module, info.Assembly);
                initialized = false;
            }
            if (!initialized && method != null && method.ReturnType == typeof(void))
            {
                LogWarning($"Void-returning methods can not be non-initialized (as indicated for {method.Name}).", info.Module, info.Assembly);
                initialized = true;
            }
            if (initialized && method != null && method.GetParameters().Length == 0)
            {
                LogWarning($"Parameterless methods can not be initialized (as indicated for {method.Name}).", info.Module, info.Assembly);
                initialized = false;
            }
            if (DebugLogging)
                LogDebug($"Set initialized setting for existing member: {member.Name}: {initialized}.", info.Module, info.Assembly);
        }
        else
        {
            if (field != null)
                initialized = true;
            else if (property != null)
                initialized = property.GetSetMethod(true) != null;
            else if (method!.ReturnType == typeof(void))
                initialized = true;
            else
                initialized = false;
            if (DebugLogging)
                LogDebug($"Assumed initialized setting for existing member: {member.Name}: {initialized}.", info.Module, info.Assembly);
        }

        if (!initialized && existingProperty != null && existingProperty.GetSetMethod(true) != null)
            LogWarning($"Setter on {existingProperty.Name} can not be used to set the original value. Recommended to make the property get-only (readonly).", info.Module, info.Assembly);

        info.ExistingMembersIntl.Add(new UIExistingMemberInfo(member, existingMember, existingIsStatic, initialized));
    }

    /// <summary>
    /// Initialize existing member patches in the extension.
    /// </summary>
    protected virtual void InitializeExtensionPatches(UIExtensionInfo info)
    {
        for (int i = 0; i < info.ExistingMembersIntl.Count; ++i)
        {
            UIExistingMemberInfo member = info.ExistingMembersIntl[i];
            if (member.IsInitialized || member.Member is not PropertyInfo property)
            {
                if (DebugLogging)
                    LogDebug($"Skipping initialized member: {member.Member.Name}.", info.Module, info.Assembly);
                continue;
            }
            MethodInfo? getter = property.GetGetMethod(true);
            MethodInfo? setter = property.GetSetMethod(true);
            if (getter == null)
            {
                LogWarning($"Unable to find getter for {property.Name}.", info.Module, info.Assembly);
                continue;
            }
            if (DebugLogging && setter == null)
                LogDebug($"Unable to find setter for {property.Name}, not an issue.", info.Module, info.Assembly);
            if (Patches.ContainsKey(getter))
            {
                if (DebugLogging)
                    LogDebug($"{getter.Name} has already been transpiled.", info.Module, info.Assembly);
            }
            else
            {
                MethodInfo transpiler = TranspileGetterPropertyMethod;
                UIExtensionExistingMemberPatchInfo patchInfo = new UIExtensionExistingMemberPatchInfo(info, transpiler, member);
                PatchInfo[getter] = patchInfo;
                UIAccessor.Patcher.Patch(getter, transpiler: new HarmonyMethod(transpiler));
                Patches.Add(getter, new UIExtensionPatch(getter, transpiler, HarmonyPatchType.Transpiler));
            }

            if (setter != null)
            {
                if (Patches.ContainsKey(setter))
                {
                    if (DebugLogging)
                        LogDebug($"{setter.Name} has already been transpiled.", info.Module, info.Assembly);
                }
                else
                {
                    MethodInfo transpiler = TranspileSetterPropertyMethod;
                    UIExtensionExistingMemberPatchInfo patchInfo = new UIExtensionExistingMemberPatchInfo(info, transpiler, member);
                    PatchInfo[setter] = patchInfo;
                    UIAccessor.Patcher.Patch(setter, transpiler: new HarmonyMethod(transpiler));
                    Patches.Add(setter, new UIExtensionPatch(setter, transpiler, HarmonyPatchType.Transpiler));
                }
            }
        }
    }

    /// <summary>
    /// Get or add <see cref="UIExtensionParentTypeInfo"/> for a UI type.
    /// </summary>
    protected UIExtensionParentTypeInfo GetOrAddParentTypeInfo(Type parentType, UITypeInfo typeInfo)
    {
        if (!ParentTypeInfoIntl.TryGetValue(parentType, out UIExtensionParentTypeInfo parentTypeInfo))
        {
            parentTypeInfo = new UIExtensionParentTypeInfo(parentType, typeInfo);
            ParentTypeInfoIntl.Add(parentType, parentTypeInfo);
        }
        return parentTypeInfo;
    }

    /// <summary>
    /// Patch methods for the UI parent type.
    /// </summary>
    protected void InitializeParentPatches(UIExtensionInfo info)
    {
        UIExtensionParentTypeInfo parentTypeInfo = GetOrAddParentTypeInfo(info.ParentType, info.TypeInfo);

        PatchParentOnOpen(info.TypeInfo, parentTypeInfo, info.Module, info.Assembly);
        PatchParentOnClose(info.TypeInfo, parentTypeInfo, info.Module, info.Assembly);
        PatchParentOnInitialize(info.TypeInfo, parentTypeInfo, info.Module, info.Assembly);
        PatchParentOnDestroy(info.TypeInfo, parentTypeInfo, info.Module, info.Assembly);
    }

    /// <summary>
    /// Patch OnOpen methods for the UI parent type.
    /// </summary>
    protected virtual void PatchParentOnOpen(UITypeInfo typeInfo, UIExtensionParentTypeInfo parentTypeInfo, Module? module, Assembly assembly)
    {
        if (typeInfo.CustomOnOpen != null)
        {
            if (!typeInfo.CustomOnOpen.HasBeenInitialized)
            {
                typeInfo.CustomOnOpen.Patch(UIAccessor.Patcher);
                typeInfo.CustomOnOpen.HasBeenInitialized = true;
            }
            if (!typeInfo.CustomOnOpen.HasOnOpenBeenInitialized)
            {
                typeInfo.CustomOnOpen.OnOpened += OnOpened;
                typeInfo.CustomOnOpen.HasOnOpenBeenInitialized = true;
            }
        }
        else if (!typeInfo.OpenOnInitialize)
        {
            foreach (UIVisibilityMethodInfo openMethod in typeInfo.OpenMethods)
            {
                if (parentTypeInfo.OpenPatchesIntl.Any(x => x.Original == openMethod.Method))
                {
                    if (DebugLogging)
                        LogDebug($"Skipped finalizer for {openMethod.Method.Name}, already done from this extension.", module, assembly);
                    continue;
                }

                if (Patches.TryGetValue(openMethod.Method, out UIExtensionPatch patchInfo))
                {
                    if (DebugLogging)
                        LogDebug($"Skipped finalizer for {openMethod.Method.Name}, already done from another extension.", module, assembly);
                    parentTypeInfo.OpenPatchesIntl.Add(patchInfo);
                    continue;
                }

                if (openMethod.Method.DeclaringType == typeInfo.Type)
                {
                    MethodInfo finalizer = openMethod.IsStatic
                        ? StaticOpenMethodFinalizerMethod
                        : InstanceOpenMethodFinalizerMethod;
                    UIAccessor.Patcher.Patch(openMethod.Method, finalizer: new HarmonyMethod(finalizer));
                    patchInfo = new UIExtensionPatch(openMethod.Method, finalizer, HarmonyPatchType.Finalizer);
                    parentTypeInfo.OpenPatchesIntl.Add(patchInfo);
                    Patches.Add(openMethod.Method, patchInfo);
                    if (DebugLogging)
                        LogDebug($"Added finalizer for {openMethod.Method.Name}: {finalizer.Name}.", module, assembly);
                }
                else if (openMethod.Method.IsStatic)
                {
                    throw new InvalidOperationException("Can't patch an open method from another class.");
                }
            }
        }
    }

    /// <summary>
    /// Patch OnClose methods for the UI parent type.
    /// </summary>
    protected virtual void PatchParentOnClose(UITypeInfo typeInfo, UIExtensionParentTypeInfo parentTypeInfo, Module? module, Assembly assembly)
    {
        if (typeInfo.CustomOnClose != null)
        {
            if (!typeInfo.CustomOnClose.HasBeenInitialized)
            {
                typeInfo.CustomOnClose.Patch(UIAccessor.Patcher);
                typeInfo.CustomOnClose.HasBeenInitialized = true;
            }
            if (!typeInfo.CustomOnClose.HasOnCloseBeenInitialized)
            {
                typeInfo.CustomOnClose.OnClosed += OnClosed;
                typeInfo.CustomOnClose.HasOnCloseBeenInitialized = true;
            }
        }
        else if (!typeInfo.CloseOnDestroy)
        {
            foreach (UIVisibilityMethodInfo closeMethod in typeInfo.CloseMethods)
            {
                if (parentTypeInfo.ClosePatchesIntl.Any(x => x.Original == closeMethod.Method))
                {
                    if (DebugLogging)
                        LogDebug($"Skipped finalizer for {closeMethod.Method.Name}, already done from this extension.", module, assembly);
                    continue;
                }

                if (Patches.TryGetValue(closeMethod.Method, out UIExtensionPatch patchInfo))
                {
                    if (DebugLogging)
                        LogDebug($"Skipped finalizer for {closeMethod.Method.Name}, already done from another extension.", module, assembly);
                    parentTypeInfo.ClosePatchesIntl.Add(patchInfo);
                    continue;
                }

                if (closeMethod.Method.DeclaringType == typeInfo.Type)
                {
                    MethodInfo finalizer = closeMethod.IsStatic
                        ? StaticCloseMethodFinalizerMethod
                        : InstanceCloseMethodFinalizerMethod;
                    UIAccessor.Patcher.Patch(closeMethod.Method, finalizer: new HarmonyMethod(finalizer));
                    patchInfo = new UIExtensionPatch(closeMethod.Method, finalizer, HarmonyPatchType.Finalizer);
                    parentTypeInfo.ClosePatchesIntl.Add(patchInfo);
                    Patches.Add(closeMethod.Method, patchInfo);
                    if (DebugLogging)
                        LogDebug($"Added finalizer for {closeMethod.Method.Name}: {finalizer.Name}.", module, assembly);
                }
                else if (closeMethod.Method.IsStatic)
                {
                    throw new InvalidOperationException("Can't patch a close method from another class.");
                }
            }
        }
    }

    /// <summary>
    /// Patch OnInitialize methods for the UI parent type.
    /// </summary>
    protected virtual void PatchParentOnInitialize(UITypeInfo typeInfo, UIExtensionParentTypeInfo parentTypeInfo, Module? module, Assembly assembly)
    {
        if (typeInfo.CustomOnInitialize != null)
        {
            if (!typeInfo.CustomOnInitialize.HasBeenInitialized)
            {
                typeInfo.CustomOnInitialize.Patch(UIAccessor.Patcher);
                typeInfo.CustomOnInitialize.HasBeenInitialized = true;
            }
            if (!typeInfo.CustomOnInitialize.HasOnInitializeBeenInitialized)
            {
                typeInfo.CustomOnInitialize.OnInitialized += OnInitialized;
                typeInfo.CustomOnInitialize.HasOnInitializeBeenInitialized = true;
            }
        }
        else
        {
            foreach (UIVisibilityMethodInfo initializeMethod in typeInfo.InitializeMethods)
            {
                if (parentTypeInfo.InitializePatchesIntl.Any(x => x.Original == initializeMethod.Method))
                {
                    if (DebugLogging)
                        LogDebug($"Skipped finalizer for {initializeMethod.Method.Name}, already done from this extension.", module, assembly);
                    continue;
                }

                if (Patches.TryGetValue(initializeMethod.Method, out UIExtensionPatch patchInfo))
                {
                    if (DebugLogging)
                        LogDebug($"Skipped finalizer for {initializeMethod.Method.Name}, already done from another extension.", module, assembly);
                    parentTypeInfo.InitializePatchesIntl.Add(patchInfo);
                    continue;
                }

                if (initializeMethod.Method.DeclaringType == typeInfo.Type)
                {
                    MethodInfo finalizer = InstanceInitializeMethodFinalizerMethod;
                    UIAccessor.Patcher.Patch(initializeMethod.Method, finalizer: new HarmonyMethod(finalizer));
                    patchInfo = new UIExtensionPatch(initializeMethod.Method, finalizer, HarmonyPatchType.Finalizer);
                    parentTypeInfo.InitializePatchesIntl.Add(patchInfo);
                    Patches.Add(initializeMethod.Method, patchInfo);
                    if (DebugLogging)
                        LogDebug($"Added finalizer for {initializeMethod.Method.Name}: {finalizer.Name}.", module, assembly);
                }
                else if (initializeMethod.Method.IsStatic)
                {
                    throw new InvalidOperationException("Can't patch a initialize method from another class.");
                }
            }
        }
    }

    /// <summary>
    /// Patch OnDestroy methods for the UI parent type.
    /// </summary>
    protected virtual void PatchParentOnDestroy(UITypeInfo typeInfo, UIExtensionParentTypeInfo parentTypeInfo, Module? module, Assembly assembly)
    {
        if (typeInfo.DestroyWhenParentDestroys && typeInfo.Parent != null && UIAccessor.TryGetUITypeInfo(typeInfo.Parent, out UITypeInfo parentUITypeInfo))
            PatchParentOnDestroy(parentUITypeInfo, GetOrAddParentTypeInfo(parentUITypeInfo.Type, typeInfo), module, assembly);

        if (typeInfo.CustomOnDestroy != null)
        {
            if (!typeInfo.CustomOnDestroy.HasBeenInitialized)
            {
                typeInfo.CustomOnDestroy.Patch(UIAccessor.Patcher);
                typeInfo.CustomOnDestroy.HasBeenInitialized = true;
            }
            if (!typeInfo.CustomOnDestroy.HasOnDestroyBeenInitialized)
            {
                typeInfo.CustomOnDestroy.OnDestroyed += OnDestroy;
                typeInfo.CustomOnDestroy.HasOnDestroyBeenInitialized = true;
            }
        }
        else if (typeInfo is not { DestroyOnClose: true, CloseOnDestroy: false })
        {
            foreach (UIVisibilityMethodInfo destroyMethod in typeInfo.DestroyMethods)
            {
                if (parentTypeInfo.DestroyPatchesIntl.Any(x => x.Original == destroyMethod.Method))
                {
                    if (DebugLogging)
                        LogDebug($"[{Source}] Skipped finalizer for {destroyMethod.Method.Name}, already done from this extension.", module, assembly);
                    continue;
                }

                if (Patches.TryGetValue(destroyMethod.Method, out UIExtensionPatch patchInfo))
                {
                    if (DebugLogging)
                        LogDebug($"[{Source}] Skipped finalizer for {destroyMethod.Method.Name}, already done from another extension.", module, assembly);
                    parentTypeInfo.DestroyPatchesIntl.Add(patchInfo);
                    continue;
                }

                if (destroyMethod.Method.DeclaringType == typeInfo.Type)
                {
                    if (destroyMethod.Method.GetParameters().Length == 0)
                    {
                        MethodInfo prefix = destroyMethod.IsStatic
                            ? StaticDestroyMethodPrefixMethod
                            : InstanceDestroyMethodPrefixMethod;
                        UIAccessor.Patcher.Patch(destroyMethod.Method, prefix: new HarmonyMethod(prefix));
                        patchInfo = new UIExtensionPatch(destroyMethod.Method, prefix, HarmonyPatchType.Prefix);
                        if (DebugLogging)
                            LogDebug($"[{Source}] Added prefix for {destroyMethod.Method.Name}: {prefix.Name}.", module, assembly);
                    }
                    else
                    {
                        MethodInfo finalizer = destroyMethod.IsStatic
                            ? StaticDestroyMethodFinalizerMethod
                            : InstanceDestroyMethodFinalizerMethod;
                        UIAccessor.Patcher.Patch(destroyMethod.Method, finalizer: new HarmonyMethod(finalizer));
                        patchInfo = new UIExtensionPatch(destroyMethod.Method, finalizer, HarmonyPatchType.Finalizer);
                        if (DebugLogging)
                            LogDebug($"[{Source}] Added finalizer for {destroyMethod.Method.Name}: {finalizer.Name}.", module, assembly);
                    }
                    parentTypeInfo.DestroyPatchesIntl.Add(patchInfo);
                    Patches.Add(destroyMethod.Method, patchInfo);
                }
                else if (destroyMethod.Method.IsStatic)
                {
                    throw new InvalidOperationException("Can't patch a destroy method from another class.");
                }
            }
        }
    }

    private static readonly MethodInfo StaticOpenMethodFinalizerMethod = typeof(UIExtensionManager).GetMethod(nameof(StaticOpenMethodFinalizer), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static void StaticOpenMethodFinalizer(MethodBase __originalMethod, bool __runOriginal, Exception? __exception)
    {
        if (!__runOriginal || __exception != null)
            return;

        (UnturnedUIToolsNexus.UIExtensionManager as UIExtensionManager)?.OnOpened(__originalMethod.DeclaringType!, null);
    }

    private static readonly MethodInfo StaticCloseMethodFinalizerMethod = typeof(UIExtensionManager).GetMethod(nameof(StaticCloseMethodFinalizer), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static void StaticCloseMethodFinalizer(MethodBase __originalMethod, bool __runOriginal, Exception? __exception)
    {
        if (!__runOriginal || __exception != null)
            return;

        (UnturnedUIToolsNexus.UIExtensionManager as UIExtensionManager)?.OnClosed(__originalMethod.DeclaringType!, null);
    }

    private static readonly MethodInfo StaticDestroyMethodPrefixMethod = typeof(UIExtensionManager).GetMethod(nameof(StaticDestroyMethodPrefix), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static void StaticDestroyMethodPrefix(MethodBase __originalMethod, bool __runOriginal)
    {
        if (!__runOriginal)
            return;

        (UnturnedUIToolsNexus.UIExtensionManager as UIExtensionManager)?.OnDestroy(__originalMethod.DeclaringType!, null);
    }

    private static readonly MethodInfo StaticDestroyMethodFinalizerMethod = typeof(UIExtensionManager).GetMethod(nameof(StaticDestroyMethodFinalizer), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static void StaticDestroyMethodFinalizer(MethodBase __originalMethod, bool __runOriginal, Exception? __exception)
    {
        if (!__runOriginal || __exception != null)
            return;

        (UnturnedUIToolsNexus.UIExtensionManager as UIExtensionManager)?.OnDestroy(__originalMethod.DeclaringType!, null);
    }

    private static readonly MethodInfo InstanceOpenMethodFinalizerMethod = typeof(UIExtensionManager).GetMethod(nameof(InstanceOpenMethodFinalizer), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static void InstanceOpenMethodFinalizer(object __instance, bool __runOriginal, Exception? __exception)
    {
        if (!__runOriginal || __exception != null)
            return;

        (UnturnedUIToolsNexus.UIExtensionManager as UIExtensionManager)?.OnOpened(null, __instance);
    }

    private static readonly MethodInfo InstanceCloseMethodFinalizerMethod = typeof(UIExtensionManager).GetMethod(nameof(InstanceCloseMethodFinalizer), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static void InstanceCloseMethodFinalizer(object __instance, bool __runOriginal, Exception? __exception)
    {
        if (!__runOriginal || __exception != null)
            return;

        (UnturnedUIToolsNexus.UIExtensionManager as UIExtensionManager)?.OnClosed(null, __instance);
    }


    private static readonly MethodInfo InstanceDestroyMethodFinalizerMethod = typeof(UIExtensionManager).GetMethod(nameof(InstanceDestroyMethodFinalizer), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static void InstanceDestroyMethodFinalizer(object __instance, bool __runOriginal, Exception? __exception)
    {
        if (!__runOriginal || __exception != null)
            return;

        (UnturnedUIToolsNexus.UIExtensionManager as UIExtensionManager)?.OnDestroy(null, __instance);
    }

    private static readonly MethodInfo InstanceDestroyMethodPrefixMethod = typeof(UIExtensionManager).GetMethod(nameof(InstanceDestroyMethodPrefix), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static void InstanceDestroyMethodPrefix(object __instance, bool __runOriginal)
    {
        if (!__runOriginal)
            return;
        (UnturnedUIToolsNexus.UIExtensionManager as UIExtensionManager)?.OnDestroy(null, __instance);
    }

    private static readonly MethodInfo InstanceInitializeMethodFinalizerMethod = typeof(UIExtensionManager).GetMethod(nameof(InstanceInitializeMethodFinalizer), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static void InstanceInitializeMethodFinalizer(MethodBase __originalMethod, object __instance, bool __runOriginal, Exception? __exception)
    {
        if (!__runOriginal || __exception != null || __originalMethod.DeclaringType != __instance.GetType())
            return;

        (UnturnedUIToolsNexus.UIExtensionManager as UIExtensionManager)?.OnInitialized(null, __instance);
    }

    private static readonly MethodInfo TranspileGetterPropertyMethod = typeof(UIExtensionManager).GetMethod(nameof(TranspileGetterProperty), BindingFlags.NonPublic | BindingFlags.Static)!;

    [UsedImplicitly]
    private static IEnumerable<CodeInstruction> TranspileGetterProperty(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase method)
    {
        if (UnturnedUIToolsNexus.UIExtensionManager is not UIExtensionManager mngr)
            throw new Exception("Can't patch with different IUIExtensionManager inplementation.");

        Type declType = method.DeclaringType!;
        if (!mngr.PatchInfo.TryGetValue(method, out UIExtensionExistingMemberPatchInfo info))
        {
            mngr.LogWarning($"Unable to patch {method.Name}: Could not find existing member info for {declType.Name}.", null, method.DeclaringType?.Assembly);
            foreach (CodeInstruction instr in EmitUtility.Throw<InvalidOperationException>($"Could not find existing member info for {declType.Name}."))
                yield return instr;

            yield break;
        }

        if (!info.MemberInfo.ExistingIsStatic)
        {
            if (typeof(UIExtension).IsAssignableFrom(info.Extension.ImplementationType) && typeof(UIExtension).GetProperty(nameof(UIExtension.Instance), BindingFlags.Instance | BindingFlags.Public)?.GetGetMethod(true) is { } prop)
            {
                // this.Instance
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Call, prop);
                yield return new CodeInstruction(OpCodes.Castclass, info.Extension.ParentType);
            }
            else
            {
                foreach (CodeInstruction instr in UIAccessor.EnumerateUIInstructions(info.Extension.ParentType, generator))
                    yield return instr;
            }
        }

        foreach (CodeInstruction instr in info.MemberInfo.EnumerateApply(true))
            yield return instr;

        yield return new CodeInstruction(OpCodes.Ret);
        
        if (mngr.DebugLogging)
            mngr.LogDebug($"Transpiled {method.Name} for extension for {info.Extension.ParentType.Name}.", info.Extension.Module, info.Extension.Assembly);
    }
    private static readonly MethodInfo TranspileSetterPropertyMethod = typeof(UIExtensionManager).GetMethod(nameof(TranspileSetterProperty), BindingFlags.NonPublic | BindingFlags.Static)!;

    [UsedImplicitly]
    private static IEnumerable<CodeInstruction> TranspileSetterProperty(IEnumerable<CodeInstruction> instructions, MethodBase method) => EmitUtility.Throw<NotImplementedException>($"{method.Name.Replace("set_", "")} can not have a setter, as it is a UI extension implementation.");
    
    /// <summary>
    /// Represents a patch for an existing member.
    /// </summary>
    protected class UIExtensionExistingMemberPatchInfo
    {
        /// <summary>
        /// The extension this was patched for.
        /// </summary>
        public UIExtensionInfo Extension { get; }

        /// <summary>
        /// The member that was patched.
        /// </summary>
        public UIExistingMemberInfo MemberInfo { get; }

        /// <summary>
        /// The transpiler method that gets patched over the member.
        /// </summary>
        public MethodInfo Transpiler { get; }

        /// <summary>
        /// Create a new <see cref="UIExtensionExistingMemberPatchInfo"/>.
        /// </summary>
        public UIExtensionExistingMemberPatchInfo(UIExtensionInfo extension, MethodInfo transpiler, UIExistingMemberInfo memberInfo)
        {
            Extension = extension;
            MemberInfo = memberInfo;
            Transpiler = transpiler;
        }
    }

    /// <summary>
    /// Caches an instance of a UI extension.
    /// </summary>
    /// <typeparam name="T">The type of the UI extension.</typeparam>
    protected static class InstanceCache<T> where T : class
    {
        private static T? _instance;

        /// <summary>
        /// UI Extension instance, or <see langword="null"/> if it can't be found.
        /// </summary>
        public static T? Instance
        {
            get
            {
                if (_instance == null)
                {
                    Recache();

                    if (_instance == null)
                    {
                        Type type = typeof(T);
                        UIExtensionInfo? extInfo = UnturnedUIToolsNexus.UIExtensionManager.Extensions.FirstOrDefault(x => x.ImplementationType == type);

                        if (UnturnedUIToolsNexus.UIExtensionManager is UIExtensionManager mngr)
                        {
                            if (extInfo != null)
                                mngr.LogWarning($"Unable to find instance of UI extension: {type.Name} extending {extInfo.TypeInfo.Type.Name}.", extInfo.Module, extInfo.Assembly);
                            else
                                mngr.LogWarning($"Unable to find instance of UI extension: {type.Name}.");
                        }
                    }
                }

                return _instance;
            }
        }

        static InstanceCache()
        {
            Recache();
            if (UnturnedUIToolsNexus.UIExtensionManager is UIExtensionManager mngr)
            {
                mngr.OnRemoved += OnDestroyed;
                mngr.OnAdd += OnAdded;
            }
        }
        private static void Recache()
        {
            UIExtensionInfo? info = UnturnedUIToolsNexus.UIExtensionManager.Extensions.FirstOrDefault(x => x.ImplementationType == typeof(T));

            if (info == null)
                return;
            _instance = info.Instantiations.OfType<T>().LastOrDefault();
        }
        private static void OnDestroyed(object instance)
        {
            if (instance is not T)
                return;

            if (ReferenceEquals(Instance, instance))
                _instance = null;
            Recache();
        }
        private static void OnAdded(object obj)
        {
            if (obj is T)
                Recache();
        }
    }

    /// <summary>
    /// Represents a method to create a UI extension object.
    /// </summary>
    public delegate object? CreateUIExtension(IUIExtensionManager extensionManager, object? uiInstance);
}
