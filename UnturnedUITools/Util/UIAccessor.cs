using DanielWillett.ReflectionTools;
using DanielWillett.ReflectionTools.Formatting;
using DanielWillett.UITools.API;
using DanielWillett.UITools.Core.Handlers;
using HarmonyLib;
using JetBrains.Annotations;
using SDG.Framework.Modules;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using UnityEngine;

namespace DanielWillett.UITools.Util;

/// <summary>
/// Reflection tools for working with vanilla UIs.
/// </summary>
public static class UIAccessor
{
    private static readonly object Sync = new object();

    private static Harmony? _patcher;
    private static string _harmonyId = "DanielWillett.UITools";
    private static string? _harmonyLogPath;
    private static bool _manageHarmonyDebugLog;
    private static int _init;

    internal const string Source = "UI TOOLS";

    internal static Harmony Patcher
    {
        get
        {
            if (_patcher != null)
                return _patcher;

            lock (Sync)
            {
                if (_patcher == null)
                {
                    // set up harmony debug log if enabled.
                    if (_manageHarmonyDebugLog)
                    {
                        _harmonyLogPath ??= Path.Combine(UnturnedPaths.RootDirectory.FullName, "Logs", "harmony.log");
                        HarmonyLog.Reset(_harmonyLogPath, enableDebug: true);
                    }

                    _patcher = new Harmony(_harmonyId);
                }

                return _patcher;
            }
        }
    }

