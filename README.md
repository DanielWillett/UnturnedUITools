# UI Tools and Extensions

Powerful Unturned Module/Library for manipulating Glazier/Sleek UI.

Requires `HarmonyLib 2.3.3+`.

NuGet package: [DanielWillett.UnturnedUITools](https://www.nuget.org/packages/DanielWillett.UnturnedUITools)

## Reflection Tools
This package references my [Reflection Tools](https://github.com/DanielWillett/ReflectionTools) library which has a lot of good tools for efficient access to non-public members.

## XML Documentation
This package includes complete XML documentation for `SDG.Glazier.Runtime.dll`, which houses all of the Glazier code.

*Note: this is done by including a reference assembly in the package, so don't copy it from the output directory to your libraries.*

## UI Tools
The `UIAccessor` class has many tools for working with UIs:
* Many efficient getters for internal types and instances of vanilla UIs.
* Optional managed Harmony log that clears on startup to help diagnose transpilers.
* `EditorUIReady` and `PlayerUIReady` events.
* UI `TypeInfo` with basic information about each UI type (used by UI extensions).
* `OnInitializingUIInfo` to add your own information to `TypeInfo`.
* `LoadUIToILGenerator` and `EnumerateUIInstructions` to load instance singleton UIs to `ILGenerators` or transpilers.
* Extension for `ISleekElement`: `CopyTransformFrom` to copy all transform values from one element to another.

## UI Extensions
Add a class as an extension to existing vanilla or modded UI classes.

You can extend the following types. It is also possible to add your own types.
* Singleton and Static UIs. This includes any of the 'standard' in-game UI classes.
* `Useable` UIs (Guns, Housing Planners, etc).
* Node and Volume editor menus.
* Any class deriving from `SleekWrapper`.
* *Modules can add their own classes in as well.*

### Existing Members
Easy access to members of the class you're expanding (static or instance) is possible with Existing Members.
```cs
// Property getter will be patched to return messageLabel.
[ExistingMember("messageLabel")]
private ISleekLabel MessageLabel { get; }

// Property will be set before the constructor runs. Change this behavior with the 'InitializeMode' attribute property.
// If you choose to use the 'PatchGetter' option with a setter,
// you cannot use the setter to set the existing member's value back.
[ExistingMember("messageLabel")]
private ISleekLabel MessageLabel { get; set; }

// Field will be set before the constructor runs.
[ExistingMember("messageLabel")]
private readonly ISleekLabel _messageLabel;
```

### Common Mistake
Due to the way UI extensions are created, setting existing members to `null!` 
to suppress nullability warnings actually overrides the value set before the constructor runs.

Take care to avoid doing this.

```cs
// Setting this to null here to fix nullable references will run after the fields
// are initialized but before the constructor so _container will have a null value.
// 
// Instead use #nullable disable and #nullable restore to do this,
// or simply mark the field nullable.

// ================== BAD ==============================

[ExistingMember("container")]
private readonly SleekFullscreenBox _container = null!;

// ================== GOOD =============================

#nullable disable

// ...

[ExistingMember("container")]
private readonly SleekFullscreenBox _container;

// ...
#nullable restore

// ================== GOOD =============================

[ExistingMember("container")]
private readonly SleekFullscreenBox? _container;

// =====================================================
```

### Static Compatibility
The system is designed to be able to be accessed from a static context (like a patch, for example).
```cs
// Getting static or singleton UI extensions:

MenuDashboardUIExtension? extension = UnturnedUIToolsNexus.UIExtensionManager.GetInstance<MenuDashboardUIExtension>();


// Getting non-singleton UI extensions:

static void Postfix(object __instance)
{
  SleekItemExtension? extension = UnturnedUIToolsNexus.UIExtensionManager.GetInstance<SleekItemExtension>(__instance);
}
```

### Examples

More examples available in: [ExampleModule/Examples](https://github.com/DanielWillett/UnturnedUITools/tree/master/ExampleModule/Examples).

This example adds a button to the bottom left of the main menu.

![ex1 1](https://i.imgur.com/JhY9HBJ.png)

```cs
[UIExtension(typeof(MenuDashboardUI))]
internal class MenuDashboardUIExtension : UIExtension, IDisposable
{
    private const string Url = "https://github.com/DanielWillett/UnturnedUITools";

    private readonly ISleekButton _githubButton;

    [ExistingMember("exitButton")]
    private readonly SleekButtonIcon _exitButton;

    [ExistingMember("container")]
    private readonly SleekFullscreenBox _container;

    public MenuDashboardUIExtension()
    {
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
```
