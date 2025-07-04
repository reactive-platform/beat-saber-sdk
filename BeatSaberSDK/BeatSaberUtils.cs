using System;
using System.Linq;
using System.Runtime.CompilerServices;
using HarmonyLib;
using HMUI;
using IPA.Utilities;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using VRUIControls;
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

    #region Canvas

    /// <summary>
    /// Sets the component up as a canvas floating around.
    /// </summary>
    /// <param name="component">A component to set up.</param>
    public static void Make3DScreen(IReactiveComponent component) {
        Make3DScreen(component.Content);
    }

    /// <summary>
    /// Sets the object up as a canvas floating around.  
    /// </summary>
    /// <param name="content">A game object to set up.</param>
    public static void Make3DScreen(GameObject content) {
        AddCanvas(content);
        Make3DRaycastable(content);

        content.transform.localScale = Vector3.one * 0.02f;
    }

    /// <summary>
    /// Makes the object with a canvas raycastable for a 3D pointer.
    /// </summary>
    /// <param name="content">A game object to set up.</param>
    public static void Make3DRaycastable(GameObject content) {
        var raycaster = content.AddComponent<VRGraphicRaycaster>();
        MenuContainer.Inject(raycaster);
    }

    /// <summary>
    /// Adds a game-configured canvas with the specified params to the reactive component.
    /// </summary>
    /// <param name="content">A reactive component to add to.</param>
    public static void AddCanvas(IReactiveComponent component) {
        AddCanvas(component, 3000, out _, out _);
    }

    /// <summary>
    /// Adds a game-configured canvas with the specified params to the reactive component.
    /// </summary>
    /// <param name="content">A reactive component to add to.</param>
    /// <param name="sortingOrder">A sorting order.</param>
    public static void AddCanvas(IReactiveComponent component, int sortingOrder, out Canvas canvas, out CanvasScaler scaler) {
        AddCanvas(component.Content, sortingOrder, out canvas, out scaler);
    }

    /// <summary>
    /// Adds a game-configured canvas with the specified params to the game object.
    /// </summary>
    /// <param name="content">A game object to add to.</param>
    public static void AddCanvas(GameObject content) {
        AddCanvas(content, 3000, out _, out _);
    }

    /// <summary>
    /// Adds a game-configured canvas with the specified params to the game object.
    /// </summary>
    /// <param name="content">A game object to add to.</param>
    /// <param name="sortingOrder">A sorting order.</param>
    public static void AddCanvas(GameObject content, int sortingOrder, out Canvas canvas, out CanvasScaler scaler) {
        canvas = content.AddComponent<Canvas>();
        canvas.sortingOrder = sortingOrder;
        canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord2;
        //
        scaler = content.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 3.44f;
        scaler.referencePixelsPerUnit = 10f;
    }

    #endregion

    #region FlowCoordinator

    /// <summary>
    /// Replaces view controllers for the flow coordinator.
    /// </summary>
    /// <param name="flowCoordinator">A flow coordinator to operate on.</param>
    public static void ReplaceInitialViewControllers(
        this FlowCoordinator flowCoordinator,
        Optional<ViewController> mainViewController = default,
        Optional<ViewController> leftScreenViewController = default,
        Optional<ViewController> rightScreenViewController = default,
        Optional<ViewController> bottomScreenViewController = default,
        Optional<ViewController> topScreenViewController = default
    ) {
        Replace(ref flowCoordinator._providedMainViewController, mainViewController);
        Replace(ref flowCoordinator._providedLeftScreenViewController, leftScreenViewController);
        Replace(ref flowCoordinator._providedRightScreenViewController, rightScreenViewController);
        Replace(ref flowCoordinator._providedBottomScreenViewController, bottomScreenViewController);
        Replace(ref flowCoordinator._providedTopScreenViewController, topScreenViewController);
        return;

        static void Replace(ref ViewController field, Optional<ViewController> view) {
            if (view.HasValue) {
                field = view.Value!;
            }
        }
    }

    /// <summary>
    /// Dismisses the flow coordinator from its parent.
    /// </summary>
    /// <param name="flowCoordinator">A flow coordinator to dismiss.</param>
    public static void DismissSelf(this FlowCoordinator flowCoordinator) {
        flowCoordinator._parentFlowCoordinator.DismissFlowCoordinator(flowCoordinator);
    }

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