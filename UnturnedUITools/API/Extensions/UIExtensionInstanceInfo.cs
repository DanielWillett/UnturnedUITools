﻿namespace DanielWillett.UITools.API.Extensions;

/// <summary>
/// Stores information about an extension instance.
/// </summary>
public class UIExtensionInstanceInfo
{
    /// <summary>
    /// A reference to the actual extension.
    /// </summary>
    public object Instance { get; }

    /// <summary>
    /// The vanilla type the instance is extending.
    /// </summary>
    public UIExtensionVanillaInstanceInfo VanillaInstance { get; }
    internal UIExtensionInstanceInfo(object instance, UIExtensionVanillaInstanceInfo vanillaInstance)
    {
        Instance = instance;
        VanillaInstance = vanillaInstance;
    }
}