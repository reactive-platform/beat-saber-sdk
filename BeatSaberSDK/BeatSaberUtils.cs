using System;
using System.Linq;
using System.Runtime.CompilerServices;
using HarmonyLib;
using JetBrains.Annotations;
using Zenject;

namespace Reactive.BeatSaber;

[HarmonyPatch(typeof(MainSettingsMenuViewControllersInstaller), "InstallBindings")]
public static class MenuInstallerPatch {
    [UsedImplicitly]
    // ReSharper disable once InconsistentNaming
    private static void Postfix(MainSettingsMenuViewControllersInstaller __instance) {
        BeatSaberUtils._menuContainer = __instance.Container;
        Console.WriteLine("MainSettingsMenuViewControllersInstaller");
    }
}

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

    public static DiContainer? _menuContainer;
    public static DiContainer? _appContainer;
    
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