    /// <summary>
    /// Set the ID to use for the <see cref="Harmony"/> instance used by the module.
    /// </summary>
    /// <remarks>Must be set in <see cref="IModuleNexus.initialize"/>.</remarks>
    /// <exception cref="InvalidOperationException">Patcher is already set up.</exception>
    public static string HarmonyId
    {
        get => _harmonyId;
        set
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            lock (Sync)
            {
                if (_patcher != null)
                    throw new InvalidOperationException("Patcher has already been set up.");

                _harmonyId = value;
            }
        }
    }

    /// <summary>
    /// Set the path of the harmony debug log. Defaults to <c>Unturned/Logs/harmony.log</c>. Enable it with <see cref="ManageHarmonyDebugLog"/>.
    /// </summary>
    /// <remarks>Must be set in <see cref="IModuleNexus.initialize"/>.</remarks>
    /// <exception cref="InvalidOperationException">Patcher is already set up.</exception>
    public static string? HarmonyLogPath
    {
        get => _harmonyLogPath;
        set
        {
            lock (Sync)
            {
                if (_patcher != null)
                    throw new InvalidOperationException("Patcher has already been set up.");

                _harmonyLogPath = value;
                Environment.SetEnvironmentVariable("HARMONY_LOG_FILE", _harmonyLogPath);
            }
        }
    }

    /// <summary>
    /// Enable managing the harmony debug log? Set the path with <see cref="HarmonyLogPath"/>.
    /// </summary>
    /// <remarks>Must be set in <see cref="IModuleNexus.initialize"/>.</remarks>
    /// <exception cref="InvalidOperationException">Patcher is already set up.</exception>
    public static bool ManageHarmonyDebugLog
    {
        get => _manageHarmonyDebugLog;
        set
        {
            lock (Sync)
            {
                if (_patcher != null)
                    throw new InvalidOperationException("Patcher has already been set up.");

                _manageHarmonyDebugLog = value;
            }
        }
    }

    private static readonly StaticGetter<EditorUI?> GetEditorUIInstance
        = Accessor.GenerateStaticGetter<EditorUI, EditorUI?>("instance", throwOnError: true)!;

    private static readonly StaticGetter<PlayerUI?> GetPlayerUIInstance
        = Accessor.GenerateStaticGetter<PlayerUI, PlayerUI?>("instance", throwOnError: true)!;

    private static readonly StaticGetter<MenuUI?>? GetMenuUIInstance
        = Accessor.GenerateStaticGetter<MenuUI, MenuUI?>("instance");

    private static readonly InstanceGetter<EditorUI, EditorDashboardUI?> GetEditorDashboardUIInstance
        = Accessor.GenerateInstanceGetter<EditorUI, EditorDashboardUI?>("dashboardUI", throwOnError: true)!;

    private static readonly InstanceGetter<EditorDashboardUI, EditorEnvironmentUI?> GetEditorEnvironmentUIInstance
        = Accessor.GenerateInstanceGetter<EditorDashboardUI, EditorEnvironmentUI?>("environmentUI", throwOnError: true)!;

    private static readonly InstanceGetter<EditorDashboardUI, EditorTerrainUI?> GetEditorTerrainUIInstance
        = Accessor.GenerateInstanceGetter<EditorDashboardUI, EditorTerrainUI?>("terrainMenu", throwOnError: true)!;

    private static readonly InstanceGetter<EditorDashboardUI, EditorLevelUI?> GetEditorLevelUIInstance
        = Accessor.GenerateInstanceGetter<EditorDashboardUI, EditorLevelUI?>("levelUI", throwOnError: true)!;

    private static readonly InstanceGetter<EditorLevelUI, EditorLevelObjectsUI?> GetEditorLevelObjectsUIInstance
        = Accessor.GenerateInstanceGetter<EditorLevelUI, EditorLevelObjectsUI?>("objectsUI", throwOnError: true)!;

    private static readonly InstanceGetter<PlayerUI, object?>? GetPlayerBrowserRequestUI
        = Accessor.GenerateInstanceGetter<PlayerUI, object?>("browserRequestUI");

    private static readonly InstanceGetter<PlayerUI, object?>? GetPlayerGroupUI
        = Accessor.GenerateInstanceGetter<PlayerUI, object?>("groupUI");

    private static readonly InstanceGetter<PlayerUI, PlayerBarricadeMannequinUI?>? GetPlayerBarricadeMannequinUI
        = Accessor.GenerateInstanceGetter<PlayerUI, PlayerBarricadeMannequinUI?>("mannequinUI");

    private static readonly InstanceGetter<PlayerUI, PlayerBarricadeStereoUI?>? GetPlayerBarricadeStereoUI
        = Accessor.GenerateInstanceGetter<PlayerUI, PlayerBarricadeStereoUI?>("boomboxUI");

    private static readonly InstanceGetter<PlayerUI, PlayerDashboardUI?>? GetPlayerDashboardUI
        = Accessor.GenerateInstanceGetter<PlayerUI, PlayerDashboardUI?>("dashboardUI");

    private static readonly InstanceGetter<PlayerUI, PlayerPauseUI?>? GetPlayerPauseUI
        = Accessor.GenerateInstanceGetter<PlayerUI, PlayerPauseUI?>("pauseUI");

    private static readonly InstanceGetter<PlayerUI, PlayerLifeUI?>? GetPlayerLifeUI
        = Accessor.GenerateInstanceGetter<PlayerUI, PlayerLifeUI?>("lifeUI");

    private static readonly InstanceGetter<PlayerDashboardUI, PlayerDashboardInformationUI?>? GetPlayerDashboardInformationUI
        = Accessor.GenerateInstanceGetter<PlayerDashboardUI, PlayerDashboardInformationUI?>("infoUI");

    private static readonly InstanceGetter<MenuDashboardUI, MenuPauseUI?>? GetMenuPauseUI
        = Accessor.GenerateInstanceGetter<MenuDashboardUI, MenuPauseUI?>("pauseUI");

    private static readonly InstanceGetter<MenuDashboardUI, MenuCreditsUI?>? GetMenuCreditsUI
        = Accessor.GenerateInstanceGetter<MenuDashboardUI, MenuCreditsUI?>("creditsUI");

    private static readonly InstanceGetter<MenuDashboardUI, MenuTitleUI?>? GetMenuTitleUI
        = Accessor.GenerateInstanceGetter<MenuDashboardUI, MenuTitleUI?>("titleUI");

    private static readonly InstanceGetter<MenuDashboardUI, MenuPlayUI?>? GetMenuPlayUI
        = Accessor.GenerateInstanceGetter<MenuDashboardUI, MenuPlayUI?>("playUI");

    private static readonly InstanceGetter<MenuDashboardUI, MenuSurvivorsUI?>? GetMenuSurvivorsUI
        = Accessor.GenerateInstanceGetter<MenuDashboardUI, MenuSurvivorsUI?>("survivorsUI");

    private static readonly InstanceGetter<MenuDashboardUI, MenuConfigurationUI?>? GetMenuConfigurationUI
        = Accessor.GenerateInstanceGetter<MenuDashboardUI, MenuConfigurationUI?>("configUI");

    private static readonly InstanceGetter<MenuConfigurationUI, MenuConfigurationAudioUI?>? GetMenuConfigurationAudioUI
        = Accessor.GenerateInstanceGetter<MenuConfigurationUI, MenuConfigurationAudioUI?>("audioMenu");

    private static readonly InstanceGetter<MenuDashboardUI, MenuWorkshopUI?>? GetMenuWorkshopUI
        = Accessor.GenerateInstanceGetter<MenuDashboardUI, MenuWorkshopUI?>("workshopUI");

    private static readonly InstanceGetter<MenuUI, MenuDashboardUI?>? GetMenuDashboardUI
        = Accessor.GenerateInstanceGetter<MenuUI, MenuDashboardUI?>("dashboard");

    private static readonly InstanceGetter<MenuPlayUI, MenuPlayConnectUI?>? GetMenuPlayConnectUI
        = Accessor.GenerateInstanceGetter<MenuPlayUI, MenuPlayConnectUI?>("connectUI");

    private static readonly InstanceGetter<MenuPlayUI, MenuPlayServerInfoUI?>? GetMenuPlayServerInfoUI
        = Accessor.GenerateInstanceGetter<MenuPlayUI, MenuPlayServerInfoUI?>("serverInfoUI");

    private static readonly InstanceGetter<MenuPlayUI, MenuPlaySingleplayerUI?>? GetMenuPlaySingleplayerUI
        = Accessor.GenerateInstanceGetter<MenuPlayUI, MenuPlaySingleplayerUI?>("singleplayerUI");

    private static readonly InstanceGetter<MenuPlayServerCurationUI, object?>? GetMenuPlayServerCurationRulesUI
        = Accessor.GenerateInstanceGetter<MenuPlayServerCurationUI, object?>("rulesUI");

    private static readonly InstanceGetter<MenuPlayUI, MenuPlayLobbiesUI?>? GetMenuPlayLobbiesUI
        = Accessor.GenerateInstanceGetter<MenuPlayUI, MenuPlayLobbiesUI?>("lobbiesUI");

    private static readonly StaticGetter<MenuServerPasswordUI?>? GetMenuServerPasswordUI
        = Accessor.GenerateStaticGetter<MenuPlayServerInfoUI, MenuServerPasswordUI?>("passwordUI");

    private static readonly InstanceGetter<MenuSurvivorsUI, MenuSurvivorsCharacterUI?>? GetMenuSurvivorsCharacterUI
        = Accessor.GenerateInstanceGetter<MenuSurvivorsUI, MenuSurvivorsCharacterUI?>("characterUI");

    private static readonly InstanceGetter<MenuSurvivorsUI, MenuSurvivorsAppearanceUI?>? GetMenuSurvivorsAppearanceUI
        = Accessor.GenerateInstanceGetter<MenuSurvivorsUI, MenuSurvivorsAppearanceUI?>("appearanceUI");

    private static readonly InstanceGetter<MenuSurvivorsUI, MenuSurvivorsGroupUI?>? GetMenuSurvivorsGroupUI
        = Accessor.GenerateInstanceGetter<MenuSurvivorsUI, MenuSurvivorsGroupUI?>("groupUI");

    private static readonly InstanceGetter<MenuSurvivorsUI, MenuSurvivorsClothingUI?>? GetMenuSurvivorsClothingUI
        = Accessor.GenerateInstanceGetter<MenuSurvivorsUI, MenuSurvivorsClothingUI?>("clothingUI");

    private static readonly InstanceGetter<MenuSurvivorsClothingUI, MenuSurvivorsClothingItemUI?>? GetMenuSurvivorsClothingItemUI
        = Accessor.GenerateInstanceGetter<MenuSurvivorsClothingUI, MenuSurvivorsClothingItemUI?>("itemUI");

    private static readonly InstanceGetter<MenuSurvivorsClothingUI, MenuSurvivorsClothingInspectUI?>? GetMenuSurvivorsClothingInspectUI
        = Accessor.GenerateInstanceGetter<MenuSurvivorsClothingUI, MenuSurvivorsClothingInspectUI?>("inspectUI");

    private static readonly InstanceGetter<MenuSurvivorsClothingUI, MenuSurvivorsClothingDeleteUI?>? GetMenuSurvivorsClothingDeleteUI
        = Accessor.GenerateInstanceGetter<MenuSurvivorsClothingUI, MenuSurvivorsClothingDeleteUI?>("deleteUI");

    private static readonly InstanceGetter<MenuSurvivorsClothingUI, MenuSurvivorsClothingBoxUI?>? GetMenuSurvivorsClothingBoxUI
        = Accessor.GenerateInstanceGetter<MenuSurvivorsClothingUI, MenuSurvivorsClothingBoxUI?>("boxUI");

    private static readonly InstanceGetter<MenuSurvivorsClothingUI, object?>? GetItemStoreMenu
        = Accessor.GenerateInstanceGetter<MenuSurvivorsClothingUI, object?>("itemStoreUI");

    private static readonly InstanceGetter<MenuWorkshopUI, MenuWorkshopSubmitUI?>? GetMenuWorkshopSubmitUI
        = Accessor.GenerateInstanceGetter<MenuWorkshopUI, MenuWorkshopSubmitUI?>("submitUI");

    private static readonly InstanceGetter<MenuWorkshopUI, MenuWorkshopEditorUI?>? GetMenuWorkshopEditorUI
        = Accessor.GenerateInstanceGetter<MenuWorkshopUI, MenuWorkshopEditorUI?>("editorUI");

    private static readonly InstanceGetter<MenuWorkshopUI, MenuWorkshopErrorUI?>? GetMenuWorkshopErrorUI
        = Accessor.GenerateInstanceGetter<MenuWorkshopUI, MenuWorkshopErrorUI?>("errorUI");

    private static readonly InstanceGetter<MenuWorkshopUI, MenuWorkshopLocalizationUI?>? GetMenuWorkshopLocalizationUI
        = Accessor.GenerateInstanceGetter<MenuWorkshopUI, MenuWorkshopLocalizationUI?>("localizationUI");

    private static readonly InstanceGetter<MenuWorkshopUI, MenuWorkshopSpawnsUI?>? GetMenuWorkshopSpawnsUI
        = Accessor.GenerateInstanceGetter<MenuWorkshopUI, MenuWorkshopSpawnsUI?>("spawnsUI");

    private static readonly InstanceGetter<MenuWorkshopUI, MenuWorkshopSubscriptionsUI?>? GetMenuWorkshopSubscriptionsUI
        = Accessor.GenerateInstanceGetter<MenuWorkshopUI, MenuWorkshopSubscriptionsUI?>("subscriptionsUI");

    private static readonly Dictionary<Type, Type> MenuTypes = new Dictionary<Type, Type>(32);

    private static readonly Func<object?>? GetEditorTerrainHeightUI;
    private static readonly Func<object?>? GetEditorTerrainMaterialsUI;
    private static readonly Func<object?>? GetEditorTerrainDetailsUI;
    private static readonly Func<object?>? GetEditorTerrainTilesUI;

    private static readonly Func<object?>? GetEditorEnvironmentNodesUI;

    private static readonly Func<object?>? GetEditorVolumesUI;

    private static readonly Func<object?>? GetItemStoreCartMenu;
    private static readonly Func<object?>? GetItemStoreDetailsMenu;
    private static readonly Func<object?>? GetItemStoreBundleContentsMenu;

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.EditorUI"/>.
    /// </summary>
    public static EditorUI? EditorUI
    {
        get
        {
            EditorUI? editorUI = GetEditorUIInstance();
            return editorUI == null ? null : editorUI;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.PlayerUI"/>.
    /// </summary>
    public static PlayerUI? PlayerUI
    {
        get
        {
            PlayerUI? playerUI = GetPlayerUIInstance();
            return playerUI == null ? null : playerUI;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.MenuUI"/>.
    /// </summary>
    public static MenuUI? MenuUI
    {
        get
        {
            MenuUI? menuUi = GetMenuUIInstance?.Invoke();
            return menuUi == null ? null : menuUi;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.LoadingUI"/>.
    /// </summary>
    public static LoadingUI? LoadingUI
    {
        get
        {
            if (_loadingUI == null)
            {
                if (!Thread.CurrentThread.IsGameThread())
                    return null;
                GameObject? host = LoadingUI.loader;
                if (host != null)
                    _loadingUI = host.GetComponent<LoadingUI>();
            }

            return _loadingUI;
        }
    }

    /// <summary>
    /// Called when the <see cref="SDG.Unturned.EditorUI"/> is initialized.
    /// </summary>
    public static event System.Action? EditorUIReady;

    /// <summary>
    /// Called when the <see cref="SDG.Unturned.PlayerUI"/> is initialized.
    /// </summary>
    public static event System.Action? PlayerUIReady;

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.EditorDashboardUI"/>.
    /// </summary>
    public static EditorDashboardUI? EditorDashboardUI
    {
        get
        {
            EditorUI? editorUi = EditorUI;
            return editorUi == null ? null : GetEditorDashboardUIInstance(editorUi);
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.EditorEnvironmentUI"/>.
    /// </summary>
    public static EditorEnvironmentUI? EditorEnvironmentUI
    {
        get
        {
            EditorDashboardUI? dashboard = EditorDashboardUI;
            return dashboard == null ? null : GetEditorEnvironmentUIInstance(dashboard);
        }
    }

    /// <summary>
    /// Type of <see cref="SDG.Unturned.EditorEnvironmentNodesUI"/>.
    /// </summary>
    public static Type? EditorEnvironmentNodesUIType { get; }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.EditorEnvironmentNodesUI"/>.
    /// </summary>
    public static object? EditorEnvironmentNodesUI => GetEditorEnvironmentNodesUI?.Invoke();

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.EditorTerrainUI"/>.
    /// </summary>
    public static EditorTerrainUI? EditorTerrainUI
    {
        get
        {
            EditorDashboardUI? dashboard = EditorDashboardUI;
            return dashboard == null ? null : GetEditorTerrainUIInstance(dashboard);
        }
    }

    /// <summary>
    /// Type of <see cref="SDG.Unturned.EditorTerrainHeightUI"/>.
    /// </summary>
    public static Type? EditorTerrainHeightUIType { get; }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.EditorTerrainHeightUI"/>.
    /// </summary>
    public static object? EditorTerrainHeightUI => GetEditorTerrainHeightUI?.Invoke();

    /// <summary>
    /// Type of <see cref="SDG.Unturned.EditorTerrainMaterialsUI"/>.
    /// </summary>
    public static Type? EditorTerrainMaterialsUIType { get; }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.EditorTerrainMaterialsUI"/>.
    /// </summary>
    public static object? EditorTerrainMaterialsUI => GetEditorTerrainMaterialsUI?.Invoke();

    /// <summary>
    /// Type of <see cref="SDG.Unturned.EditorTerrainDetailsUI"/>.
    /// </summary>
    public static Type? EditorTerrainDetailsUIType { get; }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.EditorTerrainDetailsUI"/>.
    /// </summary>
    public static object? EditorTerrainDetailsUI => GetEditorTerrainDetailsUI?.Invoke();

    /// <summary>
    /// Type of <see cref="SDG.Unturned.EditorTerrainTilesUI"/>.
    /// </summary>
    public static Type? EditorTerrainTilesUIType { get; }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.EditorTerrainTilesUI"/>.
    /// </summary>
    public static object? EditorTerrainTilesUI => GetEditorTerrainTilesUI?.Invoke();

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.EditorLevelUI"/>.
    /// </summary>
    public static EditorLevelUI? EditorLevelUI
    {
        get
        {
            EditorDashboardUI? dashboard = EditorDashboardUI;
            return dashboard == null ? null : GetEditorLevelUIInstance(dashboard);
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.EditorLevelObjectsUI"/>.
    /// </summary>
    public static EditorLevelObjectsUI? EditorLevelObjectsUI
    {
        get
        {
            EditorLevelUI? level = EditorLevelUI;
            return level == null ? null : GetEditorLevelObjectsUIInstance(level);
        }
    }

    /// <summary>
    /// Type of <see cref="SDG.Unturned.EditorVolumesUI"/>.
    /// </summary>
    public static Type? EditorVolumesUIType { get; }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.EditorVolumesUI"/>.
    /// </summary>
    public static object? EditorVolumesUI => GetEditorVolumesUI?.Invoke();

    /// <summary>
    /// Type of <see cref="SDG.Unturned.PlayerBrowserRequestUI"/>.
    /// </summary>
    public static Type? PlayerBrowserRequestUIType { get; } = typeof(Provider).Assembly.GetType("SDG.Unturned.PlayerBrowserRequestUI", false);

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.PlayerBrowserRequestUI"/>.
    /// </summary>
    public static object? PlayerBrowserRequestUI
    {
        get
        {
            PlayerUI? playerUI = PlayerUI;
            return playerUI != null ? GetPlayerBrowserRequestUI?.Invoke(playerUI) : null;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.PlayerBarricadeMannequinUI"/>.
    /// </summary>
    public static PlayerBarricadeStereoUI? PlayerBarricadeStereoUI
    {
        get
        {
            PlayerUI? playerUI = PlayerUI;
            return playerUI != null ? GetPlayerBarricadeStereoUI?.Invoke(playerUI) : null;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.PlayerBarricadeMannequinUI"/>.
    /// </summary>
    public static PlayerBarricadeMannequinUI? PlayerBarricadeMannequinUI
    {
        get
        {
            PlayerUI? playerUI = PlayerUI;
            return playerUI != null ? GetPlayerBarricadeMannequinUI?.Invoke(playerUI) : null;
        }
    }

    /// <summary>
    /// Type of <see cref="SDG.Unturned.PlayerGroupUI"/>.
    /// </summary>
    public static Type? PlayerGroupUIType { get; } = typeof(Provider).Assembly.GetType("SDG.Unturned.PlayerGroupUI", false);

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.PlayerGroupUI"/>.
    /// </summary>
    public static object? PlayerGroupUI
    {
        get
        {
            PlayerUI? playerUI = PlayerUI;
            return playerUI != null ? GetPlayerGroupUI?.Invoke(playerUI) : null;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.PlayerDashboardUI"/>.
    /// </summary>
    public static PlayerDashboardUI? PlayerDashboardUI
    {
        get
        {
            PlayerUI? playerUI = PlayerUI;
            return playerUI != null ? GetPlayerDashboardUI?.Invoke(playerUI) : null;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.PlayerPauseUI"/>.
    /// </summary>
    public static PlayerPauseUI? PlayerPauseUI
    {
        get
        {
            PlayerUI? playerUI = PlayerUI;
            return playerUI != null ? GetPlayerPauseUI?.Invoke(playerUI) : null;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.PlayerLifeUI"/>.
    /// </summary>
    public static PlayerLifeUI? PlayerLifeUI
    {
        get
        {
            PlayerUI? playerUI = PlayerUI;
            return playerUI != null ? GetPlayerLifeUI?.Invoke(playerUI) : null;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.PlayerDashboardInformationUI"/>.
    /// </summary>
    public static PlayerDashboardInformationUI? PlayerDashboardInformationUI
    {
        get
        {
            PlayerDashboardUI? playerDashboardUI = PlayerDashboardUI;
            return playerDashboardUI != null ? GetPlayerDashboardInformationUI?.Invoke(playerDashboardUI) : null;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.MenuDashboardUI"/>.
    /// </summary>
    public static MenuDashboardUI? MenuDashboardUI
    {
        get
        {
            MenuUI? menuUI = MenuUI;
            return menuUI != null ? GetMenuDashboardUI?.Invoke(menuUI) : null;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.MenuPauseUI"/>.
    /// </summary>
    public static MenuPauseUI? MenuPauseUI
    {
        get
        {
            MenuDashboardUI? menuDashboardUI = MenuDashboardUI;
            return menuDashboardUI != null ? GetMenuPauseUI?.Invoke(menuDashboardUI) : null;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.MenuCreditsUI"/>.
    /// </summary>
    public static MenuCreditsUI? MenuCreditsUI
    {
        get
        {
            MenuDashboardUI? menuDashboardUI = MenuDashboardUI;
            return menuDashboardUI != null ? GetMenuCreditsUI?.Invoke(menuDashboardUI) : null;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.MenuTitleUI"/>.
    /// </summary>
    public static MenuTitleUI? MenuTitleUI
    {
        get
        {
            MenuDashboardUI? menuDashboardUI = MenuDashboardUI;
            return menuDashboardUI != null ? GetMenuTitleUI?.Invoke(menuDashboardUI) : null;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.MenuPlayUI"/>.
    /// </summary>
    public static MenuPlayUI? MenuPlayUI
    {
        get
        {
            MenuDashboardUI? menuDashboardUI = MenuDashboardUI;
            return menuDashboardUI != null ? GetMenuPlayUI?.Invoke(menuDashboardUI) : null;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.MenuSurvivorsUI"/>.
    /// </summary>
    public static MenuSurvivorsUI? MenuSurvivorsUI
    {
        get
        {
            MenuDashboardUI? menuDashboardUI = MenuDashboardUI;
            return menuDashboardUI != null ? GetMenuSurvivorsUI?.Invoke(menuDashboardUI) : null;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.MenuConfigurationUI"/>.
    /// </summary>
    public static MenuConfigurationUI? MenuConfigurationUI
    {
        get
        {
            MenuDashboardUI? menuDashboardUI = MenuDashboardUI;
            return menuDashboardUI != null ? GetMenuConfigurationUI?.Invoke(menuDashboardUI) : null;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.MenuConfigurationAudioUI"/>.
    /// </summary>
    public static MenuConfigurationAudioUI? MenuConfigurationAudioUI
    {
        get
        {
            MenuConfigurationUI? menuConfigurationUI = MenuConfigurationUI;
            return menuConfigurationUI != null ? GetMenuConfigurationAudioUI?.Invoke(menuConfigurationUI) : null;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.MenuWorkshopUI"/>.
    /// </summary>
    public static MenuWorkshopUI? MenuWorkshopUI
    {
        get
        {
            MenuDashboardUI? menuDashboardUI = MenuDashboardUI;
            return menuDashboardUI != null ? GetMenuWorkshopUI?.Invoke(menuDashboardUI) : null;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.MenuPlayConnectUI"/>.
    /// </summary>
    public static MenuPlayConnectUI? MenuPlayConnectUI
    {
        get
        {
            MenuPlayUI? menuPlayUI = MenuPlayUI;
            return menuPlayUI != null ? GetMenuPlayConnectUI?.Invoke(menuPlayUI) : null;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.MenuPlayServersUI"/>.
    /// </summary>
    public static MenuPlayServersUI? MenuPlayServersUI => MenuPlayUI.serverListUI;

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.MenuPlayOnlineSafetyUI"/>.
    /// </summary>
    public static MenuPlayOnlineSafetyUI? MenuPlayOnlineSafetyUI => MenuPlayUI.onlineSafetyUI;

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.MenuPlayServerListFiltersUI"/>.
    /// </summary>
    public static MenuPlayServerListFiltersUI? MenuPlayServerListFiltersUI => MenuPlayServersUI.serverListFiltersUI;

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.MenuPlayServerBookmarksUI"/>.
    /// </summary>
    public static MenuPlayServerBookmarksUI? MenuPlayServerBookmarksUI => MenuPlayUI.serverBookmarksUI;

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.MenuPlayServerCurationUI"/>.
    /// </summary>
    public static MenuPlayServerCurationUI? MenuPlayServerCurationUI => MenuPlayServersUI.serverCurationUI;

    /// <summary>
    /// Type of <see cref="SDG.Unturned.MenuPlayServerCurationRulesUI"/>.
    /// </summary>
    public static Type? MenuPlayServerCurationRulesUIType { get; } = typeof(Provider).Assembly.GetType("SDG.Unturned.MenuPlayServerCurationRulesUI", false);

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.MenuPlayServerCurationUI"/>.
    /// </summary>
    public static object? MenuPlayServerCurationRulesUI
    {
        get
        {
            MenuPlayServerCurationUI? curationUI = MenuPlayServersUI.serverCurationUI;
            return curationUI != null ? GetMenuPlayServerCurationRulesUI?.Invoke(curationUI) : null;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.MenuPlayServerInfoUI"/>.
    /// </summary>
    public static MenuPlayServerInfoUI? MenuPlayServerInfoUI
    {
        get
        {
            MenuPlayUI? menuPlayUI = MenuPlayUI;
            return menuPlayUI != null ? GetMenuPlayServerInfoUI?.Invoke(menuPlayUI) : null;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.MenuPlaySingleplayerUI"/>.
    /// </summary>
    public static MenuPlaySingleplayerUI? MenuPlaySingleplayerUI
    {
        get
        {
            MenuPlayUI? menuPlayUI = MenuPlayUI;
            return menuPlayUI != null ? GetMenuPlaySingleplayerUI?.Invoke(menuPlayUI) : null;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.MenuPlayLobbiesUI"/>.
    /// </summary>
    public static MenuPlayLobbiesUI? MenuPlayLobbiesUI
    {
        get
        {
            MenuPlayUI? menuPlayUI = MenuPlayUI;
            return menuPlayUI != null ? GetMenuPlayLobbiesUI?.Invoke(menuPlayUI) : null;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.MenuServerPasswordUI"/>.
    /// </summary>
    public static MenuServerPasswordUI? MenuServerPasswordUI => GetMenuServerPasswordUI?.Invoke();

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.MenuSurvivorsCharacterUI"/>.
    /// </summary>
    public static MenuSurvivorsCharacterUI? MenuSurvivorsCharacterUI
    {
        get
        {
            MenuSurvivorsUI? menuSurvivorsUI = MenuSurvivorsUI;
            return menuSurvivorsUI != null ? GetMenuSurvivorsCharacterUI?.Invoke(menuSurvivorsUI) : null;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.MenuSurvivorsAppearanceUI"/>.
    /// </summary>
    public static MenuSurvivorsAppearanceUI? MenuSurvivorsAppearanceUI
    {
        get
        {
            MenuSurvivorsUI? menuSurvivorsUI = MenuSurvivorsUI;
            return menuSurvivorsUI != null ? GetMenuSurvivorsAppearanceUI?.Invoke(menuSurvivorsUI) : null;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.MenuSurvivorsGroupUI"/>.
    /// </summary>
    public static MenuSurvivorsGroupUI? MenuSurvivorsGroupUI
    {
        get
        {
            MenuSurvivorsUI? menuSurvivorsUI = MenuSurvivorsUI;
            return menuSurvivorsUI != null ? GetMenuSurvivorsGroupUI?.Invoke(menuSurvivorsUI) : null;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.MenuSurvivorsClothingUI"/>.
    /// </summary>
    public static MenuSurvivorsClothingUI? MenuSurvivorsClothingUI
    {
        get
        {
            MenuSurvivorsUI? menuSurvivorsUI = MenuSurvivorsUI;
            return menuSurvivorsUI != null ? GetMenuSurvivorsClothingUI?.Invoke(menuSurvivorsUI) : null;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.MenuSurvivorsClothingItemUI"/>.
    /// </summary>
    public static MenuSurvivorsClothingItemUI? MenuSurvivorsClothingItemUI
    {
        get
        {
            MenuSurvivorsClothingUI? menuSurvivorsClothingUI = MenuSurvivorsClothingUI;
            return menuSurvivorsClothingUI != null ? GetMenuSurvivorsClothingItemUI?.Invoke(menuSurvivorsClothingUI) : null;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.MenuSurvivorsClothingInspectUI"/>.
    /// </summary>
    public static MenuSurvivorsClothingInspectUI? MenuSurvivorsClothingInspectUI
    {
        get
        {
            MenuSurvivorsClothingUI? menuSurvivorsClothingUI = MenuSurvivorsClothingUI;
            return menuSurvivorsClothingUI != null ? GetMenuSurvivorsClothingInspectUI?.Invoke(menuSurvivorsClothingUI) : null;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.MenuSurvivorsClothingDeleteUI"/>.
    /// </summary>
    public static MenuSurvivorsClothingDeleteUI? MenuSurvivorsClothingDeleteUI
    {
        get
        {
            MenuSurvivorsClothingUI? menuSurvivorsClothingUI = MenuSurvivorsClothingUI;
            return menuSurvivorsClothingUI != null ? GetMenuSurvivorsClothingDeleteUI?.Invoke(menuSurvivorsClothingUI) : null;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.MenuSurvivorsClothingBoxUI"/>.
    /// </summary>
    public static MenuSurvivorsClothingBoxUI? MenuSurvivorsClothingBoxUI
    {
        get
        {
            MenuSurvivorsClothingUI? menuSurvivorsClothingUI = MenuSurvivorsClothingUI;
            return menuSurvivorsClothingUI != null ? GetMenuSurvivorsClothingBoxUI?.Invoke(menuSurvivorsClothingUI) : null;
        }
    }

    /// <summary>
    /// Type of <see cref="SDG.Unturned.ItemStoreCartMenu"/>.
    /// </summary>
    public static Type? ItemStoreMenuType { get; } = typeof(Provider).Assembly.GetType("SDG.Unturned.ItemStoreMenu", false);

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.ItemStoreMenu"/>.
    /// </summary>
    public static object? ItemStoreMenu
    {
        get
        {
            MenuSurvivorsClothingUI? menuSurvivorsClothingUI = MenuSurvivorsClothingUI;
            return menuSurvivorsClothingUI != null ? GetItemStoreMenu?.Invoke(menuSurvivorsClothingUI) : null;
        }
    }

    /// <summary>
    /// Type of <see cref="SDG.Unturned.ItemStoreCartMenu"/>.
    /// </summary>
    public static Type? ItemStoreCartMenuType { get; } = typeof(Provider).Assembly.GetType("SDG.Unturned.ItemStoreCartMenu", false);

    /// <summary>
    /// Type of <see cref="SDG.Unturned.ItemStoreDetailsMenu"/>.
    /// </summary>
    public static Type? ItemStoreDetailsMenuType { get; } = typeof(Provider).Assembly.GetType("SDG.Unturned.ItemStoreDetailsMenu", false);

    /// <summary>
    /// Type of <see cref="SDG.Unturned.ItemStoreBundleContentsMenu"/>.
    /// </summary>
    public static Type? ItemStoreBundleContentsMenuType { get; } = typeof(Provider).Assembly.GetType("SDG.Unturned.ItemStoreBundleContentsMenu", false);

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.ItemStoreCartMenu"/>.
    /// </summary>
    public static object? ItemStoreCartMenu => GetItemStoreCartMenu?.Invoke();

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.ItemStoreDetailsMenu"/>.
    /// </summary>
    public static object? ItemStoreDetailsMenu => GetItemStoreDetailsMenu?.Invoke();

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.ItemStoreBundleContentsMenu"/>.
    /// </summary>
    public static object? ItemStoreBundleContentsMenu => GetItemStoreBundleContentsMenu?.Invoke();

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.MenuWorkshopSubmitUI"/>.
    /// </summary>
    public static MenuWorkshopSubmitUI? MenuWorkshopSubmitUI
    {
        get
        {
            MenuWorkshopUI? menuWorkshopUI = MenuWorkshopUI;
            return menuWorkshopUI != null ? GetMenuWorkshopSubmitUI?.Invoke(menuWorkshopUI) : null;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.MenuWorkshopEditorUI"/>.
    /// </summary>
    public static MenuWorkshopEditorUI? MenuWorkshopEditorUI
    {
        get
        {
            MenuWorkshopUI? menuWorkshopUI = MenuWorkshopUI;
            return menuWorkshopUI != null ? GetMenuWorkshopEditorUI?.Invoke(menuWorkshopUI) : null;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.MenuWorkshopErrorUI"/>.
    /// </summary>
    public static MenuWorkshopErrorUI? MenuWorkshopErrorUI
    {
        get
        {
            MenuWorkshopUI? menuWorkshopUI = MenuWorkshopUI;
            return menuWorkshopUI != null ? GetMenuWorkshopErrorUI?.Invoke(menuWorkshopUI) : null;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.MenuWorkshopLocalizationUI"/>.
    /// </summary>
    public static MenuWorkshopLocalizationUI? MenuWorkshopLocalizationUI
    {
        get
        {
            MenuWorkshopUI? menuWorkshopUI = MenuWorkshopUI;
            return menuWorkshopUI != null ? GetMenuWorkshopLocalizationUI?.Invoke(menuWorkshopUI) : null;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.MenuWorkshopSpawnsUI"/>.
    /// </summary>
    public static MenuWorkshopSpawnsUI? MenuWorkshopSpawnsUI
    {
        get
        {
            MenuWorkshopUI? menuWorkshopUI = MenuWorkshopUI;
            return menuWorkshopUI != null ? GetMenuWorkshopSpawnsUI?.Invoke(menuWorkshopUI) : null;
        }
    }

    /// <summary>
    /// Singleton instance of <see cref="SDG.Unturned.MenuWorkshopSubscriptionsUI"/>.
    /// </summary>
    public static MenuWorkshopSubscriptionsUI? MenuWorkshopSubscriptionsUI
    {
        get
        {
            MenuWorkshopUI? menuWorkshopUI = MenuWorkshopUI;
            return menuWorkshopUI != null ? GetMenuWorkshopSubscriptionsUI?.Invoke(menuWorkshopUI) : null;
        }
    }

    private static Dictionary<Type, UITypeInfo> _typeInfoIntl = null!;
    private static LoadingUI? _loadingUI;

    /// <summary>
    /// Dictionary of vanilla UI types to their type info.
    /// </summary>
    public static IReadOnlyDictionary<Type, UITypeInfo> TypeInfo { get; private set; } = null!;


    /// <summary>
    /// Called on initialization to allow modules to add their UI types to the <see cref="TypeInfo"/> dictionary before it's finalized (allowing them to be extended).<br/>
    /// This needs to be subscribed to on plugin load for it to be called.
    /// </summary>
    /// <remarks>While removing or replacing info is possible, it's not recommended as it may mess with features of other modules.</remarks>
    public static event InitializingUIInfo? OnInitializingUIInfo;

    /// <summary>
    /// Get information about a vanilla UI type, or <see langword="null"/> if there's no information registered.
    /// </summary>
    public static UITypeInfo? GetTypeInfo(Type type) => _typeInfoIntl.TryGetValue(type, out UITypeInfo typeInfo) ? typeInfo : null;

    /// <summary>Gets the menu type from its parent <see cref="VolumeBase"/>.</summary>
    /// <remarks>Includes parent types.</remarks>
    public static bool TryGetVolumeMenuType<T>(out Type menuType) where T : VolumeBase => TryGetMenuType(typeof(T), out menuType);

    /// <summary>Gets the menu type from its parent <see cref="TempNodeBase"/>.</summary>
    /// <remarks>Includes parent types.</remarks>
    public static bool TryGetNodeMenuType<T>(out Type menuType) where T : TempNodeBase => TryGetMenuType(typeof(T), out menuType);

    /// <summary>Gets the menu type from its parent component. This currently only applies to <see cref="VolumeBase"/>'s and <see cref="TempNodeBase"/>'s.</summary>
    /// <remarks>Includes parent types.</remarks>
    public static bool TryGetMenuType(Type volumeType, out Type menuType)
    {
        Type? type2 = volumeType;
        for (; type2 != null; type2 = type2.BaseType)
        {
            if (MenuTypes.TryGetValue(volumeType, out menuType))
                return true;
        }

        menuType = null!;
        return false;
    }

    /// <remarks>Includes parent types.</remarks>
    public static bool TryGetUITypeInfo<T>(out UITypeInfo info) => TryGetUITypeInfo(typeof(T), out info);

    /// <remarks>Includes parent types.</remarks>
    public static bool TryGetUITypeInfo(Type uiType, out UITypeInfo info)
    {
        Type? type2 = uiType;
        for (; type2 != null; type2 = type2.BaseType)
        {
            if (TypeInfo.TryGetValue(uiType, out info))
                return true;
        }

        info = null!;
        return false;
    }

    internal static Type? FindUIType(string typeName)
    {
        if (typeName.IndexOf('.') != -1)
        {
            return typeName.IndexOf(',') != -1 ? Type.GetType(typeName, false, false) : typeof(Provider).Assembly.GetType(typeName, false, false);
        }

        return typeof(Provider).Assembly.GetType("SDG.Unturned." + typeName, false, false);
    }

    /// <summary>
    /// Read a UI type to an <see cref="ILGenerator"/>.
    /// </summary>
    public static void LoadUIToILGenerator(ILGenerator il, Type uiType)
    {
        UITypeInfo? info = GetTypeInfo(uiType);

        if (info == null)
            throw new ArgumentException($"{Accessor.ExceptionFormatter.Format(uiType)} is not a valid UI type. If it's new, request it on the GitHub.");

        if (info.IsStaticUI || string.IsNullOrEmpty(info.EmitProperty) && info.CustomEmitter == null)
            throw new InvalidOperationException($"{Accessor.ExceptionFormatter.Format(uiType)} is not an instanced UI.");

        if (info.CustomEmitter != null)
        {
            info.CustomEmitter(info, il);
            return;
        }

        try
        {
            PropertyInfo? property = typeof(UIAccessor).GetProperty(info.EmitProperty!, BindingFlags.Public | BindingFlags.Static);
            if (property != null)
            {
                MethodInfo? getter = property.GetGetMethod(true);
                if (getter != null && (uiType.IsAssignableFrom(getter.ReturnType) || getter.ReturnType == typeof(object)))
                {
                    il.Emit(getter.GetCallRuntime(), getter);
                    return;
                }
            }
        }
        catch (AmbiguousMatchException)
        {
            // ignored
        }

        throw new Exception($"Unable to find an emittable property at {Accessor.ExceptionFormatter.Format(typeof(UIAccessor))}.{info.EmitProperty}.");
    }

    /// <summary>
    /// Enumerate the <see cref="CodeInstruction"/>s to load a UI type.
    /// </summary>
    public static IEnumerable<CodeInstruction> EnumerateUIInstructions(Type uiType, ILGenerator il)
    {
        UITypeInfo? info = GetTypeInfo(uiType);

        if (info == null)
            throw new ArgumentException($"{Accessor.ExceptionFormatter.Format(uiType)} is not a valid UI type. If it's new, request it on the GitHub.");

        if (info.IsStaticUI || string.IsNullOrEmpty(info.EmitProperty) && info.CustomTranspiler == null)
            throw new InvalidOperationException($"{Accessor.ExceptionFormatter.Format(uiType)} is not an instanced UI.");

        if (info.CustomTranspiler != null)
        {
            foreach (CodeInstruction instr in info.CustomTranspiler(info, il))
                yield return instr;

            yield break;
        }

        MethodInfo? getter = null;
        try
        {
            PropertyInfo? property = typeof(UIAccessor).GetProperty(info.EmitProperty!, BindingFlags.Public | BindingFlags.Static);
            if (property != null)
            {
                MethodInfo? getter2 = property.GetGetMethod(true);
                if (getter != null && (uiType.IsAssignableFrom(getter.ReturnType) || getter.ReturnType == typeof(object)))
                {
                    getter = getter2;
                }
            }
        }
        catch (AmbiguousMatchException)
        {
            // ignored
        }

        if (getter != null)
        {
            yield return new CodeInstruction(getter.GetCallRuntime(), getter);
            yield break;
        }

        throw new Exception($"Unable to find an emittable property at {Accessor.ExceptionFormatter.Format(typeof(UIAccessor))}.{info.EmitProperty}.");
    }

    static UIAccessor()
    {
        lock (Sync)
        {
            try
            {
                _harmonyLogPath = Environment.GetEnvironmentVariable("HARMONY_LOG_FILE");
            }
            catch
            {
                // ignored
            }

            try
            {
                Type accessTools = typeof(UIAccessor);
                MethodInfo getEditorTerrainUI = accessTools.GetProperty(nameof(EditorTerrainUI), BindingFlags.Public | BindingFlags.Static)!.GetMethod;
                const MethodAttributes attr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;
                Assembly sdg = typeof(Provider).Assembly;

                /*
                 * TERRAIN
                 */
                Type containerType = typeof(EditorTerrainUI);

                /* HEIGHTS */
                Type? rtnType = sdg.GetType("SDG.Unturned.EditorTerrainHeightUI");
                EditorTerrainHeightUIType = rtnType;
                if (rtnType == null)
                {
                    Accessor.Logger?.LogWarning(Source, "Unable to find type: SDG.Unturned.EditorTerrainHeightUI.");
                }
                else
                {
                    FieldInfo? field = containerType.GetField("heightV2", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public) ??
                                       containerType.GetField("heights", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    if (field == null || field.IsStatic || !rtnType.IsAssignableFrom(field.FieldType))
                    {
                        Accessor.Logger?.LogWarning(Source, $"Unable to find field: {Accessor.Formatter.Format(new FieldDefinition("heightV2")
                            .DeclaredIn(containerType, isStatic: false)
                            .WithFieldType(rtnType)
                        )}.");
                    }
                    else
                    {
                        DynamicMethod method = new DynamicMethod("GetEditorTerrainHeightsUI_Impl", attr,
                            CallingConventions.Standard, rtnType,
                            Array.Empty<Type>(), accessTools, true);
                        ILGenerator il = method.GetILGenerator();
                        il.Emit(OpCodes.Call, getEditorTerrainUI);
                        il.Emit(OpCodes.Ldfld, field);
                        il.Emit(OpCodes.Ret);
                        GetEditorTerrainHeightUI = (Func<object?>)method.CreateDelegate(typeof(Func<object>));
                    }
                }

                /* MATERIALS */
                rtnType = sdg.GetType("SDG.Unturned.EditorTerrainMaterialsUI");
                EditorTerrainMaterialsUIType = rtnType;
                if (rtnType == null)
                {
                    Accessor.Logger?.LogWarning(Source, "Unable to find type: SDG.Unturned.EditorTerrainMaterialsUI.");
                }
                else
                {
                    FieldInfo? field = containerType.GetField("materialsV2", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public) ??
                                       containerType.GetField("materials", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    if (field == null || field.IsStatic || !rtnType.IsAssignableFrom(field.FieldType))
                    {
                        Accessor.Logger?.LogWarning(Source, $"Unable to find field: {Accessor.Formatter.Format(new FieldDefinition("materialsV2")
                            .DeclaredIn(containerType, isStatic: false)
                            .WithFieldType(rtnType)
                        )}.");
                    }
                    else
                    {
                        DynamicMethod method = new DynamicMethod("GetEditorTerrainMaterialsUI_Impl", attr,
                            CallingConventions.Standard, rtnType,
                            Array.Empty<Type>(), accessTools, true);
                        ILGenerator il = method.GetILGenerator();
                        il.Emit(OpCodes.Call, getEditorTerrainUI);
                        il.Emit(OpCodes.Ldfld, field);
                        il.Emit(OpCodes.Ret);
                        GetEditorTerrainMaterialsUI = (Func<object?>)method.CreateDelegate(typeof(Func<object>));
                    }
                }

                /* DETAILS */
                rtnType = sdg.GetType("SDG.Unturned.EditorTerrainDetailsUI");
                EditorTerrainDetailsUIType = rtnType;
                if (rtnType == null)
                {
                    Accessor.Logger?.LogWarning(Source, "Unable to find type: SDG.Unturned.EditorTerrainDetailsUI.");
                    return;
                }
                else
                {
                    FieldInfo? field = containerType.GetField("detailsV2", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public) ??
                                       containerType.GetField("details", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    if (field == null || field.IsStatic || !rtnType.IsAssignableFrom(field.FieldType))
                    {
                        Accessor.Logger?.LogWarning(Source, $"Unable to find field: {Accessor.Formatter.Format(new FieldDefinition("materialsV2")
                            .DeclaredIn(containerType, isStatic: false)
                            .WithFieldType(rtnType)
                        )}.");
                    }
                    else
                    {
                        DynamicMethod method = new DynamicMethod("GetEditorTerrainDetailsUI_Impl", attr,
                            CallingConventions.Standard, rtnType,
                            Array.Empty<Type>(), accessTools, true);
                        ILGenerator il = method.GetILGenerator();
                        il.Emit(OpCodes.Call, getEditorTerrainUI);
                        il.Emit(OpCodes.Ldfld, field);
                        il.Emit(OpCodes.Ret);
                        GetEditorTerrainDetailsUI = (Func<object?>)method.CreateDelegate(typeof(Func<object>));
                    }
                }

                /* TILES */
                rtnType = sdg.GetType("SDG.Unturned.EditorTerrainTilesUI");
                EditorTerrainTilesUIType = rtnType;
                if (rtnType == null)
                {
                    Accessor.Logger?.LogWarning(Source, "Unable to find type: SDG.Unturned.EditorTerrainTilesUI.");
                }
                else
                {
                    FieldInfo? field = containerType.GetField("tiles", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public) ??
                                       containerType.GetField("tilesV2", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    if (field == null || field.IsStatic || !rtnType.IsAssignableFrom(field.FieldType))
                    {
                        Accessor.Logger?.LogWarning(Source, $"Unable to find field: {Accessor.Formatter.Format(new FieldDefinition("tiles")
                            .DeclaredIn(containerType, isStatic: false)
                            .WithFieldType(rtnType)
                        )}.");
                    }
                    else
                    {
                        DynamicMethod method = new DynamicMethod("GetEditorTerrainTilesUI_Impl", attr,
                            CallingConventions.Standard, rtnType,
                            Array.Empty<Type>(), accessTools, true);
                        ILGenerator il = method.GetILGenerator();
                        il.Emit(OpCodes.Call, getEditorTerrainUI);
                        il.Emit(OpCodes.Ldfld, field);
                        il.Emit(OpCodes.Ret);
                        GetEditorTerrainTilesUI = (Func<object?>)method.CreateDelegate(typeof(Func<object>));
                    }
                }

                /*
                 * TERRAIN
                 */
                containerType = typeof(EditorEnvironmentUI);

                /* NODES */
                rtnType = sdg.GetType("SDG.Unturned.EditorEnvironmentNodesUI");
                EditorEnvironmentNodesUIType = rtnType;
                if (rtnType == null)
                {
                    Accessor.Logger?.LogWarning(Source, "Unable to find type: SDG.Unturned.EditorEnvironmentNodesUI.");
                }
                else
                {
                    FieldInfo? field = containerType.GetField("nodesUI", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public) ??
                                       containerType.GetField("nodes", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                    if (field == null || !field.IsStatic || !rtnType.IsAssignableFrom(field.FieldType))
                    {
                        Accessor.Logger?.LogWarning(Source, $"Unable to find field: {Accessor.Formatter.Format(new FieldDefinition("nodesUI")
                            .DeclaredIn(containerType, isStatic: false)
                            .WithFieldType(rtnType)
                        )}.");
                    }
                    else
                    {
                        DynamicMethod method = new DynamicMethod("GetEditorEnvironmentNodesUI_Impl", attr,
                            CallingConventions.Standard, rtnType,
                            Array.Empty<Type>(), accessTools, true);
                        ILGenerator il = method.GetILGenerator();
                        il.Emit(OpCodes.Ldsfld, field);
                        il.Emit(OpCodes.Ret);
                        GetEditorEnvironmentNodesUI = (Func<object?>)method.CreateDelegate(typeof(Func<object>));
                    }
                }

                /*
                 * LEVEL
                 */
                containerType = typeof(EditorLevelUI);

                /* VOLUMES */
                rtnType = sdg.GetType("SDG.Unturned.EditorVolumesUI");
                EditorVolumesUIType = rtnType;
                if (rtnType == null)
                {
                    Accessor.Logger?.LogWarning(Source, "Unable to find type: SDG.Unturned.EditorVolumesUI.");
                }
                else
                {
                    FieldInfo? field = containerType.GetField("volumesUI", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public) ??
                                       containerType.GetField("volumes", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                    if (field == null || !field.IsStatic || !rtnType.IsAssignableFrom(field.FieldType))
                    {
                        Accessor.Logger?.LogWarning(Source, $"Unable to find field: {Accessor.Formatter.Format(new FieldDefinition("volumesUI")
                            .DeclaredIn(containerType, isStatic: false)
                            .WithFieldType(rtnType)
                        )}.");
                    }
                    else
                    {
                        DynamicMethod method = new DynamicMethod("GetEditorVolumesUI_Impl", attr,
                            CallingConventions.Standard, rtnType,
                            Array.Empty<Type>(), accessTools, true);
                        ILGenerator il = method.GetILGenerator();
                        il.Emit(OpCodes.Ldsfld, field);
                        il.Emit(OpCodes.Ret);
                        GetEditorVolumesUI = (Func<object?>)method.CreateDelegate(typeof(Func<object>));
                    }
                }


                /*
                 * ITEM STORE
                 */
                containerType = ItemStoreMenuType!;
                if (containerType != null)
                {
                    /* CART MENU */
                    rtnType = ItemStoreCartMenuType;
                    if (rtnType != null)
                    {
                        FieldInfo? field = containerType.GetField("cartMenu", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public) ??
                                           containerType.GetField("cartMenuUI", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                        if (field == null || field.IsStatic || !rtnType.IsAssignableFrom(field.FieldType))
                        {
                            Accessor.Logger?.LogWarning(Source, $"Unable to find field: {Accessor.Formatter.Format(new FieldDefinition("cartMenu")
                                .DeclaredIn(containerType, isStatic: false)
                                .WithFieldType(rtnType)
                            )}.");
                        }
                        else
                        {
                            DynamicMethod method = new DynamicMethod("GetItemStoreCartMenu_Impl", attr,
                                CallingConventions.Standard, rtnType,
                                Array.Empty<Type>(), accessTools, true);
                            ILGenerator il = method.GetILGenerator();
                            Label lbl = il.DefineLabel();
                            il.Emit(OpCodes.Call, accessTools.GetProperty(nameof(ItemStoreMenu), BindingFlags.Public | BindingFlags.Static)!.GetMethod);
                            il.Emit(OpCodes.Dup);
                            il.Emit(OpCodes.Brfalse_S, lbl);
                            il.Emit(OpCodes.Ldfld, field);
                            il.MarkLabel(lbl);
                            il.Emit(OpCodes.Ret);
                            GetItemStoreCartMenu = (Func<object?>)method.CreateDelegate(typeof(Func<object>));
                        }
                    }
                    else
                    {
                        Accessor.Logger?.LogWarning(Source, "Unable to find type: SDG.Unturned.ItemStoreCartMenu.");
                    }

                    /* DETAILS MENU */
                    rtnType = ItemStoreDetailsMenuType;
                    if (rtnType != null)
                    {
                        FieldInfo? field = containerType.GetField("detailsMenu", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public) ??
                                           containerType.GetField("detailsMenuUI", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                        if (field == null || field.IsStatic || !rtnType.IsAssignableFrom(field.FieldType))
                        {
                            Accessor.Logger?.LogWarning(Source, $"Unable to find field: {Accessor.Formatter.Format(new FieldDefinition("detailsMenu")
                                .DeclaredIn(containerType, isStatic: false)
                                .WithFieldType(rtnType)
                            )}.");
                        }
                        else
                        {
                            DynamicMethod method = new DynamicMethod("GetItemStoreDetailsMenu_Impl", attr,
                                CallingConventions.Standard, rtnType,
                                Array.Empty<Type>(), accessTools, true);
                            ILGenerator il = method.GetILGenerator();
                            Label lbl = il.DefineLabel();
                            il.Emit(OpCodes.Call, accessTools.GetProperty(nameof(ItemStoreMenu), BindingFlags.Public | BindingFlags.Static)!.GetMethod);
                            il.Emit(OpCodes.Dup);
                            il.Emit(OpCodes.Brfalse_S, lbl);
                            il.Emit(OpCodes.Ldfld, field);
                            il.MarkLabel(lbl);
                            il.Emit(OpCodes.Ret);
                            GetItemStoreDetailsMenu = (Func<object?>)method.CreateDelegate(typeof(Func<object>));
                        }
                    }
                    else
                    {
                        Accessor.Logger?.LogWarning(Source, "Unable to find type: SDG.Unturned.ItemStoreCartMenu.");
                    }

                    /* BUNDLE CONTENTS MENU */
                    rtnType = ItemStoreBundleContentsMenuType;
                    if (rtnType != null)
                    {
                        FieldInfo? field = containerType.GetField("bundleContentsMenu", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public) ??
                                containerType.GetField("bundleContentsMenuUI", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                        if (field == null || field.IsStatic || !rtnType.IsAssignableFrom(field.FieldType))
                        {
                            Accessor.Logger?.LogWarning(Source, "Unable to find field: ItemStoreMenu.bundleContentsMenu.");
                        }
                        else
                        {
                            DynamicMethod method = new DynamicMethod("ItemStoreBundleContentsMenu_Impl", attr,
                                CallingConventions.Standard, rtnType,
                                Type.EmptyTypes, accessTools, true);
                            ILGenerator il = method.GetILGenerator();
                            Label lbl = il.DefineLabel();
                            il.Emit(OpCodes.Call, accessTools.GetProperty(nameof(ItemStoreMenu), BindingFlags.Public | BindingFlags.Static)!.GetMethod);
                            il.Emit(OpCodes.Dup);
                            il.Emit(OpCodes.Brfalse_S, lbl);
                            il.Emit(OpCodes.Ldfld, field);
                            il.MarkLabel(lbl);
                            il.Emit(OpCodes.Ret);
                            GetItemStoreBundleContentsMenu = (Func<object?>)method.CreateDelegate(typeof(Func<object>));
                        }
                    }
                    else
                    {
                        Accessor.Logger?.LogWarning(Source, "Unable to find type: SDG.Unturned.ItemStoreBundleContentsMenu.");
                    }
                }
                else
                {
                    Accessor.Logger?.LogWarning(Source, "Unable to find type: SDG.Unturned.ItemStoreMenu.");
                }
            }
            catch (Exception ex)
            {
                Accessor.Logger?.LogError(Source, ex, "Error initializing UI access tools.");
            }
        }
    }

    /// <summary>
    /// Initialize <see cref="TypeInfo"/> if it hasn't already been initialized. Calls <see cref="OnInitializingUIInfo"/>.
    /// </summary>
    public static void Init()
    {
        _ = Patcher;
        lock (Sync)
        {
            if (Interlocked.CompareExchange(ref _init, 1, 0) != 0)
                return;

            try
            {
                MethodBase[] emptyMethods = Array.Empty<MethodBase>();

                Dictionary<Type, UITypeInfo> typeInfo = new Dictionary<Type, UITypeInfo>(32);

                void Add(UITypeInfo type)
                {
                    if (type.Type == typeof(object))
                    {
                        Accessor.Logger?.LogError(Source, null, $"Missing UI: {type.ExpectedTypeName}.");
                        return;
                    }

                    if (type.Parent == typeof(object))
                    {
                        Accessor.Logger?.LogError(Source, null, $"Missing parent of UI: {Accessor.Formatter.Format(type.Type)}.");
                        return;
                    }

                    if (typeInfo.ContainsKey(type.Type))
                    {
                        Accessor.Logger?.LogError(Source, null, $"Duplicate UI: {Accessor.Formatter.Format(type.Type)}.");
                        return;
                    }

                    typeInfo.Add(type.Type, type);
                }

                Add(new UITypeInfo(nameof(SDG.Unturned.EditorDashboardUI), emptyMethods, emptyMethods, hasActiveMember: false)
                {
                    ParentName = nameof(SDG.Unturned.EditorUI),
                    Scene = UIScene.Editor,
                    EmitProperty = nameof(EditorDashboardUI),
                    OpenOnInitialize = true,
                    DefaultOpenState = true,
                    CloseOnDestroy = true
                });

                Add(new UITypeInfo(nameof(EditorEnvironmentLightingUI))
                {
                    ParentName = nameof(SDG.Unturned.EditorEnvironmentUI),
                    Scene = UIScene.Editor,
                    IsStaticUI = true
                });

                Add(new UITypeInfo(nameof(EditorEnvironmentNavigationUI))
                {
                    ParentName = nameof(SDG.Unturned.EditorEnvironmentUI),
                    Scene = UIScene.Editor,
                    IsStaticUI = true
                });

                Add(new UITypeInfo(nameof(EditorEnvironmentRoadsUI))
                {
                    ParentName = nameof(SDG.Unturned.EditorEnvironmentUI),
                    Scene = UIScene.Editor,
                    IsStaticUI = true
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.EditorEnvironmentUI))
                {
                    ParentName = nameof(SDG.Unturned.EditorDashboardUI),
                    Scene = UIScene.Editor,
                    EmitProperty = nameof(EditorEnvironmentUI)
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.EditorLevelObjectsUI))
                {
                    ParentName = nameof(SDG.Unturned.EditorLevelUI),
                    Scene = UIScene.Editor,
                    EmitProperty = nameof(EditorLevelObjectsUI)
                });

                Add(new UITypeInfo(nameof(EditorLevelPlayersUI))
                {
                    ParentName = nameof(SDG.Unturned.EditorLevelUI),
                    Scene = UIScene.Editor,
                    IsStaticUI = true
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.EditorLevelUI))
                {
                    ParentName = nameof(SDG.Unturned.EditorDashboardUI),
                    Scene = UIScene.Editor,
                    EmitProperty = nameof(EditorLevelUI)
                });

                Add(new UITypeInfo(nameof(EditorLevelVisibilityUI))
                {
                    ParentName = nameof(SDG.Unturned.EditorLevelUI),
                    Scene = UIScene.Editor,
                    IsStaticUI = true
                });

                Add(new UITypeInfo(nameof(EditorPauseUI))
                {
                    ParentName = nameof(SDG.Unturned.EditorDashboardUI),
                    Scene = UIScene.Editor,
                    IsStaticUI = true
                });

                Add(new UITypeInfo(nameof(EditorSpawnsAnimalsUI))
                {
                    ParentName = nameof(EditorSpawnsUI),
                    Scene = UIScene.Editor,
                    IsStaticUI = true
                });

                Add(new UITypeInfo(nameof(EditorSpawnsItemsUI))
                {
                    ParentName = nameof(EditorSpawnsUI),
                    Scene = UIScene.Editor,
                    IsStaticUI = true
                });

                Add(new UITypeInfo(nameof(EditorSpawnsUI))
                {
                    ParentName = nameof(SDG.Unturned.EditorDashboardUI),
                    Scene = UIScene.Editor,
                    IsStaticUI = true
                });

                Add(new UITypeInfo(nameof(EditorSpawnsVehiclesUI))
                {
                    ParentName = nameof(EditorSpawnsUI),
                    Scene = UIScene.Editor,
                    IsStaticUI = true
                });

                Add(new UITypeInfo(nameof(EditorSpawnsZombiesUI))
                {
                    ParentName = nameof(EditorSpawnsUI),
                    Scene = UIScene.Editor,
                    IsStaticUI = true
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.EditorTerrainUI))
                {
                    ParentName = nameof(SDG.Unturned.EditorDashboardUI),
                    Scene = UIScene.Editor,
                    EmitProperty = nameof(EditorTerrainUI)
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.EditorUI), emptyMethods, emptyMethods,
                    typeof(EditorUI).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance) is { } method1 ? [ method1 ] : emptyMethods,
                    hasActiveMember: false)
                {
                    Scene = UIScene.Editor,
                    EmitProperty = nameof(EditorUI),
                    OpenOnInitialize = true,
                    DefaultOpenState = true,
                    CloseOnDestroy = true,
                    IsActiveMember = FindUIType(nameof(SDG.Unturned.EditorUI))?.GetProperty(nameof(EditorUI.window), BindingFlags.Static | BindingFlags.Public)
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.LoadingUI), emptyMethods, emptyMethods, hasActiveMember: false)
                {
                    Scene = UIScene.Loading,
                    EmitProperty = nameof(LoadingUI),
                    OpenOnInitialize = true,
                    DefaultOpenState = true,
                    CloseOnDestroy = true,
                    IsActiveMember = FindUIType(nameof(SDG.Unturned.LoadingUI))?.GetProperty(nameof(LoadingUI.isBlocked), BindingFlags.Static | BindingFlags.Public)
                });

                Add(new UITypeInfo(nameof(MenuConfigurationControlsUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuConfigurationUI),
                    Scene = UIScene.Menu,
                    IsStaticUI = true
                });

                Add(new UITypeInfo(nameof(MenuConfigurationDisplayUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuConfigurationUI),
                    Scene = UIScene.Menu,
                    IsStaticUI = true
                });

                Add(new UITypeInfo(nameof(MenuConfigurationGraphicsUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuConfigurationUI),
                    Scene = UIScene.Menu,
                    IsStaticUI = true
                });

                Add(new UITypeInfo(nameof(MenuConfigurationOptionsUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuConfigurationUI),
                    Scene = UIScene.Menu,
                    IsStaticUI = true
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.MenuConfigurationAudioUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuConfigurationUI),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(MenuConfigurationAudioUI)
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.MenuConfigurationUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuDashboardUI),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(MenuConfigurationUI)
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.MenuCreditsUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuDashboardUI),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(MenuCreditsUI)
                });
                
                Add(new UITypeInfo(nameof(SDG.Unturned.MenuDashboardUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuUI),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(MenuDashboardUI),
                    DefaultOpenState = true
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.MenuPauseUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuDashboardUI),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(MenuPauseUI)
                });

                Add(new UITypeInfo(nameof(MenuPlayConfigUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuDashboardUI),
                    Scene = UIScene.Menu,
                    IsStaticUI = true
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.MenuPlayConnectUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuPlayUI),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(MenuPlayConnectUI)
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.MenuPlayLobbiesUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuPlayUI),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(MenuPlayLobbiesUI)
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.MenuPlayServerListFiltersUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuPlayServersUI),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(MenuPlayServerListFiltersUI)
                });

                Add(new UITypeInfo(nameof(MenuPlayServerCurationUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuPlayServersUI),
                    Scene = UIScene.Menu,
                    CustomEmitter = FindUIType(nameof(SDG.Unturned.MenuPlayServersUI))?.GetField("serverCurationUI", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) is { } field3
                                    ? (_, generator) => generator.Emit(OpCodes.Ldsfld, field3)
                                    : null
                });

                Add(new UITypeInfo("MenuPlayServerCurationRulesUI")
                {
                    ParentName = nameof(MenuPlayServerCurationUI),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(MenuPlayServerCurationRulesUI)
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.MenuPlayServerBookmarksUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuPlayUI),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(MenuPlayServerBookmarksUI)
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.MenuPlayServerInfoUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuPlayUI),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(MenuPlayServerInfoUI)
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.MenuPlayServersUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuPlayUI),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(MenuPlayServersUI)
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.MenuPlayOnlineSafetyUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuPlayUI),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(MenuPlayOnlineSafetyUI)
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.MenuPlaySingleplayerUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuPlayUI),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(MenuPlaySingleplayerUI)
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.MenuPlayUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuDashboardUI),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(MenuPlayUI)
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.MenuServerPasswordUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuPlayServerInfoUI),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(MenuServerPasswordUI)
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.MenuSurvivorsAppearanceUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuSurvivorsUI),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(MenuSurvivorsAppearanceUI)
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.MenuSurvivorsCharacterUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuSurvivorsUI),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(MenuSurvivorsCharacterUI)
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.MenuSurvivorsClothingBoxUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuSurvivorsClothingUI),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(MenuSurvivorsClothingBoxUI)
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.MenuSurvivorsClothingDeleteUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuSurvivorsClothingUI),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(MenuSurvivorsClothingDeleteUI)
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.MenuSurvivorsClothingInspectUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuSurvivorsClothingUI),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(MenuSurvivorsClothingInspectUI)
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.MenuSurvivorsClothingItemUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuSurvivorsClothingUI),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(MenuSurvivorsClothingItemUI)
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.MenuSurvivorsClothingUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuSurvivorsUI),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(MenuSurvivorsClothingUI)
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.MenuSurvivorsGroupUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuSurvivorsUI),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(MenuSurvivorsGroupUI)
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.MenuSurvivorsUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuDashboardUI),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(MenuSurvivorsUI)
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.MenuTitleUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuDashboardUI),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(MenuTitleUI)
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.MenuUI), emptyMethods, emptyMethods, hasActiveMember: false)
                {
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(MenuUI),
                    OpenOnInitialize = true,
                    DefaultOpenState = true,
                    CloseOnDestroy = true,
                    IsActiveMember = FindUIType(nameof(SDG.Unturned.MenuUI))?.GetProperty(nameof(MenuUI.window), BindingFlags.Static | BindingFlags.Public)
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.MenuWorkshopEditorUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuWorkshopUI),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(MenuWorkshopEditorUI)
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.MenuWorkshopErrorUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuWorkshopUI),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(MenuWorkshopErrorUI)
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.MenuWorkshopLocalizationUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuWorkshopUI),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(MenuWorkshopLocalizationUI)
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.MenuWorkshopSpawnsUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuWorkshopUI),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(MenuWorkshopSpawnsUI)
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.MenuWorkshopSubmitUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuWorkshopUI),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(MenuWorkshopSubmitUI)
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.MenuWorkshopSubscriptionsUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuWorkshopUI),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(MenuWorkshopSubscriptionsUI)
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.MenuWorkshopUI))
                {
                    ParentName = nameof(SDG.Unturned.MenuDashboardUI),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(MenuWorkshopUI)
                });

                Add(new UITypeInfo(typeof(PlayerBarricadeLibraryUI))
                {
                    ParentName = nameof(SDG.Unturned.PlayerUI),
                    Scene = UIScene.Player,
                    IsStaticUI = true
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.PlayerBarricadeMannequinUI))
                {
                    ParentName = nameof(SDG.Unturned.PlayerUI),
                    Scene = UIScene.Player,
                    EmitProperty = nameof(PlayerBarricadeMannequinUI)
                });

                Add(new UITypeInfo(nameof(PlayerBarricadeSignUI))
                {
                    ParentName = nameof(SDG.Unturned.PlayerUI),
                    Scene = UIScene.Player,
                    IsStaticUI = true
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.PlayerBarricadeStereoUI))
                {
                    ParentName = nameof(SDG.Unturned.PlayerUI),
                    Scene = UIScene.Player,
                    EmitProperty = nameof(PlayerBarricadeStereoUI)
                });

                Add(new UITypeInfo(nameof(PlayerDashboardCraftingUI))
                {
                    ParentName = nameof(SDG.Unturned.PlayerDashboardUI),
                    Scene = UIScene.Player,
                    IsStaticUI = true
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.PlayerDashboardInformationUI))
                {
                    ParentName = nameof(SDG.Unturned.PlayerDashboardUI),
                    Scene = UIScene.Player,
                    EmitProperty = nameof(PlayerDashboardInformationUI)
                });

                Add(new UITypeInfo(nameof(PlayerDashboardInventoryUI))
                {
                    ParentName = nameof(SDG.Unturned.PlayerDashboardUI),
                    Scene = UIScene.Player,
                    IsStaticUI = true
                });

                Add(new UITypeInfo(nameof(PlayerDashboardSkillsUI))
                {
                    ParentName = nameof(SDG.Unturned.PlayerDashboardUI),
                    Scene = UIScene.Player,
                    IsStaticUI = true
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.PlayerDashboardUI))
                {
                    ParentName = nameof(SDG.Unturned.PlayerUI),
                    Scene = UIScene.Player,
                    EmitProperty = nameof(PlayerDashboardUI)
                });

                Add(new UITypeInfo(nameof(PlayerDeathUI))
                {
                    ParentName = nameof(SDG.Unturned.PlayerUI),
                    Scene = UIScene.Player,
                    IsStaticUI = true
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.PlayerLifeUI))
                {
                    ParentName = nameof(SDG.Unturned.PlayerUI),
                    Scene = UIScene.Player,
                    EmitProperty = nameof(PlayerLifeUI)
                });

                Add(new UITypeInfo(nameof(PlayerNPCDialogueUI))
                {
                    ParentName = nameof(SDG.Unturned.PlayerUI),
                    Scene = UIScene.Player,
                    IsStaticUI = true
                });

                Add(new UITypeInfo(nameof(PlayerNPCQuestUI))
                {
                    ParentName = nameof(SDG.Unturned.PlayerUI),
                    Scene = UIScene.Player,
                    IsStaticUI = true
                });

                Add(new UITypeInfo(nameof(PlayerNPCVendorUI))
                {
                    ParentName = nameof(SDG.Unturned.PlayerUI),
                    Scene = UIScene.Player,
                    IsStaticUI = true
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.PlayerPauseUI))
                {
                    ParentName = nameof(SDG.Unturned.PlayerUI),
                    Scene = UIScene.Player,
                    EmitProperty = nameof(PlayerPauseUI)
                });

                Add(new UITypeInfo(nameof(SDG.Unturned.PlayerUI), emptyMethods, emptyMethods,
                    typeof(PlayerUI).GetMethod("InitializePlayer", BindingFlags.NonPublic | BindingFlags.Instance) is { } method3 ? [ method3 ] : emptyMethods
                    , hasActiveMember: false)
                {
                    Scene = UIScene.Player,
                    EmitProperty = nameof(PlayerUI),
                    OpenOnInitialize = true,
                    DefaultOpenState = true,
                    CloseOnDestroy = true,
                    IsActiveMember = FindUIType(nameof(SDG.Unturned.PlayerUI))?.GetProperty(nameof(PlayerUI.window), BindingFlags.Static | BindingFlags.Public)
                });

                Add(new UITypeInfo(nameof(PlayerWorkzoneUI))
                {
                    ParentName = nameof(SDG.Unturned.PlayerUI),
                    Scene = UIScene.Player,
                    IsStaticUI = true
                });

                Add(new UITypeInfo("EditorEnvironmentNodesUI", hasActiveMember: false)
                {
                    ParentName = nameof(SDG.Unturned.EditorEnvironmentUI),
                    Scene = UIScene.Editor,
                    EmitProperty = nameof(EditorEnvironmentNodesUI)
                });

                Add(new UITypeInfo("EditorVolumesUI", hasActiveMember: false)
                {
                    ParentName = nameof(SDG.Unturned.EditorLevelUI),
                    Scene = UIScene.Editor,
                    EmitProperty = nameof(EditorVolumesUI)
                });

                Add(new UITypeInfo("EditorTerrainHeightUI", hasActiveMember: false)
                {
                    ParentName = nameof(SDG.Unturned.EditorTerrainUI),
                    Scene = UIScene.Editor,
                    EmitProperty = nameof(EditorTerrainHeightUI),
                    DestroyWhenParentDestroys = true
                });

                Add(new UITypeInfo("EditorTerrainMaterialsUI", hasActiveMember: false)
                {
                    ParentName = nameof(SDG.Unturned.EditorTerrainUI),
                    Scene = UIScene.Editor,
                    EmitProperty = nameof(EditorTerrainMaterialsUI),
                    DestroyWhenParentDestroys = true
                });

                Add(new UITypeInfo("EditorTerrainDetailsUI", hasActiveMember: false)
                {
                    ParentName = nameof(SDG.Unturned.EditorTerrainUI),
                    Scene = UIScene.Editor,
                    EmitProperty = nameof(EditorTerrainDetailsUI),
                    DestroyWhenParentDestroys = true
                });

                Add(new UITypeInfo("EditorTerrainTilesUI", hasActiveMember: false)
                {
                    ParentName = nameof(SDG.Unturned.EditorTerrainUI),
                    Scene = UIScene.Editor,
                    EmitProperty = nameof(EditorTerrainTilesUI),
                    DestroyWhenParentDestroys = true
                });

                Add(new UITypeInfo("PlayerBrowserRequestUI")
                {
                    ParentName = nameof(SDG.Unturned.PlayerUI),
                    Scene = UIScene.Player,
                    EmitProperty = nameof(PlayerBrowserRequestUI)
                });

                Add(new UITypeInfo("PlayerGroupUI", emptyMethods, emptyMethods, hasActiveMember: false)
                {
                    ParentName = nameof(SDG.Unturned.PlayerUI),
                    Scene = UIScene.Player,
                    EmitProperty = nameof(PlayerGroupUI),
                    OpenOnInitialize = true,
                    DefaultOpenState = true,
                    CloseOnDestroy = true
                });

                Add(new UITypeInfo("ItemStoreMenu", hasActiveMember: false)
                {
                    ParentName = nameof(SDG.Unturned.MenuSurvivorsClothingUI),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(ItemStoreMenu)
                });

                Add(new UITypeInfo("ItemStoreCartMenu", hasActiveMember: false)
                {
                    Parent = ItemStoreMenuType ?? typeof(object),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(ItemStoreCartMenu)
                });

                Add(new UITypeInfo("ItemStoreDetailsMenu", hasActiveMember: false)
                {
                    Parent = ItemStoreMenuType ?? typeof(object),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(ItemStoreDetailsMenu)
                });

                Add(new UITypeInfo("ItemStoreBundleContentsMenu", hasActiveMember: false)
                {
                    Parent = ItemStoreMenuType ?? typeof(object),
                    Scene = UIScene.Menu,
                    EmitProperty = nameof(ItemStoreBundleContentsMenu)
                });


                try
                {
                    CustomVolumeMenuHandler volumeHandler = new CustomVolumeMenuHandler();
                    CustomNodeMenuHandler nodeHandler = new CustomNodeMenuHandler();
                    CustomSleekWrapperDestroyHandler sleekWrapperHandler = new CustomSleekWrapperDestroyHandler();

                    List<Type> types = Accessor.GetTypesSafe(typeof(Provider).Assembly);
                    types.AddRange(Accessor.GetTypesSafe(typeof(ISleekElement).Assembly));

                    foreach (Type menuType in types
                                 .Where(x =>
                                     x.Name.Equals("Menu", StringComparison.Ordinal) &&
                                     x.DeclaringType != null &&
                                     typeof(SleekWrapper).IsAssignableFrom(x)))
                    {
                        MenuTypes[menuType.DeclaringType] = menuType;
                        if (typeof(VolumeBase).IsAssignableFrom(menuType.DeclaringType) && EditorVolumesUIType != null)
                        {
                            typeInfo[menuType] = new UITypeInfo(menuType, emptyMethods, emptyMethods, hasActiveMember: false)
                            {
                                Parent = EditorVolumesUIType,
                                Scene = UIScene.Editor,
                                CustomEmitter = EmitVolumeMenu,
                                CustomTranspiler = EnumerateVolumeMenu,
                                CustomOnClose = volumeHandler,
                                CustomOnOpen = volumeHandler,
                                CustomOnDestroy = sleekWrapperHandler
                            };
                        }
                        else if (typeof(TempNodeBase).IsAssignableFrom(menuType.DeclaringType) && EditorEnvironmentNodesUIType != null)
                        {
                            typeInfo[menuType] = new UITypeInfo(menuType, emptyMethods, emptyMethods, hasActiveMember: false)
                            {
                                Parent = EditorEnvironmentNodesUIType,
                                Scene = UIScene.Editor,
                                CustomEmitter = EmitNodeMenu,
                                CustomTranspiler = EnumerateNodeMenu,
                                CustomOnClose = nodeHandler,
                                CustomOnOpen = nodeHandler,
                                CustomOnDestroy = sleekWrapperHandler
                            };
                        }
                    }
                    Accessor.Logger?.LogInfo(Source, $"Discovered {MenuTypes.Count} editor node/volume menu type(s).");
                    int c = typeInfo.Count;
                    foreach (Type sleekWrapper in types.Where(x => typeof(SleekWrapper).IsAssignableFrom(x)))
                    {
                        if (typeInfo.ContainsKey(sleekWrapper))
                            continue;

                        typeInfo.Add(sleekWrapper, new UITypeInfo(sleekWrapper, emptyMethods, emptyMethods, hasActiveMember: false)
                        {
                            Parent = null,
                            Scene = UIScene.Global,
                            IsInstanceUI = false,
                            DefaultOpenState = true,
                            OpenOnInitialize = true,
                            CustomOnDestroy = sleekWrapperHandler,
                            CloseOnDestroy = true
                        });
                    }
                    Accessor.Logger?.LogInfo(Source, $"Discovered {typeInfo.Count - c} UI wrapper type(s).");
                    c = typeInfo.Count;
                    Type? playerUi = FindUIType(nameof(SDG.Unturned.PlayerUI));
                    if (playerUi != null)
                    {
                        CustomUseableHandler useableHandler = new CustomUseableHandler();
                        foreach (Type useable in types.Where(x => typeof(Useable).IsAssignableFrom(x)))
                        {
                            if (typeInfo.ContainsKey(useable))
                                continue;
                            FieldInfo[] fields = useable.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                            PropertyInfo[] properties = useable.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                            if (!fields.Any(x => typeof(ISleekElement).IsAssignableFrom(x.FieldType) && !properties.Any(x => typeof(ISleekElement).IsAssignableFrom(x.PropertyType))))
                                continue;

                            typeInfo.Add(useable, new UITypeInfo(useable, emptyMethods, emptyMethods, emptyMethods, hasActiveMember: false)
                            {
                                Parent = playerUi,
                                Scene = UIScene.Player,
                                CustomEmitter = EmitUseable,
                                CustomTranspiler = EnumerateUseable,
                                CloseOnDestroy = true,
                                OpenOnInitialize = true,
                                CustomOnInitialize = useableHandler,
                                CustomOnDestroy = useableHandler
                            });
                        }
                    }
                    Accessor.Logger?.LogInfo(Source, $"Discovered {typeInfo.Count - c} useable UI type(s).");
                }
                catch (Exception ex)
                {
                    Accessor.Logger?.LogError(Source, ex, "Error initializing volume and node UI info.");
                }

                int ct = typeInfo.Count;

                if (OnInitializingUIInfo != null)
                {
                    Delegate[] delegates = OnInitializingUIInfo.GetInvocationList();
                    foreach (InitializingUIInfo ftn in delegates.OfType<InitializingUIInfo>())
                    {
                        try
                        {
                            ftn(typeInfo);
                        }
                        catch (Exception ex)
                        {
                            MethodInfo? method = Accessor.GetMethod(ftn);
                            if (method != null)
                                Accessor.Logger?.LogError(Source, ex, $"{Accessor.Formatter.Format(method)} threw an unhandled exception while invoking {Accessor.Formatter.Format(typeof(UIAccessor))}.{nameof(OnInitializingUIInfo)}.");
                            else
                                Accessor.Logger?.LogError(Source, ex, $"Unhandled exception invoking {Accessor.Formatter.Format(typeof(UIAccessor))}.{nameof(OnInitializingUIInfo)}.");
                        }
                    }
                }

                ct = typeInfo.Count - ct;
                if (ct > 0)
                    Accessor.Logger?.LogInfo(Source, $"Modules added {ct} UI type(s).");

                _typeInfoIntl = typeInfo;
                Accessor.Logger?.LogInfo(Source, $"Registered {typeInfo.Count} UI type(s).");
                TypeInfo = new ReadOnlyDictionary<Type, UITypeInfo>(typeInfo);
            }
            catch (Exception ex)
            {
                Accessor.Logger?.LogError(Source, ex, $"Error initializing {Accessor.Formatter.Format(typeof(UITypeInfo))} records.");
            }
        }
    }

    private static void EmitVolumeMenu(UITypeInfo info, ILGenerator generator)
    {
        FieldInfo? field = EditorVolumesUIType?.GetField("focusedItemMenu", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        if (field == null)
        {
            throw new MemberAccessException($"Unable to find field: {Accessor.Formatter.Format(new FieldDefinition("focusedItemMenu")
                .DeclaredIn(EditorVolumesUIType!, isStatic: false)
                .WithFieldType<ISleekElement>()
            )}.");
        }

        Label lbl = generator.DefineLabel();
        generator.Emit(OpCodes.Call, typeof(UIAccessor).GetProperty(nameof(EditorVolumesUI), BindingFlags.Public | BindingFlags.Static)!.GetMethod);
        generator.Emit(OpCodes.Dup);
        generator.Emit(OpCodes.Brfalse, lbl);
        generator.Emit(OpCodes.Ldfld, field);
        generator.Emit(OpCodes.Isinst, info.Type);
        generator.MarkLabel(lbl);
    }
    private static void EmitNodeMenu(UITypeInfo info, ILGenerator generator)
    {
        FieldInfo? field = EditorEnvironmentNodesUIType?.GetField("focusedItemMenu", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        if (field == null)
        {
            throw new MemberAccessException($"Unable to find field: {Accessor.Formatter.Format(new FieldDefinition("focusedItemMenu")
                .DeclaredIn(EditorEnvironmentNodesUIType!, isStatic: false)
                .WithFieldType<ISleekElement>()
            )}.");
        }

        Label lbl = generator.DefineLabel();
        generator.Emit(OpCodes.Call, typeof(UIAccessor).GetProperty(nameof(EditorEnvironmentNodesUI), BindingFlags.Public | BindingFlags.Static)!.GetMethod);
        generator.Emit(OpCodes.Dup);
        generator.Emit(OpCodes.Brfalse, lbl);
        generator.Emit(OpCodes.Ldfld, field);
        generator.Emit(OpCodes.Isinst, info.Type);
        generator.MarkLabel(lbl);
    }
    private static void EmitUseable(UITypeInfo info, ILGenerator generator)
    {
        MethodInfo? playerProp = typeof(Player).GetProperty(nameof(Player.player), BindingFlags.Public | BindingFlags.Static)?.GetGetMethod(true);
        MethodInfo? playerEquipmentProp = playerProp == null ? null : typeof(Player).GetProperty(nameof(Player.equipment), BindingFlags.Public | BindingFlags.Instance)?.GetGetMethod(true);
        MethodInfo? useableProp = playerEquipmentProp == null ? null : typeof(PlayerEquipment).GetProperty(nameof(PlayerEquipment.useable), BindingFlags.Public | BindingFlags.Instance)?.GetGetMethod(true);
        if (useableProp == null)
        {
            if (playerProp == null)
            {
                throw new MemberAccessException($"Unable to find property {Accessor.Formatter.Format(new PropertyDefinition("player")
                    .DeclaredIn<Player>(isStatic: true)
                    .WithPropertyType<Player>()
                    .WithNoSetter()
                )}.");
            }

            if (playerEquipmentProp == null)
            {
                throw new MemberAccessException($"Unable to find property {Accessor.Formatter.Format(new PropertyDefinition("equipment")
                    .DeclaredIn<Player>(isStatic: false)
                    .WithPropertyType<PlayerEquipment>()
                    .WithNoSetter()
                )}.");
            }

            throw new MemberAccessException($"Unable to find property {Accessor.Formatter.Format(new PropertyDefinition("useable")
                .DeclaredIn<PlayerEquipment>(isStatic: false)
                .WithPropertyType<Useable>()
                .WithNoSetter()
            )}.");
        }
            
        Label lbl = generator.DefineLabel();
        generator.Emit(OpCodes.Call, playerProp!);
        generator.Emit(OpCodes.Dup);
        generator.Emit(OpCodes.Brfalse, lbl);
        generator.Emit(playerEquipmentProp!.GetCall(), playerEquipmentProp!);
        generator.Emit(useableProp.GetCall(), useableProp);
        generator.Emit(OpCodes.Isinst, info.Type);
        generator.MarkLabel(lbl);
    }
    private static IEnumerable<CodeInstruction> EnumerateVolumeMenu(UITypeInfo info, ILGenerator generator)
    {
        FieldInfo? field = EditorVolumesUIType?.GetField("focusedItemMenu", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        if (field == null)
        {
            throw new MemberAccessException($"Unable to find field: {Accessor.Formatter.Format(new FieldDefinition("focusedItemMenu")
                .DeclaredIn(EditorEnvironmentNodesUIType!, isStatic: false)
                .WithFieldType<ISleekElement>()
            )}.");
        }

        Label lbl = generator.DefineLabel();

        return
        [
            new CodeInstruction(OpCodes.Call, typeof(UIAccessor).GetProperty(nameof(EditorVolumesUI), BindingFlags.Public | BindingFlags.Static)!.GetMethod),
            new CodeInstruction(OpCodes.Dup),
            new CodeInstruction(OpCodes.Brfalse, lbl),
            new CodeInstruction(OpCodes.Ldfld, field),
            new CodeInstruction(OpCodes.Isinst, info.Type),

            new CodeInstruction(OpCodes.Nop).WithLabels(lbl)
        ];
    }
    private static IEnumerable<CodeInstruction> EnumerateNodeMenu(UITypeInfo info, ILGenerator generator)
    {
        FieldInfo? field = EditorEnvironmentNodesUIType?.GetField("focusedItemMenu", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        if (field == null)
        {
            throw new MemberAccessException($"Unable to find field: {Accessor.Formatter.Format(new FieldDefinition("focusedItemMenu")
                .DeclaredIn(EditorEnvironmentNodesUIType!, isStatic: false)
                .WithFieldType<ISleekElement>()
            )}.");
        }

        Label lbl = generator.DefineLabel();

        return
        [
            new CodeInstruction(OpCodes.Call, typeof(UIAccessor).GetProperty(nameof(EditorEnvironmentNodesUI), BindingFlags.Public | BindingFlags.Static)!.GetMethod),
            new CodeInstruction(OpCodes.Dup),
            new CodeInstruction(OpCodes.Brfalse, lbl),
            new CodeInstruction(OpCodes.Ldfld, field),
            new CodeInstruction(OpCodes.Isinst, info.Type),

            new CodeInstruction(OpCodes.Nop).WithLabels(lbl)
        ];
    }
    private static IEnumerable<CodeInstruction> EnumerateUseable(UITypeInfo info, ILGenerator generator)
    {
        MethodInfo? playerProp = typeof(Player).GetProperty(nameof(Player.player), BindingFlags.Public | BindingFlags.Static)?.GetGetMethod(true);
        MethodInfo? playerEquipmentProp = playerProp == null ? null : typeof(Player).GetProperty(nameof(Player.equipment), BindingFlags.Public | BindingFlags.Instance)?.GetGetMethod(true);
        MethodInfo? useableProp = playerEquipmentProp == null ? null : typeof(PlayerEquipment).GetProperty(nameof(PlayerEquipment.useable), BindingFlags.Public | BindingFlags.Instance)?.GetGetMethod(true);
        if (useableProp == null)
        {
            if (playerProp == null)
            {
                throw new MemberAccessException($"Unable to find property {Accessor.Formatter.Format(new PropertyDefinition("player")
                    .DeclaredIn<Player>(isStatic: true)
                    .WithPropertyType<Player>()
                    .WithNoSetter()
                )}.");
            }

            if (playerEquipmentProp == null)
            {
                throw new MemberAccessException($"Unable to find property {Accessor.Formatter.Format(new PropertyDefinition("equipment")
                    .DeclaredIn<Player>(isStatic: false)
                    .WithPropertyType<PlayerEquipment>()
                    .WithNoSetter()
                )}.");
            }

            throw new MemberAccessException($"Unable to find property {Accessor.Formatter.Format(new PropertyDefinition("useable")
                .DeclaredIn<PlayerEquipment>(isStatic: false)
                .WithPropertyType<Useable>()
                .WithNoSetter()
            )}.");
        }

        Label lbl = generator.DefineLabel();

        return
        [
            new CodeInstruction(playerProp!.GetCall(), playerProp!),
            new CodeInstruction(OpCodes.Dup),
            new CodeInstruction(OpCodes.Brfalse, lbl),
            new CodeInstruction(playerEquipmentProp!.GetCall(), playerEquipmentProp!),
            new CodeInstruction(useableProp.GetCall(), useableProp),
            new CodeInstruction(OpCodes.Isinst, info.Type),

            new CodeInstruction(OpCodes.Nop).WithLabels(lbl)
        ];
    }

    [HarmonyPatch(typeof(EditorUI), "Start")]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void EditorUIStartPostfix()
    {
        EditorUIReady?.Invoke();
        Accessor.Logger?.LogInfo(Source, "Editor UI ready.");
    }

    [HarmonyPatch(typeof(PlayerUI), "InitializePlayer")]
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void PlayerUIStartPostfix()
    {
        PlayerUIReady?.Invoke();
        Accessor.Logger?.LogInfo(Source, "Player UI ready.");
    }
}

/// <summary>
/// Handler for <see cref="UIAccessor.OnInitializingUIInfo"/>,
/// </summary>
/// <param name="typeInfo">Mutable dictionary of UI types.</param>
public delegate void InitializingUIInfo(Dictionary<Type, UITypeInfo> typeInfo);