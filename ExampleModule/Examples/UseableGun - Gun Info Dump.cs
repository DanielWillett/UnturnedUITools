using DanielWillett.UITools.API.Extensions;
using DanielWillett.UITools.Util;
using HarmonyLib;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ExampleModule.Examples;

/*
 * In this example, we add an info dump about the gun you're holding.
 */

[UIExtension(typeof(UseableGun))]
internal class UseableGunExtension : UIExtension, IDisposable
{
    private readonly ISleekLabel _gunInfo;
    public UseableGunExtension()
    {
        _gunInfo = Glazier.Get().CreateLabel();

        _gunInfo.SizeScale_X = 0.5f;
        _gunInfo.SizeScale_Y = 1f;

        _gunInfo.PositionOffset_Y = 20f;
        _gunInfo.PositionOffset_X = 20f;

        _gunInfo.SizeOffset_X = -40f;
        _gunInfo.SizeOffset_Y = -40f;

        _gunInfo.AllowRichText = true;
        _gunInfo.TextContrastContext = ETextContrastContext.ColorfulBackdrop;
        _gunInfo.TextColor = ESleekTint.RICH_TEXT_DEFAULT;
        _gunInfo.TextAlignment = TextAnchor.MiddleLeft;

        // Build the item description list to display.
        ItemDescriptionBuilder builder = new ItemDescriptionBuilder
        {
            lines = new List<ItemDescriptionLine>()
        };

        // Get equipped 'Item'.
        byte page = Player.player.equipment.equippedPage;
        byte x = Player.player.equipment.equipped_x, y = Player.player.equipment.equipped_x;

        byte index = Player.player.inventory.getIndex(page, x, y);
        Item? item = index == byte.MaxValue ? null : Player.player.inventory.getItem(page, index)?.item;

        // Build and sort description
        Player.player.equipment.asset?.BuildDescription(builder, item);
        builder.lines.Sort();

        // Set the text, spacing each line with newline characters.
        _gunInfo.Text = builder.lines.Join(x => x.text, Environment.NewLine);

        // Add to PlayerUI container, which is where UseableGun adds it's children.
        PlayerUI.container.AddChild(_gunInfo);
    }

    public void Dispose()
    {
        // Clean up our UI after the gun is put away.
        PlayerUI.container.TryRemoveChild(_gunInfo);
    }
}