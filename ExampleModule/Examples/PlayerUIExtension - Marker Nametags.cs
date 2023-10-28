using DanielWillett.ReflectionTools;
using DanielWillett.UITools;
using DanielWillett.UITools.API.Extensions;
using DanielWillett.UITools.Util;
using HarmonyLib;
using SDG.Framework.Landscapes;
using SDG.Framework.Utilities;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ExampleModule.Examples;

/*
 * In this example, we add HUD labels to map markers that show up in the viewport.
 */

[UIExtension(typeof(PlayerUI))]                          // Implement this interface to make sure your patch can be cleaned up.
internal class PlayerUIExtension : ContainerUIExtension, IUnpatchableExtension
                                   // ContainerUIExtension is made to help with fullscreen containers
                                   //   used for adding stuff to the viewport like nametags, etc.
{
    private static bool _hasPatched;

    // Keep track of our labels.
    private readonly Dictionary<ulong, MarkerLabel> _labels = new Dictionary<ulong, MarkerLabel>();

    private bool _subbedToEvents;

    // Set the parent window of this extension's fullscreen box.
    protected override SleekWindow Parent => PlayerUI.window;

    // This method is where you should create all your components.
    protected override void OnShown()
    {
        // Subscribe to Unturneds OnUpdated event which gets invoked every frame.
        if (!_subbedToEvents)
        {
            _subbedToEvents = true;
            TimeUtility.updated += OnUpdated;
        }

        // We only want to do the patching once, not per item icon.
        if (!_hasPatched)
        {
            _hasPatched = true;
            Patch();
        }

        UpdateAllLabels();
    }

    // This method is where you should try to remove all your components.
    protected override void OnHidden()
    {
        // Unsubscribe from OnUpdated event.
        if (_subbedToEvents)
        {
            _subbedToEvents = false;
            TimeUtility.updated -= OnUpdated;
        }

        // Clean up labels.
        if (Container != null)
        {
            foreach (MarkerLabel label in _labels.Values)
            {
                Container.TryRemoveChild(label.Label);
            }
        }

        _labels.Clear();
    }

    // This method is separate to OnHidden but usually can do the same thing. Add any extra cleanup here that may need reused.
    // In the case of PlayerUI, it will be shown as soon as its initialized and hidden just before its destroyed.
    protected override void OnDestroyed()
    {
        OnHidden();
    }

    // Patch PlayerQuests.ReceiveMarkerState so that when a marker is updated we update the nametag.
    private static void Patch()
    {
        MethodInfo? originalMethod = typeof(PlayerQuests).GetMethod(nameof(PlayerQuests.ReceiveMarkerState), BindingFlags.Public | BindingFlags.Instance);

        if (originalMethod == null)
        {
            CommandWindow.LogWarning("Unable to find method PlayerQuests.ReceiveMarkerState.");
            return;
        }

        try
        {
            Nexus.Patcher.Patch(originalMethod, postfix: new HarmonyMethod(Accessor.GetMethod(OnMarkerUpdated)));
        }
        catch (Exception ex)
        {
            CommandWindow.LogWarning("Unable to patch PlayerQuests.ReceiveMarkerState:");
            CommandWindow.LogWarning(ex);
        }
    }

    // Clean up after your patch, this will be ran on the last instance of this class when the manager is being disposed.
    void IUnpatchableExtension.Unpatch()
    {
        MethodInfo? originalMethod = typeof(PlayerQuests).GetMethod(nameof(PlayerQuests.ReceiveMarkerState), BindingFlags.Public | BindingFlags.Instance);

        if (originalMethod == null)
        {
            CommandWindow.LogWarning("Unable to find method PlayerQuests.ReceiveMarkerState.");
            return;
        }

        try
        {
            Nexus.Patcher.Unpatch(originalMethod, Accessor.GetMethod(OnMarkerUpdated));
        }
        catch (Exception ex)
        {
            CommandWindow.LogWarning("Unable to patch PlayerQuests.ReceiveMarkerState:");
            CommandWindow.LogWarning(ex);
        }
    }

    // Called by our patch every time new marker information is available.
    private static void OnMarkerUpdated(PlayerQuests __instance, bool newIsMarkerPlaced, Vector3 newMarkerPosition, string newMarkerTextOverride)
    {
        // Find our extension instance.
        PlayerUIExtension? extension = UnturnedUIToolsNexus.UIExtensionManager.GetInstance<PlayerUIExtension>();
        if (extension == null)
            return;

        // Update the label for that player.
        if (!extension._labels.TryGetValue(__instance.player.channel.owner.playerID.steamID.m_SteamID, out MarkerLabel label))
        {
            if (newIsMarkerPlaced)
                extension.CreateLabel(__instance.player);
        }
        else
            extension.UpdateLabel(label, false);
    }

    // Called every frame by TimeUtility.updated.
    private void OnUpdated()
    {
        // Update all our labels every frame.
        UpdateAllLabels();
    }

    private void UpdateAllLabels()
    {
        if (Container == null)
            return;

        // Update the position of each player's label if it's placed.
        foreach (SteamPlayer player in Provider.clients)
        {
            if (!player.player.quests.isMarkerPlaced)
                continue;

            if (!_labels.TryGetValue(player.playerID.steamID.m_SteamID, out MarkerLabel label))
                CreateLabel(player.player);
            else
                UpdateLabel(label, true);
        }
    }
    private void CreateLabel(Player player)
    {
        // Create a new label for a player.

        if (Container == null)
            return;

        ISleekLabel sleekLabel = Glazier.Get().CreateLabel();

        sleekLabel.TextContrastContext = ETextContrastContext.ColorfulBackdrop;
        sleekLabel.TextAlignment = TextAnchor.MiddleCenter;
        
        sleekLabel.PositionOffset_X = -100;
        sleekLabel.PositionOffset_Y = -15;
        sleekLabel.SizeOffset_X = 200;
        sleekLabel.SizeOffset_Y = 30;

        MarkerLabel label = new MarkerLabel(player, sleekLabel);
        UpdateLabel(label, false);

        // Add the label to our dictionary and as a child on Container.
        _labels.Add(player.channel.owner.playerID.steamID.m_SteamID, label);
        Container.AddChild(sleekLabel);
    }
    private void UpdateLabel(MarkerLabel label, bool relativePositionOnly)
    {
        if (Container == null)
            return;

        if (!label.Player.quests.isMarkerPlaced)
        {
            // Remove the label if the player doesn't have a placed marker.

            _labels.Remove(label.Player.channel.owner.playerID.steamID.m_SteamID);

            if (Container.FindIndexOfChild(label.Label) >= 0)
                Container.RemoveChild(label.Label);
            
            return;
        }

        if (!relativePositionOnly || !label.Position.HasValue)
        {
            // Put the label 75m above the terrain.

            Landscape.getWorldHeight(label.Player.quests.markerPosition, out float height);
            Vector3 pos = label.Player.quests.markerPosition with
            {
                y = height + 75f
            };

            label.Position = pos;
        }

        // WorldToViewportPoint converts a world position to a position on the screen using a Camera.
        Vector3 screenPos = MainCamera.instance.WorldToViewportPoint(label.Position.Value);

        if (screenPos.z <= 0.0)
        {
            // The label is behind the player.
            label.Label.IsVisible = false;
        }
        else
        {
            // ViewportToNormalizedPosition converts that screen position to values from 0 to 1 on the Container.
            Vector2 screenPositionScale = Container.ViewportToNormalizedPosition(screenPos);

            label.Label.PositionScale_X = screenPositionScale.x;
            label.Label.PositionScale_Y = screenPositionScale.y;
            
            if (!relativePositionOnly || string.IsNullOrEmpty(label.Label.Text))
            {
                // Set the text of the label to what would show on the map.
                label.Label.Text = string.IsNullOrEmpty(label.Player.quests.markerTextOverride)
                    ? label.Player.channel.owner.playerID.nickName
                    : label.Player.quests.markerTextOverride;
            }

            label.Label.IsVisible = true;
        }
    }

    // A class to store information about our label.
    private class MarkerLabel
    {
        public Player Player { get; }
        public ISleekLabel Label { get; }
        public Vector3? Position { get; set; }
        public MarkerLabel(Player player, ISleekLabel label)
        {
            Player = player;
            Label = label;
        }
    }
}
