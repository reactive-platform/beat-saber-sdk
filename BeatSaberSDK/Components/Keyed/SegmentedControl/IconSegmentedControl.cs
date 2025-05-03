using JetBrains.Annotations;
using Reactive.Components;
using UnityEngine;

namespace Reactive.BeatSaber.Components {
    [PublicAPI]
    public class IconSegmentedControl<TKey> : SegmentedControl<TKey, Sprite, IconKeyedControlCell<TKey>> { }

    [PublicAPI]
    public class IconKeyedControlCell<TKey> : KeyedControlCell<TKey, Sprite> {
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
                Colors = new SimpleColorSet {
                    ActiveColor = BeatSaberStyle.PrimaryButtonColor,
                    HoveredColor = BeatSaberStyle.PrimaryButtonColor,
                    Color = (Color.white * 0.8f).ColorWithAlpha(0.2f)
                },
                OnStateChanged = _ => SelectSelf()
            }.Bind(ref _button).Use();
        }
    }
}