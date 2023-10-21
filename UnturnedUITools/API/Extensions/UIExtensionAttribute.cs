using JetBrains.Annotations;
using SDG.Unturned;
using System;
using System.Reflection;

namespace DanielWillett.UITools.API.Extensions;

/// <summary>
/// Mark your extension to be auto-registered to <see cref="UIExtensionManager"/> when your plugin loads.
/// </summary>
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class UIExtensionAttribute : Attribute
{
    /// <summary>
    /// Type of the vanilla UI being extended.
    /// </summary>
    public Type ParentType { get; }

    /// <summary>
    /// Supress the warning for not deriving an extension from <see cref="UIExtension"/>.
    /// </summary>
    public bool SuppressUIExtensionParentWarning { get; set; }

    public UIExtensionAttribute(Type parentType)
    {
        ParentType = parentType;
    }

    /// <summary>
    /// Add type by name, mainly for internal types.<br/>
    /// Use assembly qualified name if the type is not from SDG (Assembly-CSharp.dll).
    /// </summary>
    public UIExtensionAttribute(string parentType)
    {
        Assembly sdg = typeof(Provider).Assembly;
        ParentType = Type.GetType(parentType, false, true) ?? sdg.GetType(parentType, false, true) ?? sdg.GetType("SDG.Unturned." + parentType, true, true);
    }
}
