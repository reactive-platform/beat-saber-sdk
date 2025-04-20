using JetBrains.Annotations;
using Reactive.Components;
using UnityEngine;

namespace Reactive.BeatSaber.Components {
    [PublicAPI]
    public class TextListControl<TKey> : ListControl<TKey, string, TextKeyedControlCell<TKey>> { }

    [PublicAPI]
    public class TextKeyedControlCell<TKey> : KeyedControlCell<TKey, string>, IPreviewableCell {
        public bool UsedAsPreview {
            set => _button.Interactable = !value;
        }

        public Label Label => _button.Label;

        private LabelButton _button = null!;

        public override void OnInit(TKey key, string param) {
            Label.Text = param;
        }

        public override void OnCellStateChange(bool selected) {
            _button.Click(selected);
        }

        protected override GameObject Construct() {
            var colorSet = BeatSaberStyle.TextColorSet;
            colorSet.NotInteractableColor = colorSet.Color;
            return new LabelButton {
                Latching = true,
                Colors = colorSet,
                OnStateChanged = _ => SelectSelf(),
                Label = {
                    FontSizeMin = 2f,
                    FontSizeMax = 5f,
                    EnableAutoSizing = true
                }
            }.Bind(ref _button).Use();
        }
    }
}