using System;
using System.Linq;
using System.Runtime.CompilerServices;
using HarmonyLib;
using JetBrains.Annotations;
using Zenject;

namespace Reactive.BeatSaber;

[PublicAPI]
[HarmonyPatch]
public static class BeatSaberUtils {
    #region FPFC

    public static bool UsesFPFC {
        get {
            _usesFpfc ??= Environment.GetCommandLineArgs().Contains("fpfc");
            return _usesFpfc.Value;
        }
    }

    private static bool? _usesFpfc;

    #endregion

    #region Zenject

#if !COMPILE_EDITOR

    public static DiContainer MenuContainer => _menuContainer ?? throw EarlyInitException();
    public static DiContainer AppContainer => _appContainer ?? throw EarlyInitException();

    private static DiContainer? _menuContainer;
    private static DiContainer? _appContainer;

    [HarmonyPatch(typeof(MainSettingsMenuViewControllersInstaller), "InstallBindings")]
    [HarmonyPostfix]
    private static void MenuInstallerPostfix(MainSettingsMenuViewControllersInstaller __instance) {
        _menuContainer = __instance.Container;
    }
    
    [HarmonyPatch(typeof(PCAppInit), "InstallBindings")]
    [HarmonyPostfix]
    private static void AppInstallerPostfix(PCAppInit __instance) {
        _appContainer = __instance.Container;
    }

    private static Exception EarlyInitException([CallerMemberName] string? name = null) {
        return new UninitializedComponentException($"{name} was not initialized");
    }

#endif

    #endregion
}