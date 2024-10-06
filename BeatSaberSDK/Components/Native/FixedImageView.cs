using HarmonyLib;
using JetBrains.Annotations;

namespace Reactive.BeatSaber.Components {
#if !COMPILE_EDITOR
    [HarmonyPatch(typeof(UnityEngine.UI.Image), "get_pixelsPerUnit")]
#endif
    internal class FixedImageView : HMUI.ImageView {
        private static Harmony? _harmony;
        
        //TODO: move to plugin
        static FixedImageView() {
            _harmony = new("Reactive.BeatSaber.FixedImageView");
            _harmony.PatchAll();
        }
        
        public GradientDirection GradientDirection {
#if !COMPILE_EDITOR
            get => _gradientDirection;
            set => _gradientDirection = value;
#else
            get => GradientDirection.Vertical;
            set { }
#endif
        }

        public float Skew {
            get => _skew;
            set {
                _skew = value;
                __Refresh();
            }
        }

        [UsedImplicitly]
        private static void Postfix(UnityEngine.UI.Image __instance, ref float __result) {
            if (__instance is not FixedImageView) return;
            __result *= __instance.pixelsPerUnitMultiplier;
        }
    }
}