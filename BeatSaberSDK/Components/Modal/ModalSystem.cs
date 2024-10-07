using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

#if !COMPILE_EDITOR
using HMUI;
using VRUIControls;
#endif

namespace Reactive.BeatSaber.Components {
    [PublicAPI]
    public sealed class ModalSystem : Reactive.Components.ModalSystem<ModalSystem> {
#if !COMPILE_EDITOR
        protected override void Construct(RectTransform rectTransform) {
            base.Construct(rectTransform);
            var go = rectTransform.gameObject;
            go.AddComponent<CanvasGroup>();
            Blocker.AddComponent<Touchable>();
            ModalCanvas.additionalShaderChannels |=
                AdditionalCanvasShaderChannels.Tangent |
                AdditionalCanvasShaderChannels.TexCoord2;
            //raycaster
            var raycaster = go.AddComponent<VRGraphicRaycaster>();
            BeatSaberUtils.MenuContainer.Inject(raycaster);
            go.AddComponent<GraphicRaycaster>();
#endif
        }
    }
}