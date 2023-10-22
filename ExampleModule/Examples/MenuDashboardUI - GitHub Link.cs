using DanielWillett.UITools.API.Extensions;
using DanielWillett.UITools.API.Extensions.Members;
using DanielWillett.UITools.Util;
using SDG.Unturned;
using System;
using System.Diagnostics;

namespace ExampleModule.Examples;

// It's recommended to not use a nullable context for these because existing members
//   are initialized before the constructor runs, which the compiler doesnt like.
#nullable disable 

/*
 * In this example, we add a button to the bottom left of the main menu that opens a GitHub link.
 */

[UIExtension(typeof(MenuDashboardUI))] // Define the UI type that we will be extending
internal class MenuDashboardUIExtension : UIExtension, IDisposable
                                                       // Dispose will be called on extensions implementing IDisposable
{
    private const string Url = "https://github.com/DanielWillett/UnturnedUITools";

    private readonly ISleekButton _githubButton;

    /*
     * Existing members allow us to get fields and properties from the original UI class.
     *
     * Here we use fields for our existing members, but properties can also be used.
     *
     * By default fields and properties with setters are initialized when the class is created,
     * while properties with only getters are patched to get the value in realtime.
     *
     * Use the InitializeMode property in the ExistingMember attribute to change this behavior.
     */

    [ExistingMember("exitButton")]
    private readonly SleekButtonIcon _exitButton;

    [ExistingMember("container")]
    private readonly SleekFullscreenBox _container;

    public MenuDashboardUIExtension()
    {
        /*
         * Here we are able to add this code in the constructor.
         *
         * If you have issues, you may have to experiment with overriding
         *   the Opened method and adding it there instead.
         */

        _githubButton = Glazier.Get().CreateButton();

        _githubButton.CopyTransformFrom(_exitButton);
        _githubButton.PositionOffset_Y -= 60;
        _githubButton.Text = "UITools GitHub";
        _githubButton.FontSize = ESleekFontSize.Medium;
        _githubButton.OnClicked += OnClickedGithubButton;
        _githubButton.BackgroundColor = ESleekTint.BACKGROUND;

        _container.AddChild(_githubButton);
    }

    private static void OnClickedGithubButton(ISleekElement button)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = Url,
            UseShellExecute = true
        });
    }

    public void Dispose()
    {
        _githubButton.OnClicked -= OnClickedGithubButton;
        _container.RemoveChild(_githubButton);
    }
}

#nullable restore