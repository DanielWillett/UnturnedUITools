using DanielWillett.ReflectionTools;
using DanielWillett.UITools;
using DanielWillett.UITools.API.Extensions;
using DanielWillett.UITools.API.Extensions.Members;
using HarmonyLib;
using SDG.Unturned;
using System;
using System.Reflection;

namespace ExampleModule.Examples;

/*
 * In this example, we rotate all SleekItemIcons upside-down.
 */

[UIExtension(typeof(SleekItemIcon))]                 // Implement this interface to make sure your patch can be cleaned up.
internal class SleekItemIconExtension : UIExtension, IUnpatchableExtension
{
    private static bool _hasPatched;

    /*
     * Existing members allow us to get fields and properties from the original UI class.
     *
     * Here we use a getter-only property for our existing member, which will be patched to return the SleekSlot.image field.
     *
     * By default fields and properties with setters are initialized when the class is created,
     * while properties with only getters are patched to get the value in realtime.
     *
     * Use the InitializeMode property in the ExistingMember attribute to change this behavior.
     */

    // By passing FailureBehavior = Ignore, we're telling it to return null instead of failing to load if 'image' is not found.
    // This is good for optional features.

    [ExistingMember("internalImage", FailureBehavior = ExistingMemberFailureBehavior.Ignore)]
    private ISleekImage? Image { get; }

    public SleekItemIconExtension()
    {
        ISleekImage? image = Image;
        if (image != null)
        {
            // set the SleekItemIcon.internalImage to rotate 180 degrees.
            image.CanRotate = true;
            image.RotationAngle += 180f;
        }
        
        // We only want to do the patching once, not per item icon.
        if (!_hasPatched)
        {
            _hasPatched = true;
            Patch();
        }
    }

    // Patch SleekItemIcon.rot so that when the rotation changes we offset the rotation of the image again.
    private static void Patch()
    {
        MethodInfo? patch = typeof(SleekItemIcon).GetProperty(nameof(SleekItemIcon.rot), BindingFlags.Public | BindingFlags.Instance)?.SetMethod;

        if (patch == null)
        {
            CommandWindow.LogWarning("Unable to find setter of SleekItemIcon.rot.");
            return;
        }

        try
        {
            Nexus.Patcher.Patch(patch, postfix: new HarmonyMethod(Accessor.GetMethod(OnRotationUpdated)));
        }
        catch (Exception ex)
        {
            CommandWindow.LogWarning("Unable to patch setter of SleekItemIcon.rot:");
            CommandWindow.LogWarning(ex);
        }
    }

    // Clean up after your patch, this will be ran on the last instance of this class when the manager is being disposed.
    void IUnpatchableExtension.Unpatch()
    {
        MethodInfo? patch = typeof(SleekItemIcon).GetProperty(nameof(SleekItemIcon.rot), BindingFlags.Public | BindingFlags.Instance)?.SetMethod;

        if (patch == null)
        {
            CommandWindow.LogWarning("Unable to find setter of SleekItemIcon.rot.");
            return;
        }

        try
        {
            Nexus.Patcher.Unpatch(patch, Accessor.GetMethod(OnRotationUpdated));
        }
        catch (Exception ex)
        {
            CommandWindow.LogWarning("Unable to patch setter of SleekItemIcon.rot:");
            CommandWindow.LogWarning(ex);
        }
    }

    /*
     * Postfix for SleekItemIcon.rot setter.
     */
    private static void OnRotationUpdated(SleekItemIcon __instance, byte value)
    {
        // Use this method to get an extension from a vanilla UI instance. There is also one without an instance for singleton or static UIs.
        
        SleekItemIconExtension? extension = UnturnedUIToolsNexus.UIExtensionManager.GetInstance<SleekItemIconExtension>(__instance);

        if (extension == null)
            return;

        ISleekImage? image = extension.Image;

        if (image == null)
            return;

        // set the SleekItemIcon.internalImage to rotate 180 degrees.
        image.RotationAngle += 180f;
    }
}