using DanielWillett.UITools.API;

namespace DanielWillett.UITools.Util;

/// <summary>
/// Common sleek presets found in-game.
/// </summary>
public static class SleekTransformPresets
{
    /// <summary>
    /// Shorter buttons found in various places.
    /// </summary>
    public static readonly SleekTransformPreset SmallButton = SleekTransformPreset.SizePixels(200f, 30f);

    /// <summary>
    /// Slightly taller buttons common in the menu.
    /// </summary>
    public static readonly SleekTransformPreset LargeButton = SleekTransformPreset.SizePixels(200f, 50f);

    /// <summary>
    /// Fills the entire screen.
    /// </summary>
    public static readonly SleekTransformPreset FullScreen = new SleekTransformPreset(0f, 0f, 0f, 0f, 0f, 0f, 1f, 1f);

    /// <summary>
    /// Fills the entire screen with a 10px border. Common in a lot of dashboard menus.
    /// </summary>
    public static readonly SleekTransformPreset FullScreenWithInset = new SleekTransformPreset(10f, 10f, 0f, 0f, -20f, -20f, 1f, 1f);
}
