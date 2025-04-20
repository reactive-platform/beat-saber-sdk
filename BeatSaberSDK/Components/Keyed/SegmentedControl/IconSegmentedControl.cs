using JetBrains.Annotations;
using Reactive.Components;
using UnityEngine;

namespace Reactive.BeatSaber.Components {
    [PublicAPI]
    public class IconSegmentedControl<TKey> : SegmentedControl<TKey, Sprite, IconKeyedControlComponentCell<TKey>> { }

    [PublicAPI]
    public class IconKeyedControlComponentCell<TKey> : KeyedControlComponentCell<TKey, Sprite> {
        public ImageButton Button => _button;

        private ImageButton _button = null!;

        public override void OnInit(TKey key, Sprite param) {
            Button.Image.Sprite = param;
        }

        public override void OnCellStateChange(bool selected) {
            Button.Click(selected);
        }

        protected override GameObject Construct() {
            return new ImageButton {
                Image = {
                    PreserveAspect = true,
                    Material = BeatSaberResources.Materials.uiAdditiveGlowMaterial
                },
                Latching = true,
                Colors = BeatSaberStyle.ButtonColorSet,
                OnStateChanged = _ => SelectSelf()
            }.Bind(ref _button).Use();
        }
    }
}