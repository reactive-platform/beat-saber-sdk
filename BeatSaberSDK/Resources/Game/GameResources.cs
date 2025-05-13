using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

#nullable disable

namespace Reactive.BeatSaber;

[PublicAPI]
[CreateAssetMenu(fileName = "BeatSaberSDK_GameResources", menuName = "Reactive/BeatSaberSDK/GameResources")]
public class GameResources : ScriptableObject {
    #region Initialization

    static GameResources() {
        Init();
    }

    private static GameResources _instance;

#if COMPILE_EDITOR
    private static void Init() {
        if (_instance != null) return;
        var bundles = AssetDatabase.FindAssets("BeatSaberSDK_GameResources");
        var guid = bundles.First();
        var path = AssetDatabase.GUIDToAssetPath(guid);
        _instance = AssetDatabase.LoadAssetAtPath<GameResources>(path);
    }
#else
    internal static void Init() {
        if (_instance != null) return;
        _instance = CreateInstance<GameResources>();
        
        _instance.buttonClickSignal = Find<Signal>("UIButtonWasPressed");
        
        _instance.arrowIcon = Find<Sprite>("ArrowIcon");
        _instance.editIcon = Find<Sprite>("EditIcon");
        _instance.caretIcon = Find<Sprite>("Caret");
        _instance.verticalIndicatorIcon = Find<Sprite>("VerticalRoundRect8");
        _instance.circleIcon = Find<Sprite>("FullCircle64");
        
        _instance.uiNoGlowMaterial = Find<Material>("UINoGlow");
        _instance.uiFontMaterial = Find<Material>(
            "Teko-Medium SDF Curved Softer",
            x => x.mainTexture.name == "Teko-Medium SDF Atlas"
        );
        _instance.uiFogBackgroundMaterial = Find<Material>("UIFogBG");
        
        _instance.animatedButtonMaterial = Find<Material>("AnimatedButton");
        _instance.animatedButtonBorderMaterial = Find<Material>("AnimatedButtonBorder");
    }

    private static T Find<T>(string name, Func<T, bool> func = null) where T : Object {
        return Resources.FindObjectsOfTypeAll<T>().First(x => x.name == name && (func?.Invoke(x) ?? true));
    }
#endif

    #endregion

    #region Serialized
    
    public Signal buttonClickSignal;

    public Material uiNoGlowMaterial;
    public Material uiFontMaterial;
    public Material uiFogBackgroundMaterial;

    public Sprite arrowIcon;
    public Sprite editIcon;
    public Sprite caretIcon;
    public Sprite circleIcon;
    public Sprite verticalIndicatorIcon;

    internal Material animatedButtonMaterial;
    internal Material animatedButtonBorderMaterial;

    #endregion

    #region Internal

    internal static Material AnimatedButtonMaterial => _instance.animatedButtonMaterial;
    
    internal static Material AnimatedButtonBorderMaterial => _instance.animatedButtonBorderMaterial;

    #endregion
    
    #region Static

    public static Signal ButtonClickSignal => _instance.buttonClickSignal;
    
    public static Material UIFogBackgroundMaterial => _instance.uiFogBackgroundMaterial;

    public static Material UINoGlowMaterial => _instance.uiNoGlowMaterial;

    public static Material UIFontMaterial => _instance.uiFontMaterial;

    public static Sprite ArrowIcon => _instance.arrowIcon;
    
    public static Sprite EditIcon => _instance.editIcon;
    
    public static Sprite CaretIcon => _instance.caretIcon;
    
    public static Sprite CircleIcon => _instance.circleIcon;

    public static Sprite VerticalIndicatorIcon => _instance.verticalIndicatorIcon;

    #endregion
}