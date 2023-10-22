using DanielWillett.UITools.API.Extensions;
using SDG.Framework.Devkit;
using SDG.Unturned;
using System.Linq;
using UnityEngine;

namespace ExampleModule.Examples;

/*
 * In this example, we add a button to the LocationDevkitNode's menu that copies the location name to the clipboard.
 */

// Note that in the CLR, the '+' indicates a nested type.
[UIExtension("LocationDevkitNode+Menu")]
internal class LocationDevkitNodeMenuExtension : UIExtension<SleekWrapper>
{
    private readonly ISleekButton _copyLocationNameButton;
    public LocationDevkitNodeMenuExtension()
    {
        _copyLocationNameButton = Glazier.Get().CreateButton();

        _copyLocationNameButton.PositionOffset_Y = 90f;
        _copyLocationNameButton.SizeOffset_X = 200f;
        _copyLocationNameButton.SizeOffset_Y = 30f;
        _copyLocationNameButton.Text = "Copy Location";
        _copyLocationNameButton.OnClicked += OnClickedCopyLocationNameButton;
        
        // Menu's derive from SleekWrapper, so we can derive our extension from UIExtension<SleekWrapper> and use Instance to reference the menu.
        Instance!.AddChild(_copyLocationNameButton);
    }

    protected override void Opened()
    {
        // Change the size after the UI is opened, because automatic scaling will override changes made in the constructor.

        Instance!.SizeOffset_Y += 40;
        Instance!.PositionOffset_Y -= 40;
    }

    private static void OnClickedCopyLocationNameButton(ISleekElement button)
    {
        // Get selected location node.
        DevkitSelection? selection = DevkitSelectionManager.selection.FirstOrDefault();
        if (selection == null || !selection.transform.TryGetComponent(out LocationDevkitNode node))
            return;

        // Copy it to clipboard.
        GUIUtility.systemCopyBuffer = node.locationName;
    }

    // No need to remove this one as a child since the SleekWrapper parent is also being destroyed.
}