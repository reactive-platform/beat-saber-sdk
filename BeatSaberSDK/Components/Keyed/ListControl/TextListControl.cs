using System;
using JetBrains.Annotations;
using Reactive.Components;

namespace Reactive.BeatSaber.Components {
    [PublicAPI]
    public class TextListControl<TKey> : ListControl<TKey, string, TextKeyedControlCell<TKey>> { }

    [PublicAPI]
    public class TextKeyedControlCell<TKey> : LabelButton, IKeyedControlCell<TKey, string>, IPreviewableCell {
        public TKey Key => _key ?? throw new UninitializedComponentException();

        public bool UsedAsPreview {
            set => Interactable = !value;
        }

        public event Action<TKey>? CellAskedToBeSelectedEvent;

        private TKey? _key;
        private bool _notify;

        public void Init(TKey key, string param) {
            Text = param;
            _key = key;
        }

        public void OnCellStateChange(bool selected) {
            _notify = false;
            Click(selected);
        }

        protected override void OnInitialize() {
            Latching = true;
            FontSizeMin = 2f;
            FontSizeMax = 5f;
            EnableAutoSizing = true;
            
            var colorSet = BeatSaberStyle.TextColorSet;
            colorSet.NotInteractableColor = colorSet.Color;
            Colors = colorSet;
        }

        protected override void OnButtonStateChange() {
            base.OnButtonStateChange();
            if (Active && _notify) {
                CellAskedToBeSelectedEvent?.Invoke(Key);
            }
            _notify = true;
        }
    }
}